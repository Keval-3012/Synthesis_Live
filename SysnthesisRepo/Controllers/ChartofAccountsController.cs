using EntityModels.Models;
using NLog;
using Repository;
using Repository.IRepository;
using Syncfusion.EJ2.Base;
using Syncfusion.EJ2.GridExport;
using Syncfusion.Pdf;
using SynthesisQBOnline;
using SynthesisQBOnline.BAL;
using SynthesisViewModal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml.Linq;
using Utility;

namespace SynthesisRepo.Controllers
{
    public class ChartofAccountsController : Controller
    {
        private readonly IChartofAccountsRepository _departmentMastersRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        private readonly ICommonRepository _commonRepository;
        private readonly IUserActivityLogRepository _activityLogRepository;
        private readonly IQBRepository _qBRepository;
        private readonly ISynthesisApiRepository _synthesisApiRepository;

        DepartmentMastersViewModal departmentMastersViewModal = new DepartmentMastersViewModal();

        public ChartofAccountsController()
        {
            this._departmentMastersRepository = new ChartofAccountsRepository(new DBContext());
            this._commonRepository = new CommonRepository(new DBContext());
            this._activityLogRepository = new UserActivityLogRepository(new DBContext());
            this._qBRepository = new QBRepository(new DBContext());
            this._synthesisApiRepository = new SynthesisApiRepository();
        }

        public string Message { get; set; }
        protected static string StatusMessageString = "";
        QBResponse objResponse = new QBResponse();

        [Authorize(Roles = "Administrator,ViewQBDepartment")]
        // GET: DepartmentMasters
        public ActionResult Index()
        {
            ViewBag.Title = "Chart of Accounts - Synthesis";
            _commonRepository.LogEntries();     //Harsh's code
            try
            {
                if (Session["Sucessmesg"] != null)
                {
                    ViewBag.Message = Session["Sucessmesg"].ToString();
                    Session.Remove("Sucessmesg");
                }
                else
                {

                }
                if (StatusMessageString != "")
                {
                    ViewBag.StatusMessageString = StatusMessageString;
                    StatusMessageString = "";
                }
                //Get StoreList by Storeid.
                var StoreList = _commonRepository.GetStoreList(11).Select(s => s.StoreId);
                //using this class get Store master list.
                var storemasterlist = _departmentMastersRepository.StoreMasterList();
                if (Session["StoreId"] != null)
                {
                    ViewBag.StoreId = new SelectList(storemasterlist.Where(s => s.IsActive == true && StoreList.Contains(s.StoreId)).Select(s => new { s.StoreId, s.Name }).OrderBy(o => o.Name).ToList(), "StoreId", "Name", Session["StoreId"]);
                }
                else
                {
                    ViewBag.StoreId = new SelectList(storemasterlist.Where(s => s.IsActive == true && StoreList.Contains(s.StoreId)).Select(s => new { s.StoreId, s.Name }).OrderBy(o => o.Name).ToList(), "StoreId", "Name");
                }
            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsController - Index - " + DateTime.Now + " - " + ex.Message.ToString());              
            }           
            return View();
        }

        /// <summary>
        /// This is add details partial view.
        /// </summary>
        /// <returns></returns>
        public ActionResult AddPartial()
        {
            //Get All Account Details type List
            ViewBag.AccountDetailTypeId = new SelectList(_departmentMastersRepository.AccountDetailTypeList().Select(s => new { s.AccountDetailTypeId, s.DetailType }).OrderBy(o => o.DetailType).ToList(), "AccountDetailTypeId", "DetailType");
            //Get all Account typeId.
            ViewBag.AccountTypeId = new SelectList(_departmentMastersRepository.AccountTypeList().Select(s => new { s.AccountTypeId, s.AccountType }).OrderBy(o => o.AccountType), "AccountTypeId", "AccountType");

            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            //Using this db class get StoreList with role Wise.
            var StoreList = _commonRepository.GetStoreList_RoleWise(10, "CreateQBVendor", UserName).Select(s => s.StoreId);
            //using this class get Store master list.
            var storemasterlist = _departmentMastersRepository.StoreMasterList();
            ViewBag.MultiStoreId = storemasterlist.Where(s => s.IsActive == true && StoreList.Contains(s.StoreId)).Select(s => new { s.StoreId, s.Name }).OrderBy(o => o.Name).ToList();
            ViewBag.StoreId = new SelectList(storemasterlist.Where(k => k.IsActive == true && StoreList.Contains(k.StoreId)).Select(s => new { s.StoreId, s.Name }).OrderBy(o => o.Name).ToList(), "StoreId", "Name");

            return PartialView("_DialogAddDepartmentpartial");
        }
        /// <summary>
        /// This is edit Partial view.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ActionResult Editpartial(DepartmentMaster value)
        {
            //Get all Department Master list.
            DepartmentMaster departmentMaster = _departmentMastersRepository.DepartmentMasterList().Where(s => s.DepartmentId == value.DepartmentId).FirstOrDefault();
            //Get All Account Details type List
            ViewBag.AccountDetailTypeId = new SelectList(_departmentMastersRepository.AccountDetailTypeList().OrderBy(o => o.DetailType).ToList(), "AccountDetailTypeId", "DetailType", departmentMaster.AccountDetailTypeId);
            //Get All Account type Id
            ViewBag.AccountTypeId = new SelectList(_departmentMastersRepository.AccountTypeList().OrderBy(o => o.AccountType).ToList(), "AccountTypeId", "AccountType", departmentMaster.AccountTypeId);

            //var list = db.VendorDepartmentRelationMasters.Where(s => s.VendorId == vendorMaster.VendorId && s.StoreId == vendorMaster.StoreId).Select(s => s.DepartmentId).ToList();
            //ViewBag.DepartmentID = db.VendorDepartmentRelationMasters.Where(s => s.VendorId == departmentMaster.DepartmentId && s.StoreId == departmentMaster.StoreId).Select(s => s.DepartmentId).ToList();
            ////ViewBag.MultiDepartmentId = db.DepartmentMasters.Where(s => s.StoreId == departmentMaster.StoreId && s.IsActive == true && AccountTypeId.Contains(s.AccountTypeId)).Select(s => new { s.DepartmentId, DepartmentName = s.DepartmentName }).OrderBy(o => o.DepartmentName).ToList();

            return PartialView("_DialogEditDepartmentpartial", departmentMaster);
        }
        /// <summary>
        /// This method is get Department data.
        /// </summary>
        /// <param name="StoreID"></param>
        /// <param name="Search"></param>
        /// <param name="OrderBy"></param>
        /// <param name="IsAsc"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Index(int? StoreID, string Search, string OrderBy = "DepartmentId", int IsAsc = 1)
        {
            departmentMastersViewModal.StoreIds = StoreID;
            try
            {
                if (departmentMastersViewModal.StoreIds == null)
                {
                    //Get all Department Master list.
                    var Lists = (_departmentMastersRepository.DepartmentMasterList().Select(s => new { s.DepartmentId, s.DepartmentName, s.IsActive }).ToList()).Select(s => new DepartmentMaster
                    {
                        DepartmentId = s.DepartmentId,
                        DepartmentName = s.DepartmentName,
                        IsActive = s.IsActive
                    });
                    if (IsAsc == 1)
                    {
                        if (OrderBy == "DepartmentId") { Lists = Lists.OrderBy(a => a.DepartmentId).ToList(); }
                        else if (OrderBy == "DepartmentName") { Lists = Lists.OrderBy(a => a.DepartmentName).ToList(); }
                    }
                    else
                    {
                        if (OrderBy == "DepartmentId") { Lists = Lists.OrderByDescending(a => a.DepartmentId).ToList(); }
                        else if (OrderBy == "DepartmentName") { Lists = Lists.OrderByDescending(a => a.DepartmentName).ToList(); }
                    }
                    //Get StoreList by Storeid.

                    var StoreList = _commonRepository.GetStoreList(11).Select(s => s.StoreId);
                    //using this class get Store master list.
                    ViewBag.StoreId = new SelectList(_departmentMastersRepository.StoreMasterList().Where(s => s.IsActive == true && StoreList.Contains(s.StoreId)).Select(s => new { s.StoreId, s.Name }).OrderBy(o => o.Name).ToList(), "StoreId", "Name");
                    return Json(Lists, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //Get all Department Master list.
                    var Lists = (_departmentMastersRepository.DepartmentMasterList().Where(a => a.StoreId == departmentMastersViewModal.StoreIds && (a.DepartmentName.Contains(Search))).Select(s => new { s.DepartmentId, s.DepartmentName, s.IsActive }).ToList()).Select(s => new DepartmentMaster
                    {
                        DepartmentId = s.DepartmentId,
                        DepartmentName = s.DepartmentName,
                        IsActive = s.IsActive
                    });
                    if (IsAsc == 1)
                    {
                        if (OrderBy == "DepartmentId") { Lists = Lists.OrderBy(a => a.DepartmentId).ToList(); }
                        else if (OrderBy == "DepartmentName") { Lists = Lists.OrderBy(a => a.DepartmentName).ToList(); }
                    }
                    else
                    {
                        if (OrderBy == "DepartmentId") { Lists = Lists.OrderByDescending(a => a.DepartmentId).ToList(); }
                        else if (OrderBy == "DepartmentName") { Lists = Lists.OrderByDescending(a => a.DepartmentName).ToList(); }
                    }
                    //Get StoreList by Storeid.
                    var StoreList = _commonRepository.GetStoreList(11).Select(s => s.StoreId);
                    Session["StoreId"] = StoreID;
                    //using this class get Store master list.
                    ViewBag.StoreId = new SelectList(_departmentMastersRepository.StoreMasterList().Where(s => s.IsActive == true && StoreList.Contains(s.StoreId)).Select(s => new { s.StoreId, s.Name }).OrderBy(o => o.Name).ToList(), "StoreId", "Name");
                    return Json(Lists, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsController - Index - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("", JsonRequestBehavior.AllowGet);
            }
            
        }

        /// <summary>
        /// This method is url Data Source.
        /// </summary>
        /// <param name="dm"></param>
        /// <returns></returns>
        public ActionResult UrlDatasource(DataManagerRequest dm)
        {
            departmentMastersViewModal.StoreIds = 0;
            try
            {
                if (Session["storeid"] != null)
                {
                    departmentMastersViewModal.StoreIds = Convert.ToInt32(Convert.ToString(Session["storeid"]));
                }
                ViewBag.StoreId = departmentMastersViewModal.StoreIds;
                //using this class get Store master list.
                var Storedata = _departmentMastersRepository.StoreMasterList();
                //Get All Account Details type List
                var AccountDetaildata = _departmentMastersRepository.AccountDetailTypeList();
                //Get all Account type List.
                var AccountTypedata = _departmentMastersRepository.AccountTypeList();
                List<DepartmentMasterList> DepartmentMaster = new List<DepartmentMasterList>();
                if (departmentMastersViewModal.StoreIds == 0)
                {
                    //Get all Department Master list with selection.

                    DepartmentMaster = (_departmentMastersRepository.DepartmentMasterList().Select(s => new { s.DepartmentId, s.DepartmentName, s.IsActive, s.AccountTypeId, s.StoreId, s.AccountDetailTypeId }).ToList()).Select(s => new DepartmentMasterList
                    {
                        DepartmentId = s.DepartmentId,
                        DepartmentName = s.DepartmentName,
                        Status = s.IsActive == true ? "Active" : "Inactive",
                        AccountType = AccountTypedata.Where(w => w.AccountTypeId == s.AccountTypeId).Select(o => o.AccountType).FirstOrDefault(),
                        StoreName = Storedata.Where(w => w.StoreId == s.StoreId).Select(o => o.NickName).FirstOrDefault(),
                        AccountDetailType = AccountDetaildata.Where(w => w.AccountDetailTypeId == s.AccountDetailTypeId).Select(o => o.DetailType).FirstOrDefault()
                    }).ToList();
                }
                else
                {
                    //Get all Department Master list with selection.
                    DepartmentMaster = (_departmentMastersRepository.DepartmentMasterList().Where(a => a.StoreId == departmentMastersViewModal.StoreIds).Select(s => new { s.DepartmentId, s.DepartmentName, s.IsActive, s.AccountTypeId, s.StoreId, s.AccountDetailTypeId }).ToList()).Select(s => new DepartmentMasterList
                    {
                        DepartmentId = s.DepartmentId,
                        DepartmentName = s.DepartmentName,
                        Status = s.IsActive == true ? "Active" : "Inactive",
                        AccountType = AccountTypedata.Where(w => w.AccountTypeId == s.AccountTypeId).Select(o => o.AccountType).FirstOrDefault(),
                        StoreName = Storedata.Where(w => w.StoreId == s.StoreId).Select(o => o.NickName).FirstOrDefault(),
                        AccountDetailType = AccountDetaildata.Where(w => w.AccountDetailTypeId == s.AccountDetailTypeId).Select(o => o.DetailType).FirstOrDefault()
                    }).ToList();
                }
                IEnumerable DataSource = DepartmentMaster;
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
                departmentMastersViewModal.count = DataSource.Cast<DepartmentMasterList>().Count();
                if (dm.Skip != 0)
                {
                    DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
                }
                if (dm.Take != 0)
                {
                    DataSource = operation.PerformTake(DataSource, dm.Take);
                }

                return dm.RequiresCounts ? Json(new { result = DataSource, count = departmentMastersViewModal.count }) : Json(DataSource);
            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsController - UrlDatasource - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("", JsonRequestBehavior.AllowGet);
            }
           
        }

        /// <summary>
        /// This method is return create view.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,CreateQBDepartment")]
        // GET: DepartmentMasters/Create
        public ActionResult Create()
        {
            DepartmentMaster obj = new DepartmentMaster();
            try
            {
                if (Session["Sucessmesg"] != null)
                {
                    ViewBag.Message = Session["Sucessmesg"].ToString();
                    Session.Remove("Sucessmesg");
                }

                //Get All Account Details type List
                ViewBag.AccountDetailTypeId = new SelectList(_departmentMastersRepository.AccountDetailTypeList().Select(s => new { s.AccountDetailTypeId, s.DetailType }).OrderBy(o => o.DetailType).ToList(), "AccountDetailTypeId", "DetailType");
                //Get all Account Type list.
                ViewBag.AccountTypeId = new SelectList(_departmentMastersRepository.AccountTypeList().Select(s => new { s.AccountTypeId, s.AccountType }).OrderBy(o => o.AccountType), "AccountTypeId", "AccountType");

                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                //Using this db class get StoreList with role Wise.
                var StoreList = _commonRepository.GetStoreList_RoleWise(11, "CreateQBDepartment", UserName).Select(s => s.StoreId);
                //using this class get Store master list with selection of storeid.
                ViewBag.MultiStoreId = new MultiSelectList(_departmentMastersRepository.StoreMasterList().Where(s => s.IsActive == true && StoreList.Contains(s.StoreId)).Select(s => new { s.StoreId, s.Name }).OrderBy(o => o.Name).ToList(), "StoreId", "Name");
                //using this class get Store master list with selection of storeid.

                ViewBag.StoreId = new SelectList(_departmentMastersRepository.StoreMasterList().Where(k => k.IsActive == true && StoreList.Contains(k.StoreId)).Select(s => new { s.StoreId, s.Name }).OrderBy(o => o.Name).ToList(), "StoreId", "Name");
                //ViewBag.MultiStoreId = new MultiSelectList(db.StoreMasters.Where(k => k.IsActive == true).Select(s => new { s.StoreId, s.Name }).OrderBy(o => o.Name).ToList(), "StoreId", "Name");
            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsController - Create - " + DateTime.Now + " - " + ex.Message.ToString());
                
            }
           
            return View(obj);
        }

        // POST: DepartmentMasters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        /// <summary>
        /// This methos is Insert Department. 
        /// </summary>
        /// <param name="departmentMaster"></param>
        /// <returns></returns>
        public async Task<ActionResult> InsertDepartment(CRUDModel<DepartmentMaster> departmentMaster)
        {
            departmentMastersViewModal.error = null;
            departmentMastersViewModal.Sucsses = null;
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            if (ModelState.IsValid)
            {
                try
                {
                    List<DepartmentMaster> DepartmentList = new List<DepartmentMaster>();

                    foreach (var item in departmentMaster.Value.MultiStoreId)
                    {
                        departmentMastersViewModal.StoreId = Convert.ToInt32(item);
                        if (departmentMastersViewModal.StoreId > 0)
                        {
                            if (ModelState.IsValid)
                            {
                                int iDeptId = 0;
                                //Using this db class get stores on Line desktop.
                                string Store = _qBRepository.GetStoreOnlineDesktop(departmentMastersViewModal.StoreId);
                                //Using this db class get stores on Line desktop Flag.
                                int StoreFlag = _qBRepository.GetStoreOnlineDesktopFlag(departmentMastersViewModal.StoreId);
                                departmentMaster.Value.IsActive = true;
                                if (Store != "")
                                {
                                    //Get all Account type list.
                                    string AccType = _departmentMastersRepository.AccountTypeList().Where(a => a.AccountTypeId == departmentMaster.Value.AccountTypeId).FirstOrDefault().AccountType;
                                    //Get All Account Details type List
                                    var Dte = _departmentMastersRepository.AccountDetailTypeList().Where(a => a.AccountDetailTypeId == departmentMaster.Value.AccountDetailTypeId).ToList();
                                    string DetailsType = "";
                                    if (Dte.Count > 0)
                                    {
                                        DetailsType = Dte.FirstOrDefault().QBDetailType.ToString();
                                        //departmentMaster.AccountDetailTypeId = Dte.FirstOrDefault().AccountDetailTypeId;
                                    }
                                    if (departmentMaster.Value.IsSubAccount == "")
                                    {
                                        departmentMaster.Value.IsSubAccount = null;
                                    }
                                    if (Store == "Online")
                                    {
                                        //Get all Account type list.

                                        var Dt = _departmentMastersRepository.AccountTypeList().Where(a => a.CommonType == AccType && a.Flag == "O").ToList();
                                        string QBAccountType = "";
                                        int AccountID = 0;
                                        if (Dt.Count > 0)
                                        {
                                            QBAccountType = Dt.FirstOrDefault().AccountType.ToString();
                                            AccountID = Dt.FirstOrDefault().AccountTypeId;
                                        }
                                        departmentMaster.Value.AccountTypeId = AccountID;
                                        if (StoreFlag == 1)
                                        {
                                            QBResponse objResponse = new QBResponse();
                                            //Get QB Sync Department.
                                            _qBRepository.QBSyncDepartment(departmentMaster.Value.DepartmentName, QBAccountType, DetailsType, departmentMaster.Value.Description, departmentMaster.Value.AccountNumber, departmentMaster.Value.IsSubAccount, departmentMastersViewModal.StoreId, ref objResponse);
                                            if (objResponse.ID != "0" || objResponse.Status == "Done")
                                            {
                                                //using this db class get select by storeID name
                                                var vData1 = _departmentMastersRepository.SelectByStoreID_Name(departmentMastersViewModal.StoreId, departmentMaster.Value.DepartmentName);
                                                if (vData1.Count > 0)
                                                {
                                                    //Select for update data.
                                                    var vData = _departmentMastersRepository.SelectForUpdate(departmentMastersViewModal.StoreId, departmentMaster.Value.DepartmentName, departmentMaster.Value.DepartmentId);
                                                    if (vData.Count > 0)
                                                    {
                                                        //Message = "Department Name Already Exist";
                                                        Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                                        TempData["Sucessme"] += vData.FirstOrDefault().DepartmentName + ":" + Message;
                                                    }
                                                    int iDeptID = vData.LastOrDefault().DepartmentId;
                                                    //Select By StoreId and department id
                                                    DepartmentMaster objDept = _departmentMastersRepository.SelectByStoreID_DepartmentId(departmentMastersViewModal.StoreId, iDeptID);
                                                    objDept.ListId = objResponse.ID;
                                                    objDept.DepartmentName = departmentMaster.Value.DepartmentName == null ? null : departmentMaster.Value.DepartmentName.ToUpper();
                                                    objDept.AccountTypeId = Convert.ToInt32(departmentMaster.Value.AccountType);
                                                    //Get account Type.
                                                    var DtAcc = _departmentMastersRepository.GetAccountType(AccType);
                                                    if (DtAcc.Count > 0)
                                                    {
                                                        objDept.AccountTypeId = DtAcc.FirstOrDefault().AccountTypeId;
                                                    }
                                                    else
                                                    {
                                                        AccountTypeMaster objAcc = new AccountTypeMaster();
                                                        objAcc.AccountType = departmentMaster.Value.AccountType;
                                                        objAcc.Flag = "O";
                                                        objAcc.CommonType = departmentMaster.Value.AccountType;
                                                        //This db class is save Account type details
                                                        _departmentMastersRepository.SaveAccountTypeMasters(objAcc);
                                                        int id = objAcc.AccountTypeId;
                                                        objAcc = null;
                                                        objDept.AccountTypeId = id;
                                                    }
                                                    //Get Qb Details
                                                    var DtAccDetail = _departmentMastersRepository.GetQBDetailType(DetailsType);
                                                    if (DtAccDetail.Count > 0)
                                                    {
                                                        objDept.AccountDetailTypeId = DtAccDetail.FirstOrDefault().AccountDetailTypeId;
                                                        if (objDept.AccountDetailTypeId == 0 || objDept.AccountDetailTypeId == null)
                                                        {
                                                            AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
                                                            objDetail.DetailType = departmentMaster.Value.AccountType;
                                                            objDetail.QBDetailType = departmentMaster.Value.QBDetailType;
                                                            objDetail.AccountTypeId = objDept.AccountTypeId;
                                                            //This db class is save Account type details
                                                            _departmentMastersRepository.SaveAccountDetailTypeMasters(objDetail);
                                                            objDetail = null;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
                                                        objDetail.DetailType = departmentMaster.Value.AccountType;
                                                        objDetail.QBDetailType = departmentMaster.Value.QBDetailType;
                                                        objDetail.AccountTypeId = objDept.AccountTypeId;
                                                        //This db class is save Account type details
                                                        _departmentMastersRepository.SaveAccountDetailTypeMasters(objDetail);
                                                        int id = objDetail.AccountDetailTypeId;
                                                        objDetail = null;
                                                        objDept.AccountDetailTypeId = id;
                                                    }

                                                    objDept.Description = departmentMaster.Value.Description == null ? null : departmentMaster.Value.Description.ToUpper();
                                                    objDept.AccountNumber = departmentMaster.Value.AccountNumber == null ? null : departmentMaster.Value.AccountNumber.ToUpper();
                                                    objDept.IsSubAccount = departmentMaster.Value.IsSubAccount;
                                                    objDept.IsActive = Convert.ToBoolean(departmentMaster.Value.IsActive);
                                                    objDept.StoreId = departmentMastersViewModal.StoreId;
                                                    objDept.ModifiedOn = DateTime.Now;
                                                    objDept.LastModifiedOn = DateTime.Now;
                                                    //Get UserID by Username.
                                                    objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                                    objDept.IsSync = 1;
                                                    objDept.SyncDate = DateTime.Now;
                                                    // //This db class is Update department details
                                                    _departmentMastersRepository.UpdateDepartmentMaster(objDept);
                                                    iDeptId = objDept.DepartmentId;
                                                    if (iDeptId == 0)
                                                    {
                                                        //Message = "Department Not Synched in QuickBook";
                                                        Message = _commonRepository.GetMessageValue("DMSQ", "Department Not Synched in QuickBook");
                                                        departmentMastersViewModal.error += departmentMaster.Value.DepartmentName + ":" + Message;

                                                    }
                                                    else
                                                    {
                                                        // TempData["Sucessme"] = "Department Save Successfully..";
                                                    }
                                                    objDept = null;
                                                }
                                                else
                                                {
                                                    //This db class is Seleect or update data.
                                                    var vData = _departmentMastersRepository.SelectForUpdate(departmentMastersViewModal.StoreId, departmentMaster.Value.DepartmentName, departmentMaster.Value.DepartmentId);
                                                    if (vData.Count > 0)
                                                    {
                                                        //Message = "Department Name Already Exist";
                                                        Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                                        departmentMastersViewModal.error += vData.FirstOrDefault().DepartmentName + ":" + Message;
                                                    }
                                                    DepartmentMaster objDept = new DepartmentMaster();
                                                    objDept.ListId = objResponse.ID;
                                                    objDept.DepartmentName = departmentMaster.Value.DepartmentName == null ? null : departmentMaster.Value.DepartmentName.ToUpper();
                                                    //Get account type
                                                    var DtAcc = _departmentMastersRepository.GetAccountType(AccType);
                                                    if (DtAcc.Count > 0)
                                                    {
                                                        objDept.AccountTypeId = DtAcc.FirstOrDefault().AccountTypeId;
                                                    }
                                                    else
                                                    {
                                                        AccountTypeMaster objAcc = new AccountTypeMaster();
                                                        objAcc.AccountType = departmentMaster.Value.AccountType;
                                                        objAcc.Flag = "O";
                                                        objAcc.CommonType = departmentMaster.Value.AccountType;
                                                        //This db class is save Account type details
                                                        _departmentMastersRepository.SaveAccountTypeMasters(objAcc);
                                                        int id = objAcc.AccountTypeId;
                                                        objAcc = null;
                                                        objDept.AccountTypeId = id;
                                                    }
                                                    //Get QB details.
                                                    var DtAccDetail = _departmentMastersRepository.GetQBDetailType(DetailsType);
                                                    if (DtAccDetail.Count > 0)
                                                    {
                                                        objDept.AccountDetailTypeId = DtAccDetail.FirstOrDefault().AccountDetailTypeId;
                                                        if (objDept.AccountDetailTypeId == 0 || objDept.AccountDetailTypeId == null)
                                                        {
                                                            AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
                                                            objDetail.DetailType = departmentMaster.Value.AccountDetailType;
                                                            objDetail.QBDetailType = departmentMaster.Value.QBDetailType;
                                                            objDetail.AccountTypeId = objDept.AccountTypeId;
                                                            //This db class is save Account type details
                                                            _departmentMastersRepository.SaveAccountDetailTypeMasters(objDetail);
                                                            objDetail = null;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
                                                        objDetail.DetailType = departmentMaster.Value.AccountDetailType;
                                                        objDetail.QBDetailType = departmentMaster.Value.QBDetailType;
                                                        objDetail.AccountTypeId = objDept.AccountTypeId;
                                                        //This db class is save Account type details
                                                        _departmentMastersRepository.SaveAccountDetailTypeMasters(objDetail);
                                                        int id = objDetail.AccountDetailTypeId;
                                                        objDetail = null;
                                                        objDept.AccountDetailTypeId = id;
                                                    }
                                                    objDept.Description = departmentMaster.Value.Description == null ? null : departmentMaster.Value.Description.ToUpper();
                                                    objDept.AccountNumber = departmentMaster.Value.AccountNumber == null ? null : departmentMaster.Value.AccountNumber.ToUpper();
                                                    objDept.IsSubAccount = departmentMaster.Value.IsSubAccount;
                                                    objDept.IsActive = Convert.ToBoolean(departmentMaster.Value.IsActive);
                                                    objDept.StoreId = departmentMastersViewModal.StoreId;
                                                    objDept.CreatedOn = DateTime.Now;
                                                    objDept.ModifiedOn = DateTime.Now;
                                                    objDept.LastModifiedOn = DateTime.Now;
                                                    //Get UserID by Username.
                                                    objDept.CreatedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                                    //Get UserID by Username.
                                                    objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                                    objDept.IsSync = 1;
                                                    objDept.SyncDate = DateTime.Now;
                                                    //This db class is save Department details
                                                    _departmentMastersRepository.SaveDepartmentMaster(objDept);
                                                    objDept = null;
                                                }
                                            }
                                            else
                                            {
                                                Message = objResponse.Status.ToString();
                                                //departmentMastersViewModal.error = "This AccountType Only Created OneTime...";
                                                departmentMastersViewModal.error = _commonRepository.GetMessageValue("DMAO", "This AccountType Only Created OneTime...");
                                                return Json(new { data = departmentMaster.Value, success = departmentMastersViewModal.Sucsses, Error = departmentMastersViewModal.error });
                                            }
                                        }
                                        else
                                        {
                                            //using this db class get select by storeID name
                                            var vData1 = _departmentMastersRepository.SelectByStoreID_Name(departmentMastersViewModal.StoreId, departmentMaster.Value.DepartmentName);
                                            if (vData1.Count > 0)
                                            {     //Select for update data.

                                                var vData = _departmentMastersRepository.SelectForUpdate(departmentMastersViewModal.StoreId, departmentMaster.Value.DepartmentName, departmentMaster.Value.DepartmentId);
                                                if (vData.Count > 0)
                                                {
                                                    //Message = "Department Name Already Exist";
                                                    Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                                    departmentMastersViewModal.error += vData.FirstOrDefault().DepartmentName + ":" + Message;
                                                }
                                                //Select By StoreId and department id
                                                DepartmentMaster objDept = _departmentMastersRepository.SelectByStoreID_DepId(departmentMastersViewModal.StoreId, vData.LastOrDefault().DepartmentId);
                                                objDept.ListId = objResponse.ID;
                                                objDept.DepartmentName = departmentMaster.Value.DepartmentName == null ? null : departmentMaster.Value.DepartmentName.ToUpper();
                                                objDept.AccountTypeId = Convert.ToInt32(departmentMaster.Value.AccountType);
                                                //get Accont type.
                                                var DtAcc = _departmentMastersRepository.GetAccountType(AccType);
                                                if (DtAcc.Count > 0)
                                                {
                                                    objDept.AccountTypeId = DtAcc.FirstOrDefault().AccountTypeId;
                                                }
                                                else
                                                {
                                                    AccountTypeMaster objAcc = new AccountTypeMaster();
                                                    objAcc.AccountType = departmentMaster.Value.AccountType;
                                                    objAcc.Flag = "O";
                                                    objAcc.CommonType = departmentMaster.Value.AccountType;
                                                    //This db class is save Account type details
                                                    _departmentMastersRepository.SaveAccountTypeMasters(objAcc);
                                                    int id = objAcc.AccountTypeId;
                                                    objAcc = null;
                                                    objDept.AccountTypeId = id;
                                                }

                                                var DtAccDetail = _departmentMastersRepository.GetQBDetailType(DetailsType);
                                                if (DtAccDetail.Count > 0)
                                                {
                                                    objDept.AccountDetailTypeId = DtAccDetail.FirstOrDefault().AccountDetailTypeId;
                                                    if (objDept.AccountDetailTypeId == 0 || objDept.AccountDetailTypeId == null)
                                                    {
                                                        AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
                                                        objDetail.DetailType = departmentMaster.Value.AccountType;
                                                        objDetail.QBDetailType = departmentMaster.Value.QBDetailType;
                                                        objDetail.AccountTypeId = objDept.AccountTypeId;
                                                        //This db class is save Account type details
                                                        _departmentMastersRepository.SaveAccountDetailTypeMasters(objDetail);
                                                        objDetail = null;
                                                    }
                                                }
                                                else
                                                {
                                                    AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
                                                    objDetail.DetailType = departmentMaster.Value.AccountType;
                                                    objDetail.QBDetailType = departmentMaster.Value.QBDetailType;
                                                    objDetail.AccountTypeId = objDept.AccountTypeId;
                                                    //This db class is save Account type details
                                                    _departmentMastersRepository.SaveAccountDetailTypeMasters(objDetail);
                                                    int id = objDetail.AccountDetailTypeId;
                                                    objDetail = null;
                                                    objDept.AccountDetailTypeId = id;
                                                }

                                                objDept.Description = departmentMaster.Value.Description == null ? null : departmentMaster.Value.Description.ToUpper();
                                                objDept.AccountNumber = departmentMaster.Value.AccountNumber == null ? null : departmentMaster.Value.AccountNumber.ToUpper();
                                                objDept.IsSubAccount = departmentMaster.Value.IsSubAccount;
                                                objDept.IsActive = Convert.ToBoolean(departmentMaster.Value.IsActive);
                                                objDept.StoreId = departmentMastersViewModal.StoreId;
                                                objDept.ModifiedOn = DateTime.Now;
                                                objDept.LastModifiedOn = DateTime.Now;
                                                //Get UserID by Username.
                                                objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                                objDept.IsSync = 1;
                                                objDept.SyncDate = DateTime.Now;
                                                //This db class is update deparment details
                                                _departmentMastersRepository.UpdateDepartmentMaster(objDept);
                                                iDeptId = objDept.DepartmentId;
                                                if (iDeptId == 0)
                                                {
                                                    //Message = "Department Not Synched in QuickBook";
                                                    Message = _commonRepository.GetMessageValue("DMSQ", "Department Not Synched in QuickBook");
                                                    departmentMastersViewModal.error += departmentMaster.Value.DepartmentName + ":" + Message;

                                                }
                                                else
                                                {
                                                    // TempData["Sucessme"] = "Department Save Successfully..";
                                                }
                                                objDept = null;
                                            }
                                            else
                                            {
                                                var vData = _departmentMastersRepository.SelectForUpdate(departmentMastersViewModal.StoreId, departmentMaster.Value.DepartmentName, departmentMaster.Value.DepartmentId);
                                                if (vData.Count > 0)
                                                {
                                                    //Message = "Department Name Already Exist";
                                                    Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                                    departmentMastersViewModal.error += vData.FirstOrDefault().DepartmentName + ":" + Message;
                                                }
                                                DepartmentMaster objDept = new DepartmentMaster();
                                                objDept.ListId = objResponse.ID;
                                                objDept.DepartmentName = departmentMaster.Value.DepartmentName == null ? null : departmentMaster.Value.DepartmentName.ToUpper();
                                                var DtAcc = _departmentMastersRepository.GetAccountType(AccType);
                                                if (DtAcc.Count > 0)
                                                {
                                                    objDept.AccountTypeId = DtAcc.FirstOrDefault().AccountTypeId;
                                                }
                                                else
                                                {
                                                    AccountTypeMaster objAcc = new AccountTypeMaster();
                                                    objAcc.AccountType = departmentMaster.Value.AccountType;
                                                    objAcc.Flag = "O";
                                                    objAcc.CommonType = departmentMaster.Value.AccountType;
                                                    _departmentMastersRepository.SaveAccountTypeMasters(objAcc);
                                                    int id = objAcc.AccountTypeId;
                                                    objAcc = null;
                                                    objDept.AccountTypeId = id;
                                                }

                                                var DtAccDetail = _departmentMastersRepository.GetQBDetailType(DetailsType);
                                                if (DtAccDetail.Count > 0)
                                                {
                                                    objDept.AccountDetailTypeId = DtAccDetail.FirstOrDefault().AccountDetailTypeId;
                                                    if (objDept.AccountDetailTypeId == 0)
                                                    {
                                                        AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
                                                        objDetail.DetailType = departmentMaster.Value.AccountDetailType;
                                                        objDetail.QBDetailType = departmentMaster.Value.QBDetailType;
                                                        objDetail.AccountTypeId = objDept.AccountTypeId;
                                                        _departmentMastersRepository.SaveAccountDetailTypeMasters(objDetail);
                                                        objDetail = null;
                                                    }
                                                }
                                                else
                                                {
                                                    AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
                                                    objDetail.DetailType = departmentMaster.Value.AccountDetailType;
                                                    objDetail.QBDetailType = departmentMaster.Value.QBDetailType;
                                                    objDetail.AccountTypeId = objDept.AccountTypeId;
                                                    _departmentMastersRepository.SaveAccountDetailTypeMasters(objDetail);
                                                    int id = objDetail.AccountDetailTypeId;
                                                    objDetail = null;
                                                    objDept.AccountDetailTypeId = id;
                                                }
                                                objDept.Description = departmentMaster.Value.Description == null ? null : departmentMaster.Value.Description.ToUpper();
                                                objDept.AccountNumber = departmentMaster.Value.AccountNumber == null ? null : departmentMaster.Value.AccountNumber.ToUpper();
                                                objDept.IsSubAccount = departmentMaster.Value.IsSubAccount;
                                                objDept.IsActive = Convert.ToBoolean(departmentMaster.Value.IsActive);
                                                objDept.StoreId = departmentMastersViewModal.StoreId;
                                                objDept.CreatedOn = DateTime.Now;
                                                objDept.ModifiedOn = DateTime.Now;
                                                objDept.LastModifiedOn = DateTime.Now;
                                                //Get UserID by Username.
                                                objDept.CreatedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                                //Get UserID by Username.
                                                objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                                objDept.IsSync = 1;
                                                objDept.SyncDate = DateTime.Now;
                                                _departmentMastersRepository.SaveDepartmentMaster(objDept);
                                                objDept = null;
                                            }

                                        }
                                    }
                                    else
                                    {
                                        var Dt = _departmentMastersRepository.AccountTypeList().Where(a => a.CommonType == AccType && a.Flag == "D").ToList();
                                        string QBAccountType = "";
                                        string QBCommonType = "";
                                        int AccountID = 0;
                                        if (Dt.Count > 0)
                                        {
                                            QBAccountType = Dt.FirstOrDefault().AccountType.ToString();
                                            QBCommonType = Dt.FirstOrDefault().CommonType.ToString();
                                            AccountID = Dt.FirstOrDefault().AccountTypeId;
                                        }
                                        departmentMaster.Value.AccountTypeId = AccountID;
                                        departmentMaster.Value.AccountType = QBAccountType;
                                        //using this db class get select by storeID name
                                        var vData1 = _departmentMastersRepository.SelectByStoreID_Name(departmentMastersViewModal.StoreId, departmentMaster.Value.DepartmentName);
                                        if (vData1.Count > 0)
                                        {     //Select for update data.
                                            var vData = _departmentMastersRepository.SelectForUpdate(departmentMastersViewModal.StoreId, departmentMaster.Value.DepartmentName, departmentMaster.Value.DepartmentId);
                                            if (vData.Count > 0)
                                            {
                                                //Message = "Department Name Already Exist";
                                                Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                                departmentMastersViewModal.error += vData.FirstOrDefault().DepartmentName + ":" + Message;
                                            }
                                            //Select By StoreId and department id
                                            DepartmentMaster objDept = _departmentMastersRepository.SelectByStoreID_DepId(departmentMastersViewModal.StoreId, vData.LastOrDefault().DepartmentId);
                                            objDept.ListId = vData1.FirstOrDefault().ListId;
                                            objDept.DepartmentName = departmentMaster.Value.DepartmentName == null ? null : departmentMaster.Value.DepartmentName.ToUpper();
                                            objDept.AccountTypeId = Convert.ToInt32(departmentMaster.Value.AccountType);
                                            //get Accont type.
                                            var DtAcc = _departmentMastersRepository.GetAccountType(AccType);
                                            if (DtAcc.Count > 0)
                                            {
                                                objDept.AccountTypeId = DtAcc.FirstOrDefault().AccountTypeId;
                                            }
                                            else
                                            {
                                                AccountTypeMaster objAcc = new AccountTypeMaster();
                                                objAcc.AccountType = departmentMaster.Value.AccountType;
                                                objAcc.Flag = "D";
                                                objAcc.CommonType = departmentMaster.Value.AccountType;
                                                //This db class is save Account type details
                                                _departmentMastersRepository.SaveAccountTypeMasters(objAcc);
                                                int id = objAcc.AccountTypeId;
                                                objAcc = null;
                                                objDept.AccountTypeId = id;
                                            }
                                            //Get QB details.
                                            var DtAccDetail = _departmentMastersRepository.GetQBDetailType(DetailsType);
                                            if (DtAccDetail.Count > 0)
                                            {
                                                objDept.AccountDetailTypeId = DtAccDetail.FirstOrDefault().AccountDetailTypeId;
                                                if (objDept.AccountDetailTypeId == 0 || objDept.AccountDetailTypeId == null)
                                                {
                                                    AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
                                                    objDetail.DetailType = departmentMaster.Value.AccountType;
                                                    objDetail.QBDetailType = departmentMaster.Value.QBDetailType;
                                                    objDetail.AccountTypeId = objDept.AccountTypeId;
                                                    //This db class is save Account type details
                                                    _departmentMastersRepository.SaveAccountDetailTypeMasters(objDetail);
                                                    objDetail = null;
                                                }
                                            }
                                            else
                                            {
                                                AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
                                                objDetail.DetailType = departmentMaster.Value.AccountType;
                                                objDetail.QBDetailType = departmentMaster.Value.QBDetailType;
                                                objDetail.AccountTypeId = objDept.AccountTypeId;
                                                //This db class is save Account type details
                                                _departmentMastersRepository.SaveAccountDetailTypeMasters(objDetail);
                                                int id = objDetail.AccountDetailTypeId;
                                                objDetail = null;
                                                objDept.AccountDetailTypeId = id;
                                            }

                                            objDept.Description = departmentMaster.Value.Description == null ? null : departmentMaster.Value.Description.ToUpper();
                                            objDept.AccountNumber = departmentMaster.Value.AccountNumber == null ? null : departmentMaster.Value.AccountNumber.ToUpper();
                                            objDept.IsSubAccount = departmentMaster.Value.IsSubAccount;
                                            objDept.IsActive = Convert.ToBoolean(departmentMaster.Value.IsActive);
                                            objDept.StoreId = departmentMastersViewModal.StoreId;
                                            objDept.ModifiedOn = DateTime.Now;
                                            objDept.LastModifiedOn = DateTime.Now;
                                            //Get UserID by Username.
                                            objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                            objDept.IsSync = 0;
                                            objDept.SyncDate = DateTime.Now;
                                            //This db class is update department details
                                            _departmentMastersRepository.UpdateDepartmentMaster(objDept);
                                            iDeptId = objDept.DepartmentId;
                                            if (iDeptId == 0)
                                            {
                                                //Message = "Department Not Updated.";
                                                Message = _commonRepository.GetMessageValue("DMDU", "Department Not Updated.");
                                                departmentMastersViewModal.error += departmentMaster.Value.DepartmentName + ":" + Message;

                                            }
                                            else
                                            {
                                                // TempData["Sucessme"] = "Department Save Successfully..";
                                            }
                                            objDept = null;
                                        }
                                        else
                                        {
                                            //Select for update data.
                                            var vData = _departmentMastersRepository.SelectForUpdate(departmentMastersViewModal.StoreId, departmentMaster.Value.DepartmentName, departmentMaster.Value.DepartmentId);
                                            if (vData.Count > 0)
                                            {
                                                //Message = "Department Name Already Exist";
                                                Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                                departmentMastersViewModal.error += vData.FirstOrDefault().DepartmentName + ":" + Message;
                                            }
                                            DepartmentMaster objDept = new DepartmentMaster();
                                            objDept.ListId = objResponse.ID;
                                            objDept.DepartmentName = departmentMaster.Value.DepartmentName == null ? null : departmentMaster.Value.DepartmentName.ToUpper();
                                            //get Accont type.
                                            var DtAcc = _departmentMastersRepository.GetAccountType1(QBAccountType);
                                            if (DtAcc.Count > 0)
                                            {
                                                objDept.AccountTypeId = DtAcc.FirstOrDefault().AccountTypeId;
                                            }
                                            else
                                            {
                                                AccountTypeMaster objAcc = new AccountTypeMaster();
                                                objAcc.AccountType = departmentMaster.Value.AccountType;
                                                objAcc.Flag = "D";
                                                objAcc.CommonType = departmentMaster.Value.AccountType;
                                                //This db class is save Account type details
                                                _departmentMastersRepository.SaveAccountTypeMasters(objAcc);
                                                int id = objAcc.AccountTypeId;
                                                objAcc = null;
                                                objDept.AccountTypeId = id;
                                            }
                                            //Get QB details.
                                            var DtAccDetail = _departmentMastersRepository.GetQBDetailType(DetailsType);
                                            if (DtAccDetail.Count > 0)
                                            {
                                                objDept.AccountDetailTypeId = DtAccDetail.FirstOrDefault().AccountDetailTypeId;
                                                if (objDept.AccountDetailTypeId == 0 || objDept.AccountDetailTypeId == null)
                                                {
                                                    AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
                                                    objDetail.DetailType = departmentMaster.Value.AccountDetailType;
                                                    objDetail.QBDetailType = departmentMaster.Value.QBDetailType;
                                                    objDetail.AccountTypeId = objDept.AccountTypeId;
                                                    //This db class is save Account type details
                                                    _departmentMastersRepository.SaveAccountDetailTypeMasters(objDetail);
                                                    objDetail = null;
                                                }
                                            }
                                            else
                                            {
                                                AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
                                                objDetail.DetailType = departmentMaster.Value.AccountDetailType;
                                                objDetail.QBDetailType = departmentMaster.Value.QBDetailType;
                                                objDetail.AccountTypeId = objDept.AccountTypeId;
                                                //This db class is save Account type details
                                                _departmentMastersRepository.SaveAccountDetailTypeMasters(objDetail);
                                                int id = objDetail.AccountDetailTypeId;
                                                objDetail = null;
                                                objDept.AccountDetailTypeId = id;
                                            }
                                            objDept.Description = departmentMaster.Value.Description == null ? null : departmentMaster.Value.Description.ToUpper();
                                            objDept.AccountNumber = departmentMaster.Value.AccountNumber == null ? null : departmentMaster.Value.AccountNumber.ToUpper();
                                            objDept.IsSubAccount = departmentMaster.Value.IsSubAccount;
                                            objDept.IsActive = Convert.ToBoolean(departmentMaster.Value.IsActive);
                                            objDept.StoreId = departmentMastersViewModal.StoreId;
                                            objDept.CreatedOn = DateTime.Now;
                                            objDept.ModifiedOn = DateTime.Now;
                                            objDept.LastModifiedOn = DateTime.Now;
                                            objDept.CreatedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                            objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                            objDept.IsSync = 0;
                                            objDept.SyncDate = DateTime.Now;
                                            //This db class is save Department details
                                            _departmentMastersRepository.SaveDepartmentMaster(objDept);
                                            objDept = null;
                                        }
                                    }
                                }
                                else
                                {
                                    //Get all Account type list.
                                    var Dt = _departmentMastersRepository.AccountTypeList().Where(a => a.AccountTypeId == departmentMaster.Value.AccountTypeId).ToList();
                                    string QBAccountType = "";
                                    int AccountID = 0;
                                    if (Dt.Count > 0)
                                    {
                                        QBAccountType = Dt.FirstOrDefault().AccountType.ToString();
                                        AccountID = Dt.FirstOrDefault().AccountTypeId;
                                    }
                                    departmentMaster.Value.AccountTypeId = AccountID;
                                    //using this db class get select by storeID name
                                    var vData1 = _departmentMastersRepository.SelectByStoreID_Name1(departmentMastersViewModal.StoreId, departmentMaster.Value.DepartmentName);
                                    if (vData1 != null)
                                    {     //Select for update data.
                                        var vData = _departmentMastersRepository.SelectForUpdate(departmentMastersViewModal.StoreId, departmentMaster.Value.DepartmentName, departmentMaster.Value.DepartmentId);
                                        if (vData.Count > 0)
                                        {
                                           //Message = "Department Name Already Exist";
                                            Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                            departmentMastersViewModal.error += vData.FirstOrDefault().DepartmentName + ":" + Message;
                                        }
                                        //get Department master list
                                        DepartmentMaster objDept = _departmentMastersRepository.DepartmentMasterList().Where(a => a.DepartmentId == vData1.DepartmentId).FirstOrDefault();
                                        objDept.ListId = vData1.ListId;
                                        objDept.DepartmentName = departmentMaster.Value.DepartmentName == null ? null : departmentMaster.Value.DepartmentName.ToUpper();
                                        objDept.AccountTypeId = departmentMaster.Value.AccountTypeId;
                                        objDept.AccountDetailTypeId = departmentMaster.Value.AccountDetailTypeId;
                                        objDept.Description = departmentMaster.Value.Description == null ? null : departmentMaster.Value.Description.ToUpper();
                                        objDept.AccountNumber = departmentMaster.Value.AccountNumber == null ? null : departmentMaster.Value.AccountNumber.ToUpper();
                                        objDept.IsActive = Convert.ToBoolean(departmentMaster.Value.IsActive);
                                        objDept.StoreId = departmentMastersViewModal.StoreId;
                                        objDept.ModifiedOn = DateTime.Now;
                                        objDept.LastModifiedOn = DateTime.Now;
                                        //get userid by username.
                                        objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                        objDept.IsSync = 0;
                                        objDept.SyncDate = DateTime.Now;
                                        //This db class is update Department details
                                        _departmentMastersRepository.UpdateDepartmentMaster(objDept);
                                        //db.Entry(objDept).State = EntityState.Modified;
                                        //db.SaveChanges();
                                        iDeptId = objDept.DepartmentId;
                                        if (iDeptId == 0)
                                        {
                                            //Message = "Department Not Updated.";
                                            Message = _commonRepository.GetMessageValue("DMDU", "Department Not Updated.");
                                            departmentMastersViewModal.error += vData1.DepartmentName + ":" + Message;

                                        }
                                        else
                                        {
                                            //TempData["Sucessme"] = "Department Save Successfully..";
                                        }
                                    }
                                    else
                                    {
                                        //Select for update data.
                                        var vData = _departmentMastersRepository.SelectForUpdate(departmentMastersViewModal.StoreId, departmentMaster.Value.DepartmentName, departmentMaster.Value.DepartmentId);
                                        if (vData.Count > 0)
                                        {
                                            //Message = "Department Name Already Exist";
                                            Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                            departmentMastersViewModal.error += vData.FirstOrDefault().DepartmentName + ":" + Message;
                                        }
                                        DepartmentMaster objDept = new DepartmentMaster();
                                        objDept.ListId = objResponse.ID;
                                        objDept.DepartmentName = departmentMaster.Value.DepartmentName == null ? null : departmentMaster.Value.DepartmentName.ToUpper();
                                        objDept.AccountTypeId = departmentMaster.Value.AccountTypeId;
                                        objDept.AccountDetailTypeId = departmentMaster.Value.AccountDetailTypeId;
                                        objDept.Description = departmentMaster.Value.Description == null ? null : departmentMaster.Value.Description.ToUpper();
                                        objDept.AccountNumber = departmentMaster.Value.AccountNumber == null ? null : departmentMaster.Value.AccountNumber.ToUpper();
                                        objDept.IsSubAccount = departmentMaster.Value.IsSubAccount;
                                        objDept.IsActive = Convert.ToBoolean(departmentMaster.Value.IsActive);
                                        objDept.StoreId = departmentMastersViewModal.StoreId;
                                        objDept.CreatedOn = DateTime.Now;
                                        objDept.ModifiedOn = DateTime.Now;
                                        objDept.LastModifiedOn = DateTime.Now;
                                        objDept.CreatedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                        objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                        objDept.IsSync = 0;
                                        objDept.SyncDate = DateTime.Now;
                                        //This db class is save Department details
                                        _departmentMastersRepository.SaveDepartmentMaster(objDept);
                                        objDept = null;
                                    }
                                }
                            }
                        }
                        else
                        {
                            TempData["sMessage"] = "success";
                        }
                    }
                    //db.DepartmentMasters.AddRange(DepartmentList);
                    //await db.SaveChangesAsync();

                    if (TempData["Sucessme"] != null)
                    {

                    }
                    else
                    {
                        //TempData["Sucessme"] = "Department Save Successfully..";
                        TempData["Sucessme"] = _commonRepository.GetMessageValue("DMDSS", "Department Save Successfully..");
                        //StatusMessageString = "Department Created Successfully..";
                        StatusMessageString = _commonRepository.GetMessageValue("DMC", "Department Created Successfully..");
                        departmentMastersViewModal.Sucsses = StatusMessageString;
                    }
                    Session["Sucessmesg"] = TempData["Sucessme"].ToString();
                    TempData["Sucessme"] = null;

                }

                catch (Exception ex)
                {
                    departmentMastersViewModal.error = ex.Message.ToString();
                    logger.Error("ChartofAccountsController - InsertDepartment - " + DateTime.Now + " - " + ex.Message.ToString());
                   
                }

            }
            //Get All Account Details type List
            ViewBag.AccountDetailTypeId = new SelectList(_departmentMastersRepository.AccountDetailTypeList().Select(s => new { AccountDetailTypeId = s.AccountDetailTypeId, DetailType = s.DetailType }).OrderBy(o => o.DetailType).ToList(), "AccountDetailTypeId", "DetailType", departmentMaster.Value.AccountDetailTypeId);
            //Get All Account type List
            ViewBag.AccountTypeId = new SelectList(_departmentMastersRepository.AccountTypeList().Select(s => new { s.AccountTypeId, s.AccountType }).OrderBy(o => o.AccountType), "AccountTypeId", "AccountType", departmentMaster.Value.AccountTypeId);
            //ViewBag.StoreId = new SelectList(db.StoreMasters.Where(k => k.IsActive == true).Select(s => new { s.StoreId, s.Name }).OrderBy(o => o.Name).ToList(), "StoreId", "Name", departmentMaster.Value.StoreId);
            //ViewBag.MultiStoreId = new MultiSelectList(db.StoreMasters.Where(k => k.IsActive == true).Select(s => new { s.StoreId, s.Name }).OrderBy(o => o.Name).ToList(), "StoreId", "Name", departmentMaster.Value.MultiStoreId);
            //get StoreList with role Wise.
            var StoreList = _commonRepository.GetStoreList_RoleWise(11, "CreateQBDepartment", UserName).Select(s => s.StoreId);
            //using this class get Store master list with selection of multistoreid.

            ViewBag.MultiStoreId = new MultiSelectList(_departmentMastersRepository.StoreMasterList().Where(s => s.IsActive == true && StoreList.Contains(s.StoreId)).Select(s => new { s.StoreId, s.Name }).OrderBy(o => o.Name).ToList(), "StoreId", "Name", departmentMaster.Value.MultiStoreId);
            //using this class get Store master list with selection of storeid.
            ViewBag.StoreId = new SelectList(_departmentMastersRepository.StoreMasterList().Where(k => k.IsActive == true && StoreList.Contains(k.StoreId)).Select(s => new { s.StoreId, s.Name }).OrderBy(o => o.Name).ToList(), "StoreId", "Name", departmentMaster.Value.StoreId);
            return Json(new { data = departmentMaster.Value, success = departmentMastersViewModal.Sucsses, Error = departmentMastersViewModal.error });
        }

        /// <summary>
        /// This methos is Update Department. 
        /// </summary>
        /// <param name="departmentMaster"></param>
        /// <returns></returns>
        public async Task<ActionResult> UpdateDepartment(CRUDModel<DepartmentMaster> departmentMaster)
        {
            int iDeptId = 0;
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            if (ModelState.IsValid)
            {
                try
                {
                    //Using this db class get stores on Line desktop.
                    string Store = _qBRepository.GetStoreOnlineDesktop(Convert.ToInt32(departmentMaster.Value.StoreId));
                    //Using this db class get stores on Line desktop Flag.
                    int StoreFlag = _qBRepository.GetStoreOnlineDesktopFlag(Convert.ToInt32(departmentMaster.Value.StoreId));
                    if (Store != "")
                    {
                        string DetailsType = "";
                        string AccountType = "";
                        //Get all Account type list.
                        var AccType = _departmentMastersRepository.AccountTypeList().Where(a => a.AccountTypeId == departmentMaster.Value.AccountTypeId).ToList();
                        if (AccType.Count > 0)
                        {
                            AccountType = AccType.FirstOrDefault().AccountType.ToString();
                        }
                        //Get All Account Details type List
                        var Dte = _departmentMastersRepository.AccountDetailTypeList().Where(a => a.DetailType == AccountType).ToList();
                        if (Dte.Count > 0)
                        {
                            DetailsType = Dte.FirstOrDefault().QBDetailType.ToString();
                        }
                        else
                        {
                            DetailsType = departmentMaster.Value.AccountDetailType;
                        }

                        if (departmentMaster.Value.IsSubAccount == "" || departmentMaster.Value.IsSubAccount == "0")
                        {
                            departmentMaster.Value.IsSubAccount = null;
                        }
                        QBResponse objResponse = new QBResponse();
                        if (Store == "Online" && StoreFlag == 1)
                        {
                            if (StoreFlag == 1)
                            {
                                ////Get all Department Master list with selection.
                                var DeptExist = _departmentMastersRepository.DepartmentMasterList().Where(a => a.DepartmentId == departmentMaster.Value.DepartmentId).FirstOrDefault();
                                if (DeptExist != null)
                                {
                                    //Using this db class get QB Edit sync Deaprtment.
                                    _qBRepository.QBEditSyncDepartment(departmentMaster.Value.DepartmentName, AccountType, DetailsType, departmentMaster.Value.Description, departmentMaster.Value.AccountNumber, departmentMaster.Value.IsSubAccount, DeptExist.ListId, Convert.ToInt32(departmentMaster.Value.StoreId), ref objResponse);
                                    if ((objResponse.ID != "0" && objResponse.ID != "") || objResponse.Status == "Done")
                                    {
                                        //using this db class get select by storeID list
                                        var vData1 = _departmentMastersRepository.SelectByStoreID_ListID(departmentMaster.Value.StoreId, objResponse.ID);
                                        if (vData1.Count > 0)
                                        {
                                            //Select for update data.
                                            var vData = _departmentMastersRepository.SelectForUpdate(departmentMaster.Value.StoreId, departmentMaster.Value.DepartmentName, departmentMaster.Value.DepartmentId);
                                            if (vData.Count > 0)
                                            {
                                                //Message = "Department Name Already Exist";
                                                Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                                departmentMastersViewModal.error += vData.FirstOrDefault().DepartmentName + ":" + Message;
                                                return Json(new { data = departmentMaster.Value, success = departmentMastersViewModal.Sucsses, Error = departmentMastersViewModal.error });
                                            }
                                            int iDeptID = vData1.LastOrDefault().DepartmentId;
                                            //Get all Department Master list with selection.
                                            DepartmentMaster objDept = _departmentMastersRepository.DepartmentMasterList().Where(a => a.DepartmentId == iDeptID).FirstOrDefault();
                                            //db.Database.SqlQuery<DepartmentMaster>("SP_DepartmentMaster @Mode = {0},@StoreId = {1},@DepartmentId = {2}", "SelectByStoreID_DepartmentId", departmentMaster.StoreId, iDeptID).FirstOrDefault();
                                            objDept.DepartmentId = departmentMaster.Value.DepartmentId;
                                            objDept.ListId = objResponse.ID;
                                            objDept.DepartmentName = departmentMaster.Value.DepartmentName == null ? null : departmentMaster.Value.DepartmentName.ToUpper();
                                            objDept.Description = departmentMaster.Value.Description == null ? null : departmentMaster.Value.Description.ToUpper();
                                            objDept.AccountNumber = departmentMaster.Value.AccountNumber == null ? null : departmentMaster.Value.AccountNumber.ToUpper();
                                            objDept.IsActive = Convert.ToBoolean(departmentMaster.Value.IsActive);
                                            objDept.StoreId = departmentMaster.Value.StoreId;
                                            objDept.ModifiedOn = DateTime.Now;
                                            objDept.LastModifiedOn = DateTime.Now;
                                            objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                            objDept.IsSync = 1;
                                            objDept.SyncDate = DateTime.Now;
                                            //This db class is update Department details
                                            _departmentMastersRepository.UpdateDepartmentMaster(objDept);

                                            iDeptId = objDept.DepartmentId;
                                            if (iDeptId == 0)
                                            {
                                                //TempData["Sucessme"] = "Department Not Synched in QuickBook.";
                                                TempData["Sucessme"] = _commonRepository.GetMessageValue("DMSQ", "Department Not Synched in QuickBook");
                                                //departmentMastersViewModal.error = "Department Not Synched in QuickBook.";
                                                departmentMastersViewModal.error = _commonRepository.GetMessageValue("DMSQ", "Department Not Synched in QuickBook");
                                                return Json(new { data = departmentMaster.Value, success = departmentMastersViewModal.Sucsses, Error = departmentMastersViewModal.error });
                                            }
                                            else
                                            {
                                                //departmentMastersViewModal.Sucsses = "Department Update Successfully..";
                                                departmentMastersViewModal.Sucsses = _commonRepository.GetMessageValue("DMUS", "Department Update Successfully..");
                                            }
                                            objDept = null;
                                        }
                                        else
                                        {
                                            //Select for update data.
                                            var vData = _departmentMastersRepository.SelectForUpdate(departmentMaster.Value.StoreId, departmentMaster.Value.DepartmentName, departmentMaster.Value.DepartmentId);
                                            if (vData.Count > 0)
                                            {
                                               // Message = "Department Name Already Exist";
                                                Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                                departmentMastersViewModal.error += vData.FirstOrDefault().DepartmentName + ":" + Message;
                                                return Json(new { data = departmentMaster.Value, success = departmentMastersViewModal.Sucsses, Error = departmentMastersViewModal.error });
                                            }
                                            DepartmentMaster objDept = new DepartmentMaster();
                                            objDept.ListId = objResponse.ID;
                                            objDept.DepartmentName = departmentMaster.Value.DepartmentName == null ? null : departmentMaster.Value.DepartmentName.ToUpper();
                                            objDept.AccountTypeId = departmentMaster.Value.AccountTypeId;
                                            objDept.AccountDetailTypeId = departmentMaster.Value.AccountDetailTypeId;
                                            objDept.Description = departmentMaster.Value.Description == null ? null : departmentMaster.Value.Description.ToUpper();
                                            objDept.AccountNumber = departmentMaster.Value.AccountNumber == null ? null : departmentMaster.Value.AccountNumber.ToUpper();
                                            objDept.IsSubAccount = departmentMaster.Value.IsSubAccount;
                                            objDept.IsActive = Convert.ToBoolean(departmentMaster.Value.IsActive);
                                            objDept.StoreId = departmentMaster.Value.StoreId;
                                            objDept.CreatedOn = DateTime.Now;
                                            objDept.ModifiedOn = DateTime.Now;
                                            objDept.LastModifiedOn = DateTime.Now;
                                            objDept.CreatedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                            objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                            objDept.IsSync = 1;
                                            objDept.SyncDate = DateTime.Now;
                                            //This db class is save Department details
                                            _departmentMastersRepository.SaveDepartmentMaster(objDept);
                                            objDept = null;
                                        }
                                    }
                                    else
                                    {
                                        Message = objResponse.Status.ToString();
                                    }
                                }
                                else
                                {
                                    //Select for update data.
                                    var vData = _departmentMastersRepository.SelectForUpdate(departmentMaster.Value.StoreId, departmentMaster.Value.DepartmentName, departmentMaster.Value.DepartmentId);
                                    if (vData.Count > 0)
                                    {
                                        //Message = "Department Name Already Exist";
                                        Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                        departmentMastersViewModal.error = Message;
                                        Session["Sucessmesg"] = Message;
                                        return Json(new { data = departmentMaster.Value, success = departmentMastersViewModal.Sucsses, Error = departmentMastersViewModal.error });
                                    }
                                    //get QB Sync Deparment.
                                    _qBRepository.QBSyncDepartment(departmentMaster.Value.DepartmentName, AccountType, DetailsType, departmentMaster.Value.Description, departmentMaster.Value.AccountNumber, departmentMaster.Value.IsSubAccount, Convert.ToInt32(departmentMaster.Value.StoreId), ref objResponse);
                                    if (objResponse.ID != "0" || objResponse.Status == "Done")
                                    {
                                        DepartmentMaster objDept = new DepartmentMaster();
                                        objDept.ListId = objResponse.ID;
                                        objDept.DepartmentName = departmentMaster.Value.DepartmentName;
                                        objDept.AccountTypeId = departmentMaster.Value.AccountTypeId;
                                        objDept.AccountDetailTypeId = departmentMaster.Value.AccountDetailTypeId;
                                        objDept.Description = departmentMaster.Value.Description;
                                        objDept.AccountNumber = departmentMaster.Value.AccountNumber;
                                        objDept.IsSubAccount = departmentMaster.Value.IsSubAccount;
                                        objDept.IsActive = Convert.ToBoolean(departmentMaster.Value.IsActive);
                                        objDept.StoreId = departmentMaster.Value.StoreId;
                                        objDept.CreatedOn = DateTime.Now;
                                        objDept.ModifiedOn = DateTime.Now;
                                        objDept.LastModifiedOn = DateTime.Now;
                                        objDept.CreatedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                        objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                        objDept.IsSync = 1;
                                        objDept.SyncDate = DateTime.Now;
                                        //This db class is save Department details
                                        _departmentMastersRepository.SaveDepartmentMaster(objDept);
                                        objDept = null;
                                    }
                                }
                            }
                            else
                            {
                                var objDept1 = _departmentMastersRepository.SelectByStoreID_DepId(departmentMaster.Value.StoreId, departmentMaster.Value.DepartmentId);
                                if (objDept1 != null)
                                {
                                    //Select for update data.
                                    var vData = _departmentMastersRepository.SelectForUpdate(departmentMaster.Value.StoreId, departmentMaster.Value.DepartmentName, departmentMaster.Value.DepartmentId);
                                    if (vData.Count > 0)
                                    {
                                        //Message = "Department Name Already Exist";
                                        Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                        departmentMastersViewModal.error = Message;
                                        Session["Sucessmesg"] = Message;
                                        return Json(new { data = departmentMaster.Value, success = departmentMastersViewModal.Sucsses, Error = departmentMastersViewModal.error });
                                    }
                                    //Get all Department Master list with selection.
                                    DepartmentMaster objDept = _departmentMastersRepository.DepartmentMasterList().Where(a => a.DepartmentId == objDept1.DepartmentId).FirstOrDefault();
                                    objDept.ListId = objDept1.ListId;
                                    objDept.DepartmentName = departmentMaster.Value.DepartmentName;
                                    objDept.Description = departmentMaster.Value.Description;
                                    objDept.AccountNumber = departmentMaster.Value.AccountNumber;
                                    objDept.IsActive = Convert.ToBoolean(departmentMaster.Value.IsActive);
                                    objDept.StoreId = departmentMaster.Value.StoreId;
                                    objDept.LastModifiedOn = DateTime.Now;
                                    objDept.ModifiedOn = DateTime.Now;
                                    objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                    objDept.IsSync = 0;
                                    objDept.SyncDate = DateTime.Now;
                                    //This db class is update Department details
                                    _departmentMastersRepository.UpdateDepartmentMaster(objDept);
                                    iDeptId = objDept.DepartmentId;
                                    if (iDeptId == 0)
                                    {
                                        //Message = "Department Not Updated.";
                                        Message = _commonRepository.GetMessageValue("DMDU", "Department Not Updated.");
                                        departmentMastersViewModal.error = Message;
                                        Session["Sucessmesg"] = Message;
                                        return Json(new { data = departmentMaster.Value, success = departmentMastersViewModal.Sucsses, Error = departmentMastersViewModal.error });
                                    }
                                    else
                                    {
                                        //departmentMastersViewModal.Sucsses = "Department Update Successfully..";
                                        departmentMastersViewModal.Sucsses = _commonRepository.GetMessageValue("DMUS", "Department Update Successfully..");
                                    }
                                }
                                else
                                {
                                    //Select for update data.
                                    var vData = _departmentMastersRepository.SelectForUpdate(departmentMaster.Value.StoreId, departmentMaster.Value.DepartmentName, departmentMaster.Value.DepartmentId);
                                    if (vData.Count > 0)
                                    {
                                        Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist"); ;
                                        departmentMastersViewModal.error = Message;
                                        Session["Sucessmesg"] = Message;
                                        return Json(new { data = departmentMaster.Value, success = departmentMastersViewModal.Sucsses, Error = departmentMastersViewModal.error }); return Json(new { data = departmentMaster.Value, success = departmentMastersViewModal.Sucsses, Error = departmentMastersViewModal.error });
                                    }

                                    DepartmentMaster objDept = new DepartmentMaster();
                                    objDept.ListId = objResponse.ID;
                                    objDept.DepartmentName = departmentMaster.Value.DepartmentName;
                                    objDept.AccountTypeId = departmentMaster.Value.AccountTypeId;
                                    objDept.AccountDetailTypeId = departmentMaster.Value.AccountDetailTypeId;
                                    objDept.Description = departmentMaster.Value.Description;
                                    objDept.AccountNumber = departmentMaster.Value.AccountNumber;
                                    objDept.IsSubAccount = departmentMaster.Value.IsSubAccount;
                                    objDept.IsActive = Convert.ToBoolean(departmentMaster.Value.IsActive);
                                    objDept.StoreId = departmentMaster.Value.StoreId;
                                    objDept.CreatedOn = DateTime.Now;
                                    objDept.ModifiedOn = DateTime.Now;
                                    objDept.LastModifiedOn = DateTime.Now;
                                    objDept.CreatedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                    objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                    objDept.IsSync = 0;
                                    objDept.SyncDate = DateTime.Now;
                                    //This db class is save Department details
                                    _departmentMastersRepository.SaveDepartmentMaster(objDept);
                                    objDept = null;
                                    //departmentMastersViewModal.Sucsses = "Department Save Successfully..";
                                    departmentMastersViewModal.Sucsses = _commonRepository.GetMessageValue("DMDSS", "Department Save Successfully..");
                                }
                            }
                        }
                        else
                        {
                            var vData1 = _departmentMastersRepository.SelectByStoreID_DepId(departmentMaster.Value.StoreId, departmentMaster.Value.DepartmentId);
                            if (vData1 != null)
                            { //Select for update data.
                                var vData = _departmentMastersRepository.SelectForUpdate(departmentMaster.Value.StoreId, departmentMaster.Value.DepartmentName, departmentMaster.Value.DepartmentId);
                                if (vData.Count > 0)
                                {
                                    Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                    departmentMastersViewModal.error = Message;
                                    Session["Sucessmesg"] = Message;
                                    return Json(new { data = departmentMaster.Value, success = departmentMastersViewModal.Sucsses, Error = departmentMastersViewModal.error });
                                }
                                //Get all Department Master list with selection.
                                DepartmentMaster objDept = _departmentMastersRepository.DepartmentMasterList().Where(a => a.DepartmentId == vData1.DepartmentId).FirstOrDefault();
                                objDept.ListId = vData1.ListId;
                                objDept.DepartmentName = departmentMaster.Value.DepartmentName;
                                objDept.Description = departmentMaster.Value.Description;
                                objDept.AccountNumber = departmentMaster.Value.AccountNumber;
                                objDept.IsActive = Convert.ToBoolean(departmentMaster.Value.IsActive);
                                objDept.StoreId = departmentMaster.Value.StoreId;
                                objDept.ModifiedOn = DateTime.Now;
                                objDept.LastModifiedOn = DateTime.Now;
                                objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                objDept.IsSync = 0;
                                objDept.SyncDate = DateTime.Now;
                                //This db class is update Department details
                                _departmentMastersRepository.UpdateDepartmentMaster(objDept);
                                iDeptId = objDept.DepartmentId;
                                if (iDeptId == 0)
                                {
                                    //Message = "Department Not Updated.";
                                    Message = _commonRepository.GetMessageValue("DMDU", "Department Not Updated.");
                                    departmentMastersViewModal.error = Message;
                                    Session["Sucessmesg"] = Message;
                                    return Json(new { data = departmentMaster.Value, success = departmentMastersViewModal.Sucsses, Error = departmentMastersViewModal.error });
                                }
                                else
                                {
                                    departmentMastersViewModal.Sucsses = _commonRepository.GetMessageValue("DMUS", "Department Update Successfully..");
                                }
                            }
                            else
                            {
                                //Select for update data.
                                var vData = _departmentMastersRepository.SelectForUpdate(departmentMaster.Value.StoreId, departmentMaster.Value.DepartmentName, departmentMaster.Value.DepartmentId);
                                if (vData.Count > 0)
                                {
                                    Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                    departmentMastersViewModal.Sucsses = Message;
                                    Session["Sucessmesg"] = Message;
                                    return Json(new { data = departmentMaster.Value, success = departmentMastersViewModal.Sucsses, Error = departmentMastersViewModal.error });
                                }

                                DepartmentMaster objDept = new DepartmentMaster();
                                objDept.ListId = objResponse.ID;
                                objDept.DepartmentName = departmentMaster.Value.DepartmentName;
                                objDept.AccountTypeId = departmentMaster.Value.AccountTypeId;
                                objDept.AccountDetailTypeId = departmentMaster.Value.AccountDetailTypeId;
                                objDept.Description = departmentMaster.Value.Description;
                                objDept.AccountNumber = departmentMaster.Value.AccountNumber;
                                objDept.IsSubAccount = departmentMaster.Value.IsSubAccount;
                                objDept.IsActive = Convert.ToBoolean(departmentMaster.Value.IsActive);
                                objDept.StoreId = departmentMaster.Value.StoreId;
                                objDept.CreatedOn = DateTime.Now;
                                objDept.ModifiedOn = DateTime.Now;
                                objDept.LastModifiedOn = DateTime.Now;
                                objDept.CreatedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                objDept.IsSync = 0;
                                objDept.SyncDate = DateTime.Now;
                                //This db class is save Department details
                                _departmentMastersRepository.SaveDepartmentMaster(objDept);
                                objDept = null;
                                departmentMastersViewModal.Sucsses = _commonRepository.GetMessageValue("DMDSS", "Department Save Successfully..");
                            }
                        }
                    }
                    else
                    {
                        var vData1 = _departmentMastersRepository.SelectByStoreID_DepId(departmentMaster.Value.StoreId, departmentMaster.Value.DepartmentId);
                        if (vData1 != null)
                        { //Select for update data.
                            var vData = _departmentMastersRepository.SelectForUpdate(departmentMaster.Value.StoreId, departmentMaster.Value.DepartmentName, departmentMaster.Value.DepartmentId);
                            if (vData.Count > 0)
                            {
                                Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                departmentMastersViewModal.error = Message;
                                Session["Sucessmesg"] = Message;
                                return Json(new { data = departmentMaster.Value, success = departmentMastersViewModal.Sucsses, Error = departmentMastersViewModal.error });
                            }
                            //Get all Department Master list with selection.
                            DepartmentMaster objDept = _departmentMastersRepository.DepartmentMasterList().Where(a => a.DepartmentId == vData1.DepartmentId).FirstOrDefault();
                            objDept.ListId = vData1.ListId;
                            objDept.DepartmentName = departmentMaster.Value.DepartmentName;
                            objDept.Description = departmentMaster.Value.Description;
                            objDept.AccountNumber = departmentMaster.Value.AccountNumber;
                            objDept.AccountTypeId = departmentMaster.Value.AccountTypeId;
                            objDept.AccountDetailTypeId = departmentMaster.Value.AccountDetailTypeId;
                            objDept.IsActive = Convert.ToBoolean(departmentMaster.Value.IsActive);
                            objDept.StoreId = departmentMaster.Value.StoreId;
                            objDept.ModifiedOn = DateTime.Now;
                            objDept.LastModifiedOn = DateTime.Now;
                            //Get UserID by Username.
                            objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                            objDept.IsSync = 0;
                            objDept.SyncDate = DateTime.Now;
                            //This db class is save Department details
                            _departmentMastersRepository.UpdateDepartmentMaster(objDept);
                            iDeptId = objDept.DepartmentId;
                            if (iDeptId == 0)
                            {
                                Message = _commonRepository.GetMessageValue("DMDU", "Department Not Updated.");
                                departmentMastersViewModal.error = Message;
                                Session["Sucessmesg"] = Message;
                                return Json(new { data = departmentMaster.Value, success = departmentMastersViewModal.Sucsses, Error = departmentMastersViewModal.error });
                            }
                            else
                            {
                                departmentMastersViewModal.Sucsses = _commonRepository.GetMessageValue("DMUS", "Department Update Successfully..");
                            }
                        }
                        else
                        {
                            //Select for update data.
                            var vData = _departmentMastersRepository.SelectForUpdate(departmentMaster.Value.StoreId, departmentMaster.Value.DepartmentName, departmentMaster.Value.DepartmentId);
                            if (vData.Count > 0)
                            {
                                Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                departmentMastersViewModal.error = Message;
                                Session["Sucessmesg"] = Message;
                                return Json(new { data = departmentMaster.Value, success = departmentMastersViewModal.Sucsses, Error = departmentMastersViewModal.error });
                            }

                            DepartmentMaster objDept = new DepartmentMaster();
                            objDept.ListId = objResponse.ID;
                            objDept.DepartmentName = departmentMaster.Value.DepartmentName;
                            objDept.AccountTypeId = departmentMaster.Value.AccountTypeId;
                            objDept.AccountDetailTypeId = departmentMaster.Value.AccountDetailTypeId;
                            objDept.Description = departmentMaster.Value.Description;
                            objDept.AccountNumber = departmentMaster.Value.AccountNumber;
                            objDept.IsSubAccount = departmentMaster.Value.IsSubAccount;
                            objDept.IsActive = Convert.ToBoolean(departmentMaster.Value.IsActive);
                            objDept.StoreId = departmentMaster.Value.StoreId;
                            objDept.CreatedOn = DateTime.Now;
                            objDept.ModifiedOn = DateTime.Now;
                            objDept.LastModifiedOn = DateTime.Now;
                            objDept.CreatedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                            objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                            objDept.IsSync = 0;
                            objDept.SyncDate = DateTime.Now;
                            //This db class is save Department details
                            _departmentMastersRepository.SaveDepartmentMaster(objDept);
                            objDept = null;
                            departmentMastersViewModal.Sucsses = _commonRepository.GetMessageValue("DMDSS", "Department Save Successfully..");
                        }
                    }
                    //db.Entry(departmentMaster).State = EntityState.Modified;
                    //await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    logger.Error("ChartofAccountsController - UpdateDepartment - " + DateTime.Now + " - " + ex.Message.ToString());
                 
                }
            }
            //Get All Account Details type List
            ViewBag.AccountDetailTypeId = new SelectList(_departmentMastersRepository.AccountDetailTypeList().Select(s => new { s.AccountDetailTypeId, s.DetailType }).OrderBy(o => o.DetailType).ToList(), "AccountDetailTypeId", "DetailType", departmentMaster.Value.AccountDetailTypeId);
            //Get all Account type list.
            ViewBag.AccountTypeId = new SelectList(_departmentMastersRepository.AccountTypeList().Select(s => new { s.AccountTypeId, s.AccountType }).OrderBy(o => o.AccountType), "AccountTypeId", "AccountType", departmentMaster.Value.AccountTypeId);
            //using this class get Store master list with storeid.
            ViewBag.StoreId = new SelectList(_departmentMastersRepository.StoreMasterList().Where(k => k.IsActive == true).Select(s => new { s.StoreId, s.Name }).OrderBy(o => o.Name).ToList(), "StoreId", "Name", departmentMaster.Value.StoreId);
            //using this class get Store master list with selection of MultiStoreId.

            ViewBag.MultiStoreId = new MultiSelectList(_departmentMastersRepository.StoreMasterList().Where(k => k.IsActive == true).Select(s => new { s.StoreId, s.Name }).OrderBy(o => o.Name).ToList(), "StoreId", "Name", departmentMaster.Value.MultiStoreId);
            if (TempData["complete"] != null)
            {
                ActivityLog ActLog = new ActivityLog();
                ActLog.Action = 2;
                //This Db class is used for get user firstname.
                ActLog.Comment = "Department " + "<a href='/DepartmentMasters/Details/" + departmentMaster.Value.DepartmentId + "'>" + departmentMaster.Value.DepartmentName + "</a> Edited by " + _commonRepository.getUserFirstName(UserName) + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                //This  class is used for Activity Log Insert.
                _activityLogRepository.ActivityLogInsert(ActLog);


                departmentMastersViewModal.Sucsses = _commonRepository.GetMessageValue("DMUS", "Department Update Successfully..");
                ViewBag.StatusMessageString = departmentMastersViewModal.Sucsses;
                Session["Sucessmesg"] = TempData["complete"].ToString();
                return RedirectToAction("Index", "DepartmentMasters");
            }
            if (departmentMastersViewModal.error != null)
            {
                Session["Sucessmesg"] = TempData["Sucessme"].ToString();
                return Json(new { data = departmentMaster.Value, success = departmentMastersViewModal.Sucsses, Error = departmentMastersViewModal.error });
            }

            return Json(new { data = departmentMaster.Value, success = departmentMastersViewModal.Sucsses, Error = departmentMastersViewModal.error });
        }

        /// <summary>
        /// This methos is Set Account Type.  
        /// </summary>
        /// <returns></returns>
        public ActionResult SetAccountType()
        {
            //Get all Account type list.

            ViewBag.AccountTypeId = new SelectList(_departmentMastersRepository.AccountTypeList().Select(s => new { s.AccountTypeId, s.AccountType }).OrderBy(o => o.AccountType), "AccountTypeId", "AccountType");
            //ViewBag.AccountDetailTypeId = new SelectList(db.AccountDetailTypeMasters.Select(s => new { s.AccountDetailTypeId, s.DetailType }).OrderBy(o => o.DetailType).ToList(), "AccountDetailTypeId", "DetailType");

            return Json(ViewBag.AccountTypeId, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Save Account Type details.
        /// </summary>
        /// <param name="departmentMaster"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(DepartmentMaster departmentMaster)
        {
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            if (ModelState.IsValid)
            {
                try
                {
                    List<DepartmentMaster> DepartmentList = new List<DepartmentMaster>();

                    foreach (var item in departmentMaster.MultiStoreId)
                    {
                        int StoreId = Convert.ToInt32(item);
                        if (StoreId > 0)
                        {
                            if (ModelState.IsValid)
                            {
                                int iDeptId = 0;
                                //Using this db class get stores on Line desktop.
                                string Store = _qBRepository.GetStoreOnlineDesktop(StoreId);
                                //Using this db class get stores on Line desktop Flag.
                                int StoreFlag = _qBRepository.GetStoreOnlineDesktopFlag(StoreId);
                                departmentMaster.IsActive = true;
                                if (Store != "")
                                {
                                    //Get all Account type list.
                                    string AccType = _departmentMastersRepository.AccountTypeList().Where(a => a.AccountTypeId == departmentMaster.AccountTypeId).FirstOrDefault().AccountType;
                                    //Get All Account Details type List
                                    var Dte = _departmentMastersRepository.AccountDetailTypeList().Where(a => a.AccountDetailTypeId == departmentMaster.AccountDetailTypeId).ToList();
                                    string DetailsType = "";
                                    if (Dte.Count > 0)
                                    {
                                        DetailsType = Dte.FirstOrDefault().QBDetailType.ToString();
                                        //departmentMaster.AccountDetailTypeId = Dte.FirstOrDefault().AccountDetailTypeId;
                                    }
                                    if (departmentMaster.IsSubAccount == "")
                                    {
                                        departmentMaster.IsSubAccount = null;
                                    }
                                    if (Store == "Online")
                                    {
                                        //Get all Account type list.
                                        var Dt = _departmentMastersRepository.AccountTypeList().Where(a => a.CommonType == AccType && a.Flag == "O").ToList();
                                        string QBAccountType = "";
                                        int AccountID = 0;
                                        if (Dt.Count > 0)
                                        {
                                            QBAccountType = Dt.FirstOrDefault().AccountType.ToString();
                                            AccountID = Dt.FirstOrDefault().AccountTypeId;
                                        }
                                        departmentMaster.AccountTypeId = AccountID;
                                        if (StoreFlag == 1)
                                        {
                                            QBResponse objResponse = new QBResponse();
                                            _qBRepository.QBSyncDepartment(departmentMaster.DepartmentName, QBAccountType, DetailsType, departmentMaster.Description, departmentMaster.AccountNumber, departmentMaster.IsSubAccount, StoreId, ref objResponse);
                                            if (objResponse.ID != "0" || objResponse.Status == "Done")
                                            {
                                                var vData1 = _departmentMastersRepository.SelectByStoreID_Name(StoreId, departmentMaster.DepartmentName);
                                                if (vData1.Count > 0)
                                                { //Select for update data.
                                                    var vData = _departmentMastersRepository.SelectForUpdate(StoreId, departmentMaster.DepartmentName, departmentMaster.DepartmentId);
                                                    if (vData.Count > 0)
                                                    {
                                                        Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                                        TempData["Sucessme"] += vData.FirstOrDefault().DepartmentName + ":" + Message;
                                                    }
                                                    int iDeptID = vData.LastOrDefault().DepartmentId;
                                                    DepartmentMaster objDept = _departmentMastersRepository.SelectByStoreID_DepartmentId(StoreId, iDeptID);
                                                    objDept.ListId = objResponse.ID;
                                                    objDept.DepartmentName = departmentMaster.DepartmentName == null ? null : departmentMaster.DepartmentName.ToUpper();
                                                    objDept.AccountTypeId = Convert.ToInt32(departmentMaster.AccountType);
                                                    //get Account type
                                                    var DtAcc = _departmentMastersRepository.GetAccountType(AccType);
                                                    if (DtAcc.Count > 0)
                                                    {
                                                        objDept.AccountTypeId = DtAcc.FirstOrDefault().AccountTypeId;
                                                    }
                                                    else
                                                    {
                                                        AccountTypeMaster objAcc = new AccountTypeMaster();
                                                        objAcc.AccountType = departmentMaster.AccountType;
                                                        objAcc.Flag = "O";
                                                        objAcc.CommonType = departmentMaster.AccountType;
                                                        _departmentMastersRepository.SaveAccountTypeMasters(objAcc);
                                                        int id = objAcc.AccountTypeId;
                                                        objAcc = null;
                                                        objDept.AccountTypeId = id;
                                                    }
                                                    //get Qb Details
                                                    var DtAccDetail = _departmentMastersRepository.GetQBDetailType(DetailsType);
                                                    if (DtAccDetail.Count > 0)
                                                    {
                                                        objDept.AccountDetailTypeId = DtAccDetail.FirstOrDefault().AccountDetailTypeId;
                                                        if (objDept.AccountDetailTypeId == 0 || objDept.AccountDetailTypeId == null)
                                                        {
                                                            AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
                                                            objDetail.DetailType = departmentMaster.AccountType;
                                                            objDetail.QBDetailType = departmentMaster.QBDetailType;
                                                            objDetail.AccountTypeId = objDept.AccountTypeId;
                                                            //This db class is save Account details type
                                                            _departmentMastersRepository.SaveAccountDetailTypeMasters(objDetail);
                                                            objDetail = null;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
                                                        objDetail.DetailType = departmentMaster.AccountType;
                                                        objDetail.QBDetailType = departmentMaster.QBDetailType;
                                                        objDetail.AccountTypeId = objDept.AccountTypeId;
                                                        //This db class is save Account details type
                                                        _departmentMastersRepository.SaveAccountDetailTypeMasters(objDetail);
                                                        int id = objDetail.AccountDetailTypeId;
                                                        objDetail = null;
                                                        objDept.AccountDetailTypeId = id;
                                                    }

                                                    objDept.Description = departmentMaster.Description == null ? null : departmentMaster.Description.ToUpper();
                                                    objDept.AccountNumber = departmentMaster.AccountNumber == null ? null : departmentMaster.AccountNumber.ToUpper();
                                                    objDept.IsSubAccount = departmentMaster.IsSubAccount;
                                                    objDept.IsActive = Convert.ToBoolean(departmentMaster.IsActive);
                                                    objDept.StoreId = StoreId;
                                                    objDept.ModifiedOn = DateTime.Now;
                                                    objDept.LastModifiedOn = DateTime.Now;
                                                    //Get UserID by Username.
                                                    objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                                    objDept.IsSync = 1;
                                                    objDept.SyncDate = DateTime.Now;
                                                    ////This db class is update Account details
                                                    _departmentMastersRepository.UpdateDepartmentMaster(objDept);
                                                    iDeptId = objDept.DepartmentId;
                                                    if (iDeptId == 0)
                                                    {
                                                        Message = _commonRepository.GetMessageValue("DMSQ", "Department Not Synched in QuickBook");
                                                        TempData["Sucessme"] += departmentMaster.DepartmentName + ":" + Message;

                                                    }
                                                    else
                                                    {
                                                        // TempData["Sucessme"] = "Department Save Successfully..";
                                                    }
                                                    objDept = null;
                                                }
                                                else
                                                {
                                                    //Select for update data.
                                                    var vData = _departmentMastersRepository.SelectForUpdate(StoreId, departmentMaster.DepartmentName, departmentMaster.DepartmentId);
                                                    if (vData.Count > 0)
                                                    {
                                                        Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                                        TempData["Sucessme"] += vData.FirstOrDefault().DepartmentName + ":" + Message;
                                                    }
                                                    DepartmentMaster objDept = new DepartmentMaster();
                                                    objDept.ListId = objResponse.ID;
                                                    objDept.DepartmentName = departmentMaster.DepartmentName == null ? null : departmentMaster.DepartmentName.ToUpper();
                                                    //Get Account Type
                                                    var DtAcc = _departmentMastersRepository.GetAccountType(AccType);
                                                    if (DtAcc.Count > 0)
                                                    {
                                                        objDept.AccountTypeId = DtAcc.FirstOrDefault().AccountTypeId;
                                                    }
                                                    else
                                                    {
                                                        AccountTypeMaster objAcc = new AccountTypeMaster();
                                                        objAcc.AccountType = departmentMaster.AccountType;
                                                        objAcc.Flag = "O";
                                                        objAcc.CommonType = departmentMaster.AccountType;
                                                        //This db class is save Account type
                                                        _departmentMastersRepository.SaveAccountTypeMasters(objAcc);
                                                        int id = objAcc.AccountTypeId;
                                                        objAcc = null;
                                                        objDept.AccountTypeId = id;
                                                    }
                                                    //Get QB details
                                                    var DtAccDetail = _departmentMastersRepository.GetQBDetailType(DetailsType);
                                                    if (DtAccDetail.Count > 0)
                                                    {
                                                        objDept.AccountDetailTypeId = DtAccDetail.FirstOrDefault().AccountDetailTypeId;
                                                        if (objDept.AccountDetailTypeId == 0 || objDept.AccountDetailTypeId == null)
                                                        {
                                                            AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
                                                            objDetail.DetailType = departmentMaster.AccountDetailType;
                                                            objDetail.QBDetailType = departmentMaster.QBDetailType;
                                                            objDetail.AccountTypeId = objDept.AccountTypeId;
                                                            //This db class is save Account details type
                                                            _departmentMastersRepository.SaveAccountDetailTypeMasters(objDetail);
                                                            objDetail = null;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
                                                        objDetail.DetailType = departmentMaster.AccountDetailType;
                                                        objDetail.QBDetailType = departmentMaster.QBDetailType;
                                                        objDetail.AccountTypeId = objDept.AccountTypeId;
                                                        ////This db class is save Account details type
                                                        _departmentMastersRepository.SaveAccountDetailTypeMasters(objDetail);
                                                        int id = objDetail.AccountDetailTypeId;
                                                        objDetail = null;
                                                        objDept.AccountDetailTypeId = id;
                                                    }
                                                    objDept.Description = departmentMaster.Description == null ? null : departmentMaster.Description.ToUpper();
                                                    objDept.AccountNumber = departmentMaster.AccountNumber == null ? null : departmentMaster.AccountNumber.ToUpper();
                                                    objDept.IsSubAccount = departmentMaster.IsSubAccount;
                                                    objDept.IsActive = Convert.ToBoolean(departmentMaster.IsActive);
                                                    objDept.StoreId = StoreId;
                                                    objDept.CreatedOn = DateTime.Now;
                                                    objDept.LastModifiedOn = DateTime.Now;
                                                    objDept.ModifiedOn = DateTime.Now;
                                                    //Get UserID by Username.
                                                    objDept.CreatedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                                    objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                                    objDept.IsSync = 1;
                                                    objDept.SyncDate = DateTime.Now;
                                                    //This db class is update Department details
                                                    _departmentMastersRepository.UpdateDepartmentMaster(objDept);
                                                    objDept = null;
                                                }
                                            }
                                            else
                                            {
                                                Message = objResponse.Status.ToString();
                                                Session["Sucessmesg"] = _commonRepository.GetMessageValue("DMAO", "This AccountType Only Created OneTime...");
                                                return RedirectToAction("Create");
                                            }
                                        }
                                        else
                                        {
                                            var vData1 = _departmentMastersRepository.SelectByStoreID_Name(StoreId, departmentMaster.DepartmentName);
                                            if (vData1.Count > 0)
                                            {
                                                //Select for update data.
                                                var vData = _departmentMastersRepository.SelectForUpdate(StoreId, departmentMaster.DepartmentName, departmentMaster.DepartmentId);
                                                if (vData.Count > 0)
                                                {
                                                    Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                                    TempData["Sucessme"] += vData.FirstOrDefault().DepartmentName + ":" + Message;
                                                }
                                                DepartmentMaster objDept = _departmentMastersRepository.SelectByStoreID_DepId(StoreId, vData.LastOrDefault().DepartmentId);
                                                objDept.ListId = objResponse.ID;
                                                objDept.DepartmentName = departmentMaster.DepartmentName == null ? null : departmentMaster.DepartmentName.ToUpper();
                                                objDept.AccountTypeId = Convert.ToInt32(departmentMaster.AccountType);

                                                var DtAcc = _departmentMastersRepository.GetAccountType(AccType);
                                                if (DtAcc.Count > 0)
                                                {
                                                    objDept.AccountTypeId = DtAcc.FirstOrDefault().AccountTypeId;
                                                }
                                                else
                                                {
                                                    AccountTypeMaster objAcc = new AccountTypeMaster();
                                                    objAcc.AccountType = departmentMaster.AccountType;
                                                    objAcc.Flag = "O";
                                                    objAcc.CommonType = departmentMaster.AccountType;
                                                    ////This db class is save Account type
                                                    _departmentMastersRepository.SaveAccountTypeMasters(objAcc);
                                                    int id = objAcc.AccountTypeId;
                                                    objAcc = null;
                                                    objDept.AccountTypeId = id;
                                                }

                                                var DtAccDetail = _departmentMastersRepository.GetQBDetailType(DetailsType);
                                                if (DtAccDetail.Count > 0)
                                                {
                                                    objDept.AccountDetailTypeId = DtAccDetail.FirstOrDefault().AccountDetailTypeId;
                                                    if (objDept.AccountDetailTypeId == 0 || objDept.AccountDetailTypeId == null)
                                                    {
                                                        AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
                                                        objDetail.DetailType = departmentMaster.AccountType;
                                                        objDetail.QBDetailType = departmentMaster.QBDetailType;
                                                        objDetail.AccountTypeId = objDept.AccountTypeId;
                                                        ////This db class is save Account details type
                                                        _departmentMastersRepository.SaveAccountDetailTypeMasters(objDetail);
                                                        objDetail = null;
                                                    }
                                                }
                                                else
                                                {
                                                    ////This db class is save Account details
                                                    AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
                                                    objDetail.DetailType = departmentMaster.AccountType;
                                                    objDetail.QBDetailType = departmentMaster.QBDetailType;
                                                    objDetail.AccountTypeId = objDept.AccountTypeId;
                                                    _departmentMastersRepository.SaveAccountDetailTypeMasters(objDetail);
                                                    int id = objDetail.AccountDetailTypeId;
                                                    objDetail = null;
                                                    objDept.AccountDetailTypeId = id;
                                                }

                                                objDept.Description = departmentMaster.Description == null ? null : departmentMaster.Description.ToUpper();
                                                objDept.AccountNumber = departmentMaster.AccountNumber == null ? null : departmentMaster.AccountNumber.ToUpper();
                                                objDept.IsSubAccount = departmentMaster.IsSubAccount;
                                                objDept.IsActive = Convert.ToBoolean(departmentMaster.IsActive);
                                                objDept.StoreId = StoreId;
                                                objDept.ModifiedOn = DateTime.Now;
                                                objDept.LastModifiedOn = DateTime.Now;
                                                objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                                objDept.IsSync = 1;
                                                objDept.SyncDate = DateTime.Now;
                                                _departmentMastersRepository.UpdateDepartmentMaster(objDept);
                                                iDeptId = objDept.DepartmentId;
                                                if (iDeptId == 0)
                                                {
                                                    Message = _commonRepository.GetMessageValue("DMSQ", "Department Not Synched in QuickBook");
                                                    TempData["Sucessme"] += departmentMaster.DepartmentName + ":" + Message;

                                                }
                                                else
                                                {
                                                    // TempData["Sucessme"] = "Department Save Successfully..";
                                                }
                                                objDept = null;
                                            }
                                            else
                                            {
                                                //Select for update data.
                                                var vData = _departmentMastersRepository.SelectForUpdate(StoreId, departmentMaster.DepartmentName, departmentMaster.DepartmentId);
                                                if (vData.Count > 0)
                                                {
                                                    Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                                    TempData["Sucessme"] += vData.FirstOrDefault().DepartmentName + ":" + Message;
                                                }
                                                DepartmentMaster objDept = new DepartmentMaster();
                                                objDept.ListId = objResponse.ID;
                                                objDept.DepartmentName = departmentMaster.DepartmentName == null ? null : departmentMaster.DepartmentName.ToUpper();
                                                var DtAcc = _departmentMastersRepository.GetAccountType(AccType);
                                                if (DtAcc.Count > 0)
                                                {
                                                    objDept.AccountTypeId = DtAcc.FirstOrDefault().AccountTypeId;
                                                }
                                                else
                                                {
                                                    AccountTypeMaster objAcc = new AccountTypeMaster();
                                                    objAcc.AccountType = departmentMaster.AccountType;
                                                    objAcc.Flag = "O";
                                                    objAcc.CommonType = departmentMaster.AccountType;
                                                    ////This db class is save Account details
                                                    _departmentMastersRepository.SaveAccountTypeMasters(objAcc);
                                                    int id = objAcc.AccountTypeId;
                                                    objAcc = null;
                                                    objDept.AccountTypeId = id;
                                                }

                                                var DtAccDetail = _departmentMastersRepository.GetQBDetailType(DetailsType);
                                                if (DtAccDetail.Count > 0)
                                                {
                                                    objDept.AccountDetailTypeId = DtAccDetail.FirstOrDefault().AccountDetailTypeId;
                                                    if (objDept.AccountDetailTypeId == 0)
                                                    {
                                                        AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
                                                        objDetail.DetailType = departmentMaster.AccountDetailType;
                                                        objDetail.QBDetailType = departmentMaster.QBDetailType;
                                                        objDetail.AccountTypeId = objDept.AccountTypeId;
                                                        _departmentMastersRepository.SaveAccountDetailTypeMasters(objDetail);
                                                        objDetail = null;
                                                    }
                                                }
                                                else
                                                {
                                                    AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
                                                    objDetail.DetailType = departmentMaster.AccountDetailType;
                                                    objDetail.QBDetailType = departmentMaster.QBDetailType;
                                                    objDetail.AccountTypeId = objDept.AccountTypeId;
                                                    //This db class is save Account details
                                                    _departmentMastersRepository.SaveAccountDetailTypeMasters(objDetail);
                                                    int id = objDetail.AccountDetailTypeId;
                                                    objDetail = null;
                                                    objDept.AccountDetailTypeId = id;
                                                }
                                                objDept.Description = departmentMaster.Description == null ? null : departmentMaster.Description.ToUpper();
                                                objDept.AccountNumber = departmentMaster.AccountNumber == null ? null : departmentMaster.AccountNumber.ToUpper();
                                                objDept.IsSubAccount = departmentMaster.IsSubAccount;
                                                objDept.IsActive = Convert.ToBoolean(departmentMaster.IsActive);
                                                objDept.StoreId = StoreId;
                                                objDept.CreatedOn = DateTime.Now;
                                                objDept.ModifiedOn = DateTime.Now;
                                                objDept.LastModifiedOn = DateTime.Now;
                                                objDept.CreatedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                                objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                                objDept.IsSync = 1;
                                                objDept.SyncDate = DateTime.Now;
                                                //This db class is save Department details
                                                _departmentMastersRepository.SaveDepartmentMaster(objDept);
                                                objDept = null;
                                            }

                                        }
                                    }
                                    else
                                    {
                                        //Get all Account type list.
                                        var Dt = _departmentMastersRepository.AccountTypeList().Where(a => a.CommonType == AccType && a.Flag == "D").ToList();
                                        string QBAccountType = "";
                                        string QBCommonType = "";
                                        int AccountID = 0;
                                        if (Dt.Count > 0)
                                        {
                                            QBAccountType = Dt.FirstOrDefault().AccountType.ToString();
                                            QBCommonType = Dt.FirstOrDefault().CommonType.ToString();
                                            AccountID = Dt.FirstOrDefault().AccountTypeId;
                                        }
                                        departmentMaster.AccountTypeId = AccountID;
                                        departmentMaster.AccountType = QBAccountType;
                                        var vData1 = _departmentMastersRepository.SelectByStoreID_Name(StoreId, departmentMaster.DepartmentName);
                                        if (vData1.Count > 0)
                                        {
                                            //Select for update data.
                                            var vData = _departmentMastersRepository.SelectForUpdate(StoreId, departmentMaster.DepartmentName, departmentMaster.DepartmentId);
                                            if (vData.Count > 0)
                                            {
                                                Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                                TempData["Sucessme"] += vData.FirstOrDefault().DepartmentName + ":" + Message;
                                            }
                                            DepartmentMaster objDept = _departmentMastersRepository.SelectByStoreID_DepId(StoreId, vData.LastOrDefault().DepartmentId);
                                            objDept.ListId = vData1.FirstOrDefault().ListId;
                                            objDept.DepartmentName = departmentMaster.DepartmentName == null ? null : departmentMaster.DepartmentName.ToUpper();
                                            objDept.AccountTypeId = Convert.ToInt32(departmentMaster.AccountType);
                                            //get Accony type
                                            var DtAcc = _departmentMastersRepository.GetAccountType(AccType);
                                            if (DtAcc.Count > 0)
                                            {
                                                objDept.AccountTypeId = DtAcc.FirstOrDefault().AccountTypeId;
                                            }
                                            else
                                            {
                                                AccountTypeMaster objAcc = new AccountTypeMaster();
                                                objAcc.AccountType = departmentMaster.AccountType;
                                                objAcc.Flag = "D";
                                                objAcc.CommonType = departmentMaster.AccountType;
                                                //This db class is save Account details
                                                _departmentMastersRepository.SaveAccountTypeMasters(objAcc);
                                                int id = objAcc.AccountTypeId;
                                                objAcc = null;
                                                objDept.AccountTypeId = id;
                                            }
                                            //Get Qb details type.
                                            var DtAccDetail = _departmentMastersRepository.GetQBDetailType(DetailsType);
                                            if (DtAccDetail.Count > 0)
                                            {
                                                objDept.AccountDetailTypeId = DtAccDetail.FirstOrDefault().AccountDetailTypeId;
                                                if (objDept.AccountDetailTypeId == 0 || objDept.AccountDetailTypeId == null)
                                                {
                                                    AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
                                                    objDetail.DetailType = departmentMaster.AccountType;
                                                    objDetail.QBDetailType = departmentMaster.QBDetailType;
                                                    objDetail.AccountTypeId = objDept.AccountTypeId;
                                                    ////This db class is save Account details
                                                    _departmentMastersRepository.SaveAccountDetailTypeMasters(objDetail);
                                                    objDetail = null;
                                                }
                                            }
                                            else
                                            {
                                                AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
                                                objDetail.DetailType = departmentMaster.AccountType;
                                                objDetail.QBDetailType = departmentMaster.QBDetailType;
                                                objDetail.AccountTypeId = objDept.AccountTypeId;
                                                ////This db class is save Account details
                                                _departmentMastersRepository.SaveAccountDetailTypeMasters(objDetail);
                                                int id = objDetail.AccountDetailTypeId;
                                                objDetail = null;
                                                objDept.AccountDetailTypeId = id;
                                            }

                                            objDept.Description = departmentMaster.Description == null ? null : departmentMaster.Description.ToUpper();
                                            objDept.AccountNumber = departmentMaster.AccountNumber == null ? null : departmentMaster.AccountNumber.ToUpper();
                                            objDept.IsSubAccount = departmentMaster.IsSubAccount;
                                            objDept.IsActive = Convert.ToBoolean(departmentMaster.IsActive);
                                            objDept.StoreId = StoreId;
                                            objDept.ModifiedOn = DateTime.Now;
                                            objDept.LastModifiedOn = DateTime.Now;
                                            objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                            objDept.IsSync = 0;
                                            objDept.SyncDate = DateTime.Now;
                                            //This db class is update Department details
                                            _departmentMastersRepository.UpdateDepartmentMaster(objDept);
                                            iDeptId = objDept.DepartmentId;
                                            if (iDeptId == 0)
                                            {
                                                Message = _commonRepository.GetMessageValue("DMDU", "Department Not Updated.");
                                                TempData["Sucessme"] += departmentMaster.DepartmentName + ":" + Message;

                                            }
                                            else
                                            {
                                                // TempData["Sucessme"] = "Department Save Successfully..";
                                            }
                                            objDept = null;
                                        }
                                        else
                                        {
                                            //Select for update data.
                                            var vData = _departmentMastersRepository.SelectForUpdate(StoreId, departmentMaster.DepartmentName, departmentMaster.DepartmentId);
                                            if (vData.Count > 0)
                                            {
                                                Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                                TempData["Sucessme"] += vData.FirstOrDefault().DepartmentName + ":" + Message;
                                            }
                                            DepartmentMaster objDept = new DepartmentMaster();
                                            objDept.ListId = objResponse.ID;
                                            objDept.DepartmentName = departmentMaster.DepartmentName == null ? null : departmentMaster.DepartmentName.ToUpper();
                                            //Get account type
                                            var DtAcc = _departmentMastersRepository.GetAccountType1(QBAccountType);
                                            if (DtAcc.Count > 0)
                                            {
                                                objDept.AccountTypeId = DtAcc.FirstOrDefault().AccountTypeId;
                                            }
                                            else
                                            {
                                                AccountTypeMaster objAcc = new AccountTypeMaster();
                                                objAcc.AccountType = departmentMaster.AccountType;
                                                objAcc.Flag = "D";
                                                objAcc.CommonType = departmentMaster.AccountType;
                                                ////This db class is save Account details
                                                _departmentMastersRepository.SaveAccountTypeMasters(objAcc);
                                                int id = objAcc.AccountTypeId;
                                                objAcc = null;
                                                objDept.AccountTypeId = id;
                                            }

                                            var DtAccDetail = _departmentMastersRepository.GetQBDetailType(DetailsType);
                                            if (DtAccDetail.Count > 0)
                                            {
                                                objDept.AccountDetailTypeId = DtAccDetail.FirstOrDefault().AccountDetailTypeId;
                                                if (objDept.AccountDetailTypeId == 0 || objDept.AccountDetailTypeId == null)
                                                {
                                                    AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
                                                    objDetail.DetailType = departmentMaster.AccountDetailType;
                                                    objDetail.QBDetailType = departmentMaster.QBDetailType;
                                                    objDetail.AccountTypeId = objDept.AccountTypeId;
                                                    ////This db class is save Account details
                                                    _departmentMastersRepository.SaveAccountDetailTypeMasters(objDetail);
                                                    objDetail = null;
                                                }
                                            }
                                            else
                                            {
                                                AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
                                                objDetail.DetailType = departmentMaster.AccountDetailType;
                                                objDetail.QBDetailType = departmentMaster.QBDetailType;
                                                objDetail.AccountTypeId = objDept.AccountTypeId;
                                                ////This db class is save Account details
                                                _departmentMastersRepository.SaveAccountDetailTypeMasters(objDetail);
                                                int id = objDetail.AccountDetailTypeId;
                                                objDetail = null;
                                                objDept.AccountDetailTypeId = id;
                                            }
                                            objDept.Description = departmentMaster.Description == null ? null : departmentMaster.Description.ToUpper();
                                            objDept.AccountNumber = departmentMaster.AccountNumber == null ? null : departmentMaster.AccountNumber.ToUpper();
                                            objDept.IsSubAccount = departmentMaster.IsSubAccount;
                                            objDept.IsActive = Convert.ToBoolean(departmentMaster.IsActive);
                                            objDept.StoreId = StoreId;
                                            objDept.CreatedOn = DateTime.Now;
                                            objDept.ModifiedOn = DateTime.Now;
                                            objDept.LastModifiedOn = DateTime.Now;
                                            objDept.CreatedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                            objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                            objDept.IsSync = 0;
                                            objDept.SyncDate = DateTime.Now;
                                            //This db class is save Department details
                                            _departmentMastersRepository.SaveDepartmentMaster(objDept);
                                            objDept = null;
                                        }
                                    }
                                }
                                else
                                {
                                    //Get all Account type list using account type ID.
                                    var Dt = _departmentMastersRepository.AccountTypeList().Where(a => a.AccountTypeId == departmentMaster.AccountTypeId).ToList();
                                    string QBAccountType = "";
                                    int AccountID = 0;
                                    if (Dt.Count > 0)
                                    {
                                        QBAccountType = Dt.FirstOrDefault().AccountType.ToString();
                                        AccountID = Dt.FirstOrDefault().AccountTypeId;
                                    }
                                    departmentMaster.AccountTypeId = AccountID;
                                    //using this db class get select by storeID name
                                    var vData1 = _departmentMastersRepository.SelectByStoreID_Name1(StoreId, departmentMaster.DepartmentName);
                                    if (vData1 != null)
                                    {
                                        //Select for update data.
                                        var vData = _departmentMastersRepository.SelectForUpdate(StoreId, departmentMaster.DepartmentName, departmentMaster.DepartmentId);
                                        if (vData.Count > 0)
                                        {
                                            Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                            TempData["Sucessme"] += vData.FirstOrDefault().DepartmentName + ":" + Message;
                                        }
                                        //Get all Department Master list with selection.
                                        DepartmentMaster objDept = _departmentMastersRepository.DepartmentMasterList().Where(a => a.DepartmentId == vData1.DepartmentId).FirstOrDefault();
                                        objDept.ListId = vData1.ListId;
                                        objDept.DepartmentName = departmentMaster.DepartmentName == null ? null : departmentMaster.DepartmentName.ToUpper();
                                        objDept.AccountTypeId = departmentMaster.AccountTypeId;
                                        objDept.AccountDetailTypeId = departmentMaster.AccountDetailTypeId;
                                        objDept.Description = departmentMaster.Description == null ? null : departmentMaster.Description.ToUpper();
                                        objDept.AccountNumber = departmentMaster.AccountNumber == null ? null : departmentMaster.AccountNumber.ToUpper();
                                        objDept.IsActive = Convert.ToBoolean(departmentMaster.IsActive);
                                        objDept.StoreId = StoreId;
                                        objDept.ModifiedOn = DateTime.Now;
                                        objDept.LastModifiedOn = DateTime.Now;
                                        objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                        objDept.IsSync = 0;
                                        objDept.SyncDate = DateTime.Now;
                                        //This db class is update Department details
                                        _departmentMastersRepository.UpdateDepartmentMaster(objDept);
                                        //db.Entry(objDept).State = EntityState.Modified;
                                        //db.SaveChanges();
                                        iDeptId = objDept.DepartmentId;
                                        if (iDeptId == 0)
                                        {
                                            Message = _commonRepository.GetMessageValue("DMDU", "Department Not Updated.");
                                            TempData["Sucessme"] += vData1.DepartmentName + ":" + Message;

                                        }
                                        else
                                        {
                                            //TempData["Sucessme"] = "Department Save Successfully..";
                                        }
                                    }
                                    else
                                    {
                                        //Select for update data.
                                        var vData = _departmentMastersRepository.SelectForUpdate(StoreId, departmentMaster.DepartmentName, departmentMaster.DepartmentId);
                                        if (vData.Count > 0)
                                        {
                                            Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                            TempData["Sucessme"] += vData.FirstOrDefault().DepartmentName + ":" + Message;
                                        }
                                        DepartmentMaster objDept = new DepartmentMaster();
                                        objDept.ListId = objResponse.ID;
                                        objDept.DepartmentName = departmentMaster.DepartmentName == null ? null : departmentMaster.DepartmentName.ToUpper();
                                        objDept.AccountTypeId = departmentMaster.AccountTypeId;
                                        objDept.AccountDetailTypeId = departmentMaster.AccountDetailTypeId;
                                        objDept.Description = departmentMaster.Description == null ? null : departmentMaster.Description.ToUpper();
                                        objDept.AccountNumber = departmentMaster.AccountNumber == null ? null : departmentMaster.AccountNumber.ToUpper();
                                        objDept.IsSubAccount = departmentMaster.IsSubAccount;
                                        objDept.IsActive = Convert.ToBoolean(departmentMaster.IsActive);
                                        objDept.StoreId = StoreId;
                                        objDept.CreatedOn = DateTime.Now;
                                        objDept.ModifiedOn = DateTime.Now;
                                        objDept.LastModifiedOn = DateTime.Now;
                                        objDept.CreatedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                        objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                        objDept.IsSync = 0;
                                        objDept.SyncDate = DateTime.Now;
                                        //This db class is save Department details
                                        _departmentMastersRepository.SaveDepartmentMaster(objDept);
                                        objDept = null;
                                    }
                                }
                            }
                        }
                        else
                        {
                            TempData["sMessage"] = "success";
                        }
                    }
                    //db.DepartmentMasters.AddRange(DepartmentList);
                    //await db.SaveChangesAsync();

                    if (TempData["Sucessme"] != null)
                    {

                    }
                    else
                    {
                        TempData["Sucessme"] = _commonRepository.GetMessageValue("DMDSS", "Department Save Successfully..");
                        StatusMessageString = _commonRepository.GetMessageValue("DMC", "Department Created Successfully..");
                        ViewBag.StatusMessageString = StatusMessageString;
                    }
                    Session["Sucessmesg"] = TempData["Sucessme"].ToString();
                    TempData["Sucessme"] = null;
                }
                catch (Exception ex)
                {
                    logger.Error("ChartofAccountsController - Create - " + DateTime.Now + " - " + ex.Message.ToString());
                    
                }
                return RedirectToAction("Index");
            }
            //Get All Account Details type List
            ViewBag.AccountDetailTypeId = new SelectList(_departmentMastersRepository.AccountDetailTypeList().Select(s => new { AccountDetailTypeId = s.AccountDetailTypeId, DetailType = s.DetailType }).OrderBy(o => o.DetailType).ToList(), "AccountDetailTypeId", "DetailType", departmentMaster.AccountDetailTypeId);
            //Get All Account List
            ViewBag.AccountTypeId = new SelectList(_departmentMastersRepository.AccountTypeList().Select(s => new { s.AccountTypeId, s.AccountType }).OrderBy(o => o.AccountType), "AccountTypeId", "AccountType", departmentMaster.AccountTypeId);
            //ViewBag.StoreId = new SelectList(db.StoreMasters.Where(k => k.IsActive == true).Select(s => new { s.StoreId, s.Name }).OrderBy(o => o.Name).ToList(), "StoreId", "Name", departmentMaster.StoreId);
            //ViewBag.MultiStoreId = new MultiSelectList(db.StoreMasters.Where(k => k.IsActive == true).Select(s => new { s.StoreId, s.Name }).OrderBy(o => o.Name).ToList(), "StoreId", "Name", departmentMaster.MultiStoreId);
            //Uisng this db class get StoreList with role Wise.
            var StoreList = _commonRepository.GetStoreList_RoleWise(11, "CreateQBDepartment", UserName).Select(s => s.StoreId);
            ViewBag.MultiStoreId = new MultiSelectList(_departmentMastersRepository.StoreMasterList().Where(s => s.IsActive == true && StoreList.Contains(s.StoreId)).Select(s => new { s.StoreId, s.Name }).OrderBy(o => o.Name).ToList(), "StoreId", "Name", departmentMaster.MultiStoreId);
            ViewBag.StoreId = new SelectList(_departmentMastersRepository.StoreMasterList().Where(k => k.IsActive == true && StoreList.Contains(k.StoreId)).Select(s => new { s.StoreId, s.Name }).OrderBy(o => o.Name).ToList(), "StoreId", "Name", departmentMaster.StoreId);
            return View(departmentMaster);
        }
        /// <summary>
        /// This method is Get Department id for Edit data.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,UpdateQBDepartment")]
        // GET: DepartmentMasters/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Get Department Id.
            DepartmentMaster departmentMaster = await _departmentMastersRepository.GetDepartmentMasterId(id);
            if (departmentMaster == null)
            {
                return HttpNotFound();
            }
            //Get All Account Details type List
            ViewBag.AccountDetailTypeId = new SelectList(_departmentMastersRepository.AccountDetailTypeList().OrderBy(o => o.DetailType).ToList(), "AccountDetailTypeId", "DetailType", departmentMaster.AccountDetailTypeId);
            //Get All Account  type List
            ViewBag.AccountTypeId = new SelectList(_departmentMastersRepository.AccountTypeList().OrderBy(o => o.AccountType).ToList(), "AccountTypeId", "AccountType", departmentMaster.AccountTypeId);
            //ViewBag.StoreId = new SelectList(db.StoreMasters.Where(k => k.IsActive == true).Select(s => new { s.StoreId, s.Name }).OrderBy(o => o.Name).ToList(), "StoreId", "Name", departmentMaster.StoreId);
            //ViewBag.MultiStoreId = new MultiSelectList(db.StoreMasters.Where(k => k.IsActive == true).Select(s => new { s.StoreId, s.Name }).OrderBy(o => o.Name).ToList(), "StoreId", "Name", departmentMaster.MultiStoreId);

            return View(departmentMaster);
        }

        // POST: DepartmentMasters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// This method is Edit Department data
        /// </summary>
        /// <param name="departmentMaster"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(DepartmentMaster departmentMaster)
        {
            int iDeptId = 0;
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            if (ModelState.IsValid)
            {
                try
                {
                    //Using this db class get stores on Line desktop.
                    string Store = _qBRepository.GetStoreOnlineDesktop(Convert.ToInt32(departmentMaster.StoreId));
                    //Using this db class get stores on Line desktop Flag.
                    int StoreFlag = _qBRepository.GetStoreOnlineDesktopFlag(Convert.ToInt32(departmentMaster.StoreId));
                    if (Store != "")
                    {
                        string DetailsType = "";
                        string AccountType = "";
                        //Get All Account type List
                        var AccType = _departmentMastersRepository.AccountTypeList().Where(a => a.AccountTypeId == departmentMaster.AccountTypeId).ToList();
                        if (AccType.Count > 0)
                        {
                            AccountType = AccType.FirstOrDefault().AccountType.ToString();
                        }
                        //Get All Account Details type List
                        var Dte = _departmentMastersRepository.AccountDetailTypeList().Where(a => a.DetailType == AccountType).ToList();
                        if (Dte.Count > 0)
                        {
                            DetailsType = Dte.FirstOrDefault().QBDetailType.ToString();
                        }
                        else
                        {
                            DetailsType = departmentMaster.AccountDetailType;
                        }

                        if (departmentMaster.IsSubAccount == "" || departmentMaster.IsSubAccount == "0")
                        {
                            departmentMaster.IsSubAccount = null;
                        }
                        QBResponse objResponse = new QBResponse();
                        if (Store == "Online" && StoreFlag == 1)
                        {
                            if (StoreFlag == 1)
                            {
                                //Get all Department Master list with selection.
                                var DeptExist = _departmentMastersRepository.DepartmentMasterList().Where(a => a.DepartmentId == departmentMaster.DepartmentId).FirstOrDefault();
                                if (DeptExist != null)
                                {
                                    //QB Edit Sync Department.
                                    _qBRepository.QBEditSyncDepartment(departmentMaster.DepartmentName, AccountType, DetailsType, departmentMaster.Description, departmentMaster.AccountNumber, departmentMaster.IsSubAccount, DeptExist.ListId, Convert.ToInt32(departmentMaster.StoreId), ref objResponse);
                                    if ((objResponse.ID != "0" && objResponse.ID != "") || objResponse.Status == "Done")
                                    {
                                        //using this db class get select by storeID listid
                                        var vData1 = _departmentMastersRepository.SelectByStoreID_ListID(departmentMaster.StoreId, objResponse.ID);
                                        if (vData1.Count > 0)
                                        {
                                            //Select for update data.
                                            var vData = _departmentMastersRepository.SelectForUpdate(departmentMaster.StoreId, departmentMaster.DepartmentName, departmentMaster.DepartmentId);
                                            if (vData.Count > 0)
                                            {
                                                Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                                TempData["Sucessme"] += vData.FirstOrDefault().DepartmentName + ":" + Message;
                                                return RedirectToAction("Edit/", new { Id = departmentMaster.DepartmentId, StoreID = departmentMaster.StoreId });
                                            }
                                            int iDeptID = vData1.LastOrDefault().DepartmentId;
                                            //Get all Department Master list with selection.
                                            DepartmentMaster objDept = _departmentMastersRepository.DepartmentMasterList().Where(a => a.DepartmentId == iDeptID).FirstOrDefault();
                                            //db.Database.SqlQuery<DepartmentMaster>("SP_DepartmentMaster @Mode = {0},@StoreId = {1},@DepartmentId = {2}", "SelectByStoreID_DepartmentId", departmentMaster.StoreId, iDeptID).FirstOrDefault();
                                            objDept.DepartmentId = departmentMaster.DepartmentId;
                                            objDept.ListId = objResponse.ID;
                                            objDept.DepartmentName = departmentMaster.DepartmentName == null ? null : departmentMaster.DepartmentName.ToUpper();
                                            objDept.Description = departmentMaster.Description == null ? null : departmentMaster.Description.ToUpper();
                                            objDept.AccountNumber = departmentMaster.AccountNumber == null ? null : departmentMaster.AccountNumber.ToUpper();
                                            objDept.IsActive = Convert.ToBoolean(departmentMaster.IsActive);
                                            objDept.StoreId = departmentMaster.StoreId;
                                            objDept.ModifiedOn = DateTime.Now;
                                            objDept.LastModifiedOn = DateTime.Now;
                                            objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                            objDept.IsSync = 1;
                                            objDept.SyncDate = DateTime.Now;
                                            //This db class is update Department details
                                            _departmentMastersRepository.UpdateDepartmentMaster(objDept);

                                            iDeptId = objDept.DepartmentId;
                                            if (iDeptId == 0)
                                            {
                                                TempData["Sucessme"] = _commonRepository.GetMessageValue("DMSQ", "Department Not Synched in QuickBook");
                                                Session["Sucessmesg"] = _commonRepository.GetMessageValue("DMSQ", "Department Not Synched in QuickBook");
                                                return RedirectToAction("Edit/", new { Id = departmentMaster.DepartmentId, StoreID = departmentMaster.StoreId });
                                            }
                                            else
                                            {
                                                TempData["complete"] = _commonRepository.GetMessageValue("DMUS", "Department Update Successfully..");
                                            }
                                            objDept = null;
                                        }
                                        else
                                        {
                                            //Select for update data.
                                            var vData = _departmentMastersRepository.SelectForUpdate(departmentMaster.StoreId, departmentMaster.DepartmentName, departmentMaster.DepartmentId);
                                            if (vData.Count > 0)
                                            {
                                                Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                                TempData["Sucessme"] += vData.FirstOrDefault().DepartmentName + ":" + Message;
                                                return RedirectToAction("Edit/", new { Id = departmentMaster.DepartmentId, StoreID = departmentMaster.StoreId });
                                            }
                                            DepartmentMaster objDept = new DepartmentMaster();
                                            objDept.ListId = objResponse.ID;
                                            objDept.DepartmentName = departmentMaster.DepartmentName == null ? null : departmentMaster.DepartmentName.ToUpper();
                                            objDept.AccountTypeId = departmentMaster.AccountTypeId;
                                            objDept.AccountDetailTypeId = departmentMaster.AccountDetailTypeId;
                                            objDept.Description = departmentMaster.Description == null ? null : departmentMaster.Description.ToUpper();
                                            objDept.AccountNumber = departmentMaster.AccountNumber == null ? null : departmentMaster.AccountNumber.ToUpper();
                                            objDept.IsSubAccount = departmentMaster.IsSubAccount;
                                            objDept.IsActive = Convert.ToBoolean(departmentMaster.IsActive);
                                            objDept.StoreId = departmentMaster.StoreId;
                                            objDept.CreatedOn = DateTime.Now;
                                            objDept.ModifiedOn = DateTime.Now;
                                            objDept.LastModifiedOn = DateTime.Now;
                                            objDept.CreatedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                            objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                            objDept.IsSync = 1;
                                            objDept.SyncDate = DateTime.Now;
                                            //This db class is save Department details
                                            _departmentMastersRepository.SaveDepartmentMaster(objDept);
                                            objDept = null;
                                        }
                                    }
                                    else
                                    {
                                        Message = objResponse.Status.ToString();
                                    }
                                }
                                else
                                {
                                    //Select for update data.
                                    var vData = _departmentMastersRepository.SelectForUpdate(departmentMaster.StoreId, departmentMaster.DepartmentName, departmentMaster.DepartmentId);
                                    if (vData.Count > 0)
                                    {
                                        Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                        TempData["Sucessme"] = Message;
                                        Session["Sucessmesg"] = Message;
                                        return RedirectToAction("Edit/", new { Id = departmentMaster.DepartmentId, StoreID = departmentMaster.StoreId });
                                    }
                                    //get QB Sync Deparment.
                                    _qBRepository.QBSyncDepartment(departmentMaster.DepartmentName, AccountType, DetailsType, departmentMaster.Description, departmentMaster.AccountNumber, departmentMaster.IsSubAccount, Convert.ToInt32(departmentMaster.StoreId), ref objResponse);
                                    if (objResponse.ID != "0" || objResponse.Status == "Done")
                                    {
                                        DepartmentMaster objDept = new DepartmentMaster();
                                        objDept.ListId = objResponse.ID;
                                        objDept.DepartmentName = departmentMaster.DepartmentName;
                                        objDept.AccountTypeId = departmentMaster.AccountTypeId;
                                        objDept.AccountDetailTypeId = departmentMaster.AccountDetailTypeId;
                                        objDept.Description = departmentMaster.Description;
                                        objDept.AccountNumber = departmentMaster.AccountNumber;
                                        objDept.IsSubAccount = departmentMaster.IsSubAccount;
                                        objDept.IsActive = Convert.ToBoolean(departmentMaster.IsActive);
                                        objDept.StoreId = departmentMaster.StoreId;
                                        objDept.CreatedOn = DateTime.Now;
                                        objDept.ModifiedOn = DateTime.Now;
                                        objDept.LastModifiedOn = DateTime.Now;
                                        objDept.CreatedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                        objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                        objDept.IsSync = 1;
                                        objDept.SyncDate = DateTime.Now;
                                        //This db class is save Department details
                                        _departmentMastersRepository.SaveDepartmentMaster(objDept);
                                        objDept = null;
                                    }
                                }
                            }
                            else
                            {
                                var objDept1 = _departmentMastersRepository.SelectByStoreID_DepId(departmentMaster.StoreId, departmentMaster.DepartmentId);
                                if (objDept1 != null)
                                {
                                    //Select for update data.
                                    var vData = _departmentMastersRepository.SelectForUpdate(departmentMaster.StoreId, departmentMaster.DepartmentName, departmentMaster.DepartmentId);
                                    if (vData.Count > 0)
                                    {
                                        Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                        TempData["Sucessme"] = Message;
                                        Session["Sucessmesg"] = Message;
                                        return RedirectToAction("Edit/", new { Id = departmentMaster.DepartmentId, StoreID = departmentMaster.StoreId });
                                    }
                                    //Get all Department Master list with selection.
                                    DepartmentMaster objDept = _departmentMastersRepository.DepartmentMasterList().Where(a => a.DepartmentId == objDept1.DepartmentId).FirstOrDefault();
                                    objDept.ListId = objDept1.ListId;
                                    objDept.DepartmentName = departmentMaster.DepartmentName;
                                    objDept.Description = departmentMaster.Description;
                                    objDept.AccountNumber = departmentMaster.AccountNumber;
                                    objDept.IsActive = Convert.ToBoolean(departmentMaster.IsActive);
                                    objDept.StoreId = departmentMaster.StoreId;
                                    objDept.ModifiedOn = DateTime.Now;
                                    objDept.LastModifiedOn = DateTime.Now;
                                    objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                    objDept.IsSync = 0;
                                    objDept.SyncDate = DateTime.Now;
                                    //This db class is update Department details
                                    _departmentMastersRepository.UpdateDepartmentMaster(objDept);
                                    iDeptId = objDept.DepartmentId;
                                    if (iDeptId == 0)
                                    {
                                        Message = _commonRepository.GetMessageValue("DMDU", "Department Not Updated.");
                                        TempData["Sucessme"] = Message;
                                        Session["Sucessmesg"] = Message;
                                        return RedirectToAction("Edit/", new { Id = departmentMaster.DepartmentId, StoreID = departmentMaster.StoreId });
                                    }
                                    else
                                    {
                                        TempData["complete"] = _commonRepository.GetMessageValue("DMUS", "Department Update Successfully..");
                                    }
                                }
                                else
                                {
                                    //Select for update data.
                                    var vData = _departmentMastersRepository.SelectForUpdate(departmentMaster.StoreId, departmentMaster.DepartmentName, departmentMaster.DepartmentId);
                                    if (vData.Count > 0)
                                    {
                                        Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                        TempData["Sucessme"] = Message;
                                        Session["Sucessmesg"] = Message;
                                        return RedirectToAction("Edit/", new { Id = departmentMaster.DepartmentId, StoreID = departmentMaster.StoreId });
                                    }

                                    DepartmentMaster objDept = new DepartmentMaster();
                                    objDept.ListId = objResponse.ID;
                                    objDept.DepartmentName = departmentMaster.DepartmentName;
                                    objDept.AccountTypeId = departmentMaster.AccountTypeId;
                                    objDept.AccountDetailTypeId = departmentMaster.AccountDetailTypeId;
                                    objDept.Description = departmentMaster.Description;
                                    objDept.AccountNumber = departmentMaster.AccountNumber;
                                    objDept.IsSubAccount = departmentMaster.IsSubAccount;
                                    objDept.IsActive = Convert.ToBoolean(departmentMaster.IsActive);
                                    objDept.StoreId = departmentMaster.StoreId;
                                    objDept.CreatedOn = DateTime.Now;
                                    objDept.ModifiedOn = DateTime.Now;
                                    objDept.LastModifiedOn = DateTime.Now;
                                    objDept.CreatedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                    objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                    objDept.IsSync = 0;
                                    objDept.SyncDate = DateTime.Now;
                                    //This db class is save Department details
                                    _departmentMastersRepository.SaveDepartmentMaster(objDept);
                                    objDept = null;
                                    TempData["complete"] = _commonRepository.GetMessageValue("DMDSS", "Department Save Successfully..");
                                }
                            }
                        }
                        else
                        {
                            var vData1 = _departmentMastersRepository.SelectByStoreID_DepId(departmentMaster.StoreId, departmentMaster.DepartmentId);
                            if (vData1 != null)
                            {//Select for update data.
                                var vData = _departmentMastersRepository.SelectForUpdate(departmentMaster.StoreId, departmentMaster.DepartmentName, departmentMaster.DepartmentId);
                                if (vData.Count > 0)
                                {
                                    Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                    TempData["Sucessme"] = Message;
                                    Session["Sucessmesg"] = Message;
                                    return RedirectToAction("DepartmentEdit/", new { Id = departmentMaster.DepartmentId, StoreID = departmentMaster.StoreId });
                                }
                                //Get all Department Master list with selection.
                                DepartmentMaster objDept = _departmentMastersRepository.DepartmentMasterList().Where(a => a.DepartmentId == vData1.DepartmentId).FirstOrDefault();
                                objDept.ListId = vData1.ListId;
                                objDept.DepartmentName = departmentMaster.DepartmentName;
                                objDept.Description = departmentMaster.Description;
                                objDept.AccountNumber = departmentMaster.AccountNumber;
                                objDept.IsActive = Convert.ToBoolean(departmentMaster.IsActive);
                                objDept.StoreId = departmentMaster.StoreId;
                                objDept.ModifiedOn = DateTime.Now;
                                objDept.LastModifiedOn = DateTime.Now;
                                objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                objDept.IsSync = 0;
                                objDept.SyncDate = DateTime.Now;
                                //This db class is update Department details
                                _departmentMastersRepository.UpdateDepartmentMaster(objDept);
                                iDeptId = objDept.DepartmentId;
                                if (iDeptId == 0)
                                {
                                    Message = _commonRepository.GetMessageValue("DMDU", "Department Not Updated.");
                                    TempData["Sucessme"] = Message;
                                    Session["Sucessmesg"] = Message;
                                    return RedirectToAction("Edit/", new { Id = departmentMaster.DepartmentId, StoreID = departmentMaster.StoreId });
                                }
                                else
                                {
                                    TempData["complete"] = _commonRepository.GetMessageValue("DMUS", "Department Update Successfully..");
                                }
                            }
                            else
                            {
                                //Select for update data.
                                var vData = _departmentMastersRepository.SelectForUpdate(departmentMaster.StoreId, departmentMaster.DepartmentName, departmentMaster.DepartmentId);
                                if (vData.Count > 0)
                                {
                                    Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                    TempData["Sucessme"] = Message;
                                    Session["Sucessmesg"] = Message;
                                    return RedirectToAction("Edit/", new { Id = departmentMaster.DepartmentId, StoreID = departmentMaster.StoreId });
                                }

                                DepartmentMaster objDept = new DepartmentMaster();
                                objDept.ListId = objResponse.ID;
                                objDept.DepartmentName = departmentMaster.DepartmentName;
                                objDept.AccountTypeId = departmentMaster.AccountTypeId;
                                objDept.AccountDetailTypeId = departmentMaster.AccountDetailTypeId;
                                objDept.Description = departmentMaster.Description;
                                objDept.AccountNumber = departmentMaster.AccountNumber;
                                objDept.IsSubAccount = departmentMaster.IsSubAccount;
                                objDept.IsActive = Convert.ToBoolean(departmentMaster.IsActive);
                                objDept.StoreId = departmentMaster.StoreId;
                                objDept.CreatedOn = DateTime.Now;
                                objDept.ModifiedOn = DateTime.Now;
                                objDept.LastModifiedOn = DateTime.Now;
                                objDept.CreatedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                                objDept.IsSync = 0;
                                objDept.SyncDate = DateTime.Now;
                                //This db class is save Department details
                                _departmentMastersRepository.SaveDepartmentMaster(objDept);
                                objDept = null;
                                TempData["complete"] = _commonRepository.GetMessageValue("DMDSS", "Department Save Successfully..");
                            }
                        }
                    }
                    else
                    {
                        var vData1 = _departmentMastersRepository.SelectByStoreID_DepId(departmentMaster.StoreId, departmentMaster.DepartmentId);
                        if (vData1 != null)
                        {//Select for update data.
                            var vData = _departmentMastersRepository.SelectForUpdate(departmentMaster.StoreId, departmentMaster.DepartmentName, departmentMaster.DepartmentId);
                            if (vData.Count > 0)
                            {
                                Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                TempData["Sucessme"] = Message;
                                Session["Sucessmesg"] = Message;
                                return RedirectToAction("DepartmentEdit/", new { Id = departmentMaster.DepartmentId, StoreID = departmentMaster.StoreId });
                            }
                            //Get all Department Master list with selection.
                            DepartmentMaster objDept = _departmentMastersRepository.DepartmentMasterList().Where(a => a.DepartmentId == vData1.DepartmentId).FirstOrDefault();
                            objDept.ListId = vData1.ListId;
                            objDept.DepartmentName = departmentMaster.DepartmentName;
                            objDept.Description = departmentMaster.Description;
                            objDept.AccountNumber = departmentMaster.AccountNumber;
                            objDept.AccountTypeId = departmentMaster.AccountTypeId;
                            objDept.AccountDetailTypeId = departmentMaster.AccountDetailTypeId;
                            objDept.IsActive = Convert.ToBoolean(departmentMaster.IsActive);
                            objDept.StoreId = departmentMaster.StoreId;
                            objDept.ModifiedOn = DateTime.Now;
                            objDept.LastModifiedOn = DateTime.Now;
                            objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                            objDept.IsSync = 0;
                            objDept.SyncDate = DateTime.Now;
                            //This db class is update Department details
                            _departmentMastersRepository.UpdateDepartmentMaster(objDept);
                            iDeptId = objDept.DepartmentId;
                            if (iDeptId == 0)
                            {
                                Message = _commonRepository.GetMessageValue("DMDU", "Department Not Updated.");
                                TempData["Sucessme"] = Message;
                                Session["Sucessmesg"] = Message;
                                return RedirectToAction("Edit/", new { Id = departmentMaster.DepartmentId, StoreID = departmentMaster.StoreId });
                            }
                            else
                            {
                                TempData["complete"] = _commonRepository.GetMessageValue("DMUS", "Department Update Successfully..");
                            }
                        }
                        else
                        { //Select for update data.
                            var vData = _departmentMastersRepository.SelectForUpdate(departmentMaster.StoreId, departmentMaster.DepartmentName, departmentMaster.DepartmentId);
                            if (vData.Count > 0)
                            {
                                Message = _commonRepository.GetMessageValue("DMAE", "Department Name Already Exist");
                                TempData["Sucessme"] = Message;
                                Session["Sucessmesg"] = Message;
                                return RedirectToAction("Edit/", new { Id = departmentMaster.DepartmentId, StoreID = departmentMaster.StoreId });
                            }

                            DepartmentMaster objDept = new DepartmentMaster();
                            objDept.ListId = objResponse.ID;
                            objDept.DepartmentName = departmentMaster.DepartmentName;
                            objDept.AccountTypeId = departmentMaster.AccountTypeId;
                            objDept.AccountDetailTypeId = departmentMaster.AccountDetailTypeId;
                            objDept.Description = departmentMaster.Description;
                            objDept.AccountNumber = departmentMaster.AccountNumber;
                            objDept.IsSubAccount = departmentMaster.IsSubAccount;
                            objDept.IsActive = Convert.ToBoolean(departmentMaster.IsActive);
                            objDept.StoreId = departmentMaster.StoreId;
                            objDept.CreatedOn = DateTime.Now;
                            objDept.ModifiedOn = DateTime.Now;
                            objDept.LastModifiedOn = DateTime.Now;
                            objDept.CreatedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                            objDept.ModifiedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                            objDept.IsSync = 0;
                            objDept.SyncDate = DateTime.Now;
                            //This db class is save Department details
                            _departmentMastersRepository.SaveDepartmentMaster(objDept);
                            objDept = null;
                            TempData["complete"] = _commonRepository.GetMessageValue("DMDSS", "Department Save Successfully..");
                        }
                    }
                    //db.Entry(departmentMaster).State = EntityState.Modified;
                    //await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    logger.Error("ChartofAccountsController - Edit - " + DateTime.Now + " - " + ex.Message.ToString());
                  
                }
            }
            //Get All Account Details type List
            ViewBag.AccountDetailTypeId = new SelectList(_departmentMastersRepository.AccountDetailTypeList().Select(s => new { s.AccountDetailTypeId, s.DetailType }).OrderBy(o => o.DetailType).ToList(), "AccountDetailTypeId", "DetailType", departmentMaster.AccountDetailTypeId);
            //Get All Account Details type List
            ViewBag.AccountTypeId = new SelectList(_departmentMastersRepository.AccountTypeList().Select(s => new { s.AccountTypeId, s.AccountType }).OrderBy(o => o.AccountType), "AccountTypeId", "AccountType", departmentMaster.AccountTypeId);
            //using this class get Store master list with selection of storeid.
            ViewBag.StoreId = new SelectList(_departmentMastersRepository.StoreMasterList().Where(k => k.IsActive == true).Select(s => new { s.StoreId, s.Name }).OrderBy(o => o.Name).ToList(), "StoreId", "Name", departmentMaster.StoreId);
            //using this class get Store master list with selection of MultiStoreId.
            ViewBag.MultiStoreId = new MultiSelectList(_departmentMastersRepository.StoreMasterList().Where(k => k.IsActive == true).Select(s => new { s.StoreId, s.Name }).OrderBy(o => o.Name).ToList(), "StoreId", "Name", departmentMaster.MultiStoreId);
            if (TempData["complete"] != null)
            {
                ActivityLog ActLog = new ActivityLog();
                ActLog.Action = 2;
                //This Db class is used for get user firstname.
                ActLog.Comment = "Department " + "<a href='/DepartmentMasters/Details/" + departmentMaster.DepartmentId + "'>" + departmentMaster.DepartmentName + "</a> Edited by " + _commonRepository.getUserFirstName(UserName) + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                //This  class is used for Activity Log Insert.
                _activityLogRepository.ActivityLogInsert(ActLog);


                StatusMessageString = _commonRepository.GetMessageValue("DMUS", "Department Update Successfully..");
                ViewBag.StatusMessageString = StatusMessageString;
                Session["Sucessmesg"] = TempData["complete"].ToString();
                return RedirectToAction("Index", "DepartmentMasters");
            }
            if (TempData["Sucessme"] != null)
            {
                Session["Sucessmesg"] = TempData["Sucessme"].ToString();
                return RedirectToAction("Edit/", new { Id = departmentMaster.DepartmentId, StoreID = departmentMaster.StoreId });
            }

            return View(departmentMaster);
        }

        // GET: DepartmentMasters/Delete/5
        /// <summary>
        /// This method is Get department id for delete data.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Get Department Id.
            DepartmentMaster departmentMaster = await _departmentMastersRepository.GetDepartmentMasterId(id);
            if (departmentMaster == null)
            {
                return HttpNotFound();
            }
            return View(departmentMaster);
        }

        // POST: DepartmentMasters/Delete/5
        /// <summary>
        /// This method is delete Department data.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                //Get Department Id.
                DepartmentMaster departmentMaster = await _departmentMastersRepository.GetDepartmentMasterId(id);
                //Using this class delete department.                
                _departmentMastersRepository.DeleteConfirmed(departmentMaster);

                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                ActivityLog ActLog = new ActivityLog();
                ActLog.Action = 3;
                //This Db class is used for get user firstname.
                ActLog.Comment = "Department " + departmentMaster.DepartmentName + "</a> Deleted by " + _commonRepository.getUserFirstName(UserName) + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                //This  class is used for Activity Log Insert.
                _activityLogRepository.ActivityLogInsert(ActLog);

               
            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsController - DeleteConfirmed - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return RedirectToAction("Index");
        }
        /// <summary>
        /// This method is Active Status.
        /// </summary>
        /// <param name="deptid"></param>
        /// <returns></returns>
        public ActionResult ActiveStatus(int deptid)
        {
            try
            {
                //get active status by department id.
                _departmentMastersRepository.ActiveStatus(deptid);
                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsController - ActiveStatus - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("", JsonRequestBehavior.AllowGet);

            }
        }

        /// <summary>
        /// This method is In Active Status.
        /// </summary>
        /// <param name="deptid"></param>
        /// <returns></returns>
        public ActionResult InActiveStatus(int deptid)
        {
            try
            {
                //get In active status by department id.
                _departmentMastersRepository.InActiveStatus(deptid);
                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsController - InActiveStatus - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// This is Convert Grid object.
        /// </summary>
        /// <param name="gridProperty"></param>
        /// <returns></returns>
        private Syncfusion.EJ2.Grids.Grid ConvertGridObject(string gridProperty, List<DepartmentMasterList> data)
        {
            Syncfusion.EJ2.Grids.Grid GridModel = (Syncfusion.EJ2.Grids.Grid)Newtonsoft.Json.JsonConvert.DeserializeObject(gridProperty, typeof(Syncfusion.EJ2.Grids.Grid));

            try
            {
                GridColumnModel cols = (GridColumnModel)Newtonsoft.Json.JsonConvert.DeserializeObject("{\"allowGrouping\":false,\"allowPaging\":false,\"pageSettings\":{\"currentPage\":1,\"pageCount\":2,\"pageSize\":100},\"sortSettings\":{},\"allowPdfExport\":true,\"allowExcelExport\":true,\"aggregates\":[],\"filterSettings\":{\"immediateModeDelay\":1500,\"type\":\"CheckBox\"},\"groupSettings\":{\"columns\":[],\"enableLazyLoading\":false,\"disablePageWiseAggregates\":false},\"columns\":[],\"locale\":\"en-US\",\"searchSettings\":{\"key\":\"\",\"fields\":[]}}", typeof(GridColumnModel));
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "DepartmentName", HeaderText = "Department Name" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "StoreName", HeaderText = "Store Name" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "AccountType", HeaderText = "Account Type Name" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "AccountDetailType", HeaderText = "Account Detail Type" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "Status", HeaderText = "Status" });

                foreach (var item in cols.columns)
                {
                    item.AutoFit = true;
                    item.Width = "10%";
                }

                GridModel.Columns = cols.columns;
            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsController - ConvertGridObject - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return GridModel;
        }
        /// <summary>
        /// This method is used for PDF Export of Department Master.
        /// </summary>
        /// <param name="gridModel"></param>
        /// <returns></returns>
        public ActionResult PdfExport(string gridModel)
        {
            departmentMastersViewModal.StoreIds = 0;

            List<DepartmentMasterList> lstDepartment = new List<DepartmentMasterList>();
            GridPdfExport exp = new GridPdfExport();

            try
            {
                if (Session["storeid"] != null)
                {
                    departmentMastersViewModal.StoreIds = Convert.ToInt32(Convert.ToString(Session["storeid"]));
                }

                var Storedata = _departmentMastersRepository.StoreMasterList();
                var AccountDetaildata = _departmentMastersRepository.AccountDetailTypeList();
                var AccountTypedata = _departmentMastersRepository.AccountTypeList();

                if (departmentMastersViewModal.StoreIds == 0)
                {
                    //Get all Department Master list with selection.

                    lstDepartment = (_departmentMastersRepository.DepartmentMasterList().Select(s => new { s.DepartmentId, s.DepartmentName, s.IsActive, s.AccountTypeId, s.StoreId, s.AccountDetailTypeId }).ToList()).Select(s => new DepartmentMasterList
                    {
                        DepartmentId = s.DepartmentId,
                        DepartmentName = s.DepartmentName,
                        Status = s.IsActive == true ? "Active" : "Inactive",
                        AccountType = AccountTypedata.Where(w => w.AccountTypeId == s.AccountTypeId).Select(o => o.AccountType).FirstOrDefault(),
                        StoreName = Storedata.Where(w => w.StoreId == s.StoreId).Select(o => o.NickName).FirstOrDefault(),
                        AccountDetailType = AccountDetaildata.Where(w => w.AccountDetailTypeId == s.AccountDetailTypeId).Select(o => o.DetailType).FirstOrDefault()
                    }).ToList();
                }
                else
                {
                    //Get all Department Master list with selection.
                    lstDepartment = (_departmentMastersRepository.DepartmentMasterList().Where(a => a.StoreId == departmentMastersViewModal.StoreIds).Select(s => new { s.DepartmentId, s.DepartmentName, s.IsActive, s.AccountTypeId, s.StoreId, s.AccountDetailTypeId }).ToList()).Select(s => new DepartmentMasterList
                    {
                        DepartmentId = s.DepartmentId,
                        DepartmentName = s.DepartmentName,
                        Status = s.IsActive == true ? "Active" : "Inactive",
                        AccountType = AccountTypedata.Where(w => w.AccountTypeId == s.AccountTypeId).Select(o => o.AccountType).FirstOrDefault(),
                        StoreName = Storedata.Where(w => w.StoreId == s.StoreId).Select(o => o.NickName).FirstOrDefault(),
                        AccountDetailType = AccountDetaildata.Where(w => w.AccountDetailTypeId == s.AccountDetailTypeId).Select(o => o.DetailType).FirstOrDefault()
                    }).ToList();
                }

                PdfDocument doc = new PdfDocument();
                doc.PageSettings.Orientation = PdfPageOrientation.Landscape;
                doc.PageSettings.Size = PdfPageSize.A3;


                exp.Theme = "flat-saffron";
                exp.FileName = "Departments.pdf";
                exp.PdfDocument = doc;
            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsController - PdfExport - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            Syncfusion.EJ2.Grids.Grid gridProperty = ConvertGridObject(gridModel, lstDepartment);
            return exp.PdfExport<DepartmentMasterList>(gridProperty, lstDepartment);
        }
        /// <summary>
        /// This method is used for Excel Export of Department Master.
        /// </summary>
        /// <param name="gridModel"></param>
        /// <returns></returns>
        public async Task<ActionResult> ExcelExport(string gridModel)
        {
            departmentMastersViewModal.StoreIds = 0;

            List<DepartmentMasterList> lstDepartment = new List<DepartmentMasterList>();
            GridExcelExport exp = new GridExcelExport();

            try
            {
                if (Session["storeid"] != null)
                {
                    departmentMastersViewModal.StoreIds = Convert.ToInt32(Convert.ToString(Session["storeid"]));
                }

                var Storedata = _departmentMastersRepository.StoreMasterList();
                var AccountDetaildata = _departmentMastersRepository.AccountDetailTypeList();
                var AccountTypedata = _departmentMastersRepository.AccountTypeList();

                if (departmentMastersViewModal.StoreIds == 0)
                {
                    //Get all Department Master list with selection.

                    lstDepartment = (_departmentMastersRepository.DepartmentMasterList().Select(s => new { s.DepartmentId, s.DepartmentName, s.IsActive, s.AccountTypeId, s.StoreId, s.AccountDetailTypeId }).ToList()).Select(s => new DepartmentMasterList
                    {
                        DepartmentId = s.DepartmentId,
                        DepartmentName = s.DepartmentName,
                        Status = s.IsActive == true ? "Active" : "Inactive",
                        AccountType = AccountTypedata.Where(w => w.AccountTypeId == s.AccountTypeId).Select(o => o.AccountType).FirstOrDefault(),
                        StoreName = Storedata.Where(w => w.StoreId == s.StoreId).Select(o => o.NickName).FirstOrDefault(),
                        AccountDetailType = AccountDetaildata.Where(w => w.AccountDetailTypeId == s.AccountDetailTypeId).Select(o => o.DetailType).FirstOrDefault()
                    }).ToList();
                }
                else
                {
                    //Get all Department Master list with selection.
                    lstDepartment = (_departmentMastersRepository.DepartmentMasterList().Where(a => a.StoreId == departmentMastersViewModal.StoreIds).Select(s => new { s.DepartmentId, s.DepartmentName, s.IsActive, s.AccountTypeId, s.StoreId, s.AccountDetailTypeId }).ToList()).Select(s => new DepartmentMasterList
                    {
                        DepartmentId = s.DepartmentId,
                        DepartmentName = s.DepartmentName,
                        Status = s.IsActive == true ? "Active" : "Inactive",
                        AccountType = AccountTypedata.Where(w => w.AccountTypeId == s.AccountTypeId).Select(o => o.AccountType).FirstOrDefault(),
                        StoreName = Storedata.Where(w => w.StoreId == s.StoreId).Select(o => o.NickName).FirstOrDefault(),
                        AccountDetailType = AccountDetaildata.Where(w => w.AccountDetailTypeId == s.AccountDetailTypeId).Select(o => o.DetailType).FirstOrDefault()
                    }).ToList();
                }

                exp.FileName = "Departments.xlsx";
            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsController - ExcelExport - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            Syncfusion.EJ2.Grids.Grid gridProperty = ConvertGridObject(gridModel, lstDepartment);
            return exp.ExcelExport<DepartmentMasterList>(gridProperty, lstDepartment);

        }
        /// <summary>
        /// This method is used for CSV Export of Department Master.
        /// </summary>
        /// <param name="gridModel"></param>
        /// <returns></returns>
        public async Task<ActionResult> CsvExport(string gridModel)
        {
            departmentMastersViewModal.StoreIds = 0;
            List<DepartmentMasterList> lstDepartment = new List<DepartmentMasterList>();
            GridExcelExport exp = new GridExcelExport();

            try
            {
                if (Session["storeid"] != null)
                {
                    departmentMastersViewModal.StoreIds = Convert.ToInt32(Convert.ToString(Session["storeid"]));
                }
                var Storedata = _departmentMastersRepository.StoreMasterList();
                var AccountDetaildata = _departmentMastersRepository.AccountDetailTypeList();
                var AccountTypedata = _departmentMastersRepository.AccountTypeList();
                if (departmentMastersViewModal.StoreIds == 0)
                {
                    //Get all Department Master list with selection.
                    lstDepartment = (_departmentMastersRepository.DepartmentMasterList().Select(s => new { s.DepartmentId, s.DepartmentName, s.IsActive, s.AccountTypeId, s.StoreId, s.AccountDetailTypeId }).ToList()).Select(s => new DepartmentMasterList
                    {
                        DepartmentId = s.DepartmentId,
                        DepartmentName = s.DepartmentName,
                        Status = s.IsActive == true ? "Active" : "Inactive",
                        AccountType = AccountTypedata.Where(w => w.AccountTypeId == s.AccountTypeId).Select(o => o.AccountType).FirstOrDefault(),
                        StoreName = Storedata.Where(w => w.StoreId == s.StoreId).Select(o => o.NickName).FirstOrDefault(),
                        AccountDetailType = AccountDetaildata.Where(w => w.AccountDetailTypeId == s.AccountDetailTypeId).Select(o => o.DetailType).FirstOrDefault()
                    }).ToList();
                }
                else
                {
                    //Get all Department Master list with selection.
                    lstDepartment = (_departmentMastersRepository.DepartmentMasterList().Where(a => a.StoreId == departmentMastersViewModal.StoreIds).Select(s => new { s.DepartmentId, s.DepartmentName, s.IsActive, s.AccountTypeId, s.StoreId, s.AccountDetailTypeId }).ToList()).Select(s => new DepartmentMasterList
                    {
                        DepartmentId = s.DepartmentId,
                        DepartmentName = s.DepartmentName,
                        Status = s.IsActive == true ? "Active" : "Inactive",
                        AccountType = AccountTypedata.Where(w => w.AccountTypeId == s.AccountTypeId).Select(o => o.AccountType).FirstOrDefault(),
                        StoreName = Storedata.Where(w => w.StoreId == s.StoreId).Select(o => o.NickName).FirstOrDefault(),
                        AccountDetailType = AccountDetaildata.Where(w => w.AccountDetailTypeId == s.AccountDetailTypeId).Select(o => o.DetailType).FirstOrDefault()
                    }).ToList();
                }
                exp.FileName = "Departments.csv";
            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsController - CsvExport - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            Syncfusion.EJ2.Grids.Grid gridProperty = ConvertGridObject(gridModel, lstDepartment);
            return exp.CsvExport<DepartmentList>(gridProperty, lstDepartment);
        }
    }
}
