using EntityModels.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Repository;
using Repository.IRepository;
using SynthesisViewModal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utility;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.Reflection;
using Org.BouncyCastle.Crypto;
using NLog;
using System.Globalization;

namespace SysnthesisRepo.Controllers
{
    public class ExpenseReportController : Controller
    {
        // GET: Admin/InvoiceReport
        protected static string StatusMessage = "";
        protected static string ActivityLogMessage = "";
        protected static string DeleteMessage = "";
        private const int FirstPageIndex = 1;
        protected static int TotalDataCount;
        protected static Array Arr;
        protected static bool IsArray;
        protected static IEnumerable BindData;
        protected static bool IsEdit = false;
        protected static string strdashbordsuccess;
        private readonly ISynthesisApiRepository _synthesisApiRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly IExpenseReportRepository _expenseReportRepository;
        private readonly IChartofAccountsRepository _departmentMastersRepository;
        private readonly IVendorMasterRepository _vendorMasterRepository;
                Logger logger = LogManager.GetCurrentClassLogger();

        public ExpenseReportController()
        {
            this._synthesisApiRepository = new SynthesisApiRepository();
            this._commonRepository = new CommonRepository(new DBContext());
            this._expenseReportRepository = new ExpenseReportRepository(new DBContext());
            this._departmentMastersRepository = new ChartofAccountsRepository(new DBContext());
            this._vendorMasterRepository = new VendorMasterRepository(new DBContext());
        }
        /// <summary>
        /// This method is return view of Expense report
        /// </summary>
        /// <param name="dashbordsuccess"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,ViewExpenseReport")]
        public ActionResult Index(string dashbordsuccess)
        {
            try
            {
                ViewBag.Title = "Expenses Report - Synthesis";
                _commonRepository.LogEntries();     //Harsh's code
                strdashbordsuccess = dashbordsuccess;

                if (!string.IsNullOrEmpty(Session["storeid"] as string))
                {
                    string store_idval = Session["storeid"].ToString();
                    ViewBag.Storeidvalue = store_idval;
                }
                if (Session["storeid"] == null)
                {
                    Session["storeid"] = "0";
                }
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseReportController - GetBankDetailsByID - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View();
        }
        //  [OutputCache(Duration = 0)]
        /// <summary>
        /// This method is get Expense Grid data
        /// </summary>
        /// <param name="IsBindData"></param>
        /// <param name="currentPageIndex"></param>
        /// <param name="orderby"></param>
        /// <param name="IsAsc"></param>
        /// <param name="PageSize"></param>
        /// <param name="SearchRecords"></param>
        /// <param name="Alpha"></param>
        /// <param name="deptname"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="payment"></param>
        /// <param name="Store_val"></param>
        /// <param name="Status"></param>
        /// <param name="VendorName"></param>
        /// <param name="AmtMaximum"></param>
        /// <param name="AmtMinimum"></param>
        /// <returns></returns>
        public ActionResult Grid(int IsBindData = 1, int currentPageIndex = 1, string orderby = "InvoiceId", int IsAsc = 0, int PageSize = 100, int SearchRecords = 1, string Alpha = "", int deptname = 0, string startdate = "", string enddate = "", string payment = "", string Store_val = "", int Status = 0, int VendorName = 0, decimal AmtMaximum = 0, decimal AmtMinimum = 0)
        {
            ExpenseReportViewModal expenseReportViewModal = new ExpenseReportViewModal();
            expenseReportViewModal.IsBindData = IsBindData;
            expenseReportViewModal.currentPageIndex = currentPageIndex;
            expenseReportViewModal.orderby = orderby;
            expenseReportViewModal.IsAsc = IsAsc;
            expenseReportViewModal.PageSize = PageSize;
            expenseReportViewModal.SearchRecords = SearchRecords;
            expenseReportViewModal.Alpha = Alpha;
            expenseReportViewModal.deptname = deptname;
            expenseReportViewModal.startdate = startdate;
            expenseReportViewModal.enddate = enddate;
            expenseReportViewModal.payment = payment;
            expenseReportViewModal.Store_val = Store_val;
            expenseReportViewModal.Status = Status;
            expenseReportViewModal.VendorName = VendorName;
            expenseReportViewModal.AmtMaximum = AmtMaximum;
            expenseReportViewModal.AmtMinimum = AmtMinimum;
            int storeid = 0;
            var StoreIdSsn = Session["storeid"];
            if (StoreIdSsn != null)
            {
                if (!string.IsNullOrEmpty(Session["storeid"].ToString()))
                {

                    string store_idval = Session["storeid"].ToString();
                    ViewBag.Storeidvalue = store_idval;
                    if (store_idval != "0")
                    {
                        storeid = Convert.ToInt32(store_idval);
                        //This db class get Store Nike Name by StoreId
                        var storename = _commonRepository.GetStoreNikeName(storeid);
                        ViewBag.StoreNamevalue = storename;
                    }
                    else
                    {
                        ViewBag.StoreNamevalue = "All Stores";
                    }
                }
            }

            #region MyRegion_Array
            try
            {
                if (IsArray == true)
                {
                    foreach (string a1 in Arr)
                    {
                        if (a1.Split(':')[0].ToString() == "IsBindData")
                        {
                            expenseReportViewModal.IsBindData = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "currentPageIndex")
                        {
                            expenseReportViewModal.currentPageIndex = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "orderby")
                        {
                            expenseReportViewModal.orderby = Convert.ToString(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "IsAsc")
                        {
                            expenseReportViewModal.IsAsc = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "PageSize")
                        {
                            expenseReportViewModal.PageSize = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "SearchRecords")
                        {
                            expenseReportViewModal.SearchRecords = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "Alpha")
                        {
                            expenseReportViewModal.Alpha = Convert.ToString(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "deptname")
                        {
                            expenseReportViewModal.deptname = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "startdate")
                        {
                            expenseReportViewModal.startdate = Convert.ToString(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "enddate")
                        {
                            expenseReportViewModal.enddate = Convert.ToString(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "payment")
                        {
                            expenseReportViewModal.payment = Convert.ToString(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "Store_val")
                        {
                            expenseReportViewModal.Store_val = Convert.ToString(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "Status")
                        {
                            expenseReportViewModal.Status = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "VendorName")
                        {
                            expenseReportViewModal.VendorName = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "AmtMinimum")
                        {
                            expenseReportViewModal.AmtMinimum = Convert.ToDecimal(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "AmtMaximum")
                        {
                            expenseReportViewModal.AmtMaximum = Convert.ToDecimal(a1.Split(':')[1].ToString());
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                logger.Error("ExpenseReportController - Grid - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            IsArray = false;
            Arr = new string[]
            {  "IsBindData:" + expenseReportViewModal.IsBindData
                ,"currentPageIndex:" + expenseReportViewModal.currentPageIndex
                ,"orderby:" + expenseReportViewModal.orderby
                ,"IsAsc:" + expenseReportViewModal.IsAsc
                ,"PageSize:" + expenseReportViewModal.PageSize
                ,"SearchRecords:" + expenseReportViewModal.SearchRecords
                ,"Alpha:" + expenseReportViewModal.Alpha
              //  ,"SearchTitle:" + SearchTitle
               ,"deptname:" + expenseReportViewModal.deptname
               ,"startdate:"+ expenseReportViewModal.startdate
                ,"enddate:"+ expenseReportViewModal.enddate
                ,"payment:"+ expenseReportViewModal.payment
                ,"Store_val:"+ expenseReportViewModal.Store_val
                ,"Status:"+ expenseReportViewModal.Status
                ,"VendorName:"+ expenseReportViewModal.VendorName
                ,"AmtMaximum:"+ expenseReportViewModal.AmtMaximum
                ,"AmtMinimum:"+ expenseReportViewModal.AmtMinimum
            };
            #endregion

            #region MyRegion_BindData
           
            int startIndex = ((expenseReportViewModal.currentPageIndex - 1) * expenseReportViewModal.PageSize) + 1;
            int endIndex = startIndex + expenseReportViewModal.PageSize - 1;
            IEnumerable Data = null;
            try
            {
                if (expenseReportViewModal.IsBindData == 1 || IsEdit == true)
                {
                    expenseReportViewModal.payment = expenseReportViewModal.payment == "Expense" ? "2" : (expenseReportViewModal.payment == "Check" ? "1" : "");
                    BindData = GetData(expenseReportViewModal).OfType<ExpenseCheckSelect>().ToList();
                    TotalDataCount = BindData.OfType<ExpenseCheckSelect>().ToList().Count();
                }

                if (TotalDataCount == 0)
                {
                    StatusMessage = "NoItem";
                }

                ViewBag.IsBindData = expenseReportViewModal.IsBindData;
                ViewBag.CurrentPageIndex = expenseReportViewModal.currentPageIndex;
                ViewBag.LastPageIndex = this.getLastPageIndex(expenseReportViewModal.PageSize);
                ViewBag.OrderByVal = expenseReportViewModal.orderby;
                ViewBag.IsAscVal = expenseReportViewModal.IsAsc;
                ViewBag.PageSize = expenseReportViewModal.PageSize;
                ViewBag.Alpha = expenseReportViewModal.Alpha;
                ViewBag.SearchRecords = expenseReportViewModal.SearchRecords;
                //ViewBag.SearchTitle = SearchTitle;
                //ViewBag.StatusMessage = StatusMessage;
                if (strdashbordsuccess == "SuccessEdit" || strdashbordsuccess == "Exists" || strdashbordsuccess == "InvalidImage")
                {
                    ViewBag.StatusMessage = strdashbordsuccess;
                    strdashbordsuccess = "";
                }
                else
                {
                    ViewBag.StatusMessage = StatusMessage;
                }
                ViewBag.startindex = startIndex;
                ViewBag.deptname = expenseReportViewModal.deptname;
                ViewBag.startdate = expenseReportViewModal.startdate;
                ViewBag.enddate = expenseReportViewModal.enddate;
                ViewBag.payment = expenseReportViewModal.payment;
                ViewBag.Store_val = expenseReportViewModal.Store_val;
                ViewBag.Status = expenseReportViewModal.Status;
                ViewBag.VendorName = expenseReportViewModal.VendorName;
                ViewBag.AmtMaximum = expenseReportViewModal.AmtMaximum;
                ViewBag.AmtMinimum = expenseReportViewModal.AmtMinimum;

                //Print Headers Display Data 
                ViewBag.DepartmentNameDisp = "";
                if (expenseReportViewModal.deptname != 0)
                {
                    //Get DepartMent Master List
                    ViewBag.DepartmentNameDisp = _departmentMastersRepository.DepartmentMasterList().Where(s => s.DepartmentId == expenseReportViewModal.deptname).FirstOrDefault().DepartmentName;
                }
                ViewBag.StatusDisp = "";
                if (expenseReportViewModal.Status != 0)
                {
                    ViewBag.StatusDisp = expenseReportViewModal.Status == 1 ? "Pending" : expenseReportViewModal.Status == 2 ? "Approved" : expenseReportViewModal.Status == 3 ? "Rejected" : "OnHold";
                }
                ViewBag.VendorNameDisp = "";
                if (expenseReportViewModal.VendorName != 0)
                {
                    //This class is get vendor master by Id
                    ViewBag.VendorNameDisp = _vendorMasterRepository.GetVendormasterById().Where(s => s.VendorId == expenseReportViewModal.VendorName).FirstOrDefault().VendorName;
                }
                //--------------------------------------
                if (TotalDataCount < endIndex)
                {
                    ViewBag.endIndex = TotalDataCount;
                }
                else
                {
                    ViewBag.endIndex = endIndex;
                }
                ViewBag.TotalDataCount = TotalDataCount;
                var ColumnName = typeof(ExpenseCheckSelect).GetProperties().Where(p => p.Name == expenseReportViewModal.orderby).FirstOrDefault();

                
                if (expenseReportViewModal.IsAsc == 1)
                {
                    ViewBag.AscVal = 0;
                    Data = BindData.OfType<ExpenseCheckSelect>().ToList().OrderBy(n => ColumnName.GetValue(n, null)).Skip(startIndex - 1).Take(expenseReportViewModal.PageSize);
                }
                else
                {

                    ViewBag.AscVal = 1;
                    Data = BindData.OfType<ExpenseCheckSelect>().ToList().Skip(startIndex - 1).Take(expenseReportViewModal.PageSize);
                }
                ViewBag.TotalInvoiceType = BindData.OfType<ExpenseCheckSelect>().ToList().Skip(startIndex - 1).Take(expenseReportViewModal.PageSize).Select(x => x.Amount).Sum();

                var enumData = from InvoiceStatusEnm e in Enum.GetValues(typeof(InvoiceStatusEnm))
                               select new
                               {
                                   ID = (int)e,
                                   Name = e.ToString()
                               };
                ViewBag.EnumList = new SelectList(enumData, "ID", "Name");
                var UserTypeId = UserModule.getUserTypeId();
                //This class is get Expense department by Id
                var ExpenseDeptIds = _expenseReportRepository.GetExpenseDeptIds().Where(s => s.DepartmentId != null).Select(s => s.DepartmentId.Value).Distinct().ToList();
                //Db class is Get User type master
                if (_expenseReportRepository.GetUserTypeMasters().Any(s => s.UserTypeId == UserTypeId && s.IsViewInvoiceOnly == true))
                {
                    //Get Rights Store by usertypeid  
                    var DeptRightsList = _expenseReportRepository.GetRightsStore().Where(s => s.UserTypeId == UserTypeId && s.StoreId == storeid).Select(k => k.DepartmentId).ToList();
                    //This class is get Expense department by Id
                    ExpenseDeptIds = _expenseReportRepository.GetExpenseDeptIds().Where(s => DeptRightsList.Contains(s.DepartmentId) && s.DepartmentId != null).Select(s => s.DepartmentId.Value).Distinct().ToList();
                }
                //Get DepartMent Master List
                ViewBag.FltDepartmentId = new SelectList(_departmentMastersRepository.DepartmentMasterList().Where(s => s.StoreId == storeid && ExpenseDeptIds.Contains(s.DepartmentId)).OrderBy(o => o.DepartmentName).ToList(), "DepartmentId", "DepartmentName");
                //This class is get vendor master by Id
                ViewBag.FltVendorId = new SelectList(_vendorMasterRepository.GetVendormasterById().Where(a => a.StoreId == storeid).Select(s => new { s.VendorId, s.VendorName }), "VendorId", "VendorName");
                StatusMessage = "";
                if (expenseReportViewModal.startdate != "" || expenseReportViewModal.enddate != "" || expenseReportViewModal.payment != "" || ViewBag.DepartmentNameDisp != "" || ViewBag.StatusDisp != "" || ViewBag.VendorNameDisp != "" || expenseReportViewModal.AmtMinimum != 0 || expenseReportViewModal.AmtMaximum != 0)
                {
                    clsActivityLog clsActivityLog = new clsActivityLog();
                    clsActivityLog.Action = "Click";
                    clsActivityLog.ModuleName = "Report";
                    clsActivityLog.PageName = "Expense Report";
                    clsActivityLog.Message = " Expense Report Generated for " + (expenseReportViewModal.startdate == "" ? "" : "From Date : ") + expenseReportViewModal.startdate + (expenseReportViewModal.startdate == "" ? "" : " - To Date : ") + expenseReportViewModal.enddate + (expenseReportViewModal.payment == "" ? "" : " ,Expense Type : ") + expenseReportViewModal.payment + (ViewBag.DepartmentNameDisp == "" ? "" : " , Departments : ") + ViewBag.DepartmentNameDisp + (ViewBag.StatusDisp == "" ? "" : " ,Status : ") + ViewBag.StatusDisp + (ViewBag.VendorNameDisp == "" ? "" : " ,Vendor Name : ") + ViewBag.VendorNameDisp + (expenseReportViewModal.AmtMinimum == 0 ? "" : " ,Min Amount : ") + (expenseReportViewModal.AmtMinimum == 0 ? "" : Convert.ToString(expenseReportViewModal.AmtMinimum)) + (expenseReportViewModal.AmtMaximum == 0 ? "" : " ,Max Amount : ") + (expenseReportViewModal.AmtMaximum == 0 ? "" : Convert.ToString(expenseReportViewModal.AmtMaximum));
                    clsActivityLog.CreatedBy = Convert.ToInt32(Session["UserID"]);
                    _synthesisApiRepository.CreateLog(clsActivityLog);
                }
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseReportController - Grid - " + DateTime.Now + " - " + ex.Message.ToString());
            }
   
            return View(Data);
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
            try
            {
                if (TotalDataCount % PageSize > 0)
                {
                    lastPageIndex += 1;
                }
            }
            catch (Exception ex)
            {
                logger.Error("ExpenseReportController - getLastPageIndex - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           

            return lastPageIndex;
        }
        /// <summary>
        /// This method is get Expense Check Select data
        /// </summary>
        /// <param name="expenseReportViewModal"></param>
        /// <returns></returns>
        private IEnumerable GetData(ExpenseReportViewModal expenseReportViewModal)
        {
            string userid = UserModule.getUserId().ToString();
            
            try
            {
                expenseReportViewModal.start_date = DateTime.ParseExact(expenseReportViewModal.startdate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            }
            catch { }

            try
            {
                expenseReportViewModal.end_date = DateTime.ParseExact(expenseReportViewModal.enddate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            }
            catch { }
            IEnumerable RtnData = null;
            try
            {
                // string storeid = GlobalStore.GlobalStore_id;
                string storeid = "";
                if (!string.IsNullOrEmpty(Convert.ToString(Session["storeid"])))
                {
                    storeid = Convert.ToString(Session["storeid"]);
                    if (storeid != "")
                    {
                        expenseReportViewModal.istoreID = Convert.ToInt32(storeid);
                    }
                }
                else
                {
                    RedirectToAction("index", "Login");
                }
                //This method is get expense check select
                RtnData = _expenseReportRepository.GetExpenseCheckSelect(expenseReportViewModal);
            }
            catch (Exception ex)
            {
                AdminSiteConfiguration.WriteErrorLogs("Controller :ExpenseReport Method : ExpenseReportGetData Message:" + ex.Message + "Internal Message:" + ex.InnerException);
            }
            return RtnData;
        }
        /// <summary>
        /// This method is Export pdf of Expense report 
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="payment"></param>
        /// <param name="deptname"></param>
        /// <param name="Status"></param>
        /// <param name="VendorName"></param>
        /// <param name="AmtMaximum"></param>
        /// <param name="AmtMinimum"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,DownloadExpenseReport")]
        public ActionResult ExportPDFData(string startdate = "", string enddate = "", string payment = "", int deptname = 0, int Status = 0, int VendorName = 0, decimal AmtMaximum = 0, decimal AmtMinimum = 0)
        {
            ExpenseReportViewModal expenseReportViewModal = new ExpenseReportViewModal();
            expenseReportViewModal.deptname = deptname;
            expenseReportViewModal.startdate = startdate;
            expenseReportViewModal.enddate = enddate;
            expenseReportViewModal.payment = payment;
            expenseReportViewModal.Status = Status;
            expenseReportViewModal.VendorName = VendorName;
            expenseReportViewModal.AmtMaximum = AmtMaximum;
            expenseReportViewModal.AmtMinimum = AmtMinimum;
            try
            {
                if (!string.IsNullOrEmpty(expenseReportViewModal.startdate))
                    expenseReportViewModal.start_date = DateTime.ParseExact(expenseReportViewModal.startdate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch { }

            try
            {
                if (!string.IsNullOrEmpty(expenseReportViewModal.enddate))
                    expenseReportViewModal.end_date = DateTime.ParseExact(expenseReportViewModal.enddate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch { }
            string storeid = "";
            if (!string.IsNullOrEmpty(Convert.ToString(Session["storeid"])))
            {
                storeid = Convert.ToString(Session["storeid"]);
                expenseReportViewModal.istoreID = Convert.ToInt32(Session["storeid"]);
            }
            else
            {
                RedirectToAction("index", "login");
            }
            try
            {
                System.Data.DataTable dt1 = new System.Data.DataTable();
                //This method is get expense check select
                var data1 = _expenseReportRepository.GetExpenseCheckSelect(expenseReportViewModal);

                //if (expenseReportViewModal.start_date != null && !string.IsNullOrEmpty(Convert.ToString(expenseReportViewModal.start_date)))
                //{
                //    ViewBag.expenseReportViewModal.start_date = expenseReportViewModal.start_date.Value.ToString("dd/MM/yyyy");
                //}
                //if (expenseReportViewModal.end_date != null && !string.IsNullOrEmpty(Convert.ToString(expenseReportViewModal.end_date)))
                //{
                //    ViewBag.expenseReportViewModal.end_date = expenseReportViewModal.end_date.Value.ToString("dd/MM/yyyy");
                //}

                if (ViewBag.payment == "" || ViewBag.payment == null)
                {
                    ViewBag.payment = "Expense & Check";
                }
                else
                {
                    ViewBag.payment = expenseReportViewModal.payment;
                }

                int DeptId = Convert.ToInt32(expenseReportViewModal.deptname);
                string DeptName = "";
                //Get DepartMent Master List
                if (DeptId!=0)
                DeptName = _departmentMastersRepository.DepartmentMasterList().Where(s => s.DepartmentId == expenseReportViewModal.deptname).FirstOrDefault().DepartmentName;
                ViewBag.deptname = DeptName;

                ViewBag.storeid = Convert.ToInt32(storeid);

                int StatusId = Convert.ToInt32(expenseReportViewModal.Status);
                var StatusName = expenseReportViewModal.Status;
                ViewBag.Status = StatusName;

                int VendorId = Convert.ToInt32(expenseReportViewModal.VendorName);
                //This class is get vendor master by Id
                var Vendor = "";
                if (expenseReportViewModal.VendorName != 0)
                    Vendor= _vendorMasterRepository.GetVendormasterById().Where(s => s.VendorId == expenseReportViewModal.VendorName).FirstOrDefault().VendorName;
                ViewBag.VendorName = Vendor;

                ViewBag.AmtMaximum = expenseReportViewModal.AmtMaximum;
                ViewBag.AmtMinimum = expenseReportViewModal.AmtMinimum;
                List<ExpenseCheckExport> lstExpense_Select_ForPdf = new List<ExpenseCheckExport>();

                for (int i = 0; i < data1.Count; i++)
                {
                    ExpenseCheckExport obj = new ExpenseCheckExport();
                    obj.VendorName = data1[i].VendorName;
                    obj.Type = data1[i].Type;
                    obj.Number = data1[i].InvoiceId.ToString();

                    obj.Date = data1[i].InvoiceDate.ToString("MM/dd/yyyy");
                    obj.Amount = data1[i].Amount;

                    obj.Department = data1[i].Department;
                    obj.Memo = data1[i].Memo;
                    lstExpense_Select_ForPdf.Add(obj);
                }
                dt1 = Common.LINQToDataTable(lstExpense_Select_ForPdf);
                GeneratePDF(dt1, "Report", expenseReportViewModal.startdate, expenseReportViewModal.enddate, expenseReportViewModal.payment, expenseReportViewModal.deptname, expenseReportViewModal.Status, expenseReportViewModal.VendorName, expenseReportViewModal.AmtMaximum, expenseReportViewModal.AmtMinimum);
            }
            catch (Exception ex)
            {
                AdminSiteConfiguration.WriteErrorLogs("Controller :ExpenseReport Method : ExportPDFData Message:" + ex.Message + "Internal Message:" + ex.InnerException);
            }
            return RedirectToAction("Index", "ExpenseReport");
        }
        /// <summary>
        /// THis method is Generate pdf of Expense report
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="Name"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="payment"></param>
        /// <param name="deptname"></param>
        /// <param name="Status"></param>
        /// <param name="VendorName"></param>
        /// <param name="AmtMaximum"></param>
        /// <param name="AmtMinimum"></param>
        private void GeneratePDF(System.Data.DataTable dataTable, string Name, string startdate = "", string enddate = "", string payment = "", int deptname = 0, int Status = 0, int VendorName = 0, decimal AmtMaximum = 0, decimal AmtMinimum = 0)
        {
            try
            {
                if (dataTable.Columns.Count > 0)
                {
                    FontFactory.RegisterDirectories();
                    iTextSharp.text.Font myfont = FontFactory.GetFont("Tahoma", BaseFont.IDENTITY_H, 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
                    iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4, 5f, 5f, 50f, 0f);
                    System.IO.MemoryStream mStream = new System.IO.MemoryStream();
                    PdfWriter wri = PdfWriter.GetInstance(pdfDoc, mStream);

                    pdfDoc.Open();

                    PdfPTable _mytable = new PdfPTable(dataTable.Columns.Count);
                    _mytable.WidthPercentage = 100;
                    _mytable.SetWidths(new float[] { 8.5f, 6f, 6f, 5.5f, 5.5f, 5f, 4f });

                    PdfPTable tbl = new PdfPTable(dataTable.Columns.Count);
                    tbl.WidthPercentage = 100;
                    tbl.SetWidths(new float[] { 8.5f, 6f, 6f, 5.5f, 5.5f, 5f, 4f });

                    Phrase phrase1 = new Phrase();
                    phrase1.Add(new Chunk("Expense Report", FontFactory.GetFont("Tahoma", 20, Font.BOLD, iTextSharp.text.BaseColor.RED)));
                    PdfPCell cell1 = new PdfPCell(phrase1);
                    cell1.HorizontalAlignment = Element.ALIGN_CENTER;

                    tbl.SpacingAfter = 20f;
                    cell1.Colspan = 8;
                    cell1.Border = 0;
                    tbl.AddCell(cell1);

                    string sFilterBy = "";
                    if (payment != "")
                    {
                        sFilterBy = "Payment Method: " + payment;
                    }
                    if (startdate != "")
                    {
                        sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Start Date: " + startdate;
                    }
                    if (enddate != "")
                    {
                        sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "End Date: " + enddate;
                    }
                    if (deptname != 0)
                    {
                        //Get DepartMent Master List
                        sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Department: " + (from data in _departmentMastersRepository.DepartmentMasterList() where data.DepartmentId == deptname select data.DepartmentName).FirstOrDefault();
                    }
                    if (Status != 0)
                    {
                        sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Status: " + (Status == 1 ? "Pending" : Status == 2 ? "Approved" : Status == 3 ? "Rejected" : "OnHold");
                    }
                    if (VendorName != 0)
                    {
                        //This class is get vendor master by Id
                        sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Vendor: " + (from data in _vendorMasterRepository.GetVendormasterById() where data.VendorId == VendorName select data.VendorName).FirstOrDefault();
                    }
                    if (AmtMaximum != 0)
                    {
                        sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Max Amount: " + AmtMaximum;
                    }
                    if (AmtMinimum != 0)
                    {
                        sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Min Amount: " + AmtMinimum;
                    }
                    if (sFilterBy != "")
                    {
                        Phrase phrase2 = new Phrase();
                        phrase2.Add(new Chunk("Filter By: " + sFilterBy, FontFactory.GetFont("Tahoma", 10, Font.NORMAL, iTextSharp.text.BaseColor.BLACK)));
                        PdfPCell cell2 = new PdfPCell(phrase2);
                        cell2.HorizontalAlignment = Element.ALIGN_LEFT;

                        tbl.SpacingAfter = 25f;
                        cell2.Colspan = 8;
                        cell2.Border = 0;
                        tbl.AddCell(cell2);
                    }

                    System.Data.DataTable dt = new System.Data.DataTable();
                    dt.Columns.Add("VendorName", typeof(string));
                    dt.Columns.Add("Type", typeof(string));
                    dt.Columns.Add("Number", typeof(string));
                    dt.Columns.Add("Date", typeof(string));
                    dt.Columns.Add("Amount", typeof(string));
                    dt.Columns.Add("Department", typeof(string));
                    dt.Columns.Add("Memo", typeof(string));
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        iTextSharp.text.Font small = FontFactory.GetFont("Tahoma", BaseFont.IDENTITY_H, 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
                        Phrase p = new Phrase(dt.Columns[i].ColumnName, small);
                        PdfPCell cell = new PdfPCell(p);

                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                        _mytable.AddCell(cell);
                    }
                    //creating table data (actual result)   

                    for (int k = 0; k < dataTable.Rows.Count; k++)
                    {
                        for (int j = 0; j < dataTable.Columns.Count; j++)
                        {
                            Phrase p = new Phrase(dataTable.Rows[k][j].ToString(), myfont);
                            PdfPCell cell = new PdfPCell(p);
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                            _mytable.AddCell(cell);
                        }
                    }

                    pdfDoc.Add(tbl);
                    pdfDoc.Add(_mytable);
                    pdfDoc.Close();
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("Content-Disposition", "attachment; filename=ExpenseReport.pdf");
                    Response.Clear();
                    Response.BinaryWrite(mStream.ToArray());
                    Response.End();
                }
            }
            catch (Exception ex) 
            { logger.Error("ExpenseReportController - GeneratePDF - " + DateTime.Now + " - " + ex.Message.ToString());
            }
         
        }
        /// <summary>
        /// This method is Export Excel of expense report
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="payment"></param>
        /// <param name="deptname"></param>
        /// <param name="Status"></param>
        /// <param name="VendorName"></param>
        /// <param name="AmtMaximum"></param>
        /// <param name="AmtMinimum"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,DownloadExpenseReport")]
        public ActionResult ExportExcelData(string startdate = "", string enddate = "", string payment = "", int deptname = 0, int Status = 0, int VendorName = 0, decimal AmtMaximum = 0, decimal AmtMinimum = 0)
        {
            ExpenseReportViewModal expenseReportViewModal = new ExpenseReportViewModal();
            expenseReportViewModal.deptname = deptname;
            expenseReportViewModal.startdate = startdate;
            expenseReportViewModal.enddate = enddate;
            expenseReportViewModal.payment = payment;
            expenseReportViewModal.Status = Status;
            expenseReportViewModal.VendorName = VendorName;
            expenseReportViewModal.AmtMaximum = AmtMaximum;
            expenseReportViewModal.AmtMinimum = AmtMinimum;
            try
            {
                if (!string.IsNullOrEmpty(expenseReportViewModal.startdate))
                    expenseReportViewModal.start_date = DateTime.ParseExact(expenseReportViewModal.startdate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch { }

            try
            {
                if (!string.IsNullOrEmpty(expenseReportViewModal.enddate))
                    expenseReportViewModal.end_date = DateTime.ParseExact(expenseReportViewModal.enddate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch { }
            string storeid = "";
            if (!string.IsNullOrEmpty(Convert.ToString(Session["storeid"])))
            {
                storeid = Convert.ToString(Session["storeid"]);
                expenseReportViewModal.istoreID = Convert.ToInt32(Session["storeid"]);
            }
            else
            {
                RedirectToAction("index", "login");
            }

            try
            {
                System.Data.DataTable dt1 = new System.Data.DataTable();
                //This method is get expense check select
                var data1 = _expenseReportRepository.GetExpenseCheckSelect(expenseReportViewModal);
                List<ExpenseCheckExport> LstExpense_Select = new List<EntityModels.Models.ExpenseCheckExport>();

                for (int i = 0; i < data1.Count; i++)
                {
                    ExpenseCheckExport obj = new ExpenseCheckExport();
                    obj.VendorName = data1[i].VendorName;
                    obj.Type = data1[i].Type;
                    obj.Number = data1[i].DocNumber.ToString();
                    obj.Date = data1[i].InvoiceDate.ToString("MM/dd/yyyy");
                    obj.Amount = data1[i].Amount;
                    obj.Department = data1[i].Department;
                    obj.Memo = data1[i].Memo;
                    LstExpense_Select.Add(obj);
                }

                dt1 = Common.LINQToDataTable(LstExpense_Select);
                Export oExport = new Export();
                string FileName = "Report" + ".xls";

                //int[] ColList = { 22, 20, 6, 31, 27, 12, 3 };
                //int[] ColList = { 22, 20, 6, 31, 32, 27, 12, 3 };
                int[] ColList = { 0, 1, 2, 3, 4, 5, 6 };
                string[] arrHeader = {
             "Vendor",
             "Type",
             "DocNumber",
             "Date",
             "Amount",
             "Department",
             "Memo",
             };

                //only change file extension to .xls for excel file
                string sFilterBy = "";
                if (expenseReportViewModal.payment != "")
                {
                    sFilterBy = "Payment Method: " + expenseReportViewModal.payment;
                }
                if (expenseReportViewModal.startdate != "")
                {
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Start Date: " + expenseReportViewModal.startdate;
                }
                if (expenseReportViewModal.enddate != "")
                {
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "End Date: " + expenseReportViewModal.enddate;
                }
                if (expenseReportViewModal.deptname != 0)
                {
                    //Get DepartMent Master List
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Department: " + (from data in _departmentMastersRepository.DepartmentMasterList() where data.DepartmentId == expenseReportViewModal.deptname select data.DepartmentName).FirstOrDefault();
                }
                if (expenseReportViewModal.Status != 0)
                {
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Status: " + (expenseReportViewModal.Status == 1 ? "Pending" : expenseReportViewModal.Status == 2 ? "Approved" : expenseReportViewModal.Status == 3 ? "Rejected" : "OnHold");
                }
                if (expenseReportViewModal.VendorName != 0)
                {
                    //This class is get vendor master by Id
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Vendor: " + (from data in _vendorMasterRepository.GetVendormasterById() where data.VendorId == expenseReportViewModal.VendorName select data.VendorName).FirstOrDefault();
                }
                if (expenseReportViewModal.AmtMaximum != 0)
                {
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Max Amount: " + expenseReportViewModal.AmtMaximum;
                }
                if (expenseReportViewModal.AmtMinimum != 0)
                {
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Min Amount: " + expenseReportViewModal.AmtMinimum;
                }

                oExport.ExportDetails(dt1, ColList, arrHeader, Export.ExportFormat.Excel, FileName, sFilterBy, "Expense Report");
            }
            catch (Exception ex)
            {
                AdminSiteConfiguration.WriteErrorLogs("Controller :ExpenseReport Method : ExportExcelData Message:" + ex.Message + "Internal Message:" + ex.InnerException);
            }
            return RedirectToAction("Index", "ExpenseReport");
        }
    }
}