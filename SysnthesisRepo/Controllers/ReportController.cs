using DocumentFormat.OpenXml.EMMA;
using EntityModels.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using NLog;
using Repository;
using Repository.IRepository;
using Syncfusion.EJ2.Base;
using SynthesisQBOnline.BAL;
using SynthesisQBOnline.QBClass;
using SynthesisViewModal;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utility;

namespace SysnthesisRepo.Controllers
{
    public class ReportController : Controller
    {

        private readonly IReportRepository _reportRepository;
        private readonly ISynthesisApiRepository _synthesisApiRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly ICompaniesRepository _CompaniesRepository;
        private readonly IUserActivityLogRepository _activityLogRepository;
        private readonly IQBRepository _qBRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        ReportViewModel viewModel = new ReportViewModel();
        ReportModel model;
        public ReportController()
        {

            this._reportRepository = new ReportRepository(new DBContext());
            this._synthesisApiRepository = new SynthesisApiRepository();
            this._commonRepository = new CommonRepository(new DBContext());
            this._CompaniesRepository = new CompaniesRepository(new DBContext());
            this._activityLogRepository = new UserActivityLogRepository(new DBContext());
            this._qBRepository = new QBRepository(new DBContext());
            //if (Session["storeid"] == null)
            //{
            //    Session["storeid"] = "0";
            //}
            //if (!string.IsNullOrEmpty(Session["storeid"].ToString()))
            //{
            //    viewModel.sStoreID = Session["storeid"].ToString();
            //}
        }

        /// <summary>
        /// This method is Operating Ration report data
        /// </summary>
        /// <param name="dashbordsuccess"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,ViewOperatingRatioReport")]
        public ActionResult OperatingRatioReport(string dashbordsuccess)
        {
            if (Session["storeid"] == null)
            {
                Session["storeid"] = "0";
            }
            if (!string.IsNullOrEmpty(Session["storeid"].ToString()))
            {
                viewModel.sStoreID = Session["storeid"].ToString();
            }

            ViewBag.Title = "Operating Ratios Report - Synthesis";
            _commonRepository.LogEntries();     //Harsh's code
            ViewBag.Storeidvalue = viewModel.sStoreID;
            OperatingRatioReport obj = new OperatingRatioReport();
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            //Using this class Get Opertaing Ratio Report.
            _reportRepository.operatingRatioReport(ref obj, UserName);
            return View(obj);
        }

        /// <summary>
        /// This method is get Ration List
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="ShiftId"></param>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult getRatioList(string FromDate, string ToDate, int? ShiftId, int? StoreId)
        {
            if (Session["storeid"] == null)
            {
                Session["storeid"] = "0";
            }
            if (!string.IsNullOrEmpty(Session["storeid"].ToString()))
            {
                viewModel.sStoreID = Session["storeid"].ToString();
                ViewBag.storeid = viewModel.sStoreID;
            }

            List<OperatingRatioList> List = new List<OperatingRatioList>();
            try
            {
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                if (ShiftId == null)
                {
                    ShiftId = 0;
                }

                model = new ReportModel();
                model.FromDate = FromDate;
                model.ToDate = ToDate;
                model.ShiftId = ShiftId;
                model.StoreId = StoreId;
                if (StoreId == null)
                {
                    model.StoreId = Convert.ToInt32(viewModel.sStoreID);
                    //Get Terminal Trace Data.
                    List = _reportRepository.GetTerminalTracedata(model, viewModel, UserName);
                }
                else if (StoreId == 0)
                {
                    //Get Terminal Trace Data.
                    List = _reportRepository.GetTerminalTracedata(model, viewModel, UserName);
                    ViewBag.startindex = "1";
                    ViewBag.endIndex = List.Count();
                    ViewBag.TotalDataCount = List.Count();
                    return PartialView("_OperatingRatioListAll", List);
                }
                else
                {
                    //Get Terminal Trace Data.
                    List = _reportRepository.GetTerminalTracedata(model, viewModel, UserName);
                }
                ViewBag.startindex = "1";
                ViewBag.endIndex = List.Count();
                ViewBag.TotalDataCount = List.Count();
            }
            catch (Exception ex)
            {
                logger.Error("ReportController - getRatioList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView("_OperatingRatioList", List);
        }

        /// <summary>
        /// This method Get Store Wise Data.
        /// </summary>
        /// <param name="Department"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="ShiftId"></param>
        /// <returns></returns>
        public ActionResult getStoreWiseData(string Department, string FromDate, string ToDate, int? ShiftId)
        {
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            List<OperatingRatioList> Lst = new List<OperatingRatioList>();
            try
            {
                model = new ReportModel();
                model.FromDate = FromDate;
                model.ToDate = ToDate;
                model.ShiftId = ShiftId;
                model.Department = Department;
                if (Department == "SUPPLIES" || Department == "PAYROLL FEE" || Department == "TAX BEER" || Department == "TAX WINE & LIQUOR" || Department == "EXPENSES")
                {
                    model.CheckFlag = Department;
                }
                //Get Terminal Trace Data with departmen.
                Lst = _reportRepository.GetTerminalTracedatadepartmentwise(model, viewModel, UserName);
            }
            catch (Exception ex)
            {
                logger.Error("ReportController - getStoreWiseData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(Lst, JsonRequestBehavior.AllowGet);
        }

        #region 
        [HttpPost]
        public ActionResult getStoreWiseDataForAllDep(List<string> Department, string FromDate, string ToDate, int? ShiftId)
        {
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            List<OperatingRatioList> Lst = new List<OperatingRatioList>();
            List<OperatingRatioList> List = new List<OperatingRatioList>();
            try
            {
                model = new ReportModel();
                model.FromDate = FromDate;
                model.ToDate = ToDate;
                model.ShiftId = ShiftId;

                foreach (var Depname in Department)
                {
                    model.Department = Depname;
                    if (Depname == "SUPPLIES" || Depname == "PAYROLL FEE" || Depname == "TAX BEER" || Depname == "TAX WINE & LIQUOR")
                    {
                        model.CheckFlag = Depname;
                    }
                    Lst = _reportRepository.GetTerminalTracedatadepartmentwise(model, viewModel, UserName);
                    Lst = Lst.Select(x => { x.MasterDepartment = Depname; return x; }).ToList();
                    //foreach (var data in Lst)
                    //{
                    //    data.MasterDepartment = Depname;
                    //}
                    List.AddRange(Lst);
                }
            }
            catch (Exception ex)
            {
                logger.Error("ReportController - getStoreWiseData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(List, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public ActionResult getstorewiseExpenseDetail(string FromDate, string ToDate, int? StoreId, int? ShiftId)
        {
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            model = new ReportModel();
            model.FromDate = FromDate;
            model.ToDate = ToDate;
            model.StoreId = StoreId;
            model.ShiftId = ShiftId;
            //List<OperatingRatioExpenseDepartment> Lst = new List<OperatingRatioExpenseDepartment>();
            List<OperatingRatioList> lists = new List<OperatingRatioList>();
            try
            {
                //This class Get Storewise Expenses Details
                lists = _reportRepository.Get_Expense_Detail_Storewise(model, viewModel, UserName);
            }
            catch (Exception ex)
            {
                logger.Error("ReportController - getstorewiseExpenseDetail - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(lists, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This Method return Sales Summary Report. 
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,ViewSalesSummaryReport")]
        public ActionResult SalesSummaryReport()
        {
            if (Session["storeid"] == null)
            {
                Session["storeid"] = "0";
            }
            if (!string.IsNullOrEmpty(Session["storeid"].ToString()))
            {
                viewModel.sStoreID = Session["storeid"].ToString();
            }

            ViewBag.Title = "Sales Summary Report - Synthesis";
            _commonRepository.LogEntries();     //Harsh's code
            ViewBag.storeid = viewModel.sStoreID;
            try
            {
                //Get sales Sammaries Start date
                ViewBag.LatestDate = AdminSiteConfiguration.GetDate1(_reportRepository.GetSalesSammarieStartDate());
            }
            catch (Exception ex)
            {
                logger.Error("ReportController - SalesSummaryReport - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            SalesSummaryReport obj = new SalesSummaryReport();
            return View(obj);
        }

        /// <summary>
        /// This method is Get List of Sales Summary Reports
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [Authorize(Roles = "Administrator,ViewSalesSummaryReport")]
        public ActionResult getSalesSummaryReportList(string FromDate, string ToDate)
        {
            ShiftWiseTenderReport_Select shiftwisetenderreport_Select = new ShiftWiseTenderReport_Select();
            try
            {
                model = new ReportModel();
                model.StoreId = 0;
                if (Convert.ToInt32(Session["storeid"]) != 0)
                {
                    model.StoreId = Convert.ToInt32(Session["storeid"]);
                }

                if (!string.IsNullOrEmpty(FromDate))
                {
                    model.Sdate = Convert.ToDateTime(FromDate);
                }
                else
                {
                    try
                    {
                        //Get sales Sammaries Start date
                        model.Sdate = Convert.ToDateTime(AdminSiteConfiguration.GetDate1(_reportRepository.GetSalesSammarieStartDate()));
                        FromDate = AdminSiteConfiguration.GetDate1(_reportRepository.GetSalesSammarieStartDate());
                    }
                    catch
                    {
                        model.Sdate = AdminSiteConfiguration.GetEasternTime(DateTime.Now);
                        FromDate = AdminSiteConfiguration.GetDate1(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToShortDateString());
                    }

                }
                if (!string.IsNullOrEmpty(ToDate))
                {
                    model.Enddate = Convert.ToDateTime(ToDate);
                }
                else
                {
                    try
                    {
                        //Get sales Sammaries Start date
                        model.Enddate = Convert.ToDateTime(AdminSiteConfiguration.GetDate1(_reportRepository.GetSalesSammarieStartDate()));
                    }
                    catch
                    {
                        model.Enddate = AdminSiteConfiguration.GetEasternTime(DateTime.Now);
                    }
                }
                ViewBag.storeid = model.StoreId;
                ViewBag.StartDate = FromDate;
                ViewBag.EndDate = ToDate;

                clsActivityLog clsActivityLog = new clsActivityLog();
                clsActivityLog.Action = "Click";
                clsActivityLog.ModuleName = "Report";
                clsActivityLog.PageName = "Sales Summary Report";
                clsActivityLog.Message = " Sales Summary Report Generated for From Date : " + FromDate + " - To Date : " + ToDate + " ";
                clsActivityLog.CreatedBy = Convert.ToInt32(Session["UserID"]);
                _synthesisApiRepository.CreateLog(clsActivityLog);
                //This class is select Shift wise Tender Report
                _reportRepository.ShiftWiseTenderReport_Select(ref shiftwisetenderreport_Select, ref viewModel, model, FromDate, ToDate);
                ViewBag.TitleCount = shiftwisetenderreport_Select.TitleWiseTotal.Count;
                ViewBag.ErrorMessage = viewModel.ErrorMessage;
                viewModel.ErrorMessage = "";
                ViewBag.DepositeCount = shiftwisetenderreport_Select.OtherDepositeList.Count;
            }
            catch (Exception ex)
            {

                logger.Error("ReportController - getSalesSummaryReportList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView("_SalesSummaryList", shiftwisetenderreport_Select);
        }

        /// <summary>
        /// This method is Export Excel Report data
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="Shift"></param>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,ExportOperatingRatioReport")]
        public ActionResult ExportExcelData(string StartDate, string EndDate, string Shift, int StoreId, string value = "", string selectedvalue = null)
        {
            try
            {
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                DataTable dt1 = new DataTable();
                List<OperatingRatioList> List = new List<OperatingRatioList>();
                List<OperatingRatioList> Lst = new List<OperatingRatioList>();
                string[] SelectedvalueArray = null;
                model = new ReportModel();
                model.FromDate = StartDate;
                model.ToDate = EndDate;
                if (String.IsNullOrEmpty(Shift))
                {
                    model.ShiftId = null;
                }
                else
                {
                    model.ShiftId = Convert.ToInt32(Shift);
                }
                model.StoreId = StoreId;
                //Get Terminal Trace Data.
                List = _reportRepository.GetTerminalTracedata(model, viewModel, UserName);

                #region Code by Harsh 13-03-24, Updated - 19-03-24

                decimal Sales = List.Sum(item => item.Sales.GetValueOrDefault());
                decimal COgs = List.Where(item => item.Department != "SUPPLIES").Sum(item => item.COgs.GetValueOrDefault());
                decimal TotalSalPercentage = List.Sum(item => item.TotalSalPercentage.GetValueOrDefault());
                decimal PayrollAmount = List.Sum(item => item.PDFAmount.GetValueOrDefault());

                if (!string.IsNullOrEmpty(selectedvalue))
                {
                    SelectedvalueArray = selectedvalue.Split(',');
                    List = List.Where(item => SelectedvalueArray.Contains(item.Department)).ToList();
                }
                if (List.Count > 0)
                {
                    if (SelectedvalueArray == null)
                    {
                        OperatingRatioList total = new OperatingRatioList
                        {
                            Department = "Total",
                            Sales = Sales,
                            TotalSalPercentage = TotalSalPercentage,
                            COgs = COgs,
                            SalPercentage = Sales != 0 ? Math.Round(((Convert.ToDecimal(COgs.ToString()) / Convert.ToDecimal(Sales.ToString())) * 100), 2) : 0,
                            PDFAmount = PayrollAmount,
                            PDFPercentage = Sales != 0 ? Math.Round(((Convert.ToDecimal(PayrollAmount.ToString()) / Convert.ToDecimal(Sales.ToString())) * 100), 2) : 0,
                            PayrollDescription = ""
                        };
                        List.Add(total);
                    }
                }
                if (StoreId == 0)
                {
                    if (value == "")
                    {
                        for (int i = 0; i < List.Count; i++)
                        {
                            model.Department = List[i].Department;
                            if (List[i].Department == "SUPPLIES" || List[i].Department == "PAYROLL FEE" || List[i].Department == "TAX BEER" || List[i].Department == "TAX WINE & LIQUOR")
                            {
                                model.CheckFlag = List[i].Department;
                            }
                            else
                            {
                                model.CheckFlag = "";
                            }
                            Lst = _reportRepository.GetTerminalTracedatadepartmentwise(model, viewModel, UserName);

                            List.InsertRange(i + 1, Lst);
                            i += Lst.Count;
                        }
                    }
                    else
                    {
                        model.Department = value;
                        if (value == "SUPPLIES" || value == "PAYROLL FEE" || value == "TAX BEER" || value == "TAX WINE & LIQUOR")
                        {
                            model.CheckFlag = value;
                        }
                        else
                        {
                            model.CheckFlag = "";
                        }
                        Lst = _reportRepository.GetTerminalTracedatadepartmentwise(model, viewModel, UserName);
                        int index = List.FindIndex(item => item.Department == value);
                        if (index != -1)
                        {
                            List.InsertRange(index + 1, Lst);
                        }
                    }
                }
                else
                {
                    if (value == "-")
                    {
                        Lst = _reportRepository.Get_Expense_Detail_Storewise(model, viewModel, UserName);
                        int index = List.FindIndex(item => item.Department == "EXPENSES");
                        if (index != -1)
                        {
                            List.InsertRange(index + 1, Lst);
                        }
                    }
                }
                #endregion

                dt1 = Common.LINQToDataTable(List);
                dt1.Columns["PDFAmount"].ColumnName = "PayrollAmount";
                dt1.Columns["PDFPercentage"].ColumnName = "SalesPercentage";
                Export oExport = new Export();
                string FileName = "OperationRatioReport" + ".xls";

                int[] ColList = { 0, 1, 2, 3, 4, 5, 6 };
                string[] arrHeader =
                {
                     "Department",
                     "Sales",
                     "% of Total Sales",
                     "COGS",
                     "COGS Ratio %",
                     "Payroll Amount",
                     "Payroll Ratio %"
                };

                //only change file extension to .xls for excel file
                string sFilterBy = "";
                if (StartDate != "")
                {
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Start Date: " + StartDate;
                }
                if (EndDate != "")
                {
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "End Date: " + EndDate;
                }
                if (Shift != "")
                {
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Shift: " + Shift;
                }
                if (StoreId != 0)
                {
                    //Get Store Nike Name using storeID
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Store: " + _commonRepository.GetStoreNikeName(StoreId);
                }

                DataTable dtCloned = dt1.Clone();
                dtCloned.Columns[1].DataType = typeof(string);
                foreach (DataRow row in dt1.Rows)
                {
                    dtCloned.ImportRow(row);
                }
                for (int i = 0; i <= dtCloned.Rows.Count - 1; i++)
                {
                    if (dtCloned.Rows[i][1].ToString() != "")
                    {
                        dtCloned.Rows[i][1] = Convert.ToDecimal(dtCloned.Rows[i][1].ToString()).ToString("N");
                    }
                }

                oExport.ExportDetails(dtCloned, ColList, arrHeader, Export.ExportFormat.Excel, FileName, sFilterBy, "Operating Ratio Report");
            }
            catch (Exception ex)
            {
                logger.Error("ReportController - ExportExcelData - " + DateTime.Now + " - " + ex.Message.ToString());
                AdminSiteConfiguration.WriteErrorLogs("Controller : Report Method : ExportExcelData Message:" + ex.Message + "Internal Message:" + ex.InnerException);
            }
            return RedirectToAction("OperatingRatioReport", "Report");
        }

        /// <summary>
        /// This method is Export Pdf Report data
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="Shift"></param>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,ExportOperatingRatioReport")]
        public ActionResult ExportPDFData(string StartDate, string EndDate, string Shift, int StoreId, string value = "", string selectedvalue = null)
        {
            try
            {
                List<string> FlagAll = new List<string>();
                string[] SelectedvalueArray = null;
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                DataTable dt1 = new DataTable();
                DataTable dt2 = new DataTable();
                List<OperatingRatioList> List = new List<OperatingRatioList>();
                List<OperatingRatioListPDF> List2 = new List<OperatingRatioListPDF>();
                List<OperatingRatioList> Lst = new List<OperatingRatioList>();
                model = new ReportModel();
                model.FromDate = StartDate;
                model.ToDate = EndDate;
                if (String.IsNullOrEmpty(Shift))
                {
                    model.ShiftId = null;
                }
                else
                {
                    model.ShiftId = Convert.ToInt32(Shift);
                }
                model.StoreId = StoreId;
                //Get Terminal Trace Data.
                List = _reportRepository.GetTerminalTracedata(model, viewModel, UserName);

                #region Code by Harsh 13-03-24, Updated - 19-03-24

                decimal Sales = List.Sum(item => item.Sales.GetValueOrDefault());
                decimal COgs = List.Where(item => item.Department != "SUPPLIES").Sum(item => item.COgs.GetValueOrDefault());
                decimal TotalSalPercentage = List.Sum(item => item.TotalSalPercentage.GetValueOrDefault());
                decimal PayrollAmount = List.Sum(item => item.PDFAmount.GetValueOrDefault());

                if (!string.IsNullOrEmpty(selectedvalue))
                {
                    SelectedvalueArray = selectedvalue.Split(',');
                    List = List.Where(item => SelectedvalueArray.Contains(item.Department)).ToList();
                }
                if (List.Count > 0)
                {
                    if (SelectedvalueArray == null)
                    {
                        OperatingRatioList total = new OperatingRatioList
                        {
                            Department = "Total",
                            Sales = Sales,
                            TotalSalPercentage = TotalSalPercentage,
                            COgs = COgs,
                            SalPercentage = Sales != 0 ? Math.Round(((Convert.ToDecimal(COgs.ToString()) / Convert.ToDecimal(Sales.ToString())) * 100), 2) : 0,
                            PDFAmount = PayrollAmount,
                            PDFPercentage = Sales != 0 ? Math.Round(((Convert.ToDecimal(PayrollAmount.ToString()) / Convert.ToDecimal(Sales.ToString())) * 100), 2) : 0,
                            PayrollDescription = ""
                        };
                        List.Add(total);
                    }

                }

                if (StoreId == 0)
                {
                    if (value == "")
                    {
                        for (int i = 0; i < List.Count; i++)
                        {
                            FlagAll.Add(List[i].Department);
                            model.Department = List[i].Department;
                            if (List[i].Department == "SUPPLIES" || List[i].Department == "PAYROLL FEE" || List[i].Department == "TAX BEER" || List[i].Department == "TAX WINE & LIQUOR")
                            {
                                model.CheckFlag = List[i].Department;
                            }
                            else
                            {
                                model.CheckFlag = "";
                            }
                            Lst = _reportRepository.GetTerminalTracedatadepartmentwise(model, viewModel, UserName);

                            List.InsertRange(i + 1, Lst);
                            i += Lst.Count;
                        }
                    }
                    else
                    {
                        FlagAll.Add(value);
                        model.Department = value;
                        if (value == "SUPPLIES" || value == "PAYROLL FEE" || value == "TAX BEER" || value == "TAX WINE & LIQUOR")
                        {
                            model.CheckFlag = value;
                        }
                        else
                        {
                            model.CheckFlag = "";
                        }
                        Lst = _reportRepository.GetTerminalTracedatadepartmentwise(model, viewModel, UserName);
                        int index = List.FindIndex(item => item.Department == value);
                        if (index != -1)
                        {
                            List.InsertRange(index + 1, Lst);
                        }
                    }
                }
                else
                {
                    if (value == "-")
                    {
                        Lst = _reportRepository.Get_Expense_Detail_Storewise(model, viewModel, UserName);
                        int index = List.FindIndex(item => item.Department == "EXPENSES");
                        if (index != -1)
                        {
                            FlagAll.Add("EXPENSES");
                            List.InsertRange(index + 1, Lst);
                        }
                    }
                }
                #endregion
                List2 = List.Select(s => new OperatingRatioListPDF
                {
                    COgs = s.COgs != null ? "$ " + AdminSiteConfiguration.PriceWithComma(s.COgs.ToString()) : null,
                    Department = s.Department,
                    PDFAmount = s.PDFAmount != null ? "$ " + AdminSiteConfiguration.PriceWithComma(s.PDFAmount.ToString()) : null,
                    PDFPercentage = s.PDFPercentage.ToString(),
                    PayrollDescription = s.PayrollDescription,
                    SalPercentage = s.SalPercentage.ToString(),
                    Sales = s.Sales != null ? "$ " + AdminSiteConfiguration.PriceWithComma(s.Sales.ToString()) : null,
                    TotalSalPercentage = s.TotalSalPercentage.ToString()
                }).ToList();

                dt1 = Common.LINQToDataTable(List);
                dt2 = Common.LINQToDataTable(List2);
                dt1.Columns["PDFAmount"].ColumnName = "PayrollAmount";
                dt1.Columns["PDFPercentage"].ColumnName = "SalesPercentage";
                dt2.Columns["PDFAmount"].ColumnName = "PayrollAmount";
                dt2.Columns["PDFPercentage"].ColumnName = "SalesPercentage";
                Export oExport = new Export();

                //only change file extension to .xls for excel file
                string sFilterBy = "";
                if (StartDate != "")
                {
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Start Date: " + StartDate;
                }
                if (EndDate != "")
                {
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "End Date: " + EndDate;
                }
                if (Shift != "")
                {
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Shift: " + Shift;
                }
                if (StoreId != 0)
                {
                    //Get Store Nike Name using storeID
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Store: " + _commonRepository.GetStoreNikeName(StoreId);
                }

                dt2.Columns.RemoveAt(dt2.Columns.Count - 1);
                string[] FlagAllArray = FlagAll.ToArray();
                GeneratePDF(dt2, "OperationRatioReport" + ".pdf", sFilterBy, "Operating Ratio Report", FlagAllArray);
            }
            catch (Exception ex)
            {
                logger.Error("ReportController - ExportPDFData - " + DateTime.Now + " - " + ex.Message.ToString());
                AdminSiteConfiguration.WriteErrorLogs("Controller : Report Method : ExportExcelData Message:" + ex.Message + "Internal Message:" + ex.InnerException);
            }
            return RedirectToAction("OperatingRatioReport", "Report");
        }
        /// <summary>
        /// This method is Generate Daily PDF
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="Name"></param>
        /// <param name="sFilterBy"></param>
        /// <param name="Heading"></param>
        private void GenerateDailyPDF(DataTable dataTable, string Name, string sFilterBy = "", string Heading = "Operating Ratio Report")
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
                    if (dataTable.Columns.Count == 3)
                    {
                        _mytable.SetWidths(new float[] { 8.5f, 6f, 6f });
                    }
                    else
                    {
                        _mytable.SetWidths(new float[] { 8.5f, 6f, 6f, 5.5f, 5.5f, 5f, 4f, 4f });
                    }

                    PdfPTable tbl = new PdfPTable(dataTable.Columns.Count);
                    tbl.WidthPercentage = 100;
                    if (dataTable.Columns.Count == 3)
                    {
                        tbl.SetWidths(new float[] { 8.5f, 6f, 6f });
                    }
                    else
                    {
                        tbl.SetWidths(new float[] { 8.5f, 6f, 6f, 5.5f, 5.5f, 5f, 4f, 4f });
                    }

                    Phrase phrase1 = new Phrase();
                    phrase1.Add(new Chunk(Heading, FontFactory.GetFont("Tahoma", 20, Font.BOLD, iTextSharp.text.BaseColor.RED)));
                    PdfPCell cell1 = new PdfPCell(phrase1);
                    cell1.HorizontalAlignment = Element.ALIGN_CENTER;

                    tbl.SpacingAfter = 10f;
                    cell1.Colspan = 8;
                    cell1.Border = 0;
                    tbl.AddCell(cell1);

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

                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        iTextSharp.text.Font small = FontFactory.GetFont("Tahoma", BaseFont.IDENTITY_H, 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
                        Phrase p = new Phrase(dataTable.Columns[i].ColumnName.Replace("TotalSalPercentage", "% of Total Sales").Replace("COgs", "COGS").Replace("SalPercentage", "COGS Ratio %").Replace("PayrollAmount", "Payroll Amount").Replace("SalesPercentage", "Payroll Ratio %"), small);
                        PdfPCell cell = new PdfPCell(p);

                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
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
                            _mytable.AddCell(cell);
                        }
                    }

                    pdfDoc.Add(tbl);
                    pdfDoc.Add(_mytable);
                    pdfDoc.Close();
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + Name);
                    Response.Clear();
                    Response.BinaryWrite(mStream.ToArray());
                    Response.End();
                }
            }
            catch (Exception ex)
            {

                logger.Error("ReportController - GenerateDailyPDF - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        /// <summary>
        /// This method is Generate Pdf
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="Name"></param>
        /// <param name="sFilterBy"></param>
        /// <param name="Heading"></param>
        private void GeneratePDF(DataTable dataTable, string Name, string sFilterBy = "", string Heading = "Operating Ratio Report", string[] FlagAll = null)
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
                    if (dataTable.Columns.Count == 3)
                    {
                        _mytable.SetWidths(new float[] { 8.5f, 6f, 6f });
                    }
                    else
                    {
                        _mytable.SetWidths(new float[] { 8.5f, 6f, 6f, 5.5f, 5.5f, 5f, 4f });
                    }

                    PdfPTable tbl = new PdfPTable(dataTable.Columns.Count);
                    tbl.WidthPercentage = 100;
                    if (dataTable.Columns.Count == 3)
                    {
                        tbl.SetWidths(new float[] { 8.5f, 6f, 6f });
                    }
                    else
                    {
                        tbl.SetWidths(new float[] { 8.5f, 6f, 6f, 5.5f, 5.5f, 5f, 4f });
                    }

                    Phrase phrase1 = new Phrase();
                    phrase1.Add(new Chunk(Heading, FontFactory.GetFont("Tahoma", 20, Font.BOLD, iTextSharp.text.BaseColor.RED)));
                    PdfPCell cell1 = new PdfPCell(phrase1);
                    cell1.HorizontalAlignment = Element.ALIGN_CENTER;

                    tbl.SpacingAfter = 10f;
                    cell1.Colspan = 8;
                    cell1.Border = 0;
                    tbl.AddCell(cell1);

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

                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        iTextSharp.text.Font small = FontFactory.GetFont("Tahoma", BaseFont.IDENTITY_H, 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
                        Phrase p = new Phrase(dataTable.Columns[i].ColumnName.Replace("TotalSalPercentage", "% of Total Sales").Replace("COgs", "COGS").Replace("SalPercentage", "COGS Ratio %").Replace("PayrollAmount", "Payroll Amount").Replace("SalesPercentage", "Payroll Ratio %"), small);
                        PdfPCell cell = new PdfPCell(p);

                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _mytable.AddCell(cell);
                    }
                    //creating table data (actual result)   

                    for (int k = 0; k < dataTable.Rows.Count; k++)
                    {
                        for (int j = 0; j < dataTable.Columns.Count; j++)
                        {
                            Phrase p = new Phrase(dataTable.Rows[k][j].ToString(), myfont);
                            PdfPCell cell = new PdfPCell(p);
                            string MasterData = dataTable.Rows[k][j].ToString();
                            if (FlagAll != null)
                            {
                                foreach (var dep in FlagAll)
                                {
                                    if (MasterData == dep)
                                    {
                                        cell.BackgroundColor = new iTextSharp.text.BaseColor(200, 200, 200);
                                    }
                                }
                            }
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            _mytable.AddCell(cell);
                        }
                    }

                    pdfDoc.Add(tbl);
                    pdfDoc.Add(_mytable);
                    pdfDoc.Close();
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + Name);
                    Response.Clear();
                    Response.BinaryWrite(mStream.ToArray());
                    Response.End();
                }
            }
            catch (Exception ex)
            {

                logger.Error("ReportController - GeneratePDF - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }


        #region DailyPosFeeds
        /// <summary>
        /// This method is Get Daily POS Feeds
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,ViewDailyPOSFeeds")]
        public ActionResult DailyPosFeeds()
        {
            try
            {
                ViewBag.Title = "Daily POS Feeds - Synthesis";
                _commonRepository.LogEntries();     //Harsh's code
                viewModel.IsFirst = false;
                string storeid = "";
                if (!string.IsNullOrEmpty(Convert.ToString(Session["storeid"])))
                {
                    storeid = Convert.ToString(Session["storeid"]);
                    ViewBag.storeid = storeid;
                }
                //Get All Terminal Masters data
                ViewBag.DrpLstTerminal = new SelectList(_reportRepository.terminalMasters(), "TerminalId", "TerminalName");
                //This class Get All Store Master data.
                ViewBag.DrpLstStore = new SelectList(_CompaniesRepository.GetAllStoreMasters().Where(s => s.IsActive == true).Select(s => new { s.StoreId, s.Name }), "StoreId", "Name");
                var ShiftList = (from ShiftEnm e in Enum.GetValues(typeof(ShiftEnm))
                                 select new ShiftNewList
                                 {
                                     Id = (int)e,
                                     Name = e.ToString().Replace("_", " ")
                                 }).ToList();
                ViewBag.ShifListcount = ShiftList.Count();

                ViewBag.DrpLstShift = new SelectList(ShiftList.OrderBy(o => o.Name), "Id", "Name", 0);
                ViewBag.SearchTitle = AdminSiteConfiguration.GetDate(Convert.ToString(AdminSiteConfiguration.GetEasternTime(DateTime.Now)));
                ViewBag.SearchTitle1 = AdminSiteConfiguration.GetDate(Convert.ToString(AdminSiteConfiguration.GetEasternTime(DateTime.Now).AddDays(-6)));
            }
            catch (Exception ex)
            {
                logger.Error("ReportController - DailyPosFeeds - " + DateTime.Now + " - " + ex.Message.ToString());
                AdminSiteConfiguration.WriteErrorLogs("Controller : Report Method : DailyPosFeeds Message:" + ex.Message + "Internal Message:" + ex.InnerException);
            }
            return View();
        }

        /// <summary>
        /// This method return Grid of Daily POS feeds
        /// </summary>
        /// <param name="IsBindData"></param>
        /// <param name="currentPageIndex"></param>
        /// <param name="orderby"></param>
        /// <param name="IsAsc"></param>
        /// <param name="PageSize"></param>
        /// <param name="SearchRecords"></param>
        /// <param name="Alpha"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="Storeid"></param>
        /// <param name="Terminalid"></param>
        /// <param name="IsFalse"></param>
        /// <param name="ShiftName"></param>
        /// <returns></returns>
        //public ActionResult DailyPosFeedsGrid(int IsBindData = 1, int currentPageIndex = 1, string orderby = "SalesActivitySummaryId", int IsAsc = 0, int PageSize = 50, int SearchRecords = 1, string Alpha = "", string startdate = "", string enddate = "", int Storeid = 0, int Terminalid = 0, bool IsFalse = false, string ShiftName = "0")
        //{
        //    model = new ReportModel();
        //    try
        //    {
        //        ViewBag.StatusMessage = viewModel.StatusMessage;

        //        #region MyRegion_Array
        //        try
        //        {
        //            if (viewModel.IsFirst == false)
        //            {
        //                viewModel.IsFirst = true;
        //            }
        //            if (viewModel.IsArray == true)
        //            {
        //                foreach (string a1 in viewModel.Arr)
        //                {
        //                    if (a1.Split(':')[0].ToString() == "IsBindData")
        //                    {
        //                        IsBindData = Convert.ToInt32(a1.Split(':')[1].ToString());
        //                    }
        //                    if (a1.Split(':')[0].ToString() == "currentPageIndex")
        //                    {
        //                        currentPageIndex = Convert.ToInt32(a1.Split(':')[1].ToString());
        //                    }
        //                    if (a1.Split(':')[0].ToString() == "orderby")
        //                    {
        //                        orderby = Convert.ToString(a1.Split(':')[1].ToString());
        //                    }
        //                    if (a1.Split(':')[0].ToString() == "IsAsc")
        //                    {
        //                        IsAsc = Convert.ToInt32(a1.Split(':')[1].ToString());
        //                    }
        //                    if (a1.Split(':')[0].ToString() == "PageSize")
        //                    {
        //                        PageSize = Convert.ToInt32(a1.Split(':')[1].ToString());
        //                    }
        //                    if (a1.Split(':')[0].ToString() == "SearchRecords")
        //                    {
        //                        SearchRecords = Convert.ToInt32(a1.Split(':')[1].ToString());
        //                    }
        //                    if (a1.Split(':')[0].ToString() == "Alpha")
        //                    {
        //                        Alpha = Convert.ToString(a1.Split(':')[1].ToString());
        //                    }
        //                    if (a1.Split(':')[0].ToString() == "startdate")
        //                    {
        //                        startdate = Convert.ToString(a1.Split(':')[1].ToString());
        //                    }
        //                    if (a1.Split(':')[0].ToString() == "enddate")
        //                    {
        //                        enddate = Convert.ToString(a1.Split(':')[1].ToString());
        //                    }
        //                    if (a1.Split(':')[0].ToString() == "Storeid")
        //                    {
        //                        Storeid = Convert.ToInt32(a1.Split(':')[1].ToString());
        //                    }
        //                    if (a1.Split(':')[0].ToString() == "Terminalid")
        //                    {
        //                        Terminalid = Convert.ToInt32(a1.Split(':')[1].ToString());
        //                    }

        //                }
        //            }
        //        }
        //        catch { }

        //        viewModel.IsArray = false;
        //        viewModel.Arr = new string[]
        //        {  "IsBindData:" + IsBindData
        //        ,"currentPageIndex:" + currentPageIndex
        //        ,"orderby:" + orderby
        //        ,"IsAsc:" + IsAsc
        //        ,"PageSize:" + PageSize
        //        ,"SearchRecords:" + SearchRecords
        //        ,"Alpha:" + Alpha
        //         ,"startdate:"+startdate
        //        ,"enddate:"+enddate
        //        ,"Storeid:" + Storeid
        //        ,"Terminalid:" + Terminalid
        //        };
        //        #endregion

        //        #region MyRegion_BindData
        //        model.startIndex = ((currentPageIndex - 1) * PageSize) + 1;
        //        model.endIndex = model.startIndex + PageSize - 1;
        //        if (!string.IsNullOrEmpty(Convert.ToString(Session["storeid"])))
        //        {
        //            Storeid = Convert.ToInt32(Session["storeid"]);
        //            ViewBag.storeid = Convert.ToString(Session["storeid"]);
        //        }
        //        if (IsBindData == 1 || viewModel.IsEdit == true)
        //        {
        //            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
        //            //Get All Daily POS Data
        //            viewModel.BindDailyPOSData = _reportRepository.GetData(SearchRecords, startdate, enddate, Storeid, Terminalid, ShiftName, UserName).OfType<DailyPosFeeds>().ToList();
        //            viewModel.TotalDataCount = viewModel.BindDailyPOSData.OfType<DailyPosFeeds>().ToList().Count();
        //        }

        //        if (viewModel.TotalDataCount == 0)
        //        {
        //            viewModel.StatusMessage = "NoItem";
        //        }

        //        ViewBag.IsBindData = IsBindData;
        //        ViewBag.CurrentPageIndex = currentPageIndex;
        //        ViewBag.LastPageIndex = this.getLastPageIndex(PageSize);
        //        ViewBag.OrderByVal = orderby;
        //        ViewBag.IsAscVal = IsAsc;
        //        ViewBag.PageSize = PageSize;
        //        ViewBag.Alpha = Alpha;
        //        ViewBag.SearchRecords = SearchRecords;

        //        ViewBag.startdate = startdate;
        //        ViewBag.enddate = enddate;
        //        ViewBag.StatusMessage = viewModel.StatusMessage;
        //        ViewBag.Insert = viewModel.InsertMessage;
        //        ViewBag.Edit = viewModel.Editmessage;
        //        ViewBag.Delete = viewModel.DeleteMessage;
        //        ViewBag.startindex = model.startIndex;
        //        ViewBag.Storeid = Storeid;
        //        ViewBag.Terminalid = Terminalid;
        //        ViewBag.ShiftName = ShiftName;

        //        TempData["startdate"] = startdate;
        //        TempData["enddate"] = enddate;
        //        TempData["Storeid"] = Storeid;
        //        TempData["Terminalid"] = Terminalid;
        //        TempData["ShiftName"] = ShiftName;

        //        if (viewModel.TotalDataCount < model.endIndex)
        //        {
        //            ViewBag.endIndex = viewModel.TotalDataCount;
        //        }
        //        else
        //        {
        //            ViewBag.endIndex = model.endIndex;
        //        }
        //        ViewBag.TotalDataCount = viewModel.TotalDataCount;
        //        var ColumnName = typeof(DailyPosFeeds).GetProperties().Where(p => p.Name == orderby).FirstOrDefault();


        //        if (IsAsc == 1)
        //        {
        //            ViewBag.AscVal = 0;
        //            model.Data = viewModel.BindDailyPOSData.OfType<DailyPosFeeds>().ToList().OrderBy(n => ColumnName.GetValue(n, null)).Skip(model.startIndex - 1).Take(PageSize);
        //        }
        //        else
        //        {
        //            ViewBag.AscVal = 1;
        //            model.Data = viewModel.BindDailyPOSData.OfType<DailyPosFeeds>().ToList().OrderByDescending(n => ColumnName.GetValue(n, null)).Skip(model.startIndex - 1).Take(PageSize);
        //        }

        //        ViewBag.StatusMessage = viewModel.StatusMessage;
        //        viewModel.StatusMessage = "";
        //        if (startdate != "" || enddate != "" || ShiftName != "")
        //        {
        //            clsActivityLog clsActivityLog = new clsActivityLog();
        //            clsActivityLog.Action = "Click";
        //            clsActivityLog.ModuleName = "Registers";
        //            clsActivityLog.PageName = "Daily POS Feeds Report";
        //            clsActivityLog.Message = " Daily POS Feeds Report Generated for " + (startdate == "" ? "" : "From Date : ") + startdate + (startdate == "" ? "" : " - To Date : ") + enddate + (ShiftName == "" ? "" : " ,Shift Name : ") + ShiftName;
        //            clsActivityLog.CreatedBy = Convert.ToInt32(Session["UserID"]);
        //            _synthesisApiRepository.CreateLog(clsActivityLog);
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        logger.Error("ReportController - DailyPosFeedsGrid - " + DateTime.Now + " - " + ex.Message.ToString());
        //    }

        //    return View(model.Data);
        //    #endregion
        //}

        public ActionResult DailyPosFeedsGrid()
        {
            return View();
        }
        public ActionResult UrlDatasourceDailyPos(DataManagerRequest dm, string startdate = "", string enddate = "", int Terminalid = 0, string ShiftName = "0")
        {
            List<DailyPosFeeds> HrDeVm = new List<DailyPosFeeds>();
            IEnumerable<DailyPosFeeds> DataSource = new List<DailyPosFeeds>();
            int Count = 0;
            try
            {
                logger.Info("ReportController - UrlDatasourceDailyPos - " + DateTime.Now);
                string storeid = "";
                if (!string.IsNullOrEmpty(Convert.ToString(Session["storeid"])))
                {
                    storeid = Convert.ToString(Session["storeid"]);
                }
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;

                DataSource = _reportRepository.GetData(1, startdate, enddate, Convert.ToInt32(storeid), Terminalid, ShiftName, UserName).OfType<DailyPosFeeds>().ToList();
                //DataSource = HrDeVm;

                DataOperations operation = new DataOperations();
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    string search = dm.Search[0].Key.ToString().Trim();
                    decimal searchAmount;
                    decimal.TryParse(search, out searchAmount);
                    DataSource = DataSource.Where(x => x.StoreName.Contains(search) || x.TerminalName.Contains(search) || x.ShiftName.Contains(search) || x.CustomerCount == searchAmount || x.NetSalesWithTax == searchAmount || x.TotalTaxCollected == searchAmount || x.AverageSale == searchAmount).ToList();
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
                Count = DataSource.Cast<DailyPosFeeds>().Count();
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
                logger.Error("ReportController - UrlDatasourceDailyPos - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return dm.RequiresCounts ? Json(new { result = DataSource, count = Count }) : Json(DataSource);
        }

        /// <summary>
        /// This class use to get last page index with pagesize.
        /// </summary>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        private int getLastPageIndex(int PageSize)
        {
            int lastPageIndex = Convert.ToInt32(viewModel.TotalDataCount) / PageSize;
            if (viewModel.TotalDataCount % PageSize > 0)
                lastPageIndex += 1;
            return lastPageIndex;
        }




        /// <summary>
        /// This method is Upload Files.
        /// </summary>
        /// <param name="Storeid"></param>
        /// <param name="DocFiles"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadFiles(int Storeid, HttpPostedFileBase DocFiles)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    foreach (string file in Request.Files)
                    {
                        HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                    }
                    if (!System.IO.Directory.Exists("~/UserFiles/Pending"))
                    {
                        System.IO.Directory.CreateDirectory(Server.MapPath("~/UserFiles/Pending"));
                    }
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        if (file != null)
                        {
                            string FileExtension = Path.GetExtension(file.FileName);
                            Random random = new Random();
                            int randomnumber = random.Next();
                            string FileName = randomnumber + "_" + file.FileName;
                            string UploadPath = "~/UserFiles/Pending/" + FileName;
                            file.SaveAs(Server.MapPath(UploadPath));
                            StorewisePDFUpload uploadPDF = new StorewisePDFUpload();
                            uploadPDF.StoreId = Storeid;
                            uploadPDF.CreatedOn = DateTime.Now;
                            uploadPDF.IsSync = 0;
                            uploadPDF.FileName = FileName;
                            uploadPDF.IsActive = true;
                            uploadPDF.FileType = "";
                            //This class is Insert Store Wise Pdf Upload
                            _reportRepository.InsertStorewisePDFUpload(uploadPDF);
                            uploadPDF = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("ReportController - UploadFiles - " + DateTime.Now + " - " + ex.Message.ToString());
                    AdminSiteConfiguration.WriteErrorLogs("Controller : Report Method : UploadFiles Message:" + ex.Message + "Internal Message:" + ex.InnerException);
                }
            }
            return Json("True", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Download File using filepath.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public ActionResult DownloadFile(string filePath)
        {
            string fullName = Server.MapPath("~/UserFiles/Done/" + filePath);
            byte[] fileBytes = GetFile(fullName);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Pdf);
        }

        /// <summary>
        /// this class is Get File Byte data.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(s);
            return data;
        }

        /// <summary>
        /// This method is Bind Terminal By Store ID
        /// </summary>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        public JsonResult BindTerminalByStoreId(int StoreId = 0)
        {
            List<ddllist> Terminal = new List<ddllist>();
            try
            {
                //Get Terminal By Store Id
                Terminal = _reportRepository.GetBindTerminalByStoreId(StoreId);
                ViewBag.Chaptercount = Terminal.Count();
            }
            catch (Exception ex)
            {

                logger.Error("ReportController - BindTerminalByStoreId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new SelectList(Terminal.ToArray(), "Value", "Text"), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Get Pdf file details by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult PDFFileDetail(int id)
        {
            DailyPosFeedsDetails Storedetail = new DailyPosFeedsDetails();
            try
            {
                //Get Daily POS Feeds Details
                _reportRepository.getDailyPosFeedsDetails(ref Storedetail, id);
            }
            catch (Exception ex)
            {
                logger.Error("ReportController - PDFFileDetail - " + DateTime.Now + " - " + ex.Message.ToString());
                AdminSiteConfiguration.WriteErrorLogs("Controller : Report Method : PDFFileDetail Message:" + ex.Message + "Internal Message:" + ex.InnerException);
            }
            return View(Storedetail);
        }

        /// <summary>
        /// This method is Delete Daily POS by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult DeleteDailyPOS(int Id = 0)
        {
            try
            {
                //Delete Daily POS using ID
                _reportRepository.DeleteDailyPOS(Id);
                ActivityLog ActLog = new ActivityLog();
                ActLog.Action = 3;
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                //Get User Firstame
                ActLog.Comment = "Daily POS File Deleted by " + _commonRepository.getUserFirstName(UserName) + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                //This  class is used for Activity Log Insert.
                _activityLogRepository.ActivityLogInsert(ActLog);
                viewModel.StatusMessage = "Delete";
            }
            catch (Exception ex)
            {
                logger.Error("ReportController - DeleteDailyPOS - " + DateTime.Now + " - " + ex.Message.ToString());
                AdminSiteConfiguration.WriteErrorLogs("Controller : Report Method : DeleteDailyPOS Message:" + ex.Message + "Internal Message:" + ex.InnerException);
            }
            return null;
        }

        #endregion

        #region PayrollExpense
        /// <summary>
        /// This method return view of Payroll Expense
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,ViewPayrollReports")]
        public ActionResult PayrollExpense()
        {
            ViewBag.Title = "Payroll Expense - Synthesis";
            _commonRepository.LogEntries();     //Harsh's code
            try
            {
                if (Session["storeid"] == null)
                {
                    Session["storeid"] = "0";
                }
                if (!string.IsNullOrEmpty(Session["storeid"].ToString()))
                {
                    string store_idval = Session["storeid"].ToString();
                    ViewBag.Storeidvalue = store_idval;
                }
            }
            catch (Exception ex)
            {
                logger.Error("ReportController - PayrollExpense - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return View();
        }

        /// <summary>
        /// This method return view of grid Payroll Expense
        /// </summary>
        /// <param name="IsBindData"></param>
        /// <param name="currentPageIndex"></param>
        /// <param name="orderby"></param>
        /// <param name="IsAsc"></param>
        /// <param name="PageSize"></param>
        /// <param name="SearchRecords"></param>
        /// <param name="Alpha"></param>
        /// <param name="SearchTitle"></param>
        /// <returns></returns>
        public ActionResult PayrollExpenseGrid(int IsBindData = 1, int currentPageIndex = 1, string orderby = "Payrollid", int IsAsc = 0, int PageSize = 100, int SearchRecords = 1, string Alpha = "", string SearchTitle = "")
        {
            ViewBag.StatusMessage = viewModel.StatusMessage;
            model = new ReportModel();
            #region MyRegion_Array
            try
            {
                if (viewModel.IsFirst == false)
                {
                    viewModel.IsFirst = true;
                }
                if (viewModel.IsArray == true)
                {
                    foreach (string a1 in viewModel.Arr)
                    {
                        if (a1.Split(':')[0].ToString() == "IsBindData")
                        {
                            IsBindData = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "currentPageIndex")
                        {
                            currentPageIndex = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "orderby")
                        {
                            orderby = Convert.ToString(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "IsAsc")
                        {
                            IsAsc = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "PageSize")
                        {
                            PageSize = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "SearchRecords")
                        {
                            SearchRecords = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "Alpha")
                        {
                            Alpha = Convert.ToString(a1.Split(':')[1].ToString());
                        }
                    }
                }
            }
            catch
            {

            }

            viewModel.IsArray = false;
            viewModel.Arr = new string[]
            {  "IsBindData:" + IsBindData
                ,"currentPageIndex:" + currentPageIndex
                ,"orderby:" + orderby
                ,"IsAsc:" + IsAsc
                ,"PageSize:" + PageSize
                ,"SearchRecords:" + SearchRecords
                ,"Alpha:" + Alpha
            };
            #endregion

            #region MyRegion_BindData
            model.startIndex = ((currentPageIndex - 1) * PageSize) + 1;
            model.endIndex = model.startIndex + PageSize - 1;
            if (!string.IsNullOrEmpty(Convert.ToString(Session["storeid"])))
            {
                ViewBag.storeid = Convert.ToString(Session["storeid"]);
            }
            if (IsBindData == 1 || viewModel.IsEdit == true)
            {

                if (!string.IsNullOrEmpty(Convert.ToString(Session["storeid"])))
                {
                    model.StoreId = Convert.ToInt32(Session["storeid"]);
                }
                DateTime? salesdate = null;
                if (!string.IsNullOrEmpty(SearchTitle.Trim()))
                {
                    try
                    {
                        salesdate = Convert.ToDateTime(SearchTitle.Trim());
                    }
                    catch (Exception ex)
                    {
                        logger.Error("ReportController - PayrollExpenseGrid(1) - " + DateTime.Now + " - " + ex.Message.ToString());
                    }
                }
                else
                {
                    salesdate = DateTime.Today;
                }
                ViewBag.SearchTitle = salesdate;
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                //This class Is Get Payreoll Expense Grid
                //viewModel.BindPayrollExpenseData = _reportRepository.PayrollExpenseGridGetData(SearchRecords, SearchTitle, Convert.ToInt32(model.StoreId), UserName, Convert.ToInt32(Session["UserID"]), salesdate).OfType<PayrollExpenseSelect>().ToList();
                viewModel.TotalDataCount = viewModel.BindPayrollExpenseData.OfType<PayrollExpenseSelect>().ToList().Count();
            }

            if (viewModel.TotalDataCount == 0)
            {
                viewModel.StatusMessage = "NoItem";
            }

            try
            {
                if (string.IsNullOrEmpty(SearchTitle))
                {
                    SearchTitle = AdminSiteConfiguration.GetMonthDateFormat(Convert.ToString(AdminSiteConfiguration.GetEasternTime(DateTime.Now)));
                }
                ViewBag.IsBindData = IsBindData;
                ViewBag.CurrentPageIndex = currentPageIndex;
                ViewBag.LastPageIndex = this.getLastPageIndex(PageSize);
                ViewBag.OrderByVal = orderby;
                ViewBag.IsAscVal = IsAsc;
                ViewBag.PageSize = PageSize;
                ViewBag.Alpha = Alpha;
                ViewBag.SearchRecords = SearchRecords;
                ViewBag.SearchTitle = SearchTitle;

                ViewBag.StatusMessage = viewModel.StatusMessage;
                ViewBag.Insert = viewModel.InsertMessage;
                ViewBag.Edit = viewModel.Editmessage;
                ViewBag.Delete = viewModel.DeleteMessage;
                ViewBag.startindex = model.startIndex;

                if (viewModel.TotalDataCount < model.endIndex)
                {
                    ViewBag.endIndex = viewModel.TotalDataCount;
                }
                else
                {
                    ViewBag.endIndex = model.endIndex;
                }
                ViewBag.TotalDataCount = viewModel.TotalDataCount;
                var ColumnName = typeof(PayrollExpenseSelect).GetProperties().Where(p => p.Name == orderby).FirstOrDefault();

                model.Data = viewModel.BindPayrollExpenseData.OfType<PayrollExpenseSelect>().ToList().Skip(model.startIndex - 1).Take(PageSize);
                ViewBag.StatusMessage = viewModel.StatusMessage;
                viewModel.StatusMessage = "";
            }
            catch (Exception ex)
            {
                logger.Error("ReportController - PayrollExpenseGrid - " + DateTime.Now + " - " + ex.Message.ToString());
                AdminSiteConfiguration.WriteErrorLogs("Controller : Report Method : PayrollExpenseGrid Message:" + ex.Message + "Internal Message:" + ex.InnerException);
            }
            return View(model.Data);
            #endregion
        }

        /// <summary>
        /// This method is Get Payroll Expense details by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult PayrollExpenseDetail(int id,string payrolldate)
        {
            PayrollExpenseDetailss Detail = new PayrollExpenseDetailss();
            try
            {
                int StoreID = Convert.ToInt32(Session["storeid"]);
                //Get Payroll Expense Details using Id.
                _reportRepository.PayrollExpenseDetailss(ref Detail, StoreID, id);
            }
            catch (Exception ex)
            {
                logger.Error("ReportController - PayrollExpenseDetail - " + DateTime.Now + " - " + ex.Message.ToString());
                AdminSiteConfiguration.WriteErrorLogs("Controller : Report Method : PayrollExpenseDetail Message:" + ex.Message + "Internal Message:" + ex.InnerException);
            }
            ViewBag.SureApprove = _commonRepository.GetMessageValue("PEA", "Are you Sure you want to Approve?");
            ViewBag.Approve = _commonRepository.GetMessageValue("PEASF", "Approved Successfully.");
            ViewBag.PayrollDate = payrolldate;
            return View(Detail);
        }

        /// <summary>
        /// This method is save Payroll Expense details
        /// </summary>
        /// <param name="posteddata"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PayrollExpenseDetail(PayrollExpenseDetailss posteddata)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //This class is Payroll Cash analysis Details Amount
                    _reportRepository.PayrollCashAnalysisDetailAmount(ref posteddata);
                }
            }
            catch (Exception ex)
            {
                logger.Error("ReportController - PayrollExpenseDetail - " + DateTime.Now + " - " + ex.Message.ToString());
                AdminSiteConfiguration.WriteErrorLogs("Controller : Report Method : PayrollExpenseDetail Message:" + ex.Message + "Internal Message:" + ex.InnerException);
            }
            return RedirectToAction("PayrollExpenseDetail", "Report", new { id = posteddata.PayrollExpenseDetail[0].Payrollid });
        }

        /// <summary>
        /// This method is Get Approve Payroll Expense by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult ApprovePayrollExpense(int Id)
        {
            try
            {
                //Get Payroll Master using Id.
                PayrollMaster objp = _reportRepository.PayrollMaster(Id);
                int storeID = Convert.ToInt32(objp.StoreId);
                //Using this db class get stores on Line desktop.
                string Store = _qBRepository.GetStoreOnlineDesktop(storeID);
                //Using this db class get stores on Line desktop Flag.
                int StoreFlag = _qBRepository.GetStoreOnlineDesktopFlag(storeID);
                if (Store != "" && Id != 0)
                {
                    if (Store == "Online" && StoreFlag == 1)
                    {
                        Expense obj = new Expense();
                        obj.PaymentMethod = "Cash";
                        obj.EntityType = "Vendor";
                        //Get Vendor List Of Id From Bank account
                        string VendorListID = _reportRepository.GetVendorListIDFromBankAccount(storeID);
                        if (VendorListID != "" && VendorListID != null)
                        {
                            obj.VendorID = VendorListID;
                        }
                        else
                        {
                            AdminSiteConfiguration.WriteErrorLogs("Vendor ID Not Found: " + Id.ToString());
                            return Json(new { status = "QBError", message = "Vendor ID Not Found. Please check QB configuration." }, JsonRequestBehavior.AllowGet);
                        }
                        //Get Account List Of Id From Bank account
                        string AccountID = _reportRepository.GetAccountListIDFromBankAccount(storeID);
                        if (AccountID != "" && AccountID != null)
                        {
                            obj.AccountID = AccountID;
                        }
                        else
                        {
                            AdminSiteConfiguration.WriteErrorLogs("Bank List ID Not Found: " + Id.ToString());
                            return Json(new { status = "QBError", message = "Bank List ID Not Found. Please check QB configuration." }, JsonRequestBehavior.AllowGet);
                        }
                        obj.TxnDate = objp.EndCheckDate;
                        List<ExpenseDetail> details = new List<ExpenseDetail>();
                        //This class is Insert Expense Details
                        int PayrollReportId = _reportRepository.InsertExpenseDetail(ref obj, ref details, storeID, Id);
                        QBResponse objResponse = new QBResponse();
                        //Using this db class get Config Details.
                        QBOnlineconfiguration objOnlieDetail = _qBRepository.GetConfigDetail(storeID);
                        QBExpense.Create_Expense(obj, details, ref objResponse, objOnlieDetail);
                        if ((objResponse.ID != "0" && objResponse.ID != "") || objResponse.Status == "Done")
                        {
                            //Get Payroll List Report using ID 
                            List<int> List = _reportRepository.GetPayrollReportIdList(PayrollReportId);
                            if (List != null)
                            {
                                foreach (var item in List)
                                {
                                    //Update Payroll Reports
                                    _reportRepository.UpdatePayrollReports(objResponse.ID, item);
                                }
                                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            AdminSiteConfiguration.WriteErrorLogs(objResponse.Status + ": " + objResponse.Message);
                            // Extract detail message from QB response
                            string errorMessage = objResponse.Message;
                            return Json(new { status = "QBError", message = errorMessage }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                return Json(new { status = "Notsuccess" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("ReportController - ApprovePayrollExpense - " + DateTime.Now + " - " + ex.Message.ToString());
                AdminSiteConfiguration.WriteErrorLogs("Controller : Report Method : ApprovePayrollExpense Message:" + ex.Message + "Internal Message:" + ex.InnerException);
                return Json(new { status = "Notsuccess" }, JsonRequestBehavior.AllowGet);
            }
        }        
        #endregion

        #region Operationratioreportdaily
        /// <summary>
        /// This method is Operating Ratio Daily report 
        /// </summary>
        /// <param name="dashbordsuccess"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,ViewOperatingRatioDailyReport")]
        public ActionResult OperatingRatioReportDaily(string dashbordsuccess)
        {
            if (Session["storeid"] == null)
            {
                Session["storeid"] = "0";
            }
            if (!string.IsNullOrEmpty(Session["storeid"].ToString()))
            {
                viewModel.sStoreID = Session["storeid"].ToString();
            }
            ViewBag.Title = "Operating Ratios Daily Report - Synthesis";
            ViewBag.Storeidvalue = viewModel.sStoreID;
            OperatingRatioReport obj = new OperatingRatioReport();
            try
            {
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                //Get Storelist using RoleWise
                var StoreList = _commonRepository.GetStoreList_RoleWise(15, "ViewOperatingRatioDailyReport", UserName).Select(s => s.StoreId).ToList();
                //This class Get All Store Master data.
                obj.ReportStoreLists = _CompaniesRepository.GetAllStoreMasters().Where(s => s.IsActive == true && StoreList.Contains(s.StoreId)).Select(s => new ReportStoreList { StoreId = s.StoreId, StoreName = s.NickName }).OrderBy(O => O.StoreName).ToList();
            }
            catch (Exception ex)
            {

                logger.Error("ReportController - OperatingRatioReportDaily - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View(obj);
        }

        /// <summary>
        /// This method is get ration Daily List
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult getRatioDailyList(string FromDate, string ToDate, int? StoreId)
        {
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            if (Session["storeid"] == null)
            {
                Session["storeid"] = "0";
            }
            if (!string.IsNullOrEmpty(Session["storeid"].ToString()))
            {
                viewModel.sStoreID = Session["storeid"].ToString();
            }
            ViewBag.storeid = viewModel.sStoreID;
            List<OperatingRatioList> List = new List<OperatingRatioList>();
            try
            {
                model = new ReportModel();
                model.FromDate = FromDate;
                model.ToDate = ToDate;
                model.StoreId = StoreId;
                if (StoreId == null)
                {
                    model.StoreId = Convert.ToInt32(viewModel.sStoreID);
                    //Get Terminal Trace Daily Data.
                    List = _reportRepository.GetTerminalTracedataDaily(model, viewModel, UserName);
                }
                else if (StoreId == 0)
                {
                    //Get Terminal Trace Daily Data.
                    List = _reportRepository.GetTerminalTracedataDaily(model, viewModel, UserName);
                    ViewBag.startindex = "1";
                    ViewBag.endIndex = List.Count();
                    ViewBag.TotalDataCount = List.Count();
                    return PartialView("_OperatingRatioListDailyAll", List);
                }
                else
                {
                    //Get Terminal Trace Daily Data.
                    List = _reportRepository.GetTerminalTracedataDaily(model, viewModel, UserName);
                }
                ViewBag.startindex = "1";
                ViewBag.endIndex = List.Count();
                ViewBag.TotalDataCount = List.Count();
            }
            catch (Exception ex)
            {
                logger.Error("ReportController - getRatioDailyList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView("_OperatingRatioDailyList", List);
        }

        /// <summary>
        ///  This method is Get Store Wise Daily Data
        /// </summary>
        /// <param name="Department"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>
        public ActionResult getStoreWiseDailyData(string Department, string FromDate, string ToDate)
        {
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            model = new ReportModel();
            if (Department == "SUPPLIES" || Department == "PAYROLL FEE" || Department == "TAX BEER" || Department == "TAX WINE & LIQUOR")
            {
                model.CheckFlag = Department;
            }
            model.Department = Department;
            model.FromDate = FromDate;
            model.ToDate = ToDate;
            List<OperatingRatioList> Lst = new List<OperatingRatioList>();
            try
            {
                //  //Get Terminal Trace Daily Department wise Data.
                Lst = _reportRepository.Get_Terminal_Trace_data_department_wise_Daily(model, viewModel, UserName);
            }
            catch (Exception ex)
            {

                logger.Error("ReportController - getStoreWiseDailyData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(Lst, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///  This method is Export Excel Daily Data
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,ExportOperatingRatioDailyReport")]
        public ActionResult ExportExcelDailyData(string StartDate, string EndDate, int StoreId)
        {
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            try
            {
                DataTable dt1 = new DataTable();
                List<OperatingRatioList> List = new List<OperatingRatioList>();
                model = new ReportModel();
                model.FromDate = StartDate;
                model.ToDate = EndDate;
                model.StoreId = StoreId;
                //Get Terminal Trace Daily Data.
                List = _reportRepository.GetTerminalTracedataDaily(model, viewModel, UserName);


                dt1 = Common.LINQToDataTable(List);
                dt1.Columns["PDFAmount"].ColumnName = "PayrollAmount";
                dt1.Columns["PDFPercentage"].ColumnName = "SalesPercentage";
                Export oExport = new Export();
                string FileName = "OperationRatioDailyReport" + ".xls";

                int[] ColList = { 0, 1, 2, 3, 4, 5, 6 };
                string[] arrHeader = {
             "Department",
             "Sales",
             "% of Total Sales",
             "COGS",
             "COGS Ratio %",
             "Payroll Amount",
             "Payroll Ratio %"
             };

                //only change file extension to .xls for excel file
                string sFilterBy = "";
                if (StartDate != "")
                {
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Start Date: " + StartDate;
                }
                if (EndDate != "")
                {
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "End Date: " + EndDate;
                }
                if (StoreId != 0)
                {
                    //Get Store Nike Name using storeID
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Store: " + _commonRepository.GetStoreNikeName(StoreId);
                }

                if (dt1.Rows.Count > 0)
                {
                    DataRow dr = dt1.NewRow();
                    dt1.Rows.Add(dr);

                    dr = dt1.NewRow();
                    dr["Department"] = "Total";
                    dr["Sales"] = dt1.Compute("Sum(Sales)", string.Empty);
                    dr["TotalSalPercentage"] = dt1.Compute("Sum(TotalSalPercentage)", string.Empty);
                    dr["COgs"] = dt1.Compute("Sum(COgs)", "Department <> 'SUPPLIES'");
                    if (Convert.ToDecimal(dr["COgs"].ToString()) != 0 && Convert.ToDecimal(dr["Sales"].ToString()) != 0)
                    {
                        dr["SalPercentage"] = AdminSiteConfiguration.PriceWithComma(((Convert.ToDecimal(dr["COgs"].ToString()) / Convert.ToDecimal(dr["Sales"].ToString())) * 100).ToString());
                    }
                    else
                    {
                        dr["SalPercentage"] = "0";
                    }
                    dr["PayrollAmount"] = dt1.Compute("Sum(PayrollAmount)", string.Empty);
                    if (Convert.ToDecimal(dr["PayrollAmount"].ToString()) != 0 && Convert.ToDecimal(dr["Sales"].ToString()) != 0)
                    {
                        dr["SalesPercentage"] = AdminSiteConfiguration.PriceWithComma(((Convert.ToDecimal(dr["PayrollAmount"].ToString()) / Convert.ToDecimal(dr["Sales"].ToString())) * 100).ToString());
                    }
                    else
                    {
                        dr["SalesPercentage"] = "0";
                    }
                    dt1.Rows.Add(dr);
                }
                DataTable dtCloned = dt1.Clone();
                dtCloned.Columns[1].DataType = typeof(string);
                foreach (DataRow row in dt1.Rows)
                {
                    dtCloned.ImportRow(row);
                }
                for (int i = 0; i <= dtCloned.Rows.Count - 1; i++)
                {
                    if (dtCloned.Rows[i][1].ToString() != "")
                    {
                        dtCloned.Rows[i][1] = Convert.ToDecimal(dtCloned.Rows[i][1].ToString()).ToString("N");
                    }
                }
                oExport.ExportDetails(dtCloned, ColList, arrHeader, Export.ExportFormat.Excel, FileName, sFilterBy, "Operating Ratio Daily Report");
            }
            catch (Exception ex)
            {
                logger.Error("ReportController - ExportExcelDailyData - " + DateTime.Now + " - " + ex.Message.ToString());
                AdminSiteConfiguration.WriteErrorLogs("Controller : Report Method : ExportExcelData Message:" + ex.Message + "Internal Message:" + ex.InnerException);
            }
            return RedirectToAction("OperatingRatioReportDaily", "Report");
        }

        /// <summary>
        ///  This method is Export PDf Daily data
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,ExportOperatingRatioDailyReport")]
        public ActionResult ExportPDFDailyData(string StartDate, string EndDate, int StoreId)
        {
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            try
            {

                DataTable dt1 = new DataTable();
                List<OperatingRatioList> List = new List<OperatingRatioList>();

                model = new ReportModel();
                model.FromDate = StartDate;
                model.ToDate = EndDate;
                model.StoreId = StoreId;
                //Get Terminal Trace Daily Data.
                List = _reportRepository.GetTerminalTracedataDaily(model, viewModel, UserName);


                dt1 = Common.LINQToDataTable(List);
                dt1.Columns["PDFAmount"].ColumnName = "PayrollAmount";
                dt1.Columns["PDFPercentage"].ColumnName = "SalesPercentage";
                Export oExport = new Export();

                //only change file extension to .xls for excel file
                string sFilterBy = "";
                if (StartDate != "")
                {
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Start Date: " + StartDate;
                }
                if (EndDate != "")
                {
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "End Date: " + EndDate;
                }

                if (StoreId != 0)
                {
                    //Get Store Nike Name using storeID
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Store: " + _commonRepository.GetStoreNikeName(StoreId);
                }

                if (dt1.Rows.Count > 0)
                {
                    DataRow dr = dt1.NewRow();
                    dt1.Rows.Add(dr);

                    dr = dt1.NewRow();
                    dr["Department"] = "Total";
                    dr["Sales"] = dt1.Compute("Sum(Sales)", string.Empty);
                    dr["TotalSalPercentage"] = dt1.Compute("Sum(TotalSalPercentage)", string.Empty);
                    dr["COgs"] = dt1.Compute("Sum(COgs)", "Department <> 'SUPPLIES'");
                    if (Convert.ToDecimal(dr["COgs"].ToString()) != 0 && Convert.ToDecimal(dr["Sales"].ToString()) != 0)
                    {
                        dr["SalPercentage"] = AdminSiteConfiguration.PriceWithComma(((Convert.ToDecimal(dr["COgs"].ToString()) / Convert.ToDecimal(dr["Sales"].ToString())) * 100).ToString());
                    }
                    else
                    {
                        dr["SalPercentage"] = "0";
                    }
                    dr["PayrollAmount"] = dt1.Compute("Sum(PayrollAmount)", string.Empty);
                    if (Convert.ToDecimal(dr["PayrollAmount"].ToString()) != 0 && Convert.ToDecimal(dr["Sales"].ToString()) != 0)
                    {
                        dr["SalesPercentage"] = AdminSiteConfiguration.PriceWithComma(((Convert.ToDecimal(dr["PayrollAmount"].ToString()) / Convert.ToDecimal(dr["Sales"].ToString())) * 100).ToString());
                    }
                    else
                    {
                        dr["SalesPercentage"] = "0";
                    }
                    dt1.Rows.Add(dr);
                    GenerateDailyPDF(dt1, "OperationRatioReportDaily" + ".pdf", sFilterBy);
                }

            }
            catch (Exception ex)
            {
                logger.Error("ReportController - ExportPDFDailyData - " + DateTime.Now + " - " + ex.Message.ToString());
                AdminSiteConfiguration.WriteErrorLogs("Controller : Report Method : ExportExcelData Message:" + ex.Message + "Internal Message:" + ex.InnerException);
            }
            return RedirectToAction("OperatingRatioReportDaily", "Report");
        }
        #endregion

        #region PayrollAnalysis
        /// <summary>
        /// This method is Payroll Analysi Report 
        /// </summary>
        /// <param name="dashbordsuccess"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,ViewPayrollAnalysisReport")]
        public ActionResult PayrollAnalysisReport(string dashbordsuccess)
        {
            ViewBag.Title = "Payroll Expenses - Synthesis";
            _commonRepository.LogEntries();     //Harsh's code
            if (Session["storeid"] == null)
            {
                Session["storeid"] = "0";
            }
            if (!string.IsNullOrEmpty(Session["storeid"].ToString()))
            {
                string store_idval = Session["storeid"].ToString();
                ViewBag.Storeidvalue = store_idval;
            }
            PayrollAnalysisReport obj = new PayrollAnalysisReport();
            try
            {
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                //Get Storelist using Role Wise 
                var StoreList = _commonRepository.GetStoreList_RoleWise(14, "ViewPayrollAnalysisReport", UserName);
                var StoreIds = StoreList.Select(s => s.StoreId).ToList();
                //This class Get All Store Master data.
                obj.ReportStoreLists = _CompaniesRepository.GetAllStoreMasters().Where(s => s.IsActive == true && StoreIds.Contains(s.StoreId)).Select(s => new ReportStoreList { StoreId = s.StoreId, StoreName = s.NickName }).OrderBy(O => O.StoreName).ToList();
            }
            catch (Exception ex)
            {

                logger.Error("ReportController - PayrollAnalysisReport - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View(obj);
        }

        /// <summary>
        ///  This method is get payroll List data
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult getPayrollList(string FromDate, string ToDate, int? StoreId)
        {
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            if (Session["storeid"] == null)
            {
                Session["storeid"] = "0";
            }
            if (!string.IsNullOrEmpty(Session["storeid"].ToString()))
            {
                viewModel.sStoreID = Session["storeid"].ToString();
            }
            ViewBag.storeid = viewModel.sStoreID;
            model = new ReportModel();
            model.FromDate = FromDate;
            model.ToDate = ToDate;
            model.StoreId = StoreId;
            List<PayrollAnalysisList> List = new List<PayrollAnalysisList>();
            try
            {
                if (StoreId == null)
                {
                    model.StoreId = Convert.ToInt32(viewModel.sStoreID);
                    //This class Get payroll Analysis Data.
                    List = _reportRepository.GetPayrollAnalysisdata(model, viewModel, UserName);
                }
                else if (StoreId == 0)
                {
                    //This class Get payroll Analysis Data.
                    List = _reportRepository.GetPayrollAnalysisdata(model, viewModel, UserName);
                }
                else
                {
                    //This class Get payroll Analysis Data.
                    List = _reportRepository.GetPayrollAnalysisdata(model, viewModel, UserName);
                }
            }
            catch (Exception ex)
            {

                logger.Error("ReportController - getPayrollList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            ViewBag.startindex = "1";
            ViewBag.endIndex = List.Count();
            ViewBag.TotalDataCount = List.Count();
            return PartialView("_PayrollAnalysisList", List);
        }

        /// <summary>
        ///  This method is get store wise payroll Analysis Data
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="StoreId"></param>
        /// <param name="Department"></param>
        /// <returns></returns>
        public ActionResult getStoreWisePayrollanalysisData(string FromDate, string ToDate, int? StoreId, string Department)
        {
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            List<PayrollAnalysisList> List = new List<PayrollAnalysisList>();
            model = new ReportModel();
            model.FromDate = FromDate;
            model.ToDate = ToDate;
            model.StoreId = StoreId;
            try
            {
                //This class Get payroll Analysis Data.
                List = _reportRepository.GetPayrollAnalysisdata(model, viewModel, UserName);
            }
            catch (Exception ex)
            {

                logger.Error("ReportController - getStoreWisePayrollanalysisData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(List, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///  This method is Export Excel payroll analysis data
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,ExportPayrollAnalysisReport")]
        public ActionResult ExportExcelPayrollAnalysisData(string StartDate, string EndDate, int StoreId)
        {
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            try
            {
                DataTable dt1 = new DataTable();
                model = new ReportModel();
                model.FromDate = StartDate;
                model.ToDate = EndDate;
                model.StoreId = StoreId;
                List<PayrollAnalysisList> List = new List<PayrollAnalysisList>();
                //This class Get payroll Analysis Data.
                List = _reportRepository.GetPayrollAnalysisdata(model, viewModel, UserName);

                dt1 = Common.LINQToDataTable(List);
                Export oExport = new Export();
                string FileName = "PayrollAnalysisReport" + ".xls";

                int[] ColList = { 0, 1, 2 };
                string[] arrHeader = {
                 "Department",
                 "Payroll Amount",
                 "Payroll Breakdown %"
                 };

                //only change file extension to .xls for excel file
                string sFilterBy = "";
                if (StartDate != "")
                {
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Start Date: " + StartDate;
                }
                if (EndDate != "")
                {
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "End Date: " + EndDate;
                }

                if (StoreId != 0)
                {
                    //Get Store Nike Name using storeID
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Store: " + _commonRepository.GetStoreNikeName(StoreId);
                }

                if (dt1.Rows.Count > 0)
                {
                    DataRow dr = dt1.NewRow();
                    dt1.Rows.Add(dr);

                    dr = dt1.NewRow();
                    dr["DepartmentName"] = "Total";
                    dr["PayrollAmount"] = dt1.Compute("Sum(PayrollAmount)", string.Empty);
                    dr["PayrollPercentage"] = dt1.Compute("Sum(PayrollPercentage)", string.Empty);
                    dt1.Rows.Add(dr);
                }

                DataTable dtCloned = dt1.Clone();
                dtCloned.Columns[1].DataType = typeof(string);
                foreach (DataRow row in dt1.Rows)
                {
                    dtCloned.ImportRow(row);
                }
                for (int i = 0; i <= dtCloned.Rows.Count - 1; i++)
                {
                    if (dtCloned.Rows[i][1].ToString() != "")
                    {
                        dtCloned.Rows[i][1] = Convert.ToDecimal(dtCloned.Rows[i][1].ToString()).ToString("N");
                    }
                }
                oExport.ExportDetails(dtCloned, ColList, arrHeader, Export.ExportFormat.Excel, FileName, sFilterBy, "Payroll Expenses");
            }
            catch (Exception ex)
            {
                logger.Error("ReportController - ExportExcelPayrollAnalysisData - " + DateTime.Now + " - " + ex.Message.ToString());
                AdminSiteConfiguration.WriteErrorLogs("Controller : Report Method : ExportExcelPayrollAnalysisData Message:" + ex.Message + "Internal Message:" + ex.InnerException);
            }
            return RedirectToAction("PayrollAnalysisReport", "Report");
        }


        /// <summary>
        /// This method is Export Pdf payroll analysis data
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,ExportPayrollAnalysisReport")]
        public ActionResult ExportPDFPayrollAnalysisData(string StartDate, string EndDate, int StoreId)
        {
            try
            {
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                DataTable dt1 = new DataTable();
                List<PayrollAnalysisList> List = new List<PayrollAnalysisList>();
                model = new ReportModel();
                model.FromDate = StartDate;
                model.ToDate = EndDate;
                model.StoreId = StoreId;
                //This class Get payroll Analysis Data.
                List = _reportRepository.GetPayrollAnalysisdata(model, viewModel, UserName);

                dt1 = Common.LINQToDataTable(List);
                dt1.Columns.RemoveAt(dt1.Columns.Count - 1);
                dt1.Columns["PayrollPercentage"].ColumnName = "PayrollBreakdown";
                Export oExport = new Export();

                //only change file extension to .xls for excel file
                string sFilterBy = "";
                if (StartDate != "")
                {
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Start Date: " + StartDate;
                }
                if (EndDate != "")
                {
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "End Date: " + EndDate;
                }

                if (StoreId != 0)
                {
                    //Get Store Nike Name using storeID
                    sFilterBy = (sFilterBy == "" ? "" : sFilterBy + " ,") + "Store: " + _commonRepository.GetStoreNikeName(StoreId);
                }

                if (dt1.Rows.Count > 0)
                {
                    DataRow dr = dt1.NewRow();
                    dt1.Rows.Add(dr);

                    dr = dt1.NewRow();
                    dr["DepartmentName"] = "Total";
                    dr["PayrollAmount"] = dt1.Compute("Sum(PayrollAmount)", string.Empty);
                    dr["PayrollBreakdown"] = dt1.Compute("Sum(PayrollBreakdown)", string.Empty);
                    dt1.Rows.Add(dr);
                    GeneratePDF(dt1, "PayrollAnalysisReportDaily" + ".pdf", sFilterBy, "Payroll Expenses");
                }

            }
            catch (Exception ex)
            {
                logger.Error("ReportController - ExportPDFPayrollAnalysisData - " + DateTime.Now + " - " + ex.Message.ToString());
                AdminSiteConfiguration.WriteErrorLogs("Controller : Report Method : ExportPDFPayrollAnalysisData Message:" + ex.Message + "Internal Message:" + ex.InnerException);
            }
            return RedirectToAction("PayrollAnalysisReport", "Report");
        }

        /// <summary>
        ///  This method is Get store Wise total payroll Analysis data
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        public ActionResult getStoreWisePayrollanalysisTotal(string FromDate, string ToDate, int? StoreId)
        {
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            model = new ReportModel();
            model.FromDate = FromDate;
            model.ToDate = ToDate;
            model.StoreId = StoreId;
            List<PayrollAnalysisTotal> Lst = new List<PayrollAnalysisTotal>();
            try
            {
                //This class Get payroll Analysis total Data.
                Lst = _reportRepository.Get_PayrollAnalysis_data_Total(model, viewModel, UserName);
            }
            catch (Exception ex)
            {

                logger.Error("ReportController - getStoreWisePayrollanalysisTotal - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(Lst.OrderByDescending(o => o.PayrollPercentage), JsonRequestBehavior.AllowGet);
        }
        #endregion

        /// <summary>
        /// This method is Get store Wise Operating ratio daily All total data
        /// </summary>
        /// <param name="Department"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>
        public ActionResult getStoreWiseOperatingRationDailyAllTotal(string Department, string FromDate, string ToDate)
        {
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            model = new ReportModel();
            model.FromDate = FromDate;
            model.ToDate = ToDate;
            model.Department = Department;
            List<OperatingRatioList> Lst = new List<OperatingRatioList>();
            try
            {
                //Get Store Wise Operating Ratio Daily All Total
                Lst = _reportRepository.getStoreWiseOperatingRationDailyAllTotal(model, viewModel, UserName);
            }
            catch (Exception ex)
            {

                logger.Error("ReportController - getStoreWiseData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { Lst = Lst.OrderByDescending(o => o.TotalSalPercentage), Lst2 = Lst.OrderByDescending(o => o.SalPercentage) }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// This method is Get store Wise Operating ratio Department All total data
        /// </summary>
        /// <param name="Department"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>
        public ActionResult getStoreWiseOperatingRationDepAllTotal(string Department, string FromDate, string ToDate)
        {
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            model = new ReportModel();
            model.FromDate = FromDate;
            model.ToDate = ToDate;
            model.Department = Department;
            List<OperatingRatioList> Lst = new List<OperatingRatioList>();
            try
            {
                //Get Terminal Trace Data Department Wise.
                Lst = _reportRepository.GetTerminalTracedatadepartmentwise(model, viewModel, UserName);
            }
            catch (Exception ex)
            {

                logger.Error("ReportController - getStoreWiseOperatingRationDepAllTotal - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { Lst = Lst.OrderByDescending(o => o.TotalSalPercentage), Lst2 = Lst.OrderByDescending(o => o.SalPercentage), Lst3 = Lst.OrderByDescending(o => o.PDFPercentage) }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// This method is Get store Wise Operating ratio All total data
        /// </summary>
        /// <param name="Department"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>
        public ActionResult getStoreWiseOperatingRationAllTotal(string Department, string FromDate, string ToDate)
        {
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            model = new ReportModel();
            model.FromDate = FromDate;
            model.ToDate = ToDate;
            model.Department = Department;
            List<OperatingRatioList> Lst = new List<OperatingRatioList>();
            try
            {
                //Get Terminal Trace Daily Department wise Data.
                Lst = _reportRepository.Get_Terminal_Trace_data_department_wise_Daily(model, viewModel, UserName);
            }
            catch (Exception ex)
            {

                logger.Error("ReportController - getStoreWiseOperatingRationAllTotal - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(Lst, JsonRequestBehavior.AllowGet);
        }

        //Syncfusion grid Payroll Expenses
        [Authorize(Roles = "Administrator,ViewPayrollReports")]
        public ActionResult PayrollExpensesIndex()
        {
            ViewBag.Title = "Payroll Expenses - Synthesis";
            int storeid = Convert.ToInt32(Session["storeid"]);
            ViewBag.Storeidvalue = storeid;
            return View();
        }
        public ActionResult UrlDatasource(DataManagerRequest dm,string searchdate)
        {
            List<PayrollExpenseSelect> HrDeVm = new List<PayrollExpenseSelect>();
            IEnumerable<PayrollExpenseSelect> DataSource = new List<PayrollExpenseSelect>();
            int Count = 0;
            try
            {
                logger.Info("ReportController - UrlDatasource - " + DateTime.Now);
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                var StoreId = 0;
                if (!string.IsNullOrEmpty(Convert.ToString(Session["storeid"])))
                {
                    StoreId = Convert.ToInt32(Session["storeid"]);
                }

                int year = Convert.ToInt32(searchdate);
                if (string.IsNullOrEmpty(searchdate))
                {
                    year = DateTime.Now.Year;
                    //DateTime now = DateTime.Now;
                    //searchdate = now.ToString("MMM-yyyy");
                }
                //DateTime payrsearchdate = Convert.ToDateTime(searchdate.Trim());
                DataSource = _reportRepository.PayrollExpenseGridGetData(1, year.ToString(), Convert.ToInt32(StoreId), UserName, Convert.ToInt32(Session["UserID"]), year).OfType<PayrollExpenseSelect>().ToList();
                DataSource = DataSource.OrderByDescending(x => x.EndCheckDate).ToList();
                //DataSource = HrDeVm;

                DataOperations operation = new DataOperations();
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    string search = dm.Search[0].Key.ToString().Trim();
                    decimal searchAmount;
                    bool isNumericSearch = decimal.TryParse(search, out searchAmount);
                    DataSource = DataSource.Where(x => x.TotalAmount == searchAmount || x.Storename.Contains(search) || (x.ApproveDate.HasValue && x.ApproveDate.Value.ToString("yyyy-MM-dd HH:mm:ss").Contains(search))).ToList();
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
                Count = DataSource.Cast<PayrollExpenseSelect>().Count();
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
                logger.Error("ReportController - UrlDatasource - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return dm.RequiresCounts ? Json(new { result = DataSource, count = Count }) : Json(DataSource);
        }
    }
}