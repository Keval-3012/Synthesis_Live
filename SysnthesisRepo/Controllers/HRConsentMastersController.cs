using EntityModels.Models;
using EntityModels.HRModels;
using NLog;
using Repository;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Syncfusion.EJ2.Base;
using SynthesisViewModal;
using SynthesisViewModal.HRViewModel;
using System.Collections;
using System.Threading.Tasks;
using System.Web.Razor.Tokenizer;
using System.Web.Security;
using System.IO;

namespace SysnthesisRepo.Controllers
{
    public class HRConsentMastersController : Controller
    {
        private readonly ICommonRepository _CommonRepository;
        private readonly IHRConsentMastersRepository _HRConsentMastersRepository;
        private readonly ICompaniesRepository _CompaniesRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        string SuccessMessage = string.Empty;
        string ErrorMessage = string.Empty;
        string UserName = System.Web.HttpContext.Current.User.Identity.Name;

        public HRConsentMastersController()
        {
            this._HRConsentMastersRepository = new HRConsentMastersRepository(new DBContext());
            this._CommonRepository = new CommonRepository(new DBContext());
            this._CompaniesRepository = new CompaniesRepository(new DBContext());
        }

        // GET: HRConsentMasters
        public ActionResult Index()
        {
            try
            {
                logger.Info("HRConsentMasters - Index - " + DateTime.Now);
                ViewBag.ToasterMessage = HttpContext.Session["ToasterMessage"] as string;
                HttpContext.Session.Remove("ToasterMessage");
            }
            catch (Exception ex)
            {
                logger.Error("HRConsentMasters - Index - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            
            return View();
        }

        public ActionResult Create()
        {
            try
            {
                logger.Info("HRConsentMasters - Create - " + DateTime.Now);
                SetDropdown();
            }
            catch (Exception ex)
            {

                logger.Error("HRConsentMasters - Create - " + DateTime.Now + " - " + ex.Message.ToString());
            }


            return View();
        }

        public void SetDropdown()
        {
            try
            {
                logger.Info("HRConsentMasters - SetDropdown - " + DateTime.Now);
                List<HRFormTypeMaster> Formlist = new List<HRFormTypeMaster>();
                Formlist = _HRConsentMastersRepository.GetFormList();

                ViewBag.FormTypeId = new SelectList(new[] { new HRFormTypeMaster { FormTypeId = 0, FormTypeName = "-- Select Form Type --" } }
                                     .Concat(Formlist.ToList()), "FormTypeId", "FormTypeName");

                ViewBag.LanguageId = new SelectList(new[] { new EnumDropDown { value = 0, text = "-- Select Language --" } }
                                     .Concat(from Language e in Enum.GetValues(typeof(Language)) select new EnumDropDown { value = (int)e, text = e.ToString() }), "value", "text");

                ViewBag.TypeId = new SelectList(new[] { new EnumDropDown { value = 0, text = "-- Select Type --" } }
                                     .Concat(from InputTypes e in Enum.GetValues(typeof(InputTypes)) select new EnumDropDown { value = (int)e, text = e.ToString() }), "value", "text");

            }
            catch (Exception ex)
            {

                logger.Error("HRConsentMasters - SetDropdown - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        [HttpPost]
        public ActionResult Create(ConsentModel objConsent)
        {
            try
            {
                logger.Info("HRConsentMasters - Create - " + DateTime.Now);
                HRConsentMaster obj = new HRConsentMaster();
                obj.FormTypeId = Convert.ToInt32(objConsent.FormTypeId);
                obj.CreatedOn = DateTime.Now;
                obj.CreatedBy = _CommonRepository.getUserId(UserName);
                _HRConsentMastersRepository.SaveConsent(objConsent, obj);
                HttpContext.Session["ToasterMessage"] = "Consent Added Successfully.";
                return Json("success", JsonRequestBehavior.AllowGet);
                
            }
            catch (Exception ex)
            {
                logger.Error("HRConsentMasters - Create - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View();
        }


        public ActionResult Edit(int? ConsentId)
        {
            try
            {
                logger.Info("HRConsentMasters - Edit - " + DateTime.Now);
                SetDropdown();
                var item = _HRConsentMastersRepository.consentList().Where(a => a.ConsentId == ConsentId).FirstOrDefault();
                return View(item);
            }
            catch (Exception ex)
            {
                logger.Error("HRConsentMasters - Edit - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View();
        }

        [HttpPost]
        public ActionResult Edit(ConsentModel objConsent)
        {
            try
            {
                logger.Info("HRConsentMasters - Edit - " + DateTime.Now);
                var obj = _HRConsentMastersRepository.GetConsentByID(objConsent);
                if (obj != null)
                {
                    obj.FormTypeId = Convert.ToInt32(objConsent.FormTypeId);
                    obj.ModifiedOn = DateTime.Now;
                    obj.ModifiedBy = _CommonRepository.getUserId(UserName);
                    _HRConsentMastersRepository.EditConsentDetails(obj, objConsent);
                }
                HttpContext.Session["ToasterMessage"] = "Consent Updated Successfully.";
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                logger.Error("HRConsentMasters - Edit - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View();
        }
        public ActionResult GetConsentDetail(int ConsentId)
        {
            try
            {
                logger.Info("HRConsentMasters - GetConsentDetail - " + DateTime.Now);
                var data = _HRConsentMastersRepository.GetConsentDetail(ConsentId).OrderBy(s => s.DetailId);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("HRConsentMasters - GetConsentDetail - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View();
        }
        public ActionResult UrlDatasource(DataManagerRequest dm)
        {
            List<HRConsentViewModel> data = new List<HRConsentViewModel>();
            IEnumerable DataSource = new List<HRConsentViewModel>();
            int Count = 0;
            try
            {
                logger.Info("HRConsentMasters - UrlDatasource - " + DateTime.Now);
                data = _HRConsentMastersRepository.GetConsentList();
                DataSource = data;
                
                DataOperations operation = new DataOperations();
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    string search = dm.Search[0].Key.ToString().Trim();
                    DataSource = data.ToList().Where(x => x.FormType.Contains(search)).ToList();
                    //DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
                }
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                }
                Count = DataSource.Cast<HRConsentViewModel>().Count();
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
                logger.Error("HRConsentMasters - UrlDatasource - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return dm.RequiresCounts ? Json(new { result = DataSource, count = Count }) : Json(DataSource);
        }
        public ActionResult RemoveConsent(int ConsentId)
        {
            HRConsentMaster HR = new HRConsentMaster();
            SuccessMessage = null;
            ErrorMessage = null;
            try
            {
                logger.Info("HRConsentMasters - RemoveConsent - " + DateTime.Now);
                _HRConsentMastersRepository.DeleteConsentDetail(ConsentId);
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("HRConsentMastersController - RemoveConsent - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View();
        }

        public ActionResult EditConsent(int ConsentId)
        {
            try
            {
                logger.Info("HRConsentMasters - EditConsent - " + DateTime.Now);
                return RedirectToAction("Edit", new { ConsentId = ConsentId });
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("HRConsentMastersController - EditConsent - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View();
        }

        //Himanshu 09-5-2024
        [Authorize(Roles = "Administrator,ViewConsentStatusHRModule")]
        public ActionResult ConsentStatusList()
        {
            ViewBag.Title = "Consent Status - Synthesis";

            ConsentStatusListModel obj = new ConsentStatusListModel();
            try
            {
                logger.Info("HRConsentMasters - ConsentStatusList - " + DateTime.Now);

                
                List<StoreMaster> list = new List<StoreMaster>();
                if (Roles.IsUserInRole("Administrator") || Roles.IsUserInRole("ViewConsentStatusHRModule"))
                {
                    if(Roles.IsUserInRole("Administrator"))
                    {
                        list = _CompaniesRepository.GetAllStoreMasters().Where(a => a.IsActive == true).OrderBy(a => a.StoreId).ToList();
                    }
                    else
                    {
                        int userid = Convert.ToInt32(Session["UserId"]);
                        list = _HRConsentMastersRepository.GetRolewiseStore(userid);
                    }
                }
                else
                {
                    int iUserID = _CommonRepository.getUserId(UserName);
                    list = _CompaniesRepository.GetAllStoreMasters().Where(a => a.StoreManageId == iUserID && a.IsActive == true).OrderBy(a => a.StoreId).ToList();
                }

                ViewBag.StoreId = new SelectList(new[] { new StoreMaster { StoreId = 0, Name = "-- Select Store --" } }
                       .Concat(list.ToList()), "StoreId", "Name");

                ViewBag.SignCheck = new SelectList((from Sign e in Enum.GetValues(typeof(Sign)) select new EnumDropDown { value = (int)e, text = e.ToString() }), "value", "text",1);

                obj.FromDate = DateTime.Now.ToString("MM/dd/yyyy");
                obj.ToDate = DateTime.Now.ToString("MM/dd/yyyy");
            }
            catch (Exception ex)
            {
                logger.Error("HRConsentMasters - ConsentStatusList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View(obj);
        }
        public ActionResult PartialIndexConsentlist()
        {
            List<ConsentStatusListModel> list = new List<ConsentStatusListModel>();
            return PartialView("_EmployeeChildConsentList",list);
        }
        public ActionResult UrlDatasource1(DataManagerRequest dm,string fromdate, string todate,int? signcheck,int? storeid)
        {
            List<ConsentStatusListModel> data = new List<ConsentStatusListModel>();
            IEnumerable DataSource = new List<ConsentStatusListModel>();
            int Count = 0;
            try
            {
                logger.Info("HRConsentMasters - UrlDatasource1 - " + DateTime.Now);
                data = _HRConsentMastersRepository.GetConsentStatusList(fromdate,todate,signcheck,storeid, Roles.IsUserInRole("Administrator"));
                DataSource = data;
                
                DataOperations operation = new DataOperations();
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    string search = dm.Search[0].Key.ToString().Trim();
                    DataSource = data.ToList().Where(x => x.EmployeeName.Contains(search) || x.DepartmentName.Contains(search) || x.CreatedDate.Contains(search)).ToList();
                }
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                }
                Count = DataSource.Cast<ConsentStatusListModel>().Count();
                if (dm.Skip != 0)
                {
                    DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
                }
                if (dm.Take != 0)
                {
                    DataSource = operation.PerformTake(DataSource, dm.Take);
                }
                
                logger.Info("HRConsentMasters - ConsentCountGet - " + Count);
            }
            catch (Exception ex)
            {
                logger.Error("HRConsentMasters - UrlDatasource1 - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return dm.RequiresCounts ? Json(new { result = DataSource, count = Count }) : Json(DataSource);
        }
        public ActionResult ConsentFileDownload(string filename,int employeeid)
        {
            try
            {
                logger.Info("HRConsentMasters - ConsentFileDownload - " + DateTime.Now);
                if (_CommonRepository.getUserId(UserName) != 0)
                {
                    string NewFilePath = Server.MapPath("~/UserFiles/HR_File/EmployeeDocument/"+ employeeid + "/");
                    if (Directory.Exists(Server.MapPath("~/UserFiles/HR_File/EmployeeDocument/"+ employeeid + "/")))
                    {
                        string FilePath = NewFilePath + filename.Trim();
                        if (System.IO.File.Exists(FilePath))
                        {
                            return Json("Success", JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json("Error", JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        ViewBag.Message = "File Not Found..";
                        return Json("Error", JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("HRConsentMasters - ConsentFileDownload - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConsentFileDownloadSuccess(string filename, int employeeid)
        {
            try
            {
                logger.Info("HRConsentMasters - ConsentFileDownloadSuccess - " + DateTime.Now);
                string filePath = Server.MapPath("~/UserFiles/HR_File/EmployeeDocument/" + employeeid + "/" + filename);

                if (System.IO.File.Exists(filePath))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                    return File(fileBytes, "application/pdf", filename);
                }
                else
                {
                    // File not found, return error
                    return Json("Error", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error("HRConsentMasters - ConsentFileDownloadSuccess - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
        }
    }
}