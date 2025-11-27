using EntityModels.Models;
using Repository.IRepository;
using Repository;
using Syncfusion.EJ2.Base;
using SynthesisViewModal.HRViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Extensions.Logging;
using NLog;
using System.Web.Razor.Tokenizer;
using EntityModels.HRModels;
using System.Web.Security;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;
using System.Globalization;
using DocumentFormat.OpenXml.Wordprocessing;
using Aspose.Pdf.Operators;
using System.IO;
using Syncfusion.EJ2.Popups;
using Syncfusion.EJ2.GridExport;
using Syncfusion.Pdf;
using System.Threading.Tasks;
using RestSharp;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;

namespace SysnthesisRepo.Controllers
{
    public class HREmployeeController : Controller
    {
        private readonly IHREmployeeRepository _hREmployeeRepository;
        private readonly ICommonRepository _CommonRepository;
        private readonly ICompaniesRepository _CompaniesRepository;
        private readonly IHRDepartmentRepository _hRDepartmentRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        string SuccessMessage = string.Empty;
        string ErrorMessage = string.Empty;
        string UserName = System.Web.HttpContext.Current.User.Identity.Name;
        public HREmployeeController()
        {
            this._hREmployeeRepository = new HREmployeeRepository(new DBContext());
            this._CommonRepository = new CommonRepository(new DBContext());
            this._CompaniesRepository = new CompaniesRepository(new DBContext());
            this._hRDepartmentRepository = new HRDepartmentRepository(new DBContext());
        }
        // GET: HREmployee
        [Authorize(Roles = "Administrator,ViewEmployeeListHRModule")]
        public ActionResult Index()
        {
            ViewBag.Title = "Employees List - Synthesis";

            List<DialogDialogButton> btnSSN = new List<DialogDialogButton>() { };
            btnSSN.Add(new DialogDialogButton() { Click = "GetSSNDetail", ButtonModel = new promptButtonModel() { content = "Validate", isPrimary = true } });
            btnSSN.Add(new DialogDialogButton() { Click = "SSNpromptBtnClick", ButtonModel = new promptButtonModel() { content = "Cancel" } });

            ViewBag.PromptButtonSSN = btnSSN;

            List<DialogDialogButton> filterButtons = new List<DialogDialogButton>() { };
            filterButtons.Add(new DialogDialogButton() { Click = "applyFilters", ButtonModel = new promptButtonModel() { content = "Apply Filter", isPrimary = true } });

            ViewBag.FilterButtons = filterButtons;

            //if ((int)Session["UserId"] != 1089)
            //{
            //    if (Session["StoreId"] != null)
            //    {
            //        if (!string.IsNullOrEmpty(Session["StoreId"].ToString()))
            //        {

            //            int store_idval = Convert.ToInt32(Session["StoreId"]);
            //            ViewBag.Storeidvalue = store_idval;
            //        }
            //    }
            //}
            return View();
        }

        public ActionResult UrlDatasource(DataManagerRequest dm, string filterss)
        {
            EmployeeFilterModel filters = new EmployeeFilterModel();
            if (filterss != null)
            {
                filters = JsonConvert.DeserializeObject<EmployeeFilterModel>(filterss);
            }
            List<HREmployeeViewModal> HrEVm = new List<HREmployeeViewModal>();
            IEnumerable DataSource = new List<HREmployeeViewModal>();
            int Count = 0;
            try
            {
                logger.Info("HREmployeeController - UrlDatasource - " + DateTime.Now);
                Session["EmployeeListFilter"] = null;
                int store_idval = 0;
                //if (Session["StoreId"] != null)
                //{
                if (!string.IsNullOrEmpty(Session["StoreId"].ToString()))
                {

                    store_idval = Convert.ToInt32(Session["StoreId"]);
                }
                //}
                int userid = (int)Session["UserId"];
                //Using this db class Get Web cam details by storeid.
                HrEVm = _hREmployeeRepository.GetHREmployeeList(store_idval, userid);


                var filteredData = HrEVm.AsQueryable();

                var predicates = dm.Where?.SelectMany(w => w.predicates ?? new List<WhereFilter>()).ToList();
                if (predicates != null && predicates.Count > 0)
                {
                    var isTrainingFilter = predicates.FirstOrDefault(w => w.Field == "IsTraningCompleted");

                    bool? trainingFilterValue = null;

                    if (isTrainingFilter != null && bool.TryParse(isTrainingFilter.value.ToString(), out bool parsedValue))
                    {
                        trainingFilterValue = parsedValue;
                        predicates.Remove(isTrainingFilter);
                    }

                    // Apply training completion filter based on LastSlidename
                    if (trainingFilterValue != null)
                    {
                        if (trainingFilterValue == true)
                        {
                            filteredData = filteredData.Where(x => (x.LastSlidename == ".q14Data" || x.LastSlidename == ".q14DataSp"));
                        }
                        else
                        {
                            filteredData = filteredData.Where(x => (x.LastSlidename != ".q14Data" && x.LastSlidename != ".q14DataSp"));
                        }
                    }
                }

                if (filters != null)
                {
                    //filters = Session["EmployeeListFilter"] as EmployeeFilterModel;
                    if (filters.HiringFromDate == "month/day/year")
                    {
                        filters.HiringFromDate = null;
                    }
                    if (filters.HiringToDate == "month/day/year")
                    {
                        filters.HiringToDate = null;
                    }

                    DateTime? fromDate = null;
                    DateTime? toDate = null;
                    string dateFormat = "MM/dd/yyyy";

                    if (!string.IsNullOrEmpty(filters.HiringFromDate))
                    {
                        if (DateTime.TryParseExact(filters.HiringFromDate, dateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime parsedFromDate))
                        {
                            fromDate = parsedFromDate;
                        }
                    }

                    if (!string.IsNullOrEmpty(filters.HiringToDate))
                    {
                        if (DateTime.TryParseExact(filters.HiringToDate, dateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime parsedToDate))
                        {
                            toDate = parsedToDate;
                        }
                    }



                    // Apply date filters
                    if (fromDate.HasValue)
                    {
                        filteredData = filteredData.Where(e => e.HireDate.HasValue && e.HireDate.Value >= fromDate.Value);
                    }

                    if (toDate.HasValue)
                    {
                        filteredData = filteredData.Where(e => e.HireDate.HasValue && e.HireDate.Value <= toDate.Value);
                    }

                    // Apply training status filters
                    if (filters.TStatusComplete || filters.TStatusPending)
                    {
                        if (filters.TStatusComplete && filters.TStatusPending)
                        {
                            // No need to apply additional filter, both are selected
                        }
                        else
                        {
                            //filteredData = filteredData.Where(e => (filters.TStatusComplete && e.IsTraningCompleted) || (filters.TStatusPending && !e.IsTraningCompleted));
                            if (filters.TStatusComplete == true)
                            {
                                filteredData = filteredData.Where(x => (x.LastSlidename == ".q14Data" || x.LastSlidename == ".q14DataSp"));
                            }
                            else
                            {
                                filteredData = filteredData.Where(x => (x.LastSlidename != ".q14Data" && x.LastSlidename != ".q14DataSp"));
                            }
                        }
                    }

                    // Apply employee status filters
                    if (filters.EStatusActive || filters.EStatusDeceased || filters.EStatusOnTerminated || filters.EStatusResigned)
                    {
                        List<string> statusList = new List<string>();

                        if (filters.EStatusActive) { statusList.Add("Active"); }
                        if (filters.EStatusDeceased) { statusList.Add("Deceased"); }
                        if (filters.EStatusOnTerminated) { statusList.Add("Terminated"); }
                        if (filters.EStatusResigned) { statusList.Add("Resigned"); }

                        filteredData = filteredData.Where(e => statusList.Contains(e.StatusName));
                    }

                    // Apply 401(K) plan status filters
                    if (filters.PStatusNotUsed || filters.PStatusOptIn || filters.PStatusOptOut)
                    {
                        List<string> optInList = new List<string>();

                        if (filters.PStatusNotUsed) { optInList.Add("Not Used"); }
                        if (filters.PStatusOptIn) { optInList.Add("Opt-In"); }
                        if (filters.PStatusOptOut) { optInList.Add("Opt-Out"); }

                        filteredData = filteredData.Where(e => optInList.Contains(e.OptStatusValue));
                    }

                    // Apply medical insurance filters
                    if (filters.MIStatusNotUsed == true || filters.MIStatusUsed == true)
                    {
                        if (filters.MIStatusNotUsed == true && filters.MIStatusUsed == true)
                        {
                            // No need to apply additional filter, both are selected
                        }
                        else if (filters.MIStatusNotUsed == true)
                        {
                            filteredData = filteredData.Where(e => e.HelathBenefitUsed == "Not Used");
                        }
                        else if (filters.MIStatusUsed == true)
                        {
                            filteredData = filteredData.Where(e => e.HelathBenefitUsed == "Used");
                        }
                    }

                    // Apply department filter
                    filters.DepartmentId = filters.DepartmentId != null ? filters.DepartmentId : 0;
                    if (filters.DepartmentId != 0)
                    {
                        filteredData = filteredData.Where(e => e.DepartmentId == filters.DepartmentId.Value);
                    }
                }

                DataSource = filteredData;
                DataOperations operation = new DataOperations();
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    string search = dm.Search[0].Key.ToString().Trim();
                    bool? trainingSearchValue = null;
                    if (search.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                    {
                        trainingSearchValue = true;
                    }
                    else if (search.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                    {
                        trainingSearchValue = false;
                    }

                    DataSource = filteredData.ToList().Where(x => x.FullName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 || x.EmployeeUserName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 || x.sDateofBirth.Contains(search) || x.DepartmentName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 || x.SSN.Contains(search)
                    || x.OfficeEmployeeID.ToString().Contains(search) || x.NickName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 || x.sHireDate.Contains(search) || x.OptStatusValue.Contains(search) || x.HelathBenefitUsed.Contains(search) || (trainingSearchValue != null && x.IsTraningCompleted == trainingSearchValue)).ToList();
                }
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                }
                if (predicates != null && predicates.Count > 0) //Filtering
                {
                    DataSource = operation.PerformFiltering(DataSource, predicates, dm.Where[0].Operator);
                }
                Count = DataSource.Cast<HREmployeeViewModal>().Count();
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
                logger.Error("HREmployeeController - UrlDatasource - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return dm.RequiresCounts ? Json(new { result = DataSource, count = Count }) : Json(DataSource);
        }

        public ActionResult Create()
        {
            try
            {
                logger.Info("HREmployeeController - Create - " + DateTime.Now);
                if (_CommonRepository.getUserId(UserName) != 0)
                {

                    ViewBag.Gender = new SelectList(new[] { new EnumDropDown { value = 0, text = "-- Select Gender --" } }
                    .Concat(from Gender e in Enum.GetValues(typeof(Gender)) select new EnumDropDown { value = (int)e, text = e.ToString() }), "value", "text");

                    ViewBag.MaritalStatus = new SelectList(new[] { new EnumDropDown { value = 0, text = "-- Select Marital Status --" } }
                    .Concat(from MaritalStatus e in Enum.GetValues(typeof(MaritalStatus)) select new EnumDropDown { value = (int)e, text = e.ToString() }), "value", "text");

                    ViewBag.LanguageId = new SelectList(new[] { new EnumDropDown { value = 0, text = "-- Select Language --" } }
                    .Concat(from Language e in Enum.GetValues(typeof(Language)) select new EnumDropDown { value = (int)e, text = e.ToString() }), "value", "text");

                    ViewBag.State = new SelectList(new[] { new EnumDropDown { value = 0, text = "-- Select State --" } }
                    .Concat(from State e in Enum.GetValues(typeof(State)) select new EnumDropDown { value = (int)e, text = e.ToString() }), "value", "text");

                    ViewBag.EthnicityId = new SelectList(new[] { new HREthnicityMaster { EthnicityId = 0, EthnicityName = "-- Select Race/Ethnicity --" } }
                    .Concat(_hREmployeeRepository.GetHREthnicityList()), "EthnicityId", "EthnicityName");

                    ViewBag.CheckDobdate = DateTime.Now.AddDays(-1);

                    Session.Remove("PSChildData");
                    Session.Add("PSChildData", new List<HREmployeeChildViewModal>());
                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Login");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult InitialChildFormValue()
        {
            try
            {
                logger.Info("HREmployeeController - InitialChildFormValue - " + DateTime.Now);
                ModelState.Clear();
                HREmployeeChildViewModal obj = new HREmployeeChildViewModal();
                SetDropDown();
                obj.SrNo = GetMaxSrNo();
                obj.sHireDate = DateTime.Now.ToString("MM/dd/yyyy");
                obj.sTerminationDate = DateTime.Now.ToString("MM/dd/yyyy");
                return PartialView("_HREmployeeChild", obj);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int GetMaxSrNo()
        {
            ICollection<HREmployeeChildViewModal> temp = (ICollection<HREmployeeChildViewModal>)Session["PSChildData"];

            return (int)(((temp != null) && (temp.Count > 0)) ? ((temp.Max(m => m.SrNo != null ? m.SrNo : 0)) + 1) : 1);
        }

        public ActionResult PartialIndex()
        {
            var list = (ICollection<HREmployeeChildViewModal>)Session["PSChildData"];
            return PartialView("_EmployeeChildIndex", list.OrderBy(o => o.SrNo));
        }

        private bool ValidateEmployeeChild(HREmployeeChildViewModal obj)
        {
            bool flag = true;
            if (obj.StatusName == null)
            {
                ModelState.AddModelError("Status", "Select Status");
                flag = false;
            }
            if (obj.StoreId == null || obj.StoreId == 0)
            {
                ModelState.AddModelError("StoreId", "Select Store");
                flag = false;
            }
            if (obj.DepartmentId == 0 || obj.DepartmentId == null)
            {
                ModelState.AddModelError("DepartmentId", "Select Department");
                flag = false;
            }
            if (obj.EmployeementTypeStatusName == "0" || obj.EmployeementTypeStatusName == null)
            {
                ModelState.AddModelError("EmployeementTypeStatus", "Select Employment Type Status");
                flag = false;
            }
            if (String.IsNullOrEmpty(obj.OfficeEmployeeID))
            {
                ModelState.AddModelError("OfficeEmployeeID", "Employee Id is Required");
                flag = false;
            }

            return flag;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEmployeeChild(HREmployeeChildViewModal myformdatadoc)
        {
            if (ValidateEmployeeChild(myformdatadoc))
            {
                if (ModelState.IsValid)
                {
                    ICollection<HREmployeeChildViewModal> temp = (ICollection<HREmployeeChildViewModal>)Session["PSChildData"];
                    if (temp.Any(a => a.SrNo == myformdatadoc.SrNo))
                    {
                        HREmployeeChildViewModal PsChild = temp.First(a => a.SrNo == myformdatadoc.SrNo);
                        temp.Remove(PsChild);
                    }
                    myformdatadoc.StoreName = _CompaniesRepository.GetStoreMastersbyID(Convert.ToInt32(myformdatadoc.StoreId)).Name;
                    myformdatadoc.DepartmentName = _hRDepartmentRepository.GetHRDepartmentById(Convert.ToInt32(myformdatadoc.DepartmentId)).DepartmentName;
                    if (myformdatadoc.Status == Status.Active) { myformdatadoc.sTerminationDate = null; }
                    temp.Add(myformdatadoc);
                    Session["PSChildData"] = temp;
                    ModelState.Clear();
                    return RedirectToAction("InitialChildFormValue");
                }
            }
            SetDropDown();
            return PartialView("_HREmployeeChild", myformdatadoc);
        }

        public ActionResult EditChild(int SrNo)
        {
            ICollection<HREmployeeChildViewModal> temp = (ICollection<HREmployeeChildViewModal>)Session["PSChildData"];
            HREmployeeChildViewModal PsChild = temp.First(a => a.SrNo == SrNo);

            if (PsChild == null)
            {
                return HttpNotFound();
            }

            Session["PSChildData"] = temp;
            SetDropDown();
            return PartialView("_HREmployeeChild", PsChild);
        }
        public ActionResult DeleteChild(int SrNo)
        {
            ICollection<HREmployeeChildViewModal> temp = (ICollection<HREmployeeChildViewModal>)Session["PSChildData"];
            temp.Remove(temp.First(a => a.SrNo == SrNo));
            Session["PSChildData"] = temp;
            ModelState.Clear();
            AutoSrNo();
            SetDropDown();
            return RedirectToAction("InitialChildFormValue");
        }
        public ActionResult AutoSrNo()
        {
            ICollection<HREmployeeChildViewModal> temp = (ICollection<HREmployeeChildViewModal>)Session["PSChildData"];
            if ((temp != null) && (temp.Count > 0))
            {
                int i = 1;
                foreach (HREmployeeChildViewModal child in temp.OrderBy(o => o.SrNo))
                    child.SrNo = i++;

                Session["PSChildData"] = temp;
            }
            return RedirectToAction("InitialChildForm");
        }

        public void SetDropDown()
        {
            List<StoreMaster> list = new List<StoreMaster>();
            list = _CompaniesRepository.GetAllStoreMasters().Where(a => a.IsActive == true && a.StoreId != 13).OrderBy(a => a.StoreId).ToList();

            ViewBag.StoreId = new SelectList(new[] { new StoreMaster { StoreId = 0, Name = "-- Select Store --" } }
                   .Concat(list.ToList()), "StoreId", "Name");

            ViewBag.DepartmentId = new SelectList(new[] { new HRDepartmentViewModel { DepartmentId = 0, DepartmentName = "-- Select Department --" } }
                   .Concat(_hRDepartmentRepository.GetHRDepartmentList().Where(a => a.IsActive == true).OrderBy(a => a.DepartmentName).ToList()), "DepartmentId", "DepartmentName");


            ViewBag.EmployeementTypeStatus = new SelectList(new[] { new EnumDropDown { value = 0, text = "-- Select Employment Type --" } }
            .Concat(from EmployeementTypeStatus e in Enum.GetValues(typeof(EmployeementTypeStatus))
                    let displayAttribute = typeof(EmployeementTypeStatus).GetField(e.ToString()).GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute
                    select new EnumDropDown { value = (int)e, text = displayAttribute != null ? displayAttribute.Name : e.ToString() }), "value", "text");

            ViewBag.Status = new SelectList(new[] { new EnumDropDown { value = 0, text = "-- Select Employment Status --" } }
            .Concat(from Status e in Enum.GetValues(typeof(Status)) select new EnumDropDown { value = (int)e, text = e.ToString() }), "value", "text");

        }

        [HttpPost]
        public ActionResult Create(HREmployeeMaster hR)
        {
            try
            {
                logger.Info("HREmployeeController - Create(post) - " + DateTime.Now);
                hR.SSN = hR.SSN.Contains("_") ? "" : hR.SSN;
                hR.MobileNo = hR.MobileNo.Contains("_") ? "" : hR.MobileNo.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                hR.Phone = hR.Phone.Contains("_") ? "" : hR.Phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");

                var objEmpNameExist = _hREmployeeRepository.CheckHrEmployeeIfExist(hR.EmployeeUserName, hR.EmployeeId);
                if (objEmpNameExist != null)
                {
                    ModelState.AddModelError("EmployeeUserName", "Employee Name Already Exist");
                    return View();
                }

                ICollection<HREmployeeChildViewModal> EmployeeModelChilds = null;

                if ((((ICollection<HREmployeeChildViewModal>)Session["PSChildData"]) != null) && (((ICollection<HREmployeeChildViewModal>)Session["PSChildData"]).Count > 0))
                    EmployeeModelChilds = (ICollection<HREmployeeChildViewModal>)Session["PSChildData"];
                else
                    ModelState.AddModelError("Message", "Please Insert Employee Information.......");

                //ModelState.Remove("MobileNo");
                //ModelState.Remove("Phone");
                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");
                if (ModelState.IsValid)
                {
                    try
                    {
                        hR.LoginUserId = _CommonRepository.getUserId(UserName);
                        hR.Password = hR.EmployeeUserName;
                        hR.ConfirmPassword = hR.EmployeeUserName;
                        hR.FullSSN = hR.SSN;
                        hR.SSN = hR.SSN.Replace(" ", "");
                        hR.SSN = "XXX-XX-" + hR.SSN.Substring(hR.SSN.Length - 4, 4);
                        if (!string.IsNullOrEmpty(hR.sDateofBirth))
                        {
                            hR.DateofBirth = DateTime.ParseExact(hR.sDateofBirth, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                        }
                        hR.CreatedBy = _CommonRepository.getUserId(UserName);
                        int EmployeeID = _hREmployeeRepository.InsertEmployeeMaster(hR);
                        int EmployeeChildID = 0;
                        if (EmployeeID != 0)
                        {
                            List<HREmployeeChild> li = new List<HREmployeeChild>();
                            foreach (var item in EmployeeModelChilds)
                            {
                                HREmployeeChild em = new HREmployeeChild();
                                em.EmployeeId = EmployeeID;
                                em.SrNo = item.SrNo;
                                em.OfficeEmployeeID = item.OfficeEmployeeID;
                                if (!string.IsNullOrEmpty(item.sHireDate))
                                {
                                    em.HireDate = DateTime.ParseExact(item.sHireDate, "MM/dd/yyyy", CultureInfo.InvariantCulture); ;
                                }
                                if (item.Status != Status.Active)
                                {
                                    if (string.IsNullOrEmpty(item.sTerminationDate))
                                    {
                                        em.TerminationDate = null;
                                    }
                                    else
                                    {
                                        em.TerminationDate = DateTime.ParseExact(item.sTerminationDate, "MM/dd/yyyy", CultureInfo.InvariantCulture); ;
                                    }
                                }
                                else
                                {
                                    em.TerminationDate = null;
                                }
                                em.Status = item.Status;
                                em.StoreId = item.StoreId;
                                em.DepartmentId = item.DepartmentId;
                                em.EmployeementTypeStatus = item.EmployeementTypeStatus;
                                em.ModifiedBy = _CommonRepository.getUserId(UserName);
                                li.Add(em);
                            }
                            EmployeeChildID = _hREmployeeRepository.InsertEmployeeChilde(li);
                        }
                        TempData["ToastrMessage"] = new ToastModel
                        {
                            Title = "Success!",
                            Content = "Employee created successfully",
                            CssClass = "e-toast-success",
                            Icon = "e-success toast-icons"
                        };
                    }
                    catch (Exception ex)
                    {
                        logger.Error("HREmployeeController - Create - " + DateTime.Now + " - " + ex.Message.ToString());
                    }
                    return RedirectToAction("Index");
                }

                ViewBag.Gender = new SelectList(new[] { new EnumDropDown { value = 0, text = "-- Select Gender --" } }
                    .Concat(from Gender e in Enum.GetValues(typeof(Gender)) select new EnumDropDown { value = (int)e, text = e.ToString() }), "value", "text");

                ViewBag.MaritalStatus = new SelectList(new[] { new EnumDropDown { value = 0, text = "-- Select Marital Status --" } }
                .Concat(from MaritalStatus e in Enum.GetValues(typeof(MaritalStatus)) select new EnumDropDown { value = (int)e, text = e.ToString() }), "value", "text");

                ViewBag.LanguageId = new SelectList(new[] { new EnumDropDown { value = 0, text = "-- Select Language --" } }
                .Concat(from Language e in Enum.GetValues(typeof(Language)) select new EnumDropDown { value = (int)e, text = e.ToString() }), "value", "text");

                ViewBag.State = new SelectList(new[] { new EnumDropDown { value = 0, text = "-- Select State --" } }
                .Concat(from State e in Enum.GetValues(typeof(State)) select new EnumDropDown { value = (int)e, text = e.ToString() }), "value", "text");

                ViewBag.EthnicityId = new SelectList(new[] { new HREthnicityMaster { EthnicityId = 0, EthnicityName = "-- Select Race/Ethnicity --" } }
                .Concat(_hREmployeeRepository.GetHREthnicityList()), "EthnicityId", "EthnicityName");
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeController - Create(PostError) - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View();
        }

        public ActionResult Edit(int? EmployeeId)
        {
            try
            {
                logger.Info("HREmployeeController - Edit - " + DateTime.Now);
                HREmployeeMaster hR = _hREmployeeRepository.GetHREmployeeMaster(Convert.ToInt32(EmployeeId));
                hR.sDateofBirth = (hR.DateofBirth.ToString() == "" ? "" : Convert.ToDateTime(hR.DateofBirth).ToString("MM/dd/yyyy"));
                hR.SSN = hR.FullSSN;
                Session.Remove("PSChildData");

                List<HREmployeeChild> hRchild = _hREmployeeRepository.GetHREmployeeChild(Convert.ToInt32(EmployeeId));
                var partyData = hRchild.Select(m => new HREmployeeChildViewModal
                {
                    EmployeeChildId = m.EmployeeChildId,
                    SrNo = m.SrNo,
                    EmployeeId = m.EmployeeId,
                    sHireDate = (m.HireDate.ToString() == "" ? "" : Convert.ToDateTime(m.HireDate).ToString("MM/dd/yyyy").Replace("-", "/")),
                    HireDate = m.HireDate,
                    TerminationDate = m.TerminationDate,
                    sTerminationDate = (m.TerminationDate.ToString() == "" ? "" : Convert.ToDateTime(m.TerminationDate).ToString("MM/dd/yyyy").Replace("-", "/")),
                    StoreId = m.StoreId,
                    DepartmentId = m.DepartmentId,
                    Status = m.Status,
                    EmployeementTypeStatus = m.EmployeementTypeStatus,
                    OfficeEmployeeID = m.OfficeEmployeeID,
                    StoreName = _CompaniesRepository.GetStoreMastersbyID(Convert.ToInt32(m.StoreId)).Name,
                    DepartmentName = _hRDepartmentRepository.GetHRDepartmentById(Convert.ToInt32(m.DepartmentId)).DepartmentName

                }).ToList();


                ViewBag.Gender = new SelectList(new[] { new EnumDropDown { value = 0, text = "-- Select Gender --" } }
                    .Concat(from Gender e in Enum.GetValues(typeof(Gender)) select new EnumDropDown { value = (int)e, text = e.ToString() }), "value", "text");

                ViewBag.MaritalStatus = new SelectList(new[] { new EnumDropDown { value = 0, text = "-- Select Marital Status --" } }
                .Concat(from MaritalStatus e in Enum.GetValues(typeof(MaritalStatus)) select new EnumDropDown { value = (int)e, text = e.ToString() }), "value", "text");

                ViewBag.LanguageId = new SelectList(new[] { new EnumDropDown { value = 0, text = "-- Select Language --" } }
                .Concat(from Language e in Enum.GetValues(typeof(Language)) select new EnumDropDown { value = (int)e, text = e.ToString() }), "value", "text");

                ViewBag.State = new SelectList(new[] { new EnumDropDown { value = 0, text = "-- Select State --" } }
                .Concat(from State e in Enum.GetValues(typeof(State)) select new EnumDropDown { value = (int)e, text = e.ToString() }), "value", "text");

                ViewBag.EthnicityId = new SelectList(new[] { new HREthnicityMaster { EthnicityId = 0, EthnicityName = "-- Select Race/Ethnicity --" } }
                .Concat(_hREmployeeRepository.GetHREthnicityList()), "EthnicityId", "EthnicityName");

                ViewBag.CheckDobdate = DateTime.Now.AddDays(-1);

                Session.Add("PSChildData", partyData);
                return View(hR);

            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeController - Edit - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View();
        }

        [HttpPost]
        public ActionResult Edit(HREmployeeMaster hR)
        {
            try
            {
                logger.Info("HREmployeeController - Edit(Post) - " + DateTime.Now);
                hR.SSN = hR.SSN.Contains("_") ? "" : hR.SSN;
                hR.MobileNo = hR.MobileNo.Contains("_") ? "" : hR.MobileNo.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                hR.Phone = hR.Phone.Contains("_") ? "" : hR.Phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");

                var objEmpNameExist = _hREmployeeRepository.CheckHrEmployeeIfExist(hR.EmployeeUserName, hR.EmployeeId);
                if (objEmpNameExist != null)
                {
                    ModelState.AddModelError("EmployeeUserName", "Employee Name Already Exist");
                    return View();
                }

                ICollection<HREmployeeChildViewModal> EmployeeModelChilds = null;

                if ((((ICollection<HREmployeeChildViewModal>)Session["PSChildData"]) != null) && (((ICollection<HREmployeeChildViewModal>)Session["PSChildData"]).Count > 0))
                    EmployeeModelChilds = (ICollection<HREmployeeChildViewModal>)Session["PSChildData"];
                else
                    ModelState.AddModelError("Message", "Please Insert Employee Information.......");

                ModelState.Remove("MobileNo");
                ModelState.Remove("Phone");
                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");
                if (ModelState.IsValid)
                {
                    try
                    {
                        hR.LoginUserId = _CommonRepository.getUserId(UserName);
                        hR.Password = hR.EmployeeUserName;
                        hR.ConfirmPassword = hR.EmployeeUserName;
                        hR.FullSSN = hR.SSN;
                        hR.SSN = hR.SSN.Replace(" ", "");
                        hR.SSN = "XXX-XX-" + hR.SSN.Substring(hR.SSN.Length - 4, 4);
                        if (!string.IsNullOrEmpty(hR.sDateofBirth))
                        {
                            hR.DateofBirth = DateTime.ParseExact(hR.sDateofBirth, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                        }
                        hR.ModifiedBy = _CommonRepository.getUserId(UserName);
                        int EmployeeID = _hREmployeeRepository.InsertEmployeeMaster(hR);
                        int EmployeeChildID = 0;
                        if (EmployeeID != 0)
                        {
                            List<HREmployeeChild> li = new List<HREmployeeChild>();
                            foreach (var item in EmployeeModelChilds)
                            {
                                HREmployeeChild em = new HREmployeeChild();
                                em.EmployeeId = EmployeeID;
                                em.SrNo = item.SrNo;
                                em.OfficeEmployeeID = item.OfficeEmployeeID;
                                if (!string.IsNullOrEmpty(item.sHireDate))
                                {
                                    em.HireDate = DateTime.ParseExact(item.sHireDate, "MM/dd/yyyy", CultureInfo.InvariantCulture); ;
                                }
                                if (item.Status != Status.Active)
                                {
                                    if (string.IsNullOrEmpty(item.sTerminationDate))
                                    {
                                        em.TerminationDate = null;
                                    }
                                    else
                                    {
                                        em.TerminationDate = DateTime.ParseExact(item.sTerminationDate, "MM/dd/yyyy", CultureInfo.InvariantCulture); ;
                                    }
                                }
                                else
                                {
                                    em.TerminationDate = null;
                                }
                                em.Status = item.Status;
                                em.StoreId = item.StoreId;
                                em.DepartmentId = item.DepartmentId;
                                em.EmployeementTypeStatus = item.EmployeementTypeStatus;
                                em.ModifiedBy = _CommonRepository.getUserId(UserName);
                                em.CreatedBy = _CommonRepository.getUserId(UserName);
                                li.Add(em);
                            }
                            EmployeeChildID = _hREmployeeRepository.InsertEmployeeChilde(li);
                            int id = _hREmployeeRepository.DeleteChildExtraRecode(EmployeeID, li.Max(s => s.SrNo));
                        }
                        TempData["ToastrMessage"] = new ToastModel
                        {
                            Title = "Success!",
                            Content = "Employee record updated successfully",
                            CssClass = "e-toast-success",
                            Icon = "e-success toast-icons"
                        };
                    }
                    catch (Exception ex)
                    {
                        logger.Error("HREmployeeController - Edit(Post-1) - " + DateTime.Now + " - " + ex.Message.ToString());
                    }
                    return RedirectToAction("Index");
                }

                ViewBag.Gender = new SelectList(new[] { new EnumDropDown { value = 0, text = "-- Select Gender --" } }
                    .Concat(from Gender e in Enum.GetValues(typeof(Gender)) select new EnumDropDown { value = (int)e, text = e.ToString() }), "value", "text");

                ViewBag.MaritalStatus = new SelectList(new[] { new EnumDropDown { value = 0, text = "-- Select Marital Status --" } }
                .Concat(from MaritalStatus e in Enum.GetValues(typeof(MaritalStatus)) select new EnumDropDown { value = (int)e, text = e.ToString() }), "value", "text");

                ViewBag.LanguageId = new SelectList(new[] { new EnumDropDown { value = 0, text = "-- Select Language --" } }
                .Concat(from Language e in Enum.GetValues(typeof(Language)) select new EnumDropDown { value = (int)e, text = e.ToString() }), "value", "text");

                ViewBag.State = new SelectList(new[] { new EnumDropDown { value = 0, text = "-- Select State --" } }
                .Concat(from State e in Enum.GetValues(typeof(State)) select new EnumDropDown { value = (int)e, text = e.ToString() }), "value", "text");

                ViewBag.EthnicityId = new SelectList(new[] { new HREthnicityMaster { EthnicityId = 0, EthnicityName = "-- Select Race/Ethnicity --" } }
                .Concat(_hREmployeeRepository.GetHREthnicityList()), "EthnicityId", "EthnicityName");
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeController - Edit(Post-2) - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View();
        }

        [HttpPost]
        public ActionResult Delete(int EmployeeId)
        {
            try
            {
                logger.Info("HREmployeeController - Delete - " + DateTime.Now);
                int ModifiedBy = _CommonRepository.getUserId(UserName);

                // Call your repository method to delete the employee record
                bool isDeleted = _hREmployeeRepository.DeleteEmployee(EmployeeId, ModifiedBy);

                if (isDeleted)
                {
                    // If the record is deleted successfully, return a success response
                    return Json(new { success = true, message = "Employee deleted successfully" });
                }
                else
                {
                    // If the record is not deleted, return an error response
                    return Json(new { success = false, message = "Failed to delete employee record" });
                }
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeController - Delete - " + DateTime.Now + " - " + ex.Message.ToString());
                // Handle any exceptions that occur during the deletion process
                return Json(new { success = false, message = "An error occurred while deleting the employee record" });
            }
        }
        [HttpPost]
        public ActionResult ResetPassword(int EmployeeId)
        {
            try
            {
                logger.Info("HREmployeeController - ResetPassword - " + DateTime.Now);
                int ModifiedBy = _CommonRepository.getUserId(UserName);
                bool IsPassword = _hREmployeeRepository.UpdateEmployeePassword(EmployeeId, ModifiedBy);
                if (IsPassword)
                {
                    // If the record is deleted successfully, return a success response
                    return Json(new { success = true, message = "Employee Password Reset successfully" });
                }
                else
                {
                    // If the record is not deleted, return an error response
                    return Json(new { success = false, message = "Failed to reset employee password" });
                }
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeController - ResetPassword - " + DateTime.Now + " - " + ex.Message.ToString());
                // Handle any exceptions that occur during the deletion process
                return Json(new { success = false, message = "An error occurred while changing the employee password" });
            }
        }

        public ActionResult DownLoadFile(string TraningFilePath)
        {
            try
            {
                logger.Info("HREmployeeController - DownLoadFile - " + DateTime.Now);
                if (_CommonRepository.getUserId(UserName) != 0)
                {
                    string NewFilePath = Server.MapPath("~/UserFiles/HR_File/Certificates/");
                    if (Directory.Exists(Server.MapPath("~/UserFiles/HR_File/Certificates/")))
                    {
                        string FilePath = NewFilePath + TraningFilePath.Trim();
                        if (System.IO.File.Exists(FilePath))
                        {
                            byte[] fileBytes = System.IO.File.ReadAllBytes(FilePath);
                            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, TraningFilePath.Trim());
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
                logger.Error("HREmployeeController - DownloadFile - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        //Keval 10/05/2024
        public async Task<ActionResult> ExcelExport(string gridModel, string filterss)
        {
            EmployeeFilterModel filters = new EmployeeFilterModel();
            if (filterss != null)
            {
                filters = JsonConvert.DeserializeObject<EmployeeFilterModel>(filterss);
            }

            List<HREmployeeViewModal> BindData = new List<HREmployeeViewModal>();

            DBContext db1 = new DBContext();
            var StoreId = Convert.ToInt32(Session["storeid"]);
            var userid = Convert.ToInt32(Session["UserId"]);
            BindData = _hREmployeeRepository.GetHREmployeeList(StoreId, userid);
            var filteredData = BindData.AsQueryable();


            if (filters != null)
            {
                //filters = Session["EmployeeListFilter"] as EmployeeFilterModel;
                if (filters.HiringFromDate == "month/day/year")
                {
                    filters.HiringFromDate = null;
                }
                if (filters.HiringToDate == "month/day/year")
                {
                    filters.HiringToDate = null;
                }

                DateTime? fromDate = null;
                DateTime? toDate = null;
                string dateFormat = "MM/dd/yyyy";

                if (!string.IsNullOrEmpty(filters.HiringFromDate))
                {
                    if (DateTime.TryParseExact(filters.HiringFromDate, dateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime parsedFromDate))
                    {
                        fromDate = parsedFromDate;
                    }
                }

                if (!string.IsNullOrEmpty(filters.HiringToDate))
                {
                    if (DateTime.TryParseExact(filters.HiringToDate, dateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime parsedToDate))
                    {
                        toDate = parsedToDate;
                    }
                }



                // Apply date filters
                if (fromDate.HasValue)
                {
                    filteredData = filteredData.Where(e => e.HireDate.HasValue && e.HireDate.Value >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    filteredData = filteredData.Where(e => e.HireDate.HasValue && e.HireDate.Value <= toDate.Value);
                }

                // Apply training status filters
                if (filters.TStatusComplete || filters.TStatusPending)
                {
                    if (filters.TStatusComplete && filters.TStatusPending)
                    {
                        // No need to apply additional filter, both are selected
                    }
                    else
                    {
                        //filteredData = filteredData.Where(e => (filters.TStatusComplete && e.IsTraningCompleted) || (filters.TStatusPending && !e.IsTraningCompleted));
                        if (filters.TStatusComplete == true)
                        {
                            filteredData = filteredData.Where(x => (x.LastSlidename == ".q14Data" || x.LastSlidename == ".q14DataSp"));
                        }
                        else
                        {
                            filteredData = filteredData.Where(x => (x.LastSlidename != ".q14Data" && x.LastSlidename != ".q14DataSp"));
                        }
                    }
                }

                // Apply employee status filters
                if (filters.EStatusActive || filters.EStatusDeceased || filters.EStatusOnTerminated || filters.EStatusResigned)
                {
                    List<string> statusList = new List<string>();

                    if (filters.EStatusActive) { statusList.Add("Active"); }
                    if (filters.EStatusDeceased) { statusList.Add("Deceased"); }
                    if (filters.EStatusOnTerminated) { statusList.Add("Terminated"); }
                    if (filters.EStatusResigned) { statusList.Add("Resigned"); }

                    filteredData = filteredData.Where(e => statusList.Contains(e.StatusName));
                }

                // Apply 401(K) plan status filters
                if (filters.PStatusNotUsed || filters.PStatusOptIn || filters.PStatusOptOut)
                {
                    List<string> optInList = new List<string>();

                    if (filters.PStatusNotUsed) { optInList.Add("Not Used"); }
                    if (filters.PStatusOptIn) { optInList.Add("Opt-In"); }
                    if (filters.PStatusOptOut) { optInList.Add("Opt-Out"); }

                    filteredData = filteredData.Where(e => optInList.Contains(e.OptStatusValue));
                }

                // Apply medical insurance filters
                if (filters.MIStatusNotUsed == true || filters.MIStatusUsed == true)
                {
                    if (filters.MIStatusNotUsed == true && filters.MIStatusUsed == true)
                    {
                        // No need to apply additional filter, both are selected
                    }
                    else if (filters.MIStatusNotUsed == true)
                    {
                        filteredData = filteredData.Where(e => e.HelathBenefitUsed == "Not Used");
                    }
                    else if (filters.MIStatusUsed == true)
                    {
                        filteredData = filteredData.Where(e => e.HelathBenefitUsed == "Used");
                    }
                }

                // Apply department filter
                if (filters.DepartmentId != 0)
                {
                    filteredData = filteredData.Where(e => e.DepartmentId == filters.DepartmentId.Value);
                }
            }


            GridExcelExport exp = new GridExcelExport();
            exp.FileName = "EmployeeDetails.xlsx";
            Syncfusion.EJ2.Grids.Grid gridProperty = ConvertGridObject(gridModel);
            return exp.ExcelExport<HREmployeeViewModal>(gridProperty, filteredData.ToList());

        }

        private Syncfusion.EJ2.Grids.Grid ConvertGridObject(string gridProperty)
        {
            Syncfusion.EJ2.Grids.Grid GridModel = (Syncfusion.EJ2.Grids.Grid)Newtonsoft.Json.JsonConvert.DeserializeObject(gridProperty, typeof(Syncfusion.EJ2.Grids.Grid));

            GridColumnModel cols = (GridColumnModel)Newtonsoft.Json.JsonConvert.DeserializeObject("{\"allowGrouping\":false,\"allowPaging\":false,\"pageSettings\":{\"currentPage\":1,\"pageCount\":2,\"pageSize\":100},\"sortSettings\":{},\"allowPdfExport\":true,\"allowExcelExport\":true,\"aggregates\":[],\"filterSettings\":{\"immediateModeDelay\":1500,\"type\":\"CheckBox\"},\"groupSettings\":{\"columns\":[],\"enableLazyLoading\":false,\"disablePageWiseAggregates\":false},\"columns\":[],\"locale\":\"en-US\",\"searchSettings\":{\"key\":\"\",\"fields\":[]}}", typeof(GridColumnModel));
            cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "FullName", HeaderText = "Employee" });
            cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "NickName", HeaderText = "Store Name" });
            cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "OfficeEmployeeID", HeaderText = "Employee Id" });
            cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "DateofBirth", HeaderText = "DOB" });
            cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "HireDate", HeaderText = "Hire Date" });
            cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "SSN", HeaderText = "SSN" });
            cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "IsTraningCompleted", HeaderText = "Training" });
            cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "OptStatusValue", HeaderText = "401(K) Plan Status" });
            cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "HelathBenefitUsed", HeaderText = "Medical Insurance" });

            foreach (var item in cols.columns)
            {
                item.AutoFit = true;
            }
            GridModel.Columns = cols.columns;
            return GridModel;
        }

        public ActionResult PdfExport(string gridModel, string filterss)
        {
            EmployeeFilterModel filters = new EmployeeFilterModel();
            if (filterss != null)
            {
                filters = JsonConvert.DeserializeObject<EmployeeFilterModel>(filterss);
            }
            List<HREmployeeViewModal> BindData = new List<HREmployeeViewModal>();

            DBContext db1 = new DBContext();
            var StoreId = Convert.ToInt32(Session["storeid"]);
            var userid = Convert.ToInt32(Session["UserId"]);
            BindData = _hREmployeeRepository.GetHREmployeeList(StoreId, userid);
            var filteredData = BindData.AsQueryable();


            if (filters != null)
            {
                //filters = Session["EmployeeListFilter"] as EmployeeFilterModel;
                if (filters.HiringFromDate == "month/day/year")
                {
                    filters.HiringFromDate = null;
                }
                if (filters.HiringToDate == "month/day/year")
                {
                    filters.HiringToDate = null;
                }

                DateTime? fromDate = null;
                DateTime? toDate = null;
                string dateFormat = "MM/dd/yyyy";

                if (!string.IsNullOrEmpty(filters.HiringFromDate))
                {
                    if (DateTime.TryParseExact(filters.HiringFromDate, dateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime parsedFromDate))
                    {
                        fromDate = parsedFromDate;
                    }
                }

                if (!string.IsNullOrEmpty(filters.HiringToDate))
                {
                    if (DateTime.TryParseExact(filters.HiringToDate, dateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime parsedToDate))
                    {
                        toDate = parsedToDate;
                    }
                }



                // Apply date filters
                if (fromDate.HasValue)
                {
                    filteredData = filteredData.Where(e => e.HireDate.HasValue && e.HireDate.Value >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    filteredData = filteredData.Where(e => e.HireDate.HasValue && e.HireDate.Value <= toDate.Value);
                }

                // Apply training status filters
                if (filters.TStatusComplete || filters.TStatusPending)
                {
                    if (filters.TStatusComplete && filters.TStatusPending)
                    {
                        // No need to apply additional filter, both are selected
                    }
                    else
                    {
                        //filteredData = filteredData.Where(e => (filters.TStatusComplete && e.IsTraningCompleted) || (filters.TStatusPending && !e.IsTraningCompleted));
                        if (filters.TStatusComplete == true)
                        {
                            filteredData = filteredData.Where(x => (x.LastSlidename == ".q14Data" || x.LastSlidename == ".q14DataSp"));
                        }
                        else
                        {
                            filteredData = filteredData.Where(x => (x.LastSlidename != ".q14Data" && x.LastSlidename != ".q14DataSp"));
                        }
                    }
                }

                // Apply employee status filters
                if (filters.EStatusActive || filters.EStatusDeceased || filters.EStatusOnTerminated || filters.EStatusResigned)
                {
                    List<string> statusList = new List<string>();

                    if (filters.EStatusActive) { statusList.Add("Active"); }
                    if (filters.EStatusDeceased) { statusList.Add("Deceased"); }
                    if (filters.EStatusOnTerminated) { statusList.Add("Terminated"); }
                    if (filters.EStatusResigned) { statusList.Add("Resigned"); }

                    filteredData = filteredData.Where(e => statusList.Contains(e.StatusName));
                }

                // Apply 401(K) plan status filters
                if (filters.PStatusNotUsed || filters.PStatusOptIn || filters.PStatusOptOut)
                {
                    List<string> optInList = new List<string>();

                    if (filters.PStatusNotUsed) { optInList.Add("Not Used"); }
                    if (filters.PStatusOptIn) { optInList.Add("Opt-In"); }
                    if (filters.PStatusOptOut) { optInList.Add("Opt-Out"); }

                    filteredData = filteredData.Where(e => optInList.Contains(e.OptStatusValue));
                }

                // Apply medical insurance filters
                if (filters.MIStatusNotUsed == true || filters.MIStatusUsed == true)
                {
                    if (filters.MIStatusNotUsed == true && filters.MIStatusUsed == true)
                    {
                        // No need to apply additional filter, both are selected
                    }
                    else if (filters.MIStatusNotUsed == true)
                    {
                        filteredData = filteredData.Where(e => e.HelathBenefitUsed == "Not Used");
                    }
                    else if (filters.MIStatusUsed == true)
                    {
                        filteredData = filteredData.Where(e => e.HelathBenefitUsed == "Used");
                    }
                }

                // Apply department filter
                if (filters.DepartmentId != 0)
                {
                    filteredData = filteredData.Where(e => e.DepartmentId == filters.DepartmentId.Value);
                }
            }


            PdfDocument doc = new PdfDocument();
            doc.PageSettings.Orientation = PdfPageOrientation.Landscape;
            doc.PageSettings.Size = PdfPageSize.A3;


            GridPdfExport exp = new GridPdfExport();
            exp.Theme = "flat-saffron";
            exp.FileName = "EmployeeDetails.pdf";
            exp.PdfDocument = doc;

            Syncfusion.EJ2.Grids.Grid gridProperty = ConvertGridObject(gridModel);
            return exp.PdfExport<HREmployeeViewModal>(gridProperty, filteredData.ToList());
        }

        //Changes Done in Dani 20/08/2024
        public ActionResult GetFilterPartial()
        {
            try
            {
                logger.Info("HREmployeeController - GetFilterPartial - " + DateTime.Now);
                ViewBag.DepartmentId = new SelectList(new[] { new HRDepartmentViewModel { DepartmentId = 0, DepartmentName = "-- Select Department --" } }
                   .Concat(_hRDepartmentRepository.GetHRDepartmentList().Where(a => a.IsActive == true).OrderBy(a => a.DepartmentName).ToList()), "DepartmentId", "DepartmentName");
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeController - GetFilterPartial - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView("_EmployeeFilterPartial");
        }

        public ActionResult GetEmployeeFilterData(EmployeeFilterModel filters)
        {
            Session["EmployeeListFilter"] = filters;
            List<HREmployeeViewModal> HrEVm = _hREmployeeRepository.GetHREmployeeList(Convert.ToInt32(Session["StoreId"]), Convert.ToInt32(Session["UserId"]));

            if (filters.HiringFromDate == "month/day/year")
            {
                filters.HiringFromDate = null;
            }
            if (filters.HiringToDate == "month/day/year")
            {
                filters.HiringToDate = null;
            }

            DateTime? fromDate = null;
            DateTime? toDate = null;
            string dateFormat = "MM/dd/yyyy";

            if (!string.IsNullOrEmpty(filters.HiringFromDate))
            {
                if (DateTime.TryParseExact(filters.HiringFromDate, dateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime parsedFromDate))
                {
                    fromDate = parsedFromDate;
                }
            }

            if (!string.IsNullOrEmpty(filters.HiringToDate))
            {
                if (DateTime.TryParseExact(filters.HiringToDate, dateFormat, null, System.Globalization.DateTimeStyles.None, out DateTime parsedToDate))
                {
                    toDate = parsedToDate;
                }
            }

            var filteredData = HrEVm.AsQueryable();

            // Apply date filters
            if (fromDate.HasValue)
            {
                filteredData = filteredData.Where(e => e.HireDate.HasValue && e.HireDate.Value >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                filteredData = filteredData.Where(e => e.HireDate.HasValue && e.HireDate.Value <= toDate.Value);
            }

            // Apply training status filters
            if (filters.TStatusComplete || filters.TStatusPending)
            {
                if (filters.TStatusComplete && filters.TStatusPending)
                {
                    // No need to apply additional filter, both are selected
                }
                else
                {
                    //filteredData = filteredData.Where(e => (filters.TStatusComplete && e.IsTraningCompleted) || (filters.TStatusPending && !e.IsTraningCompleted));
                    if (filters.TStatusComplete == true)
                    {
                        filteredData = filteredData.Where(x => (x.LastSlidename == ".q14Data" || x.LastSlidename == ".q14DataSp"));
                    }
                    else
                    {
                        filteredData = filteredData.Where(x => (x.LastSlidename != ".q14Data" && x.LastSlidename != ".q14DataSp"));
                    }
                }
            }

            // Apply employee status filters
            if (filters.EStatusActive || filters.EStatusDeceased || filters.EStatusOnTerminated || filters.EStatusResigned)
            {
                List<string> statusList = new List<string>();

                if (filters.EStatusActive) { statusList.Add("Active"); }
                if (filters.EStatusDeceased) { statusList.Add("Deceased"); }
                if (filters.EStatusOnTerminated) { statusList.Add("Terminated"); }
                if (filters.EStatusResigned) { statusList.Add("Resigned"); }

                filteredData = filteredData.Where(e => statusList.Contains(e.StatusName));
            }

            // Apply 401(K) plan status filters
            if (filters.PStatusNotUsed || filters.PStatusOptIn || filters.PStatusOptOut)
            {
                List<string> optInList = new List<string>();

                if (filters.PStatusNotUsed) { optInList.Add("Not Used"); }
                if (filters.PStatusOptIn) { optInList.Add("Opt-In"); }
                if (filters.PStatusOptOut) { optInList.Add("Opt-Out"); }

                filteredData = filteredData.Where(e => optInList.Contains(e.OptStatusValue));
            }

            // Apply medical insurance filters
            if (filters.MIStatusNotUsed == true || filters.MIStatusUsed == true)
            {
                if (filters.MIStatusNotUsed == true && filters.MIStatusUsed == true)
                {
                    // No need to apply additional filter, both are selected
                }
                else if (filters.MIStatusNotUsed == true)
                {
                    filteredData = filteredData.Where(e => e.HelathBenefitUsed == "Not Used");
                }
                else if (filters.MIStatusUsed == true)
                {
                    filteredData = filteredData.Where(e => e.HelathBenefitUsed == "Used");
                }
            }

            // Apply department filter
            if (filters.DepartmentId != 0)
            {
                filteredData = filteredData.Where(e => e.DepartmentId == filters.DepartmentId.Value);
            }

            var resultData = filteredData.ToList();
            var jsonData = resultData.Select(e => new
            {
                e.EmployeeId,
                e.EmployeeUserName,
                e.FullName,
                DateofBirth = e.DateofBirth.HasValue ? e.DateofBirth.Value.ToString("MM/dd/yyyy") : null,
                e.SSN,
                e.OfficeEmployeeID,
                e.NickName,
                e.IsTraningCompleted,
                e.IsLocked,
                e.Status,
                e.StatusName,
                e.OptStatus,
                e.OptStatusValue,
                e.HelathBenefitUsed,
                e.TraningFilePath,
                e.TraningContent,
                e.DepartmentId,
                e.DepartmentName,
                HireDate = e.HireDate.HasValue ? e.HireDate.Value.ToString("MM/dd/yyyy") : null
            }).ToList();
            return Json(new { result = jsonData, count = resultData.Count, filters = filters });
        }

    }
}