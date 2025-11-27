using EntityModels.HRModels;
using EntityModels.Models;
using Newtonsoft.Json;
using NLog;
using Repository;
using Repository.IRepository;
using Syncfusion.EJ2.Base;
using Syncfusion.EJ2.GridExport;
using Syncfusion.EJ2.Inputs;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using SynthesisQBOnline.BAL;
using SynthesisViewModal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Utility;

namespace SysnthesisRepo.Controllers
{
    public class ExpenseAccountsController : Controller
    {
        private readonly IExpenseAccountsRepository _ExpenseCheckSettingRepository;
        private readonly ICommonRepository _CommonRepository;
        private readonly IUserActivityLogRepository _ActivityLogRepository;
        private readonly IMastersBindRepository _MastersBindRepository;
        private readonly IChartofAccountsRepository _departmentMastersRepository;
        private readonly IQBRepository _qBRepository;
        protected static string StatusMessage = "";
        protected static int sType = 0;

        protected static Array Arr;
        protected static bool IsArray;
        protected static bool IsEdit = false;
        private const int FirstPageIndex = 1;
        protected static int TotalDataCount;
        protected static string IsFilter = "0";
        Logger logger = LogManager.GetCurrentClassLogger();
        string SuccessMessage = string.Empty;
        string ErrorMessage = string.Empty;
        public ExpenseAccountsController()
        {
            this._ExpenseCheckSettingRepository = new ExpenseAccountsRepository(new DBContext());
            this._CommonRepository = new CommonRepository(new DBContext());
            this._ActivityLogRepository = new UserActivityLogRepository(new DBContext());
            this._MastersBindRepository = new MastersBindRepository(new DBContext());
            this._departmentMastersRepository = new ChartofAccountsRepository(new DBContext());
            this._qBRepository = new QBRepository(new DBContext());
        }

        // GET: ExpenseCheckSetting
        /// <summary>
        /// This Method return Index view with Expense-Check Setting.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,ViewExpenseCheckSetting")]
        public ActionResult Index()
        {
            ViewBag.Title = "Expense Accounts - Synthesis";

            ExpenseCheck_Setting obj = new ExpenseCheck_Setting();
            try
            {
                int storeid = 0;
                if (Convert.ToString(Session["storeid"]) != "0" && Convert.ToString(Session["storeid"]) != "")
                {
                    ViewBag.StatusMessage = StatusMessage;
                    StatusMessage = "";
                    ViewBag.sType = sType;
                    sType = 0;
                    storeid = Convert.ToInt32(Session["storeid"]);
                }
                else
                {
                    ViewBag.StatusMessage = "NoStoreSelected";
                }
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsController - Index - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return View(obj);
        }
        /// <summary>
        /// This method is get Expense-Check Setting with Id.
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetData(int Type)
        {
            ExpenseCheck_Setting obj = new ExpenseCheck_Setting();
            int storeid = 0;
            try
            {
                if (Convert.ToString(Session["storeid"]) != "0" && Convert.ToString(Session["storeid"]) != "")
                {
                    sType = Type;
                    storeid = Convert.ToInt32(Session["storeid"]);
                    //This  class is Expense Check_SettingList.
                    try
                    {
                        obj.ExpenseCheck_SettingList = _ExpenseCheckSettingRepository.ExpenseCheck_SettingList(storeid, Type);
                    }
                    catch (Exception ex)
                    {
                        logger.Error("ExpenseAccountsController:GetData :" + ex.Message.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsController - Getdata - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return PartialView("_ExpenseCheckSetting", obj);
        }
        /// <summary>
        /// This method is Add Expense-Check Setting.
        /// </summary>
        /// <param name="ExpenseCheckSettingViewModal"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(int[] Type = null, int[] IsActive = null, int[] StoreID = null, string[] DeptExcludeList = null)
        {
            ExpenseCheckSettingViewModal expenseCheckSettingViewModal = new ExpenseCheckSettingViewModal();
            expenseCheckSettingViewModal.Type = Type;
            expenseCheckSettingViewModal.IsActive = IsActive;
            expenseCheckSettingViewModal.StoreID = StoreID;
            expenseCheckSettingViewModal.DeptExcludeList = DeptExcludeList;

            try
            {
                int StoreIDs = expenseCheckSettingViewModal.StoreID[0];
                int TypeID = expenseCheckSettingViewModal.Type[0];
                ViewBag.sType = TypeID;
                string strDept = "";
                //This  class is Expense Check_SettingList Remove range.
                _ExpenseCheckSettingRepository.ExpenseCheck_SettingsRemoveRange(StoreIDs, TypeID);

                for (int i = 0; i < IsActive.Count(); i++)
                {
                    strDept = "";
                    ExpenseCheck_Setting obj = new ExpenseCheck_Setting();
                    obj.Type = expenseCheckSettingViewModal.Type[0];
                    obj.StoreId = expenseCheckSettingViewModal.StoreID[0];
                    obj.IsActive = true;
                    obj.AccountId = expenseCheckSettingViewModal.IsActive[i];
                    if (expenseCheckSettingViewModal.DeptExcludeList != null)
                    {
                        strDept = "";
                        obj.Type = expenseCheckSettingViewModal.Type[0];
                        obj.StoreId = expenseCheckSettingViewModal.StoreID[0];
                        obj.IsActive = true;
                        obj.AccountId = expenseCheckSettingViewModal.IsActive[i];
                        if (expenseCheckSettingViewModal.DeptExcludeList != null)
                        {
                            for (int j = 0; j < expenseCheckSettingViewModal.DeptExcludeList.Count(); j++)
                            {
                                if (expenseCheckSettingViewModal.DeptExcludeList[j].ToString().Split('_')[0].ToString() == expenseCheckSettingViewModal.IsActive[i].ToString())
                                {
                                    strDept = strDept + (strDept == "" ? expenseCheckSettingViewModal.DeptExcludeList[j].ToString().Split('_')[1].ToString() : "," + expenseCheckSettingViewModal.DeptExcludeList[j].ToString().Split('_')[1].ToString());
                                }
                            }
                        }
                        obj.ExcludeList = strDept;
                        //This  class is Add Expense Check_SettingList.
                        _ExpenseCheckSettingRepository.AddExpenseCheck_Settings(obj);
                    }
                    StatusMessage = "Success";
                    sType = TypeID;
                }
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsController - Index - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return RedirectToAction("Index");
        }
        /// <summary>
        /// This method Get Expense Check
        /// </summary>
        /// <param name="ExpenseCheckSettingViewModal"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,ViewExpense")]
        // GET: Invoices
        public async Task<ActionResult> ExpenseCheckIndex(ExpenseCheckSettingViewModal ExpenseCheckSettingViewModal)
        {
            ViewBag.Title = "Expense Accounts - Synthesis";

            ExpenseCheck_Setting obj = new ExpenseCheck_Setting();
            ViewBag.checksetting = obj;


            if (StatusMessage == "Success1" || StatusMessage == "Success" || StatusMessage == "Delete")
            {
                ViewBag.StatusMessage = StatusMessage;
                //StatusMessage = "";
            }
            else
            {
                ViewBag.StatusMessage = "";
            }
            return View();
        }
        /// <summary>
        /// This method Get Expense Check Grid.
        /// </summary>
        /// <param name="ExpenseCheckSettingViewModal"></param>
        /// <returns></returns>
        public ActionResult ExpenseCheckIndexGrid(ExpenseCheckSettingViewModal ExpenseCheckSettingViewModal)
        {
            List<ExpenseCheckSelect> BindData = new List<ExpenseCheckSelect>();
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            //This  class is Get user id.
            int UserTypesID = _CommonRepository.getUserTypeId(UserName);
            #region MyRegion_BindData
            int startIndex = ((ExpenseCheckSettingViewModal.currentPageIndex - 1) * ExpenseCheckSettingViewModal.PageSize) + 1;
            int endIndex = startIndex + ExpenseCheckSettingViewModal.PageSize - 1;
            var ColumnName = typeof(Invoice).GetProperties().Where(p => p.Name == ExpenseCheckSettingViewModal.orderby).FirstOrDefault();
            //var OrderByCol = ColumnName.GetValue(n, null);

            IEnumerable Data = null;
            if (ExpenseCheckSettingViewModal.searchdashbord == "Clear")
            {
                Session["ECsearchdashbord"] = "";
                ExpenseCheckSettingViewModal.searchdashbord = "";
            }
            else
            {
                if (ExpenseCheckSettingViewModal.searchdashbord != "")
                {
                    Session["ECsearchdashbord"] = ExpenseCheckSettingViewModal.searchdashbord;
                }
            }
            //This Db class is used for get user firstname.
            var UserTypeId = _CommonRepository.getUserFirstName(UserName);
            if (ExpenseCheckSettingViewModal.IsBindData == 1 || IsEdit == true)
            {
                var StoreId = Convert.ToInt32(Session["storeid"]);
                var AscDsc = "Desc";
                if (ExpenseCheckSettingViewModal.IsAsc == 1)
                {
                    AscDsc = "asc";
                }
                if (ExpenseCheckSettingViewModal.searchdashbord == "Clear")
                {
                    ExpenseCheckSettingViewModal.searchdashbord = "";
                }
                else
                {
                    ExpenseCheckSettingViewModal.searchdashbord = (Session["ECsearchdashbord"] == null ? "" : Session["ECsearchdashbord"].ToString()); //.Replace("'", "''")
                }
                if (Session["ECstartdate"] != null)
                {
                    ExpenseCheckSettingViewModal.startdate = Session["ECstartdate"].ToString();
                }
                if (Session["ECenddate"] != null)
                {
                    ExpenseCheckSettingViewModal.enddate = Session["ECenddate"].ToString();
                }
                if (Session["ECLstStore"] != null)
                {
                    ExpenseCheckSettingViewModal.storename = (Session["ECLstStore"].ToString() == "" ? 0 : Convert.ToInt32(Session["ECLstStore"].ToString()));
                }
                if (Session["ECAmtMinimum"] != null)
                {
                    ExpenseCheckSettingViewModal.AmtMinimum = (Session["ECAmtMinimum"].ToString() == "" ? 0 : Convert.ToDecimal(Session["ECAmtMinimum"].ToString()));
                }
                if (Session["ECAmtMaximum"] != null)
                {
                    ExpenseCheckSettingViewModal.AmtMaximum = (Session["ECAmtMaximum"].ToString() == "" ? 0 : Convert.ToDecimal(Session["ECAmtMaximum"].ToString()));
                }
                if (Session["payment"] != null)
                {
                    ExpenseCheckSettingViewModal.payment = Session["payment"].ToString();
                }
                if (Session["deptname"] != null)
                {
                    ExpenseCheckSettingViewModal.deptid = (Session["deptname"].ToString() == "" ? 0 : Convert.ToInt32(Session["deptname"].ToString()));
                }
                try
                {
                    var strStore = "";
                    //This  class is Get user id.
                    int UserID = _CommonRepository.getUserId(UserName);
                    //This  class is Expense Check_SettingList.
                    strStore = _ExpenseCheckSettingRepository.GetExpenseCheck_StoreList(StoreId, UserID, Roles.IsUserInRole("Administrator"));
                    if (StoreId == 0 && strStore.Length > 0)
                    {
                        var S = (strStore.Split(',').Count() == 1 ? Convert.ToInt32(strStore.Split(',')[0]) : 0);
                        if (S > 0)
                        {
                            Session["storeid"] = S;
                            StoreId = S;
                        }
                    }

                    if (ExpenseCheckSettingViewModal.payment == "")
                    {
                        ExpenseCheckSettingViewModal.payment = null;
                    }
                    else
                    {
                        if (ExpenseCheckSettingViewModal.payment == "Expense")
                        {
                            ExpenseCheckSettingViewModal.payment = "2";
                        }
                        else
                        {
                            ExpenseCheckSettingViewModal.payment = "1";
                        }
                    }
                    //This  class is Expense Check Bind data. 
                    BindData = _ExpenseCheckSettingRepository.GetExpenseCheck_BindData(ExpenseCheckSettingViewModal, strStore, UserTypesID, AscDsc);
                    TotalDataCount = BindData.OfType<ExpenseCheckSelect>().ToList().Count();
                }
                catch (Exception ex)
                {
                    logger.Error("ExpenseAccountsController - ExpenseCheckIndexGrid -" + DateTime.Now + " - " + ex.Message.ToString());
                }
            }


            if (TotalDataCount == 0)
            {
                StatusMessage = "NoItem";
            }

            ViewBag.IsBindData = ExpenseCheckSettingViewModal.IsBindData;
            ViewBag.CurrentPageIndex = ExpenseCheckSettingViewModal.currentPageIndex;
            ViewBag.LastPageIndex = this.getLastPageIndex(ExpenseCheckSettingViewModal.PageSize);
            ViewBag.OrderByVal = ExpenseCheckSettingViewModal.orderby;
            ViewBag.IsAscVal = ExpenseCheckSettingViewModal.IsAsc;
            ViewBag.PageSize = ExpenseCheckSettingViewModal.PageSize;
            ViewBag.Alpha = ExpenseCheckSettingViewModal.Alpha;
            ViewBag.SearchRecords = ExpenseCheckSettingViewModal.SearchRecords;
            ViewBag.startindex = startIndex;
            ViewBag.SearchFlg = ExpenseCheckSettingViewModal.SearchFlg;

            ViewBag.searchdashbord = ExpenseCheckSettingViewModal.searchdashbord;
            ViewBag.DocSaved = _CommonRepository.GetMessageValue("ECDS", "Document Save Successfully.");
            ViewBag.ExcExpense = _CommonRepository.GetMessageValue("ECEES", "Expense Excluded Successfully.");
            if (ExpenseCheckSettingViewModal.IsAsc == 1)
            {
                ViewBag.AscVal = 0;
            }
            else
            {
                ViewBag.AscVal = 1;
            }
            if (IsFilter == "1")
            {
                ViewBag.IsFilter = "1";
                IsFilter = "0";
            }
            if (StatusMessage == null) { StatusMessage = ""; }
            if (StatusMessage == "")
            {
                ViewBag.StatusMessage = "";
            }
            else
            {
                ViewBag.StatusMessage = StatusMessage;
            }
            if (TotalDataCount < endIndex)
            {
                ViewBag.endIndex = TotalDataCount;
            }
            else
            {
                ViewBag.endIndex = endIndex;
            }
            ViewBag.TotalDataCount = TotalDataCount;
            StatusMessage = "";
            int StoreID = 0;
            if (Convert.ToString(Session["storeid"]) != "0")
            {
                StoreID = Convert.ToInt32(Session["storeid"]);
            }
            //Get Expense Check Details.
            var ExpenseDeptIds = _ExpenseCheckSettingRepository.GetExpenseCheckDetails();
            //Check User type.
            if (_ExpenseCheckSettingRepository.CheckUserTypeMasters(UserTypesID))
            {
                //Get Expense Department Ids
                ExpenseDeptIds = _ExpenseCheckSettingRepository.GetExpenseDeptIds(UserTypesID, StoreID);
            }
            //Get All Departments
            ViewBag.DrpLstdept = new SelectList(_ExpenseCheckSettingRepository.GetDepartmentMasters(ExpenseDeptIds, StoreID), "DepartmentId", "DepartmentName");
            return View(BindData);
            #endregion
        }

        /// <summary>
        /// This class use to get last page index with pagesize.
        /// </summary>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        private int getLastPageIndex(int PageSize)
        {
            int lastPageIndex = Convert.ToInt32(TotalDataCount) / PageSize;
            if (TotalDataCount % PageSize > 0)
            {
                lastPageIndex += 1;
            }

            return lastPageIndex;
        }

        /// <summary>
        /// This method Get Expense Check Grid
        /// </summary>
        /// <param name="ExpenseCheckSettingViewModal"></param>
        /// <param name="StDate"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExpenseCheckIndexGrid(ExpenseCheckSettingViewModal ExpenseCheckSettingViewModal, string StDate = "")
        {
            IsFilter = "0";
            string paymethod = "";
            try
            {
                if (ExpenseCheckSettingViewModal.radio != null)
                {
                    IsFilter = "1";
                    if (ExpenseCheckSettingViewModal.radio.Count() > 1)
                    {
                        paymethod = "";
                    }
                    else if (ExpenseCheckSettingViewModal.radio.Count() == 1)
                    {
                        paymethod = ExpenseCheckSettingViewModal.radio[0];
                    }
                }
                else
                {
                    paymethod = "";
                    IsFilter = "0";
                }
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsController - ExpenseCheckIndexGrid - " + DateTime.Now + " - " + ex.Message.ToString());
            }


            Session["ECLstStore"] = ExpenseCheckSettingViewModal.ECLstStore;
            Session["ECstartdate"] = ExpenseCheckSettingViewModal.txtstartdate;
            Session["ECenddate"] = ExpenseCheckSettingViewModal.txtenddate;
            Session["ECAmtMinimum"] = ExpenseCheckSettingViewModal.AmtMinimum;
            Session["ECAmtMaximum"] = ExpenseCheckSettingViewModal.AmtMaximum;
            Session["payment"] = paymethod;
            Session["deptname"] = ExpenseCheckSettingViewModal.DrpLstdept;
            //Session["DrpLstVendor"] = DrpLstVendor;
            return RedirectToAction("ExpenseCheckIndex", "ExpenseCheckSetting", new { IsBindData = 1, currentPageIndex = 1, orderby = "InvoiceId", IsAsc = 0, PageSize = 50, SearchRecords = 1, Alpha = "", storename = ExpenseCheckSettingViewModal.ECLstStore, startdate = ExpenseCheckSettingViewModal.txtstartdate, enddate = ExpenseCheckSettingViewModal.txtenddate, searchdashbord = "", SearchFlg = "F" });
        }
        /// <summary>
        /// This method Get Export Expense Check data.
        /// </summary>
        /// <param name="ExpenseCheckSettingViewModal"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,ViewExpense")]
        public ActionResult ExportExpenseCheckData(ExpenseCheckSettingViewModal ExpenseCheckSettingViewModal)
        {
            try
            {
                ExpenseCheckSettingViewModal.StoreId = Convert.ToInt32(Session["storeid"]);
                var strStore = "";
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                //This  class is Get user id.
                int UserID = _CommonRepository.getUserId(UserName);
                //This  class is Get usertype id.
                int UserTypeId = _CommonRepository.getUserTypeId(UserName);
                //This  class is Expense Check_StoreList.
                strStore = _ExpenseCheckSettingRepository.GetExpenseCheck_StoreList(ExpenseCheckSettingViewModal.StoreId, UserID, Roles.IsUserInRole("Administrator"));
                if (ExpenseCheckSettingViewModal.StoreId == 0 && strStore.Length > 0)
                {
                    var S = (strStore.Split(',').Count() == 1 ? Convert.ToInt32(strStore.Split(',')[0]) : 0);
                    if (S > 0)
                    {
                        Session["storeid"] = S;
                        ExpenseCheckSettingViewModal.StoreId = S;
                    }
                }

                System.Data.DataTable dt1 = new System.Data.DataTable();
                List<ExpenseCheckSelect> List = new List<ExpenseCheckSelect>();
                //Check Expense preview data.
                List = _ExpenseCheckSettingRepository.ExpenseCheckPreviewData(ExpenseCheckSettingViewModal, UserTypeId, strStore);

                dt1 = Common.LINQToDataTable(List);
                Export oExport = new Export();
                string FileName = "ExpenseCheckReport" + ".xls";
                dt1.Columns.Remove("InvoiceID");

                int[] ColList = { 0, 1, 2, 3, 4, 5, 6, 7 };
                string[] arrHeader = {
                 "Vendor Name",
                 "Type",
                 "Invoice Date",
                 "Modify Date",
                 "Store",
                 "Amount",
                 "Department",
                 "Memo"
                 };

                //only change file extension to .xls for excel file
                string sFilterBy = "";
                if (ExpenseCheckSettingViewModal.startdate != "")
                {
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Start Date: " + ExpenseCheckSettingViewModal.startdate;
                }
                if (ExpenseCheckSettingViewModal.enddate != "")
                {
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "End Date: " + ExpenseCheckSettingViewModal.enddate;
                }

                if (ExpenseCheckSettingViewModal.StoreId != 0)
                {
                    //Get All Store Name.
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Store: " + _ExpenseCheckSettingRepository.GetStoreName(ExpenseCheckSettingViewModal.StoreId);
                }

                oExport.ExportDetails(dt1, ColList, arrHeader, Export.ExportFormat.Excel, FileName, sFilterBy, "Expense Check Report");
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsController - ExportExpenseCheckData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return RedirectToAction("ExpenseCheckSetting", "ExpenseCheckIndex");
        }

        /// <summary>
        /// This class use for  Get Expense Check_storelist 
        /// </summary>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        private string GetExpenseCheck_StoreList(int StoreId)
        {
            string strList = "";
            try
            {
                if (!Roles.IsUserInRole("Administrator"))
                {
                    if (StoreId == 0)
                    {
                        var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                        //This  class is Get user id.
                        //Get Store for expense check.
                        strList = _ExpenseCheckSettingRepository.GetStore_ForExpenseCheck(_CommonRepository.getUserId(UserName));
                    }
                    else
                    {
                        strList = StoreId.ToString();
                    }
                }
                else
                {
                    strList = StoreId.ToString();
                }
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsController - GetExpenseCheck_StoreList - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return strList;
        }
        /// <summary>
        /// This method is used to clear session.
        /// </summary>
        /// <returns></returns>
        public JsonResult ClearSession()
        {
            Session["searchdashbord"] = "";
            Session["AmtMaximum"] = "";
            Session["AmtMinimum"] = "";
            Session["deptname"] = "";
            Session["startdate"] = "";
            Session["enddate"] = "";
            Session["payment"] = "";
            Session["searchdashbord"] = "";
            Session["DrpLstStore"] = "";
            Session["DrpLstVendor"] = "";
            Session["IPsearchdashbord"] = "";
            Session["IPstartdate"] = "";
            Session["IPenddate"] = "";

            Session["ECLstStore"] = "";
            Session["ECsearchdashbord"] = "";
            Session["ECstartdate"] = "";
            Session["ECenddate"] = "";

            return Json("True", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is save Expense document.
        /// </summary>
        /// <param name="fileinput"></param>
        /// <param name="ExpenseId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveExpenseDocument(HttpPostedFileBase fileinput, string ExpenseId)
        {
            try
            {
                if (fileinput != null)
                {
                    if (fileinput.ContentLength > 0)
                    {
                        int? StoreID = null;
                        if (Session["StoreId"] != null && !string.IsNullOrEmpty(Session["StoreId"].ToString()))
                        {
                            StoreID = Convert.ToInt32(Session["StoreId"]);
                        }
                        var allowedExtensions = new[] { ".pdf" };
                        var extension = Path.GetExtension(fileinput.FileName);
                        var Ext = Convert.ToString(extension).ToLower();
                        var DSacn_Title = AdminSiteConfiguration.GetRandomNo() + Path.GetFileName(fileinput.FileName);
                        DSacn_Title = AdminSiteConfiguration.RemoveSpecialCharacter(DSacn_Title);
                        //var folderName = Request.PhysicalApplicationPath + "UserFiles\\ExpenseDocuments";
                        //if (!Directory.Exists(folderName))
                        //{
                        //    Directory.CreateDirectory(folderName);
                        //}


                        var baseFolderPath = Request.PhysicalApplicationPath + "UserFiles\\ExpenseDocuments";
                        if (!Directory.Exists(baseFolderPath))
                        {
                            Directory.CreateDirectory(baseFolderPath);
                        }
                        var yearFolderPath = Path.Combine(baseFolderPath, DateTime.Now.Year.ToString());
                        if (!Directory.Exists(yearFolderPath))
                        {
                            Directory.CreateDirectory(yearFolderPath);
                        }
                        var storeFolderPath = Path.Combine(yearFolderPath, StoreID.ToString());
                        if (!Directory.Exists(storeFolderPath))
                        {
                            Directory.CreateDirectory(storeFolderPath);
                        }
                        var monthFolderPath = Path.Combine(storeFolderPath, DateTime.Now.ToString("MMMM"));
                        if (!Directory.Exists(monthFolderPath))
                        {
                            Directory.CreateDirectory(monthFolderPath);
                        }
                        var userIdFolderPath = Path.Combine(monthFolderPath, UserModule.getUserId().ToString());
                        if (!Directory.Exists(userIdFolderPath))
                        {
                            Directory.CreateDirectory(userIdFolderPath);
                        }
                        var fileSavePath = System.IO.Path.Combine(userIdFolderPath, DSacn_Title);
                        var relativePath = fileSavePath.Substring(baseFolderPath.Length).TrimStart('\\', '/');


                        //var path1 = fileSavePath + "\\" + DSacn_Title;
                        fileinput.SaveAs(fileSavePath);
                        ExpenseCheckDocuments obj = new ExpenseCheckDocuments();
                        obj.ExpenseCheckId = Convert.ToInt32(ExpenseId);
                        //obj.DocumentName = DSacn_Title;
                        obj.DocumentName = relativePath;
                        obj.CreatedOn = DateTime.Now;
                        var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                        //This  class is Get user id.
                        obj.CreatedBy = _CommonRepository.getUserId(UserName);
                        //Add expense check Documents.
                        _ExpenseCheckSettingRepository.AddexpenseCheckDocuments(obj);

                        return Json("OK", JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsController - SaveExpenseDocument - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return Json("Error", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Exclude Expense.
        /// </summary>
        /// <param name="ExpenseId"></param>
        /// <returns></returns>
        public ActionResult ExcludeExpense(int ExpenseId)
        {
            try
            {
                //Update Expense check details.
                _ExpenseCheckSettingRepository.UpdateExpenseCheckDetail(ExpenseId, true);
                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsController - ExcludeExpense - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// This method is Exclude Expense List With preview data.
        /// </summary>
        /// <returns></returns>
        public ActionResult ExcludedExpenseList()
        {
            var StoreId = Convert.ToInt32(Session["storeid"]);
            var strStore = "";
            try
            {
                strStore = GetExpenseCheck_StoreList(StoreId);
                if (StoreId == 0 && strStore.Length > 0)
                {
                    var S = (strStore.Split(',').Count() == 1 ? Convert.ToInt32(strStore.Split(',')[0]) : 0);
                    if (S > 0)
                    {
                        Session["storeid"] = S;
                        StoreId = S;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsController - ExcludedExpenseList - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            List<ExpenseCheckExcludedList> lstExpense = new List<ExpenseCheckExcludedList>();
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            //This  class is Get usertype id.
            lstExpense = _ExpenseCheckSettingRepository.ExpenseCheckPreviewData(_CommonRepository.getUserTypeId(UserName), strStore);
            return Json(lstExpense, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// This method is get Include Expense.
        /// </summary>
        /// <param name="selectedval"></param>
        /// <returns></returns>
        public ActionResult IncludeExpense(int selectedval)
        {
            try
            {
                //This class is Update Expense check fro Include expense details.
                _ExpenseCheckSettingRepository.UpdateExpenseCheckDetail(false, selectedval);
                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsController - IncludeExpense - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// This method is get Force Iclude Exclude Expense.
        /// </summary>
        /// <param name="selectedval"></param>
        /// <returns></returns>
        public ActionResult ForceIncludeExpense(int selectedval)
        {
            try
            {
                //This class is Update Expense check force Include expense details.
                _ExpenseCheckSettingRepository.UpdateExpenseCheckDetail(selectedval, false);
                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsController - ForceIncludeExpense - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("Error", JsonRequestBehavior.AllowGet);

            }
        }

        /// <summary>
        /// This method is get Force Exclude Expense.
        /// </summary>
        /// <param name="selectedval"></param>
        /// <returns></returns>
        public ActionResult ForceExcludeExpense(int selectedval)
        {
            try
            {
                //This class is Update Expense check force expense details
                _ExpenseCheckSettingRepository.UpdateExpenseCheckDetail(true, selectedval);
                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsController - ForceExcludeExpense - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("Error", JsonRequestBehavior.AllowGet);

            }
        }

        /// <summary>
        /// This method is get ExcludedExpenseListDocument.
        /// </summary>
        /// <param name="ExpenseCheckID"></param>
        /// <returns></returns>
        public ActionResult ExcludedExpenseListDocument(int ExpenseCheckID)
        {
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            //This  class is Get user id.
            var UserId = _CommonRepository.getUserId(UserName);
            //This  class is Get usertype id.
            var UserTypeId = _CommonRepository.getUserTypeId(UserName);
            //This class is Get Expense Check Documents.
            var lstExpense = _ExpenseCheckSettingRepository.GETExpenseCheckDocuments(ExpenseCheckID).Select(s => new { s.DocumentName, s.CreatedBy, s.ExpenseCheckDocumentId }).ToList();
            return Json(new { list = lstExpense, id = UserId, type = UserTypeId }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is delete the Expense documents.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Delete(int Id = 0)
        {
            try
            {
                string DocumentName = "";
                //Delete Expense check Documents.
                DocumentName = _ExpenseCheckSettingRepository.DeleteexpenseCheckDocuments(Id);
                var path1 = Request.PhysicalApplicationPath + "UserFiles\\ExpenseDocuments" + "\\" + DocumentName;
                //string filePaths = Directory.GetFiles(path1, Data.DocumentName);
                System.IO.File.Delete(path1);
                ActivityLog ActLog = new ActivityLog();
                ActLog.Action = 3;
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                //This Db class is used for get user firstname.
                ActLog.Comment = "Expense Check Document " + DocumentName + " Deleted by " + _CommonRepository.getUserFirstName(UserName) + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                //This  class is used for Activity Log Insert.
                _ActivityLogRepository.ActivityLogInsert(ActLog);
                //ViewBag.DeleteMessage = "Payroll data deleted successfully.";
                ViewBag.DeleteMessage = _CommonRepository.GetMessageValue("ECSD", "Payroll data deleted successfully.");
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsController - Delete - " + DateTime.Now + " - " + ex.Message.ToString());

            }
            return null;
        }


        /// <summary>
        /// This method is get Expense view data.
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetExpenseViewData(int Type)
        {
            ExpenseCheck_Setting obj = new ExpenseCheck_Setting();
            int storeid = 0;
            try
            {
                if (Convert.ToString(Session["storeid"]) != "0" && Convert.ToString(Session["storeid"]) != "")
                {
                    sType = Type;
                    storeid = Convert.ToInt32(Session["storeid"]);
                    // This class is Expense Check_SettingList.
                    obj.ExpenseCheck_SettingList = _ExpenseCheckSettingRepository.ExpenseCheck_SettingList(storeid, Type);
                    ViewBag.StoreIDValue = Session["storeid"];
                }
                else
                {
                    if (Session["storeid"] == null)
                        Session["storeid"] = 0;
                    ViewBag.StoreIDValue = Session["storeid"];
                }
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsController - GetExpenseViewData - " + DateTime.Now + " - " + ex.Message.ToString());

            }

            return PartialView("_ExpenseCheckSettingView", obj);
        }

        /// <summary>
        /// This method is get Expense check index.
        /// </summary>
        /// <param name="MSG"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,AllExpenseCheck,IncludeExpenseCheck,ExcludeExpenseCheck")]
        public async Task<ActionResult> ExpenseCheckIndexNew(string MSG)
        {
            ViewBag.Title = "Expenses/Checks - Synthesis";

            ExpenseCheck_Setting obj = new ExpenseCheck_Setting();
            ViewBag.checksetting = obj;
            Session["SessionTake"] = 0;

            if (StatusMessage == "Success1" || StatusMessage == "Success" || StatusMessage == "Delete" || StatusMessage == "Success Expense" || StatusMessage == "Success Check")
            {
                ViewBag.StatusMessage = StatusMessage;
                //StatusMessage = "";
            }
            else
            {
                ViewBag.StatusMessage = "";
            }
            return View();
        }

        /// <summary>
        /// This method is return the Url Data Source Expense.
        /// </summary>
        /// <param name="dm"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        public ActionResult UrlDatasourceExpense(DataManagerRequest dm, string Status, string Startdate = null, string Enddate = null)
        {
            List<Invoice> BindData1 = new List<Invoice>();
            List<ExpenseCheckSelect> BindData = new List<ExpenseCheckSelect>();
            IEnumerable DataSource = new List<ExpenseCheckSelect>();
            DataOperations operation = new DataOperations();

            if (Startdate == "")
            {
                Startdate = null;
            }
            if (Enddate == "")
            {
                Enddate = null;
            }
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            //This  class is Get usertype id.
            var UserTypeId = _CommonRepository.getUserTypeId(UserName);
            string payment = "";
            int count = 0;
            try
            {
                if (Session["payment"] != null)
                {
                    payment = Session["payment"].ToString();
                }
                var StoreId = Convert.ToInt32(Session["storeid"]);

                var strStore = "";
                strStore = GetExpenseCheck_StoreList(StoreId);
                if (StoreId == 0 && strStore.Length > 0)
                {
                    var S = (strStore.Split(',').Count() == 1 ? Convert.ToInt32(strStore.Split(',')[0]) : 0);
                    if (S > 0)
                    {
                        Session["storeid"] = S;
                        StoreId = S;
                    }
                }

                if (Status == null || Status == "All")
                {
                    Status = "";
                }
                string ForceStatus = "";
                if (Status == "" && !Roles.IsUserInRole("Administrator"))
                {
                    Boolean ischeck = false;
                    if (Roles.IsUserInRole("IncludeExpenseCheck"))
                    {
                        ischeck = true;
                        Status = "Include";
                        ForceStatus = "WithForceInclude";
                    }
                    if (Roles.IsUserInRole("ExcludeExpenseCheck"))
                    {
                        if (ischeck == true)
                            Status = "";
                        else
                        {
                            Status = "Exclude";
                            ForceStatus = "WithForceExclude";
                        }
                    }
                }
                if (Status == "Override" && !Roles.IsUserInRole("Administrator"))
                {
                    if (Roles.IsUserInRole("IncludeExpenseCheck"))
                    {
                        Status = "OverrideInclude";
                    }
                    if (Roles.IsUserInRole("ExcludeExpenseCheck"))
                    {
                        Status = "OverrideExclude";
                    }
                }

                //This  class is Get usertype id.
                //Get Expense Check previes data.
                BindData = _ExpenseCheckSettingRepository.ExpenseCheckPreviewData_Beta(payment, "TXnDate", "Desc", Status, ForceStatus, _CommonRepository.getUserTypeId(UserName), strStore, Startdate, Enddate);
                BindData = BindData.Select(s => new ExpenseCheckSelect
                {
                    VendorName = s.VendorName,
                    Type = s.Type,
                    InvoiceDate = s.InvoiceDate,
                    ModifyDate = s.ModifyDate,
                    CreateOn = s.CreateOn,
                    StoreName = s.StoreName,
                    Amount = s.Amount,
                    InvoiceId = s.InvoiceId,
                    Department = s.Department,
                    Memo = s.Memo,
                    DocNumber = s.DocNumber,
                    ExpenseCheckDetailId = s.ExpenseCheckDetailId,
                    ExpenseCheckId = s.ExpenseCheckId,
                    ViewDocFlg = s.ViewDocFlg,
                    QBType = s.QBType,
                    DisplayExpenseStatus = s.DisplayExpenseStatus,
                    ExpenseStatus = s.ExpenseStatus,
                    AmountString = s.Amount.ToString("#,##0.00"),
                    PrintLater = s.PrintLater,
                    IsSync = s.IsSync,
                    QBErrorMessage = s.QBErrorMessage,
                    PaymentAccountName = s.PaymentAccountName,
                }).ToList();
                DataSource = BindData;

                count = DataSource.Cast<ExpenseCheckSelect>().Count();
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    string search = dm.Search[0].Key.ToString().Trim().ToLower();
                    DataSource = BindData.ToList().Where(x => (x.VendorName == null ? "" : x.VendorName.ToLower()).ToString().Contains(search) || (x.Type == null ? "" : x.Type.ToLower()).ToString().Contains(search) || (x.DocNumber == null ? "" : x.DocNumber).ToString().Contains(search) || (x.StoreName == null ? "" : x.StoreName.ToLower()).ToString().Contains(search)
                   || (x.Amount == null ? 0 : x.Amount).ToString().Contains(search) || (x.Department == null ? "" : x.Department.ToLower()).ToString().Contains(search) || (x.Memo == null ? "" : x.Memo.ToLower()).ToString().Contains(search) || Convert.ToDateTime(x.InvoiceDate).ToString("MM-dd-yyyy").Equals(search)).ToList();
                }
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                }

                //int count = totalRows;
                if (dm.Skip != 0)
                {
                    DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
                }
                if (Session["SessionTake"].ToString() == "1")
                {
                    dm.Take = 150;
                }
                if (dm.Take != 0)
                {
                    DataSource = operation.PerformTake(DataSource, dm.Take);
                }

                //Session.Remove("SessionTake");
                Session["SessionTake"] = 0;

                //TotalDataCount = DataSource.OfType<ExpenseCheckSelect>().ToList().Count();
                // ViewBag.Invoicecount = TotalDataCount;
                if (TotalDataCount == 0)
                {
                    StatusMessage = "NoItem";
                }
                if (StatusMessage == null) { StatusMessage = ""; }
                if (StatusMessage == "")
                {
                    ViewBag.StatusMessage = "";
                }
                else
                {
                    ViewBag.StatusMessage = StatusMessage;
                }
                ViewBag.TotalDataCount = TotalDataCount;

                StatusMessage = "";
                int StoreID = 0;
                if (Convert.ToString(Session["storeid"]) != "0")
                {
                    StoreID = Convert.ToInt32(Session["storeid"]);
                }
                //Get Expense Check details.
                var ExpenseDeptIds = _ExpenseCheckSettingRepository.GetExpenseCheckDetails();
                //Check User type.
                if (_ExpenseCheckSettingRepository.CheckUserTypeMasters(UserTypeId))
                {
                    //Get Expense Department Ids
                    ExpenseDeptIds = _ExpenseCheckSettingRepository.GetExpenseDeptIds(UserTypeId, StoreID);
                }
                //Get All Departments
                ViewBag.DrpLstdept = _ExpenseCheckSettingRepository.GetDepartmentMasters(ExpenseDeptIds, StoreID);
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsController - UrlDatasourceExpense - " + DateTime.Now + " - " + ex.Message.ToString());

            }

            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }


        /// <summary>
        /// This method is the Expense CheckIndexGrid New.
        /// </summary>
        /// <returns></returns>
        public ActionResult ExpenseCheckIndexGridNew()
        {
            ViewBag.StatusMessage = "";
            ViewBag.TotalDataCount = TotalDataCount;
            ViewBag.IsBindData = null; ;
            ViewBag.CurrentPageIndex = null;
            ViewBag.LastPageIndex = null;
            ViewBag.OrderByVal = null;
            ViewBag.IsAscVal = 1;
            ViewBag.PageSize = null;
            ViewBag.Alpha = "";
            ViewBag.SearchRecords = 0;
            ViewBag.startindex = null;
            ViewBag.SearchFlg = null;
            ViewBag.searchdashbord = null;
            ViewBag.AscVal = 0;
            ViewBag.IsFilter = "0";
            ViewBag.endIndex = 0;
            ViewBag.DocSaved = _CommonRepository.GetMessageValue("ECDS", "Document Save Successfully.");
            ViewBag.ExcExpense = _CommonRepository.GetMessageValue("ECEES", "Expense Excluded Successfully.");

            var StoreIdSsn = Session["storeid"];
            if (StoreIdSsn != null)
            {
                if (!string.IsNullOrEmpty(Session["storeid"].ToString()))
                {
                    string store_idval = Session["storeid"].ToString();
                    ViewBag.Storeidvalue = store_idval;
                }
            }

            if (StatusMessage == "Success Expense" || StatusMessage == "Error Expense" || StatusMessage == "Success Check" || StatusMessage == "Error Check")
            {
                ViewBag.StatusMessage = StatusMessage;
                StatusMessage = "";
            }
            else
            {
                ViewBag.StatusMessage = "";
            }
            return View();
        }

        /// <summary>
        /// This method is the Expense Check Grid New.
        /// </summary>
        /// <param name="radio"></param>
        /// <param name="txtstartdate"></param>
        /// <param name="txtenddate"></param>
        /// <param name="ECLstStore"></param>
        /// <param name="AmtMinimum"></param>
        /// <param name="AmtMaximum"></param>
        /// <param name="DrpLstdept"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExpenseCheckIndexGridNew(string[] radio, string txtstartdate, string txtenddate, string ECLstStore, string AmtMinimum, string AmtMaximum, string DrpLstdept) //, string DrpLstVendor
        {


            IsFilter = "0";
            string paymethod = "";
            if (radio != null)
            {
                IsFilter = "1";
                if (radio.Count() > 1)
                {
                    paymethod = "";
                }
                else if (radio.Count() == 1)
                {
                    paymethod = radio[0];
                }
            }
            else
            {
                paymethod = "";
                IsFilter = "0";
            }

            Session["ECLstStore"] = ECLstStore;
            Session["ECstartdate"] = txtstartdate;
            Session["ECenddate"] = txtenddate;
            Session["ECAmtMinimum"] = AmtMinimum;
            Session["ECAmtMaximum"] = AmtMaximum;
            Session["payment"] = paymethod;
            Session["deptname"] = DrpLstdept;
            //Session["DrpLstVendor"] = DrpLstVendor;
            return RedirectToAction("ExpenseCheckIndex", "ExpenseCheckSetting", new { IsBindData = 1, currentPageIndex = 1, orderby = "InvoiceId", IsAsc = 0, PageSize = 50, SearchRecords = 1, Alpha = "", storename = ECLstStore, startdate = txtstartdate, enddate = txtenddate, searchdashbord = "", SearchFlg = "F" });
        }

        public ActionResult ExcelExport(string gridModel, string startdate, string enddate, string Status)
        {
            List<ExpenseCheckSelect> BindData = new List<ExpenseCheckSelect>();
            GridExcelExport exp = new GridExcelExport();

            try
            {
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                //This  class is Get usertype id.
                var UserTypeId = _CommonRepository.getUserTypeId(UserName);
                string payment = "";

                if (Session["payment"] != null)
                {
                    payment = Session["payment"].ToString();
                }
                var StoreId = Convert.ToInt32(Session["storeid"]);

                var strStore = "";
                strStore = GetExpenseCheck_StoreList(StoreId);
                if (StoreId == 0 && strStore.Length > 0)
                {
                    var S = (strStore.Split(',').Count() == 1 ? Convert.ToInt32(strStore.Split(',')[0]) : 0);
                    if (S > 0)
                    {
                        Session["storeid"] = S;
                        StoreId = S;
                    }
                }

                if (Status == null || Status == "All")
                {
                    Status = "";
                }
                string ForceStatus = "";
                if (Status == "" && !Roles.IsUserInRole("Administrator"))
                {
                    Boolean ischeck = false;
                    if (Roles.IsUserInRole("IncludeExpenseCheck"))
                    {
                        ischeck = true;
                        Status = "Include";
                        ForceStatus = "WithForceInclude";
                    }
                    if (Roles.IsUserInRole("ExcludeExpenseCheck"))
                    {
                        if (ischeck == true)
                            Status = "";
                        else
                        {
                            Status = "Exclude";
                            ForceStatus = "WithForceExclude";
                        }
                    }
                }
                if (Status == "Override" && !Roles.IsUserInRole("Administrator"))
                {
                    if (Roles.IsUserInRole("IncludeExpenseCheck"))
                    {
                        Status = "OverrideInclude";
                    }
                    if (Roles.IsUserInRole("ExcludeExpenseCheck"))
                    {
                        Status = "OverrideExclude";
                    }
                }
                if (string.IsNullOrEmpty(startdate))
                {
                    startdate = null;
                }
                if (string.IsNullOrEmpty(enddate))
                {
                    enddate = null;
                }

                //This  class is Get usertype id.
                //Get Expense Check previes data.
                BindData = _ExpenseCheckSettingRepository.ExpenseCheckPreviewData_Beta(payment, "TXnDate", "Desc", Status, ForceStatus, _CommonRepository.getUserTypeId(UserName), strStore, startdate, enddate);

                exp.FileName = "Expenses.xlsx";
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsController - ExcelExport - " + DateTime.Now + " - " + ex.Message.ToString());
            }


            Syncfusion.EJ2.Grids.Grid gridProperty = ConvertGridObject(gridModel, BindData);
            return exp.ExcelExport<ExpenseCheckSelect>(gridProperty, BindData);

        }

        private Syncfusion.EJ2.Grids.Grid ConvertGridObject(string gridProperty, List<ExpenseCheckSelect> data)
        {
            Syncfusion.EJ2.Grids.Grid GridModel = (Syncfusion.EJ2.Grids.Grid)Newtonsoft.Json.JsonConvert.DeserializeObject(gridProperty, typeof(Syncfusion.EJ2.Grids.Grid));

            try
            {
                GridColumnModel cols = (GridColumnModel)Newtonsoft.Json.JsonConvert.DeserializeObject("{\"allowGrouping\":false,\"allowTextWrap\":true,\"allowPaging\":false,\"pageSettings\":{\"currentPage\":1,\"pageCount\":2,\"pageSize\":100},\"sortSettings\":{},\"allowPdfExport\":true,\"allowExcelExport\":true,\"aggregates\":[],\"filterSettings\":{\"immediateModeDelay\":1500,\"type\":\"CheckBox\"},\"groupSettings\":{\"columns\":[],\"enableLazyLoading\":false,\"disablePageWiseAggregates\":false},\"columns\":[],\"locale\":\"en-US\",\"searchSettings\":{\"key\":\"\",\"fields\":[]}}", typeof(GridColumnModel));
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "VendorName", HeaderText = "Vendor Name" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "StoreName", HeaderText = "Store Name" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "Type", HeaderText = "Type" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "InvoiceDate", HeaderText = "Invoice Date" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "DocNumber", HeaderText = "No #" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "Amount", HeaderText = "Total Amount", });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "Department", HeaderText = "Department" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "Memo", HeaderText = "Memo" });
                cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "DisplayExpenseStatus", HeaderText = "Status" });


                foreach (var item in cols.columns)
                {
                    item.AutoFit = true;
                    item.Width = "10%";
                }

                foreach (var item in data)
                {
                    foreach (var col in cols.columns)
                    {
                        if (col.Field == "VendorName")
                        {
                            int maxLineLength = 20;

                            if (!string.IsNullOrEmpty(item.VendorName))
                            {
                                StringBuilder memoBuilder = new StringBuilder();
                                for (int i = 0; i < item.VendorName.Length; i += maxLineLength)
                                {
                                    int length = Math.Min(maxLineLength, item.VendorName.Length - i);
                                    memoBuilder.AppendLine(item.VendorName.Substring(i, length));
                                }
                                item.VendorName = memoBuilder.ToString().Trim();
                            }
                        }
                        else if (col.Field == "Department")
                        {
                            int maxLineLength = 25;

                            if (!string.IsNullOrEmpty(item.Department))
                            {
                                StringBuilder memoBuilder = new StringBuilder();
                                for (int i = 0; i < item.Department.Length; i += maxLineLength)
                                {
                                    int length = Math.Min(maxLineLength, item.Department.Length - i);
                                    memoBuilder.AppendLine(item.Department.Substring(i, length));
                                }
                                item.Department = memoBuilder.ToString().Trim();
                            }
                        }
                        else if (col.Field == "Memo")
                        {
                            int maxLineLength = 40;

                            if (!string.IsNullOrEmpty(item.Memo))
                            {
                                StringBuilder memoBuilder = new StringBuilder();
                                for (int i = 0; i < item.Memo.Length; i += maxLineLength)
                                {
                                    int length = Math.Min(maxLineLength, item.Memo.Length - i);
                                    memoBuilder.AppendLine(item.Memo.Substring(i, length));
                                }
                                item.Memo = memoBuilder.ToString().Trim();
                            }
                        }
                    }
                }

                GridModel.Columns = cols.columns;
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsController - ConvertGridObject - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return GridModel;
        }

        public ActionResult PdfExport(string gridModel, string startdate, string enddate, string Status)
        {
            List<ExpenseCheckSelect> BindData = new List<ExpenseCheckSelect>();
            GridPdfExport exp = new GridPdfExport();

            try
            {
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                //This  class is Get usertype id.
                var UserTypeId = _CommonRepository.getUserTypeId(UserName);
                string payment = "";

                if (Session["payment"] != null)
                {
                    payment = Session["payment"].ToString();
                }
                var StoreId = Convert.ToInt32(Session["storeid"]);

                var strStore = "";
                strStore = GetExpenseCheck_StoreList(StoreId);
                if (StoreId == 0 && strStore.Length > 0)
                {
                    var S = (strStore.Split(',').Count() == 1 ? Convert.ToInt32(strStore.Split(',')[0]) : 0);
                    if (S > 0)
                    {
                        Session["storeid"] = S;
                        StoreId = S;
                    }
                }

                if (Status == null || Status == "All")
                {
                    Status = "";
                }
                string ForceStatus = "";
                if (Status == "" && !Roles.IsUserInRole("Administrator"))
                {
                    Boolean ischeck = false;
                    if (Roles.IsUserInRole("IncludeExpenseCheck"))
                    {
                        ischeck = true;
                        Status = "Include";
                        ForceStatus = "WithForceInclude";
                    }
                    if (Roles.IsUserInRole("ExcludeExpenseCheck"))
                    {
                        if (ischeck == true)
                            Status = "";
                        else
                        {
                            Status = "Exclude";
                            ForceStatus = "WithForceExclude";
                        }
                    }
                }
                if (Status == "Override" && !Roles.IsUserInRole("Administrator"))
                {
                    if (Roles.IsUserInRole("IncludeExpenseCheck"))
                    {
                        Status = "OverrideInclude";
                    }
                    if (Roles.IsUserInRole("ExcludeExpenseCheck"))
                    {
                        Status = "OverrideExclude";
                    }
                }
                if (string.IsNullOrEmpty(startdate))
                {
                    startdate = null;
                }
                if (string.IsNullOrEmpty(enddate))
                {
                    enddate = null;
                }

                //This  class is Get usertype id.
                //Get Expense Check previes data.
                BindData = _ExpenseCheckSettingRepository.ExpenseCheckPreviewData_Beta(payment, "TXnDate", "Desc", Status, ForceStatus, _CommonRepository.getUserTypeId(UserName), strStore, startdate, enddate);

                PdfDocument doc = new PdfDocument();
                doc.PageSettings.Orientation = PdfPageOrientation.Landscape;
                doc.PageSettings.Size = PdfPageSize.A3;

                PdfTextElement element = new PdfTextElement();

                PdfStringFormat format = new PdfStringFormat
                {
                    WordWrap = PdfWordWrapType.Word
                };
                element.StringFormat = format;
                exp.Theme = "flat-saffron";
                exp.FileName = "Expense.pdf";
                exp.PdfDocument = doc;
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsController - PdfExport - " + DateTime.Now + " - " + ex.Message.ToString());
            }


            Syncfusion.EJ2.Grids.Grid gridProperty = ConvertGridObject(gridModel, BindData);
            return exp.PdfExport<ExpenseCheckSelect>(gridProperty, BindData);
        }

        //syncfusion code start from here(Himanshu 03-12-2024)

        public ActionResult ExpenseIndex()
        {
            ViewBag.Title = "ExpenseList - Synthesis";
            var StoreIdSsn = Session["storeid"];
            if (StoreIdSsn != null)
            {
                if (!string.IsNullOrEmpty(Session["storeid"].ToString()))
                {
                    string store_idval = Session["storeid"].ToString();
                    ViewBag.Storeidvalue = store_idval;
                }
            }
            return View();
        }

        public ActionResult UrlDatasourceExpenseCheck(DataManagerRequest dm, string Status, string Startdate = null, string Enddate = null)
        {
            List<ExpenseCheckSelect> BindData = new List<ExpenseCheckSelect>();
            IEnumerable DataSource = new List<ExpenseCheckSelect>();
            DataOperations operation = new DataOperations();

            if (Startdate == "")
            {
                Startdate = null;
            }
            if (Enddate == "")
            {
                Enddate = null;
            }
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            //This  class is Get usertype id.
            var UserTypeId = _CommonRepository.getUserTypeId(UserName);
            string payment = "";
            int count = 0;
            try
            {
                if (Session["payment"] != null)
                {
                    payment = Session["payment"].ToString();
                }
                var StoreId = Convert.ToInt32(Session["storeid"]);

                var strStore = "";
                strStore = GetExpenseCheck_StoreList(StoreId);
                if (StoreId == 0 && strStore.Length > 0)
                {
                    var S = (strStore.Split(',').Count() == 1 ? Convert.ToInt32(strStore.Split(',')[0]) : 0);
                    if (S > 0)
                    {
                        Session["storeid"] = S;
                        StoreId = S;
                    }
                }

                if (Status == null || Status == "All")
                {
                    Status = "";
                }
                string ForceStatus = "";
                if (Status == "" && !Roles.IsUserInRole("Administrator"))
                {
                    Boolean ischeck = false;
                    if (Roles.IsUserInRole("IncludeExpenseCheck"))
                    {
                        ischeck = true;
                        Status = "Include";
                        ForceStatus = "WithForceInclude";
                    }
                    if (Roles.IsUserInRole("ExcludeExpenseCheck"))
                    {
                        if (ischeck == true)
                            Status = "";
                        else
                        {
                            Status = "Exclude";
                            ForceStatus = "WithForceExclude";
                        }
                    }
                }
                if (Status == "Override" && !Roles.IsUserInRole("Administrator"))
                {
                    if (Roles.IsUserInRole("IncludeExpenseCheck"))
                    {
                        Status = "OverrideInclude";
                    }
                    if (Roles.IsUserInRole("ExcludeExpenseCheck"))
                    {
                        Status = "OverrideExclude";
                    }
                }

                //This  class is Get usertype id.
                //Get Expense Check previes data.
                BindData = _ExpenseCheckSettingRepository.ExpenseCheckPreviewData_Beta(payment, "CreateTime", "Desc", Status, ForceStatus, _CommonRepository.getUserTypeId(UserName), strStore, Startdate, Enddate);
                BindData = BindData.Select(s => new ExpenseCheckSelect
                {
                    VendorName = s.VendorName,
                    Type = s.Type,
                    InvoiceDate = s.InvoiceDate,
                    ModifyDate = s.ModifyDate,
                    CreateOn = s.CreateOn,
                    StoreName = s.StoreName,
                    Amount = s.Amount,
                    InvoiceId = s.InvoiceId,
                    Department = s.Department,
                    Memo = s.Memo,
                    DocNumber = s.DocNumber,
                    ExpenseCheckDetailId = s.ExpenseCheckDetailId,
                    ExpenseCheckId = s.ExpenseCheckId,
                    ViewDocFlg = s.ViewDocFlg,
                    QBType = s.QBType,
                    DisplayExpenseStatus = s.DisplayExpenseStatus,
                    ExpenseStatus = s.ExpenseStatus,
                    AmountString = s.Amount.ToString("#,##0.00"),
                    PaymentAccountName = s.PaymentAccountName,
                }).ToList();
                DataSource = BindData;

                count = DataSource.Cast<ExpenseCheckSelect>().Count();
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    string search = dm.Search[0].Key.ToString().Trim().ToLower();
                    DataSource = BindData.ToList().Where(x => (x.VendorName == null ? "" : x.VendorName.ToLower()).ToString().Contains(search) || (x.Type == null ? "" : x.Type.ToLower()).ToString().Contains(search) || (x.DocNumber == null ? "" : x.DocNumber).ToString().Contains(search) || (x.StoreName == null ? "" : x.StoreName.ToLower()).ToString().Contains(search)
                   || (x.Amount == null ? 0 : x.Amount).ToString().Contains(search) || (x.Department == null ? "" : x.Department.ToLower()).ToString().Contains(search) || (x.Memo == null ? "" : x.Memo.ToLower()).ToString().Contains(search) || Convert.ToDateTime(x.InvoiceDate).ToString("MM-dd-yyyy").Equals(search)).ToList();
                }
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                }

                //int count = totalRows;
                if (dm.Skip != 0)
                {
                    DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
                }
                if (Session["SessionTake"].ToString() == "1")
                {
                    dm.Take = 150;
                }
                if (dm.Take != 0)
                {
                    DataSource = operation.PerformTake(DataSource, dm.Take);
                }

                //Session.Remove("SessionTake");
                Session["SessionTake"] = 0;

                //TotalDataCount = DataSource.OfType<ExpenseCheckSelect>().ToList().Count();
                // ViewBag.Invoicecount = TotalDataCount;
                if (TotalDataCount == 0)
                {
                    StatusMessage = "NoItem";
                }
                if (StatusMessage == null) { StatusMessage = ""; }
                if (StatusMessage == "")
                {
                    ViewBag.StatusMessage = "";
                }
                else
                {
                    ViewBag.StatusMessage = StatusMessage;
                }
                ViewBag.TotalDataCount = TotalDataCount;

                StatusMessage = "";
                int StoreID = 0;
                if (Convert.ToString(Session["storeid"]) != "0")
                {
                    StoreID = Convert.ToInt32(Session["storeid"]);
                }
                //Get Expense Check details.
                var ExpenseDeptIds = _ExpenseCheckSettingRepository.GetExpenseCheckDetails();
                //Check User type.
                if (_ExpenseCheckSettingRepository.CheckUserTypeMasters(UserTypeId))
                {
                    //Get Expense Department Ids
                    ExpenseDeptIds = _ExpenseCheckSettingRepository.GetExpenseDeptIds(UserTypeId, StoreID);
                }
                //Get All Departments
                ViewBag.DrpLstdept = _ExpenseCheckSettingRepository.GetDepartmentMasters(ExpenseDeptIds, StoreID);
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsController - UrlDatasourceExpenseCheck - " + DateTime.Now + " - " + ex.Message.ToString());

            }

            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }

        public ActionResult AddPartial()
        {
            ExpenseCheck expensevalue = new ExpenseCheck();
            try
            {
                logger.Info("ExpenseAccountsController - AddPartial - " + DateTime.Now);
                int StoreId = 0;
                StoreId = Convert.ToInt32(Session["storeid"]);

                var vendors = _MastersBindRepository.GetVendorMaster(StoreId).Select(s => new DropdownViewModelExpense { Text = s.VendorName + " (Vendor)", Value = s.VendorId.ToString() + "-V" }).OrderBy(o => o.Text).ToList();
                var employees = _MastersBindRepository.GetEmployeeMaster(StoreId).Select(s => new DropdownViewModelExpense { Text = s.DisplayName + " (Employee)", Value = s.EmployeeId.ToString() + "-E" }).OrderBy(o => o.Text).ToList();

                // Combine Vendors and Employees
                ViewBag.PayeeName = vendors.Concat(employees).ToList();

                //ViewBag.AccountName = _departmentMastersRepository.AccountTypeList().Where(s => s.Flag == "O" && (s.AccountType == "Bank" || s.AccountType == "Credit Card" || s.AccountType == "Other Current Asset")).Select(s => new DropdownViewModel { Text = s.AccountType, Value = s.AccountTypeId }).OrderBy(o => o.Text).ToList();
                ViewBag.AccountName = _ExpenseCheckSettingRepository.DepartmentAccountTypeList(StoreId).Select(s => new DropdownViewModel { Text = s.DepartmentName, Value = s.DepartmentId }).OrderBy(o => o.Text).ToList();
                //ViewBag.PaymentName = _ExpenseCheckSettingRepository.PaymentMethodList().Select(s => new DropdownViewModel { Text = s.PaymentMethod, Value = s.PaymentMethodId }).OrderBy(o => o.Text).ToList();
                ViewBag.PaymentName = Enum.GetValues(typeof(PaymentType)).Cast<PaymentType>().Select(e => new DropdownViewModel { Text = e.ToString(), Value = (int)e }).OrderBy(o => o.Text).ToList();
                ViewBag.DepartmentName = _ExpenseCheckSettingRepository.DepartmentDetailList(StoreId).Select(s => new DropdownViewModel { Text = s.DepartmentName, Value = s.DepartmentId }).OrderBy(o => o.Text).ToList();
                ViewBag.PaymentMethodName = _ExpenseCheckSettingRepository.PaymentMethodList().Where(s => s.IsActive == true && s.StoreId == StoreId).Select(s => new DropdownViewModel { Text = s.PaymentMethod, Value = s.PaymentMethodId }).OrderBy(o => o.Text).ToList();
                ViewBag.PayeeName.Insert(0, new DropdownViewModelExpense { Text = "-Select Payee-", Value = "" });
                ViewBag.AccountName.Insert(0, new DropdownViewModel { Text = "-Select Payment Account-", Value = 0 });
                ViewBag.PaymentName.Insert(0, new DropdownViewModel { Text = "-Select Payment Type-", Value = 0 });
                ViewBag.PaymentMethodName.Insert(0, new DropdownViewModel { Text = "-Select Payment Method-", Value = 0 });
                expensevalue.PrintLater = true;
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsController - AddPartial - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView("_DialogExpenseAddpartial", expensevalue);
        }

        public ActionResult EditPartial(ExpenseCheck value)
        {
            ExpenseCheck expensevalue = new ExpenseCheck();
            try
            {
                logger.Info("ExpenseAccountsController - EditPartial - " + DateTime.Now);
                int StoreId = 0;
                StoreId = Convert.ToInt32(Session["storeid"]);

                expensevalue = _ExpenseCheckSettingRepository.GetExpenseCheckById(value.ExpenseCheckId);
                expensevalue.PaymentTypeId = expensevalue.PaymentType == PaymentType.Check ? 1 : expensevalue.PaymentType == PaymentType.Expense ? 2 : (int?)null;
                var expensedetailvalue = _ExpenseCheckSettingRepository.GetExpenseCheckDetailsById(value.ExpenseCheckId);

                List<ExpenseCheckDetail> expdetaillist = new List<ExpenseCheckDetail>();
                foreach (var item in expensedetailvalue)
                {
                    ExpenseCheckDetail expdetails = new ExpenseCheckDetail();
                    expdetails.ExpenseCheckDetailId = item.ExpenseCheckDetailId;
                    expdetails.ExpenseCheckId = item.ExpenseCheckId;
                    expdetails.DepartmentId = item.DepartmentId;
                    expdetails.Amount = item.Amount;
                    expdetails.Description = item.Description;
                    expdetaillist.Add(expdetails);
                }
                expensevalue.ExpenseCheckDetails = expdetaillist;

                var vendors = _MastersBindRepository.GetVendorMaster(expensevalue.StoreId.Value).Select(s => new DropdownViewModelExpense { Text = s.VendorName + " (Vendor)", Value = s.VendorId.ToString() + "-V" }).OrderBy(o => o.Text).ToList();
                var employees = _MastersBindRepository.GetEmployeeMaster(expensevalue.StoreId.Value).Select(s => new DropdownViewModelExpense { Text = s.DisplayName + " (Employee)", Value = s.EmployeeId.ToString() + "-E" }).OrderBy(o => o.Text).ToList();

                // Combine Vendors and Employees
                ViewBag.PayeeName = vendors.Concat(employees).ToList();

                expensevalue.VendorIdstr = expensevalue.VendorId.ToString() + "-" + expensevalue.RefType;

                //ViewBag.PayeeName = _MastersBindRepository.GetVendorMaster(expensevalue.StoreId.Value).Select(s => new DropdownViewModel { Text = s.VendorName, Value = s.VendorId }).OrderBy(o => o.Text).ToList();
                //ViewBag.AccountName = _departmentMastersRepository.AccountTypeList().Where(s => s.Flag == "O" && (s.AccountType == "Bank" || s.AccountType == "Credit Card" || s.AccountType == "Other Current Asset")).Select(s => new DropdownViewModel { Text = s.AccountType, Value = s.AccountTypeId }).OrderBy(o => o.Text).ToList();
                ViewBag.AccountName = _ExpenseCheckSettingRepository.DepartmentAccountTypeList(expensevalue.StoreId.Value).Select(s => new DropdownViewModel { Text = s.DepartmentName, Value = s.DepartmentId }).OrderBy(o => o.Text).ToList();
                //ViewBag.PaymentName = _ExpenseCheckSettingRepository.PaymentMethodList().Select(s => new DropdownViewModel { Text = s.PaymentMethod, Value = s.PaymentMethodId }).OrderBy(o => o.Text).ToList();
                ViewBag.PaymentName = Enum.GetValues(typeof(PaymentType)).Cast<PaymentType>().Select(e => new DropdownViewModel { Text = e.ToString(), Value = (int)e }).OrderBy(o => o.Text).ToList();
                ViewBag.DepartmentName = _ExpenseCheckSettingRepository.DepartmentDetailList(expensevalue.StoreId.Value).Select(s => new DropdownViewModel { Text = s.DepartmentName, Value = s.DepartmentId }).OrderBy(o => o.Text).ToList();
                ViewBag.PaymentMethodName = _ExpenseCheckSettingRepository.PaymentMethodList().Where(s => s.IsActive == true && s.StoreId == expensevalue.StoreId.Value).Select(s => new DropdownViewModel { Text = s.PaymentMethod, Value = s.PaymentMethodId }).OrderBy(o => o.Text).ToList();

                ViewBag.PayeeName.Insert(0, new DropdownViewModelExpense { Text = "-Select Payee-", Value = "" });
                ViewBag.AccountName.Insert(0, new DropdownViewModel { Text = "-Select Payment Account-", Value = 0 });
                ViewBag.PaymentName.Insert(0, new DropdownViewModel { Text = "-Select Payment Method-", Value = 0 });
                ViewBag.PaymentMethodName.Insert(0, new DropdownViewModel { Text = "-Select Payment Method-", Value = 0 });
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsController - EditPartial - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView("_DialogExpenseEditpartial", expensevalue);
        }

        public async Task<ActionResult> InsertExpenseCheck(CRUDModel<ExpenseCheck> expensevalue)
        {
            logger.Info("ExpenseAccountsController - InsertExpenseCheckData - " + JsonConvert.SerializeObject(expensevalue) + DateTime.Now);
            ExpenseCheck expensecheck = new ExpenseCheck();
            SuccessMessage = null;
            ErrorMessage = null;
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            try
            {
                Session["SessionTake"] = 1;
                logger.Info("ExpenseAccountsController - InsertExpenseCheck - " + DateTime.Now);
                int StoreId = 0;
                StoreId = Convert.ToInt32(Session["storeid"]);
                expensevalue.Value.CreatedBy = _CommonRepository.getUserId(UserName);
                expensevalue.Value.StoreId = StoreId;


                // Extract VendorId and RefType from VendorIdstr
                if (!string.IsNullOrEmpty(expensevalue.Value.VendorIdstr))
                {
                    if (expensevalue.Value.VendorIdstr.EndsWith("-V"))
                    {
                        expensevalue.Value.RefType = "V";
                        expensevalue.Value.VendorIdstr = expensevalue.Value.VendorIdstr.Replace("-V", "");
                    }
                    else if (expensevalue.Value.VendorIdstr.EndsWith("-E"))
                    {
                        expensevalue.Value.RefType = "E";
                        expensevalue.Value.VendorIdstr = expensevalue.Value.VendorIdstr.Replace("-E", "");
                    }

                    if (int.TryParse(expensevalue.Value.VendorIdstr, out int vendorId))
                    {
                        expensevalue.Value.VendorId = vendorId;
                    }
                }



                if (expensevalue.Value.PaymentTypeId == 0)
                {
                    expensevalue.Value.PaymentTypeId = 2;
                }
                if (expensevalue.Value.PaymentTypeId == 1)
                {
                    expensevalue.Value.PaymentMethodId = _ExpenseCheckSettingRepository.PaymentMethodList().Where(s => s.PaymentMethod == "Check" && s.StoreId == StoreId).Select(s => s.PaymentMethodId).FirstOrDefault();
                }
                if (expensevalue.Value.VendorId != 0 && expensevalue.Value.BankAccountId != 0)
                {
                    expensecheck = _ExpenseCheckSettingRepository.InsertExpenseData(expensevalue.Value);

                    if (Convert.ToInt32(expensecheck.ExpenseCheckId) > 0)
                    {
                        SynthesisQBOnline.BAL.QBResponse objResponse = new SynthesisQBOnline.BAL.QBResponse();
                        //This db class is QB sync Expense
                        bool QBSyncData = _qBRepository.QBSyncExpense(expensevalue.Value, expensevalue.Value.ExpenseCheckDetails, Convert.ToInt32(expensevalue.Value.StoreId), ref objResponse);
                        logger.Info("ExpenseAccountsController - InsertExpenseCheck return QBSyncData - " + QBSyncData + "ref Message objResponse" + objResponse + DateTime.Now);
                        if (QBSyncData)
                        {
                            logger.Info("ExpenseAccountsController - InsertExpenseCheck InsertExpenseCheckSyncId - " + QBSyncData + DateTime.Now);
                            _ExpenseCheckSettingRepository.InsertExpenseSyncId(expensecheck.ExpenseCheckId, objResponse);
                        }
                        else
                        {
                            logger.Info("ExpenseAccountsController - InsertExpenseCheck InsertExpenseCheckErrorSyncId - " + QBSyncData + DateTime.Now);
                            _ExpenseCheckSettingRepository.InsertExpenseErrorSyncId(expensevalue.Value.ExpenseCheckId, objResponse);
                        }
                    }

                    if (Convert.ToInt32(expensecheck.ExpenseCheckId) > 0)
                    {
                        foreach (var item in expensevalue.Value.ExpenseCheckDetails)
                        {
                            if (item.DepartmentId != null && item.Amount != null)
                            {
                                ExpenseCheckDetail expensedetails = new ExpenseCheckDetail();
                                expensedetails.ExpenseCheckId = expensecheck.ExpenseCheckId;
                                expensedetails.DepartmentId = item.DepartmentId;
                                expensedetails.Amount = item.Amount;
                                expensedetails.Description = item.Description;
                                _ExpenseCheckSettingRepository.InsertExpenseDetailsData(expensedetails);
                            }
                        }
                        if (!string.IsNullOrEmpty(expensevalue.Value.FileName))
                        {
                            var files = expensevalue.Value.FileName.Split(',').ToList();
                            foreach (var file in files)
                            {
                                if (!string.IsNullOrWhiteSpace(file))
                                {
                                    ExpenseCheckDocuments expensefile = new ExpenseCheckDocuments();
                                    expensefile.ExpenseCheckId = expensecheck.ExpenseCheckId;
                                    expensefile.DocumentName = file.Trim();
                                    expensefile.CreatedBy = _CommonRepository.getUserId(UserName);
                                    _ExpenseCheckSettingRepository.InsertExpenseFileData(expensefile);
                                }
                            }
                        }
                    }
                    SuccessMessage = "Expense Created Successfully.";
                }
                else
                {
                    ErrorMessage = "Payee/Payment Account/Payment Method Please Select this dropdown";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("ExpenseAccountsController - InsertExpenseCheck - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = expensecheck, success = SuccessMessage, Error = ErrorMessage });
        }
        public async Task<ActionResult> UpdateExpenseCheck(CRUDModel<ExpenseCheck> expensevalue)
        {
            logger.Info("ExpenseAccountsController - UpdateExpenseCheckData - " + JsonConvert.SerializeObject(expensevalue) + DateTime.Now);
            ExpenseCheck updateexpnese = new ExpenseCheck();
            SuccessMessage = null;
            ErrorMessage = null;
            try
            {
                logger.Info("ExpenseAccountsController - UpdateExpenseCheck - " + DateTime.Now);
                var existingExpenseCheck = _ExpenseCheckSettingRepository.GetExpenseCheckById(expensevalue.Value.ExpenseCheckId);

                // Extract VendorId and RefType from VendorIdstr
                if (!string.IsNullOrEmpty(expensevalue.Value.VendorIdstr))
                {
                    if (expensevalue.Value.VendorIdstr.EndsWith("-V"))
                    {
                        expensevalue.Value.RefType = "V";
                        expensevalue.Value.VendorIdstr = expensevalue.Value.VendorIdstr.Replace("-V", "");
                    }
                    else if (expensevalue.Value.VendorIdstr.EndsWith("-E"))
                    {
                        expensevalue.Value.RefType = "E";
                        expensevalue.Value.VendorIdstr = expensevalue.Value.VendorIdstr.Replace("-E", "");
                    }

                    if (int.TryParse(expensevalue.Value.VendorIdstr, out int vendorId))
                    {
                        expensevalue.Value.VendorId = vendorId;
                    }
                }

                if (expensevalue.Value.PaymentTypeId == 0)
                {
                    expensevalue.Value.PaymentTypeId = 2;
                }
                //if (expensevalue.Value.PaymentTypeId == 1)
                //{
                //    expensevalue.Value.QBType = "Check";
                //}
                if (expensevalue.Value.VendorId != 0 && expensevalue.Value.BankAccountId != 0)
                {
                    if (existingExpenseCheck != null)
                    {
                        // Update main table fields
                        existingExpenseCheck.VendorId = expensevalue.Value.VendorId;
                        existingExpenseCheck.BankAccountId = expensevalue.Value.BankAccountId;
                        existingExpenseCheck.PaymentTypeId = expensevalue.Value.PaymentTypeId;
                        existingExpenseCheck.TotalAmt = expensevalue.Value.TotalAmt;
                        existingExpenseCheck.TxnDate = expensevalue.Value.TxnDate;
                        existingExpenseCheck.DocNumber = expensevalue.Value.DocNumber;
                        existingExpenseCheck.Memo = expensevalue.Value.Memo;
                        existingExpenseCheck.PaymentMethodId = expensevalue.Value.PaymentMethodId;
                        if (expensevalue.Value.PaymentTypeId == 1)
                        {
                            existingExpenseCheck.PaymentMethodId = _ExpenseCheckSettingRepository.PaymentMethodList().Where(s => s.PaymentMethod == "Check" && s.StoreId == expensevalue.Value.StoreId).Select(s => s.PaymentMethodId).FirstOrDefault();
                            existingExpenseCheck.MailingAddress = expensevalue.Value.MailingAddress;
                            existingExpenseCheck.PrintLater = expensevalue.Value.PrintLater;
                        }
                        existingExpenseCheck.UpdatedBy = _CommonRepository.getUserId(System.Web.HttpContext.Current.User.Identity.Name);
                        existingExpenseCheck.RefType = expensevalue.Value.RefType;

                        // Get existing child rows from the database
                        var existingDetails = _ExpenseCheckSettingRepository.GetExpenseCheckDetailsById(expensevalue.Value.ExpenseCheckId);

                        // Loop through incoming data
                        foreach (var item in expensevalue.Value.ExpenseCheckDetails)
                        {
                            var existingDetail = existingDetails.FirstOrDefault(d => d.DepartmentId == item.DepartmentId);

                            if (existingDetail != null)
                            {
                                // Update existing row
                                existingDetail.DepartmentId = item.DepartmentId;
                                existingDetail.Amount = item.Amount;
                                existingDetail.Description = item.Description;
                                _ExpenseCheckSettingRepository.InsertExpenseDetailsData(existingDetail);
                            }
                            else
                            {
                                // Add new row
                                if (item.DepartmentId != null && item.Amount != null)
                                {
                                    ExpenseCheckDetail newDetail = new ExpenseCheckDetail
                                    {
                                        ExpenseCheckId = existingExpenseCheck.ExpenseCheckId,
                                        DepartmentId = item.DepartmentId,
                                        Amount = item.Amount,
                                        Description = item.Description
                                    };
                                    _ExpenseCheckSettingRepository.InsertExpenseDetailsData(newDetail);
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(expensevalue.Value.FileName))
                        {
                            var files = expensevalue.Value.FileName.Split(',').ToList();
                            foreach (var file in files)
                            {
                                if (!string.IsNullOrWhiteSpace(file))
                                {
                                    ExpenseCheckDocuments expensefile = new ExpenseCheckDocuments();
                                    expensefile.ExpenseCheckId = existingExpenseCheck.ExpenseCheckId;
                                    expensefile.DocumentName = file.Trim();
                                    expensefile.CreatedBy = _CommonRepository.getUserId(System.Web.HttpContext.Current.User.Identity.Name);
                                    _ExpenseCheckSettingRepository.InsertExpenseFileData(expensefile);
                                }
                            }
                        }

                        // Remove rows that are no longer present in the incoming data
                        var incomingDepartmentIds = expensevalue.Value.ExpenseCheckDetails.Select(d => d.DepartmentId).ToList();
                        var rowsToRemove = existingDetails.Where(d => !incomingDepartmentIds.Contains(d.DepartmentId)).ToList();

                        foreach (var row in rowsToRemove)
                        {
                            _ExpenseCheckSettingRepository.DeleteExpenseDetailsData(row.ExpenseCheckDetailId);
                        }

                        // Save changes to the main table
                        updateexpnese = _ExpenseCheckSettingRepository.InsertExpenseData(existingExpenseCheck);

                        SynthesisQBOnline.BAL.QBResponse objResponse = new SynthesisQBOnline.BAL.QBResponse();
                        //This db class is QB sync Expense
                        var latestexpesecheckdata = _ExpenseCheckSettingRepository.GetExpenseCheckById(expensevalue.Value.ExpenseCheckId);
                        if (latestexpesecheckdata.PaymentType == PaymentType.Check)
                        {
                            latestexpesecheckdata.PaymentTypeId = 1;
                        }
                        else
                        {
                            latestexpesecheckdata.PaymentTypeId = 2;
                        }
                        bool QBSyncData = false;
                        if (latestexpesecheckdata.IsSync)
                        {
                            QBSyncData = _qBRepository.QBEditSyncExpense(latestexpesecheckdata, expensevalue.Value.ExpenseCheckDetails, Convert.ToInt32(expensevalue.Value.StoreId), ref objResponse);
                            logger.Info("ExpenseAccountsController - UpdateExpenseCheck return QBSyncData - " + QBSyncData + "ref Message objResponse" + objResponse + DateTime.Now);
                            if (QBSyncData)
                            {
                                logger.Info("ExpenseAccountsController - UpdateExpenseCheck UpdateExpenseCheckSyncId - " + QBSyncData + DateTime.Now);
                                _ExpenseCheckSettingRepository.UpdateExpenseSyncId(expensevalue.Value.ExpenseCheckId, objResponse);
                            }
                            else
                            {
                                logger.Info("ExpenseAccountsController - UpdateExpenseCheck InsertExpenseCheckErrorSyncId - " + QBSyncData + DateTime.Now);
                                _ExpenseCheckSettingRepository.InsertExpenseErrorSyncId(expensevalue.Value.ExpenseCheckId, objResponse);
                            }
                        }
                        else
                        {
                            QBSyncData = _qBRepository.QBSyncExpense(latestexpesecheckdata, expensevalue.Value.ExpenseCheckDetails, Convert.ToInt32(expensevalue.Value.StoreId), ref objResponse);
                            logger.Info("ExpenseAccountsController - InsertExpenseCheck return QBSyncData - " + QBSyncData + "ref Message objResponse" + objResponse + DateTime.Now);
                            if (QBSyncData)
                            {
                                logger.Info("ExpenseAccountsController - InsertExpenseCheck InsertExpenseCheckSyncId - " + QBSyncData + DateTime.Now);
                                _ExpenseCheckSettingRepository.InsertExpenseSyncId(expensevalue.Value.ExpenseCheckId, objResponse);
                            }
                            else
                            {
                                logger.Info("ExpenseAccountsController - InsertExpenseCheck InsertExpenseCheckErrorSyncId - " + QBSyncData + DateTime.Now);
                                _ExpenseCheckSettingRepository.InsertExpenseErrorSyncId(expensevalue.Value.ExpenseCheckId, objResponse);
                            }
                        }

                    }
                    SuccessMessage = "Expense Updated Successfully.";
                }
                else
                {
                    ErrorMessage = "Payee/Payment Account/Payment Method Please Select this dropdown";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("ExpenseAccountsController - UpdateExpenseCheck - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = updateexpnese, success = SuccessMessage, Error = ErrorMessage });
        }

        public async Task<ActionResult> RemoveExpenseCheck(int ExpenseCheckId)
        {
            ExpenseCheck deleteexpnese = new ExpenseCheck();
            SuccessMessage = null;
            ErrorMessage = null;
            try
            {
                deleteexpnese.ExpenseCheckId = ExpenseCheckId;
                deleteexpnese.UpdatedBy = _CommonRepository.getUserId(System.Web.HttpContext.Current.User.Identity.Name);
                _ExpenseCheckSettingRepository.DeleteExpenseCheckData(deleteexpnese);

                var expensevalue = _ExpenseCheckSettingRepository.GetExpenseCheckById(ExpenseCheckId);
                if (expensevalue.PaymentType == PaymentType.Check)
                {
                    SuccessMessage = "Check Deleted Successfully.";
                }
                else
                {
                    SuccessMessage = "Expense Deleted Successfully.";
                }

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("ExpenseAccountsController - RemoveExpenseCheck - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = deleteexpnese, success = SuccessMessage, Error = ErrorMessage });
        }

        //add expense directly (Himanshu 21-12-2024)
        [Authorize(Roles = "Administrator,AddExpenseExpenseCheck")]
        public ActionResult ExpenseAddNew()
        {
            ViewBag.Title = "Add Expense - Synthesis";
            ExpenseCheck expensevalue = new ExpenseCheck();
            try
            {
                logger.Info("ExpenseAccountsController - ExpenseAddNew - " + DateTime.Now);
                int StoreId = 0;
                StoreId = Convert.ToInt32(Session["storeid"]);
                var vendors = _MastersBindRepository.GetVendorMaster(StoreId).Select(s => new DropdownViewModelExpense { Text = s.VendorName + " (Vendor)", Value = s.VendorId.ToString() + "-V" }).OrderBy(o => o.Text).ToList();
                var employees = _MastersBindRepository.GetEmployeeMaster(StoreId).Select(s => new DropdownViewModelExpense { Text = s.DisplayName + " (Employee)", Value = s.EmployeeId.ToString() + "-E" }).OrderBy(o => o.Text).ToList();

                // Combine Vendors and Employees
                ViewBag.PayeeName = vendors.Concat(employees).ToList();

                ViewBag.AccountName = _ExpenseCheckSettingRepository.DepartmentAccountTypeList(StoreId).Select(s => new DropdownViewModel { Text = s.DepartmentName, Value = s.DepartmentId }).OrderBy(o => o.Text).ToList();
                ViewBag.PaymentName = Enum.GetValues(typeof(PaymentType)).Cast<PaymentType>().Select(e => new DropdownViewModel { Text = e.ToString(), Value = (int)e }).OrderBy(o => o.Text).ToList();
                ViewBag.DepartmentName = _ExpenseCheckSettingRepository.DepartmentDetailList(StoreId).Select(s => new DropdownViewModel { Text = s.DepartmentName, Value = s.DepartmentId }).OrderBy(o => o.Text).ToList();
                ViewBag.PaymentMethodName = _ExpenseCheckSettingRepository.PaymentMethodList().Where(s => s.IsActive == true && s.StoreId == StoreId).Select(s => new DropdownViewModel { Text = s.PaymentMethod, Value = s.PaymentMethodId }).OrderBy(o => o.Text).ToList();

                ViewBag.PayeeName.Insert(0, new DropdownViewModelExpense { Text = "-Select Payee-", Value = "" });
                ViewBag.AccountName.Insert(0, new DropdownViewModel { Text = "-Select Payment Account-", Value = 0 });
                ViewBag.PaymentName.Insert(0, new DropdownViewModel { Text = "-Select Payment Type-", Value = 0 });
                ViewBag.PaymentMethodName.Insert(0, new DropdownViewModel { Text = "-Select Payment Method-", Value = 0 });

                if (StoreId != 0)
                {
                    if (!string.IsNullOrEmpty(Session["storeid"].ToString()))
                    {
                        string store_idval = Session["storeid"].ToString();
                        ViewBag.Storeidvalue = store_idval;
                    }
                }

                if (StatusMessage == "Success Expense" || StatusMessage == "Error Expense")
                {
                    ViewBag.StatusMessage = StatusMessage;
                    StatusMessage = "";
                }
                else
                {
                    ViewBag.StatusMessage = "";
                }
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsController - ExpenseAddNew - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View(expensevalue);
        }

        [HttpPost]
        public ActionResult ExpenseAddNew(ExpenseCheck obj)
        {
            logger.Info("ExpenseAccountsController - ExpenseAddNewData - " + JsonConvert.SerializeObject(obj) + DateTime.Now);
            ExpenseCheck expensecheck = new ExpenseCheck();
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            try
            {
                logger.Info("ExpenseAccountsController - ExpenseAddNew - " + DateTime.Now);
                int StoreId = 0;
                StoreId = Convert.ToInt32(Session["storeid"]);
                obj.CreatedBy = _CommonRepository.getUserId(UserName);
                obj.StoreId = StoreId;
                //obj.TxnDate = obj.TxnDate.Value.Date.Add(DateTime.Now.TimeOfDay);
                obj.TxnDate = obj.TxnDate.Value.Date;
                obj.PaymentTypeId = 2;

                // Extract VendorId and RefType from VendorIdstr
                if (!string.IsNullOrEmpty(obj.VendorIdstr))
                {
                    if (obj.VendorIdstr.EndsWith("-V"))
                    {
                        obj.RefType = "V";
                        obj.VendorIdstr = obj.VendorIdstr.Replace("-V", "");
                    }
                    else if (obj.VendorIdstr.EndsWith("-E"))
                    {
                        obj.RefType = "E";
                        obj.VendorIdstr = obj.VendorIdstr.Replace("-E", "");
                    }

                    if (int.TryParse(obj.VendorIdstr, out int vendorId))
                    {
                        obj.VendorId = vendorId;
                    }
                }


                if (obj.VendorId != 0 && obj.BankAccountId != 0)
                {

                    expensecheck = _ExpenseCheckSettingRepository.InsertExpenseData(obj);

                    if (Convert.ToInt32(expensecheck.ExpenseCheckId) > 0)
                    {
                        SynthesisQBOnline.BAL.QBResponse objResponse = new SynthesisQBOnline.BAL.QBResponse();
                        //This db class is QB sync Expense
                        bool QBSyncData = _qBRepository.QBSyncExpense(obj, obj.ExpenseCheckDetails, Convert.ToInt32(obj.StoreId), ref objResponse);
                        logger.Info("ExpenseAccountsController - ExpenseAddNew return QBSyncData - " + QBSyncData + "ref Message objResponse" + objResponse + DateTime.Now);
                        if (QBSyncData)
                        {
                            logger.Info("ExpenseAccountsController - ExpenseAddNew InsertExpenseSyncId - " + QBSyncData + DateTime.Now);
                            _ExpenseCheckSettingRepository.InsertExpenseSyncId(expensecheck.ExpenseCheckId, objResponse);
                        }
                        else
                        {
                            logger.Info("ExpenseAccountsController - ExpenseAddNew InsertExpenseCheckErrorSyncId - " + QBSyncData + DateTime.Now);
                            _ExpenseCheckSettingRepository.InsertExpenseErrorSyncId(expensecheck.ExpenseCheckId, objResponse);
                        }
                    }

                    if (Convert.ToInt32(expensecheck.ExpenseCheckId) > 0)
                    {
                        foreach (var item in obj.ExpenseCheckDetails)
                        {
                            if (item.DepartmentId != null && item.Amount != null)
                            {
                                ExpenseCheckDetail expensedetails = new ExpenseCheckDetail();
                                expensedetails.ExpenseCheckId = expensecheck.ExpenseCheckId;
                                expensedetails.DepartmentId = item.DepartmentId;
                                expensedetails.Amount = item.Amount;
                                expensedetails.Description = item.Description;
                                _ExpenseCheckSettingRepository.InsertExpenseDetailsData(expensedetails);
                            }
                        }
                        if (!string.IsNullOrEmpty(obj.FileName))
                        {
                            var files = obj.FileName.Split(',').ToList();
                            foreach (var file in files)
                            {
                                if (!string.IsNullOrWhiteSpace(file))
                                {
                                    ExpenseCheckDocuments expensefile = new ExpenseCheckDocuments();
                                    expensefile.ExpenseCheckId = expensecheck.ExpenseCheckId;
                                    expensefile.DocumentName = file.Trim();
                                    expensefile.CreatedBy = _CommonRepository.getUserId(UserName);
                                    _ExpenseCheckSettingRepository.InsertExpenseFileData(expensefile);
                                }
                            }
                        }
                    }
                    StatusMessage = "Success Expense";
                    ViewBag.StatusMessage = StatusMessage;
                }

            }
            catch (Exception ex)
            {
                StatusMessage = "Error Expense";
                ViewBag.StatusMessage = StatusMessage;
                logger.Error("ExpenseAccountsController - ExpenseAddNew - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            if (obj.SavebtnName == "SaveAndNew")
            {
                return RedirectToAction("ExpenseAddNew");
            }
            return RedirectToAction("ExpenseCheckIndexNew");
        }

        //add check directly (Himanshu 26-12-2024)
        [Authorize(Roles = "Administrator,AddCheckExpenseCheck")]
        public ActionResult CheckAddNew()
        {
            ViewBag.Title = "Add Check - Synthesis";
            ExpenseCheck expensevalue = new ExpenseCheck();
            try
            {
                logger.Info("ExpenseAccountsController - CheckAddNew - " + DateTime.Now);
                int StoreId = 0;
                StoreId = Convert.ToInt32(Session["storeid"]);
                var vendors = _MastersBindRepository.GetVendorMaster(StoreId).Select(s => new DropdownViewModelExpense { Text = s.VendorName + " (Vendor)", Value = s.VendorId.ToString() + "-V" }).OrderBy(o => o.Text).ToList();
                var employees = _MastersBindRepository.GetEmployeeMaster(StoreId).Select(s => new DropdownViewModelExpense { Text = s.DisplayName + " (Employee)", Value = s.EmployeeId.ToString() + "-E" }).OrderBy(o => o.Text).ToList();

                // Combine Vendors and Employees
                ViewBag.PayeeName = vendors.Concat(employees).ToList();

                //ViewBag.PayeeName = _MastersBindRepository.GetVendorMaster(StoreId).Select(s => new DropdownViewModel { Text = s.VendorName, Value = s.VendorId }).OrderBy(o => o.Text).ToList();
                ViewBag.AccountName = _ExpenseCheckSettingRepository.CheckDepartmentAccountTypeList(StoreId).Select(s => new DropdownViewModel { Text = s.DepartmentName, Value = s.DepartmentId }).OrderBy(o => o.Text).ToList();
                ViewBag.PaymentName = Enum.GetValues(typeof(PaymentType)).Cast<PaymentType>().Select(e => new DropdownViewModel { Text = e.ToString(), Value = (int)e }).OrderBy(o => o.Text).ToList();
                ViewBag.DepartmentName = _ExpenseCheckSettingRepository.DepartmentDetailList(StoreId).Select(s => new DropdownViewModel { Text = s.DepartmentName, Value = s.DepartmentId }).OrderBy(o => o.Text).ToList();

                ViewBag.PayeeName.Insert(0, new DropdownViewModelExpense { Text = "-Select Payee-", Value = "" });
                ViewBag.AccountName.Insert(0, new DropdownViewModel { Text = "-Select Bank Account-", Value = 0 });
                ViewBag.PaymentName.Insert(0, new DropdownViewModel { Text = "-Select Payment Type-", Value = 0 });
                expensevalue.PrintLater = true;


                if (StoreId != 0)
                {
                    if (!string.IsNullOrEmpty(Session["storeid"].ToString()))
                    {
                        string store_idval = Session["storeid"].ToString();
                        ViewBag.Storeidvalue = store_idval;
                    }
                }

                if (StatusMessage == "Success Check" || StatusMessage == "Error Check")
                {
                    ViewBag.StatusMessage = StatusMessage;
                    StatusMessage = "";
                }
                else
                {
                    ViewBag.StatusMessage = "";
                }
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsController - CheckAddNew - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View(expensevalue);
        }

        [HttpPost]
        public ActionResult CheckAddNew(ExpenseCheck obj)
        {
            logger.Info("ExpenseAccountsController - CheckAddNewData - " + JsonConvert.SerializeObject(obj) + DateTime.Now);
            ExpenseCheck expensecheck = new ExpenseCheck();
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            try
            {
                logger.Info("ExpenseAccountsController - CheckAddNew - " + DateTime.Now);
                int StoreId = 0;
                StoreId = Convert.ToInt32(Session["storeid"]);
                obj.CreatedBy = _CommonRepository.getUserId(UserName);
                obj.StoreId = StoreId;
                //obj.PaymentDate = obj.PaymentDate.Value.Date.Add(DateTime.Now.TimeOfDay);
                obj.TxnDate = obj.TxnDate.Value.Date;
                obj.PaymentTypeId = 1;
                obj.PaymentMethodId = _ExpenseCheckSettingRepository.PaymentMethodList().Where(s => s.PaymentMethod == "Check" && s.StoreId == StoreId).Select(s => s.PaymentMethodId).FirstOrDefault();

                // Extract VendorId and RefType from VendorIdstr
                if (!string.IsNullOrEmpty(obj.VendorIdstr))
                {
                    if (obj.VendorIdstr.EndsWith("-V"))
                    {
                        obj.RefType = "V";
                        obj.VendorIdstr = obj.VendorIdstr.Replace("-V", "");
                    }
                    else if (obj.VendorIdstr.EndsWith("-E"))
                    {
                        obj.RefType = "E";
                        obj.VendorIdstr = obj.VendorIdstr.Replace("-E", "");
                    }

                    if (int.TryParse(obj.VendorIdstr, out int vendorId))
                    {
                        obj.VendorId = vendorId;
                    }
                }

                if (obj.VendorId != 0 && obj.BankAccountId != 0)
                {
                    expensecheck = _ExpenseCheckSettingRepository.InsertExpenseData(obj);

                    if (Convert.ToInt32(expensecheck.ExpenseCheckId) > 0)
                    {
                        SynthesisQBOnline.BAL.QBResponse objResponse = new SynthesisQBOnline.BAL.QBResponse();
                        //This db class is QB sync Expense
                        bool QBSyncData = _qBRepository.QBSyncExpense(obj, obj.ExpenseCheckDetails, Convert.ToInt32(obj.StoreId), ref objResponse);
                        logger.Info("ExpenseAccountsController - CheckAddNew return QBSyncData - " + QBSyncData + "ref Message objResponse" + objResponse + DateTime.Now);
                        if (QBSyncData)
                        {
                            logger.Info("ExpenseAccountsController - CheckAddNew InsertCheckSyncId - " + QBSyncData + DateTime.Now);
                            _ExpenseCheckSettingRepository.InsertExpenseSyncId(expensecheck.ExpenseCheckId, objResponse);
                        }
                        else
                        {
                            logger.Info("ExpenseAccountsController - CheckAddNew InsertExpenseCheckErrorSyncId - " + QBSyncData + DateTime.Now);
                            _ExpenseCheckSettingRepository.InsertExpenseErrorSyncId(expensecheck.ExpenseCheckId, objResponse);
                        }
                    }

                    if (Convert.ToInt32(expensecheck.ExpenseCheckId) > 0)
                    {
                        foreach (var item in obj.ExpenseCheckDetails)
                        {
                            if (item.DepartmentId != null && item.Amount != null)
                            {
                                ExpenseCheckDetail expensedetails = new ExpenseCheckDetail();
                                expensedetails.ExpenseCheckId = expensecheck.ExpenseCheckId;
                                expensedetails.DepartmentId = item.DepartmentId;
                                expensedetails.Amount = item.Amount;
                                expensedetails.Description = item.Description;
                                _ExpenseCheckSettingRepository.InsertExpenseDetailsData(expensedetails);
                            }
                        }
                        if (!string.IsNullOrEmpty(obj.FileName))
                        {
                            var files = obj.FileName.Split(',').ToList();
                            foreach (var file in files)
                            {
                                if (!string.IsNullOrWhiteSpace(file))
                                {
                                    ExpenseCheckDocuments expensefile = new ExpenseCheckDocuments();
                                    expensefile.ExpenseCheckId = expensecheck.ExpenseCheckId;
                                    expensefile.DocumentName = file.Trim();
                                    expensefile.CreatedBy = _CommonRepository.getUserId(UserName);
                                    _ExpenseCheckSettingRepository.InsertExpenseFileData(expensefile);
                                }
                            }
                        }
                    }
                    StatusMessage = "Success Check";
                    ViewBag.StatusMessage = StatusMessage;
                }

            }
            catch (Exception ex)
            {
                StatusMessage = "Error";
                ViewBag.StatusMessage = StatusMessage;
                logger.Error("ExpenseAccountsController - CheckAddNew - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            if (obj.SavebtnName == "SaveAndNew")
            {
                return RedirectToAction("CheckAddNew");
            }
            return RedirectToAction("ExpenseCheckIndexNew");
        }

        public ActionResult GetPayeeMailingAddress(string payeeidstr)
        {
            string mailingaddress = "";
            int payeeid = 0;
            try
            {
                string numericPart = payeeidstr.Split('-')[0];
                payeeid = Convert.ToInt32(numericPart);
                mailingaddress = _ExpenseCheckSettingRepository.GetPayeeMailingAddressById(payeeid);
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsController - GetPayeeMailingAddress - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return Json(new { mailingaddress = mailingaddress });
        }

        public ActionResult GetExpenseCheckPaymentAccount(int paytype, int editstoreid)
        {
            int StoreId = 0;
            StoreId = Convert.ToInt32(Session["storeid"]);
            if (editstoreid != 0)
            {
                StoreId = editstoreid;
            }
            List<DropdownViewModel> accountList = new List<DropdownViewModel>();
            try
            {
                if (paytype == 1)
                {
                    accountList = _ExpenseCheckSettingRepository.CheckDepartmentAccountTypeList(StoreId).Select(s => new DropdownViewModel { Text = s.DepartmentName, Value = s.DepartmentId }).OrderBy(o => o.Text).ToList();
                    accountList.Insert(0, new DropdownViewModel { Text = "-Select Bank Account-", Value = 0 });
                }
                else
                {
                    accountList = _ExpenseCheckSettingRepository.DepartmentAccountTypeList(StoreId).Select(s => new DropdownViewModel { Text = s.DepartmentName, Value = s.DepartmentId }).OrderBy(o => o.Text).ToList();
                    accountList.Insert(0, new DropdownViewModel { Text = "-Select Payment Account-", Value = 0 });
                }

                return Json(accountList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsController - GetExpenseCheckPaymentAccount - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json(new { success = false, message = "An error occurred." });
            }
        }

        [AcceptVerbs("Post")]
        public void Save()
        {
            try
            {
                logger.Info("ExpenseAccountsController - Save - " + DateTime.Now);
                string Sacn_Title = "";
                int? StoreID = null;
                if (Session["StoreId"] != null && !string.IsNullOrEmpty(Session["StoreId"].ToString()))
                {
                    StoreID = Convert.ToInt32(Session["StoreId"]);
                }
                if (System.Web.HttpContext.Current.Request.Files.AllKeys.Length > 0)
                {
                    var httpPostedFile = System.Web.HttpContext.Current.Request.Files["UploadFiles"];
                    Sacn_Title = AdminSiteConfiguration.GetRandomNo() + System.IO.Path.GetFileName(httpPostedFile.FileName);
                    Sacn_Title = AdminSiteConfiguration.RemoveSpecialCharacter(Sacn_Title);
                    if (httpPostedFile != null)
                    {
                        var baseFolderPath = System.Web.HttpContext.Current.Server.MapPath("~/UserFiles/ExpenseDocuments");
                        if (!Directory.Exists(baseFolderPath))
                        {
                            Directory.CreateDirectory(baseFolderPath);
                        }
                        var yearFolderPath = Path.Combine(baseFolderPath, DateTime.Now.Year.ToString());
                        if (!Directory.Exists(yearFolderPath))
                        {
                            Directory.CreateDirectory(yearFolderPath);
                        }
                        var storeFolderPath = Path.Combine(yearFolderPath, StoreID.ToString());
                        if (!Directory.Exists(storeFolderPath))
                        {
                            Directory.CreateDirectory(storeFolderPath);
                        }
                        var monthFolderPath = Path.Combine(storeFolderPath, DateTime.Now.ToString("MMMM"));
                        if (!Directory.Exists(monthFolderPath))
                        {
                            Directory.CreateDirectory(monthFolderPath);
                        }
                        var userIdFolderPath = Path.Combine(monthFolderPath, UserModule.getUserId().ToString());
                        if (!Directory.Exists(userIdFolderPath))
                        {
                            Directory.CreateDirectory(userIdFolderPath);
                        }
                        var fileSavePath = System.IO.Path.Combine(userIdFolderPath, Sacn_Title);
                        var relativePath = fileSavePath.Substring(baseFolderPath.Length).TrimStart('\\', '/');
                        if (!System.IO.File.Exists(fileSavePath))
                        {
                            httpPostedFile.SaveAs(fileSavePath);
                            HttpResponse Response = System.Web.HttpContext.Current.Response;
                            Response.Clear();
                            Response.Headers.Add("name", relativePath);
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
                            AdminSiteConfiguration.WriteErrorLogs(relativePath + " File already exists.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsController - Save - " + DateTime.Now + " - " + ex.Message.ToString());
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

        //printchecks code start from here(Himanshu 17-01-2025)
        [Authorize(Roles = "Administrator,PrintExpenseCheck")]
        public ActionResult PrintCheckIndex(int? ExpenseCheckId)
        {
            ViewBag.Title = "PrintCheck - Synthesis";
            var StoreIdSsn = Session["storeid"];
            if (StoreIdSsn != null)
            {
                if (!string.IsNullOrEmpty(Session["storeid"].ToString()))
                {
                    string store_idval = Session["storeid"].ToString();
                    ViewBag.Storeidvalue = store_idval;
                }
            }
            ViewBag.SelectedExpenseCheckId = ExpenseCheckId;
            return View();
        }

        public ActionResult UrlDatasourcePrintCheck(DataManagerRequest dm)
        {
            List<ExpenseCheckSelect> BindData = new List<ExpenseCheckSelect>();
            IEnumerable DataSource = new List<HRStoreList>();
            int Count = 0;
            int StoreId = 0;
            StoreId = Convert.ToInt32(Session["storeid"]);
            try
            {
                logger.Info("ExpenseAccountsController - UrlDatasourcePrintCheck - " + DateTime.Now);

                BindData = _ExpenseCheckSettingRepository.GetPrintCheckList(StoreId);
                BindData = BindData.Select(s => new ExpenseCheckSelect
                {
                    ExpenseCheckId = s.ExpenseCheckId,
                    InvoiceDate = s.InvoiceDate,
                    Type = s.Type,
                    VendorName = s.VendorName,
                    Amount = s.Amount,
                    AmountString = s.Amount.ToString("#,##0.00")
                }).ToList();
                DataSource = BindData;

                DataOperations operation = new DataOperations();
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    string search = dm.Search[0].Key.ToString().Trim().ToLower();
                    DataSource = BindData.ToList().Where(x => (x.VendorName == null ? "" : x.VendorName.ToLower()).ToString().Contains(search) || (x.Type == null ? "" : x.Type.ToLower()).ToString().Contains(search)
                   || (x.Amount == null ? 0 : x.Amount).ToString().Contains(search) || Convert.ToDateTime(x.InvoiceDate).ToString("MM-dd-yyyy").Equals(search)).ToList();
                }
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                }
                Count = DataSource.Cast<ExpenseCheckSelect>().Count();
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
                logger.Error("ExpenseAccountsController - UrlDatasourcePrintCheck - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return dm.RequiresCounts ? Json(new { result = DataSource, count = Count }) : Json(DataSource);
        }

        public ActionResult GetPrintCheckPayeeData(string expenseCheckIds)
        {
            List<CheckPdfPrint> checkData = new List<CheckPdfPrint>();
            try
            {
                checkData = _ExpenseCheckSettingRepository.GetPrintCheckPayeeData(expenseCheckIds);
                for (int i = 0; i < checkData.Count; i++)
                {
                    var item = checkData[i];

                    item.TotalAmountInWord = NumberToWordsConverter.ConvertNumberToWords(item.TotalAmount);
                    item.DateString = item.Date.ToString("MM/dd/yyyy");

                    item.CheckPdfPrintDetails = _ExpenseCheckSettingRepository.GetPrintCheckDetails(item.ExpenseCheckId);
                }
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsController - GetPrintCheckPayeeData - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return Json(checkData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetQbErrorMessage(int expensecheckid)
        {
            string qberrormessage = "";
            try
            {
                qberrormessage = _ExpenseCheckSettingRepository.GetQbErrorMessageById(expensecheckid);
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseAccountsController - GetQbErrorMessage - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return Json(new { qberrormessage = qberrormessage });
        }
        public static class NumberToWordsConverter
        {
            private static readonly string[] UnitsMap = { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
            private static readonly string[] TensMap = { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
            private static readonly string[] ThousandsMap = { "", "Thousand", "Million", "Billion", "Trillion" };

            public static string ConvertNumberToWords(decimal number)
            {
                if (number == 0) return "Zero";

                string words = "";

                // Process integer part
                int intPart = (int)number;
                int thousandCounter = 0;

                while (intPart > 0)
                {
                    if (intPart % 1000 != 0)
                    {
                        words = ConvertHundreds(intPart % 1000) + ThousandsMap[thousandCounter] + " " + words;
                    }
                    intPart /= 1000;
                    thousandCounter++;
                }

                // Process decimal part (cents)
                int decimalPart = (int)((number - Math.Floor(number)) * 100);
                if (decimalPart > 0)
                {
                    words += "and " + ConvertHundreds(decimalPart) + " Cents";
                }

                return words.Trim();
            }

            private static string ConvertHundreds(int number)
            {
                string words = "";
                if (number >= 100)
                {
                    words += UnitsMap[number / 100] + " Hundred ";
                    number %= 100;
                }
                if (number >= 20)
                {
                    words += TensMap[number / 10] + " ";
                    number %= 10;
                }
                if (number > 0)
                {
                    words += UnitsMap[number] + " ";
                }
                return words;
            }
        }
        //end

        #region Bulk Transaction Check Start From here(Himanshu 13-02-2025)
        [Authorize(Roles = "Administrator,AddBulkChecksExpenseCheck")]
        public ActionResult ExpenseCheckBulkTransaction()
        {
            ViewBag.Title = "Bulk Import Check - Synthesis";
            int StoreId = 0;
            StoreId = Convert.ToInt32(Session["storeid"]);

            var vendors = _MastersBindRepository.GetVendorMaster(StoreId).Select(s => new DropdownViewModelExpense { Text = s.VendorName + " (Vendor)", Value = s.VendorId.ToString() + "-V" }).OrderBy(o => o.Text).ToList();
            var employees = _MastersBindRepository.GetEmployeeMaster(StoreId).Select(s => new DropdownViewModelExpense { Text = s.DisplayName + " (Employee)", Value = s.EmployeeId.ToString() + "-E" }).OrderBy(o => o.Text).ToList();

            // Combine Vendors and Employees
            ViewBag.PayeeName = vendors.Concat(employees).ToList();
            ViewBag.AccountName = _ExpenseCheckSettingRepository.CheckDepartmentAccountTypeList(StoreId).Select(s => new DropdownViewModel { Text = s.DepartmentName, Value = s.DepartmentId }).OrderBy(o => o.Text).ToList();
            ViewBag.DepartmentName = _ExpenseCheckSettingRepository.DepartmentDetailList(StoreId).Select(s => new DropdownViewModel { Text = s.DepartmentName, Value = s.DepartmentId }).OrderBy(o => o.Text).ToList();
            //ViewBag.PayeeName.Insert(0, new DropdownViewModelExpense { Text = "-Select Payee-", Value = "" });
            ViewBag.AccountName.Insert(0, new DropdownViewModel { Text = "-Select Bank Account-", Value = 0 });

            var StoreIdSsn = Session["storeid"];
            if (StoreIdSsn != null)
            {
                if (!string.IsNullOrEmpty(Session["storeid"].ToString()))
                {
                    string store_idval = Session["storeid"].ToString();
                    ViewBag.Storeidvalue = store_idval;
                }
            }
            return View();
        }

        public ActionResult BulkUrlDatasourceCheck(DataManagerRequest dm, string Status, string Startdate = null, string Enddate = null)
        {
            List<Invoice> BindData1 = new List<Invoice>();
            List<ExpenseCheckSelectMain> BindData = new List<ExpenseCheckSelectMain>();
            IEnumerable DataSource = new List<ExpenseCheckSelectMain>();
            DataOperations operation = new DataOperations();

            if (Startdate == "")
            {
                Startdate = null;
            }
            if (Enddate == "")
            {
                Enddate = null;
            }
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            //This  class is Get usertype id.
            var UserTypeId = _CommonRepository.getUserTypeId(UserName);
            string payment = "";
            int count = 0;
            try
            {
                if (Session["payment"] != null)
                {
                    payment = Session["payment"].ToString();
                }
                var StoreId = Convert.ToInt32(Session["storeid"]);

                var strStore = "";
                strStore = GetExpenseCheck_StoreList(StoreId);
                if (StoreId == 0 && strStore.Length > 0)
                {
                    var S = (strStore.Split(',').Count() == 1 ? Convert.ToInt32(strStore.Split(',')[0]) : 0);
                    if (S > 0)
                    {
                        Session["storeid"] = S;
                        StoreId = S;
                    }
                }

                if (Status == null || Status == "All")
                {
                    Status = "";
                }
                string ForceStatus = "";
                if (Status == "" && !Roles.IsUserInRole("Administrator"))
                {
                    Boolean ischeck = false;
                    if (Roles.IsUserInRole("IncludeExpenseCheck"))
                    {
                        ischeck = true;
                        Status = "Include";
                        ForceStatus = "WithForceInclude";
                    }
                    if (Roles.IsUserInRole("ExcludeExpenseCheck"))
                    {
                        if (ischeck == true)
                            Status = "";
                        else
                        {
                            Status = "Exclude";
                            ForceStatus = "WithForceExclude";
                        }
                    }
                }
                if (Status == "Override" && !Roles.IsUserInRole("Administrator"))
                {
                    if (Roles.IsUserInRole("IncludeExpenseCheck"))
                    {
                        Status = "OverrideInclude";
                    }
                    if (Roles.IsUserInRole("ExcludeExpenseCheck"))
                    {
                        Status = "OverrideExclude";
                    }
                }

                //This  class is Get usertype id.
                //Get Expense Check previes data.
                if (StoreId == 0)
                {
                    DataSource = new List<ExpenseCheckSelectMain>();
                    count = 0;
                }
                else
                {

                    BindData = _ExpenseCheckSettingRepository.BulkCheckPreviewData_Beta(payment, "TXnDate", "Desc", Status, ForceStatus, _CommonRepository.getUserTypeId(UserName), strStore, Startdate, Enddate);
                    BindData = BindData.Select(s => new ExpenseCheckSelectMain
                    {
                        VendorName = s.VendorName,
                        Type = s.Type,
                        InvoiceDate = s.InvoiceDate,
                        ModifyDate = s.ModifyDate,
                        CreateOn = s.CreateOn,
                        StoreName = s.StoreName,
                        Amount = s.Amount,
                        InvoiceId = s.InvoiceId,
                        Department = s.Department,
                        Memo = s.Memo,
                        DocNumber = s.DocNumber,
                        ExpenseCheckDetailId = s.ExpenseCheckDetailId,
                        ExpenseCheckId = s.ExpenseCheckId,
                        ViewDocFlg = s.ViewDocFlg,
                        QBType = s.QBType,
                        DisplayExpenseStatus = s.DisplayExpenseStatus,
                        ExpenseStatus = s.ExpenseStatus,
                        AmountString = s.Amount.ToString("#,##0.00"),
                        PrintLater = s.PrintLater,
                        IsSync = s.IsSync,
                        QBErrorMessage = s.QBErrorMessage,
                        PaymentAccountName = s.PaymentAccountName,
                    }).ToList();
                    DataSource = BindData;

                    count = DataSource.Cast<ExpenseCheckSelectMain>().Count();
                }



                if (dm.Search != null && dm.Search.Count > 0)
                {
                    string search = dm.Search[0].Key.ToString().Trim().ToLower();
                    DataSource = BindData.ToList().Where(x => (x.VendorName == null ? "" : x.VendorName.ToLower()).ToString().Contains(search) || (x.Type == null ? "" : x.Type.ToLower()).ToString().Contains(search) || (x.DocNumber == null ? "" : x.DocNumber).ToString().Contains(search) || (x.StoreName == null ? "" : x.StoreName.ToLower()).ToString().Contains(search) || (x.PaymentAccountName == null ? "" : x.PaymentAccountName.ToLower()).ToString().Contains(search)
                   || (x.Amount == null ? 0 : x.Amount).ToString().Contains(search) || (x.Department == null ? "" : x.Department.ToLower()).ToString().Contains(search) || (x.Memo == null ? "" : x.Memo.ToLower()).ToString().Contains(search) || Convert.ToDateTime(x.InvoiceDate).ToString("MM-dd-yyyy").Equals(search)).ToList();
                }
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                }

                //int count = totalRows;
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
                logger.Error("ExpenseAccountsController - UrlDatasourceExpense - " + DateTime.Now + " - " + ex.Message.ToString());

            }

            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }

        [HttpPost]
        public async Task<ActionResult> InsertBulkTransaction(CRUDModel<ExpenseCheckSelectMain> expensevalue, int bankid, string paymentdate)
        {
            try
            {
                logger.Info("ExpenseAccountsController - InsertBulkTransaction - " + JsonConvert.SerializeObject(expensevalue) + " BankId:" + bankid + " PaymentDate:" + paymentdate + DateTime.Now);
                SuccessMessage = null;
                ErrorMessage = null;
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                int StoreId = Convert.ToInt32(Session["storeid"]);

                var vendormaster = _MastersBindRepository.GetVendorMaster(StoreId).ToList();
                var employeemaster = _MastersBindRepository.GetEmployeeMaster(StoreId).ToList();
                var departments = _ExpenseCheckSettingRepository.DepartmentDetailList(StoreId).ToList();
                //var bankacct = _ExpenseCheckSettingRepository.CheckDepartmentAccountTypeList(StoreId).ToList();

                // Remove "(Vendor)" or "(Employee)" from VendorName
                if (expensevalue?.Value?.VendorName != null)
                {
                    expensevalue.Value.VendorName = Regex.Replace(expensevalue.Value.VendorName, @"\s*\((Vendor|Employee)\)\s*$", "");
                }

                ExpenseCheck expensecheck = new ExpenseCheck
                {
                    ExpenseCheckId = expensevalue.Value.ExpenseCheckId,
                    CreatedBy = _CommonRepository.getUserId(UserName),
                    StoreId = StoreId,
                    PaymentTypeId = 1,
                    BankAccountId = Convert.ToInt32(bankid),
                    PaymentMethodId = _ExpenseCheckSettingRepository.PaymentMethodList().Where(s => s.PaymentMethod == "Check" && s.StoreId == StoreId).Select(s => s.PaymentMethodId).FirstOrDefault(),
                    DocNumber = expensevalue.Value.DocNumber,
                    Memo = expensevalue.Value.Memo,
                    PrintLater = !string.IsNullOrEmpty(expensevalue.Value.DocNumber) ? false : true,
                    TxnDate = Convert.ToDateTime(paymentdate),
                    QBErrorMessage = "Check Created by Bulk Transaction"
                };

                // Determine VendorId and RefType
                var vendor = vendormaster.FirstOrDefault(v => v.VendorName == expensevalue.Value.VendorName);
                var employee = employeemaster.FirstOrDefault(e => e.DisplayName == expensevalue.Value.VendorName);

                if (vendor != null)
                {
                    expensecheck.VendorId = vendor.VendorId;
                    expensecheck.RefType = "V";
                    expensecheck.MailingAddress = _ExpenseCheckSettingRepository.GetPayeeMailingAddressById(vendor.VendorId);
                }
                else if (employee != null)
                {
                    expensecheck.VendorId = employee.EmployeeId;
                    expensecheck.RefType = "E";
                }

                expensecheck.TotalAmt = Convert.ToDecimal(expensevalue.Value.AmountString);
                expensecheck = _ExpenseCheckSettingRepository.InsertExpenseData(expensecheck);

                if (expensecheck.ExpenseCheckId > 0)
                {
                    // Get DepartmentId
                    var department = departments.FirstOrDefault(d => d.DepartmentName == expensevalue.Value.Department);
                    int departmentId = department != null ? department.DepartmentId : 0;

                    ExpenseCheckDetail expensedetails = new ExpenseCheckDetail
                    {
                        ExpenseCheckDetailId = expensevalue.Value.ExpenseCheckDetailId,
                        ExpenseCheckId = expensecheck.ExpenseCheckId,
                        DepartmentId = departmentId,
                        Amount = Convert.ToDecimal(expensevalue.Value.AmountString)
                    };

                    _ExpenseCheckSettingRepository.InsertExpenseDetailsData(expensedetails);

                    StatusMessage = "Success BulkCheck";
                    SuccessMessage = "Check Created Successfully";
                    ViewBag.StatusMessage = StatusMessage;
                }
                else
                {
                    ErrorMessage = "Check Created UnSuccessfully";
                    ViewBag.StatusMessage = StatusMessage;
                }

                
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("ExpenseAccountsController - InsertBulkTransaction - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return Json(new { data = expensevalue, success = SuccessMessage, Error = ErrorMessage });
        }

        [HttpPost]
        public async Task<ActionResult> UpdateBulkTransaction(CRUDModel<ExpenseCheckSelectMain> expensevalue, int bankid, string paymentdate)
        {
            try
            {
                logger.Info("ExpenseAccountsController - UpdateBulkTransaction - " + JsonConvert.SerializeObject(expensevalue) + " BankId:" + bankid + " PaymentDate:" + paymentdate + DateTime.Now);
                SuccessMessage = null;
                ErrorMessage = null;
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                int StoreId = Convert.ToInt32(Session["storeid"]);

                var vendormaster = _MastersBindRepository.GetVendorMaster(StoreId).ToList();
                var employeemaster = _MastersBindRepository.GetEmployeeMaster(StoreId).ToList();
                var departments = _ExpenseCheckSettingRepository.DepartmentDetailList(StoreId).ToList();
                //var bankacct = _ExpenseCheckSettingRepository.CheckDepartmentAccountTypeList(StoreId).ToList();

                // Remove "(Vendor)" or "(Employee)" from VendorName
                if (expensevalue?.Value?.VendorName != null)
                {
                    expensevalue.Value.VendorName = Regex.Replace(expensevalue.Value.VendorName, @"\s*\((Vendor|Employee)\)\s*$", "");
                }

                ExpenseCheck expensecheck = new ExpenseCheck
                {
                    ExpenseCheckId = expensevalue.Value.ExpenseCheckId,
                    UpdatedBy = _CommonRepository.getUserId(UserName),
                    StoreId = StoreId,
                    PaymentTypeId = 1,
                    BankAccountId = Convert.ToInt32(bankid),
                    PaymentMethodId = _ExpenseCheckSettingRepository.PaymentMethodList().Where(s => s.PaymentMethod == "Check" && s.StoreId == StoreId).Select(s => s.PaymentMethodId).FirstOrDefault(),
                    DocNumber = expensevalue.Value.DocNumber,
                    Memo = expensevalue.Value.Memo,
                    PrintLater = !string.IsNullOrEmpty(expensevalue.Value.DocNumber) ? false : true,
                    TxnDate = Convert.ToDateTime(paymentdate),
                    QBErrorMessage = "Check Created by Bulk Transaction"
                };

                // Determine VendorId and RefType
                var vendor = vendormaster.FirstOrDefault(v => v.VendorName == expensevalue.Value.VendorName);
                var employee = employeemaster.FirstOrDefault(e => e.DisplayName == expensevalue.Value.VendorName);

                if (vendor != null)
                {
                    expensecheck.VendorId = vendor.VendorId;
                    expensecheck.RefType = "V";
                    expensecheck.MailingAddress = _ExpenseCheckSettingRepository.GetPayeeMailingAddressById(vendor.VendorId);
                }
                else if (employee != null)
                {
                    expensecheck.VendorId = employee.EmployeeId;
                    expensecheck.RefType = "E";
                }

                expensecheck.TotalAmt = Convert.ToDecimal(expensevalue.Value.AmountString);
                expensecheck = _ExpenseCheckSettingRepository.InsertExpenseData(expensecheck);

                if (Convert.ToInt32(expensecheck.ExpenseCheckId) > 0)
                {
                    // Get DepartmentId
                    var department = departments.FirstOrDefault(d => d.DepartmentName == expensevalue.Value.Department);
                    int departmentId = department != null ? department.DepartmentId : 0;

                    ExpenseCheckDetail expensedetails = new ExpenseCheckDetail
                    {
                        ExpenseCheckDetailId = expensevalue.Value.ExpenseCheckDetailId,
                        ExpenseCheckId = expensecheck.ExpenseCheckId,
                        DepartmentId = departmentId,
                        Amount = Convert.ToDecimal(expensevalue.Value.AmountString)
                    };

                    _ExpenseCheckSettingRepository.InsertExpenseDetailsData(expensedetails);
                }

                StatusMessage = "Success BulkCheck";
                SuccessMessage = "Check Updated Successfully";
                ViewBag.StatusMessage = StatusMessage;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("ExpenseAccountsController - UpdateBulkTransaction - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return Json(new { data = expensevalue, success = SuccessMessage, Error = ErrorMessage });
        }

        public ActionResult DeleteBulkTransaction(CRUDModel<ExpenseCheckSelectMain> expensevalue)
        {
            SuccessMessage = null;
            ErrorMessage = null;
            ExpenseCheck checkval = new ExpenseCheck();
            try
            {
                if(expensevalue.Deleted == null)
                {
                    checkval.ExpenseCheckId = Convert.ToInt32(expensevalue.Key);
                    checkval.UpdatedBy = _CommonRepository.getUserId(System.Web.HttpContext.Current.User.Identity.Name);
                    _ExpenseCheckSettingRepository.DeleteExpenseCheckData(checkval);
                }
                else
                {
                    foreach (var item in expensevalue.Deleted)
                    {
                        checkval.ExpenseCheckId = item.ExpenseCheckId;
                        checkval.UpdatedBy = _CommonRepository.getUserId(System.Web.HttpContext.Current.User.Identity.Name);
                        _ExpenseCheckSettingRepository.DeleteExpenseCheckData(checkval);
                    }
                }
                SuccessMessage = "Successfully Deleted!";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("ExpenseAccountsController - DeleteBulkTransaction - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = expensevalue.Deleted, success = SuccessMessage, Error = ErrorMessage });
        }

        public ActionResult BulkCheckSyncWithQB(string ExpenseCheckIds)
        {

            try
            {
                var expenseCheckIdArray = ExpenseCheckIds.Split(',');
                var expenseCheckDataList = new List<ExpenseCheck>();

                foreach (var expenseCheckId in expenseCheckIdArray)
                {
                    int id = int.Parse(expenseCheckId);
                    var getexpensedata = _ExpenseCheckSettingRepository.GetExpenseCheckById(id);
                    var getexpensedetailsdata = _ExpenseCheckSettingRepository.GetExpenseCheckDetailsById(id);

                    logger.Info("ExpenseAccountsController - BulkCheckSyncWithQB ExpenseCheckTableData - " + getexpensedata + DateTime.Now);
                    logger.Info("ExpenseAccountsController - BulkCheckSyncWithQB ExpenseCheckDetailsTableData - " + getexpensedetailsdata + DateTime.Now);

                    if (getexpensedata != null)
                    {
                        if(getexpensedata.PaymentType == PaymentType.Check)
                        {
                            getexpensedata.PaymentTypeId = 1;
                        }
                        else
                        {
                            getexpensedata.PaymentTypeId = 2;
                        }
                        SynthesisQBOnline.BAL.QBResponse objResponse = new SynthesisQBOnline.BAL.QBResponse();

                        bool QBSyncData = _qBRepository.QBSyncExpense(getexpensedata, getexpensedetailsdata, Convert.ToInt32(getexpensedata.StoreId), ref objResponse);
                        logger.Info("ExpenseAccountsController - BulkCheckSyncWithQB return QBSyncData - " + QBSyncData + "ref Message objResponse" + objResponse + DateTime.Now);
                        if (QBSyncData)
                        {
                            logger.Info("ExpenseAccountsController - BulkCheckSyncWithQB InsertCheckSyncId - " + QBSyncData + DateTime.Now);
                            _ExpenseCheckSettingRepository.InsertExpenseSyncId(id, objResponse);
                        }
                        else
                        {
                            logger.Info("ExpenseAccountsController - BulkCheckSyncWithQB InsertExpenseCheckErrorSyncId - " + QBSyncData + DateTime.Now);
                            _ExpenseCheckSettingRepository.InsertExpenseErrorSyncId(id, objResponse);
                        }
                        SuccessMessage = "Success";
                    }
                }
                
            }
            catch (Exception ex)
            {
                SuccessMessage = "Error";
                logger.Error("ExpenseAccountsController - BulkCheckSyncWithQB - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return Json(new { message = SuccessMessage });
        }

        #endregion Bulk Transaction Check End here

        #region Bulk Transaction Expense Start From here(Himanshu 19-02-2025)
        [Authorize(Roles = "Administrator,AddBulkExpensesExpenseCheck")]
        public ActionResult ExpenseBulkTransaction()
        {
            ViewBag.Title = "Bulk Import Expense - Synthesis";
            int StoreId = 0;
            StoreId = Convert.ToInt32(Session["storeid"]);

            var vendors = _MastersBindRepository.GetVendorMaster(StoreId).Select(s => new DropdownViewModelExpense { Text = s.VendorName + " (Vendor)", Value = s.VendorId.ToString() + "-V" }).OrderBy(o => o.Text).ToList();
            var employees = _MastersBindRepository.GetEmployeeMaster(StoreId).Select(s => new DropdownViewModelExpense { Text = s.DisplayName + " (Employee)", Value = s.EmployeeId.ToString() + "-E" }).OrderBy(o => o.Text).ToList();

            // Combine Vendors and Employees
            ViewBag.PayeeName = vendors.Concat(employees).ToList();
            ViewBag.AccountName = _ExpenseCheckSettingRepository.DepartmentAccountTypeList(StoreId).Select(s => new DropdownViewModel { Text = s.DepartmentName, Value = s.DepartmentId }).OrderBy(o => o.Text).ToList();
            ViewBag.DepartmentName = _ExpenseCheckSettingRepository.DepartmentDetailList(StoreId).Select(s => new DropdownViewModel { Text = s.DepartmentName, Value = s.DepartmentId }).OrderBy(o => o.Text).ToList();
            ViewBag.PaymentMethodName = _ExpenseCheckSettingRepository.PaymentMethodList().Where(s => s.IsActive == true && s.StoreId == StoreId).Select(s => new DropdownViewModel { Text = s.PaymentMethod, Value = s.PaymentMethodId }).OrderBy(o => o.Text).ToList();

            //ViewBag.PayeeName.Insert(0, new DropdownViewModelExpense { Text = "-Select Payee-", Value = "" });
            ViewBag.AccountName.Insert(0, new DropdownViewModel { Text = "-Select Payment Account-", Value = 0 });
            //ViewBag.PaymentMethodName.Insert(0, new DropdownViewModel { Text = "-Select Payment Method-", Value = 0 });

            var StoreIdSsn = Session["storeid"];
            if (StoreIdSsn != null)
            {
                if (!string.IsNullOrEmpty(Session["storeid"].ToString()))
                {
                    string store_idval = Session["storeid"].ToString();
                    ViewBag.Storeidvalue = store_idval;
                }
            }
            return View();
        }

        public ActionResult BulkUrlDatasourceExpense(DataManagerRequest dm, string Status, string Startdate = null, string Enddate = null)
        {
            List<Invoice> BindData1 = new List<Invoice>();
            List<ExpenseCheckSelectMain> BindData = new List<ExpenseCheckSelectMain>();
            IEnumerable DataSource = new List<ExpenseCheckSelectMain>();
            DataOperations operation = new DataOperations();

            if (Startdate == "")
            {
                Startdate = null;
            }
            if (Enddate == "")
            {
                Enddate = null;
            }
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            //This  class is Get usertype id.
            var UserTypeId = _CommonRepository.getUserTypeId(UserName);
            string payment = "";
            int count = 0;
            try
            {
                if (Session["payment"] != null)
                {
                    payment = Session["payment"].ToString();
                }
                var StoreId = Convert.ToInt32(Session["storeid"]);

                var strStore = "";
                strStore = GetExpenseCheck_StoreList(StoreId);
                if (StoreId == 0 && strStore.Length > 0)
                {
                    var S = (strStore.Split(',').Count() == 1 ? Convert.ToInt32(strStore.Split(',')[0]) : 0);
                    if (S > 0)
                    {
                        Session["storeid"] = S;
                        StoreId = S;
                    }
                }

                if (Status == null || Status == "All")
                {
                    Status = "";
                }
                string ForceStatus = "";
                if (Status == "" && !Roles.IsUserInRole("Administrator"))
                {
                    Boolean ischeck = false;
                    if (Roles.IsUserInRole("IncludeExpenseCheck"))
                    {
                        ischeck = true;
                        Status = "Include";
                        ForceStatus = "WithForceInclude";
                    }
                    if (Roles.IsUserInRole("ExcludeExpenseCheck"))
                    {
                        if (ischeck == true)
                            Status = "";
                        else
                        {
                            Status = "Exclude";
                            ForceStatus = "WithForceExclude";
                        }
                    }
                }
                if (Status == "Override" && !Roles.IsUserInRole("Administrator"))
                {
                    if (Roles.IsUserInRole("IncludeExpenseCheck"))
                    {
                        Status = "OverrideInclude";
                    }
                    if (Roles.IsUserInRole("ExcludeExpenseCheck"))
                    {
                        Status = "OverrideExclude";
                    }
                }

                //This  class is Get usertype id.
                //Get Expense Check previes data.
                if (StoreId == 0)
                {
                    DataSource = new List<ExpenseCheckSelectMain>();
                    count = 0;
                }
                else
                {

                    BindData = _ExpenseCheckSettingRepository.BulkExpensePreviewData_Beta(payment, "TXnDate", "Desc", Status, ForceStatus, _CommonRepository.getUserTypeId(UserName), strStore, Startdate, Enddate);
                    BindData = BindData.Select(s => new ExpenseCheckSelectMain
                    {
                        VendorName = s.VendorName,
                        Type = s.Type,
                        InvoiceDate = s.InvoiceDate,
                        ModifyDate = s.ModifyDate,
                        CreateOn = s.CreateOn,
                        StoreName = s.StoreName,
                        Amount = s.Amount,
                        InvoiceId = s.InvoiceId,
                        Department = s.Department,
                        Memo = s.Memo,
                        DocNumber = s.DocNumber,
                        ExpenseCheckDetailId = s.ExpenseCheckDetailId,
                        ExpenseCheckId = s.ExpenseCheckId,
                        ViewDocFlg = s.ViewDocFlg,
                        QBType = s.QBType,
                        DisplayExpenseStatus = s.DisplayExpenseStatus,
                        ExpenseStatus = s.ExpenseStatus,
                        AmountString = s.Amount.ToString("#,##0.00"),
                        PrintLater = s.PrintLater,
                        IsSync = s.IsSync,
                        QBErrorMessage = s.QBErrorMessage,
                        PaymentAccountName = s.PaymentAccountName,
                        PaymentMethodName = s.PaymentMethodName
                    }).ToList();
                    DataSource = BindData;

                    count = DataSource.Cast<ExpenseCheckSelectMain>().Count();
                }



                if (dm.Search != null && dm.Search.Count > 0)
                {
                    string search = dm.Search[0].Key.ToString().Trim().ToLower();
                    DataSource = BindData.ToList().Where(x => (x.VendorName == null ? "" : x.VendorName.ToLower()).ToString().Contains(search) || (x.Type == null ? "" : x.Type.ToLower()).ToString().Contains(search) || (x.DocNumber == null ? "" : x.DocNumber).ToString().Contains(search) || (x.StoreName == null ? "" : x.StoreName.ToLower()).ToString().Contains(search) || (x.PaymentAccountName == null ? "" : x.PaymentAccountName.ToLower()).ToString().Contains(search)
                   || (x.Amount == null ? 0 : x.Amount).ToString().Contains(search) || (x.Department == null ? "" : x.Department.ToLower()).ToString().Contains(search) || (x.Memo == null ? "" : x.Memo.ToLower()).ToString().Contains(search) || Convert.ToDateTime(x.InvoiceDate).ToString("MM-dd-yyyy").Equals(search)).ToList();
                }
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                }

                //int count = totalRows;
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
                logger.Error("ExpenseAccountsController - UrlDatasourceExpense - " + DateTime.Now + " - " + ex.Message.ToString());

            }

            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }

        [HttpPost]
        public async Task<ActionResult> InsertExpenseBulkTransaction(CRUDModel<ExpenseCheckSelectMain> expensevalue, int bankid, string paymentdate)
        {
            try
            {
                SuccessMessage = null;
                ErrorMessage = null;
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                int StoreId = Convert.ToInt32(Session["storeid"]);

                var vendormaster = _MastersBindRepository.GetVendorMaster(StoreId).ToList();
                var employeemaster = _MastersBindRepository.GetEmployeeMaster(StoreId).ToList();
                var departments = _ExpenseCheckSettingRepository.DepartmentDetailList(StoreId).ToList();
                var paymentmethodmaster = _ExpenseCheckSettingRepository.PaymentMethodList().ToList();

                // Remove "(Vendor)" or "(Employee)" from VendorName
                if (expensevalue?.Value?.VendorName != null)
                {
                    expensevalue.Value.VendorName = Regex.Replace(expensevalue.Value.VendorName, @"\s*\((Vendor|Employee)\)\s*$", "");
                }

                ExpenseCheck expensecheck = new ExpenseCheck
                {
                    ExpenseCheckId = expensevalue.Value.ExpenseCheckId,
                    CreatedBy = _CommonRepository.getUserId(UserName),
                    StoreId = StoreId,
                    PaymentTypeId = 2,
                    BankAccountId = Convert.ToInt32(bankid),
                    DocNumber = expensevalue.Value.DocNumber,
                    Memo = expensevalue.Value.Memo,
                    PrintLater = false,
                    TxnDate = Convert.ToDateTime(paymentdate),
                    QBErrorMessage = "Expense Created by Bulk Transaction"
                };

                // Determine VendorId and RefType
                var vendor = vendormaster.FirstOrDefault(v => v.VendorName == expensevalue.Value.VendorName);
                var employee = employeemaster.FirstOrDefault(e => e.DisplayName == expensevalue.Value.VendorName);

                if (vendor != null)
                {
                    expensecheck.VendorId = vendor.VendorId;
                    expensecheck.RefType = "V";
                    expensecheck.MailingAddress = _ExpenseCheckSettingRepository.GetPayeeMailingAddressById(vendor.VendorId);
                }
                else if (employee != null)
                {
                    expensecheck.VendorId = employee.EmployeeId;
                    expensecheck.RefType = "E";
                }

                var paymentmethod = paymentmethodmaster.FirstOrDefault(p => p.PaymentMethod == expensevalue.Value.PaymentMethodName && p.StoreId == StoreId);
                if (paymentmethod != null) { 
                    expensecheck.PaymentMethodId = paymentmethod.PaymentMethodId;
                }

                expensecheck.TotalAmt = Convert.ToDecimal(expensevalue.Value.AmountString);
                expensecheck = _ExpenseCheckSettingRepository.InsertExpenseData(expensecheck);

                if (expensecheck.ExpenseCheckId > 0)
                {
                    // Get DepartmentId
                    var department = departments.FirstOrDefault(d => d.DepartmentName == expensevalue.Value.Department);
                    int departmentId = department != null ? department.DepartmentId : 0;

                    ExpenseCheckDetail expensedetails = new ExpenseCheckDetail
                    {
                        ExpenseCheckDetailId = expensevalue.Value.ExpenseCheckDetailId,
                        ExpenseCheckId = expensecheck.ExpenseCheckId,
                        DepartmentId = departmentId,
                        Amount = Convert.ToDecimal(expensevalue.Value.AmountString)
                    };

                    _ExpenseCheckSettingRepository.InsertExpenseDetailsData(expensedetails);

                    StatusMessage = "Success BulkExpense";
                    SuccessMessage = "Expense Created Successfully";
                    ViewBag.StatusMessage = StatusMessage;
                }
                else
                {
                    ErrorMessage = "Expense Created UnSuccessfully";
                    ViewBag.StatusMessage = StatusMessage;
                }


            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("ExpenseAccountsController - InsertExpenseBulkTransaction - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return Json(new { data = expensevalue, success = SuccessMessage, Error = ErrorMessage });
        }

        [HttpPost]
        public async Task<ActionResult> UpdateExpenseBulkTransaction(CRUDModel<ExpenseCheckSelectMain> expensevalue, int bankid, string paymentdate)
        {
            try
            {
                SuccessMessage = null;
                ErrorMessage = null;
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                int StoreId = Convert.ToInt32(Session["storeid"]);

                var vendormaster = _MastersBindRepository.GetVendorMaster(StoreId).ToList();
                var employeemaster = _MastersBindRepository.GetEmployeeMaster(StoreId).ToList();
                var departments = _ExpenseCheckSettingRepository.DepartmentDetailList(StoreId).ToList();
                var paymentmethodmaster = _ExpenseCheckSettingRepository.PaymentMethodList().ToList();

                // Remove "(Vendor)" or "(Employee)" from VendorName
                if (expensevalue?.Value?.VendorName != null)
                {
                    expensevalue.Value.VendorName = Regex.Replace(expensevalue.Value.VendorName, @"\s*\((Vendor|Employee)\)\s*$", "");
                }

                ExpenseCheck expensecheck = new ExpenseCheck
                {
                    ExpenseCheckId = expensevalue.Value.ExpenseCheckId,
                    UpdatedBy = _CommonRepository.getUserId(UserName),
                    StoreId = StoreId,
                    PaymentTypeId = 2,
                    BankAccountId = Convert.ToInt32(bankid),
                    DocNumber = expensevalue.Value.DocNumber,
                    Memo = expensevalue.Value.Memo,
                    PrintLater = false,
                    TxnDate = Convert.ToDateTime(paymentdate),
                    QBErrorMessage = "Expense Created by Bulk Transaction"
                };

                // Determine VendorId and RefType
                var vendor = vendormaster.FirstOrDefault(v => v.VendorName == expensevalue.Value.VendorName);
                var employee = employeemaster.FirstOrDefault(e => e.DisplayName == expensevalue.Value.VendorName);

                if (vendor != null)
                {
                    expensecheck.VendorId = vendor.VendorId;
                    expensecheck.RefType = "V";
                    expensecheck.MailingAddress = _ExpenseCheckSettingRepository.GetPayeeMailingAddressById(vendor.VendorId);
                }
                else if (employee != null)
                {
                    expensecheck.VendorId = employee.EmployeeId;
                    expensecheck.RefType = "E";
                }

                var paymentmethod = paymentmethodmaster.FirstOrDefault(p => p.PaymentMethod == expensevalue.Value.PaymentMethodName && p.StoreId == StoreId);
                if (paymentmethod != null)
                {
                    expensecheck.PaymentMethodId = paymentmethod.PaymentMethodId;
                }

                expensecheck.TotalAmt = Convert.ToDecimal(expensevalue.Value.AmountString);
                expensecheck = _ExpenseCheckSettingRepository.InsertExpenseData(expensecheck);

                if (Convert.ToInt32(expensecheck.ExpenseCheckId) > 0)
                {
                    // Get DepartmentId
                    var department = departments.FirstOrDefault(d => d.DepartmentName == expensevalue.Value.Department);
                    int departmentId = department != null ? department.DepartmentId : 0;

                    ExpenseCheckDetail expensedetails = new ExpenseCheckDetail
                    {
                        ExpenseCheckDetailId = expensevalue.Value.ExpenseCheckDetailId,
                        ExpenseCheckId = expensecheck.ExpenseCheckId,
                        DepartmentId = departmentId,
                        Amount = Convert.ToDecimal(expensevalue.Value.AmountString)
                    };

                    _ExpenseCheckSettingRepository.InsertExpenseDetailsData(expensedetails);
                }

                StatusMessage = "Success BulkExpense";
                SuccessMessage = "Expense Updated Successfully";
                ViewBag.StatusMessage = StatusMessage;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("ExpenseAccountsController - UpdateExpenseBulkTransaction - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return Json(new { data = expensevalue, success = SuccessMessage, Error = ErrorMessage });
        }

        #endregion Bulk Transaction Expense End here
    }
}