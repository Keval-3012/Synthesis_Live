using Aspose.Pdf.Operators;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Word;
using DocumentFormat.OpenXml.Vml;
using DocumentFormat.OpenXml.Wordprocessing;
using EntityModels.HRModels;
using EntityModels.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using Repository;
using Repository.IRepository;
using Syncfusion.EJ2.Base;
using Syncfusion.EJ2.Navigations;
using Syncfusion.EJ2.Popups;
using SynthesisViewModal.HRViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Tokenizer;
using System.Web.Security;
using System.Xml;
using Utility;

namespace SysnthesisRepo.Controllers
{
    public class HREmployeeProfileController : Controller
    {
        private readonly IHREmployeeRepository _hREmployeeRepository;
        private readonly ICommonRepository _CommonRepository;
        private readonly ICompaniesRepository _CompaniesRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        string SuccessMessage = string.Empty;
        string ErrorMessage = string.Empty;
        string UserName = System.Web.HttpContext.Current.User.Identity.Name;
        public HREmployeeProfileController()
        {
            this._hREmployeeRepository = new HREmployeeRepository(new DBContext());
            this._CommonRepository = new CommonRepository(new DBContext());
            this._CompaniesRepository = new CompaniesRepository(new DBContext());
        }
        // GET: HREmployeeProfile
        //public ActionResult Index(int EmployeeId)
        //{
        //    HREmployeeProfileViewModal vm = new HREmployeeProfileViewModal();
        //    try
        //    {
        //        logger.Info("HREmployeeProfileController - Index - " + DateTime.Now);
        //        HREmployeeMaster hR = _hREmployeeRepository.GetHREmployeeMaster(EmployeeId);
        //        if (hR != null)
        //        {
        //            vm.EmployeeId = hR.EmployeeId;
        //            vm.DateofBirth = hR.DateofBirth;
        //            vm.FirstName = hR.FirstName;
        //            vm.LastName = hR.LastName;
        //            vm.Email = hR.Email;
        //            vm.Phone = hR.Phone;
        //            vm.Street = hR.Street;
        //            vm.City = hR.City;
        //            vm.FullSSN = hR.FullSSN;
        //            vm.Gender = hR.Gender;
        //            vm.MaritalStatus = hR.MaritalStatus;
        //            vm.LanguageId = hR.LanguageId;
        //            vm.MobileNo = hR.MobileNo;
        //            vm.Designation = hR.Designation;
        //        }
        //        List<StoreDetail> sr = _hREmployeeRepository.GetEmployeeUsingStore(EmployeeId);
        //        vm.EmployeeChildId = _hREmployeeRepository.GetHREmployeeChildByEmployeeIdandStoreId(EmployeeId, sr.FirstOrDefault().StoreId).EmployeeChildId;
        //        ViewBag.StoreId = new SelectList(sr, "StoreId", "Name", sr.FirstOrDefault().StoreId);


        //        List<DialogDialogButton> btn = new List<DialogDialogButton>() { };
        //        btn.Add(new DialogDialogButton() { Click = "GetHourlyRateDetail", ButtonModel = new promptButtonModel() { content = "Validate", isPrimary = true } });
        //        btn.Add(new DialogDialogButton() { Click = "promptBtnClick", ButtonModel = new promptButtonModel() { content = "Cancel" } });

        //        ViewBag.PromptButton = btn;

        //        List<DialogDialogButton> btnSSN = new List<DialogDialogButton>() { };
        //        btnSSN.Add(new DialogDialogButton() { Click = "GetSSNDetail", ButtonModel = new promptButtonModel() { content = "Validate", isPrimary = true } });
        //        btnSSN.Add(new DialogDialogButton() { Click = "SSNpromptBtnClick", ButtonModel = new promptButtonModel() { content = "Cancel" } });

        //        ViewBag.PromptButtonSSN = btnSSN;

        //        List<DialogDialogButton> btnDocumentAdd = new List<DialogDialogButton>() { };
        //        btnDocumentAdd.Add(new DialogDialogButton() { Click = "InsertDocument", ButtonModel = new promptButtonModel() { content = "Save", isPrimary = true } });
        //        btnDocumentAdd.Add(new DialogDialogButton() { Click = "AddDocumentpromptBtnClick", ButtonModel = new promptButtonModel() { content = "Cancel" } });

        //        ViewBag.PromptButtonAddDocument = btnDocumentAdd;

        //        List<DialogDialogButton> btnUpload401plan = new List<DialogDialogButton>() { };
        //        btnUpload401plan.Add(new DialogDialogButton() { Click = "Insert401Document", ButtonModel = new promptButtonModel() { content = "Save", isPrimary = true } });
        //        btnUpload401plan.Add(new DialogDialogButton() { Click = "Upload401DocumentpromptBtnClick", ButtonModel = new promptButtonModel() { content = "Cancel" } });

        //        ViewBag.PromptbtnUpload401plan = btnUpload401plan;
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("HREmployeeProfileController - Index - " + DateTime.Now + " - " + ex.Message.ToString());
        //    }
        //    return View(vm);
        //}
        public ActionResult Index(int EmployeeId)
        {
            HREmployeeProfileViewModal vm = new HREmployeeProfileViewModal();
            try
            {
                logger.Info("HREmployeeProfileController - Index - " + DateTime.Now);
                HREmployeeMaster hR = _hREmployeeRepository.GetHREmployeeMaster(EmployeeId);
                if (hR != null)
                {
                    vm.EmployeeId = hR.EmployeeId;
                    vm.DateofBirth = hR.DateofBirth;
                    vm.FirstName = hR.FirstName;
                    vm.LastName = hR.LastName;
                    vm.Email = hR.Email;
                    vm.Phone = hR.Phone;
                    vm.Street = hR.Street;
                    vm.City = hR.City;
                    vm.FullSSN = hR.FullSSN;
                    vm.Gender = hR.Gender;
                    vm.MaritalStatus = hR.MaritalStatus;
                    vm.LanguageId = hR.LanguageId;
                    vm.MobileNo = hR.MobileNo;
                    vm.Designation = hR.Designation;
                }
                List<StoreDetail> sr = _hREmployeeRepository.GetEmployeeUsingStore(EmployeeId);
                var storeId = sr.FirstOrDefault().StoreId;
                vm.EmployeeChildId = _hREmployeeRepository.GetHREmployeeChildByEmployeeIdandStoreId(EmployeeId, storeId).EmployeeChildId;
                ViewBag.StoreId = new SelectList(sr, "StoreId", "Name", storeId);

                // Store StoreId in TempData
                TempData["StoreId"] = storeId;

                List<DialogDialogButton> btn = new List<DialogDialogButton>() { };
                btn.Add(new DialogDialogButton() { Click = "GetHourlyRateDetail", ButtonModel = new promptButtonModel() { content = "Validate", isPrimary = true } });
                btn.Add(new DialogDialogButton() { Click = "promptBtnClick", ButtonModel = new promptButtonModel() { content = "Cancel" } });

                ViewBag.PromptButton = btn;

                List<DialogDialogButton> btnSSN = new List<DialogDialogButton>() { };
                btnSSN.Add(new DialogDialogButton() { Click = "GetSSNDetail", ButtonModel = new promptButtonModel() { content = "Validate", isPrimary = true } });
                btnSSN.Add(new DialogDialogButton() { Click = "SSNpromptBtnClick", ButtonModel = new promptButtonModel() { content = "Cancel" } });

                ViewBag.PromptButtonSSN = btnSSN;

                List<DialogDialogButton> btnDocumentAdd = new List<DialogDialogButton>() { };
                btnDocumentAdd.Add(new DialogDialogButton() { Click = "InsertDocument", ButtonModel = new promptButtonModel() { content = "Save", isPrimary = true } });
                btnDocumentAdd.Add(new DialogDialogButton() { Click = "AddDocumentpromptBtnClick", ButtonModel = new promptButtonModel() { content = "Cancel" } });

                ViewBag.PromptButtonAddDocument = btnDocumentAdd;

                List<DialogDialogButton> btnUpload401plan = new List<DialogDialogButton>() { };
                btnUpload401plan.Add(new DialogDialogButton() { Click = "Insert401Document", ButtonModel = new promptButtonModel() { content = "Save", isPrimary = true } });
                btnUpload401plan.Add(new DialogDialogButton() { Click = "Upload401DocumentpromptBtnClick", ButtonModel = new promptButtonModel() { content = "Cancel" } });

                ViewBag.PromptbtnUpload401plan = btnUpload401plan;
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeProfileController - Index - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View(vm);
        }


        public ActionResult GetStoerWiseDetail(int EmployeeId, int StoreId)
        {
            HREmployeeProfileChild vm = new HREmployeeProfileChild();
            try
            {
                logger.Info("HREmployeeProfileController - GetStoerWiseDetail - " + DateTime.Now);
                HREmployeeChild hR = _hREmployeeRepository.GetHREmployeeChildByEmployeeIdandStoreId(EmployeeId, StoreId);
                if (hR != null)
                {
                    vm.EmployeeId = hR.EmployeeId;
                    vm.StoreId = Convert.ToInt32(hR.StoreId);
                    vm.StoreName = _CompaniesRepository.GetStoreMastersbyID(Convert.ToInt32(hR.StoreId)).Name;
                    vm.HireDate = hR.HireDate;
                    vm.TerminationDate = hR.TerminationDate;
                    vm.EmployeementTypeStatus = hR.EmployeementTypeStatus;
                    vm.EmployeeChildId = hR.EmployeeChildId;
                    vm.OfficeEmployeeID = hR.OfficeEmployeeID;
                }

            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeProfileController - GetStoerWiseDetail - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSSN(int EmployeeId, string Password)
        {
            SuccessMessage = null;
            ErrorMessage = null;
            string SSN = "";
            try
            {
                logger.Info("HREmployeeProfileController - GetSSN - " + DateTime.Now);
                int UserId = (int)Session["UserId"];
                SSN = _hREmployeeRepository.GetSSN(Password, EmployeeId, UserId);
                if (SSN == "")
                {
                    ErrorMessage = "Password Is Incorrect.";
                }
                else
                {
                    SuccessMessage = "SSN viewed successfully.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("HREmployeeProfileController - GetSSN - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = SSN, success = SuccessMessage, Error = ErrorMessage });
        }

        #region Pay Rate Tab
        [Authorize(Roles = "Administrator,ViewPayRatesHRSubModule")]
        public ActionResult HRPayRateUrlDatasource(DataManagerRequest dm, int EmployeeId, int StoreId, int EmployeeChildId)
        {
            List<HREmployeePayRate> HrEPR = new List<HREmployeePayRate>();
            IEnumerable DataSource = new List<HREmployeePayRate>();
            int Count = 0;
            try
            {
                logger.Info("HREmployeeProfileController - HRPayRateUrlDatasource - " + DateTime.Now);
                //Using this db class Get Web cam details by storeid.
                HrEPR = _hREmployeeRepository.GetHREmployeePayRateList(EmployeeId, StoreId, EmployeeChildId);
                DataSource = HrEPR;
                
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
                Count = DataSource.Cast<HREmployeePayRate>().Count();
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
                logger.Error("HREmployeeProfileController - HRPayRateUrlDatasource - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return dm.RequiresCounts ? Json(new { result = DataSource, count = Count }) : Json(DataSource);
        }
        public ActionResult AddPartial()
        {
            HREmployeePayRate hREmployee = new HREmployeePayRate();
            try
            {
                logger.Info("HREmployeeProfileController - AddPartial - " + DateTime.Now);

                hREmployee.PayRateDate = DateTime.Now;
                ViewBag.PayType = from PayType e in Enum.GetValues(typeof(PayType)) select new EnumDropDown { value = (int)e, text = e.ToString() };
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeProfileController - AddPartial - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView("_PayRate", hREmployee);
        }

        public ActionResult Editpartial(HREmployeePayRate value)
        {
            HREmployeePayRate PayRate = new HREmployeePayRate();
            try
            {
                logger.Info("HREmployeeProfileController - Editpartial - " + DateTime.Now);
                PayRate = _hREmployeeRepository.GetHREmployeePayRateByID(value.EmployeePayRateId);
                ViewBag.PayType = from PayType e in Enum.GetValues(typeof(PayType)) select new EnumDropDown { value = (int)e, text = e.ToString() };
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeProfileController - Editpartial - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView("_PayRate", PayRate);
        }
        public async Task<ActionResult> HRPayRateInsert(CRUDModel<HREmployeePayRate> PayRate, int EmployeeId, int StoreId, int EmployeeChildId)
        {
            HREmployeePayRate HR = new HREmployeePayRate();
            SuccessMessage = null;
            ErrorMessage = null;
            try
            {
                logger.Info("HREmployeeProfileController - HRPayRateInsert - " + DateTime.Now);
                PayRate.Value.CreatedBy = _CommonRepository.getUserId(UserName);
                PayRate.Value.ModifiedBy = PayRate.Value.CreatedBy;
                PayRate.Value.EmployeeId = EmployeeId;
                PayRate.Value.EmployeeChildId = EmployeeChildId;
                HR = _hREmployeeRepository.InsertUpdateHREmployeePayRate(PayRate.Value);
                SuccessMessage = "Pay Rate Created Successfully.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("HREmployeeProfileController - HRPayRateInsert - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = HR, success = SuccessMessage, Error = ErrorMessage });
        }

        public async Task<ActionResult> HRPayRateUpdate(CRUDModel<HREmployeePayRate> PayRate)
        {
            HREmployeePayRate HR = new HREmployeePayRate();
            SuccessMessage = null;
            ErrorMessage = null;
            try
            {
                logger.Info("HREmployeeProfileController - HRPayRateUpdate - " + DateTime.Now);
                HR = _hREmployeeRepository.GetHREmployeePayRateByID(Convert.ToInt32(PayRate.Key));
                HR.PayRateDate = PayRate.Value.PayRateDate;
                HR.PayType = (PayType)PayRate.Value.PayType;
                HR.PayRate = PayRate.Value.PayRate;
                HR.Comments = PayRate.Value.Comments;
                HR.ModifiedBy = _CommonRepository.getUserId(UserName);
                HR.ModifiedOn = DateTime.Now;
                HR = _hREmployeeRepository.InsertUpdateHREmployeePayRate(HR);
                SuccessMessage = "Pay Rate Updated Successfully.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("HREmployeeProfileController - HRPayRateUpdate - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = PayRate, success = SuccessMessage, Error = ErrorMessage });
        }

        public async Task<ActionResult> HRPayRateRemove(CRUDModel<HREmployeePayRate> PayRate)
        {
            HREmployeePayRate HR = new HREmployeePayRate();
            SuccessMessage = null;
            ErrorMessage = null;
            try
            {
                logger.Info("HREmployeeProfileController - HRPayRateRemove - " + DateTime.Now);
                HR = _hREmployeeRepository.RemoveHRPayRate(Convert.ToInt32(PayRate.Key));
                SuccessMessage = "Pay Rate Deleted Successfully.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("HREmployeeProfileController - HRPayRateRemove - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = HR, success = SuccessMessage, Error = ErrorMessage });
        }

        public ActionResult GetValidateRate(string EmployeePayRateId)
        {
            ValidPayRate validPayRate = new ValidPayRate();
            try
            {
                logger.Info("HREmployeeProfileController - GetValidateRate - " + DateTime.Now);
                validPayRate.EmployeePayRateId = Convert.ToInt32(EmployeePayRateId);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("HREmployeeProfileController - GetValidateRate - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView("_ValidationGetPayRate", validPayRate);
        }

        public ActionResult GetValidatePasswordForSSN(string EmployeeId)
        {
            ValidPassForSSN validPass = new ValidPassForSSN();
            try
            {
                logger.Info("HREmployeeProfileController - GetValidatePasswordForSSN - " + DateTime.Now);
                validPass.EmployeeId = Convert.ToInt32(EmployeeId);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("HREmployeeProfileController - GetValidatePasswordForSSN - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView("_ValidatePassForSSN", validPass);
        }

        public ActionResult GetHourlyRate(int EmployeePayRateId, string password)
        {
            SuccessMessage = null;
            ErrorMessage = null;
            decimal PayRate = 0;
            try
            {
                logger.Info("HREmployeeProfileController - GetHourlyRate - " + DateTime.Now);
                int UserId = (int)Session["UserId"];
                PayRate = _hREmployeeRepository.GetHourlyRate(password, EmployeePayRateId, UserId);
                if (PayRate == 0)
                {
                    ErrorMessage = "Password Is Incorrect.";
                }
                else
                {
                    SuccessMessage = "Pay Rate get successfully.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("HREmployeeProfileController - GetHourlyRate - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return Json(new { data = PayRate, success = SuccessMessage, Error = ErrorMessage });
        }
        #endregion Pay Rate Tab

        #region Sick Time
        [Authorize(Roles = "Administrator,ViewSickTimesHRSubModule")]
        public ActionResult SickTimesUrlDatasource(DataManagerRequest dm, int EmployeeId, int StoreId, int EmployeeChildId)
        {
            List<HREmployeeSickTimes> HrEPR = new List<HREmployeeSickTimes>();
            IEnumerable DataSource = new List<HREmployeeSickTimes>();
            int Count = 0;
            try
            {
                logger.Info("HREmployeeProfileController - SickTimesUrlDatasource - " + DateTime.Now);
                //Using this db class Get Web cam details by storeid.
                HrEPR = _hREmployeeRepository.GetHREmployeeSickTimeList(EmployeeId, StoreId, EmployeeChildId);
                DataSource = HrEPR;
                
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
                Count = DataSource.Cast<HREmployeeSickTimes>().Count();
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
                logger.Error("HREmployeeProfileController - SickTimesUrlDatasource - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return dm.RequiresCounts ? Json(new { result = DataSource, count = Count }) : Json(DataSource);
        }

        public ActionResult SickTimeAddpartial()
        {
            HREmployeeSickTimes hREmployee = new HREmployeeSickTimes();
            try
            {
                logger.Info("HREmployeeProfileController - SickTimeAddpartial - " + DateTime.Now);

                hREmployee.sEffectiveDate = DateTime.Now.ToString("MM/dd/yyyy");
                ViewBag.TimeType = from TimeType e in Enum.GetValues(typeof(TimeType)) select new EnumDropDown { value = (int)e, text = e.ToString() };
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeProfileController - SickTimeAddpartial - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView("_SickTime", hREmployee);
        }

        public ActionResult SickTimeEditpartial(HREmployeeSickTimes value)
        {
            HREmployeeSickTimes sickTimes = new HREmployeeSickTimes();
            try
            {
                logger.Info("HREmployeeProfileController - SickTimeEditpartial - " + DateTime.Now);
                sickTimes = _hREmployeeRepository.GetHREmployeeSickTimeByID(value.EmployeeSickTimeId);
                sickTimes.sEffectiveDate = Convert.ToDateTime(sickTimes.EffectiveDate).ToString("MM/dd/yyyy");
                ViewBag.TimeType = from TimeType e in Enum.GetValues(typeof(TimeType)) select new EnumDropDown { value = (int)e, text = e.ToString() };
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeProfileController - SickTimeEditpartial - " + DateTime.Now + " - " + ex.Message.ToString());
                throw;
            }
            return PartialView("_SickTime", sickTimes);
        }

        public async Task<ActionResult> SickTimesInsert(CRUDModel<HREmployeeSickTimes> SickTime, int EmployeeId, int StoreId, int EmployeeChildId)
        {
            HREmployeeSickTimes HR = new HREmployeeSickTimes();
            SuccessMessage = null;
            ErrorMessage = null;
            try
            {
                logger.Info("HREmployeeProfileController - SickTimesInsert - " + DateTime.Now);
                SickTime.Value.CreatedBy = _CommonRepository.getUserId(UserName);
                SickTime.Value.ModifiedBy = SickTime.Value.CreatedBy;
                SickTime.Value.EmployeeId = EmployeeId;
                SickTime.Value.EmployeeChildId = EmployeeChildId;
                SickTime.Value.EffectiveDate = Convert.ToDateTime(SickTime.Value.sEffectiveDate);
                HR = _hREmployeeRepository.InsertUpdateHREmployeeSickTimes(SickTime.Value);
                SuccessMessage = "Sick Time Created Successfully.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("HREmployeeProfileController - SickTimesInsert - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = HR, success = SuccessMessage, Error = ErrorMessage });
        }

        public async Task<ActionResult> SickTimesUpdate(CRUDModel<HREmployeeSickTimes> SickTime)
        {
            HREmployeeSickTimes HR = new HREmployeeSickTimes();
            SuccessMessage = null;
            ErrorMessage = null;
            try
            {
                logger.Info("HREmployeeProfileController - SickTimesUpdate - " + DateTime.Now);
                HR = _hREmployeeRepository.GetHREmployeeSickTimeByID(Convert.ToInt32(SickTime.Key));
                HR.EffectiveDate = Convert.ToDateTime(SickTime.Value.sEffectiveDate);
                HR.sEffectiveDate = Convert.ToDateTime(HR.EffectiveDate).ToString("MM/dd/yyyy");
                HR.TimeType = SickTime.Value.TimeType;
                HR.Time = SickTime.Value.Time;
                HR.Comments = SickTime.Value.Comments;
                HR.ModifiedBy = _CommonRepository.getUserId(UserName);
                HR.ModifiedOn = DateTime.Now;
                HR = _hREmployeeRepository.InsertUpdateHREmployeeSickTimes(HR);
                SuccessMessage = "Sick Time Updated Successfully.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("HREmployeeProfileController - SickTimesUpdate - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = SickTime, success = SuccessMessage, Error = ErrorMessage });
        }

        public async Task<ActionResult> SickTimesRemove(CRUDModel<HREmployeeSickTimes> SickTime)
        {
            HREmployeeSickTimes HR = new HREmployeeSickTimes();
            SuccessMessage = null;
            ErrorMessage = null;
            try
            {
                logger.Info("HREmployeeProfileController - SickTimesRemove - " + DateTime.Now);
                HR = _hREmployeeRepository.RemoveHREmployeeSickTime(Convert.ToInt32(SickTime.Key));
                SuccessMessage = "Sick Time Deleted Successfully.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("HREmployeeProfileController - SickTimesRemove - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = HR, success = SuccessMessage, Error = ErrorMessage });
        }
        #endregion Sick Time

        #region Insurance Tab
        [Authorize(Roles = "Administrator,ViewInsuranceHRSubModule")]
        public ActionResult HRInsuranceUrlDatasource(DataManagerRequest dm, int EmployeeId, int StoreId, int EmployeeChildId)
        {
            List<HREmployeeInsurance> HrEPR = new List<HREmployeeInsurance>();
            IEnumerable DataSource = new List<HREmployeeInsurance>();
            int Count = 0;
            try
            {
                logger.Info("HREmployeeProfileController - HRInsuranceUrlDatasource - " + DateTime.Now);
                //Using this db class Get Web cam details by storeid.
                HrEPR = _hREmployeeRepository.GetHREmployeeInsurance(EmployeeId, StoreId, EmployeeChildId);
                DataSource = HrEPR;
                
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
                Count = DataSource.Cast<HREmployeeInsurance>().Count();
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
                logger.Error("HREmployeeProfileController - HRInsuranceUrlDatasource - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return dm.RequiresCounts ? Json(new { result = DataSource, count = Count }) : Json(DataSource);
        }

        public async Task<ActionResult> HRInsuranceInsert(CRUDModel<HREmployeeInsurance> Insurance, int EmployeeId, int StoreId, int EmployeeChildId)
        {
            HREmployeeInsurance HR = new HREmployeeInsurance();
            SuccessMessage = null;
            ErrorMessage = null;
            try
            {
                logger.Info("HREmployeeProfileController - HRInsuranceInsert - " + DateTime.Now);
                Insurance.Value.CreatedBy = _CommonRepository.getUserId(UserName);
                Insurance.Value.ModifiedBy = Insurance.Value.CreatedBy;
                Insurance.Value.EmployeeId = EmployeeId;
                Insurance.Value.EmployeeChildId = EmployeeChildId;
                Insurance.Value.EffectiveDate = Convert.ToDateTime(Insurance.Value.sEffectiveDate);

                if (Insurance.Value.FileName == null)
                {
                    ErrorMessage = "Please Upload File.";
                }
                else
                {
                    HR = _hREmployeeRepository.InsertUpdateHREmployeeInsurance(Insurance.Value);
                    SuccessMessage = "Insurance Created Successfully.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("HREmployeeProfileController - HRInsuranceInsert - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = HR, success = SuccessMessage, Error = ErrorMessage });
        }

        public async Task<ActionResult> HRInsuranceUpdate(CRUDModel<HREmployeeInsurance> Insurance)
        {
            HREmployeeInsurance HR = new HREmployeeInsurance();
            SuccessMessage = null;
            ErrorMessage = null;
            try
            {
                logger.Info("HREmployeeProfileController - HRInsuranceUpdate - " + DateTime.Now);
                HR = _hREmployeeRepository.GetHREmployeeInsuranceID(Convert.ToInt32(Insurance.Key));
                HR.EffectiveDate = Insurance.Value.EffectiveDate;
                HR.EnrollmentStatus = Insurance.Value.EnrollmentStatus;
                HR.FileName = Insurance.Value.FileName;
                HR.Comments = Insurance.Value.Comments;
                HR.ModifiedBy = _CommonRepository.getUserId(UserName);
                HR.ModifiedOn = DateTime.Now;
                HR = _hREmployeeRepository.InsertUpdateHREmployeeInsurance(HR);
                SuccessMessage = "Insurance Updated Successfully.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("HREmployeeProfileController - HRInsuranceUpdate - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = Insurance, success = SuccessMessage, Error = ErrorMessage });
        }

        public async Task<ActionResult> HRInsuranceRemove(CRUDModel<HREmployeeInsurance> Insurance)
        {
            HREmployeeInsurance HR = new HREmployeeInsurance();
            SuccessMessage = null;
            ErrorMessage = null;
            try
            {
                logger.Info("HREmployeeProfileController - HRInsuranceRemove - " + DateTime.Now);
                HR = _hREmployeeRepository.RemoveHREmployeeInsurance(Convert.ToInt32(Insurance.Key));
                SuccessMessage = "Insurance Deleted Successfully.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("HREmployeeProfileController - HRInsuranceRemove - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = HR, success = SuccessMessage, Error = ErrorMessage });
        }

        public ActionResult HRInsuranceAddPartial()
        {
            HREmployeeInsurance hRInsurance = new HREmployeeInsurance();
            string DirectoryPath = "~/InsuranceFiles/";
            if (!Directory.Exists(Server.MapPath(DirectoryPath)))
            {
                Directory.CreateDirectory(Server.MapPath(DirectoryPath));
            }
            try
            {
                logger.Info("HREmployeeProfileController - HRInsuranceAddPartial - " + DateTime.Now);
                hRInsurance.sEffectiveDate = DateTime.Now.ToString("MM/dd/yyyy");
                ViewBag.EnrollmentStatus = from EnrollmentStatus e in Enum.GetValues(typeof(EnrollmentStatus)) select new EnumDropDown { value = (int)e, text = e.ToString() };
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeProfileController - HRInsuranceAddPartial - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView("_Insurance", hRInsurance);
        }

        public ActionResult HRInsuranceEditpartial(HREmployeeInsurance value)
        {
            HREmployeeInsurance hRInsurance = new HREmployeeInsurance();
            try
            {
                logger.Info("HREmployeeProfileController - HRInsuranceEditpartial - " + DateTime.Now);
                hRInsurance = _hREmployeeRepository.GetHREmployeeInsuranceID(value.EmployeeInsuranceId);
                hRInsurance.sEffectiveDate = Convert.ToDateTime(hRInsurance.EffectiveDate).ToString("MM/dd/yyyy");
                ViewBag.EnrollmentStatus = from EnrollmentStatus e in Enum.GetValues(typeof(EnrollmentStatus)) select new EnumDropDown { value = (int)e, text = e.ToString() };
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeProfileController - HRInsuranceEditpartial - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView("_Insurance", hRInsurance);
        }

        [AcceptVerbs("Post")]
        public void Save()
        {
            try
            {
                logger.Info("HREmployeeProfileController - Save - " + DateTime.Now);
                string Sacn_Title = "";
                if (System.Web.HttpContext.Current.Request.Files.AllKeys.Length > 0)
                {
                    var httpPostedFile = System.Web.HttpContext.Current.Request.Files["UploadFiles"];
                    Sacn_Title = AdminSiteConfiguration.GetRandomNo() + System.IO.Path.GetFileName(httpPostedFile.FileName);
                    Sacn_Title = AdminSiteConfiguration.RemoveSpecialCharacter(Sacn_Title);
                    if (httpPostedFile != null)
                    {
                        var fileSave = System.Web.HttpContext.Current.Server.MapPath("~/UserFiles/HR_File/InsuranceFiles");
                        if (!System.IO.File.Exists(fileSave))
                        {
                            System.IO.Directory.CreateDirectory(fileSave);
                        }
                        var fileSavePath = System.IO.Path.Combine(fileSave, Sacn_Title);
                        if (!System.IO.File.Exists(fileSavePath))
                        {
                            httpPostedFile.SaveAs(fileSavePath);
                            HttpResponse Response = System.Web.HttpContext.Current.Response;
                            Response.Clear();
                            Response.Headers.Add("name", Sacn_Title);
                            Response.ContentType = "application/json; charset=utf-8";
                            Response.StatusDescription = "File uploaded succesfully";
                            Response.End();
                            AdminSiteConfiguration.WriteErrorLogs(Sacn_Title + " File uploaded succesfully");
                        }
                        else
                        {
                            HttpResponse Response = System.Web.HttpContext.Current.Response;
                            Response.Clear();
                            Response.Status = "204 File already exists";
                            Response.StatusCode = 204;
                            Response.StatusDescription = "File already exists";
                            Response.End();
                            AdminSiteConfiguration.WriteErrorLogs(Sacn_Title + " File already exists.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("BulkUploadFileController - Save - " + DateTime.Now + " - " + ex.Message.ToString());
                AdminSiteConfiguration.WriteErrorLogs(ex.Message);
                HttpResponse Response = System.Web.HttpContext.Current.Response;
                Response.Clear();
                Response.ContentType = "application/json; charset=utf-8";
                Response.StatusCode = 204;
                Response.Status = "204 No Content";
                Response.StatusDescription = ex.Message;
                Response.End();
            }
        }

        [AcceptVerbs("Post")]
        public void Remove()
        {
            try
            {
                logger.Info("HREmployeeProfileController - Remove - " + DateTime.Now);
                var httpPostedFile = System.Web.HttpContext.Current.Request.Files["UploadFiles"];
                var fileSave = System.Web.HttpContext.Current.Server.MapPath("~/UserFiles/HR_File/InsuranceFiles");
                var fileSavePath = System.IO.Path.Combine(fileSave, httpPostedFile.FileName);
                if (System.IO.File.Exists(fileSavePath))
                {
                    System.IO.File.Delete(fileSavePath); // removed the uploaded file from the saved area
                }

                HttpResponse Response = System.Web.HttpContext.Current.Response;
                Response.Clear();
                Response.Status = "200 OK";
                Response.StatusCode = 200;
                Response.ContentType = "application/json; charset=utf-8";
                Response.StatusDescription = "File removed succesfully";
                Response.End();
            }
            catch (Exception e)
            {
                HttpResponse Response = System.Web.HttpContext.Current.Response;
                Response.Clear();
                Response.Status = "200 OK";
                Response.StatusCode = 200;
                Response.ContentType = "application/json; charset=utf-8";
                Response.StatusDescription = "File removed succesfully";
                Response.End();
            }
        }

        public ActionResult DownloadInsuranceLetter(string InsuranceFileName, int EmployeeId)
        {
            if (InsuranceFileName != "" && InsuranceFileName != null)
            {
                string FilePath = Server.MapPath("~/UserFiles/HR_File/InsuranceFiles/" + InsuranceFileName.Trim());
                if (System.IO.File.Exists(FilePath))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, InsuranceFileName.Trim());
                }
            }
            return RedirectToAction("Index", "HREmployeeProfile", new { EmployeeId = EmployeeId });
        }
        #endregion Insurance Tab

        #region Vacation Time Tab
        [Authorize(Roles = "Administrator,ViewVacationTimesHRSubModule")]
        public ActionResult HRVacationUrlDatasource(DataManagerRequest dm, int EmployeeId, int StoreId, int EmployeeChildId)
        {
            List<HREmployeeVacationTime> HrEPR = new List<HREmployeeVacationTime>();
            IEnumerable DataSource = new List<HREmployeeVacationTime>();
            int Count = 0;
            try
            {
                logger.Info("HREmployeeProfileController - HRVacationUrlDatasource - " + DateTime.Now);
                //Using this db class Get Web cam details by storeid.
                HrEPR = _hREmployeeRepository.GetHREmployeeVacationTimeList(EmployeeId, StoreId, EmployeeChildId);
                DataSource = HrEPR;
                
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
                Count = DataSource.Cast<HREmployeeVacationTime>().Count();
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
                logger.Error("HREmployeeProfileController - HRVacationUrlDatasource - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return dm.RequiresCounts ? Json(new { result = DataSource, count = Count }) : Json(DataSource);
        }

        public async Task<ActionResult> HRVacationTimeInsert(CRUDModel<HREmployeeVacationTime> VacationTime, int EmployeeId, int StoreId, int EmployeeChildId)
        {
            HREmployeeVacationTime HR = new HREmployeeVacationTime();
            SuccessMessage = null;
            ErrorMessage = null;
            try
            {
                logger.Info("HREmployeeProfileController - HRVacationTimeInsert - " + DateTime.Now);
                VacationTime.Value.CreatedBy = _CommonRepository.getUserId(UserName);
                VacationTime.Value.ModifiedBy = VacationTime.Value.CreatedBy;
                VacationTime.Value.EmployeeId = EmployeeId;
                VacationTime.Value.EmployeeChildId = EmployeeChildId;
                VacationTime.Value.EffectiveDate = Convert.ToDateTime(VacationTime.Value.sEffectiveDate);
                HR = _hREmployeeRepository.InsertUpdateHREmployeeVacationTime(VacationTime.Value);
                SuccessMessage = "Vacation Time Created Successfully.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("HREmployeeProfileController - HRVacationTimeInsert - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = HR, success = SuccessMessage, Error = ErrorMessage });
        }

        public async Task<ActionResult> HRVacationTimeUpdate(CRUDModel<HREmployeeVacationTime> VacationTime)
        {
            HREmployeeVacationTime HR = new HREmployeeVacationTime();
            SuccessMessage = null;
            ErrorMessage = null;
            try
            {
                logger.Info("HREmployeeProfileController - HRVacationTimeUpdate - " + DateTime.Now);
                HR = _hREmployeeRepository.GetHREmployeeVacationByID(Convert.ToInt32(VacationTime.Key));
                HR.EffectiveDate = Convert.ToDateTime(VacationTime.Value.sEffectiveDate);
                HR.sEffectiveDate = Convert.ToDateTime(HR.EffectiveDate).ToString("MM/dd/yyyy");
                HR.TimeType = VacationTime.Value.TimeType;
                HR.Time = VacationTime.Value.Time;
                HR.Comments = VacationTime.Value.Comments;
                HR.ModifiedBy = _CommonRepository.getUserId(UserName);
                HR.ModifiedOn = DateTime.Now;
                HR = _hREmployeeRepository.InsertUpdateHREmployeeVacationTime(HR);
                SuccessMessage = "Vacation Time Updated Successfully.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("HREmployeeProfileController - HRVacationTimeUpdate - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = VacationTime, success = SuccessMessage, Error = ErrorMessage });
        }

        public async Task<ActionResult> HRVacationTimeRemove(CRUDModel<HREmployeeVacationTime> VacationTime)
        {
            HREmployeeVacationTime HR = new HREmployeeVacationTime();
            SuccessMessage = null;
            ErrorMessage = null;
            try
            {
                logger.Info("HREmployeeProfileController - HRVacationTimeRemove - " + DateTime.Now);
                HR = _hREmployeeRepository.RemoveHREmployeeVacationTime(Convert.ToInt32(VacationTime.Key));
                SuccessMessage = "Vacation Time Deleted Successfully.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("HREmployeeProfileController - HRVacationTimeRemove - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = HR, success = SuccessMessage, Error = ErrorMessage });
        }

        public ActionResult HRVacationTimeAddPartial()
        {
            HREmployeeVacationTime hRVacation = new HREmployeeVacationTime();
            try
            {
                logger.Info("HREmployeeProfileController - HRVacationTimeAddPartial - " + DateTime.Now);
                hRVacation.sEffectiveDate = DateTime.Now.ToString("MM/dd/yyyy");
                ViewBag.TimeType = from TimeType e in Enum.GetValues(typeof(TimeType)) select new EnumDropDown { value = (int)e, text = e.ToString() };
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeProfileController - HRVacationTimeAddPartial - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView("_VacationTime", hRVacation);
        }

        public ActionResult HRVacationTimeEditpartial(HREmployeeVacationTime value)
        {
            HREmployeeVacationTime hRVacation = new HREmployeeVacationTime();
            try
            {
                logger.Info("HREmployeeProfileController - HRVacationTimeEditpartial - " + DateTime.Now);
                hRVacation = _hREmployeeRepository.GetHREmployeeVacationByID(value.EmployeeVacationTimeId);
                hRVacation.sEffectiveDate = Convert.ToDateTime(hRVacation.EffectiveDate).ToString("MM/dd/yyyy");
                ViewBag.TimeType = from TimeType e in Enum.GetValues(typeof(TimeType)) select new EnumDropDown { value = (int)e, text = e.ToString() };
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeProfileController - HRVacationTimeEditpartial - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView("_VacationTime", hRVacation);
        }
        #endregion Vacation Time Tab

        #region Documents Tab
        [Authorize(Roles = "Administrator,ViewDocumentsHRSubModule")]
        public ActionResult HRDocumnetsUrlDatasource(int EmployeeId, int StoreId, int EmployeeChildId)
        {
            List<HRFileDetailViewModel> HrEPR = new List<HRFileDetailViewModel>();
            HRAllFileList hRAllFileList = new HRAllFileList();
            try
            {
                logger.Info("HREmployeeProfileController - HRDocumnetsUrlDatasource - " + DateTime.Now);
                //Using this db class Get Web cam details by storeid.
                HrEPR = _hREmployeeRepository.GetHREmployeeDocumentsList(EmployeeId, StoreId, EmployeeChildId);
                hRAllFileList.File401KPlan = HrEPR.Where(s => s.DocType == "1").ToList();
                hRAllFileList.MobileInsurance = HrEPR.Where(s => s.DocType == "2").ToList();
                hRAllFileList.EssentialDocuments = HrEPR.Where(s => s.DocType == "3").ToList();
                hRAllFileList.SignedForms = HrEPR.Where(s => s.DocType == "4").ToList();
                hRAllFileList.VaccinationInfo = HrEPR.Where(s => s.DocType == "5").ToList();
                hRAllFileList.Warning = HrEPR.Where(s => s.DocType == "6").ToList();
                hRAllFileList.Termination = HrEPR.Where(s => s.DocType == "7").ToList();
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeProfileController - HRDocumnetsUrlDatasource - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { result = hRAllFileList }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> HRDocumentsInsert(int EmployeeId, int EmployeeChildId, string FileName, string Comments)
        {
            HREmployeeDocument HR = new HREmployeeDocument();
            SuccessMessage = null;
            ErrorMessage = null;
            try
            {
                logger.Info("HREmployeeProfileController - HRDocumentsInsert - " + DateTime.Now);
                HR.EmployeeId = EmployeeId;
                HR.EmployeeChildId = EmployeeChildId;
                HR.FileName = FileName;
                HR.LocationFrom = 1;
                HR.Extension = System.IO.Path.GetExtension(FileName);
                HR.DocumentType = 0;
                HR.Comments = Comments;
                HR.CreatedBy = _CommonRepository.getUserId(UserName);
                HR.ModifiedBy = HR.CreatedBy;
                HR.EmployeeId = EmployeeId;
                HR.EmployeeChildId = EmployeeChildId;
                HR = _hREmployeeRepository.InsertHREmployeeDocuments(HR);
                SuccessMessage = "Document Uploaded Successfully.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("HREmployeeProfileController - HRDocumentsInsert - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { success = SuccessMessage, Error = ErrorMessage });
        }

        public ActionResult HRDocumentsAddPartial(int EmployeeId, int EmployeeChildId, int StoreId)
        {
            HREmployeeDocument hRdocuments = new HREmployeeDocument();
            hRdocuments.EmployeeId = EmployeeId;
            hRdocuments.EmployeeChildId = EmployeeChildId;
            string DirectoryPath = "~/DocumentFiles/";
            if (!Directory.Exists(Server.MapPath(DirectoryPath)))
            {
                Directory.CreateDirectory(Server.MapPath(DirectoryPath));
            }
            try
            {
                logger.Info("HREmployeeProfileController - HRDocumentsAddPartial - " + DateTime.Now);

            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeProfileController - HRDocumentsAddPartial - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView("_Documents", hRdocuments);
        }

        public ActionResult HRDocumentsUpload401planPartial(int EmployeeId, int EmployeeChildId, int StoreId)
        {
            HREmployeeDocument hRdocuments = new HREmployeeDocument();
            hRdocuments.EmployeeId = EmployeeId;
            hRdocuments.EmployeeChildId = EmployeeChildId;
            try
            {
                logger.Info("HREmployeeProfileController - HRDocumentsUpload401planPartial - " + DateTime.Now);

            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeProfileController - HRDocumentsUpload401planPartial - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView("_Document401KPlan", hRdocuments);
        }


        [AcceptVerbs("Post")]
        public void SaveDocuments(int EmployeeId, int Type)
        {
            try
            {
                logger.Info("HREmployeeProfileController - SaveDocuments - " + DateTime.Now);
                string Sacn_Title = "";
                if (System.Web.HttpContext.Current.Request.Files.AllKeys.Length > 0)
                {
                    var httpPostedFile = System.Web.HttpContext.Current.Request.Files["UploadFiles"];
                    Sacn_Title = AdminSiteConfiguration.GetRandomNo() + System.IO.Path.GetFileName(httpPostedFile.FileName);
                    Sacn_Title = AdminSiteConfiguration.RemoveSpecialCharacter(Sacn_Title);
                    if (httpPostedFile != null)
                    {
                        var fileSave = "";
                        if (Type == 1)
                        {
                            fileSave = System.Web.HttpContext.Current.Server.MapPath("~/UserFiles/HR_File/EmployeeDocument/" + EmployeeId);
                        }
                        else
                        {
                            fileSave = System.Web.HttpContext.Current.Server.MapPath("~/UserFiles/HR_File/EmployeeDocument_401K/" + EmployeeId);
                        }

                        if (!System.IO.File.Exists(fileSave))
                        {
                            System.IO.Directory.CreateDirectory(fileSave);
                        }
                        var fileSavePath = System.IO.Path.Combine(fileSave, Sacn_Title);
                        if (!System.IO.File.Exists(fileSavePath))
                        {
                            httpPostedFile.SaveAs(fileSavePath);
                            HttpResponse Response = System.Web.HttpContext.Current.Response;
                            Response.Clear();
                            Response.Headers.Add("name", Sacn_Title);
                            Response.ContentType = "application/json; charset=utf-8";
                            Response.StatusDescription = "File uploaded succesfully";
                            Response.End();
                            AdminSiteConfiguration.WriteErrorLogs(Sacn_Title + " File uploaded succesfully");
                        }
                        else
                        {
                            HttpResponse Response = System.Web.HttpContext.Current.Response;
                            Response.Clear();
                            Response.Status = "204 File already exists";
                            Response.StatusCode = 204;
                            Response.StatusDescription = "File already exists";
                            Response.End();
                            AdminSiteConfiguration.WriteErrorLogs(Sacn_Title + " File already exists.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeProfileController - SaveDocuments - " + DateTime.Now + " - " + ex.Message.ToString());
                AdminSiteConfiguration.WriteErrorLogs(ex.Message);
                HttpResponse Response = System.Web.HttpContext.Current.Response;
                Response.Clear();
                Response.ContentType = "application/json; charset=utf-8";
                Response.StatusCode = 204;
                Response.Status = "204 No Content";
                Response.StatusDescription = ex.Message;
                Response.End();
            }
        }

        [AcceptVerbs("Post")]
        public void RemoveDocuments(int EmployeeId, int Type)
        {
            try
            {
                logger.Info("HREmployeeProfileController - RemoveDocuments - " + DateTime.Now);
                var httpPostedFile = System.Web.HttpContext.Current.Request.Files["UploadFiles"];
                var fileSave = "";
                if (Type == 1)
                {
                    fileSave = System.Web.HttpContext.Current.Server.MapPath("~/UserFiles/HR_File/EmployeeDocument/" + EmployeeId);
                }
                else
                {
                    fileSave = System.Web.HttpContext.Current.Server.MapPath("~/UserFiles/HR_File/EmployeeDocument_401K/" + EmployeeId);
                }
                var fileSavePath = System.IO.Path.Combine(fileSave, httpPostedFile.FileName);
                if (System.IO.File.Exists(fileSavePath))
                {
                    System.IO.File.Delete(fileSavePath); // removed the uploaded file from the saved area
                }

                HttpResponse Response = System.Web.HttpContext.Current.Response;
                Response.Clear();
                Response.Status = "200 OK";
                Response.StatusCode = 200;
                Response.ContentType = "application/json; charset=utf-8";
                Response.StatusDescription = "File removed succesfully";
                Response.End();
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeProfileController - RemoveDocuments - " + DateTime.Now + " - " + ex.Message.ToString());
                HttpResponse Response = System.Web.HttpContext.Current.Response;
                Response.Clear();
                Response.Status = "200 OK";
                Response.StatusCode = 200;
                Response.ContentType = "application/json; charset=utf-8";
                Response.StatusDescription = "File not removed succesfully";
                Response.End();
            }
        }

        public ActionResult DownloadDocument(string DocumentFileName, int EmployeeId, int Type)
        {
            if (DocumentFileName != "" && DocumentFileName != null)
            {
                string FilePath = "";
                if (Type == 1)
                {
                    FilePath = Server.MapPath("~/UserFiles/HR_File/EmployeeDocument_401K/" + EmployeeId + "/" + DocumentFileName.Trim());
                }
                else if (Type == 2)
                {
                    FilePath = Server.MapPath("~/UserFiles/HR_File/EmployeeHealthBenefitDocument/" + EmployeeId + "/" + DocumentFileName.Trim());
                }
                else if (Type == 5)
                {
                    FilePath = Server.MapPath("~/UserFiles/HR_File/VaccineCertificate/" + EmployeeId + "/" + DocumentFileName.Trim());
                }
                else if (Type == 6)
                {
                    FilePath = Server.MapPath("~/UserFiles/HR_File/EmployeeWarning/" + EmployeeId + "/" + DocumentFileName.Trim());
                }
                else if (Type == 7)
                {
                    FilePath = Server.MapPath("~/UserFiles/HR_File/EmployeeTermination/" + EmployeeId + "/" + DocumentFileName.Trim());
                }
                else
                {
                    FilePath = Server.MapPath("~/UserFiles/HR_File/EmployeeDocument/" + EmployeeId + "/" + DocumentFileName.Trim());
                }

                if (System.IO.File.Exists(FilePath))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, DocumentFileName.Trim());
                }
            }
            return RedirectToAction("Index", "HREmployeeProfile", new { EmployeeId = EmployeeId });
        }

        public ActionResult DeleteEmployeeDocument(int DocId, int EmployeeId, string FileName, int Type)
        {
            try
            {
                HREmployeeDocument HR = new HREmployeeDocument();
                SuccessMessage = null;
                ErrorMessage = null;
                try
                {
                    logger.Info("HREmployeeProfileController - DeleteEmployeeDocument - " + DateTime.Now);
                    int ModifiedBy = _CommonRepository.getUserId(UserName);
                    HR = _hREmployeeRepository.RemoveHREmployeeDocument(DocId, EmployeeId, FileName, ModifiedBy, Type);
                    SuccessMessage = "Document Deleted Successfully.";
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message.ToString();
                    logger.Error("HREmployeeProfileController - DeleteEmployeeDocument - " + DateTime.Now + " - " + ex.Message.ToString());
                }
                return Json(new { success = SuccessMessage, Error = ErrorMessage });
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<ActionResult> HRDocuments401kInsert(int EmployeeId, int EmployeeChildId, string FileName, int rdopt)
        {
            HREmployeeRetirementInfo HR = new HREmployeeRetirementInfo();
            SuccessMessage = null;
            ErrorMessage = null;
            try
            {
                logger.Info("HREmployeeProfileController - HRDocuments401kInsert - " + DateTime.Now);
                HR.EmployeeId = EmployeeId;
                HR.EmployeeChildId = EmployeeChildId;
                HR.FileName = FileName;
                HR.OptStatus = rdopt;
                HR.CreatedBy = _CommonRepository.getUserId(UserName);
                HR.ModifiedBy = HR.CreatedBy;
                HR.EmployeeId = EmployeeId;
                HR.EmployeeChildId = EmployeeChildId;
                HR = _hREmployeeRepository.InsertHREmployeeDocuments401K(HR);
                SuccessMessage = "Document Uploaded Successfully.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("HREmployeeProfileController - HRDocuments401kInsert - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { success = SuccessMessage, Error = ErrorMessage });
        }
        #endregion Documents Tab

        #region Notes Tab
        [Authorize(Roles = "Administrator,ViewNotesHRSubModule")]
        public ActionResult HRNotesUrlDatasource(DataManagerRequest dm, int EmployeeId, int StoreId, int EmployeeChildId)
        {
            List<HRNoteViewModal> HrEPR = new List<HRNoteViewModal>();
            IEnumerable DataSource = new List<HREmployeeNotes>();
            int Count = 0;
            try
            {
                logger.Info("HREmployeeProfileController - HRNotesUrlDatasource - " + DateTime.Now);
                //Using this db class Get Web cam details by storeid.
                HrEPR = _hREmployeeRepository.GetHREmployeeNotesList(EmployeeId, StoreId, EmployeeChildId);

                DataSource = HrEPR;
                
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
                Count = DataSource.Cast<HRNoteViewModal>().Count();
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
                logger.Error("HREmployeeProfileController - HRNotesUrlDatasource - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return dm.RequiresCounts ? Json(new { result = DataSource, count = Count }) : Json(DataSource);
        }

        public async Task<ActionResult> HRNotesInsert(CRUDModel<HREmployeeNotes> Notes, int EmployeeId, int StoreId, int EmployeeChildId)
        {
            HREmployeeNotes HR = new HREmployeeNotes();
            SuccessMessage = null;
            ErrorMessage = null;
            try
            {
                logger.Info("HREmployeeProfileController - HRNotesInsert - " + DateTime.Now);
                //int? EmpId = Convert.ToInt32(Session["HREmployeeId"].ToString());
                Notes.Value.CreatedBy = _CommonRepository.getUserId(UserName);
                Notes.Value.ModifiedBy = Notes.Value.CreatedBy;
                Notes.Value.EmployeeId = EmployeeId;
                Notes.Value.EmployeeChildId = EmployeeChildId;
                HR = _hREmployeeRepository.InsertHREmployeeNotes(Notes.Value);
                SuccessMessage = "Notes Created Successfully.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("HREmployeeProfileController - HRNotesInsert - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = HR, success = SuccessMessage, Error = ErrorMessage });
        }

        public ActionResult HRNotesAddPartial()
        {
            HREmployeeNotes hRNotes = new HREmployeeNotes();
            try
            {
                logger.Info("HREmployeeProfileController - HRNotesAddPartial - " + DateTime.Now);
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeProfileController - HRNotesAddPartial - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView("_Notes", hRNotes);
        }
        #endregion Notes Tab

        #region Training History Tab
        [Authorize(Roles = "Administrator,ViewTrainingHistoryHRSubModule")]
        public ActionResult HRTrainingHistoryUrlDatasource(DataManagerRequest dm, int EmployeeId, int StoreId)
        {
            List<HREmployeeMaster> HrEPR = new List<HREmployeeMaster>();
            IEnumerable DataSource = new List<HREmployeeMaster>();
            int Count = 0;
            try
            {
                logger.Info("HREmployeeProfileController - HRTrainingHistoryUrlDatasource - " + DateTime.Now);
                //Using this db class Get Web cam details by storeid.
                HrEPR = _hREmployeeRepository.GetHREmployeeTrainingHistory(EmployeeId, StoreId);
                DataSource = HrEPR;
                
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
                Count = DataSource.Cast<HREmployeeMaster>().Count();
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
                logger.Error("HREmployeeProfileController - HRTrainingHistoryUrlDatasource - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return dm.RequiresCounts ? Json(new { result = DataSource, count = Count }) : Json(DataSource);
        }
        #endregion Training History Tab

        #region Warning Tab
        [Authorize(Roles = "Administrator,ViewWarningHRSubModule")]
        public ActionResult HRWarningUrlDatasource(int EmployeeId, int StoreId, int EmployeeChildId)
        {
            List<HRWarningViewModel> HrEPR = new List<HRWarningViewModel>();
            HRWarningList hRwarningList = new HRWarningList();
            try
            {
                logger.Info("HREmployeeProfileController - HRWarningUrlDatasource - " + DateTime.Now);
                //Using this db class Get Web cam details by storeid.
                HrEPR = _hREmployeeRepository.GetHREmployeeWarningList(EmployeeId, StoreId, EmployeeChildId);
                hRwarningList.firstwarningfile = HrEPR.Where(s => s.Warning == "First").ToList();
                hRwarningList.secondwarningfile = HrEPR.Where(s => s.Warning == "Second").ToList();
                hRwarningList.finalwarningfile = HrEPR.Where(s => s.Warning == "Final").ToList();
                return Json(new { result = hRwarningList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeProfileController - HRWarningUrlDatasource - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json(new { result = "error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult DownloadWarning(string DocumentFileName, int EmployeeId)
        {
            if (DocumentFileName != "" && DocumentFileName != null)
            {
                string FilePath = "";
                FilePath = Server.MapPath("~/UserFiles/HR_File/EmployeeWarning/" + EmployeeId + "/" + DocumentFileName.Trim());
                if (System.IO.File.Exists(FilePath))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, DocumentFileName.Trim());
                }
            }
            return RedirectToAction("Index", "HREmployeeProfile", new { EmployeeId = EmployeeId });
        }
        #endregion Warning Tab

        #region Termination Tab
        [Authorize(Roles = "Administrator,ViewTerminationHRSubModule")]
        public ActionResult HRTerminationUrlDatasource(int EmployeeId, int StoreId, int EmployeeChildId)
        {
            List<HRTerminationViewModel> HrEPR = new List<HRTerminationViewModel>();
            HRTerminationList hRterminationList = new HRTerminationList();
            try
            {
                logger.Info("HREmployeeProfileController - HRTerminationUrlDatasource - " + DateTime.Now);
                //Using this db class Get Web cam details by storeid.
                HrEPR = _hREmployeeRepository.GetHREmployeeTerminationList(EmployeeId, StoreId, EmployeeChildId);
                hRterminationList.terminationfile = HrEPR.ToList();
                return Json(new { result = hRterminationList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeProfileController - HRTerminationUrlDatasource - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json(new { result = "error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult DownloadTermination(string DocumentFileName, int EmployeeId)
        {
            if (DocumentFileName != "" && DocumentFileName != null)
            {
                string FilePath = "";
                FilePath = Server.MapPath("~/UserFiles/HR_File/EmployeeTermination/" + EmployeeId + "/" + DocumentFileName.Trim());
                if (System.IO.File.Exists(FilePath))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, DocumentFileName.Trim());
                }
            }
            return RedirectToAction("Index", "HREmployeeProfile", new { EmployeeId = EmployeeId });
        }
        #endregion Termination Tab

        //public ActionResult BindTabingView()
        //{
        //    List<TabTabItem> tabItems = new List<TabTabItem>();
        //    int store_idval = 0;
        //    if (!string.IsNullOrEmpty(Session["StoreId"].ToString()))
        //    {
        //        store_idval = Convert.ToInt32(Session["StoreId"]);
        //    }
        //    if ((Roles.IsUserInRole("Administrator") || Roles.IsUserInRole("ViewPayRatesHRSubModule")) || store_idval !=0)
        //    {
        //        tabItems.Add(new TabTabItem { Header = new TabHeader { Text = "Pay Rates" }, Content = "#PayRatesContent" });
        //    }
        //    if (Roles.IsUserInRole("Administrator") || Roles.IsUserInRole("ViewSickTimesHRSubModule"))
        //    {
        //        tabItems.Add(new TabTabItem { Header = new TabHeader { Text = "Sick Times" }, Content = "#SickTimesContent" });
        //    }
        //    if (Roles.IsUserInRole("Administrator") || Roles.IsUserInRole("ViewVacationTimesHRSubModule"))
        //    {
        //        tabItems.Add(new TabTabItem { Header = new TabHeader { Text = "Vacation Times" }, Content = "#VacationTimesContent" });
        //    }
        //    if (Roles.IsUserInRole("Administrator") || Roles.IsUserInRole("ViewInsuranceHRSubModule"))
        //    {
        //        tabItems.Add(new TabTabItem { Header = new TabHeader { Text = "Insurance" }, Content = "#InsuranceContent" });
        //    }
        //    if (Roles.IsUserInRole("Administrator") || Roles.IsUserInRole("ViewDocumentsHRSubModule"))
        //    {
        //        tabItems.Add(new TabTabItem { Header = new TabHeader { Text = "Documents" }, Content = "#DocumentsContent" });
        //    }
        //    if (Roles.IsUserInRole("Administrator") || Roles.IsUserInRole("ViewNotesHRSubModule"))
        //    {
        //        tabItems.Add(new TabTabItem { Header = new TabHeader { Text = "Notes" }, Content = "#NotesContent" });
        //    }
        //    if (Roles.IsUserInRole("Administrator") || Roles.IsUserInRole("ViewTrainingHistoryHRSubModule"))
        //    {
        //        tabItems.Add(new TabTabItem { Header = new TabHeader { Text = "Training History" }, Content = "#TrainingHistoryContent" });
        //    }
        //    //if (Roles.IsUserInRole("Administrator") || Roles.IsUserInRole("ViewWarningHRSubModule"))
        //    //{
        //    //    tabItems.Add(new TabTabItem { Header = new TabHeader { Text = "Warning" }, Content = "#WarningContent" });
        //    //}
        //    //if (Roles.IsUserInRole("Administrator") || Roles.IsUserInRole("ViewTerminationHRSubModule"))
        //    //{
        //    //    tabItems.Add(new TabTabItem { Header = new TabHeader { Text = "Termination" }, Content = "#TerminationContent" });
        //    //}
        //    ViewBag.items = tabItems;
        //    return PartialView("_ProfileTabingView");
        //}

        public ActionResult BindTabingView()
        {
            List<TabTabItem> tabItems = new List<TabTabItem>();
            int store_idval = 0;

            if (TempData["StoreId"] != null)
            {
                store_idval = Convert.ToInt32(TempData["StoreId"]);
            }
            var ModuleId = _hREmployeeRepository.GetModuleMastersId("HRSubModule");
            bool roleFlg = false;
            List<string> userRoles = new List<string>();
            try
            {
                if (!Roles.IsUserInRole("Administrator"))
                {
                    var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                    int UserTypeId = _CommonRepository.getUserTypeId(UserName);
                    if (UserTypeId > 0)
                    {
                        //this db class is check user type module Approvers
                        userRoles = _hREmployeeRepository.GetUserRoleList_BaasedOnTypeWise(UserTypeId, store_idval, ModuleId);
                    }                   
                }
                else
                {
                    roleFlg = true;
                }
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeProfileController - BindTabingView - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            if (Roles.IsUserInRole("Administrator") || userRoles.Contains("ViewPayRatesHRSubModule"))
            {
                tabItems.Add(new TabTabItem { Header = new TabHeader { Text = "Pay Rates" }, Content = "#PayRatesContent" });
            }
            if (Roles.IsUserInRole("Administrator") || userRoles.Contains("ViewSickTimesHRSubModule"))
            {
                tabItems.Add(new TabTabItem { Header = new TabHeader { Text = "Sick Times" }, Content = "#SickTimesContent" });
            }
            if (Roles.IsUserInRole("Administrator") || userRoles.Contains("ViewVacationTimesHRSubModule"))
            {
                tabItems.Add(new TabTabItem { Header = new TabHeader { Text = "Vacation Times" }, Content = "#VacationTimesContent" });
            }
            if (Roles.IsUserInRole("Administrator") || userRoles.Contains("ViewInsuranceHRSubModule"))
            {
                tabItems.Add(new TabTabItem { Header = new TabHeader { Text = "Insurance" }, Content = "#InsuranceContent" });
            }
            if (Roles.IsUserInRole("Administrator") || userRoles.Contains("ViewDocumentsHRSubModule"))
            {
                tabItems.Add(new TabTabItem { Header = new TabHeader { Text = "Documents" }, Content = "#DocumentsContent" });
            }
            if (Roles.IsUserInRole("Administrator") || userRoles.Contains("ViewNotesHRSubModule"))
            {
                tabItems.Add(new TabTabItem { Header = new TabHeader { Text = "Notes" }, Content = "#NotesContent" });
            }
            if (Roles.IsUserInRole("Administrator") || userRoles.Contains("ViewTrainingHistoryHRSubModule"))
            {
                tabItems.Add(new TabTabItem { Header = new TabHeader { Text = "Training History" }, Content = "#TrainingHistoryContent" });
            }
            //if (Roles.IsUserInRole("Administrator") || Roles.IsUserInRole("ViewWarningHRSubModule"))
            //{
            //    tabItems.Add(new TabTabItem { Header = new TabHeader { Text = "Warning" }, Content = "#WarningContent" });
            //}
            //if (Roles.IsUserInRole("Administrator") || Roles.IsUserInRole("ViewTerminationHRSubModule"))
            //{
            //    tabItems.Add(new TabTabItem { Header = new TabHeader { Text = "Termination" }, Content = "#TerminationContent" });
            //}
            ViewBag.items = tabItems;
            return PartialView("_ProfileTabingView");
        }
    }
}