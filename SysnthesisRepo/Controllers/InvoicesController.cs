using DocumentFormat.OpenXml.Wordprocessing;
using EntityModels.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Newtonsoft.Json;
using NLog;
using Org.BouncyCastle.Asn1.Cmp;
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
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Utility;
using Font = iTextSharp.text.Font;
using PageSize = iTextSharp.text.PageSize;
using OfficeOpenXml;
using HtmlAgilityPack;
using OfficeOpenXml.Style;
using System.Globalization;
using Aspose.Pdf.Operators;
using static SynthesisRepo.Controllers.SpreadsheetController;
using System.Web.Script.Serialization;
using DocumentFormat.OpenXml.EMMA;
using System.Text.RegularExpressions;
using static SysnthesisRepo.Controllers.InvoicesController;
using EntityModels.HRModels;

namespace SysnthesisRepo.Controllers
{
    public class InvoicesController : Controller
    {

        protected static string DeleteMessage = "";
        protected static string StatusMessage = "";
        protected static string ExistCode = "";
        protected static string ActivityLogMessage = "";
        private const int FirstPageIndex = 1;
        protected static int TotalDataCount;
        protected static Array Arr;
        protected static bool IsArray;
        protected static string IsFilter = "0";
        protected static int SaveFile = 0;
        protected static bool IsEdit = false;
        protected static int Quickvalue;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMastersBindRepository _MastersBindRepository;
        private readonly IUserActivityLogRepository _ActivityLogRepository;
        private readonly ICommonRepository _CommonRepository;
        private readonly IVendorMasterRepository _VendorMasterRepository;
        private readonly ISynthesisApiRepository _SynthesisApiRepository;
        private readonly IQBRepository _QBRepository;
        private readonly IUserActivityLogRepository _activityLogRepository;

        public InvoicesController()
        {
            this._invoiceRepository = new InvoiceRepository(new DBContext());
            this._MastersBindRepository = new MastersBindRepository(new DBContext());
            this._ActivityLogRepository = new UserActivityLogRepository(new DBContext());
            this._CommonRepository = new CommonRepository(new DBContext());
            this._VendorMasterRepository = new VendorMasterRepository(new DBContext());
            this._SynthesisApiRepository = new SynthesisApiRepository();
            this._QBRepository = new QBRepository(new DBContext());
            this._activityLogRepository = new UserActivityLogRepository(new DBContext());
        }
        // GET: Invoice
        /// <summary>
        /// This method return View of Invoice
        /// </summary>
        /// <param name="MSG"></param>
        /// <returns></returns>
        public async Task<ActionResult> Index(string MSG)
        {
            ViewBag.Title = "View Invoices - Synthesis";
            _CommonRepository.LogEntries();     //Harsh's code
            if (StatusMessage == "Success1" || StatusMessage == "Success" || StatusMessage == "Delete")
            {
                ViewBag.StatusMessage = StatusMessage;
            }
            else
            {
                ViewBag.StatusMessage = "";
            }
            return View();
        }

        /// <summary>
        /// This method is get grid of Invoice
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
        /// <param name="searchdashbord"></param>
        /// <param name="AmtMaximum"></param>
        /// <param name="AmtMinimum"></param>
        /// <param name="SearchFlg"></param>
        /// <returns></returns>
        public ActionResult Grid(int IsBindData = 1, int currentPageIndex = 1, string orderby = "InvoiceId", int IsAsc = 0, int PageSize = 50, int SearchRecords = 1, string Alpha = "", int deptname = 0, string startdate = "", string enddate = "", string payment = "", string Store_val = "", string searchdashbord = "", string AmtMaximum = "0", string AmtMinimum = "0", string SearchFlg = "")
        {
            List<Invoice> BindData1 = new List<Invoice>();
            List<InvoiceSelect> BindData = new List<InvoiceSelect>();


            #region MyRegion_BindData
            int startIndex = ((currentPageIndex - 1) * PageSize) + 1;
            int endIndex = startIndex + PageSize - 1;
            var ColumnName = typeof(Invoice).GetProperties().Where(p => p.Name == orderby).FirstOrDefault();
            //var OrderByCol = ColumnName.GetValue(n, null);

            IEnumerable Data = null;
            if (searchdashbord == "Clear")
            {
                Session["searchdashbord"] = "";
                searchdashbord = "";
            }
            else
            {
                if (searchdashbord != "")
                {
                    Session["searchdashbord"] = searchdashbord.Trim();
                }
            }

            var strStore = "";
            var UserTypeId = _CommonRepository.getUserTypeId(User.Identity.Name);
            if (IsBindData == 1 || IsEdit == true)
            {
                var StoreId = Convert.ToInt32(Session["storeid"]);
                var AscDsc = "Desc";
                if (IsAsc == 1)
                {
                    AscDsc = "asc";
                }
                if (searchdashbord == "Clear")
                {
                    searchdashbord = "";
                }
                else
                {
                    searchdashbord = (Session["searchdashbord"] == null ? "" : Session["searchdashbord"].ToString()); //.Replace("'", "''")
                }
                if (Session["AmtMaximum"] != null)
                {
                    AmtMaximum = Session["AmtMaximum"].ToString();
                }
                if (Session["AmtMinimum"] != null)
                {
                    AmtMinimum = Session["AmtMinimum"].ToString();
                }
                if (Session["startdate"] != null)
                {
                    startdate = Session["startdate"].ToString();
                }
                if (Session["enddate"] != null)
                {
                    enddate = Session["enddate"].ToString();
                }
                if (Session["paymethod"] != null)
                {
                    payment = Session["paymethod"].ToString();
                }
                if (Session["deptname"] != null)
                {
                    deptname = (Session["deptname"].ToString() == "" ? 0 : Convert.ToInt32(Session["deptname"].ToString()));
                }

                try
                {
                    // Get Invoice Stores List
                    ////this class is get UserID by username
                    strStore = _invoiceRepository.GetInvoice_StoreList(StoreId, Roles.IsUserInRole("Administrator"), _CommonRepository.getUserId(User.Identity.Name));

                    if (StoreId == 0 && strStore.Length > 0)
                    {
                        var S = (strStore.Split(',').Count() == 1 ? Convert.ToInt32(strStore.Split(',')[0]) : 0);
                        if (S > 0)
                        {
                            Session["storeid"] = S;
                            StoreId = S;
                        }
                    }
                    try
                    {
                        if (startdate.ToString() != "" || enddate.ToString() != "" || AmtMaximum.ToString() != "" || AmtMinimum.ToString() != "" || payment.ToString() != "" || deptname.ToString() != "")
                        {
                            SearchFlg = "F";
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error("InvoicesController - Grid - " + DateTime.Now + " - " + ex.Message.ToString());
                    }
                    // //This db class is Get Invoice using Select
                    BindData = _invoiceRepository.GetInvoiceSelects(startdate, enddate, payment, deptname, strStore, AmtMaximum, AmtMinimum, ((startIndex - 1) * PageSize), PageSize, orderby, AscDsc, searchdashbord, _CommonRepository.getUserTypeId(User.Identity.Name), SearchFlg);

                    TotalDataCount = BindData.OfType<InvoiceSelect>().ToList().Count();
                }
                catch (Exception ex)
                {
                    logger.Error("InvoicesController - Grid - " + DateTime.Now + " - " + ex.Message.ToString());
                }
            }


            if (TotalDataCount == 0)
            {
                StatusMessage = "NoItem";
            }

            ViewBag.IsBindData = IsBindData;
            ViewBag.CurrentPageIndex = currentPageIndex;
            ViewBag.LastPageIndex = this.getLastPageIndex(PageSize);
            ViewBag.OrderByVal = orderby;
            ViewBag.IsAscVal = IsAsc;
            ViewBag.PageSize = PageSize;
            ViewBag.Alpha = Alpha;
            ViewBag.SearchRecords = SearchRecords;
            ViewBag.startindex = startIndex;
            ViewBag.Store_val = Store_val;
            ViewBag.SearchFlg = SearchFlg;

            ViewBag.searchdashbord = searchdashbord;
            try
            {
                if (IsAsc == 1)
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
                //This class is get departments with department cond
                ViewBag.DrpLstdept = new SelectList(_invoiceRepository.GetDepartments_WithDepartmentCond(UserTypeId, StoreID), "DepartmentId", "DepartmentName");


                ViewBag.CreateIn = _CommonRepository.GetMessageValue("INICSS", "Invoice Created Successfully.");
                ViewBag.CreateUpdate = _CommonRepository.GetMessageValue("INIUSF", "Invoice Updated Successfully.");
                ViewBag.CreateDelete = _CommonRepository.GetMessageValue("INIDSF", "Invoice Deleted Successfully.");
                ViewBag.SyncForce = _CommonRepository.GetMessageValue("INSF", "Are you sure want to sync forcefully?");
                ViewBag.CreateInApprove = _CommonRepository.GetMessageValue("INSA", "Invoice Created Successfully and Sent for Approval.");

            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - Grid - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            ViewBag.CreateIn = _CommonRepository.GetMessageValue("INICSS", "Invoice Created Successfully.");
            ViewBag.CreateUpdate = _CommonRepository.GetMessageValue("INIUSF", "Invoice Updated Successfully.");
            ViewBag.CreateDelete = _CommonRepository.GetMessageValue("INIDSF", "Invoice Deleted Successfully.");
            ViewBag.SyncForce = _CommonRepository.GetMessageValue("INSF", "Are you sure want to sync forcefully?");
            ViewBag.CreateInApprove = _CommonRepository.GetMessageValue("INSA", "Invoice Created Successfully and Sent for Approval.");

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
        /// This method is return view of Invoices Beta
        /// </summary>
        /// <param name="MSG"></param>
        /// <returns></returns>
        public async Task<ActionResult> IndexBeta(string MSG)
        {
            ViewBag.Title = "View Invoices - Synthesis";
            _CommonRepository.LogEntries();     //Harsh's code
            //UserModule.ManageSession(User.Identity.Name);
            if (StatusMessage == "Success1" || StatusMessage == "Success" || StatusMessage == "Delete")
            {
                ViewBag.StatusMessage = StatusMessage;

                //StatusMessage = "";
            }
            else
            {
                ViewBag.StatusMessage = "";
            }

            if (!string.IsNullOrEmpty(Session["StoreId"].ToString()))
            {

                ViewBag.Store = Convert.ToInt32(Session["StoreId"]);
            }
            return View();
        }

        /// <summary>
        /// This method is return Department data
        /// </summary>
        /// <returns></returns>
        public ActionResult Grid2()
        {
            int storeid = Convert.ToInt32(Session["storeid"]);
            var strStore = "";
            var UserTypeId = _CommonRepository.getUserTypeId(User.Identity.Name);

            try
            {
                strStore = _invoiceRepository.GetInvoice_StoreList(storeid, Roles.IsUserInRole("Administrator"), _CommonRepository.getUserId(User.Identity.Name));

                try
                {
                    if (storeid == 0 && strStore.Length > 0)
                    {
                        var S = (strStore.Split(',').Count() == 1 ? Convert.ToInt32(strStore.Split(',')[0]) : 0);
                        if (S > 0)
                        {
                            Session["storeid"] = S;
                            storeid = S;
                        }
                    }

                }
                catch (Exception ex)
                {
                    logger.Error("InvoicesController - Grid2(1) - " + DateTime.Now + " - " + ex.Message.ToString());
                }

                if (storeid != 0)
                {
                    //This class is Get Department using storeId
                    ViewBag.departmentid = new SelectList(_invoiceRepository.getDepartment_WithSP(storeid), "DepartmentId", "DepartmentName", "");
                }
                ViewBag.Storeidvalue = storeid;
                ViewBag.headerstoreid = Convert.ToInt32(Session["storeid"]);
                ViewBag.StatusMessage = "";
                ViewBag.CreateIn = _CommonRepository.GetMessageValue("INICSS", "Invoice Created Successfully.");
                ViewBag.CreateUpdate = _CommonRepository.GetMessageValue("INIUSF", "Invoice Updated Successfully.");
                ViewBag.CreateInApprove = _CommonRepository.GetMessageValue("INSA", "Invoice Created Successfully and Sent for Approval.");
                Session["Filter"] = "";
            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - Grid2 - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return View();

        }

        /// <summary>
        /// This method is get url data source for Invoice
        /// </summary>
        /// <param name="dm"></param>
        /// <param name="storeid"></param>
        /// <param name="deptid"></param>
        /// <returns></returns>
        /// 
        public ActionResult UrlDatasourceInvoice(DataManagerRequest dm, string deptid)
        {
            List<InvoiceSelect> BindData = new List<InvoiceSelect>();
            IEnumerable DataSource = new List<InvoiceSelect>();
            var UserTypeId = _CommonRepository.getUserTypeId(User.Identity.Name);
            string SearchTerm = "";
            string SortFields = "";
            string SortDirections = "";
            string Filters = "";
            int Skip = 0;
            int Take = 50;
            string storeid = "";

            int sid = Convert.ToInt32(Session["storeid"]);
            if (sid != 0)
            {
                storeid = sid.ToString();
            }
            int count = 0;
            try
            {
                if (!String.IsNullOrEmpty(Session["Filter"].ToString()))
                {
                    // update this value if the Grid's PageSize setting is modified.
                    if (dm.Take == 50)
                    {
                        if (dm.Where == null)
                        {
                            Session["Filter"] = "";
                        }
                    }

                    Filters = Session["Filter"].ToString();
                }

                if (dm.Search != null && dm.Search.Count > 0)
                {
                    SearchTerm = dm.Search[0].Key.ToString().Trim().Replace("'", "''");
                }
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    SortFields = dm.Sorted[0].Name.ToString().Trim();
                    SortDirections = dm.Sorted[0].Direction.ToString().Trim() == "descending" ? "DESC" : "ASC";
                }
                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    Session["Filter"] = "";
                    string json = JsonConvert.SerializeObject(dm.Where);
                    List<Predicate> predicates = JsonConvert.DeserializeObject<List<Predicate>>(json);
                    string whereClause = BuildWhereCondition(predicates);
                    Filters = whereClause.Replace("StoreName", "NickName");
                    logger.Info(Filters + DateTime.Now);
                    if (String.IsNullOrEmpty(Session["Filter"].ToString()))
                    {
                        Session["Filter"] = Filters;
                    }
                }
                if (dm.Skip != 0)
                {
                    Skip = dm.Skip;
                }
                if (dm.Take != 0)
                {
                    Take = dm.Take;
                }

                var invoices = _invoiceRepository.InvoiceSyncfusionGrid_Data(SearchTerm, SortFields, SortDirections, Filters, Skip, Take, UserTypeId, deptid, storeid);
                BindData = invoices.Item1;
                DataSource = BindData;
                count = invoices.Item2;
                ViewBag.Invoicecount = count;
            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - UrlDatasourceInvoice - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }

        public static string BuildWhereCondition(List<Predicate> predicates, string parentCondition = null)
        {
            if (predicates == null || predicates.Count == 0)
                return string.Empty;

            List<string> conditions = new List<string>();

            foreach (var predicate in predicates)
            {
                if (predicate.IsComplex && predicate.Predicates != null && predicate.Predicates.Any())
                {
                    string nestedCondition = BuildWhereCondition(predicate.Predicates, predicate.Condition?.ToLower());
                    if (!string.IsNullOrEmpty(nestedCondition))
                    {
                        conditions.Add($"({nestedCondition})");
                    }
                }
                else if (!string.IsNullOrEmpty(predicate.Field) && !string.IsNullOrEmpty(predicate.Operator) && predicate.Value != null)
                {
                    string condition;
                    if (!string.IsNullOrEmpty(predicate.Operator))
                    {
                        switch (predicate.Operator.ToLower())
                        {
                            case "equal":
                                if (!string.IsNullOrEmpty(predicate.Field) && predicate.Value != null)
                                    condition = $"{predicate.Field} = '{predicate.Value}'";
                                else
                                    throw new ArgumentException("Field or Value cannot be null for 'equal' operator.");
                                break;

                            case "notequal":
                                if (!string.IsNullOrEmpty(predicate.Field) && predicate.Value != null)
                                    condition = $"{predicate.Field} != '{predicate.Value}'";
                                else
                                    throw new ArgumentException("Field or Value cannot be null for 'notequal' operator.");
                                break;

                            case "contains":
                                if (!string.IsNullOrEmpty(predicate.Field) && predicate.Value != null)
                                    condition = $"{predicate.Field} LIKE '%{predicate.Value}%'";
                                else
                                    throw new ArgumentException("Field or Value cannot be null for 'contains' operator.");
                                break;

                            case "startswith":
                                if (!string.IsNullOrEmpty(predicate.Field) && predicate.Value != null)
                                    condition = $"{predicate.Field} LIKE '{predicate.Value}%'";
                                else
                                    throw new ArgumentException("Field or Value cannot be null for 'startswith' operator.");
                                break;

                            case "endswith":
                                if (!string.IsNullOrEmpty(predicate.Field) && predicate.Value != null)
                                    condition = $"{predicate.Field} LIKE '%{predicate.Value}'";
                                else
                                    throw new ArgumentException("Field or Value cannot be null for 'endswith' operator.");
                                break;

                            case "doesnotstartwith":
                                if (!string.IsNullOrEmpty(predicate.Field) && predicate.Value != null)
                                    condition = $"{predicate.Field} NOT LIKE '{predicate.Value}%'";
                                else
                                    throw new ArgumentException("Field or Value cannot be null for 'doesnotstartwith' operator.");
                                break;
                            case "doesnotendwith":
                                if (!string.IsNullOrEmpty(predicate.Field) && predicate.Value != null)
                                    condition = $"{predicate.Field} NOT LIKE '%{predicate.Value}'";
                                else
                                    throw new ArgumentException("Field or Value cannot be null for 'doesnotendwith' operator.");
                                break;
                            case "doesnotcontain":
                                if (!string.IsNullOrEmpty(predicate.Field) && predicate.Value != null)
                                    condition = $"{predicate.Field} NOT LIKE '%{predicate.Value}%'";
                                else
                                    throw new ArgumentException("Field or Value cannot be null for 'doesnotcontain' operator.");
                                break;

                            case "greaterthan":
                                if (!string.IsNullOrEmpty(predicate.Field) && predicate.Value != null)
                                    condition = $"{predicate.Field} > '{predicate.Value}'";
                                else
                                    throw new ArgumentException("Field or Value cannot be null for 'greaterthan' operator.");
                                break;

                            case "greaterthanorequal":
                                if (!string.IsNullOrEmpty(predicate.Field) && predicate.Value != null)
                                    condition = $"{predicate.Field} >= '{predicate.Value}'";
                                else
                                    throw new ArgumentException("Field or Value cannot be null for 'greaterthanorequal' operator.");
                                break;

                            case "lessthan":
                                if (!string.IsNullOrEmpty(predicate.Field) && predicate.Value != null)
                                    condition = $"{predicate.Field} < '{predicate.Value}'";
                                else
                                    throw new ArgumentException("Field or Value cannot be null for 'lessthan' operator.");
                                break;

                            case "lessthanorequal":
                                if (!string.IsNullOrEmpty(predicate.Field) && predicate.Value != null)
                                    condition = $"{predicate.Field} <= '{predicate.Value}'";
                                else
                                    throw new ArgumentException("Field or Value cannot be null for 'lessthanorequal' operator.");
                                break;

                            case "isnull":
                                if (string.IsNullOrEmpty(predicate.Field))
                                    throw new ArgumentException("Field cannot be null or empty.", nameof(predicate.Field));
                                condition = $"{predicate.Field} IS NULL";
                                break;

                            case "isnotnull":
                                if (string.IsNullOrEmpty(predicate.Field))
                                    throw new ArgumentException("Field cannot be null or empty.", nameof(predicate.Field));
                                condition = $"{predicate.Field} IS NOT NULL";
                                break;

                            case "isempty":
                                if (string.IsNullOrEmpty(predicate.Field))
                                    throw new ArgumentException("Field cannot be null or empty.", nameof(predicate.Field));
                                condition = $"{predicate.Field} = ''";
                                break;

                            case "isnotempty":
                                if (string.IsNullOrEmpty(predicate.Field))
                                    throw new ArgumentException("Field cannot be null or empty.", nameof(predicate.Field));
                                condition = $"{predicate.Field} != ''";
                                break;

                            case "between":
                                if (string.IsNullOrEmpty(predicate.Field))
                                    throw new ArgumentException("Field cannot be null or empty.", nameof(predicate.Field));
                                if (predicate.Value == null)
                                    throw new ArgumentException("Value cannot be null.", nameof(predicate.Value));
                                if (predicate.Value is Tuple<object, object> range && range.Item1 != null && range.Item2 != null)
                                {
                                    condition = $"{predicate.Field} BETWEEN '{range.Item1}' AND '{range.Item2}'";
                                }
                                else
                                {
                                    throw new ArgumentException("For 'between' operator, Value must be a tuple containing both start and end values.");
                                }
                                break;

                            default:
                                throw new InvalidOperationException($"Unsupported operator: {predicate.Operator}");
                        }
                    }
                    else
                    {
                        throw new ArgumentNullException(nameof(predicate.Operator), "Operator cannot be null or empty.");
                    }


                    if (!string.IsNullOrEmpty(condition))
                    {
                        conditions.Add(condition);
                    }
                }
            }
            string conjunction = parentCondition?.ToUpper() ?? "AND";
            return string.Join($" {conjunction} ", conditions);
        }

        //public string BuildWhereCondition(List<Predicate> predicates)
        //{

        //    StringBuilder sb = new StringBuilder();
        //    bool isFirstCondition = true;  // Flag to handle leading AND/OR conditions

        //    foreach (var predicate in predicates)
        //    {
        //        if (predicate.IsComplex && predicate.Predicates != null)
        //        {
        //            // Recursively build complex predicates
        //            string subClause = BuildWhereCondition(predicate.Predicates);

        //            // Wrap the sub-clause in parentheses if it's complex
        //            if (!string.IsNullOrWhiteSpace(subClause))
        //            {
        //                if (!isFirstCondition) sb.Append($" {predicate.Condition.ToUpper()} ");
        //                sb.Append($"({subClause})");
        //                isFirstCondition = false;
        //            }
        //        }
        //        else
        //        {
        //            if (predicate.Field != null && predicate.Value != null)
        //            {
        //                // Handle the operator and value for the field
        //                string operatorStr = GetSqlOperator(predicate.Operator);

        //                // Add the condition for this predicate
        //                if (!isFirstCondition) sb.Append($" {predicate.Condition.ToUpper()} ");
        //                sb.Append($"{predicate.Field} {operatorStr} '{predicate.Value}'");

        //                isFirstCondition = false;
        //            }
        //        }
        //    }

        //    // Return the final string without unnecessary spaces or trailing AND/OR
        //    return sb.ToString().Trim();

        //    //foreach (var predicate in predicates)
        //    //{
        //    //    if (predicate.IsComplex && predicate.Predicates != null)
        //    //    {
        //    //        // Recursively build complex predicates
        //    //        string subClause = BuildWhereClause(predicate.Predicates);
        //    //        if (predicate.Condition != null)
        //    //        {
        //    //            sb.Append($"({subClause}) {predicate.Condition.ToUpper()} ");
        //    //        }
        //    //    }
        //    //    else
        //    //    {
        //    //        if (predicate.Field != null && predicate.Value != null)
        //    //        {
        //    //            // Handle different types of conditions
        //    //            string operatorStr = "";
        //    //            switch (predicate.Operator)
        //    //            {
        //    //                case "equal":
        //    //                    operatorStr = "=";
        //    //                    break;
        //    //                case "greaterthan":
        //    //                    operatorStr = ">";
        //    //                    break;
        //    //                case "greaterthanorequal":
        //    //                    operatorStr = ">=";
        //    //                    break;
        //    //                default:
        //    //                    throw new InvalidOperationException($"Unsupported operator: {predicate.Operator ?? "null"}");
        //    //            }

        //    //            sb.Append($"{predicate.Field} {operatorStr} '{predicate.Value}' ");
        //    //            if (predicate.Condition != null)
        //    //            {
        //    //                sb.Append(predicate.Condition.ToUpper() + " ");
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //    //return sb.ToString().Trim();
        //}
        private string GetSqlOperator(string operatorStr)
        {
            // Return corresponding SQL operator
            // string operar = "";
            switch (operatorStr)
            {
                case "equal":
                    operatorStr = "=";
                    break;
                case "greaterthan":
                    operatorStr = ">";
                    break;
                case "greaterthanorequal":
                    operatorStr = ">=";
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported operator: {operatorStr ?? "null"}");
            }
            return operatorStr;
        }

        /// <summary>
        /// This method is return grid of Invoice
        /// </summary>
        /// <param name="radio"></param>
        /// <param name="txtstartdate"></param>
        /// <param name="txtenddate"></param>
        /// <param name="DrpLstdept"></param>
        /// <param name="AmtMaximum"></param>
        /// <param name="AmtMinimum"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Grid(string[] radio, string txtstartdate, string txtenddate, string DrpLstdept, string AmtMaximum = "0", string AmtMinimum = "0")
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
            Session["AmtMaximum"] = AmtMaximum;
            Session["AmtMinimum"] = AmtMinimum;
            Session["deptname"] = DrpLstdept;
            Session["startdate"] = txtstartdate;
            Session["enddate"] = txtenddate;
            Session["payment"] = paymethod;
            return RedirectToAction("Index", "Invoices", new { IsBindData = 1, currentPageIndex = 1, orderby = "InvoiceId", IsAsc = 0, PageSize = 50, SearchRecords = 1, Alpha = "", deptname = DrpLstdept, startdate = txtstartdate, enddate = txtenddate, payment = paymethod, Store_val = "", searchdashbord = "", AmtMaximum = AmtMaximum, AmtMinimum = AmtMinimum, SearchFlg = "F" });
        }

        /// <summary>
        /// This method is view of add Invoice
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,CreateInvoice")]
        // GET: Invoices/Create
        public ActionResult Create(string val)
        {
            ViewBag.Title = "Add Invoice - Synthesis";
            _CommonRepository.LogEntries();     //Harsh's code
            ViewBag.CreateIn = _CommonRepository.GetMessageValue("INICSS", "Invoice Created Successfully.");
            ViewBag.CreateInApprove = _CommonRepository.GetMessageValue("INSA", "Invoice Created Successfully and Sent for Approval.");
            ViewBag.SentApprove = _CommonRepository.GetMessageValue("INSS", "Invoice Created & Approved Successfully ");
            ViewBag.LeastOnedep = _CommonRepository.GetMessageValue("INLODA", "Enter at least In One Department Amount.");
            Invoice obj = new Invoice();
            try
            {
                ViewBag.CurrentDate = DateTime.Today.Date;
                //This class is get invoice object
                obj = _invoiceRepository.getInvoiceObj(obj, val);

                int? store = null;
                store = obj.StoreId;
                if (store == null || store == 0)
                {
                    //obj.InvoiceDate = DateTime.Today;
                    if (Convert.ToString(Session["storeid"]) != "0")
                    {
                        obj.StoreId = Convert.ToInt32(Session["storeid"]);
                        //ViewBag.StoreId = new SelectList(AdminSiteConfiguration.GetStoreList(1), "StoreId", "NickName", obj.StoreId);
                        //This class is get store list using role wise
                        ViewBag.StoreId = new SelectList(_CommonRepository.GetStoreList_RoleWise(1, "CreateInvoice", User.Identity.Name), "StoreId", "NickName", obj.StoreId);
                        //This db class is get Vendor master
                        ViewBag.VendorId = new SelectList(_MastersBindRepository.GetVendorMaster(obj.StoreId).OrderBy(o => o.VendorName).ToList(), "VendorId", "VendorName");
                        //This class is get department masters
                        ViewBag.Disc_Dept_id = new SelectList(_MastersBindRepository.GetDepartmentMasters(obj.StoreId).OrderBy(o => o.DepartmentName).ToList(), "DepartmentId", "DepartmentName");
                    }
                    else
                    {
                        //This class is get store list using role wise
                        ViewBag.StoreId = new SelectList(_CommonRepository.GetStoreList_RoleWise(1, "CreateInvoice", User.Identity.Name), "StoreId", "NickName");
                        ViewBag.VendorId = new SelectList("");
                        ViewBag.Disc_Dept_id = new SelectList("");
                    }
                    //Get Discount Type master
                    ViewBag.DiscountTypeId = new SelectList(_MastersBindRepository.GetDiscountTypeMaster(), "DiscountTypeId", "DiscountType", 1);
                    //Get Invoice type master
                    ViewBag.InvoiceTypeId = new SelectList(_MastersBindRepository.GetInvoiceTypeMaster(), "InvoiceTypeId", "InvoiceType");
                    ViewBag.PaymentTypeId = (Convert.ToString(Session["storeid"]) != "0" ? new SelectList(_MastersBindRepository.GetPaymentTypeMaster(), "PaymentTypeId", "PaymentType").Skip(0) : new SelectList(_MastersBindRepository.GetPaymentTypeMaster(), "PaymentTypeId", "PaymentType"));
                }
                else
                {
                    if (store != null)
                    {
                        obj.StoreId = (int)store;
                        //This class is get store list using role wise
                        ViewBag.StoreId = new SelectList(_CommonRepository.GetStoreList_RoleWise(1, "CreateInvoice", User.Identity.Name), "StoreId", "NickName", obj.StoreId);
                        //This db class is get Vendor master
                        ViewBag.VendorId = new SelectList(_MastersBindRepository.GetVendorMaster(obj.StoreId).OrderBy(o => o.VendorName).ToList(), "VendorId", "VendorName");
                        //This class is get department masters
                        ViewBag.Disc_Dept_id = new SelectList(_MastersBindRepository.GetDepartmentMasters(obj.StoreId).OrderBy(o => o.DepartmentName).ToList(), "DepartmentId", "DepartmentName");
                        //Get Discount Type master
                        ViewBag.DiscountTypeId = new SelectList(_MastersBindRepository.GetDiscountTypeMaster(), "DiscountTypeId", "DiscountType", 1);
                        //Get Invoice type master
                        ViewBag.InvoiceTypeId = new SelectList(_MastersBindRepository.GetInvoiceTypeMaster(), "InvoiceTypeId", "InvoiceType");
                        //Get Payment Type Master
                        ViewBag.PaymentTypeId = (store.ToString() != "0" ? new SelectList(_MastersBindRepository.GetPaymentTypeMaster(), "PaymentTypeId", "PaymentType").Skip(0) : new SelectList(_MastersBindRepository.GetPaymentTypeMaster(), "PaymentTypeId", "PaymentType"));
                    }
                    else
                    {
                        //This class is get store list using role wise
                        ViewBag.StoreId = new SelectList(_CommonRepository.GetStoreList_RoleWise(1, "CreateInvoice", User.Identity.Name), "StoreId", "NickName");
                        ViewBag.VendorId = new SelectList("");
                        ViewBag.Disc_Dept_id = new SelectList("");
                        //Get Discount Type master
                        ViewBag.DiscountTypeId = new SelectList(_MastersBindRepository.GetDiscountTypeMaster(), "DiscountTypeId", "DiscountType", 1);
                        //Get Invoice type master
                        ViewBag.InvoiceTypeId = new SelectList(_MastersBindRepository.GetInvoiceTypeMaster(), "InvoiceTypeId", "InvoiceType");
                        //Get Payment Type Master
                        ViewBag.PaymentTypeId = (store != null ? new SelectList(_MastersBindRepository.GetPaymentTypeMaster(), "PaymentTypeId", "PaymentType").Skip(0) : new SelectList(_MastersBindRepository.GetPaymentTypeMaster(), "PaymentTypeId", "PaymentType"));
                    }
                }
                if (StatusMessage == "Success1" || StatusMessage == "Success" || StatusMessage == "Exists" || StatusMessage == "Existence")
                {
                    ViewBag.StatusMessage = StatusMessage;
                    StatusMessage = "";
                }
                else
                {
                    ViewBag.StatusMessage = "";
                }

                ViewBag.closingyear = Convert.ToInt32(ConfigurationManager.AppSettings["ClosingYear"].ToString()) + 1;
            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - Create - " + DateTime.Now + " - " + ex.Message.ToString());
            }


            return View(obj);
        }
        public class HttpPostedFileBaseCustom : HttpPostedFileBase
        {
            MemoryStream stream;
            string contentType;
            string fileName;

            public HttpPostedFileBaseCustom(MemoryStream stream, string contentType, string fileName)
            {
                this.stream = stream;
                this.contentType = contentType;
                this.fileName = fileName;
            }

            public override int ContentLength
            {
                get { return (int)stream.Length; }
            }

            public override string ContentType
            {
                get { return contentType; }
            }

            public override string FileName
            {
                get { return fileName; }
            }

            public override Stream InputStream
            {
                get { return stream; }
            }
            public override void SaveAs(string filename)
            {
                using (var file = System.IO.File.Open(filename, FileMode.CreateNew))
                    stream.WriteTo(file);
            }
        }

        /// <summary>
        /// This method is Add Invoice.
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="UploadInvoice"></param>
        /// <param name="ChildDepartmentId"></param>
        /// <param name="ChildAmount"></param>
        /// <param name="btnsubmit"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,CreateInvoice")]
        public async Task<ActionResult> Create(Invoice invoice, HttpPostedFileBase UploadInvoice, string[] ChildDepartmentId, string[] ChildAmount, string btnsubmit = "")
        {
            try
            {
                if (invoice.CheckDup == 0 || invoice.CheckDup == 1)
                {
                    ChildAmount = ChildAmount.Select(s => string.IsNullOrWhiteSpace(s) ? "0" : s).ToArray();
                    var bulk = 0;
                    if (invoice.strInvoiceDate == null || invoice.strInvoiceDate == "")
                    {
                        ModelState.AddModelError("strInvoiceDate", "Required");
                    }
                    // Ignore City from ModelState.
                    invoice.QBTransfer = (invoice.QBtransferss == "1" ? true : false);
                    int Storeidval = 0;
                    if (invoice.DiscountTypeId == 1)
                    {
                        ModelState.Remove("Disc_Dept_id");
                    }
                    if (ModelState.IsValid)
                    {

                        if (UploadInvoice == null)
                        {
                            string filePath = invoice.UploadInvoice;
                            if (filePath.Contains(@"\") || filePath.Contains("/"))
                            {
                                byte[] bytes = System.IO.File.ReadAllBytes(HttpContext.Server.MapPath("~/UserFiles/Automatic-Invoice-File/" + invoice.UploadInvoice));
                                var contentTypeFile = "application/pdf";
                                var fileName = invoice.UploadInvoice;
                                UploadInvoice = (HttpPostedFileBase)new HttpPostedFileBaseCustom(new MemoryStream(bytes), contentTypeFile, fileName);
                                bulk = 1;
                            }
                            else
                            {
                                byte[] bytes = System.IO.File.ReadAllBytes(HttpContext.Server.MapPath("~/UserFiles/BulkUploadFile/" + invoice.UploadInvoice));
                                var contentTypeFile = "application/pdf";
                                var fileName = invoice.UploadInvoice;
                                UploadInvoice = (HttpPostedFileBase)new HttpPostedFileBaseCustom(new MemoryStream(bytes), contentTypeFile, fileName);
                                bulk = 1;
                            }

                        }
                        //This  class is used for Activity Log Insert.
                        string Store = _QBRepository.GetStoreOnlineDesktop(invoice.StoreId);
                        //Using this db class get stores on Line desktop Flag.
                        int StoreFlag = _QBRepository.GetStoreOnlineDesktopFlag(invoice.StoreId);

                        char[] MyChar = { '0' };
                        string Inoicno = invoice.InvoiceNumber.TrimStart(MyChar);
                        Console.WriteLine(Inoicno);
                        invoice.InvoiceNumber = Inoicno;
                        if (invoice.strInvoiceDate != null)
                        {
                            invoice.InvoiceDate = Convert.ToDateTime(invoice.strInvoiceDate);
                        }
                        if (Roles.IsUserInRole("Administrator"))
                        {
                            Storeidval = invoice.StoreId;
                        }
                        else
                        {
                            if (invoice.StoreId != 0)
                            {
                                Storeidval = invoice.StoreId;
                            }
                            else
                            {
                                if (Session["storeid"] == null)
                                {
                                    Session["storeid"] = "0";
                                }
                                if (Convert.ToString(Session["storeid"]) != "0")
                                {
                                    Storeidval = Convert.ToInt32(Session["storeid"].ToString());
                                }
                                else
                                {
                                    RedirectToAction("Index", "Login", new { area = "" });
                                }
                            }
                        }
                        #region AttachNote
                        //This class is Invoice Attach Note
                        int FlgInvAtch = InvoiceAttachNote(invoice, UploadInvoice);
                        if (FlgInvAtch == 1)
                        {
                            ViewBag.StatusMessage = "InvalidImage";
                            return View(invoice);
                        }
                        else if (FlgInvAtch == 2)
                        {
                            ViewBag.StatusMessage = "InvalidPDFSize";
                            return View(invoice);
                        }
                        #endregion
                        int iInvoiceId = 0; string iInvoiceStatus = "";
                        //---------- User Aprrove Rights Code Check----------/// 
                        //This class is get Module Masters Id
                        var ModuleId = _invoiceRepository.GetModuleMastersId();
                        bool roleFlg = false;
                        try
                        {
                            if (!Roles.IsUserInRole("Administrator"))
                            {
                                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                                int UserTypeId = _CommonRepository.getUserTypeId(UserName);
                                if (UserTypeId > 0)
                                {
                                    //this db class is check user type module Approvers
                                    if (_invoiceRepository.CheckUserTypeModuleApprovers(UserTypeId, ModuleId))
                                    {
                                        roleFlg = true;
                                    }
                                }
                            }


                        }
                        catch (Exception ex)
                        {
                            logger.Error("InvoicesController - Create - " + DateTime.Now + " - " + ex.Message.ToString());
                        }
                        bool FLG = GetroleForCRApproval("ApproveInvoice", Convert.ToInt32(invoice.StoreId), 1);

                        //------------------------End-------------------------------//
                        //This class is Create Invoice Post
                        InvoiceFlgs invoiceFlgs = _invoiceRepository.CreateInvoicePost(invoice, Roles.IsUserInRole("Administrator"), Roles.IsUserInRole("nnfapprovalInvoice"), FLG, roleFlg, Store, StoreFlag, ChildDepartmentId, ChildAmount, Storeidval, User.Identity.Name);
                        bool QuickCRInvoice = GetroleForCRApproval("nnfapprovalCrdMemoInvoice", invoice.StoreId, 1);
                        try
                        {
                            if (Convert.ToInt32(invoiceFlgs.iInvoiceId) > 0)
                            {
                                if (ChildDepartmentId != null)
                                {
                                    int j = 0;
                                    foreach (var val_id in ChildDepartmentId)
                                    {
                                        if (!String.IsNullOrEmpty(ChildAmount[j].ToString()))
                                        {
                                            if (Convert.ToDecimal(ChildAmount[j]) > 0) // Remove "=" Sign to Stop 0 Amount Department
                                            {
                                                InvoiceDepartmentDetail deptDetail = new InvoiceDepartmentDetail();
                                                deptDetail.InvoiceId = invoiceFlgs.iInvoiceId;
                                                deptDetail.DepartmentId = Convert.ToInt32(val_id);
                                                deptDetail.Amount = Convert.ToDecimal(ChildAmount[j]);
                                                //Insert Invoice Department Details.
                                                _invoiceRepository.InsertInvoiceDepartmentDetails(deptDetail);
                                            }
                                        }
                                        j++;
                                    }
                                }
                                if (invoice.QuickInvoice == "1")
                                {
                                    ActivityLogMessage = "Quick Invoice Number " + "<a href='/Invoices/Details/" + invoice.InvoiceId + "'>" + invoice.InvoiceNumber + "</a> Created by " + UserModule.getUserFirstName() + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                                }
                                else
                                {
                                    ActivityLogMessage = "Invoice Number " + "<a href='/Invoices/Details/" + invoice.InvoiceId + "'>" + invoice.InvoiceNumber + "</a> Created by " + UserModule.getUserFirstName() + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                                }
                                ActivityLog ActLog1 = new ActivityLog();
                                ActLog1.UserId = _CommonRepository.getUserId(User.Identity.Name);
                                ActLog1.Action = 1;
                                ActLog1.Comment = ActivityLogMessage;
                                //This  class is used for Activity Log Insert.
                                _ActivityLogRepository.ActivityLogInsert(ActLog1);

                                IsArray = false;
                                if (invoice.QuickInvoice == "1")
                                {
                                    StatusMessage = "Success1";
                                    ViewBag.StatusMessage = StatusMessage;
                                }
                                else
                                {
                                    StatusMessage = "Success";
                                    ViewBag.StatusMessage = StatusMessage;
                                }


                                if (invoice.DiscountTypeId != 1)
                                {
                                    //db.Database.ExecuteSqlCommand("SP_InvoiceDiscount @Mode = {0},@InvoiceId = {1},@DiscountTypeId = {2},@DiscountAmount = {3},@DiscountPercent = {4}", "UpdateDiscountDetail", iInvoiceId, invoice.DiscountTypeId, invoice.DiscountAmount, (invoice.DiscountPercent == null ? 0 : invoice.DiscountPercent));
                                    _invoiceRepository.SaveInvoiceDiscount(invoiceFlgs.iInvoiceId, invoice.DiscountTypeId, invoice.DiscountAmount, (invoice.DiscountPercent == null ? 0 : invoice.DiscountPercent));
                                    int iiInvoiceID = 0;
                                    var DSacn_Title = "";
                                    string InvoiceNM = "";
                                    if (UploadInvoice != null)
                                    {
                                        if (UploadInvoice.ContentLength > 0)
                                        {
                                            var allowedExtensions = new[] { ".pdf" };
                                            var extension = Path.GetExtension(UploadInvoice.FileName);
                                            var Ext = Convert.ToString(extension).ToLower();
                                            if (!allowedExtensions.Contains(Ext))
                                            {
                                                ViewBag.StatusMessage = "InvalidImage";
                                                if (bulk == 1)
                                                {
                                                    return View("InvoiceAutomationGrid", "Invoices");
                                                }
                                                else
                                                {
                                                    return View("IndexBeta");
                                                }
                                            }
                                            else
                                            {
                                                string FileName = invoice.InvoiceDate.ToString("MMddyyyy") + "-" + _MastersBindRepository.GetVendorRemoveSpecialCarecters(invoice.VendorId) + "-" + AdminSiteConfiguration.RemoveSpecialCharacterInvoice(Inoicno) + "-" + "CR" + "-" + DateTime.Now.Ticks.ToString() + ".pdf";
                                                //"CR" + "-" + AdminSiteConfiguration.RemoveSpecialCharacterInvoice(Inoicno) + "-" + invoice.InvoiceDate.ToString("MMddyyyy") + "-" + GetVendorName(invoice.VendorId) + ".pdf";
                                                string PathRel = CreateDirectory(invoice.InvoiceDate.ToString("MMddyyyy"), invoice.VendorId, invoice.StoreId);
                                                string FullPath = PathRel + "\\" + FileName;
                                                UploadInvoice.SaveAs(FullPath);
                                                InvoiceNM = FullPath.Replace(Request.PhysicalApplicationPath + "UserFiles\\Invoices\\", "");

                                            }
                                        }
                                    }
                                    string CRdiscount = "";
                                    string Credit_Invoiceno = invoice.InvoiceNumber + "_cr";
                                    if (invoice.DiscountTypeId == 2)
                                    {
                                        CRdiscount = invoice.DiscountPercent.ToString();
                                        Credit_Invoiceno = Credit_Invoiceno + CRdiscount + "%";
                                    }
                                    //This db class is Create Invoice Credit memo post
                                    iiInvoiceID = _invoiceRepository.CreateInvoiceCreditmemoPost(invoice, Roles.IsUserInRole("ApproveInvoice"), Roles.IsUserInRole("Administrator"), Store, StoreFlag, User.Identity.Name, invoiceFlgs.iInvoiceId, Credit_Invoiceno, InvoiceNM, invoiceFlgs.iInvoiceStatus);

                                    if (iiInvoiceID > 0)
                                    {
                                        if (invoice.QuickInvoice == "1")
                                        {
                                            ActivityLogMessage = "Quick Invoice Number " + "<a href='/Invoices/Details/" + invoice.InvoiceId + "'>" + invoice.InvoiceNumber + "</a> Created by " + UserModule.getUserFirstName() + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                                        }
                                        else
                                        {
                                            ActivityLogMessage = "Invoice Number " + "<a href='/Invoices/Details/" + invoice.InvoiceId + "'>" + invoice.InvoiceNumber + "</a> Created by " + UserModule.getUserFirstName() + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                                        }

                                        ActivityLog ActLog2 = new ActivityLog();
                                        ActLog2.UserId = _CommonRepository.getUserId(User.Identity.Name);
                                        ActLog2.Action = 1;
                                        ActLog2.Comment = ActivityLogMessage;
                                        //This  class is used for Activity Log Insert.
                                        _ActivityLogRepository.ActivityLogInsert(ActLog2);
                                    }
                                }
                                if (invoice.btnName == "Save & New")
                                {
                                    invoice.DepartmentMasters = _invoiceRepository.getDepartment_WithSP(invoice.StoreId).ToList();
                                    ViewBag.Disc_Dept_id = new SelectList(_MastersBindRepository.GetAllDepartmentMasters().OrderBy(o => o.DepartmentName).ToList(), "DepartmentId", "DepartmentName", invoice.Disc_Dept_id);
                                    ViewBag.DiscountTypeId = new SelectList(_MastersBindRepository.GetDiscountTypeMaster(), "DiscountTypeId", "DiscountType", invoice.DiscountTypeId);
                                    ViewBag.InvoiceTypeId = new SelectList(_MastersBindRepository.GetInvoiceTypeMaster(), "InvoiceTypeId", "InvoiceType", invoice.InvoiceTypeId);
                                    ViewBag.PaymentTypeId = (Convert.ToString(Session["storeid"]) != "0" ? new SelectList(_MastersBindRepository.GetPaymentTypeMaster(), "PaymentTypeId", "PaymentType", invoice.PaymentTypeId).Skip(0) : new SelectList(_MastersBindRepository.GetPaymentTypeMaster(), "PaymentTypeId", "PaymentType", invoice.PaymentTypeId));
                                    //new SelectList(db.PaymentTypeMasters, "PaymentTypeId", "PaymentType", invoice.PaymentTypeId);
                                    ViewBag.StoreId = new SelectList(_CommonRepository.GetStoreList(1), "StoreId", "NickName", invoice.StoreId);
                                    ViewBag.VendorId = new SelectList(_MastersBindRepository.GetVendorMaster(invoice.StoreId).Select(s => new { s.VendorId, s.VendorName }).ToList(), "VendorId", "VendorName", invoice.VendorId);
                                }
                                else
                                {
                                    string message = StatusMessage;
                                    if ((invoice.QuickInvoice == "1" && ((Roles.IsUserInRole("Administrator")) || Roles.IsUserInRole("nnfapprovalInvoice"))) || (FLG == true && roleFlg == false))
                                    {
                                        message = "Success2";
                                    }
                                    if (invoice.DiscountTypeId != 1 && QuickCRInvoice == true)
                                    {
                                        message = "Success2";
                                    }
                                    //StatusMessage = "";
                                    if (bulk == 1)
                                    {
                                        StatusMessage = "";
                                        return RedirectToAction("InvoiceAutomationGrid", "Invoices", new { MSG = message });
                                    }
                                    else
                                    {
                                        return RedirectToAction("IndexBeta", "Invoices", new { MSG = message });
                                    }
                                }
                            }
                            else
                            {
                                return RedirectToAction("Index", "Login", new { area = "" });
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error("InvoicesController - Create - " + DateTime.Now + " - " + ex.Message.ToString());
                        }
                        if (Roles.IsUserInRole("ApproveInvoice") && roleFlg == false && invoice.btnName == "Save & New")
                        {
                            StatusMessage = "Success2";
                            ViewBag.StatusMessage = StatusMessage;
                            return RedirectToAction("Create");
                        }
                        else if (invoice.btnName == "Save & New")
                        {
                            StatusMessage = "Success1";
                            ViewBag.StatusMessage = StatusMessage;
                            return RedirectToAction("Create");
                        }
                        else
                        {
                            StatusMessage = "Success1";
                            ViewBag.StatusMessage = StatusMessage;
                            if (bulk == 1)
                            {
                                StatusMessage = "";
                                return RedirectToAction("InvoiceAutomationGrid", "Invoices", new { MSG = StatusMessage });
                            }
                            else
                            {
                                return RedirectToAction("IndexBeta");
                            }
                        }
                    }
                    else
                    {
                        invoice.DepartmentMasters = _invoiceRepository.getDepartment_WithSP(invoice.StoreId).ToList();
                        var DepartmentList = _invoiceRepository.getDepartment_WithSP(invoice.StoreId).ToList();
                        ViewBag.Disc_Dept_id = new SelectList(DepartmentList.Where(s => s.StoreId == invoice.StoreId).ToList(), "DepartmentId", "DepartmentName", invoice.Disc_Dept_id);

                        ViewBag.DiscountTypeId = new SelectList(_MastersBindRepository.GetDiscountTypeMaster(), "DiscountTypeId", "DiscountType", invoice.DiscountTypeId);
                        ViewBag.InvoiceTypeId = new SelectList(_MastersBindRepository.GetInvoiceTypeMaster(), "InvoiceTypeId", "InvoiceType", invoice.InvoiceTypeId);
                        ViewBag.PaymentTypeId = (Convert.ToString(Session["storeid"]) != "0" ? new SelectList(_MastersBindRepository.GetPaymentTypeMaster(), "PaymentTypeId", "PaymentType", invoice.PaymentTypeId).Skip(0) : new SelectList(_MastersBindRepository.GetPaymentTypeMaster(), "PaymentTypeId", "PaymentType", invoice.PaymentTypeId));

                        ViewBag.StoreId = new SelectList(_CommonRepository.GetStoreList(1), "StoreId", "NickName", invoice.StoreId);
                        ViewBag.VendorId = new SelectList(_MastersBindRepository.GetVendorMaster(invoice.StoreId).Select(s => new { s.VendorId, s.VendorName }).ToList(), "VendorId", "VendorName", invoice.VendorId);

                        return View("Create", invoice);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - Create - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return RedirectToAction("IndexBeta");
        }
        /// <summary>
        /// This class is The Invoice Attach Note
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="UploadInvoice"></param>
        /// <returns></returns>
        public int InvoiceAttachNote(Invoice invoice, HttpPostedFileBase UploadInvoice)
        {
            int returnFlg = 0;
            #region AttachNote
            var Sacn_Title = "";
            if (UploadInvoice != null)
            {
                try
                {
                    if (UploadInvoice.ContentLength > 0)
                    {
                        var allowedExtensions = new[] { ".pdf" };
                        var extension = Path.GetExtension(UploadInvoice.FileName);
                        var Ext = Convert.ToString(extension).ToLower();
                        if (!allowedExtensions.Contains(Ext))
                        {
                            ViewBag.StatusMessage = "InvalidImage";
                            invoice.DepartmentMasters = _invoiceRepository.getDepartment_WithSP(invoice.StoreId).ToList();
                            //This class is Get All department masters.
                            ViewBag.Disc_Dept_id = new SelectList(_MastersBindRepository.GetAllDepartmentMasters().OrderBy(o => o.DepartmentName).ToList(), "DepartmentId", "DepartmentName", invoice.Disc_Dept_id);
                            //This class is Get Discount Type Master.
                            ViewBag.DiscountTypeId = new SelectList(_MastersBindRepository.GetDiscountTypeMaster(), "DiscountTypeId", "DiscountType", invoice.DiscountTypeId);
                            //This class is Get invoice Type master.
                            ViewBag.InvoiceTypeId = new SelectList(_MastersBindRepository.GetInvoiceTypeMaster(), "InvoiceTypeId", "InvoiceType", invoice.InvoiceTypeId);
                            ViewBag.PaymentTypeId = (Convert.ToString(Session["storeid"]) != "0" ? new SelectList(_MastersBindRepository.GetPaymentTypeMaster(), "PaymentTypeId", "PaymentType", invoice.PaymentTypeId).Skip(0) : new SelectList(_MastersBindRepository.GetPaymentTypeMaster(), "PaymentTypeId", "PaymentType", invoice.PaymentTypeId));
                            //new SelectList(db.PaymentTypeMasters, "PaymentTypeId", "PaymentType", invoice.PaymentTypeId);
                            ViewBag.StoreId = new SelectList(_CommonRepository.GetStoreList(1), "StoreId", "NickName", invoice.StoreId);
                            ViewBag.VendorId = new SelectList(_MastersBindRepository.GetVendorMaster(invoice.StoreId).Select(s => new { s.VendorId, s.VendorName }).ToList(), "VendorId", "VendorName", invoice.VendorId);
                            returnFlg = 1;
                            return returnFlg;
                        }
                        else
                        {
                            if (UploadInvoice.ContentLength > 50971520)
                            {
                                returnFlg = 2;
                                return returnFlg;
                            }
                            else
                            {
                                try
                                {
                                    //Sacn_Title = AdminSiteConfiguration.GetRandomNo() + Path.GetFileName(UploadInvoice.FileName);
                                    //Sacn_Title = AdminSiteConfiguration.RemoveSpecialCharacter(Sacn_Title);
                                    //string path1 = Request.PhysicalApplicationPath + "UserFiles\\Invoices" + "\\" + Sacn_Title;
                                    string FileName = invoice.InvoiceDate.ToString("MMddyyyy") + "-" + _MastersBindRepository.GetVendorRemoveSpecialCarecters(invoice.VendorId) + "-" + AdminSiteConfiguration.RemoveSpecialCharacterInvoice(invoice.InvoiceNumber) + "-" + (invoice.InvoiceTypeId == 1 ? "INV" : "CR") + "-" + DateTime.Now.Ticks.ToString() + ".pdf";
                                    //(invoice.InvoiceTypeId == 1 ? "INV" : "CR") + "-" + AdminSiteConfiguration.RemoveSpecialCharacterInvoice(Inoicno) + "-" + invoice.InvoiceDate.ToString("MMddyyyy") +"-"+ GetVendorName(invoice.VendorId) +".pdf";
                                    string PathRel = CreateDirectory(invoice.InvoiceDate.ToString("MMddyyyy"), invoice.VendorId, invoice.StoreId);
                                    string FullPath = PathRel + "\\" + FileName;
                                    UploadInvoice.SaveAs(FullPath);
                                    var InvoiceNM = FullPath.Replace(Request.PhysicalApplicationPath + "UserFiles\\Invoices\\", "");
                                    invoice.UploadInvoice = InvoiceNM;

                                    if (!String.IsNullOrEmpty(Convert.ToString(invoice.UploadPdfAutomationId)))
                                    {
                                        //This db class is upload pdf by Id.
                                        _invoiceRepository.UploadAutomationPdfId(invoice.UploadPdfAutomationId);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logger.Error("InvoicesController - InvoiceAttachNote - " + DateTime.Now + " - " + ex.Message.ToString());
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    logger.Error("InvoicesController - InvoiceAttachNote - " + DateTime.Now + " - " + ex.Message.ToString());
                }

            }
            return returnFlg;
            #endregion

        }

        /// <summary>
        /// This class is Remove Invoice directory
        /// </summary>
        /// <param name="InvoicePath"></param>
        private void RemoveDirectory(string InvoicePath)
        {
            var RootURL = Request.PhysicalApplicationPath + "UserFiles\\Invoices" + "\\";
            try
            {
                if (InvoicePath != "")
                {
                    var FullURL = RootURL + InvoicePath;
                    FileInfo file = new FileInfo(FullURL);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - RemoveDirectory - " + DateTime.Now + " - " + ex.Message.ToString());
            }

        }
        /// <summary>
        /// This class is Create Invoice directory
        /// </summary>
        /// <param name="Date"></param>
        /// <param name="VendorId"></param>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        public string CreateDirectory(string Date, int VendorId, int StoreId)
        {
            var RootURL = Request.PhysicalApplicationPath + "UserFiles\\Invoices" + "\\";

            var RelationalURL = RootURL + Date.Substring(4, 4);
            try
            {
                if (!(Directory.Exists(RelationalURL)))
                {
                    Directory.CreateDirectory(RelationalURL);
                    RelationalURL = RelationalURL + "\\" + StoreId;
                    if (!(Directory.Exists(RelationalURL)))
                    {
                        Directory.CreateDirectory(RelationalURL);
                        //Get Month Name
                        RelationalURL = RelationalURL + "\\" + _CommonRepository.GetMonthName(Convert.ToInt32(Date.Substring(0, 2)));
                        if (!(Directory.Exists(RelationalURL)))
                        {
                            Directory.CreateDirectory(RelationalURL);
                            RelationalURL = RelationalURL + "\\" + VendorId;
                            if (!(Directory.Exists(RelationalURL)))
                            {
                                Directory.CreateDirectory(RelationalURL);
                            }
                        }
                    }
                }
                else
                {
                    RelationalURL = RelationalURL + "\\" + StoreId;
                    if (!(Directory.Exists(RelationalURL)))
                    {
                        Directory.CreateDirectory(RelationalURL);
                        //Get Month Name
                        RelationalURL = RelationalURL + "\\" + _CommonRepository.GetMonthName(Convert.ToInt32(Date.Substring(0, 2)));
                        if (!(Directory.Exists(RelationalURL)))
                        {
                            Directory.CreateDirectory(RelationalURL);
                            RelationalURL = RelationalURL + "\\" + VendorId;
                            if (!(Directory.Exists(RelationalURL)))
                            {
                                Directory.CreateDirectory(RelationalURL);
                            }
                        }
                    }
                    else
                    {
                        //Get Month Name
                        RelationalURL = RelationalURL + "\\" + _CommonRepository.GetMonthName(Convert.ToInt32(Date.Substring(0, 2)));
                        if (!(Directory.Exists(RelationalURL)))
                        {
                            Directory.CreateDirectory(RelationalURL);
                            RelationalURL = RelationalURL + "\\" + VendorId;
                            if (!(Directory.Exists(RelationalURL)))
                            {
                                Directory.CreateDirectory(RelationalURL);
                            }
                        }
                        else
                        {
                            RelationalURL = RelationalURL + "\\" + VendorId;
                            if (!(Directory.Exists(RelationalURL)))
                            {
                                Directory.CreateDirectory(RelationalURL);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - CreateDirectory - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return RelationalURL;
        }


        /// <summary>
        /// This class is copy Invoice directory 
        /// </summary>
        /// <param name="Date"></param>
        /// <param name="VendorId"></param>
        /// <param name="StoreId"></param>
        /// <param name="CurrentPath"></param>
        /// <param name="InvoiceTypeId"></param>
        /// <param name="InvoiceNo"></param>
        /// <returns></returns>
        public string CopyDirectory(string Date, int VendorId, int StoreId, string CurrentPath, int InvoiceTypeId, string InvoiceNo)
        {
            var TargetURL = "";
            var RootURL = Request.PhysicalApplicationPath + "UserFiles\\Invoices" + "\\";
            try
            {
                if (CurrentPath != "")
                {
                    var FullURL = RootURL + CurrentPath;
                    FileInfo file = new FileInfo(FullURL);
                    if (file.Exists)
                    {
                        var FileName = FullURL.Split('\\');
                        var File = "";
                        if (FileName.Count() > 0)
                        {
                            File = FileName[FileName.Count() - 1];
                        }
                        string InvNo = InvoiceNo;
                        if (InvoiceTypeId == 2)
                        {
                            var InvSptData = InvoiceNo.Split('_');
                            if (InvSptData.Count() > 0)
                            {
                                InvNo = InvSptData[0];
                            }
                        }
                        //Db class is get vendor Name
                        string NewFile = Date + "-" + _MastersBindRepository.GetVendorName(VendorId) + "-" + AdminSiteConfiguration.RemoveSpecialCharacterInvoice(InvNo) + "-" + (InvoiceTypeId == 1 ? "INV" : "CR") + "-" + DateTime.Now.Ticks.ToString() + ".pdf";
                        TargetURL = CreateDirectory(Date, VendorId, StoreId) + "\\" + NewFile;
                        file.MoveTo(TargetURL);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - CopyDirectory - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return TargetURL.Replace(Request.PhysicalApplicationPath + "UserFiles\\Invoices\\", "");
        }

        /// <summary>
        /// This method is get Invoice details by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,UpdateInvoice")]
        // GET: Invoices/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.CurrentDate = DateTime.Today.Date;
            Invoice invoice = new Invoice();
            try
            {
                //This class is Edit Inovoice Object
                invoice = _invoiceRepository.EditInvoiceObj(id.Value);
                if (invoice == null)
                {
                    return HttpNotFound();
                }
                Session["storeid"] = invoice.StoreId;

                if (invoice.StatusValue == InvoiceStatusEnm.Approved)
                {
                    //This db class is get Invoice master
                    ViewBag.InvoiceTypeId = new SelectList(_MastersBindRepository.GetInvoiceTypeMaster().Where(s => s.InvoiceTypeId == invoice.InvoiceTypeId), "InvoiceTypeId", "InvoiceType", invoice.InvoiceTypeId);
                }
                else
                {
                    //This db class is get Invoice master
                    ViewBag.InvoiceTypeId = new SelectList(_MastersBindRepository.GetInvoiceTypeMaster(), "InvoiceTypeId", "InvoiceType", invoice.InvoiceTypeId);
                }
                //This class is get department masters
                ViewBag.Disc_Dept_id = new SelectList(_MastersBindRepository.GetDepartmentMasters(invoice.StoreId).OrderBy(o => o.DepartmentName).ToList(), "DepartmentId", "DepartmentName", invoice.Disc_Dept_id);

                //Get Discount Type master
                ViewBag.DiscountTypeId = new SelectList(_MastersBindRepository.GetDiscountTypeMaster(), "DiscountTypeId", "DiscountType", invoice.DiscountTypeId);
                //Get Payment Type Master
                ViewBag.PaymentTypeId = new SelectList(_MastersBindRepository.GetPaymentTypeMaster().Where(s => s.PaymentTypeId == invoice.PaymentTypeId), "PaymentTypeId", "PaymentType", invoice.PaymentTypeId);
                //This class is Get Storelist
                ViewBag.StoreId = new SelectList(_CommonRepository.GetStoreList(1), "StoreId", "NickName", invoice.StoreId);
                //This db class is get Vendor master
                ViewBag.VendorId = new SelectList(_MastersBindRepository.GetVendorMaster(invoice.StoreId).Select(s => new { s.VendorId, s.VendorName }).ToList().OrderBy(o => o.VendorName), "VendorId", "VendorName", invoice.VendorId);


                ViewBag.closingyear = Convert.ToInt32(ConfigurationManager.AppSettings["ClosingYear"].ToString()) + 1;
            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - Edit - " + DateTime.Now + " - " + ex.Message.ToString());
            }


            return View(invoice);
        }
        /// <summary>
        /// This method is update Invoice
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="UploadInvoice"></param>
        /// <param name="ChildDepartmentId"></param>
        /// <param name="ChildAmount"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,UpdateInvoice")]
        public async Task<ActionResult> Edit(Invoice invoice, HttpPostedFileBase UploadInvoice, string[] ChildDepartmentId, string[] ChildAmount)
        {
            try
            {
                if (invoice.CheckDup == 0 || invoice.CheckDup == 1)
                {
                    ChildAmount = ChildAmount.Select(s => string.IsNullOrWhiteSpace(s) ? "0" : s).ToArray();
                    if (invoice.strInvoiceDate != null)
                    {
                        invoice.InvoiceDate = Convert.ToDateTime(invoice.strInvoiceDate);
                    }
                    if (invoice.DiscountTypeId == 1)
                    {
                        ModelState.Remove("Disc_Dept_id");
                    }
                    //invoice.QBTransfer = (invoice.QBTransfer == false ? true : false);
                    if (invoice.QBtransferss == "1")
                    {
                        invoice.QBTransfer = true;
                    }
                    else
                    {
                        invoice.QBTransfer = false;
                    }
                    if (ModelState.IsValid)
                    {
                        char[] MyChar = { '0' };
                        string Inoicno = invoice.InvoiceNumber.TrimStart(MyChar);
                        Console.WriteLine(Inoicno);
                        invoice.InvoiceNumber = Inoicno;
                        bool FltgExist = false;
                        //Db class is Check invoice Exist.
                        int exists = _invoiceRepository.CheckInvoiceExist(invoice);
                        //This class is check Invoice Exist Amount Details
                        var existsbyAmt = _invoiceRepository.CheckInvoiceExistAmountDetails(invoice);
                        if (Convert.ToInt32(exists) == 0 && Convert.ToInt32(existsbyAmt.InvoiceId) != 0)
                        {
                            FltgExist = true;
                        }
                        if (FltgExist == false)
                        {
                            #region AttachNote
                            string ReturnFlg = EditInvoiceAttachNote(invoice, UploadInvoice);
                            if (ReturnFlg == "InvalidImage")
                            {
                                ViewBag.StatusMessage = "InvalidImage";
                                return View(invoice);
                            }
                            else if (ReturnFlg == "InvalidPDFSize")
                            {
                                ViewBag.StatusMessage = "InvalidPDFSize";
                                return View(invoice);
                            }
                            #endregion
                            //This class is save child Department
                            invoice = _invoiceRepository.ChildDepartmentSave(invoice, ChildDepartmentId, ChildAmount, User.Identity.Name);
                            //This class is save Invoice Discount 
                            _invoiceRepository.SaveInvoiceDiscountEdit(invoice);
                            //This  class is used for Activity Log Insert.
                            string Store = _QBRepository.GetStoreOnlineDesktop(Convert.ToInt32(invoice.StoreId));
                            //Using this db class get stores on Line desktop Flag.
                            int Flag = _QBRepository.GetStoreOnlineDesktopFlag(Convert.ToInt32(invoice.StoreId));
                            string success = "";
                            try
                            {
                                success = _invoiceRepository.InvoiceQBSync(invoice, ChildDepartmentId, ChildAmount, Store, Flag);
                            }
                            catch (Exception ex)
                            {
                                logger.Error("InvoicesController - Edit - " + DateTime.Now + " - " + ex.Message.ToString());
                                invoice.DepartmentMasters = _invoiceRepository.getDepartment_WithSP(invoice.StoreId).ToList();
                                ViewBag.Disc_Dept_id = new SelectList(_MastersBindRepository.GetAllDepartmentMasters().OrderBy(o => o.DepartmentName).ToList(), "DepartmentId", "DepartmentName", invoice.Disc_Dept_id);
                                ViewBag.DiscountTypeId = new SelectList(_MastersBindRepository.GetDiscountTypeMaster(), "DiscountTypeId", "DiscountType", invoice.DiscountTypeId);
                                ViewBag.InvoiceTypeId = new SelectList(_MastersBindRepository.GetInvoiceTypeMaster().Where(s => s.InvoiceTypeId == invoice.InvoiceTypeId), "InvoiceTypeId", "InvoiceType", invoice.InvoiceTypeId);
                                ViewBag.PaymentTypeId = new SelectList(_MastersBindRepository.GetPaymentTypeMaster().Where(s => s.PaymentTypeId == invoice.PaymentTypeId), "PaymentTypeId", "PaymentType", invoice.PaymentTypeId);
                                ViewBag.StoreId = new SelectList(_CommonRepository.GetStoreList(1), "StoreId", "NickName", invoice.StoreId);
                                ViewBag.VendorId = new SelectList(_MastersBindRepository.GetVendorMaster(invoice.StoreId).Select(s => new { s.VendorId, s.VendorName }).ToList().OrderBy(o => o.VendorName), "VendorId", "VendorName", invoice.VendorId);
                            }
                        }

                        if (invoice.InvoiceId != 0)
                        {
                            ActivityLogMessage = "Invoice Note with this Invoice Number " + "<a href='/Invoices/Details/" + invoice.InvoiceId + "'>" + invoice.InvoiceNumber + "</a> Edited by " + UserModule.getUserFirstName() + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());

                            try
                            {
                                ActivityLog ActLog1 = new ActivityLog();
                                ActLog1.UserId = _CommonRepository.getUserId(User.Identity.Name);
                                ActLog1.Action = 1;
                                ActLog1.Comment = ActivityLogMessage;
                                //This  class is used for Activity Log Insert.
                                _ActivityLogRepository.ActivityLogInsert(ActLog1);

                                #region creditmemo
                                if (invoice.DiscountTypeId != 1 && invoice.DiscountTypeId != null)
                                {
                                    string CRdiscount = "";
                                    string Credit_Invoiceno = invoice.InvoiceNumber + "_cr";
                                    if (invoice.DiscountTypeId == 2)
                                    {
                                        CRdiscount = invoice.DiscountPercent.ToString();
                                        Credit_Invoiceno = Credit_Invoiceno + CRdiscount + "%";
                                    }
                                    string sucess1 = "";
                                    #region AttachNote

                                    AttachmentNoteCls attachmentNoteCls = EditInvoiceAttachNoteCR(invoice, UploadInvoice);
                                    if (attachmentNoteCls.StatusMessage == "InvalidImage")
                                    {
                                        ViewBag.StatusMessage = "InvalidImage";
                                        return View("IndexBeta");
                                    }
                                    string scandocument = attachmentNoteCls.InvoiceNM;
                                    #endregion
                                    var ModuleId = _invoiceRepository.GetModuleMastersId();
                                    bool roleFlg = false;
                                    if (!Roles.IsUserInRole("Administrator"))
                                    {
                                        var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                                        int UserTypeId = _CommonRepository.getUserTypeId(UserName);
                                        if (UserTypeId > 0)
                                        {
                                            if (_invoiceRepository.CheckUserTypeModuleApprovers(UserTypeId, ModuleId))
                                            {
                                                roleFlg = true;
                                            }
                                        }
                                    }

                                    bool QuickCRInvoice = GetroleForCRApproval("nnfapprovalCrdMemoInvoice", Convert.ToInt32(invoice.StoreId), 1);
                                    sucess1 = _invoiceRepository.InvoiceCreditmemoSaveEdit(invoice, QuickCRInvoice, Roles.IsUserInRole("Administrator"), User.Identity.Name, Credit_Invoiceno, scandocument);

                                }
                                #endregion
                                StatusMessage = "SuccessEdit";
                            }
                            catch (Exception ex)
                            {
                                logger.Error("InvoicesController - Edit - " + DateTime.Now + " - " + ex.Message.ToString());
                            }

                        }
                        ActivityLog ActLog = new ActivityLog();
                        ActLog.Action = 2;
                        ActLog.Comment = "Invoice " + "<a href='/Invoices/Details/" + invoice.InvoiceId + "'>" + invoice.InvoiceNumber + "</a> Edited by " + UserModule.getUserFirstName() + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                        //This  class is used for Activity Log Insert.
                        _ActivityLogRepository.ActivityLogInsert(ActLog);

                        StatusMessage = "SuccessEdit";
                        ViewBag.StatusMessage = StatusMessage;
                        if (invoice.FromInvoicePage == "InvoiceReport")
                        {
                            return RedirectToAction("Index", "InvoiceReport");
                        }
                        else
                        {
                            return RedirectToAction("IndexBeta");
                        }
                    }
                    //This class is Get Department with sp.
                    invoice.DepartmentMasters = _invoiceRepository.getDepartment_WithSP(invoice.StoreId).ToList();
                    //This class is Get all department master.
                    ViewBag.Disc_Dept_id = new SelectList(_MastersBindRepository.GetAllDepartmentMasters().OrderBy(o => o.DepartmentName).ToList(), "DepartmentId", "DepartmentName", invoice.Disc_Dept_id);
                    ViewBag.DiscountTypeId = new SelectList(_MastersBindRepository.GetDiscountTypeMaster(), "DiscountTypeId", "DiscountType", invoice.DiscountTypeId);
                    ViewBag.InvoiceTypeId = new SelectList(_MastersBindRepository.GetInvoiceTypeMaster().Where(s => s.InvoiceTypeId == invoice.InvoiceTypeId), "InvoiceTypeId", "InvoiceType", invoice.InvoiceTypeId);
                    ViewBag.PaymentTypeId = new SelectList(_MastersBindRepository.GetPaymentTypeMaster().Where(s => s.PaymentTypeId == invoice.PaymentTypeId), "PaymentTypeId", "PaymentType", invoice.PaymentTypeId);
                    ViewBag.StoreId = new SelectList(_CommonRepository.GetStoreList(1), "StoreId", "NickName", invoice.StoreId);
                    ViewBag.VendorId = new SelectList(_MastersBindRepository.GetVendorMaster(invoice.StoreId).Select(s => new { s.VendorId, s.VendorName }).ToList().OrderBy(o => o.VendorName), "VendorId", "VendorName", invoice.VendorId);

                    return View(invoice);
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - Edit - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return View(invoice);
        }
        /// <summary>
        /// This method is the Edit Invoice Attachment Note CR
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="UploadInvoice"></param>
        /// <returns></returns>
        public AttachmentNoteCls EditInvoiceAttachNoteCR(Invoice invoice, HttpPostedFileBase UploadInvoice)
        {
            AttachmentNoteCls attachmentNoteCls = new AttachmentNoteCls();
            try
            {
                #region AttachNote
                var InvoiceNM = "";
                if (UploadInvoice != null)
                {

                    if (UploadInvoice.ContentLength > 0)
                    {

                        var allowedExtensions = new[] { ".pdf" };
                        var extension = Path.GetExtension(UploadInvoice.FileName);
                        var Ext = Convert.ToString(extension).ToLower();
                        if (!allowedExtensions.Contains(Ext))
                        //if (!allowedExtensions.Contains(extension))
                        {
                            attachmentNoteCls.StatusMessage = "InvalidImage";
                        }
                        else
                        {
                            string FileName = invoice.InvoiceDate.ToString("MMddyyyy") + "-" + _MastersBindRepository.GetVendorRemoveSpecialCarecters(invoice.VendorId) + "-" + AdminSiteConfiguration.RemoveSpecialCharacterInvoice(invoice.InvoiceNumber) + "-" + "CR" + "-" + DateTime.Now.Ticks.ToString() + ".pdf";
                            string PathRel = CreateDirectory(invoice.InvoiceDate.ToString("MMddyyyy"), invoice.VendorId, invoice.StoreId);
                            string FullPath = PathRel + "\\" + FileName;
                            UploadInvoice.SaveAs(FullPath);
                            attachmentNoteCls.InvoiceNM = FullPath.Replace(Request.PhysicalApplicationPath + "UserFiles\\Invoices\\", "");
                        }
                    }
                }
                else
                {

                    if (System.IO.File.Exists(Request.PhysicalApplicationPath + "UserFiles\\Invoices" + "\\" + invoice.UploadInvoice))
                    {

                        string FileName = invoice.InvoiceDate.ToString("MMddyyyy") + "-" + _MastersBindRepository.GetVendorRemoveSpecialCarecters(invoice.VendorId) + "-" + AdminSiteConfiguration.RemoveSpecialCharacterInvoice(invoice.InvoiceNumber) + "-" + "CR" + "-" + DateTime.Now.Ticks.ToString() + ".pdf";
                        string PathRel = CreateDirectory(invoice.InvoiceDate.ToString("MMddyyyy"), invoice.VendorId, invoice.StoreId);


                        string ExistingFile = Request.PhysicalApplicationPath + "UserFiles\\Invoices" + "\\" + invoice.UploadInvoice;
                        string NewCopy = PathRel + "\\" + FileName;
                        System.IO.File.Copy(ExistingFile, NewCopy);
                        attachmentNoteCls.InvoiceNM = NewCopy.Replace(Request.PhysicalApplicationPath + "UserFiles\\Invoices\\", "");
                    }
                }
                string scandocument = attachmentNoteCls.InvoiceNM;
                #endregion
            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - EditInvoiceAttachNoteCR - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return attachmentNoteCls;
        }
        /// <summary>
        /// This class is used for Edit Invoice Attach Note
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="UploadInvoice"></param>
        /// <returns></returns>
        public string EditInvoiceAttachNote(Invoice invoice, HttpPostedFileBase UploadInvoice)
        {
            string ReturnFlg = "";
            var Sacn_Title = "";
            try
            {
                if (UploadInvoice != null)
                {
                    if (UploadInvoice.ContentLength > 0)
                    {
                        var allowedExtensions = new[] { ".pdf" };
                        var extension = Path.GetExtension(UploadInvoice.FileName);
                        var Ext = Convert.ToString(extension).ToLower();
                        if (!allowedExtensions.Contains(Ext))
                        {
                            //ViewBag.StatusMessage = "InvalidImage";
                            //return View(invoice);
                            ReturnFlg = "InvalidImage";
                        }
                        else
                        {
                            if (UploadInvoice.ContentLength > 50971520)
                            {
                                //ViewBag.StatusMessage = "InvalidPDFSize";
                                //return View(invoice);
                                ReturnFlg = "InvalidPDFSize";
                            }
                            else
                            {
                                Sacn_Title = AdminSiteConfiguration.GetRandomNo() + Path.GetFileName(UploadInvoice.FileName);
                                Sacn_Title = AdminSiteConfiguration.RemoveSpecialCharacter(Sacn_Title);

                                RemoveDirectory(invoice.UploadInvoice);
                                string InvNo = invoice.InvoiceNumber;
                                if (invoice.InvoiceTypeId == 2)
                                {
                                    var InvSptData = invoice.InvoiceNumber.Split('_');
                                    if (InvSptData.Count() > 0)
                                    {
                                        InvNo = InvSptData[0];
                                    }
                                }

                                string FileName = invoice.InvoiceDate.ToString("MMddyyyy") + "-" + _MastersBindRepository.GetVendorRemoveSpecialCarecters(invoice.VendorId) + "-" + AdminSiteConfiguration.RemoveSpecialCharacterInvoice(InvNo) + "-" + (invoice.InvoiceTypeId == 1 ? "INV" : "CR") + "-" + DateTime.Now.Ticks.ToString() + ".pdf";
                                string PathRel = CreateDirectory(invoice.InvoiceDate.ToString("MMddyyyy"), invoice.VendorId, invoice.StoreId);
                                string FullPath = PathRel + "\\" + FileName;
                                UploadInvoice.SaveAs(FullPath);
                                var InvoiceNM = FullPath.Replace(Request.PhysicalApplicationPath + "UserFiles\\Invoices\\", "");
                                invoice.UploadInvoice = InvoiceNM;
                            }
                        }
                    }

                }
                else
                {
                    invoice.UploadInvoice = CopyDirectory(invoice.InvoiceDate.ToString("MMddyyyy"), invoice.VendorId, invoice.StoreId, invoice.UploadInvoice, invoice.InvoiceTypeId, invoice.InvoiceNumber);
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - EditInvoiceAttachNote - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return ReturnFlg;
        }
        // GET: Invoices/Delete/5
        /// <summary>
        /// This method is delete Invoice data
        /// </summary>
        /// <param name="id"></param>
        /// <param name="From"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,DeleteInvoice")]
        public async Task<ActionResult> Delete(int? id, string From)
        {
            try
            {
                //This class is delete Invoice by Id
                Invoice invoice = _invoiceRepository.deleteInvoice(id.Value, From);
                RemoveDirectory(invoice.UploadInvoice);

                //insert delete log on userlog table
                int DeletedBy = _CommonRepository.getUserId(User.Identity.Name);
                _invoiceRepository.UserlogDeleteInsert(invoice.InvoiceNumber, DeletedBy);

                ActivityLog ActLog = new ActivityLog();
                ActLog.Action = 3;
                //This Db class is used for get user firstname.
                ActLog.Comment = "Invoice " + invoice.InvoiceNumber + " Deleted by " + _CommonRepository.getUserFirstName(User.Identity.Name) + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                //This  class is used for Activity Log Insert
                _ActivityLogRepository.ActivityLogInsert(ActLog);

                StatusMessage = "Delete";
                //DeleteMessage = invoice.InvoiceNumber + " deleted successfully.";
                DeleteMessage = _CommonRepository.GetMessageValue("IN_DEL", "##InvoiceNumber##  deleted successfully.").Replace("##InvoiceNumber##", invoice.InvoiceNumber);
                ViewBag.Delete = DeleteMessage;
            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - Delete - " + DateTime.Now + " - " + ex.Message.ToString());
                DeleteMessage = "Invoice Already Used in Error Log";
                ViewBag.Delete = DeleteMessage;
            }
            return null;

        }

        /// <summary>
        /// This method is get Invoice type
        /// </summary>
        /// <param name="paytype"></param>
        /// <returns></returns>
        public JsonResult getInvoiceType(string paytype)
        {
            //This db class is get Invoice master
            var InvoiceTypeList = _MastersBindRepository.GetInvoiceTypeMaster().Select(s => new { s.InvoiceTypeId, s.InvoiceType }).ToList();
            try
            {
                if (paytype == "1")
                {
                    InvoiceTypeList = InvoiceTypeList.Where(s => s.InvoiceType == "Invoice").ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - getInvoiceType - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return Json(InvoiceTypeList, JsonRequestBehavior.AllowGet);
        }

        //Vendor Dropdown
        /// <summary>
        /// This method is get vendor List by storeId
        /// </summary>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        public JsonResult getVendorList(int StoreId)
        {
            //This db class is get Vendor master
            var VendorList = _MastersBindRepository.GetVendorMaster(StoreId).Select(s => new { s.VendorId, s.VendorName }).ToList();
            return Json(VendorList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Get DepartMent List by StoreId
        /// </summary>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        public JsonResult getDepartmentList(int StoreId)
        {
            //Get Department with store id
            var DepartmentList = _invoiceRepository.getDepartment_WithSP(StoreId);
            return Json(DepartmentList, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// This method is Bind vendor address
        /// </summary>
        /// <param name="vid"></param>
        /// <returns></returns>
        public JsonResult BindVendorAddress(int vid = 0)
        {
            try
            {
                if (vid > 0)
                {
                    QBResponse objResponse = new QBResponse();
                    var dbvendor = _VendorMasterRepository.getVendor(vid);
                    //Get Quick Book vendor Status
                    string QBStatus = _QBRepository.QBGetVendorStatus(dbvendor.ListId, Convert.ToInt32(dbvendor.StoreId), ref objResponse);
                    //This db class is get vendor master
                    var dataval = _VendorMasterRepository.GetVendormaster(vid)
                        .Select(x => new
                        {
                            Address = (x.Address != null ? (x.Address != "" ? x.Address + "," : "") : "") + (x.Address2 != null ? (x.Address2 != "" ? x.Address2 + "," : "") : "") + (x.City != null ? (x.City != "" ? x.City + "," : "") : "") + (x.State != null ? (x.State != "" ? x.State + " " : "") : "") + (x.Country != null ? (x.Country != "" ? x.Country + " " : "") : "") + (x.PostalCode != null ? (x.PostalCode != "" ? x.PostalCode : "") : "")
                            ,
                            PhoneNumber = x.PhoneNumber,
                            Instruction = x.Instruction,
                            SynthesisStatus = (x.IsActive == true ? "Active" : "InActive"),
                            ListID = x.ListId,
                            StoreID = x.StoreId,
                            QBStatus = QBStatus
                        }).FirstOrDefault();
                    return Json(dataval);
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - BindVendorAddress - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return Json(JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// This method is get vendor Tool Tip
        /// </summary>
        /// <param name="vid"></param>
        /// <returns></returns>
        public JsonResult GetVendorToolTip(int vid = 0)
        {
            try
            {
                if (vid > 0)
                {
                    //This db class is get vendor master by vendor id
                    var dataval = _VendorMasterRepository.GetVendormaster(vid).Where(x => x.VendorId == vid).Select(x => new { x.Instruction }).FirstOrDefault();
                    return Json(dataval);
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - GetVendorToolTip - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return Json(JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Get invoice details by Userid and Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Details(int Id = 0)
        {
            //ViewBag.Detailstorenamevalue = storenameval;
            IsArray = true;
            if (Id == 0)
            {
                return RedirectToAction("index", "Invoices");
            }
            Invoice Data = new Invoice();
            //this class is get UserID by username
            int UserId = _CommonRepository.getUserId(User.Identity.Name);
            //Get Invoice Details
            Data = _invoiceRepository.invoiceDetail(Id, UserId);
            Session["storeid"] = Data.StoreId;
            ViewBag.IDOnDetailPage = Id;
            ViewBag.StatusApp = _CommonRepository.GetMessageValue("IN_SA", "Status Approved Successfully.");
            ViewBag.StatusUnApp = _CommonRepository.GetMessageValue("IN_SU", "Status UnApproved Successfully.");
            ViewBag.Reset = _CommonRepository.GetMessageValue("INAYWR", "Are you sure Want to reset?");
            ViewBag.ResetSuccess = _CommonRepository.GetMessageValue("INR", "Reset Successfully.");

            var UserDroplist = _CommonRepository.GetUserList().Where(x => x.IsActive == true).Select(x => new { x.UserId, x.FirstName }).OrderBy(x => x.FirstName).ToList();
            ViewBag.UserDropData = new SelectList(UserDroplist, "UserId", "FirstName");
            var priorityList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Low", Value = "1" },
                new SelectListItem { Text = "Medium", Value = "2" },
                new SelectListItem { Text = "High", Value = "3" }
            };
            ViewBag.PriorityDropData = new SelectList(priorityList, "Value", "Text");

            return View(Data);
            //
        }
        /// <summary>
        /// This method is Bind Payment Type
        /// </summary>
        /// <param name="Store_idval"></param>
        /// <returns></returns>
        public JsonResult BindPaymentType(int Store_idval = 0)
        {
            //Get Payment Type Master
            var LstPamentype = _MastersBindRepository.GetPaymentTypeMaster().Select(a => new ddllist
            {
                Value = a.PaymentTypeId.ToString(),
                Text = a.PaymentType,
            }).ToList();

            if (Store_idval != 11)
            {
                LstPamentype.RemoveAt(0);
            }
            return Json(new SelectList(LstPamentype, "Value", "Text"), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="PostedData"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Details(Invoice PostedData, int ID = 0)
        {
            try
            {
                IsEdit = true;
                Invoice custdata = new Invoice();
                QBResponse objResponse = new QBResponse();
                //this class is get userby
                int userid = _CommonRepository.getUserId(User.Identity.Name);


                if (ID > 0)
                {
                    //This Db class is used for get user firstname.
                    string userFullName = _CommonRepository.getUserFirstName(User.Identity.Name);
                    //This db class is save invoice details
                    _invoiceRepository.InvoiceDetailSave(PostedData, ID, userFullName, userid);


                    if (PostedData.FromInvoicePage == "InvoiceReport")
                    {
                        return RedirectToAction("Index", "InvoiceReport");
                    }
                    else
                    {
                        return RedirectToAction("IndexBeta");
                    }
                }
                else
                {
                    StatusMessage = "Error";
                }
                ViewBag.StatusMessage = StatusMessage;
                return View(PostedData);
            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - Details - " + DateTime.Now + " - " + ex.Message.ToString());
                return RedirectToAction("IndexBeta");
            }
        }

        /// <summary>
        /// This method is reject Invoice
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Administrator,RejectInvoice")]
        public ActionResult InvoiceReject(int Id = 0)
        {
            try
            {
                //this class is get userby
                int UserId = _CommonRepository.getUserId(User.Identity.Name);
                //Db class is Reject Invoice
                _invoiceRepository.InvoiceReject(Id, UserId);
            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - InvoiceReject - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return null;
        }
        /// <summary>
        /// This method is Approve Invoice
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="qbtransfer"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Administrator,ApproveInvoice")]
        public async Task<ActionResult> InvoiceApprove(int Id = 0, int qbtransfer = 1)
        {
            //db class is Apptove Invoice
            _invoiceRepository.InvoiceApprove(Id, qbtransfer);
            return null;
        }
        /// <summary>
        /// This method is Get Invoide on hold by ID
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Administrator,OnHoldInvoice")]
        public ActionResult InvoiceOnHold(int Id = 0)
        {
            //Get Invoice on hold by id
            _invoiceRepository.InvoiceOnHold(Id);

            return null;
        }

        /// <summary>Releases unmanaged resources and optionally releases managed resources.</summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            //This db class is dispose
            _invoiceRepository.Dispose(disposing);
            base.Dispose(disposing);
        }

        /// <summary>
        /// This method is Check Existence Invoice
        /// </summary>
        /// <param name="vendorid"></param>
        /// <param name="invoiceno"></param>
        /// <param name="invoicedate"></param>
        /// <param name="type"></param>
        /// <param name="storeid"></param>
        /// <param name="invoiceid"></param>
        /// <param name="totalamtvalue"></param>
        /// <returns></returns>
        public ActionResult CheckExistence(string vendorid, string invoiceno, string invoicedate, string type, string storeid, string invoiceid, decimal totalamtvalue = 0)
        {
            InvoicesViewModel data = new InvoicesViewModel();
            //this method is check Existence Invoice
            data = _invoiceRepository.CheckExistence(vendorid, invoiceno, invoicedate, type, storeid, invoiceid, totalamtvalue);
            return Json(new { result = data.Message, data = data }, "text/html", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Update Invoice Product Price
        /// </summary>
        /// <param name="invoiceProduct"></param>
        /// <returns></returns>
        public ActionResult UpdateProductprice(InvoiceProduct invoiceProduct)
        {
            try
            {
                //This class is update Product price
                invoiceProduct = _invoiceRepository.UpdateProductprice(invoiceProduct);
                return Json(invoiceProduct, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - UpdateProductprice - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// This method is Insert Invoice Product Price
        /// </summary>
        /// <param name="invoiceProduct"></param>
        /// <returns></returns>
        public ActionResult InsertProductprice(InvoiceProduct invoiceProduct)
        {
            //This class is Insert Product price
            _invoiceRepository.InsertProductprice(invoiceProduct);
            return Json(invoiceProduct, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Delete Invoice Product Price
        /// </summary>
        /// <param name="InvoiceProductId"></param>
        /// <returns></returns>
        public ActionResult DeleteProductprice(string InvoiceProductId)
        {
            //This class is Delete Product price
            _invoiceRepository.DeleteProductprice(InvoiceProductId);
            return Json("sucess", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is get data page scroll
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="currentPageIndex"></param>
        /// <param name="rdcash"></param>
        /// <param name="rdcheck"></param>
        /// <param name="datval"></param>
        /// <param name="datendval"></param>
        /// <param name="deptnm"></param>
        /// <param name="searchdashbord"></param>
        /// <param name="AmtMaximum"></param>
        /// <param name="AmtMinimum"></param>
        /// <param name="orderby"></param>
        /// <param name="IsAsc"></param>
        /// <param name="SearchFlg"></param>
        /// <returns></returns>
        public ActionResult GetDatapagescroll(int PageSize, int currentPageIndex, string rdcash, string rdcheck, string datval, string datendval, int deptnm, string searchdashbord = "", string AmtMaximum = "0", string AmtMinimum = "0", string orderby = "InvoiceId", int IsAsc = 0, string SearchFlg = "")
        {
            string chk = "";
            if (rdcash != "")
            {
                chk = rdcash;
            }

            if (rdcheck != "")
            {
                if (chk != "")
                {
                    chk = "";
                }
                else
                {
                    chk = rdcheck;
                }
            }
            if (AmtMaximum == "" || AmtMaximum == " " || AmtMaximum == null)
            {
                AmtMaximum = "0";
            }
            if (AmtMinimum == "" || AmtMinimum == " " || AmtMinimum == null)
            {
                AmtMinimum = "0";
            }

            DateTime? start_date = null;
            DateTime? end_date = null;
            try
            {
                if (datval != "")
                    start_date = Convert.ToDateTime(datval);
            }
            catch (Exception ex)
            {

                logger.Error("InvoicesController - GetDatapagescroll - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            try
            {
                if (datendval != "")
                    end_date = Convert.ToDateTime(datendval);
            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - GetDatapagescroll - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            int startcount = 0, endcount = 0;
            int startIndex = ((currentPageIndex - 1) * PageSize) + 1;
            int endIndex = startIndex + PageSize - 1;
            IEnumerable RtnData = null;
            #region dashbord grid and search
            try
            {
                string storeid = "";
                int StoreID = 0;
                if (Session["storeid"] == null) { Session["storeid"] = "0"; }
                if ((Convert.ToString(Session["storeid"])) != "0" || Roles.IsUserInRole("Administrator"))
                {
                    storeid = Session["storeid"].ToString();
                    StoreID = Convert.ToInt32(storeid);
                }
                else
                {
                    storeid = Session["storeid"].ToString();
                    StoreID = Convert.ToInt32(storeid);
                }
                string ASC = "";
                if (IsAsc == 1)
                {
                    ASC = "Asc";
                }
                else
                {
                    ASC = "desc";
                }

                var strStore = GetInvoice_StoreList(Convert.ToInt32(storeid));
                //this class is get data page scroll
                RtnData = _invoiceRepository.GetDatapagescroll(PageSize, currentPageIndex, rdcash, rdcheck, datval, datendval, deptnm, searchdashbord, AmtMaximum, AmtMinimum, orderby, IsAsc, SearchFlg, chk, strStore, ASC);


                TotalDataCount = RtnData.OfType<InvoiceSelect>().Count();

                #endregion
                ViewBag.Invoicecount = TotalDataCount;
                if (TotalDataCount == 0)
                {
                    StatusMessage = "NoItem";
                }
                #region productcount
                if (TotalDataCount > PageSize)
                {
                    if (currentPageIndex > 1)
                    {
                        startcount = ((Convert.ToInt32(currentPageIndex - 1) * Convert.ToInt32(PageSize)) + 1);
                        endcount = (Convert.ToInt32(PageSize) * Convert.ToInt32(currentPageIndex));
                        if (Convert.ToInt32(endcount) > TotalDataCount)
                        {
                            endcount = TotalDataCount;
                        }
                    }
                    else
                    {
                        startcount = 1;
                        endcount = (PageSize);
                    }
                }
                else
                {
                    startcount = 1;
                    endcount = TotalDataCount;
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - GetDatapagescroll - " + DateTime.Now + " - " + ex.Message.ToString());
            }


            try
            { }
            catch { TotalDataCount = 0; }


            ViewBag.startcount = startcount;
            ViewBag.endcount = endcount;
            ViewBag.totalcount = TotalDataCount;

            ViewBag.PageSize = PageSize;
            ViewBag.CurrentPageIndex = currentPageIndex;
            ViewBag.LastPageIndex = this.getLastPageIndex(PageSize);
            ViewBag.SearchFlg = SearchFlg;
            #endregion

            return Json(RtnData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Get role for Js approval 
        /// </summary>
        /// <param name="Role"></param>
        /// <param name="StoreId"></param>
        /// <param name="ModuleID"></param>
        /// <returns></returns>
        public JsonResult GetroleForJSApproval(string Role, int? StoreId, int ModuleID)
        {
            try
            {
                if (Roles.IsUserInRole("Administrator"))
                {
                    return Json("True", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //Get Role for Approval
                    var Result = _invoiceRepository.GetroleForApproval(Role, StoreId, ModuleID);
                    if (Result == null)
                    {
                        return Json("False", JsonRequestBehavior.AllowGet);
                    }
                    return Json("True", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                logger.Error("InvoicesController - GetroleForJSApproval - " + DateTime.Now + " - " + ex.Message.ToString());


            }
            return Json("True", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Get role for CR approval 
        /// </summary>
        /// <param name="Role"></param>
        /// <param name="StoreId"></param>
        /// <param name="ModuleID"></param>
        /// <returns></returns>
        public bool GetroleForCRApproval(string Role, int? StoreId, int ModuleID)
        {
            try
            {
                if (Roles.IsUserInRole("Administrator"))
                {
                    return false;
                }
                else
                {
                    //Get Role for Approval
                    var Result = _invoiceRepository.GetroleForApproval(Role, StoreId, ModuleID);
                    if (Result == null)
                    {
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - GetroleForCRApproval - " + DateTime.Now + " - " + ex.Message.ToString());


            }
            return false;
        }

        /// <summary>
        /// This method is check invoice used any where
        /// </summary>
        /// <param name="InvoiceID"></param>
        /// <returns></returns>
        public JsonResult CheckInvoiceUSedAnywhere(int InvoiceID)
        {
            var InvExist = false;
            try
            {
                //this class is Get cash or paid out Invoices
                var CPInvoice = _invoiceRepository.CashPaidoutInvoices(InvoiceID);
                if (CPInvoice != null)
                {
                    //This db class is sales Activity summaries
                    var SalesActivity = _invoiceRepository.SalesActivitySummaries(CPInvoice);
                    if (SalesActivity != null)
                    {
                        //return Json("This Invoice used in CashPaid Out entry With StoreID = " + CPInvoice.StoreMasters.NickName + " , ShiftName = " + SalesActivity.ShiftMasters.ShiftName + ", Terminal = " + SalesActivity.StoreTerminals.TerminalMasters.TerminalName + " <br/> Are you sure want to delete this Invoice? If Yes, then its deleted from CashInvoice also.", JsonRequestBehavior.AllowGet);
                        return Json(_CommonRepository.GetMessageValue("INCID", "This Invoice used in CashPaid Out entry With StoreID = ##NickName##, ShiftName = ##ShiftName##, Terminal = ##TerminalName## Are you sure want to delete this Invoice? If Yes, then its deleted from CashInvoice also.").Replace("##NickName##", CPInvoice.StoreMasters.NickName).Replace("##ShiftName##", SalesActivity.ShiftMasters.ShiftName).Replace("##TerminalName##", SalesActivity.StoreTerminals.TerminalMasters.TerminalName), JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        //return Json("This Invoice used in CashPaid Out entry With StoreID = " + CPInvoice.StoreMasters.NickName, JsonRequestBehavior.AllowGet);
                        return Json(_CommonRepository.GetMessageValue("INDC", "This Invoice used in CashPaid Out entry With StoreID = ##NickName##").Replace("##NickName##", CPInvoice.StoreMasters.NickName), JsonRequestBehavior.AllowGet);
                    }
                }
                //This class is get Invoices by InvoiceID
                var Inv = _invoiceRepository.Invoices(InvoiceID);
                if (Inv != null)
                {
                    if (Inv.StatusValue == InvoiceStatusEnm.Approved && Inv.TXNId != null)
                    {
                        //return Json("This Invoice already approved and synced with QB. <br/> Are you sure want to delete this Invoice? If Yes, you have to manual delete from QB.", JsonRequestBehavior.AllowGet);
                        return Json(_CommonRepository.GetMessageValue("INDQ", "This Invoice already approved and synced with QB. ##br## Are you sure want to delete this Invoice? If Yes, you have to manual delete from QB.").Replace("##br##", "<br/>"), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        //return Json("Are you sure want to delete this Invoice?", JsonRequestBehavior.AllowGet);
                        return Json(_CommonRepository.GetMessageValue("INDN", "Are you sure want to delete this Invoice?"), JsonRequestBehavior.AllowGet);
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - CheckInvoiceUSedAnywhere - " + DateTime.Now + " - " + ex.Message.ToString());


            }
            return Json(InvExist, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is clear session 
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
        /// This method is get invoice store list by store id
        /// </summary>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        private string GetInvoice_StoreList(int StoreId)
        {
            string strList = "";
            try
            {
                if (!Roles.IsUserInRole("Administrator"))
                {
                    if (StoreId == 0)
                    {
                        //Get Invoice Store list
                        var list = _invoiceRepository.GetInvoice_StoreList(StoreId, Roles.IsUserInRole("Administrator"), _CommonRepository.getUserId(User.Identity.Name));
                        if (list != null) { strList = String.Join(",", list); }
                        list = null;
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
                logger.Error("InvoicesController - GetInvoice_StoreList - " + DateTime.Now + " - " + ex.Message.ToString());

            }

            return strList;
        }

        /// <summary>
        /// This method is get vendor department list
        /// </summary>
        /// <param name="VendorId"></param>
        /// <returns></returns>
        public ActionResult getVendorDepartmentList(int VendorId)
        {
            List<int> lst = new List<int>();
            //this class is get Vendor Department List by vendorid
            lst = _invoiceRepository.getVendorDepartmentList(VendorId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Url data source by Invoice Id
        /// </summary>
        /// <param name="dm"></param>
        /// <param name="InvoiceID"></param>
        /// <returns></returns>
        public ActionResult UrlDatasource(DataManagerRequest dm, int InvoiceID)
        {
            List<InvoiceProductSelect> productSelects = new List<InvoiceProductSelect>();
            //Get Item List by InvoiceID
            productSelects = _invoiceRepository.GetItemList(InvoiceID);
            IEnumerable DataSource = productSelects;
            DataOperations operation = new DataOperations();
            int count = 0;
            try
            {
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
                count = DataSource.Cast<InvoiceProductSelect>().Count();
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
                logger.Error("InvoicesController - UrlDatasource - " + DateTime.Now + " - " + ex.Message.ToString());

            }

            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }, JsonRequestBehavior.AllowGet) : Json(DataSource, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Add Invoice Product
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Insert(CRUDModel<InvoiceProduct> model)
        {
            InvoiceProduct MD = model.Value;
            //This db class is Insert Product Price
            _invoiceRepository.InsertProductprice(MD);
            return Json(model.Value);
        }

        /// <summary>
        /// This method is Update Invoice Product
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Update(CRUDModel<InvoiceProduct> model)
        {
            InvoiceProduct MD = model.Value;
            //This db class is update Product Price
            _invoiceRepository.UpdateProductprice(MD);
            return Json(model.Value);
        }

        /// <summary>
        /// This method is Update Invoice Status 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateStatus(string id, string value)
        {
            string message = "";
            string approvermsg = "";
            try
            {
                //This db class is update status
                _invoiceRepository.UpdateStatus(id, value);
                message = "success";
                if (value == "True")
                {
                    approvermsg = _CommonRepository.GetMessageValue("IN_SA", "Status Approved Successfully.");
                }
                else
                {
                    approvermsg = _CommonRepository.GetMessageValue("IN_SU", "Status UnApproved Successfully.");
                }

            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - UpdateStatus - " + DateTime.Now + " - " + ex.Message.ToString());

            }
            return Json(new { message = message, approvermsg = approvermsg });

        }

        /// <summary>
        /// This method is Approve All status by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ApproveAllStatus(string id)
        {
            string message = "";
            try
            {
                //This db class is Approve All Status
                _invoiceRepository.ApproveAllStatus(id);
                message = "success";
            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - ApproveAllStatus - " + DateTime.Now + " - " + ex.Message.ToString());

            }
            return Json(message);
        }

        /// <summary>
        /// This method is Reject status by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ResetStatus(string id)
        {
            string message = "";
            try
            {
                //This db class is Reset All Status
                _invoiceRepository.ResetStatus(id);
                message = "success";
            }
            catch (Exception ex)
            {

                logger.Error("InvoicesController - ResetStatus - " + DateTime.Now + " - " + ex.Message.ToString());

            }
            return Json(message);
        }

        /// <summary>
        /// This method is remove Invoice Product
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Remove(CRUDModel<InvoiceProduct> model)
        {
            InvoiceProduct invoiceProduct = new InvoiceProduct();
            try
            {
                if (model.Deleted == null)
                {
                    //This class is delete Product price
                    _invoiceRepository.DeleteProductprice(model.Key.ToString());
                }
                else
                {
                    foreach (var item in model.Deleted)
                    {
                        //This class is Delete Product price
                        _invoiceRepository.DeleteProductprice(model.Key.ToString());
                    }
                }
            }
            catch (Exception ex)
            {

                logger.Error("InvoicesController - Remove - " + DateTime.Now + " - " + ex.Message.ToString());

            }

            return Json(model.Deleted);
        }

        /// <summary>
        /// This method is View Invoice
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> ViewInvoice()
        {
            //Get All View Invoice
            _CommonRepository.LogEntries();     //Harsh's code
            ViewBag.VendorList = _invoiceRepository.ViewInvoice().Select(s => new { VendorName = s.VendorName }).Distinct().ToList();
            ViewBag.VendorList.Insert(0, new { VendorName = "Select" });
            ViewBag.FileRead = _CommonRepository.GetMessageValue("INFR", "File Read Successfully.");
            return View();
        }

        /// <summary>
        /// This method is return Invoice view
        /// </summary>
        /// <param name="dm"></param>
        /// <param name="vendor"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public ActionResult ViewInv(DataManagerRequest dm, string vendor, string startdate, string enddate)
        {
            List<ViewInvoice> BindData = new List<ViewInvoice>();

            DBContext db1 = new DBContext();
            IEnumerable DataSource = BindData;
            DataOperations operation = new DataOperations();
            int count = 0;
            try
            {
                if (vendor != "" && startdate != "" && enddate != "" && vendor != null && startdate != null && enddate != null)
                {
                    BindData = db1.Database.SqlQuery<ViewInvoice>("SP_UploadPDFSpecial @Spara={0},@VendorName={1},@StartDate={2},@EndDate={3}", "Special_View", vendor.ToString().Replace("^", "&"), startdate, enddate).ToList();
                }
                DataSource = BindData;
                count = DataSource.Cast<ViewInvoice>().Count();
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
                logger.Error("InvoicesController - ViewInv - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }

        /// <summary>
        /// This methos is read files by vendor,stardate and enddate
        /// </summary>
        /// <param name="vendor"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ReadFiles(string vendor, string startdate, string enddate)
        {
            string message = "";
            try
            {
                if (vendor != "" && startdate != "" && enddate != "" && vendor != null && startdate != null && enddate != null)
                {
                    //This class is Read Files 
                    _invoiceRepository.ReadFiles(vendor, startdate, enddate);
                    message = "success";
                }
            }
            catch (Exception ex)
            {

                logger.Error("InvoicesController - ReadFiles - " + DateTime.Now + " - " + ex.Message.ToString());

            }
            return Json(message);
        }

        /// <summary>
        /// This method is Create Split Invoice 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,CreateSplitInvoice")]
        // GET: Invoices/CreateSplitInvoice
        public ActionResult CreateSplitInvoice(string val)
        {
            ViewBag.Title = "Add Split Invoice - Synthesis";
            _CommonRepository.LogEntries();     //Harsh's code
            ViewBag.CreateIn = _CommonRepository.GetMessageValue("INICSS", "Invoice Created Successfully.");
            ViewBag.CreateInApprove = _CommonRepository.GetMessageValue("INSA", "Invoice Created Successfully and Sent for Approval.");
            ViewBag.SentApprove = _CommonRepository.GetMessageValue("INSS", "Invoice Created & Approved Successfully ");
            ViewBag.LeastOnedep = _CommonRepository.GetMessageValue("INLODA", "Enter at least In One Department Amount.");
            SplitInvoice obj = new SplitInvoice();

            ViewBag.CurrentDate = DateTime.Today.Date;
            //This class is get all  department masters
            obj.DepartmentMasters = _MastersBindRepository.GetAllDepartmentMasters().Where(s => s.DepartmentId == 0).ToList();

            UploadPDFAutomation UploadFile = new UploadPDFAutomation();
            int? store = null;
            try
            {
                if (!String.IsNullOrEmpty(val))
                {
                    //This db class is create Split Invoice 
                    UploadFile = _invoiceRepository.GetUploadAutomation(Convert.ToInt32(val));
                    obj.UploadInvoice = UploadFile.FileName;
                    store = UploadFile.StoreId;
                    obj.UploadPdfAutomationId = UploadFile.UploadPdfAutomationId;
                }
                if (store == null)
                {
                    //obj.InvoiceDate = DateTime.Today;
                    if (Convert.ToString(Session["storeid"]) != "0")
                    {
                        //This db class is Get store list using role wise
                        var a = _CommonRepository.GetStoreList_RoleWise(1, "CreateInvoice", User.Identity.Name);
                        List<Store> lststore = new List<Store>();
                        foreach (var b in a)
                        {
                            lststore.Add(new Store { StoreId = b.StoreId, StoreName = b.NickName });
                        }
                        obj.StoreMaster = lststore;
                    }
                    else
                    {
                        //This db class is Get store list using role wise
                        var a = _CommonRepository.GetStoreList_RoleWise(1, "CreateInvoice", User.Identity.Name);
                        List<Store> lststore = new List<Store>();
                        foreach (var b in a)
                        {
                            lststore.Add(new Store { StoreId = b.StoreId, StoreName = b.NickName });
                        }
                        obj.StoreMaster = lststore;

                        ViewBag.VendorId = new SelectList("");
                        ViewBag.Disc_Dept_id = new SelectList("");
                    }
                    //Get Discount Type master
                    ViewBag.DiscountTypeId = new SelectList(_MastersBindRepository.GetDiscountTypeMaster(), "DiscountTypeId", "DiscountType", 1);
                    //This db class is get Invoice master
                    var InvtypeList = _MastersBindRepository.GetInvoiceTypeMaster().Where(a => a.InvoiceType == "Invoice").ToList();
                    ViewBag.InvoiceTypeId = new SelectList(InvtypeList, "InvoiceTypeId", "InvoiceType");
                    //Get Payment Type Master
                    ViewBag.PaymentTypeId = (Convert.ToString(Session["storeid"]) != "0" ? new SelectList(_MastersBindRepository.GetPaymentTypeMaster(), "PaymentTypeId", "PaymentType").Skip(0) : new SelectList(_MastersBindRepository.GetPaymentTypeMaster(), "PaymentTypeId", "PaymentType"));
                }
                else
                {
                    if (store != null)
                    {
                        //This class is get store list using role wise
                        var a = _CommonRepository.GetStoreList_RoleWise(1, "CreateInvoice", User.Identity.Name);
                        List<Store> lststore = new List<Store>();
                        foreach (var b in a)
                        {
                            lststore.Add(new Store { StoreId = b.StoreId, StoreName = b.NickName });
                        }
                        obj.StoreMaster = lststore;
                        //This db class is get Invoice master
                        ViewBag.InvoiceTypeId = new SelectList(_MastersBindRepository.GetInvoiceTypeMaster(), "InvoiceTypeId", "InvoiceType");
                        //Get Payment Type Master
                        ViewBag.PaymentTypeId = (store.ToString() != "0" ? new SelectList(_MastersBindRepository.GetPaymentTypeMaster(), "PaymentTypeId", "PaymentType").Skip(0) : new SelectList(_MastersBindRepository.GetPaymentTypeMaster(), "PaymentTypeId", "PaymentType"));
                    }
                    else
                    {
                        //This class is get store list using role wise
                        var a = _CommonRepository.GetStoreList_RoleWise(1, "CreateInvoice", User.Identity.Name);
                        List<Store> lststore = new List<Store>();
                        foreach (var b in a)
                        {
                            lststore.Add(new Store { StoreId = b.StoreId, StoreName = b.NickName });
                        }
                        obj.StoreMaster = lststore;

                        ViewBag.VendorId = new SelectList("");
                        ViewBag.Disc_Dept_id = new SelectList("");
                        //Get Discount Type master
                        ViewBag.DiscountTypeId = new SelectList(_MastersBindRepository.GetDiscountTypeMaster(), "DiscountTypeId", "DiscountType", 1);
                        //Get Invoice type master
                        ViewBag.InvoiceTypeId = new SelectList(_MastersBindRepository.GetInvoiceTypeMaster(), "InvoiceTypeId", "InvoiceType");
                        //Get Payment Type Master
                        ViewBag.PaymentTypeId = (store != null ? new SelectList(_MastersBindRepository.GetPaymentTypeMaster(), "PaymentTypeId", "PaymentType").Skip(0) : new SelectList(_MastersBindRepository.GetPaymentTypeMaster(), "PaymentTypeId", "PaymentType"));
                    }
                }
                if (StatusMessage == "Success1" || StatusMessage == "Success" || StatusMessage == "Exists" || StatusMessage == "Existence")
                {
                    ViewBag.StatusMessage = StatusMessage;
                    StatusMessage = "";
                }
                else
                {
                    ViewBag.StatusMessage = "";
                }

                ViewBag.closingyear = Convert.ToInt32(ConfigurationManager.AppSettings["ClosingYear"].ToString()) + 1;
            }
            catch (Exception ex)
            {

                logger.Error("InvoicesController - CreateSplitInvoice - " + DateTime.Now + " - " + ex.Message.ToString());
            }


            return View(obj);
        }

        /// <summary>
        /// This method is insert Invoice details
        /// </summary>
        /// <param name="splitinvoice"></param>
        /// <param name="UploadInvoice"></param>
        /// <param name="btnsubmit"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator,CreateSplitInvoice")]
        public async Task<ActionResult> CreateSplitInvoice(SplitInvoice splitinvoice, HttpPostedFileBase UploadInvoice, string btnsubmit = "")
        {
            HttpFileCollectionBase files = Request.Files;
            HttpPostedFileBase file = files[0];
            UploadInvoice = file;
            try
            {
                if (splitinvoice.CheckDup == 0 || splitinvoice.CheckDup == 1)
                {
                    if (UploadInvoice != null)
                    {
                        var bulk = 0;

                        if (ModelState.IsValid)
                        {
                            var invdetaildst = splitinvoice.InvoiceDepartmentDetails.GroupBy(note => note.StoreId).Select(g => new { StoreId = g.Key, Amount = g.Sum(e => e.Amount) }).ToList();

                            foreach (var invdetails in invdetaildst)
                            {
                                //Storewise set vendor and storeid...
                                Invoice invoice = new Invoice();
                                invoice.InvoiceTypeId = splitinvoice.InvoiceTypeId;
                                invoice.PaymentTypeId = splitinvoice.PaymentTypeId;
                                invoice.strInvoiceDate = splitinvoice.strInvoiceDate;
                                invoice.InvoiceDate = splitinvoice.InvoiceDate;
                                invoice.InvoiceNumber = splitinvoice.InvoiceNumber;
                                invoice.Note = splitinvoice.Note;
                                if (splitinvoice.QuickInvoice.ToString().ToLower() == "true")
                                    invoice.QuickInvoice = "1";
                                else
                                    invoice.QuickInvoice = "0";
                                invoice.QBtransferss = (splitinvoice.QBtransferss.ToString().ToLower() == "true" ? "true" : "false");
                                invoice.QBTransfer = (splitinvoice.QBtransferss.ToString().ToLower() == "true" ? true : false);
                                invoice.SplitInvoiceDepartmentDetails = splitinvoice.InvoiceDepartmentDetails;


                                if (invoice.strInvoiceDate == null || invoice.strInvoiceDate == "")
                                {
                                    ModelState.AddModelError("strInvoiceDate", "Required");
                                }
                                int Storeidval = 0;

                                invoice.StoreId = Convert.ToInt32(invdetails.StoreId);
                                invoice.TotalAmount = invdetails.Amount;

                                //get invoice details by storeid...
                                var invdetaillist = invoice.SplitInvoiceDepartmentDetails.Where(a => a.StoreId == invdetails.StoreId).ToList();
                                if (invdetaillist != null)
                                {
                                    invoice.VendorId = Convert.ToInt32(invdetaillist[0].VendorId);

                                    if (UploadInvoice == null)
                                    {
                                        string filePath = invoice.UploadInvoice;
                                        if (filePath.Contains(@"\") || filePath.Contains("/"))
                                        {
                                            byte[] bytes = System.IO.File.ReadAllBytes(HttpContext.Server.MapPath("~/UserFiles/Automatic-Invoice-File/" + invoice.UploadInvoice));
                                            var contentTypeFile = "application/pdf";
                                            var fileName = invoice.UploadInvoice;
                                            UploadInvoice = (HttpPostedFileBase)new HttpPostedFileBaseCustom(new MemoryStream(bytes), contentTypeFile, fileName);
                                            bulk = 1;
                                        }
                                        else
                                        {
                                            byte[] bytes = System.IO.File.ReadAllBytes(HttpContext.Server.MapPath("~/UserFiles/BulkUploadFile/" + invoice.UploadInvoice));
                                            var contentTypeFile = "application/pdf";
                                            var fileName = invoice.UploadInvoice;
                                            UploadInvoice = (HttpPostedFileBase)new HttpPostedFileBaseCustom(new MemoryStream(bytes), contentTypeFile, fileName);
                                            bulk = 1;
                                        }
                                    }
                                    //Get Store online desktop by storeid
                                    string Store = _QBRepository.GetStoreOnlineDesktop(Convert.ToInt32(invdetails.StoreId));
                                    //Get Store online desktop flag by storeid
                                    int StoreFlag = _QBRepository.GetStoreOnlineDesktopFlag(Convert.ToInt32(invdetails.StoreId));

                                    char[] MyChar = { '0' };
                                    string Inoicno = invoice.InvoiceNumber.TrimStart(MyChar);
                                    Console.WriteLine(Inoicno);
                                    invoice.InvoiceNumber = Inoicno;
                                    if (invoice.strInvoiceDate != null)
                                    {
                                        invoice.InvoiceDate = Convert.ToDateTime(invoice.strInvoiceDate);
                                    }
                                    if (Roles.IsUserInRole("Administrator"))
                                    {
                                        Storeidval = Convert.ToInt32(invdetails.StoreId);
                                    }
                                    else
                                    {
                                        if (Convert.ToInt32(invdetails.StoreId) != 0)
                                        {
                                            Storeidval = Convert.ToInt32(invdetails.StoreId);
                                        }
                                        else
                                        {
                                            if (Session["storeid"] == null)
                                            {
                                                Session["storeid"] = "0";
                                            }
                                            if (Convert.ToString(Session["storeid"]) != "0")
                                            {
                                                Storeidval = Convert.ToInt32(Session["storeid"].ToString());
                                            }
                                            else
                                            {
                                                RedirectToAction("Index", "Login", new { area = "" });
                                            }
                                        }
                                    }

                                    #region AttachNote
                                    var Sacn_Title = "";
                                    if (UploadInvoice != null)
                                    {
                                        if (UploadInvoice.ContentLength > 0)
                                        {
                                            var allowedExtensions = new[] { ".pdf" };
                                            var extension = Path.GetExtension(UploadInvoice.FileName);
                                            var Ext = Convert.ToString(extension).ToLower();
                                            if (!allowedExtensions.Contains(Ext))
                                            {
                                                ViewBag.StatusMessage = "InvalidImage";
                                                //Get department using storeid
                                                invoice.DepartmentMasters = _invoiceRepository.getDepartment_WithSP(Convert.ToInt32(invdetails.StoreId));
                                                //This db class is get Invoice master
                                                ViewBag.InvoiceTypeId = new SelectList(_MastersBindRepository.GetInvoiceTypeMaster(), "InvoiceTypeId", "InvoiceType", invoice.InvoiceTypeId);
                                                //Get Payment Type Master
                                                ViewBag.PaymentTypeId = (Convert.ToString(Session["storeid"]) != "0" ? new SelectList(_MastersBindRepository.GetPaymentTypeMaster(), "PaymentTypeId", "PaymentType", invoice.PaymentTypeId).Skip(0) : new SelectList(_MastersBindRepository.GetPaymentTypeMaster(), "PaymentTypeId", "PaymentType", invoice.PaymentTypeId));

                                                return View(invoice);
                                            }
                                            else
                                            {
                                                if (UploadInvoice.ContentLength > 50971520)
                                                {
                                                    ViewBag.StatusMessage = "InvalidPDFSize";
                                                    return View(invoice);
                                                }
                                                else
                                                {
                                                    try
                                                    {
                                                        //Get Vendor Name
                                                        string FileName = invoice.InvoiceDate.ToString("MMddyyyy") + "-" + _MastersBindRepository.GetVendorName(Convert.ToInt32(invoice.VendorId)) + "-" + AdminSiteConfiguration.RemoveSpecialCharacterInvoice(Inoicno) + "-" + (invoice.InvoiceTypeId == 1 ? "INV" : "CR") + "-" + DateTime.Now.Ticks.ToString() + ".pdf";
                                                        //(invoice.InvoiceTypeId == 1 ? "INV" : "CR") + "-" + AdminSiteConfiguration.RemoveSpecialCharacterInvoice(Inoicno) + "-" + invoice.InvoiceDate.ToString("MMddyyyy") +"-"+ GetVendorName(invoice.VendorId) +".pdf";
                                                        string PathRel = CreateDirectory(invoice.InvoiceDate.ToString("MMddyyyy"), Convert.ToInt32(invoice.VendorId), Convert.ToInt32(invdetails.StoreId));
                                                        string FullPath = PathRel + "\\" + FileName;
                                                        UploadInvoice.SaveAs(FullPath);
                                                        var InvoiceNM = FullPath.Replace(Request.PhysicalApplicationPath + "UserFiles\\Invoices\\", "");
                                                        invoice.UploadInvoice = InvoiceNM;

                                                        if (!String.IsNullOrEmpty(Convert.ToString(invoice.UploadPdfAutomationId)))
                                                        {
                                                            //This db class is Upload PDf
                                                            _invoiceRepository.UploadAutomationPdfId(invoice.UploadPdfAutomationId);
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        logger.Error("InvoicesController - CreateSplitInvoice - " + DateTime.Now + " - " + ex.Message.ToString());
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                    int iInvoiceId = 0; string iInvoiceStatus = "";
                                    //---------- User Aprrove Rights Code Check----------/// 
                                    //This class is Get Module masters id
                                    var ModuleId = _invoiceRepository.GetModuleMastersId();
                                    bool roleFlg = false;
                                    if (!Roles.IsUserInRole("Administrator"))
                                    {
                                        var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                                        //Get usertype by username
                                        int UserTypeId = _CommonRepository.getUserTypeId(UserName);
                                        if (UserTypeId > 0)
                                        {
                                            //Check user type moduke approvers
                                            if (_invoiceRepository.CheckUserTypeModuleApprovers(UserTypeId, ModuleId))
                                            {
                                                roleFlg = true;
                                            }
                                        }
                                    }

                                    bool FLG = GetroleForCRApproval("ApproveInvoice", Convert.ToInt32(invdetails.StoreId), 1);
                                    //------------------------End-------------------------------//
                                    if ((invoice.QuickInvoice == "1" && ((Roles.IsUserInRole("Administrator")) || Roles.IsUserInRole("nnfapprovalInvoice"))) || (FLG == true && roleFlg == false))
                                    {
                                        if (Store != "")
                                        {
                                            if (Store == "Online" && StoreFlag == 1 && invoice.PaymentTypeId == 2) //Check/ACH
                                            {
                                                if (invoice.QBTransfer == false)
                                                {
                                                    try
                                                    {
                                                        if (invoice.strInvoiceDate != null)
                                                        {
                                                            invoice.InvoiceDate = Convert.ToDateTime(invoice.strInvoiceDate);
                                                        }
                                                        invoice.StatusValue = InvoiceStatusEnm.Approved;
                                                        //this class is get UserID by username
                                                        invoice.CreatedBy = _CommonRepository.getUserId(User.Identity.Name);
                                                        invoice.CreatedOn = DateTime.Now;
                                                        invoice.LastModifiedOn = DateTime.Now;
                                                        //this class is get UserID by username
                                                        invoice.ApproveRejectBy = _CommonRepository.getUserId(User.Identity.Name);
                                                        invoice.ApproveRejectDate = DateTime.Now;
                                                        invoice.IsActive = true;
                                                        //This db class is Insert Invoice
                                                        _invoiceRepository.InsertInvoices(invoice);

                                                        iInvoiceId = invoice.InvoiceId;
                                                        iInvoiceStatus = invoice.StatusValue.ToString();
                                                        if (iInvoiceId > 0)
                                                        {
                                                            int j = 0;

                                                            List<BillDetail> objList = new List<BillDetail>();

                                                            foreach (var val_id in invdetaillist)
                                                            {
                                                                int iID = Convert.ToInt32(val_id.DepartmentId);
                                                                BillDetail objDetail = new BillDetail();
                                                                objDetail.Description = "";
                                                                objDetail.Amount = Convert.ToDecimal(val_id.Amount);
                                                                //This class is Get department masters list
                                                                objDetail.DepartmentID = _invoiceRepository.GetDepartmentMastersList(Storeidval, iID).Count() > 0 ? _invoiceRepository.GetDepartmentMastersList(Storeidval, iID).FirstOrDefault().ListId : "";
                                                                objList.Add(objDetail);
                                                                objDetail = null;
                                                                j++;
                                                            }
                                                            //This class is create invoice bill 
                                                            _QBRepository.CreateBill(Convert.ToInt32(iInvoiceId), objList);
                                                            objList = null;
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    { logger.Error("InvoicesController - CreateSplitInvoice - " + DateTime.Now + " - " + ex.Message.ToString()); }
                                                }
                                                else
                                                {
                                                    //this class is get UserID by username
                                                    invoice.CreatedBy = _CommonRepository.getUserId(User.Identity.Name);
                                                    invoice.CreatedOn = DateTime.Now;
                                                    invoice.LastModifiedOn = DateTime.Now;
                                                    invoice.Source = "WEB";
                                                    invoice.StatusValue = InvoiceStatusEnm.Approved;
                                                    //this class is get userby
                                                    invoice.ApproveRejectBy = _CommonRepository.getUserId(User.Identity.Name);
                                                    invoice.ApproveRejectDate = DateTime.Now;
                                                    invoice.IsActive = true;
                                                    //This class is insert invoices
                                                    _invoiceRepository.InsertInvoices(invoice);
                                                    iInvoiceId = invoice.InvoiceId;
                                                    iInvoiceStatus = invoice.StatusValue.ToString();
                                                }
                                            }
                                            else
                                            {
                                                //this class is get userby
                                                invoice.CreatedBy = _CommonRepository.getUserId(User.Identity.Name);
                                                invoice.CreatedOn = DateTime.Now;
                                                invoice.LastModifiedOn = DateTime.Now;
                                                invoice.Source = "WEB";
                                                invoice.StatusValue = InvoiceStatusEnm.Approved;
                                                invoice.ApproveRejectBy = _CommonRepository.getUserId(User.Identity.Name);
                                                invoice.ApproveRejectDate = DateTime.Now;
                                                invoice.IsActive = true;
                                                //This class is insert invoices
                                                _invoiceRepository.InsertInvoices(invoice);
                                                iInvoiceId = invoice.InvoiceId;
                                                iInvoiceStatus = invoice.StatusValue.ToString();
                                            }
                                        }
                                        else
                                        {
                                            //this class is get UserID by username
                                            invoice.CreatedBy = _CommonRepository.getUserId(User.Identity.Name);
                                            invoice.CreatedOn = DateTime.Now;
                                            invoice.LastModifiedOn = DateTime.Now;
                                            invoice.Source = "WEB";
                                            invoice.StatusValue = InvoiceStatusEnm.Approved;
                                            invoice.ApproveRejectBy = _CommonRepository.getUserId(User.Identity.Name);
                                            invoice.ApproveRejectDate = DateTime.Now;
                                            invoice.IsActive = true;
                                            //This class is insert invoices
                                            _invoiceRepository.InsertInvoices(invoice);
                                            iInvoiceId = invoice.InvoiceId;
                                            iInvoiceStatus = invoice.StatusValue.ToString();
                                        }
                                    }
                                    else
                                    {
                                        //this class is get UserID by username
                                        invoice.CreatedBy = _CommonRepository.getUserId(User.Identity.Name);
                                        invoice.CreatedOn = DateTime.Now;
                                        invoice.LastModifiedOn = DateTime.Now;
                                        invoice.Source = "WEB";
                                        invoice.StatusValue = InvoiceStatusEnm.Pending;
                                        invoice.IsActive = true;
                                        //This class is insert invoices
                                        _invoiceRepository.InsertInvoices(invoice);
                                        iInvoiceId = invoice.InvoiceId;
                                        iInvoiceStatus = invoice.StatusValue.ToString();
                                    }


                                    try
                                    {
                                        if (Convert.ToInt32(iInvoiceId) > 0)
                                        {

                                            int j = 0;
                                            foreach (var val_id in invdetaillist)
                                            {
                                                if (!String.IsNullOrEmpty(val_id.Amount.ToString()))
                                                {
                                                    if (Convert.ToDecimal(val_id.Amount) > 0) // Remove "=" To Stop 0 Amount Deparment
                                                    {
                                                        InvoiceDepartmentDetail deptDetail = new InvoiceDepartmentDetail();
                                                        deptDetail.InvoiceId = iInvoiceId;
                                                        deptDetail.DepartmentId = Convert.ToInt32(val_id.DepartmentId);
                                                        deptDetail.Amount = Convert.ToDecimal(val_id.Amount);
                                                        //This class is insert invoices department details
                                                        _invoiceRepository.InsertInvoiceDepartmentDetails(deptDetail);
                                                    }
                                                }
                                                j++;
                                            }


                                            if (invoice.QuickInvoice == "1")
                                            {
                                                //ActivityLogMessage = "Quick Invoice Number " + "<a href='/Invoices/Details/" + invoice.InvoiceId + "'>" + invoice.InvoiceNumber + "</a> Created by " + UserModule.getUserFirstName() + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                                                ActivityLogMessage = _CommonRepository.GetMessageValue("INC", "Quick Invoice Number ##a href='/Invoices/Details/## ##invoice.InvoiceId'## ##invoice.InvoiceNumber## ##/a## Created by ##getUserFirstName## on ##DateTime.Now##").Replace("##a href='/Invoices/Details/##", "<a href='/Invoices/Details/").Replace("invoice.InvoiceId'", invoice.InvoiceId + "'>").Replace("##invoice.InvoiceNumber##", invoice.InvoiceNumber).Replace("/a", "</a>").Replace("##getUserFirstName##", UserModule.getUserFirstName()).Replace("##DateTime.Now##", AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString()));
                                            }
                                            else
                                            {
                                                //ActivityLogMessage = "Invoice Number " + "<a href='/Invoices/Details/" + invoice.InvoiceId + "'>" + invoice.InvoiceNumber + "</a> Created by " + UserModule.getUserFirstName() + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                                                ActivityLogMessage = _CommonRepository.GetMessageValue("INCI", "Invoice Number ##a href='/Invoices/Details/## ##invoice.InvoiceId'## ##invoice.InvoiceNumber## ##/a## Created by ##getUserFirstName## on ##DateTime.Now##").Replace("##a href='/Invoices/Details/##", "<a href='/Invoices/Details/").Replace("invoice.InvoiceId'", invoice.InvoiceId + "'>").Replace("##invoice.InvoiceNumber##", invoice.InvoiceNumber).Replace("/a", "</a>").Replace("##getUserFirstName##", UserModule.getUserFirstName()).Replace("##DateTime.Now##", AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString()));
                                            }

                                            ActivityLog ActLog1 = new ActivityLog();
                                            ActLog1.UserId = _CommonRepository.getUserId(User.Identity.Name);
                                            ActLog1.Action = 1;
                                            ActLog1.Comment = ActivityLogMessage;
                                            //This  class is used for Activity Log Insert
                                            _ActivityLogRepository.ActivityLogInsert(ActLog1);

                                            IsArray = false;
                                            if (invoice.QuickInvoice == "1")
                                            {
                                                StatusMessage = "Success1";
                                                ViewBag.StatusMessage = StatusMessage;
                                            }
                                            else
                                            {
                                                StatusMessage = "Success";
                                                ViewBag.StatusMessage = StatusMessage;
                                            }

                                        }
                                        else
                                        {
                                            return RedirectToAction("Index", "Login", new { area = "" });
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.Error("InvoicesController - CreateSplitInvoice - " + DateTime.Now + " - " + ex.Message.ToString());
                                    }

                                }
                            }

                            StatusMessage = "Success";

                            string message = StatusMessage;
                            if ((splitinvoice.QuickInvoice == "1" && ((Roles.IsUserInRole("Administrator")) || Roles.IsUserInRole("nnfapprovalInvoice"))))
                            {
                                message = "Success";
                            }

                            return Json(new { success = StatusMessage });

                        }
                        else
                        {
                            StatusMessage = "Please fill all required fields.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                logger.Error("InvoicesController - CreateSplitInvoice - " + DateTime.Now + " - " + ex.Message.ToString());
            }


            return Json(new { success = StatusMessage });
        }

        /// <summary>
        /// This method is Force invoice sysnc
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> ForceInvoiceSync(int id)
        {
            string message = "";
            try
            {
                if (id != 0)
                {
                    //Get Invoice details by ID
                    Invoice invoice = _invoiceRepository.GetInvoiceDetail(id);
                    if (invoice.InvoiceTypeId == 2) //Invoice = 1, CreditMemo =2
                    {

                        List<VendorCredirDetail> objList1 = new List<VendorCredirDetail>();

                        //Get Invoice department details by Invoice Id
                        var invDept = _invoiceRepository.GetInvoiceDepartmentDetailsbyInvoiceID(id);
                        foreach (var item in invDept)
                        {
                            VendorCredirDetail objDetail = new VendorCredirDetail();
                            objDetail.Description = "";
                            objDetail.Amount = Convert.ToDecimal(item.Amount);
                            //GEt department Master list
                            objDetail.DepartmentID = _invoiceRepository.GetDepartmentMastersList(invoice.StoreId, item.DepartmentId).Count() > 0 ? _invoiceRepository.GetDepartmentMastersList(invoice.StoreId, item.DepartmentId).FirstOrDefault().ListId : "";
                            objList1.Add(objDetail);
                            objDetail = null;
                        }

                        if (invoice.TXNId == "" || invoice.TXNId == null)
                        {
                            //this class is create vendorcredit in Quick books
                            _QBRepository.CreateVendorCredit(Convert.ToInt32(id), objList1, 0);
                            StatusMessage = "Success1";
                            //ViewBag.Delete = "VendorCredit Create Successfully in QuickBooks.";
                            ViewBag.Delete = _CommonRepository.GetMessageValue("INFIS", "VendorCredit Create Successfully in QuickBooks.");
                        }
                        else
                        {
                            QBResponse objResponse = new QBResponse();
                            //Get Configuration Details
                            QBOnlineconfiguration objOnlieDetail = _QBRepository.GetConfigDetail(invoice.StoreId);
                            //Get vendor Sync Token
                            string SyncToken = _QBRepository.GetVendorSyncToken(invoice.TXNId, objOnlieDetail, ref objResponse);
                            if (SyncToken == "")
                            {
                                //Bill not found in QB, Now try to create...
                                //this class is create vendorcredit in Quick books
                                _QBRepository.CreateVendorCredit(Convert.ToInt32(id), objList1, 0);
                                StatusMessage = "Success1";
                                //ViewBag.Delete = "VendorCredit Create Successfully in QuickBooks.";
                                ViewBag.Delete = _CommonRepository.GetMessageValue("INFIS", "VendorCredit Create Successfully in QuickBooks.");
                            }
                            else
                            {
                                //this class is Edit vendorcredit data
                                _QBRepository.QBEditVendorCreditData(invoice.InvoiceId, invoice.TXNId, invoice.InvoiceNumber, Convert.ToDateTime(invoice.InvoiceDate), invoice.Note, Convert.ToInt32(invoice.StoreId), Convert.ToInt32(invoice.VendorId), objList1, ref objResponse);
                                if (objResponse.Status == "Done")
                                {
                                    StatusMessage = "Edit";
                                    //ViewBag.Delete = "VendorCredit Update Successfully in QuickBooks.";
                                    ViewBag.Delete = _CommonRepository.GetMessageValue("INFIE", "VendorCredit Update Successfully in QuickBooks.");
                                }
                            }
                        }
                        objList1 = null;
                    }
                    else
                    {
                        List<BillDetail> objList = new List<BillDetail>();
                        //Get Invoice department details by Invoice Id
                        var invDept = _invoiceRepository.GetInvoiceDepartmentDetailsbyInvoiceID(id).ToList();
                        if (invoice.PaymentTypeId == 1) //cash
                        {
                            //This class is Update ISSync Status
                            _invoiceRepository.UpdateIsSyncStatus_reset(invoice.InvoiceId);
                            if (invoice.TXNId == "" || invoice.TXNId == null)
                            {
                                StatusMessage = "Success1";
                                //ViewBag.Delete = "Cash Entry Create Successfully in QuickBooks.";
                                ViewBag.Delete = _CommonRepository.GetMessageValue("INCC", "Cash Entry Create Successfully in QuickBooks.");
                            }
                            else
                            {
                                StatusMessage = "Edit";
                                //ViewBag.Delete = "Cash Entry Update Successfully in QuickBooks.";
                                ViewBag.Delete = _CommonRepository.GetMessageValue("INCE", "Cash Entry Update Successfully in QuickBooks.");
                            }
                        }
                        else
                        {
                            foreach (var item in invDept)
                            {
                                BillDetail objDetail = new BillDetail();
                                objDetail.Description = "";
                                objDetail.Amount = Convert.ToDecimal(item.Amount);
                                objDetail.DepartmentID = _invoiceRepository.GetDepartmentMastersList(invoice.StoreId, item.DepartmentId).Count() > 0 ? _invoiceRepository.GetDepartmentMastersList(invoice.StoreId, item.DepartmentId).FirstOrDefault().ListId : "";
                                objList.Add(objDetail);
                                objDetail = null;
                            }

                            if (invoice.TXNId == "" || invoice.TXNId == null)
                            {
                                _QBRepository.CreateBill(Convert.ToInt32(id), objList);
                                StatusMessage = "Success1";
                                //ViewBag.Delete = "Bill Create Successfully in QuickBooks.";
                                ViewBag.Delete = _CommonRepository.GetMessageValue("INBC", "Bill Create Successfully in QuickBooks.");
                            }
                            else
                            {
                                //First check then update...
                                QBResponse objResponse = new QBResponse();
                                //This class is get Config details
                                QBOnlineconfiguration objOnlieDetail = _QBRepository.GetConfigDetail(invoice.StoreId);
                                //Get Sync Token
                                string SyncToken = _QBRepository.GetSyncToken(invoice.TXNId, objOnlieDetail, ref objResponse);
                                if (SyncToken == "")
                                {

                                    //This class is create bill in Quick books
                                    //Bill not found in QB, Now try to create...
                                    _QBRepository.CreateBill(Convert.ToInt32(id), objList);
                                    StatusMessage = "Success1";
                                    //ViewBag.Delete = "Bill Create Successfully in QuickBooks.";
                                    ViewBag.Delete = _CommonRepository.GetMessageValue("INBC", "Bill Create Successfully in QuickBooks.");
                                }
                                else
                                {

                                    //This class is Edit bill in Quick books
                                    _QBRepository.QBEditBillData(invoice.InvoiceId, invoice.TXNId, invoice.InvoiceNumber, Convert.ToDateTime(invoice.InvoiceDate), invoice.Note, Convert.ToInt32(invoice.StoreId), Convert.ToInt32(invoice.VendorId), objList, ref objResponse);
                                    if (objResponse.Status == "Done")
                                    {
                                        StatusMessage = "Edit";
                                        //ViewBag.Delete = "Bill Update Successfully in QuickBooks.";
                                        ViewBag.Delete = _CommonRepository.GetMessageValue("INBE", "Bill Update Successfully in QuickBooks.");
                                    }
                                }
                            }
                        }
                        objList = null;
                    }
                }


            }
            catch (Exception ex)
            {

                logger.Error("InvoicesController - ForceInvoiceSync - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(message);
        }

        /// <summary>
        /// This method is get invoice details by Invoice ID
        /// </summary>
        /// <param name="InvoiceID"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetInvoiceDetails(int InvoiceID)
        {
            try
            {
                if (InvoiceID > 0)
                {
                    //Get List of Invoice Department details by Invoice Id
                    List<InvoiceDepartmentDetail> Departments = _invoiceRepository.GetInvoiceDepartmentDetailsbyInvoiceID(InvoiceID).ToList().Select(s => new InvoiceDepartmentDetail { DepartmentId = s.DepartmentId, DepartmentName = s.DepartmentName, Amount = s.Amount }).ToList();
                    //Get Department Master by department ID
                    var Deptlist = _invoiceRepository.GetDepartmentMastersListbyDepartmentId(Departments);
                    var q = (from pd in Departments
                             join od in Deptlist on pd.DepartmentId equals od.DepartmentId
                             select new
                             {
                                 od.DepartmentName,
                                 pd.Amount
                             }).ToList();
                    return Json(q, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {

                logger.Error("InvoicesController - GetInvoiceDetails - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        //public async Task<ActionResult> ExcelExport(string gridModel, DataManagerRequest dm)
        //{
        //    List<InvoiceSelect> lstProducts = new List<InvoiceSelect>();

        //    var UserTypeId = _CommonRepository.getUserTypeId(User.Identity.Name);
        //    var storeid = Convert.ToInt32(Session["storeid"]).ToString();
        //    var deptid = "";
        //    lstProducts = _invoiceRepository.InvoiceDataDepartmentWise_Get(deptid, storeid, UserTypeId,dm);

        //    GridExcelExport exp = new GridExcelExport();
        //    exp.FileName = "Invoice.xlsx";
        //    Syncfusion.EJ2.Grids.Grid gridProperty = ConvertGridObject(gridModel, lstProducts);
        //    return exp.ExcelExport<InvoiceSelect>(gridProperty, lstProducts);

        //}

        //private Syncfusion.EJ2.Grids.Grid ConvertGridObject(string gridProperty, List<InvoiceSelect> data)
        //{
        //    Syncfusion.EJ2.Grids.Grid GridModel = (Syncfusion.EJ2.Grids.Grid)Newtonsoft.Json.JsonConvert.DeserializeObject(gridProperty, typeof(Syncfusion.EJ2.Grids.Grid));

        //    GridColumnModel cols = (GridColumnModel)Newtonsoft.Json.JsonConvert.DeserializeObject("{\"allowGrouping\":false,\"allowPaging\":false,\"pageSettings\":{\"currentPage\":1,\"pageCount\":2,\"pageSize\":100},\"sortSettings\":{},\"allowPdfExport\":true,\"allowExcelExport\":true,\"aggregates\":[],\"filterSettings\":{\"immediateModeDelay\":1500,\"type\":\"CheckBox\"},\"groupSettings\":{\"columns\":[],\"enableLazyLoading\":false,\"disablePageWiseAggregates\":false},\"columns\":[],\"locale\":\"en-US\",\"searchSettings\":{\"key\":\"\",\"fields\":[]}}", typeof(GridColumnModel));
        //    cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "VendorName", HeaderText = "Vendor Name" });
        //    cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "InvoiceType", HeaderText = "Invoice Type" });
        //    cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "InvoiceNumber", HeaderText = "Invoice #" });
        //    cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "StoreName", HeaderText = "Store Name" });
        //    cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "InvoiceDate", HeaderText = "Invoice Date" });
        //    cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "TotalAmount", HeaderText = "Amount" });
        //    cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "UploadInvoice", HeaderText = "Docs" });
        //    cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "StatusValue", HeaderText = "Status" });
        //    cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "IsQbStatus", HeaderText = "QB Status" });
        //    cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "PaymentType", HeaderText = "Payment Method" });

        //    foreach (var item in cols.columns)
        //    {
        //        item.AutoFit = true;
        //        item.Width = "10%";
        //    }

        //    GridModel.Columns = cols.columns;
        //    return GridModel;
        //}

        //public ActionResult PdfExport(string gridModel)
        //{
        //    List<InvoiceSelect> lstProducts = new List<InvoiceSelect>();
        //    var UserTypeId = _CommonRepository.getUserTypeId(User.Identity.Name);
        //    var storeid = Convert.ToInt32(Session["storeid"]).ToString();
        //    var deptid = "";
        //    lstProducts = _invoiceRepository.InvoiceDataDepartmentWise_Get(deptid, storeid, UserTypeId);

        //    PdfDocument doc = new PdfDocument();
        //    doc.PageSettings.Orientation = PdfPageOrientation.Landscape;
        //    doc.PageSettings.Size = PdfPageSize.A3;


        //    GridPdfExport exp = new GridPdfExport();
        //    exp.Theme = "flat-saffron";
        //    exp.FileName = "Invoice.pdf";
        //    exp.PdfDocument = doc;

        //    Syncfusion.EJ2.Grids.Grid gridProperty = ConvertGridObject(gridModel, lstProducts);
        //    return exp.PdfExport<InvoiceSelect>(gridProperty, lstProducts);
        //}

        #region Invoice Count Dani 01-04-2024
        public ActionResult InvoiceCount()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetInvoiceCountData(string Date, int InvoiceType)
        {
            try
            {
                List<InvoiceCount> data = new List<InvoiceCount>();
                data = _invoiceRepository.InvoiceCounts(Date, InvoiceType);
                if (data != null)
                {
                    return Json(JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
        #region Dani - Invoice Count PDf and Excel Export - 10-04-2024
        public ActionResult DownloadInvoiceCountPDF(string Date, int InvoiceType)
        {
            try
            {
                var data = InvoiceCountPDF(Date, InvoiceType);
                string htmlContent = RenderViewToString("InvoiceCountPDF", data);
                byte[] pdfBytes = GeneratePdf(htmlContent.ToString());
                return File(pdfBytes, "application/pdf", "InvoiceCounts.pdf");
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                logger.Error("InvoicesController - DownloadInvoiceCountPDF - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return RedirectToAction("InvoiceCount");
        }
        public List<InvoiceCount> InvoiceCountPDF(string Date, int InvoiceType)
        {
            List<InvoiceCount> data = new List<InvoiceCount>();
            data = _invoiceRepository.InvoiceCounts(Date, InvoiceType);
            return data;
        }

        protected string RenderViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
        protected byte[] GeneratePdf(string htmlContent)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                iTextSharp.text.Document document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();
                iTextSharp.text.html.simpleparser.StyleSheet styles = new iTextSharp.text.html.simpleparser.StyleSheet();
                iTextSharp.text.html.simpleparser.HTMLWorker hw = new iTextSharp.text.html.simpleparser.HTMLWorker(document);
                hw.SetStyleSheet(styles);
                hw.Parse(new StringReader(htmlContent));
                document.Close();
                return ms.ToArray();
            }
        }

        public ActionResult DownloadInvoiceCountExcel(string Date, int InvoiceType)
        {
            try
            {
                var data = InvoiceCountExcel(Date, InvoiceType);
                string htmlContent = RenderViewToString("InvoiceCountExcel", data);
                byte[] excelBytes = GenerateExcel(htmlContent.ToString());
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "InvoiceCounts.xlsx");
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                logger.Error("InvoicesController - DownloadInvoiceCountExcel - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return RedirectToAction("InvoiceCount");
        }
        public List<InvoiceCount> InvoiceCountExcel(string Date, int InvoiceType)
        {
            List<InvoiceCount> data = new List<InvoiceCount>();
            data = _invoiceRepository.InvoiceCounts(Date, InvoiceType);
            return data;
        }
        protected byte[] GenerateExcel(string htmlContent)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (ExcelPackage package = new ExcelPackage(ms))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Invoice Counts");

                    LoadHtmlContentToWorksheet(htmlContent, worksheet);

                    worksheet.Cells.AutoFitColumns();

                    package.Save();
                }

                return ms.ToArray();
            }
        }
        protected void LoadHtmlContentToWorksheet(string htmlContent, ExcelWorksheet worksheet)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            HtmlNodeCollection rows = doc.DocumentNode.SelectNodes("//table/tr");

            int rowIndex = 1;
            foreach (HtmlNode row in rows)
            {
                HtmlNodeCollection cells = row.SelectNodes("td|th");
                int columnIndex = 1;
                foreach (HtmlNode cell in cells)
                {
                    worksheet.Cells[rowIndex, columnIndex].Value = cell.InnerText.Trim();

                    if (cell.Name == "th")
                    {
                        worksheet.Cells[rowIndex, columnIndex].Style.Font.Bold = true;
                        worksheet.Cells[rowIndex, columnIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[rowIndex, columnIndex].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }
                    else
                    {
                        worksheet.Cells[rowIndex, columnIndex].Style.Font.Bold = false;
                        worksheet.Cells[rowIndex, columnIndex].Style.Fill.PatternType = ExcelFillStyle.None;
                    }

                    columnIndex++;
                }
                rowIndex++;
            }
        }
        #endregion


        //Changes done by Dani on 19/07/2024
        public ActionResult InvoiceAutomationIndex(DataManagerRequest dm, bool ShowMyInvoice)
        {
            List<UploadPDFAutomationList> uploadfileslist = new List<UploadPDFAutomationList>();
            try
            {
                if (Session["StoreId"] != null)
                {
                    if (!string.IsNullOrEmpty(Session["StoreId"].ToString()))
                    {

                        int store_idval = Convert.ToInt32(Session["StoreId"]);
                        ViewBag.Storeidvalue = store_idval;
                    }
                }

                int userid = UserModule.getUserId();
                if (ShowMyInvoice == true)
                {
                    uploadfileslist = _invoiceRepository.getuploadautomationlist(ViewBag.Storeidvalue, userid);
                }
                else
                {
                    uploadfileslist = _invoiceRepository.getuploadautomationlist(ViewBag.Storeidvalue, 0);
                }
                IEnumerable DataSource = uploadfileslist;
                int Count = 0;
                DataOperations operation = new DataOperations();

                if (dm.Search != null && dm.Search.Count > 0)
                {
                    string search = dm.Search[0].Key.ToString().Trim();
                    int? statusSearchValue = null;
                    if (search.Equals("Approved", StringComparison.OrdinalIgnoreCase))
                    {
                        statusSearchValue = 1;
                    }
                    else if (search.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                    {
                        statusSearchValue = 0;
                    }

                    DateTime searchDate;
                    bool isDate = DateTime.TryParseExact(search, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out searchDate);

                    DataSource = uploadfileslist.ToList().Where(x => x.FileName.Contains(search) || x.StoreName.Contains(search) || (isDate && x.CreatedDate.HasValue && x.CreatedDate.Value.Date == searchDate.Date) || x.UserName.Contains(search) || (statusSearchValue != null && x.IsProcess == statusSearchValue)).ToList();
                }

                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                }
                Count = DataSource.Cast<UploadPDFAutomationList>().Count();
                if (dm.Skip != 0)
                {
                    DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
                }
                if (dm.Take != 0)
                {
                    DataSource = operation.PerformTake(DataSource, dm.Take);
                }
                ViewBag.InvoicesReviCount = uploadfileslist.Count;
                return dm.RequiresCounts ? Json(new { result = DataSource, count = Count }) : Json(DataSource);

            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - InvoiceAutomationGrid - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }

        [Authorize(Roles = "Administrator,ViewBulkUploadFile")]
        public ActionResult InvoiceAutomationGrid()
        {
            if (Session["StoreId"] != null)
            {
                if (!string.IsNullOrEmpty(Session["StoreId"].ToString()))
                {

                    int store_idval = Convert.ToInt32(Session["StoreId"]);
                    ViewBag.Storeidvalue = store_idval;
                }
            }
            List<UploadPDFAutomationList> uploadfileslist = new List<UploadPDFAutomationList>();


            int userid = UserModule.getUserId();
            int storeid = ViewBag.Storeidvalue ?? 0;
            uploadfileslist = _invoiceRepository.getuploadautomationlist(storeid, userid);
            ViewBag.InvoicesReviCount = uploadfileslist.Count;
            return View();
        }

        [AcceptVerbs("Post")]
        public void Save()
        {
            SaveFile++;
            try
            {
                int? StoreID = null;
                var selectedMode = Session["selectedMode"].ToString();
                //string Sacn_Title = "";
                if (Session["StoreId"] != null && !string.IsNullOrEmpty(Session["StoreId"].ToString()))
                {
                    StoreID = Convert.ToInt32(Session["StoreId"]);
                }
                if (System.Web.HttpContext.Current.Request.Files.AllKeys.Length > 0)
                {
                    var httpPostedFile = System.Web.HttpContext.Current.Request.Files["UploadFiles"];
                    //Sacn_Title = AdminSiteConfiguration.GetRandomNo() + Path.GetFileName(httpPostedFile.FileName);
                    //Sacn_Title = AdminSiteConfiguration.RemoveSpecialCharacter(Sacn_Title);
                    if (httpPostedFile != null)
                    {
                        var baseFolderPath = System.Web.HttpContext.Current.Server.MapPath("~/UserFiles/Automatic-Invoice-File");
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

                        var Specialcharacter = DateTime.Now.ToString("MMddyyyy") + "_" + DateTime.Now.ToString("HHmmssfff") + "_" + Path.GetFileNameWithoutExtension(httpPostedFile.FileName) + Path.GetExtension(httpPostedFile.FileName);
                        var fileName = Regex.Replace(Specialcharacter, "[^a-zA-Z0-9_\\.]", "");
                        var fileSavePath = Path.Combine(userIdFolderPath, fileName);
                        if (!System.IO.File.Exists(fileSavePath))
                        {
                            logger.Info(httpPostedFile.FileName + " File uploading processing..");

                            //for store in MultithreadingInvoiceLog table
                            int MultithreadingInvoiceLogId = 0;
                            MultithreadingInvoiceLogId = _invoiceRepository.UploadProcessStart();

                            httpPostedFile.SaveAs(fileSavePath);
                            string formattedDateTime = DateTime.Now.ToString("MMddyyyyHHmmssfff");
                            string queueId = UserModule.getUserId().ToString() + formattedDateTime;
                            var relativePath = fileSavePath.Substring(baseFolderPath.Length).TrimStart('\\', '/');

                            UploadPDFAutomation uploadPDF = new UploadPDFAutomation
                            {
                                FileName = relativePath,
                                IsProcess = 0,
                                CreatedBy = UserModule.getUserId(),
                                CreatedDate = DateTime.Now,
                                UpdatedBy = null,
                                UpdatedDate = null,
                                PageCount = 0,
                                StoreId = StoreID == 0 ? null : StoreID,
                                Synthesis_Live_InvID = 0,
                                IsDeleted = false,
                                Is_Processing_Enabled = selectedMode == "Automatic" ? true : false,
                                QueueId = queueId
                            };

                            int UploadPdfAutomationId = _invoiceRepository.SaveAutoMationFileChanges(uploadPDF);

                            HttpResponse Response = System.Web.HttpContext.Current.Response;
                            Response.Clear();
                            Response.Headers.Add("name", httpPostedFile.FileName);
                            Response.ContentType = "application/json; charset=utf-8";
                            Response.StatusDescription = "Ready to Upload";

                            var responseObj = new { name = relativePath };
                            var jsonResponse = new JavaScriptSerializer().Serialize(responseObj);
                            Response.Write(jsonResponse);
                            Response.End();

                            _invoiceRepository.UploadProcessFinish(MultithreadingInvoiceLogId, UploadPdfAutomationId);
                            logger.Info("UploadPdfAutomationId :" + UploadPdfAutomationId + ",UserId :" + Convert.ToInt32(Session["UserId"]) + ",FileName :" + fileName + ",StoreID :" + StoreID + " Successfully Inserted.");
                        }
                        else
                        {
                            HttpResponse Response = System.Web.HttpContext.Current.Response;
                            Response.Clear();
                            Response.Status = "204 File already exists";
                            Response.StatusCode = 204;
                            Response.StatusDescription = "File already exists";
                            Response.End();
                            logger.Info(httpPostedFile.FileName + "File already exists.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - Save - " + DateTime.Now + " - " + ex.Message.ToString());
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

        public ActionResult InsertUploadFileAutomation(string fileName, string enabledmode)
        {
            logger.Info("InvoicesController - InsertUploadFileAutomation - " + DateTime.Now);
            string message = "";
            try
            {
                int? StoreID = null;
                if (Session["StoreId"] != null && !string.IsNullOrEmpty(Session["StoreId"].ToString()))
                {
                    StoreID = Convert.ToInt32(Session["StoreId"]);
                }

                var fileNamesArray = fileName.Split(',');

                foreach (var file in fileNamesArray)
                {
                    logger.Info(file + " File inserting processing..");

                    string formattedDateTime = DateTime.Now.ToString("MMddyyyyHHmm");
                    string queueId = UserModule.getUserId().ToString() + formattedDateTime;

                    UploadPDFAutomation uploadPDF = new UploadPDFAutomation
                    {
                        FileName = file,
                        IsProcess = 0,
                        CreatedBy = UserModule.getUserId(),
                        CreatedDate = DateTime.Now,
                        UpdatedBy = null,
                        UpdatedDate = null,
                        PageCount = 0,
                        StoreId = StoreID == 0 ? null : StoreID,
                        Synthesis_Live_InvID = 0,
                        IsDeleted = false,
                        Is_Processing_Enabled = enabledmode == "Automatic" ? true : false,
                        QueueId = queueId
                    };

                    int UploadPdfAutomationId = _invoiceRepository.SaveAutoMationFileChanges(uploadPDF);
                    logger.Info("UploadPdfAutomationId :" + UploadPdfAutomationId + ",UserId :" + Convert.ToInt32(Session["UserId"]) + ",FileName :" + file + ",StoreID :" + StoreID + " Successfully Inserted.");
                }

                message = "Success";
            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - InsertUploadFileAutomation - " + DateTime.Now + " - " + ex.Message.ToString());
                message = "Error";
            }
            return Json(new { success = message });
        }

        public ActionResult DeleteInvoiceAutomation(int? id)
        {
            logger.Info("InvoicesController - DeleteInvoiceAutomation - " + DateTime.Now);
            string message = "";
            try
            {
                UploadPDFAutomation uploadPDF = _invoiceRepository.GetUploadAutomation(id.Value);

                //var FileName = uploadPDF.FileName;
                //var fileSave = "";
                //var fileMove = "";
                //fileSave = System.Web.HttpContext.Current.Server.MapPath("~/UserFiles/BulkUploadFile");
                //fileMove = System.Web.HttpContext.Current.Server.MapPath("~/UserFiles/DeleteBulkUploadFile");
                //if (!System.IO.File.Exists(fileMove))
                //{
                //    System.IO.Directory.CreateDirectory(fileMove);
                //}
                //var fileSavePath = Path.Combine(fileSave, uploadPDF.FileName);
                //var fileMovePath = Path.Combine(fileMove, uploadPDF.FileName);
                //if (System.IO.File.Exists(fileSavePath))
                //{
                //    System.IO.File.Move(fileSavePath, fileMovePath);
                //    AdminSiteConfiguration.WriteErrorLogs("File successfully moved to another folder.");
                //}
                _invoiceRepository.DeleteInvoiceAutomation(id.Value);

                ActivityLog ActLog = new ActivityLog();
                ActLog.Action = 3;
                ActLog.Comment = "InvoiceAutomation Upload file Deleted by " + UserModule.getUserFirstName() + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                //This  class is used for Activity Log Insert.
                _activityLogRepository.ActivityLogInsert(ActLog);
                message = "Delete";
                AdminSiteConfiguration.WriteErrorLogs(uploadPDF.FileName + " File removed successfully");
                logger.Info(uploadPDF.FileName + " File removed successfully");
            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - DeleteInvoiceAutomation - " + DateTime.Now + " - " + ex.Message.ToString());
                message = "Error";
            }
            return Json(new { success = message });
        }

        [Authorize(Roles = "Administrator,UpdateInvoice")]
        // GET: Invoices/Edit/5
        public async Task<ActionResult> InvoiceAutomationEdit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.CurrentDate = DateTime.Today.Date;
            InvoiceAutomation invoice = new InvoiceAutomation();
            try
            {
                //This class is Edit Inovoice Object
                invoice = _invoiceRepository.EditInvoiceAutomationObj(id.Value);

                if (invoice.InvoiceTypeId == 2 && invoice.TotalAmount < 0 || invoice.ChildAmount < 0)
                {
                    invoice.TotalAmount = Math.Abs(invoice.TotalAmount);
                    invoice.ChildAmount = Math.Abs(invoice.ChildAmount.Value);
                }

                if (invoice == null)
                {
                    return HttpNotFound();
                }
                Session["storeid"] = invoice.StoreId;

                //This db class is get Invoice master
                ViewBag.InvoiceTypeId = new SelectList(_MastersBindRepository.GetInvoiceTypeMaster(), "InvoiceTypeId", "InvoiceType", invoice.InvoiceTypeId);

                //This class is get department masters
                ViewBag.Disc_Dept_id = new SelectList(_MastersBindRepository.GetDepartmentMasters(invoice.StoreId).OrderBy(o => o.DepartmentName).ToList(), "DepartmentId", "DepartmentName");

                //Get Discount Type master
                ViewBag.DiscountTypeId = new SelectList(_MastersBindRepository.GetDiscountTypeMaster(), "DiscountTypeId", "DiscountType", 1);
                //Get Payment Type Master
                ViewBag.PaymentTypeId = new SelectList(_MastersBindRepository.GetPaymentTypeMaster(), "PaymentTypeId", "PaymentType", 2);
                //This class is Get Storelist
                ViewBag.StoreId = new SelectList(_CommonRepository.GetStoreList(1), "StoreId", "NickName", invoice.StoreId);
                //This db class is get Vendor master
                ViewBag.VendorId = new SelectList(_MastersBindRepository.GetVendorMaster(invoice.StoreId).Select(s => new { s.VendorId, s.VendorName }).ToList().OrderBy(o => o.VendorName), "VendorId", "VendorName", invoice.VendorId);
                ViewBag.VendorName = invoice.VendorName;
                ViewBag.VendorIdSele = invoice.VendorId;

                ViewBag.closingyear = Convert.ToInt32(ConfigurationManager.AppSettings["ClosingYear"].ToString()) + 1;

                //Added Logic to handle the total amount to first department & rest of dept set the amount to be 0.00 by Dani on 12-11-2025.

                // Store the original total amount for first department
                ViewBag.OriginalTotalAmount = invoice.TotalAmount;

                if (Session["StoreId"] != null)
                {
                    if (!string.IsNullOrEmpty(Session["StoreId"].ToString()))
                    {

                        int store_idval = Convert.ToInt32(Session["StoreId"]);
                        ViewBag.Storeidvalue = store_idval;
                    }
                }
                //ViewBag.InvoiceDepartmentDetails = _invoiceRepository.getuploadautomationlist(ViewBag.Storeidvalue);
                //for (int j = 0; j < ViewBag.InvoiceDepartmentDetails.Count; j++)
                //{
                //    InvoiceAutomation detail = new InvoiceAutomation();

                //    detail.DepartmentId = 0;
                //    detail.TotalAmount = ViewBag.InvoiceDepartmentDetails[j].TotalAmount;

                //    ViewBag.InvoiceDepartmentDetails.Add(detail);
                //}
            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - Edit - " + DateTime.Now + " - " + ex.Message.ToString());
            }


            return View(invoice);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,CreateInvoice")]
        public async Task<ActionResult> InvoiceAutomationEdit(InvoiceAutomation invoice, HttpPostedFileBase UploadInvoice, string[] ChildDepartmentId, string[] ChildAmount, string btnsubmit = "")
        {
            try
            {
                if (invoice.CheckDup == 0 || invoice.CheckDup == 1)
                {
                    ChildAmount = ChildAmount.Select(s => string.IsNullOrWhiteSpace(s) ? "0" : s).ToArray();
                    var bulk = 0;
                    if (invoice.strInvoiceDate == null || invoice.strInvoiceDate == "")
                    {
                        ModelState.AddModelError("strInvoiceDate", "Required");
                    }
                    // Ignore City from ModelState.
                    invoice.QBTransfer = (invoice.QBtransferss == "1" ? true : false);
                    int Storeidval = 0;
                    if (invoice.DiscountTypeId == 1)
                    {
                        ModelState.Remove("Disc_Dept_id");
                    }
                    if (ModelState.IsValid)
                    {

                        if (UploadInvoice == null)
                        {
                            string filePath = invoice.UploadInvoice;
                            if (filePath.Contains(@"\") || filePath.Contains("/"))
                            {
                                byte[] bytes = System.IO.File.ReadAllBytes(HttpContext.Server.MapPath("~/UserFiles/Automatic-Invoice-File/" + invoice.UploadInvoice));
                                var contentTypeFile = "application/pdf";
                                var fileName = invoice.UploadInvoice;
                                UploadInvoice = (HttpPostedFileBase)new HttpPostedFileBaseCustom(new MemoryStream(bytes), contentTypeFile, fileName);
                                bulk = 1;
                            }
                            else
                            {
                                byte[] bytes = System.IO.File.ReadAllBytes(HttpContext.Server.MapPath("~/UserFiles/BulkUploadFile/" + invoice.UploadInvoice));
                                var contentTypeFile = "application/pdf";
                                var fileName = invoice.UploadInvoice;
                                UploadInvoice = (HttpPostedFileBase)new HttpPostedFileBaseCustom(new MemoryStream(bytes), contentTypeFile, fileName);
                                bulk = 1;
                            }
                        }
                        //This  class is used for Activity Log Insert.
                        string Store = _QBRepository.GetStoreOnlineDesktop(invoice.StoreId);
                        //Using this db class get stores on Line desktop Flag.
                        int StoreFlag = _QBRepository.GetStoreOnlineDesktopFlag(invoice.StoreId);

                        char[] MyChar = { '0' };
                        string Inoicno = invoice.InvoiceNumber.TrimStart(MyChar);
                        Console.WriteLine(Inoicno);
                        invoice.InvoiceNumber = Inoicno;
                        if (invoice.strInvoiceDate != null)
                        {
                            invoice.InvoiceDate = Convert.ToDateTime(invoice.strInvoiceDate);
                        }
                        if (Roles.IsUserInRole("Administrator"))
                        {
                            Storeidval = invoice.StoreId;
                        }
                        else
                        {
                            if (invoice.StoreId != 0)
                            {
                                Storeidval = invoice.StoreId;
                            }
                            else
                            {
                                if (Session["storeid"] == null)
                                {
                                    Session["storeid"] = "0";
                                }
                                if (Convert.ToString(Session["storeid"]) != "0")
                                {
                                    Storeidval = Convert.ToInt32(Session["storeid"].ToString());
                                }
                                else
                                {
                                    RedirectToAction("Index", "Login", new { area = "" });
                                }
                            }
                        }
                        #region AttachNote
                        //This class is Invoice Attach Note
                        int FlgInvAtch = InvoiceAutomationAttachNote(invoice, UploadInvoice);
                        if (FlgInvAtch == 1)
                        {
                            ViewBag.StatusMessage = "InvalidImage";
                            return View(invoice);
                        }
                        else if (FlgInvAtch == 2)
                        {
                            ViewBag.StatusMessage = "InvalidPDFSize";
                            return View(invoice);
                        }
                        #endregion
                        int iInvoiceId = 0; string iInvoiceStatus = "";
                        //---------- User Aprrove Rights Code Check----------/// 
                        //This class is get Module Masters Id
                        var ModuleId = _invoiceRepository.GetModuleMastersId();
                        bool roleFlg = false;
                        try
                        {
                            if (!Roles.IsUserInRole("Administrator"))
                            {
                                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                                int UserTypeId = _CommonRepository.getUserTypeId(UserName);
                                if (UserTypeId > 0)
                                {
                                    //this db class is check user type module Approvers
                                    if (_invoiceRepository.CheckUserTypeModuleApprovers(UserTypeId, ModuleId))
                                    {
                                        roleFlg = true;
                                    }
                                }
                            }


                        }
                        catch (Exception ex)
                        {
                            logger.Error("InvoicesController - InvoiceAutomationEdit - " + DateTime.Now + " - " + ex.Message.ToString());
                        }
                        bool FLG = GetroleForCRApproval("ApproveInvoice", Convert.ToInt32(invoice.StoreId), 1);

                        //------------------------End-------------------------------//

                        string Creditdiscount = "";
                        string Credit_Invoice = invoice.InvoiceNumber + "_cr";
                        if (invoice.InvoiceTypeId == 2 && invoice.DiscountPercent == null)
                        {
                            Creditdiscount = invoice.TotalAmount.ToString();
                            invoice.InvoiceNumber = Credit_Invoice + Creditdiscount;
                        }

                        //This class is Create Invoice Post
                        InvoiceFlgs invoiceFlgs = _invoiceRepository.CreateInvoiceAutomationPost(invoice, Roles.IsUserInRole("Administrator"), Roles.IsUserInRole("nnfapprovalInvoice"), FLG, roleFlg, Store, StoreFlag, ChildDepartmentId, ChildAmount, Storeidval, User.Identity.Name);
                        bool QuickCRInvoice = GetroleForCRApproval("nnfapprovalCrdMemoInvoice", invoice.StoreId, 1);
                        try
                        {
                            if (Convert.ToInt32(invoiceFlgs.iInvoiceId) > 0)
                            {
                                if (ChildDepartmentId != null)
                                {
                                    int j = 0;
                                    foreach (var val_id in ChildDepartmentId)
                                    {
                                        if (!String.IsNullOrEmpty(ChildAmount[j].ToString()))
                                        {
                                            if (Convert.ToDecimal(ChildAmount[j]) > 0) // Remove "=" Sign to Stop 0 Amount Department
                                            {
                                                InvoiceDepartmentDetail deptDetail = new InvoiceDepartmentDetail();
                                                deptDetail.InvoiceId = invoiceFlgs.iInvoiceId;
                                                deptDetail.DepartmentId = Convert.ToInt32(val_id);
                                                deptDetail.Amount = Convert.ToDecimal(ChildAmount[j]);
                                                //Insert Invoice Department Details.
                                                _invoiceRepository.InsertInvoiceDepartmentDetails(deptDetail);
                                            }
                                        }
                                        j++;
                                    }
                                }
                                if (invoice.QuickInvoice == "1")
                                {
                                    ActivityLogMessage = "Quick Invoice Number " + "<a href='/Invoices/Details/" + invoiceFlgs.iInvoiceId + "'>" + invoice.InvoiceNumber + "</a> Created by " + UserModule.getUserFirstName() + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                                }
                                else
                                {
                                    ActivityLogMessage = "Invoice Number " + "<a href='/Invoices/Details/" + invoiceFlgs.iInvoiceId + "'>" + invoice.InvoiceNumber + "</a> Created by " + UserModule.getUserFirstName() + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                                }
                                ActivityLog ActLog1 = new ActivityLog();
                                ActLog1.UserId = _CommonRepository.getUserId(User.Identity.Name);
                                ActLog1.Action = 1;
                                ActLog1.Comment = ActivityLogMessage;
                                //This  class is used for Activity Log Insert.
                                _ActivityLogRepository.ActivityLogInsert(ActLog1);

                                IsArray = false;
                                if (invoice.QuickInvoice == "1")
                                {
                                    StatusMessage = "Success1";
                                    ViewBag.StatusMessage = StatusMessage;
                                }
                                else
                                {
                                    StatusMessage = "Success";
                                    ViewBag.StatusMessage = StatusMessage;
                                }


                                if (invoice.DiscountTypeId != 1)
                                {
                                    //db.Database.ExecuteSqlCommand("SP_InvoiceDiscount @Mode = {0},@InvoiceId = {1},@DiscountTypeId = {2},@DiscountAmount = {3},@DiscountPercent = {4}", "UpdateDiscountDetail", iInvoiceId, invoice.DiscountTypeId, invoice.DiscountAmount, (invoice.DiscountPercent == null ? 0 : invoice.DiscountPercent));
                                    _invoiceRepository.SaveInvoiceDiscount(invoiceFlgs.iInvoiceId, invoice.DiscountTypeId, invoice.DiscountAmount, (invoice.DiscountPercent == null ? 0 : invoice.DiscountPercent));
                                    int iiInvoiceID = 0;
                                    var DSacn_Title = "";
                                    string InvoiceNM = "";
                                    if (UploadInvoice != null)
                                    {
                                        if (UploadInvoice.ContentLength > 0)
                                        {
                                            var allowedExtensions = new[] { ".pdf" };
                                            var extension = Path.GetExtension(UploadInvoice.FileName);
                                            var Ext = Convert.ToString(extension).ToLower();
                                            if (!allowedExtensions.Contains(Ext))
                                            {
                                                ViewBag.StatusMessage = "InvalidImage";
                                                if (bulk == 1)
                                                {
                                                    return View("InvoiceAutomationGrid", "Invoices");
                                                }
                                                else
                                                {
                                                    return View("InvoiceAutomationGrid");
                                                }
                                            }
                                            else
                                            {
                                                string FileName = invoice.InvoiceDate.ToString("MMddyyyy") + "-" + _MastersBindRepository.GetVendorRemoveSpecialCarecters(invoice.VendorId) + "-" + AdminSiteConfiguration.RemoveSpecialCharacterInvoice(Inoicno) + "-" + "CR" + "-" + DateTime.Now.Ticks.ToString() + ".pdf";
                                                //"CR" + "-" + AdminSiteConfiguration.RemoveSpecialCharacterInvoice(Inoicno) + "-" + invoice.InvoiceDate.ToString("MMddyyyy") + "-" + GetVendorName(invoice.VendorId) + ".pdf";
                                                string PathRel = CreateDirectory(invoice.InvoiceDate.ToString("MMddyyyy"), invoice.VendorId, invoice.StoreId);
                                                string FullPath = PathRel + "\\" + FileName;
                                                UploadInvoice.SaveAs(FullPath);
                                                InvoiceNM = FullPath.Replace(Request.PhysicalApplicationPath + "UserFiles\\Invoices\\", "");

                                            }
                                        }
                                    }
                                    string CRdiscount = "";
                                    string Credit_Invoiceno = invoice.InvoiceNumber + "_cr";
                                    if (invoice.DiscountTypeId == 2)
                                    {
                                        CRdiscount = invoice.DiscountPercent.ToString();
                                        Credit_Invoiceno = Credit_Invoiceno + CRdiscount + "%";
                                    }
                                    //This db class is Create Invoice Credit memo post
                                    iiInvoiceID = _invoiceRepository.CreateInvoiceAutomationCreditmemoPost(invoice, Roles.IsUserInRole("ApproveInvoice"), Roles.IsUserInRole("Administrator"), Store, StoreFlag, User.Identity.Name, invoiceFlgs.iInvoiceId, Credit_Invoiceno, InvoiceNM, invoiceFlgs.iInvoiceStatus);

                                    if (iiInvoiceID > 0)
                                    {
                                        if (invoice.QuickInvoice == "1")
                                        {
                                            ActivityLogMessage = "Quick Invoice Number " + "<a href='/Invoices/Details/" + invoiceFlgs.iInvoiceId + "'>" + invoice.InvoiceNumber + "</a> Created by " + UserModule.getUserFirstName() + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                                        }
                                        else
                                        {
                                            ActivityLogMessage = "Invoice Number " + "<a href='/Invoices/Details/" + invoiceFlgs.iInvoiceId + "'>" + invoice.InvoiceNumber + "</a> Created by " + UserModule.getUserFirstName() + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                                        }

                                        ActivityLog ActLog2 = new ActivityLog();
                                        ActLog2.UserId = _CommonRepository.getUserId(User.Identity.Name);
                                        ActLog2.Action = 1;
                                        ActLog2.Comment = ActivityLogMessage;
                                        //This  class is used for Activity Log Insert.
                                        _ActivityLogRepository.ActivityLogInsert(ActLog2);
                                    }
                                }
                                if (invoice.btnName == "Save & New")
                                {
                                    invoice.DepartmentMasters = _invoiceRepository.getDepartment_WithSP(invoice.StoreId).ToList();
                                    ViewBag.Disc_Dept_id = new SelectList(_MastersBindRepository.GetAllDepartmentMasters().OrderBy(o => o.DepartmentName).ToList(), "DepartmentId", "DepartmentName", invoice.Disc_Dept_id);
                                    ViewBag.DiscountTypeId = new SelectList(_MastersBindRepository.GetDiscountTypeMaster(), "DiscountTypeId", "DiscountType", invoice.DiscountTypeId);
                                    ViewBag.InvoiceTypeId = new SelectList(_MastersBindRepository.GetInvoiceTypeMaster(), "InvoiceTypeId", "InvoiceType", invoice.InvoiceTypeId);
                                    ViewBag.PaymentTypeId = (Convert.ToString(Session["storeid"]) != "0" ? new SelectList(_MastersBindRepository.GetPaymentTypeMaster(), "PaymentTypeId", "PaymentType", invoice.PaymentTypeId).Skip(0) : new SelectList(_MastersBindRepository.GetPaymentTypeMaster(), "PaymentTypeId", "PaymentType", invoice.PaymentTypeId));
                                    //new SelectList(db.PaymentTypeMasters, "PaymentTypeId", "PaymentType", invoice.PaymentTypeId);
                                    ViewBag.StoreId = new SelectList(_CommonRepository.GetStoreList(1), "StoreId", "NickName", invoice.StoreId);
                                    ViewBag.VendorId = new SelectList(_MastersBindRepository.GetVendorMaster(invoice.StoreId).Select(s => new { s.VendorId, s.VendorName }).ToList(), "VendorId", "VendorName", invoice.VendorId);
                                }
                                else
                                {
                                    string message = StatusMessage;
                                    if ((invoice.QuickInvoice == "1" && ((Roles.IsUserInRole("Administrator")) || Roles.IsUserInRole("nnfapprovalInvoice"))) || (FLG == true && roleFlg == false))
                                    {
                                        message = "Success2";
                                    }
                                    if (invoice.DiscountTypeId != 1 && QuickCRInvoice == true)
                                    {
                                        message = "Success2";
                                    }
                                    //StatusMessage = "";
                                    if (bulk == 1)
                                    {
                                        StatusMessage = "";
                                        return RedirectToAction("InvoiceAutomationGrid", "Invoices");
                                    }
                                    else
                                    {
                                        return RedirectToAction("InvoiceAutomationGrid", "Invoices");
                                    }
                                }
                            }
                            else
                            {
                                return RedirectToAction("Index", "Login", new { area = "" });
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error("InvoicesController - InvoiceAutomationEdit - " + DateTime.Now + " - " + ex.Message.ToString());
                        }
                        if (Roles.IsUserInRole("ApproveInvoice") && roleFlg == false && invoice.btnName == "Save & New")
                        {
                            StatusMessage = "Success2";
                            ViewBag.StatusMessage = StatusMessage;
                            return RedirectToAction("InvoiceAutomationEdit");
                        }
                        else if (invoice.btnName == "Save & New")
                        {
                            StatusMessage = "Success1";
                            ViewBag.StatusMessage = StatusMessage;
                            return RedirectToAction("InvoiceAutomationEdit");
                        }
                        else
                        {
                            StatusMessage = "Success1";
                            ViewBag.StatusMessage = StatusMessage;
                            if (bulk == 1)
                            {
                                StatusMessage = "";
                                return RedirectToAction("InvoiceAutomationGrid", "Invoices");
                            }
                            else
                            {
                                return RedirectToAction("InvoiceAutomationGrid");
                            }
                        }
                    }
                    else
                    {
                        invoice.DepartmentMasters = _invoiceRepository.getDepartment_WithSP(invoice.StoreId).ToList();
                        var DepartmentList = _invoiceRepository.getDepartment_WithSP(invoice.StoreId).ToList();
                        ViewBag.Disc_Dept_id = new SelectList(DepartmentList.Where(s => s.StoreId == invoice.StoreId).ToList(), "DepartmentId", "DepartmentName", invoice.Disc_Dept_id);

                        ViewBag.DiscountTypeId = new SelectList(_MastersBindRepository.GetDiscountTypeMaster(), "DiscountTypeId", "DiscountType", invoice.DiscountTypeId);
                        ViewBag.InvoiceTypeId = new SelectList(_MastersBindRepository.GetInvoiceTypeMaster(), "InvoiceTypeId", "InvoiceType", invoice.InvoiceTypeId);
                        ViewBag.PaymentTypeId = (Convert.ToString(Session["storeid"]) != "0" ? new SelectList(_MastersBindRepository.GetPaymentTypeMaster(), "PaymentTypeId", "PaymentType", invoice.PaymentTypeId).Skip(0) : new SelectList(_MastersBindRepository.GetPaymentTypeMaster(), "PaymentTypeId", "PaymentType", invoice.PaymentTypeId));

                        ViewBag.StoreId = new SelectList(_CommonRepository.GetStoreList(1), "StoreId", "NickName", invoice.StoreId);
                        ViewBag.VendorId = new SelectList(_MastersBindRepository.GetVendorMaster(invoice.StoreId).Select(s => new { s.VendorId, s.VendorName }).ToList(), "VendorId", "VendorName", invoice.VendorId);

                        return View("InvoiceAutomationEdit", invoice);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - InvoiceAutomationEdit - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return RedirectToAction("InvoiceAutomationGrid");
        }

        public int InvoiceAutomationAttachNote(InvoiceAutomation invoice, HttpPostedFileBase UploadInvoice)
        {
            int returnFlg = 0;
            #region AttachNote
            var Sacn_Title = "";
            if (UploadInvoice != null)
            {
                try
                {
                    if (UploadInvoice.ContentLength > 0)
                    {
                        var allowedExtensions = new[] { ".pdf" };
                        var extension = Path.GetExtension(UploadInvoice.FileName);
                        var Ext = Convert.ToString(extension).ToLower();
                        if (!allowedExtensions.Contains(Ext))
                        {
                            ViewBag.StatusMessage = "InvalidImage";
                            invoice.DepartmentMasters = _invoiceRepository.getDepartment_WithSP(invoice.StoreId).ToList();
                            //This class is Get All department masters.
                            ViewBag.Disc_Dept_id = new SelectList(_MastersBindRepository.GetAllDepartmentMasters().OrderBy(o => o.DepartmentName).ToList(), "DepartmentId", "DepartmentName", invoice.Disc_Dept_id);
                            //This class is Get Discount Type Master.
                            ViewBag.DiscountTypeId = new SelectList(_MastersBindRepository.GetDiscountTypeMaster(), "DiscountTypeId", "DiscountType", invoice.DiscountTypeId);
                            //This class is Get invoice Type master.
                            ViewBag.InvoiceTypeId = new SelectList(_MastersBindRepository.GetInvoiceTypeMaster(), "InvoiceTypeId", "InvoiceType", invoice.InvoiceTypeId);
                            ViewBag.PaymentTypeId = (Convert.ToString(Session["storeid"]) != "0" ? new SelectList(_MastersBindRepository.GetPaymentTypeMaster(), "PaymentTypeId", "PaymentType", invoice.PaymentTypeId).Skip(0) : new SelectList(_MastersBindRepository.GetPaymentTypeMaster(), "PaymentTypeId", "PaymentType", invoice.PaymentTypeId));
                            //new SelectList(db.PaymentTypeMasters, "PaymentTypeId", "PaymentType", invoice.PaymentTypeId);
                            ViewBag.StoreId = new SelectList(_CommonRepository.GetStoreList(1), "StoreId", "NickName", invoice.StoreId);
                            ViewBag.VendorId = new SelectList(_MastersBindRepository.GetVendorMaster(invoice.StoreId).Select(s => new { s.VendorId, s.VendorName }).ToList(), "VendorId", "VendorName", invoice.VendorId);
                            returnFlg = 1;
                            return returnFlg;
                        }
                        else
                        {
                            if (UploadInvoice.ContentLength > 50971520)
                            {
                                returnFlg = 2;
                                return returnFlg;
                            }
                            else
                            {
                                try
                                {
                                    //Sacn_Title = AdminSiteConfiguration.GetRandomNo() + Path.GetFileName(UploadInvoice.FileName);
                                    //Sacn_Title = AdminSiteConfiguration.RemoveSpecialCharacter(Sacn_Title);
                                    //string path1 = Request.PhysicalApplicationPath + "UserFiles\\Invoices" + "\\" + Sacn_Title;
                                    string FileName = invoice.InvoiceDate.ToString("MMddyyyy") + "-" + _MastersBindRepository.GetVendorRemoveSpecialCarecters(invoice.VendorId) + "-" + AdminSiteConfiguration.RemoveSpecialCharacterInvoice(invoice.InvoiceNumber) + "-" + (invoice.InvoiceTypeId == 1 ? "INV" : "CR") + "-" + DateTime.Now.Ticks.ToString() + ".pdf";
                                    //(invoice.InvoiceTypeId == 1 ? "INV" : "CR") + "-" + AdminSiteConfiguration.RemoveSpecialCharacterInvoice(Inoicno) + "-" + invoice.InvoiceDate.ToString("MMddyyyy") +"-"+ GetVendorName(invoice.VendorId) +".pdf";
                                    string PathRel = CreateDirectory(invoice.InvoiceDate.ToString("MMddyyyy"), invoice.VendorId, invoice.StoreId);
                                    string FullPath = PathRel + "\\" + FileName;
                                    UploadInvoice.SaveAs(FullPath);
                                    var InvoiceNM = FullPath.Replace(Request.PhysicalApplicationPath + "UserFiles\\Invoices\\", "");
                                    invoice.UploadInvoice = InvoiceNM;

                                    if (!String.IsNullOrEmpty(Convert.ToString(invoice.UploadPdfAutomationId)))
                                    {
                                        //This db class is upload pdf by Id.
                                        _invoiceRepository.UploadAutomationPdfId(invoice.UploadPdfAutomationId);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logger.Error("InvoicesController - InvoiceAttachNote - " + DateTime.Now + " - " + ex.Message.ToString());
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    logger.Error("InvoicesController - InvoiceAttachNote - " + DateTime.Now + " - " + ex.Message.ToString());
                }

            }
            return returnFlg;
            #endregion

        }

        public ActionResult UpdateInvoiceUserReview(int invoiceid, string reviewnote)
        {
            string message = "";
            try
            {
                _invoiceRepository.UpdateInvoiceReview(invoiceid, reviewnote);

                message = "Success";
            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - UpdateInvoiceUserReview - " + DateTime.Now + " - " + ex.Message.ToString());
                message = "Error";
            }
            return Json(new { success = message });
        }

        public ActionResult UpdateInvoiceUserTasks(int invoiceid, string invoicetask, string invoiceno, int priorityid, int assignid, DateTime duedatetask)
        {
            string message = "";
            try
            {
                int createdby = UserModule.getUserId();
                _invoiceRepository.UpdateInvoiceTasks(invoiceid, invoicetask, invoiceno, createdby, priorityid, assignid, duedatetask);

                message = "Success";
            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - UpdateInvoiceUserTasks - " + DateTime.Now + " - " + ex.Message.ToString());
                message = "Error";
            }
            return Json(new { success = message });
        }

        [HttpPost]
        public ActionResult SaveModeToSession(string mode)
        {
            if (mode == "")
            {
                Session["selectedMode"] = "Automatic";
            }
            else
            {
                Session["selectedMode"] = mode;
            }
            return Json(new { success = true });
        }


        //public void InvoiceGridStorePersistData(string persistData)
        //{
        //    int Storeidval = Convert.ToInt32(Session["storeid"].ToString());
        //    Session["SelectedPersistData"] = persistData;
        //}


        //public string InvoiceGridRestore()
        //{
        //    var gridState = Session["SelectedPersistData"];
        //    return gridState == null ? "" : gridState.ToString();
        //}

        [HttpPost]
        public JsonResult StoreIdFromSession(string storeid)
        {
            // You now have the storeid passed from the client-side (sessionStorage)
            // You can use it as needed, for example:
            Session["storeid"] = storeid;  // Optionally store it in the session for future requests

            return Json(new { success = true, storeid = storeid });
        }

        public JsonResult GetInvoiceFilterdData(string columnName)
        {
            List<InvoiceFilter> InvoiceFilter = new List<InvoiceFilter>();
            try
            {
                logger.Info("InvoicesController - GetInvoiceFilterdData - " + DateTime.Now);
                InvoiceFilter = _invoiceRepository.GetInvoiceFilteredRecords();
            }
            catch (Exception ex)
            {
                logger.Error("InvoicesController - GetInvoiceFilterdData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(InvoiceFilter, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetQbPaidStatusDetails(int invoiceid)
        {
            List<InvoicePaymentStatusDetailsList> obj = new List<InvoicePaymentStatusDetailsList>();
            try
            {
                obj = _invoiceRepository.InvoicePaymentStatus(invoiceid);
                StatusMessage = "Success";
            }
            catch (Exception ex)
            {
                StatusMessage = "Error";
                logger.Error("ExpenseAccountsController - GetQbPaidStatusDetails - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = obj, statusmessage = StatusMessage });
        }
    }
}