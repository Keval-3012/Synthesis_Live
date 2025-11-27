using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Tokenizer;
using System.Web.Security;
using System.Web.Services.Description;
using System.Web.UI;
using Aspose.Pdf.Operators;
using EntityModels.HRModels;
using EntityModels.Models;
using Microsoft.Extensions.Logging;
using NLog;
using Repository;
using Repository.IRepository;
using Syncfusion.EJ2.Base;
using SynthesisCF.Migrations;
using SynthesisViewModal;
using SynthesisViewModal.HRViewModel;
using Utility;

namespace SysnthesisRepo.Controllers
{
    public class HRTrainingController : Controller
    {
        private readonly IHRTrainingRepository _hRTrainingRepository;
        private readonly ICommonRepository _CommonRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        string SuccessMessage = string.Empty;
        string ErrorMessage = string.Empty;
        string UserName = System.Web.HttpContext.Current.User.Identity.Name;
        public HRTrainingController()
        {
            this._hRTrainingRepository = new HRTrainingRepository(new DBContext());
            this._CommonRepository = new CommonRepository(new DBContext());
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult UrlDatasource(DataManagerRequest dm)
        {
            List<HRTrainingViewModal> HrDeVm = new List<HRTrainingViewModal>();
            IEnumerable DataSource = new List<HRTrainingViewModal>();
            int Count = 0;
            try
            {
                logger.Info("HRTrainingController - UrlDatasource - " + DateTime.Now);
                int store_idval = 0;
                if (Session["StoreId"] != null)
                {
                    if (!string.IsNullOrEmpty(Session["StoreId"].ToString()))
                    {

                        store_idval = Convert.ToInt32(Session["StoreId"]);
                    }
                }
                //Using this db class Get Web cam details by storeid.
                HrDeVm = _hRTrainingRepository.GetTrainingList(store_idval);
                DataSource = HrDeVm;
                
                DataOperations operation = new DataOperations();
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    //string search = dm.Search[0].Key.ToString().Trim();
                    //DataSource = HrDeVm.ToList().Where(x => x.EmployeeUserName.Contains(search) || x.StoreNikeName.Contains(search) || x.DOB.Contains(search) || x.HireDate.Contains(search) || x.SSN.Contains(search)).ToList();
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
                Count = DataSource.Cast<HRTrainingViewModal>().Count();
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
                logger.Error("HRTrainingController - UrlDatasource - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return dm.RequiresCounts ? Json(new { result = DataSource, count = Count }) : Json(DataSource);
        }
        [HttpPost]
        public ActionResult ResetTraining(List<int> selectedIds)
        {
            try
            {
                logger.Info("HRTrainingController - ResetTraining - " + DateTime.Now);
                if (_CommonRepository.getUserId(UserName) != 0)
                {
                    int UserId = _CommonRepository.getUserId(UserName);
                    _hRTrainingRepository.ResetTrainingCheck(selectedIds,UserId);
                    return Json("deleted", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return RedirectToAction("Login", "Login");
                }
            }
            catch (Exception ex)
            {
                logger.Error("HRTrainingController - ResetTraining - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json("",JsonRequestBehavior.AllowGet);
        }
        public ActionResult CerificateDownload(string filename)
        {
            try
            {
                logger.Info("HRTrainingController - CerificateDownload - " + DateTime.Now);
                if (_CommonRepository.getUserId(UserName) != 0)
                {
                    string NewFilePath = Server.MapPath("~/UserFiles/HR_File/Certificates/");
                    if (Directory.Exists(Server.MapPath("~/UserFiles/HR_File/Certificates/")))
                    {
                        string FilePath = NewFilePath + filename.Trim();
                        if (System.IO.File.Exists(FilePath))
                        {
                            byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename.Trim());
                        }
                        else
                        {
                            return Redirect(Request.Headers["Referer"].ToString());
                        }
                    }
                    else
                    {
                        ViewBag.Message = "File Not Found..";
                        return Redirect(Request.Headers["Referer"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("HRTrainingController - CerificateDownload - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult KeepSessionAlive()
        {
            return new JsonResult
            {
                Data = "Beat Generated"
            };
        }
    }
}
