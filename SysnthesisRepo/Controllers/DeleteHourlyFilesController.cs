using EntityModels.Models;
using NLog;
using Repository;
using Repository.IRepository;
using Syncfusion.EJ2.Base;
using SynthesisViewModal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Utility;

namespace SysnthesisRepo.Controllers
{
    public class DeleteHourlyFilesController : Controller
    {
        private readonly IDeleteHourlyFilesRepository _deleteHourlyFilesRepository;
        private readonly IUserActivityLogRepository _activityLogRepository;
        private readonly ICommonRepository _CommonRepository;
        private readonly ISynthesisApiRepository _synthesisApiRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        DeleteHourlyFilesViewModal deleteHourlyFilesViewModal = new DeleteHourlyFilesViewModal();
        public DeleteHourlyFilesController()
        {
            this._deleteHourlyFilesRepository = new DeleteHourlyFilesRepository(new DBContext());
            this._CommonRepository = new CommonRepository(new DBContext());
            this._activityLogRepository = new UserActivityLogRepository(new DBContext());
            this._synthesisApiRepository = new SynthesisApiRepository();
        }
        // GET: DeleteHourlyFiles
        /// <summary>
        /// This method return view of Hourlt POS Feeds.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            try
            {
                ViewBag.Title = "Hourly POS feeds - Synthesis";
                _CommonRepository.LogEntries();     //Harsh's code
                deleteHourlyFilesViewModal.storeid = 0;
                var StoreIdSsn = Session["storeid"];
                if (StoreIdSsn != null)
                {
                    if (!string.IsNullOrEmpty(Session["storeid"].ToString()))
                    {
                        string store_idval = Session["storeid"].ToString();
                        ViewBag.Storeidvalue = store_idval;
                        if (store_idval != "0")
                        {
                            deleteHourlyFilesViewModal.storeid = Convert.ToInt32(store_idval);
                            //Using this db class get Name store master data.
                            var storename = (_deleteHourlyFilesRepository.GetNameStoreMaster().Where(x => x.StoreId == deleteHourlyFilesViewModal.storeid).Select(x => x.NickName)).FirstOrDefault();
                            ViewBag.StoreNamevalue = storename;

                        }
                        else
                        {
                            ViewBag.StoreNamevalue = "All Stores";
                        }
                        ViewBag.Startdate = DateTime.Now.Date.AddDays(-1).ToString("MM-dd-yyyy");
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error("DeleteHourlyFilesController - Index - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            
            return View();
        }
        /// <summary>
        /// This grid data get delete hourly Files List.
        /// </summary>
        /// <param name="startdate"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Grid(string startdate = "")
        {
            var date = startdate.Replace("_", " ");
            deleteHourlyFilesViewModal.start_date = null;
            try
            {
                deleteHourlyFilesViewModal.start_date = Convert.ToDateTime(date);
            }
            catch { }

            string storeid = "";
            if (!string.IsNullOrEmpty(Convert.ToString(Session["storeid"])))
            {
                List<DeleteHourlyFilesModel> List = new List<DeleteHourlyFilesModel>();
                try
                {
                    storeid = Convert.ToString(Session["storeid"]);
                    if (storeid != "")
                    {
                        deleteHourlyFilesViewModal.istoreID = Convert.ToInt32(storeid);
                    }
                    //Using this class delete Hourly Files List.
                    List = _deleteHourlyFilesRepository.DeleteHourlyFList(deleteHourlyFilesViewModal);
                }
                catch (Exception ex)
                {
                    logger.Error("DeleteHourlyFilesController - Grid - " + DateTime.Now + " - " + ex.Message.ToString());
                }
                return View(List);
            }
            else
            {
                return RedirectToAction("index", "Login");
            }

        }


        /// <summary>
        /// This method is delete Hourly files
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,DeleteInvoice")]
        public async Task<ActionResult> Delete(int? id)
        {
            try
            {
                //Get Sales Summary By Id.
                SalesActivitySummaryHourly sales = await _deleteHourlyFilesRepository.GetSalesSummaryId(id);
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                ActivityLog ActLog = new ActivityLog();
                ActLog.Action = 3;
                //This Db class is used for get user firstname.
                ActLog.Comment = "Hourly File Deleted by " + _CommonRepository.getUserFirstName(UserName) + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                //This  class is used for Activity Log Insert
                _activityLogRepository.ActivityLogInsert(ActLog);

                deleteHourlyFilesViewModal.StatusMessage = "Delete";
                deleteHourlyFilesViewModal.DeleteMessage = sales.FileName + " deleted successfully.";
                deleteHourlyFilesViewModal.StatusMessageString = "Hourly File Deleted Successfully..";
                ViewBag.StatusMessageString = deleteHourlyFilesViewModal.StatusMessageString;
                return Json(deleteHourlyFilesViewModal.StatusMessageString, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("DeleteHourlyFilesController - Delete - " + DateTime.Now + " - " + ex.Message.ToString());
            
                string Msg = "";
                Msg = "Hourly File Already Used in Error Log";
                return Json(Msg, JsonRequestBehavior.AllowGet);
            }

        }

        //Syncfusion grid DeleteHourlyFiles
        public ActionResult DeleteHourlyFileIndex()
        {
            try
            {
                ViewBag.Title = "Hourly POS feeds - Synthesis";
                _CommonRepository.LogEntries();     //Harsh's code
                deleteHourlyFilesViewModal.storeid = 0;
                var StoreIdSsn = Session["storeid"];
                if (StoreIdSsn != null)
                {
                    if (!string.IsNullOrEmpty(Session["storeid"].ToString()))
                    {
                        string store_idval = Session["storeid"].ToString();
                        ViewBag.Storeidvalue = store_idval;
                        if (store_idval != "0")
                        {
                            deleteHourlyFilesViewModal.storeid = Convert.ToInt32(store_idval);
                            //Using this db class get Name store master data.
                            var storename = (_deleteHourlyFilesRepository.GetNameStoreMaster().Where(x => x.StoreId == deleteHourlyFilesViewModal.storeid).Select(x => x.NickName)).FirstOrDefault();
                            ViewBag.StoreNamevalue = storename;

                        }
                        else
                        {
                            ViewBag.StoreNamevalue = "All Stores";
                        }
                        ViewBag.Startdate = DateTime.Now.Date.AddDays(-1).ToString("MM-dd-yyyy");
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error("DeleteHourlyFilesController - DeleteHourlyFileIndex - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View();
        }

        public ActionResult UrlDatasource(DataManagerRequest dm, string startdate = "")
        {
            List<DeleteHourlyFilesModel> HrDeVm = new List<DeleteHourlyFilesModel>();
            IEnumerable<DeleteHourlyFilesModel> DataSource = new List<DeleteHourlyFilesModel>();
            int Count = 0;
            try
            {
                logger.Info("JournalEntriesController - UrlDatasource - " + DateTime.Now);

                var date = startdate.Replace("_", " ");
                if(date == "")
                {
                    date = DateTime.Today.AddDays(-1).ToString();
                }
                deleteHourlyFilesViewModal.start_date = null;
                deleteHourlyFilesViewModal.start_date = Convert.ToDateTime(date);
                string storeid = "";

                if (!string.IsNullOrEmpty(Convert.ToString(Session["storeid"])))
                {
                    storeid = Convert.ToString(Session["storeid"]);
                    if (storeid != "")
                    {
                        deleteHourlyFilesViewModal.istoreID = Convert.ToInt32(storeid);
                        HrDeVm = _deleteHourlyFilesRepository.DeleteHourlyFList(deleteHourlyFilesViewModal);
                    }
                }
                DataSource = HrDeVm;

                DataOperations operation = new DataOperations();
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    string search = dm.Search[0].Key.ToString().Trim();
                    DataSource = DataSource.Where(x => x.FileName.Contains(search) || x.HSequence.ToString().Contains(search)).ToList();
                }
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                }
                Count = DataSource.Cast<DeleteHourlyFilesModel>().Count();
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
                logger.Error("DeleteHourlyFilesController - UrlDatasource - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return dm.RequiresCounts ? Json(new { result = DataSource, count = Count }) : Json(DataSource);
        }
    }
}