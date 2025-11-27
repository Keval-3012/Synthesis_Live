using EntityModels.Models;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using System.Data;
using System.Data.SqlClient;
using NLog;
using System.Collections;
using Utility;
using System.Web;
using Newtonsoft.Json;
using System.Net.Http;
using System.Configuration;

namespace Repository
{
    public class CommonRepository : ICommonRepository
    {
        private DBContext _context;
        protected static bool IsArray;
        Logger logger = LogManager.GetCurrentClassLogger();

        public CommonRepository(DBContext context)
        {
            _context = context;
        }
        public int getUserId(string UserName)
        {
            int UserId = 0;
            try
            {
                if (UserName != null && UserName != "")
                {
                    UserId = _context.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserId;
                }
            }
            catch (Exception ex)
            {
                logger.Error("CommonRepository - GetUserId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return UserId;
        }
        public string getUserFirstName(string UserName)
        {
            string UFname = "";
            try
            {
                if (UserName != null && UserName != "")
                {
                    UFname = _context.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().FirstName;
                }
            }
            catch (Exception ex)
            {
                logger.Error("CommonRepository - GetUserFirstName - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return UFname;
        }
        public int getUserTypeId(string UserName)
        {
            int UTypeId = 0;
            try
            {
                if (UserName != null && UserName != "")
                {
                    UTypeId = _context.UserMasters.Where(s => s.UserName == UserName).FirstOrDefault().UserTypeId;
                }
            }
            catch (Exception ex)
            {
                logger.Error("CommonRepository - getUserTypeId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return UTypeId;
        }
        public List<StoreMaster> GetStoreList_RoleWise(int ModuleNo, string Role, string UserName)
        {
            List<StoreMaster> StoreList = new List<StoreMaster>();
            try
            {
                if (Roles.IsUserInRole("Administrator"))
                {
                    StoreList = _context.StoreMasters.Where(s => s.IsActive == true).Select(s => new { s.StoreId, s.NickName }).ToList().Select(s => new StoreMaster { StoreId = s.StoreId, NickName = s.NickName }).OrderBy(o => o.NickName).ToList();
                }
                else
                {
                    var UserId = getUserId(UserName);
                    if (UserId > 0)
                    {
                        var UserType = _context.UserMasters.Where(s => s.UserId == UserId).FirstOrDefault().UserTypeId;
                        var StoresIdsList = _context.userRoles.Where(s => s.UserTypeId == UserType && s.ModuleMasters.ModuleNo == ModuleNo && s.Role.Contains(Role)).Select(s => s.StoreId).ToList();
                        StoreList = _context.StoreMasters.Where(s => s.IsActive == true && StoresIdsList.Contains(s.StoreId)).Select(s => new { s.StoreId, s.NickName }).ToList().Select(s => new StoreMaster { StoreId = s.StoreId, NickName = s.NickName }).OrderBy(o => o.NickName).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("CommonRepository - GetStoreList_RoleWise - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return StoreList;
        }
        public string GetStoreNikeName(int StoreId)
        {
            string StoreNickName = "";
            try
            {
                if(StoreId != 0)
                {
                    StoreNickName = _context.StoreMasters.Where(a => a.StoreId == StoreId).FirstOrDefault().NickName;
                }
            }
            catch (Exception ex)
            {
                logger.Error("CommonRepository - GetStoreNikeName - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return StoreNickName;
        }
        public List<StoreMaster> GetStoreList(int ModuleNo)
        {
            List<StoreMaster> StoreList = new List<StoreMaster>();
            try
            {
                if (Roles.IsUserInRole("Administrator"))
                {
                    StoreList = _context.StoreMasters.Where(s => s.IsActive == true).Select(s => new { s.StoreId, s.NickName }).ToList().Select(s => new StoreMaster { StoreId = s.StoreId, NickName = s.NickName }).OrderBy(o => o.NickName).ToList();
                }
                else
                {
                    var UserId = UserModule.getUserId();
                    if (UserId > 0)
                    {
                        var UserType = _context.UserMasters.Where(s => s.UserId == UserId).FirstOrDefault().UserTypeId;
                        var StoresIdsList = _context.userRoles.Where(s => s.UserTypeId == UserType && s.ModuleMasters.ModuleNo == ModuleNo).Select(s => s.StoreId).ToList();
                        StoreList = _context.StoreMasters.Where(s => s.IsActive == true && StoresIdsList.Contains(s.StoreId)).Select(s => new { s.StoreId, s.NickName }).ToList().Select(s => new StoreMaster { StoreId = s.StoreId, NickName = s.NickName }).OrderBy(o => o.NickName).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("CommonRepository - GetStoreList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return StoreList;
        }
        public List<StoreMaster> GetHeaderStoreList(int ModuleNo)
        {
            List<StoreMaster> StoreList = new List<StoreMaster>();
            try
            {
                using (var context = new DBContext())
                {
                    if (Roles.IsUserInRole("Administrator"))
                    {
                        StoreList = context.StoreMasters.Where(s => s.IsActive == true).Select(s => new { s.StoreId, s.NickName }).ToList().Select(s => new StoreMaster { StoreId = s.StoreId, NickName = s.NickName }).OrderBy(o => o.NickName).ToList();
                    }
                    else
                    {
                        var UserId = UserModule.getUserId();
                        if (UserId > 0)
                        {
                            var UserType = context.UserMasters.Where(s => s.UserId == UserId).FirstOrDefault().UserTypeId;
                            var StoresIdsList = context.userRoles.Where(s => s.UserTypeId == UserType).Select(s => s.StoreId).ToList();
                            StoreList = context.StoreMasters.Where(s => s.IsActive == true && StoresIdsList.Contains(s.StoreId)).Select(s => new { s.StoreId, s.NickName }).ToList().Select(s => new StoreMaster { StoreId = s.StoreId, NickName = s.NickName }).OrderBy(o => o.NickName).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("CommonRepository - GetHeaderStoreList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return StoreList;
        }
        public string getUserFirstNameById(int UserId)
        {
            string UFname = "";
            try
            {
                if (UserId != 0)
                {
                    UFname = _context.UserMasters.Find(UserId).FirstName;
                }
            }
            catch (Exception ex)
            {
                logger.Error("CommonRepository - GetUserFirstNameById - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return UFname;
        }
        public DataTable Select(SqlCommand cmd)
        {
            DataTable dt = null;
            try
            {
                string strConn = System.Configuration.ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString;
                SqlConnection conn = new SqlConnection(strConn);
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;

                dt = new DataTable();
                SqlDataReader dr;
                conn.Open();
                dr = cmd.ExecuteReader();
                dt.Load(dr);
                conn.Close();
            }
            catch (Exception ex)
            {
                logger.Error("CommonRepository - Select - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return dt;
        }
        public int InsertProduct(Products obj)
        {
            try
            {
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                obj.CreatedBy = getUserId(UserName);
                obj.DateCreated = DateTime.Now;
                _context.products.Add(obj);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("CommonRepository - InsertProduct - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj.ProductId;
        }
        public int InsertProductVendor(ProductVendor obj)
        {
            try
            {
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                obj.CreatedBy = getUserId(UserName);
                obj.DateCreated = DateTime.Now;
                _context.productVendors.Add(obj);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("CommonRepository - InsertProductVendor - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj.ProductVendorId;
        }
        public string GetMonthName(int Month)
        {
            string MonthName = "";
            try
            {
                switch (Month)
                {
                    case 1:
                        MonthName = "January";
                        break;
                    case 2:
                        MonthName = "February";
                        break;
                    case 3:
                        MonthName = "March";
                        break;
                    case 4:
                        MonthName = "April";
                        break;
                    case 5:
                        MonthName = "May";
                        break;
                    case 6:
                        MonthName = "June";
                        break;
                    case 7:
                        MonthName = "July";
                        break;
                    case 8:
                        MonthName = "August";
                        break;
                    case 9:
                        MonthName = "September";
                        break;
                    case 10:
                        MonthName = "October";
                        break;
                    case 11:
                        MonthName = "November";
                        break;
                    case 12:
                        MonthName = "December";
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error("CommonRepository - GetMonthName - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return MonthName;
        }
        public string GetMessageValue(string strKey, string Defaultvalue = "")
        {
            string strValue = "";
            try
            {
                DataTable dtSetting = new DataTable();
                Hashtable Hdtsetting = new Hashtable();
                if (System.Web.HttpContext.Current.Application["MessageData"] == null)
                {
                    _context = new DBContext();
                    List<MessageMaster> mm = new List<MessageMaster>();
                    mm = _context.MessageMasters.ToList();
                    dtSetting = Common.LINQToDataTable(mm);
                    if (dtSetting.Rows.Count > 0)
                    {
                        for (int s = 0; s < dtSetting.Rows.Count; s++)
                        {
                            Hdtsetting.Add(Convert.ToString(dtSetting.Rows[s]["KeyNum"].ToString().Trim()), Convert.ToString(dtSetting.Rows[s]["ValueStr"]));
                        }
                        System.Web.HttpContext.Current.Application["MessageData"] = Hdtsetting;
                    }
                }
                System.Collections.Hashtable dt = System.Web.HttpContext.Current.Application["MessageData"] as System.Collections.Hashtable;
                if (dt != null)
                {
                    if (Convert.ToString(dt[strKey]) != "")
                    {
                        strValue = Convert.ToString(dt[strKey]);
                    }
                    else
                    {
                        strValue = Defaultvalue;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("CommonRepository - GetMessageValue - " + DateTime.Now + " - " + ex.Message.ToString());
            } 
            return strValue;
        }

        public List<UserMaster> GetUserList()
        {
            List<UserMaster> userlist = new List<UserMaster>();
            try
            {
                userlist = _context.UserMasters.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("CommonRepository - GetUserList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return userlist;
        }

        #region Updated By Dani on 08-15-2024 

        #region Module Log Entries
        public void LogEntries()
        {
            string action = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("action");
            string Controller = System.Web.HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("Controller");
            Uri referrerUri = System.Web.HttpContext.Current.Request.UrlReferrer;
            string referrer = referrerUri != null ? referrerUri.AbsoluteUri : null;
            Uri CurrentReq = System.Web.HttpContext.Current.Request.Url;
            string Current = CurrentReq != null ? CurrentReq.AbsoluteUri : null;
            if (referrer == null)
            {
                referrer = HttpContext.Current.Session["SessionURL"].ToString();
            }
            else if (referrer != null && HttpContext.Current.Session["SessionURL"] != null)
            {
                if (referrer != null && Current == HttpContext.Current.Session["SessionURL"].ToString())
                {
                    referrer = Current;
                }
                else
                {
                    HttpContext.Current.Session["SessionURL"] = Current;
                }
            }
            else
            {
                HttpContext.Current.Session["SessionURL"] = Current;
            }
            bool? isReload = !string.IsNullOrEmpty(referrer) && referrer.Equals(System.Web.HttpContext.Current.Request.Url.AbsoluteUri, StringComparison.OrdinalIgnoreCase);
            LogEntry(Controller, action, isReload);
        }
        public clsActivityLog GetActivityDetails(string controllerName, string actionName)
        {
            clsActivityLog clsActivityLog = new clsActivityLog();
            try
            {
                switch (controllerName.ToUpper())
                {
                    case "DASHBOARD":
                        switch (actionName.ToUpper())
                        {
                            case "DAILY":
                                clsActivityLog.ModuleName = "Dashboard";
                                clsActivityLog.PageName = "Daily Dashbord";
                                break;
                            case "WEEKLY":
                                clsActivityLog.ModuleName = "Dashboard";
                                clsActivityLog.PageName = "Weekly Dashbord";
                                break;
                            case "PERIODIC":
                                clsActivityLog.ModuleName = "Dashboard";
                                clsActivityLog.PageName = "Periodic Dashbord";
                                break;
                            case "YEARLY":
                                clsActivityLog.ModuleName = "Dashboard";
                                clsActivityLog.PageName = "Yearly Dashbord";
                                break;
                            default:
                                // code block
                                break;
                        }
                        break;
                    case "INVOICES":
                        switch (actionName.ToUpper())
                        {
                            //case "INDEX":
                            //    clsActivityLog.ModuleName = "Invoice";
                            //    clsActivityLog.PageName = "Invoices Data";
                            //    break;
                            case "INDEXBETA":
                                clsActivityLog.ModuleName = "Invoice";
                                clsActivityLog.PageName = "View Invoices";
                                break;
                            case "CREATE":
                                clsActivityLog.ModuleName = "Invoice";
                                clsActivityLog.PageName = "Add Invoice";
                                break;
                            case "CREATESPLITINVOICE":
                                clsActivityLog.ModuleName = "Invoice";
                                clsActivityLog.PageName = "Add Split Invoice";
                                break;
                            case "VIEWINVOICE":
                                clsActivityLog.ModuleName = "Invoice";
                                clsActivityLog.PageName = "Invoice File Read";
                                break;
                            default:
                                // code block
                                break;
                        }
                        // code block
                        break;
                    case "PRODUCTPRICE":
                        switch (actionName.ToUpper())
                        {
                            case "PRODUCTPRICE":
                                clsActivityLog.ModuleName = "Dashboard";
                                clsActivityLog.PageName = "Cost Price Comparison";
                                break;
                            default:
                                // code block
                                break;
                        }
                        // code block
                        break;
                    case "SELLPRICE":
                        switch (actionName.ToUpper())
                        {
                            case "SELLPRICE":
                                clsActivityLog.ModuleName = "Dashboard";
                                clsActivityLog.PageName = "Sales Price Comparison";
                                break;
                            default:
                                // code block
                                break;
                        }
                        // code block
                        break;
                    case "TOPSELLERPRICE":
                        switch (actionName.ToUpper())
                        {
                            case "TOPSELLERPRICE":
                                clsActivityLog.ModuleName = "Dashboard";
                                clsActivityLog.PageName = "Top Seller Items";
                                break;
                            default:
                                // code block
                                break;
                        }
                        // code block
                        break;
                    case "BULKUPLOADFILE":
                        switch (actionName.ToUpper())
                        {
                            case "INDEX":
                                clsActivityLog.ModuleName = "Invoice";
                                clsActivityLog.PageName = "Add Bulk Invoice";
                                break;
                            default:
                                // code block
                                break;
                        }
                        break;
                    case "PRODUCTMAPPINGS":
                        switch (actionName.ToUpper())
                        {
                            case "IMPORTPRODUCTEXCEL":
                                clsActivityLog.ModuleName = "Inventory";
                                clsActivityLog.PageName = "Items Library";
                                break;
                            default:
                                // code block
                                break;
                        }
                        // code block
                        break;
                    case "INVOICEPREVIEW":
                        switch (actionName.ToUpper())
                        {
                            case "INDEX":
                                clsActivityLog.ModuleName = "Inventory";
                                clsActivityLog.PageName = "Vendor Product Lists";
                                break;
                            default:
                                // code block
                                break;
                        }
                        break;
                    case "LINEITEM":
                        switch (actionName.ToUpper())
                        {
                            case "INDEX":
                                clsActivityLog.ModuleName = "Inventory";
                                clsActivityLog.PageName = "Line Items";
                                break;
                            default:
                                // code block
                                break;
                        }
                        break;
                    case "ITEMMOVEMENT":
                        switch (actionName.ToUpper())
                        {
                            case "INDEX":
                                clsActivityLog.ModuleName = "Inventory";
                                clsActivityLog.PageName = "Items Movement Reports";
                                break;
                            default:
                                // code block
                                break;
                        }
                        break;
                    case "EXPENSEACCOUNTS":
                        switch (actionName.ToUpper())
                        {
                            case "EXPENSECHECKINDEXNEW":
                                clsActivityLog.ModuleName = "Expense";
                                clsActivityLog.PageName = "Expense";
                                break;
                            default:
                                // code block
                                break;
                        }
                        break;
                    case "EXPENSEWEEKLYSETTING":
                        switch (actionName.ToUpper())
                        {
                            case "HOMEEXPENSEINDEXNEW":
                                clsActivityLog.ModuleName = "Expense";
                                clsActivityLog.PageName = "Home some Expenses";
                                break;
                            default:
                                // code block
                                break;
                        }
                        break;
                    case "CHECKLISTEXPENSE":
                        switch (actionName.ToUpper())
                        {
                            case "CHECKLISTINDEXNEW":
                                clsActivityLog.ModuleName = "Expense";
                                clsActivityLog.PageName = "Uncleared Checks";
                                break;
                            default:
                                // code block
                                break;
                        }
                        break;
                    case "PDFREAD":
                        switch (actionName.ToUpper())
                        {
                            case "CREATE":
                                clsActivityLog.ModuleName = "HR";
                                clsActivityLog.PageName = "Payroll Files";
                                break;
                            default:
                                // code block
                                break;
                        }
                        break;
                    case "MANAGEIPADDRESS":
                        switch (actionName.ToUpper())
                        {
                            case "USERTIMEHOURSE":
                                clsActivityLog.ModuleName = "HR";
                                clsActivityLog.PageName = "Employee Timecards";
                                break;
                            default:
                                // code block
                                break;
                        }
                        break;
                    case "REPORT":
                        switch (actionName.ToUpper())
                        {
                            case "SALESSUMMARYREPORT":
                                clsActivityLog.ModuleName = "Report";
                                clsActivityLog.PageName = "Sales Summary Report";
                                break;
                            case "OPERATINGRATIOREPORT":
                                clsActivityLog.ModuleName = "Report";
                                clsActivityLog.PageName = "Operating Ratio Report";
                                break;
                            case "PAYROLLANALYSISREPORT":
                                clsActivityLog.ModuleName = "Report";
                                clsActivityLog.PageName = "Payroll Analysis Report";
                                break;
                            case "DAILYPOSFEEDS":
                                clsActivityLog.ModuleName = "Registers";
                                clsActivityLog.PageName = "Daily POS Feeds";
                                break;
                            case "PAYROLLEXPENSE":
                                clsActivityLog.ModuleName = "HR";
                                clsActivityLog.PageName = "Payroll Expenses";
                                break;
                            default:
                                // code block
                                break;
                        }
                        break;
                    case "INVOICEREPORT":
                        switch (actionName.ToUpper())
                        {
                            case "SALESSUMMARYREPORT":
                                clsActivityLog.ModuleName = "Report";
                                clsActivityLog.PageName = "COGS/Bills Report";
                                break;
                            case "INDEX":
                                clsActivityLog.ModuleName = "Report";
                                clsActivityLog.PageName = "COGS/Bills Report";
                                break;
                            default:
                                // code block
                                break;
                        }
                        break;
                    case "EXPENSEREPORT":
                        switch (actionName.ToUpper())
                        {
                            case "INDEX":
                                clsActivityLog.ModuleName = "Report";
                                clsActivityLog.PageName = "Expense Report";
                                break;
                            default:
                                // code block
                                break;
                        }
                        break;
                    case "CHARTOFACCOUNTS":
                        switch (actionName.ToUpper())
                        {
                            case "INDEX":
                                clsActivityLog.ModuleName = "QuickBooks";
                                clsActivityLog.PageName = "Departments";
                                break;
                            default:
                                // code block
                                break;
                        }
                        break;
                    case "DELETEHOURLYFILES":
                        switch (actionName.ToUpper())
                        {
                            case "INDEX":
                                clsActivityLog.ModuleName = "Registers";
                                clsActivityLog.PageName = "Hourly POS feeds";
                                break;
                            default:
                                // code block
                                break;
                        }
                        break;
                    case "DOCUMENTS":
                        switch (actionName.ToUpper())
                        {
                            case "INDEX":
                                clsActivityLog.ModuleName = "Documents";
                                clsActivityLog.PageName = "Documents";
                                break;
                            default:
                                // code block
                                break;
                        }
                        break;
                    case "TERMINAL":
                        switch (actionName.ToUpper())
                        {
                            case "INDEX":
                                clsActivityLog.ModuleName = "Registers";
                                clsActivityLog.PageName = "Shifts/Registers Close Out";
                                break;
                            default:
                                // code block
                                break;
                        }
                        break;
                    case "JOURNALENTRIES":
                        switch (actionName.ToUpper())
                        {
                            case "INDEX":
                                clsActivityLog.ModuleName = "Registers";
                                clsActivityLog.PageName = "Journal Entries";
                                break;
                            default:
                                // code block
                                break;
                        }
                        break;
                    case "HREMPLOYEE":
                        switch (actionName.ToUpper())
                        {
                            case "INDEX":
                                clsActivityLog.ModuleName = "HR";
                                clsActivityLog.PageName = "Employee List";
                                break;
                            default:
                                // code block
                                break;
                        }
                        break;
                    case "HRCONSENTMASTERS":
                        switch (actionName.ToUpper())
                        {
                            case "INDEX":
                                clsActivityLog.ModuleName = "HR";
                                clsActivityLog.PageName = "Consent Status";
                                break;
                            default:
                                // code block
                                break;
                        }
                        break;
                    case "HRSTOREMANAGERS":
                        switch (actionName.ToUpper())
                        {
                            case "INDEX":
                                clsActivityLog.ModuleName = "HR";
                                clsActivityLog.PageName = "Mobile App User";
                                break;
                            default:
                                // code block
                                break;
                        }
                        break;
                    case "HRDEPARTMENTMASTERS":
                        switch (actionName.ToUpper())
                        {
                            case "INDEX":
                                clsActivityLog.ModuleName = "HR";
                                clsActivityLog.PageName = "Store Departments";
                                break;
                            default:
                                // code block
                                break;
                        }
                        break;
                    case "VENDORMASTERS":
                        switch (actionName.ToUpper())
                        {
                            case "INDEX":
                                clsActivityLog.ModuleName = "QuickBooks";
                                clsActivityLog.PageName = "Vendors";
                                break;
                            case "MERGEVENDOR":
                                clsActivityLog.ModuleName = "QuickBooks";
                                clsActivityLog.PageName = "Merge Vendors";
                                break;
                            default:
                                // code block
                                break;
                        }
                        break;
                    case "CUSTOMERINFORMATION":
                        switch (actionName.ToUpper())
                        {
                            case "INDEX":
                                clsActivityLog.ModuleName = "CRM";
                                clsActivityLog.PageName = "Customers Information";
                                break;
                            case "CUSTOMERRECIPT":
                                clsActivityLog.ModuleName = "CRM";
                                clsActivityLog.PageName = "Customers Receipts";
                                break;
                            default:
                                // code block
                                break;
                        }
                        break;
                    case "QBCONFIGURATION":
                        switch (actionName.ToUpper())
                        {
                            case "QBSYNCONLINEDATA":
                                clsActivityLog.ModuleName = "QuickBooks";
                                clsActivityLog.PageName = "QuickBooks sync start/stop";
                                break;
                            case "INDEX":
                                clsActivityLog.ModuleName = "QuickBooks";
                                clsActivityLog.PageName = "QuickBooks Configuration";
                                break;
                            default:
                                // code block
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error("SynthesisApiRepository - GetActivityDetails - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return clsActivityLog;
        }
        public void LogEntry(string controller, string action, bool? IsReload = null)
        {
            try
            {
                clsActivityLog clsActivityLog = new clsActivityLog();
                if (controller != null && action != null)
                {
                    clsActivityLog = GetActivityDetails(controller, action);
                    if (clsActivityLog.ModuleName != null)
                    {
                        if (HttpContext.Current.Session != null && HttpContext.Current.Session["UserId"] != null)
                        {
                            if (IsReload != null)
                            {
                                if (IsReload == true)
                                {
                                    clsActivityLog.Message = "Reload " + clsActivityLog.PageName;
                                    clsActivityLog.CreatedBy = Convert.ToInt32(HttpContext.Current.Session["UserId"]);
                                    clsActivityLog.Action = "Reload";
                                    CreateLog(clsActivityLog);
                                }
                                else
                                {
                                    clsActivityLog.Message = "Opened " + clsActivityLog.PageName;
                                    clsActivityLog.CreatedBy = Convert.ToInt32(HttpContext.Current.Session["UserId"]);
                                    clsActivityLog.Action = "Opened";
                                    CreateLog(clsActivityLog);
                                }
                            }
                            else
                            {
                                clsActivityLog.Message = "Opened " + clsActivityLog.PageName;
                                clsActivityLog.CreatedBy = Convert.ToInt32(HttpContext.Current.Session["UserId"]);
                                clsActivityLog.Action = "Opened";
                                CreateLog(clsActivityLog);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("SynthesisApiRepository - LogEntry - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public void CreateLog(clsActivityLog clsActivityLog)
        {
            try
            {
                if (clsActivityLog.PageName != "" && clsActivityLog.ModuleName != "" && clsActivityLog.Action != "")
                {
                    var jsonString = JsonConvert.SerializeObject(clsActivityLog);
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, ConfigurationManager.AppSettings["ApiUrl"].ToString() + "/api/Log/Write");
                    request.Headers.Add("Authorization", $"Bearer {Convert.ToString(HttpContext.Current.Session["UserAccessTokan"])}");
                    var content = new StringContent(jsonString, null, "application/json");
                    request.Content = content;
                    var response = client.SendAsync(request);
                }
            }
            catch (Exception ex)
            {
                logger.Error("SynthesisApiRepository - CreateLog - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        #endregion
        #endregion
    }

}
