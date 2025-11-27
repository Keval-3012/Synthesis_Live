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
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Utility;
using NLog;
using System.Globalization;
using System.Web.Http.Results;

namespace SysnthesisRepo.Controllers
{
    public class InvoiceReportController : Controller
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
        private readonly IChartofAccountsRepository _departmentMastersRepository;
        private readonly IVendorMasterRepository _vendorMasterRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        public InvoiceReportController()
        {
            this._synthesisApiRepository = new SynthesisApiRepository();
            this._commonRepository = new CommonRepository(new DBContext());
            this._departmentMastersRepository = new ChartofAccountsRepository(new DBContext());
            this._vendorMasterRepository = new VendorMasterRepository(new DBContext());
            this._invoiceRepository = new InvoiceRepository(new DBContext());
        }
        /// <summary>
        /// This method is Return view of COGS/Bill Reports
        /// </summary>
        /// <param name="dashbordsuccess"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,ViewCOGsReport")]
        public ActionResult Index(string dashbordsuccess)
        {
            ViewBag.Title = "COGS/Bills Report - Synthesis";
            _commonRepository.LogEntries();     //Harsh's code
            strdashbordsuccess = dashbordsuccess;
            try
            {
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
                logger.Error("InvoiceReportController - Index - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            
            return View();
        }
        //  [OutputCache(Duration = 0)]
        /// <summary>
        /// This method is Get Grid data Invoice Report
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
            InvoiceReportViewModal invoiceReportViewModal = new InvoiceReportViewModal();
            invoiceReportViewModal.IsBindData = IsBindData;
            invoiceReportViewModal.currentPageIndex = currentPageIndex;
            invoiceReportViewModal.orderby = orderby;
            invoiceReportViewModal.IsAsc = IsAsc;
            invoiceReportViewModal.PageSize = PageSize;
            invoiceReportViewModal.SearchRecords = SearchRecords;
            invoiceReportViewModal.Alpha = Alpha;
            invoiceReportViewModal.deptname = deptname;
            invoiceReportViewModal.startdate = startdate;
            invoiceReportViewModal.enddate = enddate;
            invoiceReportViewModal.payment = payment;
            invoiceReportViewModal.Store_val = Store_val;
            invoiceReportViewModal.Status = Status;
            invoiceReportViewModal.VendorName = VendorName;
            invoiceReportViewModal.AmtMaximum = AmtMaximum;
            invoiceReportViewModal.AmtMinimum = AmtMinimum;
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
                            invoiceReportViewModal.IsBindData = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "currentPageIndex")
                        {
                            invoiceReportViewModal.currentPageIndex = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "orderby")
                        {
                            invoiceReportViewModal.orderby = Convert.ToString(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "IsAsc")
                        {
                            invoiceReportViewModal.IsAsc = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "PageSize")
                        {
                            invoiceReportViewModal.PageSize = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "SearchRecords")
                        {
                            invoiceReportViewModal.SearchRecords = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "Alpha")
                        {
                            invoiceReportViewModal.Alpha = Convert.ToString(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "deptname")
                        {
                            invoiceReportViewModal.deptname = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "startdate")
                        {
                            invoiceReportViewModal.startdate = Convert.ToString(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "enddate")
                        {
                            invoiceReportViewModal.enddate = Convert.ToString(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "payment")
                        {
                            invoiceReportViewModal.payment = Convert.ToString(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "Store_val")
                        {
                            invoiceReportViewModal.Store_val = Convert.ToString(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "Status")
                        {
                            invoiceReportViewModal.Status = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "VendorName")
                        {
                            invoiceReportViewModal.VendorName = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "AmtMinimum")
                        {
                            invoiceReportViewModal.AmtMinimum = Convert.ToDecimal(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "AmtMaximum")
                        {
                            invoiceReportViewModal.AmtMaximum = Convert.ToDecimal(a1.Split(':')[1].ToString());
                        }
                    }
                }
            }
            catch(Exception ex) {
                logger.Error("InvoiceReportController - Grid - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            IsArray = false;
            Arr = new string[]
            {  "IsBindData:" + invoiceReportViewModal.IsBindData
                ,"currentPageIndex:" + invoiceReportViewModal.currentPageIndex
                ,"orderby:" + invoiceReportViewModal.orderby
                ,"IsAsc:" + invoiceReportViewModal.IsAsc
                ,"PageSize:" + invoiceReportViewModal.PageSize
                ,"SearchRecords:" + invoiceReportViewModal.SearchRecords
                ,"Alpha:" + invoiceReportViewModal.Alpha
               ,"deptname:" + invoiceReportViewModal.deptname
               ,"startdate:"+invoiceReportViewModal.startdate
                ,"enddate:"+invoiceReportViewModal.enddate
                ,"payment:"+invoiceReportViewModal.payment
                ,"Store_val:"+invoiceReportViewModal.Store_val
                ,"Status:"+invoiceReportViewModal.Status
                ,"VendorName:"+invoiceReportViewModal.VendorName
                ,"AmtMaximum:"+invoiceReportViewModal.AmtMaximum
                ,"AmtMinimum:"+invoiceReportViewModal.AmtMinimum
            };
            #endregion

            #region MyRegion_BindData
            int startIndex = ((invoiceReportViewModal.currentPageIndex - 1) * invoiceReportViewModal.PageSize) + 1;
            int endIndex = startIndex + invoiceReportViewModal.PageSize - 1;
            IEnumerable Data = null;
            try
            {
                if (invoiceReportViewModal.IsBindData == 1 || IsEdit == true)
                {
                    BindData = GetData(invoiceReportViewModal).OfType<InvoiceSelect>().ToList();
                    TotalDataCount = BindData.OfType<InvoiceSelect>().ToList().Count();

                }


                if (TotalDataCount == 0)
                {
                    StatusMessage = "NoItem";
                }

                ViewBag.IsBindData = invoiceReportViewModal.IsBindData;
                ViewBag.CurrentPageIndex = invoiceReportViewModal.currentPageIndex;
                ViewBag.LastPageIndex = this.getLastPageIndex(invoiceReportViewModal.PageSize);
                ViewBag.OrderByVal = invoiceReportViewModal.orderby;
                ViewBag.IsAscVal = invoiceReportViewModal.IsAsc;
                ViewBag.PageSize = invoiceReportViewModal.PageSize;
                ViewBag.Alpha = invoiceReportViewModal.Alpha;
                ViewBag.SearchRecords = invoiceReportViewModal.SearchRecords;
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
                ViewBag.deptname = invoiceReportViewModal.deptname;
                ViewBag.startdate = invoiceReportViewModal.startdate;
                ViewBag.enddate = invoiceReportViewModal.enddate;
                ViewBag.payment = invoiceReportViewModal.payment;
                ViewBag.Store_val = invoiceReportViewModal.Store_val;
                ViewBag.Status = invoiceReportViewModal.Status;
                ViewBag.VendorName = invoiceReportViewModal.VendorName;
                ViewBag.AmtMaximum = invoiceReportViewModal.AmtMaximum;
                ViewBag.AmtMinimum = invoiceReportViewModal.AmtMinimum;

                //Print Headers Display Data 
                ViewBag.DepartmentNameDisp = "";
                if (invoiceReportViewModal.deptname != 0)
                {
                    //Get DepartMent Master List
                    ViewBag.DepartmentNameDisp = _departmentMastersRepository.DepartmentMasterList().Where(s => s.DepartmentId == invoiceReportViewModal.deptname).FirstOrDefault().DepartmentName;
                }
                ViewBag.StatusDisp = "";
                if (invoiceReportViewModal.Status != 0)
                {
                    ViewBag.StatusDisp = invoiceReportViewModal.Status == 1 ? "Pending" : invoiceReportViewModal.Status == 2 ? "Approved" : invoiceReportViewModal.Status == 3 ? "Rejected" : "OnHold";
                }
                ViewBag.VendorNameDisp = "";
                if (invoiceReportViewModal.VendorName != 0)
                {
                    //This class is get vendor master by Id
                    ViewBag.VendorNameDisp = _vendorMasterRepository.GetVendormasterById().Where(s => s.VendorId == invoiceReportViewModal.VendorName).FirstOrDefault().VendorName;
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
                var ColumnName = typeof(InvoiceSelect).GetProperties().Where(p => p.Name == invoiceReportViewModal.orderby).FirstOrDefault();
                if (invoiceReportViewModal.IsAsc == 1)
                {
                    ViewBag.AscVal = 0;
                    Data = BindData.OfType<InvoiceSelect>().ToList().OrderBy(n => ColumnName.GetValue(n, null)).Skip(startIndex - 1).Take(invoiceReportViewModal.PageSize);
                }
                else
                {

                    ViewBag.AscVal = 1;
                    Data = BindData.OfType<InvoiceSelect>().ToList().Skip(startIndex - 1).Take(invoiceReportViewModal.PageSize);
                }
                if (invoiceReportViewModal.deptname == 0)
                {
                    ViewBag.TotalInvoiceType = BindData.OfType<InvoiceSelect>().ToList().Skip(startIndex - 1).Take(invoiceReportViewModal.PageSize).Where(x => x.InvoiceTypeId == 1).Select(x => x.TotalAmount).Sum();
                    ViewBag.TotalCreditMemo = BindData.OfType<InvoiceSelect>().ToList().Skip(startIndex - 1).Take(invoiceReportViewModal.PageSize).Where(x => x.InvoiceTypeId == 2).Select(x => x.TotalAmount).Sum();
                }
                else
                {
                    ViewBag.TotalInvoiceType = BindData.OfType<InvoiceSelect>().ToList().Skip(startIndex - 1).Take(invoiceReportViewModal.PageSize).Where(x => x.InvoiceTypeId == 1).Select(x => x.TotalAmount).Sum();
                    ViewBag.TotalCreditMemo = BindData.OfType<InvoiceSelect>().ToList().Skip(startIndex - 1).Take(invoiceReportViewModal.PageSize).Where(x => x.InvoiceTypeId == 2).Select(x => x.TotalAmount).Sum();
                }
                var enumData = from InvoiceStatusEnm e in Enum.GetValues(typeof(InvoiceStatusEnm))
                               select new
                               {
                                   ID = (int)e,
                                   Name = e.ToString()
                               };
                ViewBag.EnumList = new SelectList(enumData, "ID", "Name");
                //Get DepartMent Master List
                ViewBag.FltDepartmentId = new SelectList(_departmentMastersRepository.DepartmentMasterList().Where(a => a.StoreId == storeid).Select(s => new { s.DepartmentId, s.DepartmentName }), "DepartmentId", "DepartmentName");
                //This class is get vendor master by Id
                ViewBag.FltVendorId = new SelectList(_vendorMasterRepository.GetVendormasterById().Where(a => a.StoreId == storeid).Select(s => new { s.VendorId, s.VendorName }), "VendorId", "VendorName");
                StatusMessage = "";
                if (invoiceReportViewModal.startdate != "" || invoiceReportViewModal.enddate != "" || invoiceReportViewModal.payment != "" || ViewBag.DepartmentNameDisp != "" || ViewBag.StatusDisp != "" || ViewBag.VendorNameDisp != "" || invoiceReportViewModal.AmtMinimum != 0 || invoiceReportViewModal.AmtMaximum != 0)
                {
                    clsActivityLog clsActivityLog = new clsActivityLog();
                    clsActivityLog.Action = "Click";
                    clsActivityLog.ModuleName = "Report";
                    clsActivityLog.PageName = "COGS/Bills Report";
                    clsActivityLog.Message = " COGS/Bills Report Generated for " + (invoiceReportViewModal.startdate == "" ? "" : "From Date : ") + invoiceReportViewModal.startdate + (invoiceReportViewModal.startdate == "" ? "" : " - To Date : ") + invoiceReportViewModal.enddate + (invoiceReportViewModal.payment == "" ? "" : " ,Payment Method : ") + invoiceReportViewModal.payment + (ViewBag.DepartmentNameDisp == "" ? "" : " , Departments : ") + ViewBag.DepartmentNameDisp + (ViewBag.StatusDisp == "" ? "" : " ,Status : ") + ViewBag.StatusDisp + (ViewBag.VendorNameDisp == "" ? "" : " ,Vendor Name : ") + ViewBag.VendorNameDisp + (invoiceReportViewModal.AmtMinimum == 0 ? "" : " ,Min Amount : ") + (invoiceReportViewModal.AmtMinimum == 0 ? "" : Convert.ToString(invoiceReportViewModal.AmtMinimum)) + (invoiceReportViewModal.AmtMaximum == 0 ? "" : " ,Max Amount : ") + (invoiceReportViewModal.AmtMaximum == 0 ? "" : Convert.ToString(invoiceReportViewModal.AmtMaximum));
                    clsActivityLog.CreatedBy = Convert.ToInt32(Session["UserID"]);
                    _synthesisApiRepository.CreateLog(clsActivityLog);
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportController - grid - " + DateTime.Now + " - " + ex.Message.ToString());
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
            if (TotalDataCount % PageSize > 0)
            {
                lastPageIndex += 1;
            }

            return lastPageIndex;
        }
        /// <summary>
        /// This method is Get Invoice Report Data.
        /// </summary>
        /// <param name="invoiceReportViewModal"></param>
        /// <returns></returns>
        private IEnumerable GetData(InvoiceReportViewModal invoiceReportViewModal)
        {
            string userid = UserModule.getUserId().ToString();

            try
            {
                invoiceReportViewModal.start_date = DateTime.ParseExact(invoiceReportViewModal.startdate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            }
            catch { }

            try
            {
                invoiceReportViewModal.end_date = DateTime.ParseExact(invoiceReportViewModal.enddate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            }
            catch { }
            IEnumerable RtnData = null;

            try
            {
                string storeid = "";
                if (!string.IsNullOrEmpty(Convert.ToString(Session["storeid"])))
                {
                    storeid = Convert.ToString(Session["storeid"]);
                    if (storeid != "")
                    {
                        invoiceReportViewModal.istoreID = Convert.ToInt32(storeid);
                    }
                }
                else
                {
                    RedirectToAction("index", "Login");
                }
                //This db class is Get Expense Check Select invoice report
                RtnData = _invoiceRepository.GetExpenseCheckSelect(invoiceReportViewModal);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportController - GetData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return RtnData;

        }
        /// <summary>
        /// This method is Export PDF data
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
        [Authorize(Roles = "Administrator,DownloadCOGsReport")]
        public ActionResult ExportPDFData(string startdate = "", string enddate = "", string payment = "", int deptname = 0, int Status = 0, int VendorName = 0, decimal AmtMaximum = 0, decimal AmtMinimum = 0)
        {
            InvoiceReportViewModal invoiceReportViewModal = new InvoiceReportViewModal();
            invoiceReportViewModal.deptname = deptname;
            invoiceReportViewModal.startdate = startdate;
            invoiceReportViewModal.enddate = enddate;
            invoiceReportViewModal.payment = payment;
            invoiceReportViewModal.Status = Status;
            invoiceReportViewModal.VendorName = VendorName;
            invoiceReportViewModal.AmtMaximum = AmtMaximum;
            invoiceReportViewModal.AmtMinimum = AmtMinimum;
            try
            {
                if(!string.IsNullOrEmpty(invoiceReportViewModal.startdate))
                    invoiceReportViewModal.start_date = DateTime.ParseExact(invoiceReportViewModal.startdate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch { }

            try
            {
                if (!string.IsNullOrEmpty(invoiceReportViewModal.enddate))
                    invoiceReportViewModal.end_date = DateTime.ParseExact(invoiceReportViewModal.enddate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch { }
            string storeid = "";
            if (!string.IsNullOrEmpty(Convert.ToString(Session["storeid"])))
            {
                storeid = Convert.ToString(Session["storeid"]);
                invoiceReportViewModal.storeid= Convert.ToInt32(Session["storeid"]);
            }
            else
            {
                RedirectToAction("index", "login");
            }
            try
            {
                DataTable dt1 = new DataTable();
                //This db class is Get Invoice using Select invoice report
                var data1 = _invoiceRepository.GetInvoiceSelect(invoiceReportViewModal);

                //if (invoiceReportViewModal.start_date != null && !string.IsNullOrEmpty(Convert.ToString(invoiceReportViewModal.start_date)))
                //{
                //    ViewBag.start_date = invoiceReportViewModal.start_date.Value.ToString("MM/dd/yyyy");
                //}
                //if (invoiceReportViewModal.end_date != null && !string.IsNullOrEmpty(Convert.ToString(invoiceReportViewModal.end_date)))
                //{
                //    ViewBag.end_date = invoiceReportViewModal.end_date.Value.ToString("MM/dd/yyyy");
                //}

                if (ViewBag.payment == "" || ViewBag.payment == null)
                {
                    ViewBag.payment = "Cash & Check/ACH";
                }
                else
                {
                    ViewBag.payment = invoiceReportViewModal.payment;
                }

                int DeptId = Convert.ToInt32(invoiceReportViewModal.deptname);
                string DeptName = "";
                //Get DepartMent Master List
                if (DeptId != 0)
                {
                    DeptName = _departmentMastersRepository.DepartmentMasterList().Count() > 0 ? _departmentMastersRepository.DepartmentMasterList().Where(s => s.DepartmentId == invoiceReportViewModal.deptname).FirstOrDefault().DepartmentName : "";
                }
                ViewBag.deptname = DeptName;

                ViewBag.storeid = Convert.ToInt32(storeid);

                int StatusId = Convert.ToInt32(invoiceReportViewModal.Status);
                var StatusName = invoiceReportViewModal.Status;
                ViewBag.Status = StatusName;

                int VendorId = Convert.ToInt32(invoiceReportViewModal.VendorName);
                //This class is get vendor master by Id
                string Vendor = "";
                if (invoiceReportViewModal.VendorName != 0)
                {
                    Vendor = _vendorMasterRepository.GetVendormasterById().Where(s => s.VendorId == invoiceReportViewModal.VendorName).FirstOrDefault().VendorName;
                }
                ViewBag.VendorName = Vendor;

                ViewBag.AmtMaximum = invoiceReportViewModal.AmtMaximum;
                ViewBag.AmtMinimum = invoiceReportViewModal.AmtMinimum;
                List<Invoice_Select_ForPdf> lstInvoice_Select_ForPdf = new List<Invoice_Select_ForPdf>();

                for (int i = 0; i < data1.Count; i++)
                {
                    Invoice_Select_ForPdf obj = new Invoice_Select_ForPdf();
                    obj.VendorName = data1[i].VendorName;
                    obj.InvoiceType = data1[i].InvoiceType;
                    obj.InvoiceNumber = data1[i].InvoiceNumber;
                    obj.CreatedOn = data1[i].CreatedOn.Value.ToString("MM/dd/yyyy");
                    obj.InvoiceDate = data1[i].InvoiceDate.ToString("MM/dd/yyyy");
                    obj.TotalAmount = data1[i].TotalAmount;
                    obj.Status = data1[i].StatusValue.ToString();
                    obj.PaymentType = data1[i].PaymentType;
                    lstInvoice_Select_ForPdf.Add(obj);
                }
                dt1 = Common.LINQToDataTable(lstInvoice_Select_ForPdf);
                GeneratePDF(dt1, "Report", invoiceReportViewModal.startdate, invoiceReportViewModal.enddate, invoiceReportViewModal.payment, invoiceReportViewModal.deptname, invoiceReportViewModal.Status, invoiceReportViewModal.VendorName, invoiceReportViewModal.AmtMaximum, invoiceReportViewModal.AmtMinimum);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportController - ExportPDFData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return RedirectToAction("Index", "InvoiceReport");
        }
        /// <summary>
        /// This method is Genearte PDf
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
        private void GeneratePDF(DataTable dataTable, string Name, string startdate = "", string enddate = "", string payment = "", int deptname = 0, int Status = 0, int VendorName = 0, decimal AmtMaximum = 0, decimal AmtMinimum = 0)
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
                    _mytable.SetWidths(new float[] { 8.5f, 6f, 6f, 5.5f, 5.5f, 5f, 4f, 7.5f });

                    PdfPTable tbl = new PdfPTable(dataTable.Columns.Count);
                    tbl.WidthPercentage = 100;
                    tbl.SetWidths(new float[] { 8.5f, 6f, 6f, 5.5f, 5.5f, 5f, 4f, 7.5f });

                    Phrase phrase1 = new Phrase();
                    phrase1.Add(new Chunk("Invoice Report", FontFactory.GetFont("Tahoma", 20, Font.BOLD, iTextSharp.text.BaseColor.RED)));
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
                    DataTable dt = new DataTable();
                    dt.Columns.Add("VendorName", typeof(string));
                    dt.Columns.Add("InvoiceType", typeof(string));
                    dt.Columns.Add("InvoiceNo", typeof(string));
                    dt.Columns.Add("CreatedOn", typeof(string));
                    dt.Columns.Add("InvoiceDate", typeof(string));
                    dt.Columns.Add("Amount", typeof(string));
                    dt.Columns.Add("Status", typeof(string));
                    dt.Columns.Add("PaymentMethod", typeof(string));
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        iTextSharp.text.Font small = FontFactory.GetFont("Tahoma", BaseFont.IDENTITY_H, 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
                        Phrase p = new Phrase(dt.Columns[i].ColumnName, small);
                        PdfPCell cell = new PdfPCell(p);

                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                        _mytable.AddCell(cell);
                    }
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
                    Response.AddHeader("Content-Disposition", "attachment; filename=InvoiceReport.pdf");
                    Response.Clear();
                    Response.BinaryWrite(mStream.ToArray());
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportController - GeneratePDF - " + DateTime.Now + " - " + ex.Message.ToString());
            }
          
        }

        /// <summary>
        /// This method is Export Excel data.
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
        [Authorize(Roles = "Administrator,DownloadCOGsReport")]
        public ActionResult ExportExcelData(int IsBindData = 1, int currentPageIndex = 1, string orderby = "InvoiceId", int IsAsc = 0, int PageSize = 100, int SearchRecords = 1, string Alpha = "", int deptname = 0, string startdate = "", string enddate = "", string payment = "", string Store_val = "", int Status = 0, int VendorName = 0, decimal AmtMaximum = 0, decimal AmtMinimum = 0)
        {
            InvoiceReportViewModal invoiceReportViewModal = new InvoiceReportViewModal();
            invoiceReportViewModal.IsBindData = IsBindData;
            invoiceReportViewModal.currentPageIndex = currentPageIndex;
            invoiceReportViewModal.orderby = orderby;
            invoiceReportViewModal.IsAsc = IsAsc;
            invoiceReportViewModal.PageSize = PageSize;
            invoiceReportViewModal.SearchRecords = SearchRecords;
            invoiceReportViewModal.Alpha = Alpha;
            invoiceReportViewModal.deptname = deptname;
            invoiceReportViewModal.startdate = startdate;
            invoiceReportViewModal.enddate = enddate;
            invoiceReportViewModal.payment = payment;
            invoiceReportViewModal.Store_val = Store_val;
            invoiceReportViewModal.Status = Status;
            invoiceReportViewModal.VendorName = VendorName;
            invoiceReportViewModal.AmtMaximum = AmtMaximum;
            invoiceReportViewModal.AmtMinimum = AmtMinimum;
            DateTime? start_date = null;
            DateTime? end_date = null;
            try
            {
                if(invoiceReportViewModal.startdate!="")
                    invoiceReportViewModal.start_date = DateTime.ParseExact(invoiceReportViewModal.startdate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch { }

            try
            {
                if (invoiceReportViewModal.enddate != "")
                    invoiceReportViewModal.end_date = DateTime.ParseExact(invoiceReportViewModal.enddate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch { }
            string storeid = "";
            if (!string.IsNullOrEmpty(Convert.ToString(Session["storeid"])))
            {
                storeid = Convert.ToString(Session["storeid"]);
                invoiceReportViewModal.storeid= Convert.ToInt32(Session["storeid"]);

            }
            else
            {
                RedirectToAction("index", "login");
            }

            try
            {
                DataTable dt1 = new DataTable();
                //This db class is Get Invoice using Select invoice report
                var data1 = _invoiceRepository.GetInvoiceSelect(invoiceReportViewModal);
                List<Invoice_Select_ForPdf> LstInvoice_Select = new List<EntityModels.Models.Invoice_Select_ForPdf>();

                for (int i = 0; i < data1.Count; i++)
                {
                    Invoice_Select_ForPdf obj = new Invoice_Select_ForPdf();
                    obj.VendorName = data1[i].VendorName;
                    obj.InvoiceType = data1[i].InvoiceType;
                    obj.InvoiceNumber = data1[i].InvoiceNumber;
                    obj.CreatedOn = data1[i].CreatedOn.Value.ToString("MM/dd/yyyy");
                    obj.InvoiceDate = data1[i].InvoiceDate.ToString("MM/dd/yyyy");
                    obj.TotalAmount = data1[i].TotalAmount;
                    obj.Status = data1[i].StatusValue.ToString();
                    obj.PaymentType = data1[i].PaymentType;
                    LstInvoice_Select.Add(obj);
                }

                dt1 = Common.LINQToDataTable(LstInvoice_Select);
                Export oExport = new Export();
                string FileName = "Report" + ".xls";

                //int[] ColList = { 22, 20, 6, 31, 27, 12, 3 };
                //int[] ColList = { 22, 20, 6, 31, 32, 27, 12, 3 };
                int[] ColList = { 0, 1, 2, 3, 4, 5, 6, 7 };
                string[] arrHeader = {
             "Vendor",
             "Type",
             "Invoice#",
             "CreatedOn",
             "Invoice Date",
             "Amount",
             "Status",
             "Payment Method",
             };

                //only change file extension to .xls for excel file
                string sFilterBy = "";
                if (invoiceReportViewModal.payment != "")
                {
                    sFilterBy = "Payment Method: " + invoiceReportViewModal.payment;
                }
                if (invoiceReportViewModal.startdate != "")
                {
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Start Date: " + invoiceReportViewModal.startdate;
                }
                if (invoiceReportViewModal.enddate != "")
                {
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "End Date: " + invoiceReportViewModal.enddate;
                }
                if (invoiceReportViewModal.deptname != 0)
                {
                    //Get DepartMent Master List
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Department: " + (from data in _departmentMastersRepository.DepartmentMasterList() where data.DepartmentId == invoiceReportViewModal.deptname select data.DepartmentName).FirstOrDefault();
                }
                if (invoiceReportViewModal.Status != 0)
                {
                   
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Status: " + (invoiceReportViewModal.Status == 1 ? "Pending" : invoiceReportViewModal.Status == 2 ? "Approved" : invoiceReportViewModal.Status == 3 ? "Rejected" : "OnHold");
                }
                if (invoiceReportViewModal.VendorName != 0)
                {
                    //This class is get vendor master by Id
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Vendor: " + (from data in _vendorMasterRepository.GetVendormasterById() where data.VendorId == invoiceReportViewModal.VendorName select data.VendorName).FirstOrDefault();
                }
                if (invoiceReportViewModal.AmtMaximum != 0)
                {
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Max Amount: " + invoiceReportViewModal.AmtMaximum;
                }
                if (invoiceReportViewModal.AmtMinimum != 0)
                {
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Min Amount: " + invoiceReportViewModal.AmtMinimum;
                }

                oExport.ExportDetails(dt1, ColList, arrHeader, Export.ExportFormat.Excel, FileName, sFilterBy, "Invoice Reports");
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceReportController - ExportExcelData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return RedirectToAction("Index", "InvoiceReport");
        }
    }
}