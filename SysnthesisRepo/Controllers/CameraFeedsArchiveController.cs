//using Amazon;
//using Amazon.S3;
//using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Syncfusion.EJ2.Base;
using System.Collections;
using EntityModels.Models;
using SynthesisQBOnline;
using NLog;
using Repository.IRepository;
using SynthesisViewModal;
using Repository;

namespace SynthesisRepo.Controllers
{
    public class CameraFeedsArchiveController : Controller
    {
        private readonly ICameraFeedsArchiveRepository _listWebcamHistoryRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        private readonly ICommonRepository _commonRepository;
        private readonly IUserActivityLogRepository _activityLogRepository;
        ListWebcamHistoryViewModal listWebcamHistoryViewModal = new ListWebcamHistoryViewModal();
        public CameraFeedsArchiveController()
        {
            this._listWebcamHistoryRepository = new CameraFeedsArchiveRepository(new DBContext());
            this._commonRepository = new CommonRepository(new DBContext());
            this._activityLogRepository = new UserActivityLogRepository(new DBContext());
        }
        /// <summary>
        /// This method is url Data Source.
        /// </summary>
        /// <param name="dm"></param>
        /// <returns></returns>
        public ActionResult UrlDatasource(DataManagerRequest dm)
        {
            listWebcamHistoryViewModal.StoreIds = 0;
            if (Session["storeid"] != null)
            {
                listWebcamHistoryViewModal.StoreIds = Convert.ToInt32(Convert.ToString(Session["storeid"]));
            }
            ViewBag.StoreId = listWebcamHistoryViewModal.StoreIds;
            List<CameraList> itemMovement = new List<CameraList>();
            IEnumerable DataSource = new List<CameraList>();
            try
            {
                //Using this db class Get Web cam details by storeid.
                itemMovement = _listWebcamHistoryRepository.GetWebcamdetails((int)listWebcamHistoryViewModal.StoreIds);
                DataSource = itemMovement;
                listWebcamHistoryViewModal.count = DataSource.Cast<CameraList>().Count();
                DataOperations operation = new DataOperations();
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
                }
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                }

                if (dm.Skip != 0)
                {
                    DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
                }
                if (dm.Take != 0)
                {
                    DataSource = operation.PerformTake(DataSource, dm.Take);
                }

            }
            catch (Exception ex)
            {
                logger.Error("CameraFeedsArchiveController - UrlDatasource - " + DateTime.Now + " - " + ex.Message.ToString());
            }          
            return dm.RequiresCounts ? Json(new { result = DataSource, count = listWebcamHistoryViewModal.count }) : Json(DataSource);
        }

        /// <summary>
        /// This method is url Data Source for child.
        /// </summary>
        /// <param name="dm"></param>
        /// <returns></returns>
        public ActionResult UrlDatasourceChild(DataManagerRequest dm)
        {
            listWebcamHistoryViewModal.StoreIds = 0;
            listWebcamHistoryViewModal.RecordingDate = "";

            if (Session["storeid"] != null)
            {
                listWebcamHistoryViewModal.StoreIds = Convert.ToInt32(Convert.ToString(Session["storeid"]));
            }
            if (!String.IsNullOrEmpty(Session["Date"].ToString()))
            {
                listWebcamHistoryViewModal.RecordingDate = Convert.ToDateTime(Session["Date"]).ToString("dd-MMM-yyyy");
            }

            List<ListWebcamRecordingHistory> itemMovement = new List<ListWebcamRecordingHistory>();
            IEnumerable DataSource = new List<ListWebcamRecordingHistory>();
            try
            {
                //Filtering
                if (dm.Where != null && dm.Where.Count > 0)
                {
                    //Get Web cam recording History List.
                    itemMovement = _listWebcamHistoryRepository.GetWebcamRecordingHistoryListID(dm.Where[0].value, (int)listWebcamHistoryViewModal.StoreIds, listWebcamHistoryViewModal.RecordingDate);
                }
                DataSource = itemMovement;
                listWebcamHistoryViewModal.count = DataSource.Cast<ListWebcamRecordingHistory>().Count();
                DataOperations operation = new DataOperations();
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
                }
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                }
                if (dm.Skip != 0)
                {
                    DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
                }
                if (dm.Take != 0)
                {
                    DataSource = operation.PerformTake(DataSource, dm.Take);
                }
            }
            catch (Exception ex)
            {
                logger.Error("CameraFeedsArchiveController - UrlDatasourceChild - " + DateTime.Now + " - " + ex.Message.ToString());
            }      
            return dm.RequiresCounts ? Json(new { result = DataSource, count = listWebcamHistoryViewModal.count }) : Json(DataSource);
        }
        /// <summary>
        /// This method is PartialView of link details.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetList()
        {
            return PartialView("LinkedDetails");
        }
        // GET: ListWebcamHistory
        /// <summary>
        /// This method get List of web cam history.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.Title = "Camera Feeds Archive - Synthesis";
            listWebcamHistoryViewModal.StoreIds = 0;
            try
            {
                if (Session["storeid"] != null)
                {
                    listWebcamHistoryViewModal.StoreIds = Convert.ToInt32(Convert.ToString(Session["storeid"]));
                }
                List<ListWebcamRecordingHistory> lstWebcamRecordingHistorys = new List<ListWebcamRecordingHistory>();
                //Get Web cam recording History List.
                lstWebcamRecordingHistorys = _listWebcamHistoryRepository.GetWebcamRecordingHistoryListID1(0, listWebcamHistoryViewModal.StoreIds);
                listWebcamHistoryViewModal.maxdate = lstWebcamRecordingHistorys.Select(x => x.Date).Max().ToString();
                DateTime date = new DateTime();
                if (listWebcamHistoryViewModal.maxdate != null && listWebcamHistoryViewModal.maxdate != "")
                {
                    date = Convert.ToDateTime(listWebcamHistoryViewModal.maxdate);
                }
                else
                {
                    date = DateTime.Now;
                }
                ViewBag.Date = date;
                Session["Date"] = date;
            }
            catch (Exception ex)
            {
                logger.Error("CameraFeedsArchiveController - Index - " + DateTime.Now + " - " + ex.Message.ToString());
            }         
            return View();
        }

        /// <summary>
        /// This method changes data.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangeData(String date)
        {
            //Process using recieved date 
            Session["Date"] = DateTime.Parse(date);
            return Json("");
        }
        /// <summary>
        /// This method delete All files.
        /// </summary>
        public void RemoveAllfiles()
        {
            try
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/Videos"));

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            catch (Exception ex)
            {
                logger.Error("CameraFeedsArchiveController - RemoveAllfiles - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
    }
}