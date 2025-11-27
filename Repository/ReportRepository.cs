using EntityModels.Models;
using NLog;
using Repository.IRepository;
using SynthesisQBOnline.BAL;
using SynthesisViewModal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using Utility;

namespace Repository
{
    public class ReportRepository : IReportRepository
    {
        private DBContext _context;
        private readonly ICommonRepository _commonRepository;
        private readonly ISynthesisApiRepository _synthesisApiRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        public ReportRepository(DBContext context)
        {
            _context = context;
            this._commonRepository = new CommonRepository(new DBContext());
            this._synthesisApiRepository = new SynthesisApiRepository();
        }

        public void operatingRatioReport(ref OperatingRatioReport obj,string UserName) 
        {
            try
            {
                var StoreList = _commonRepository.GetStoreList_RoleWise(5, "ViewOperatingRatioReport", UserName).Select(s =>s.StoreId).ToList();
                obj.ReportStoreLists = _context.StoreMasters.Where(s => s.IsActive == true && StoreList.Contains(s.StoreId)).Select(s => new ReportStoreList { StoreId = s.StoreId, StoreName = s.NickName }).OrderBy(O => O.StoreName).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ReportRepository - OperatingRatioReport - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public List<OperatingRatioList> Get_Expense_Detail_Storewise(ReportModel model, ReportViewModel obj, string UserName)
        {
            List<OperatingRatioList> List = new List<OperatingRatioList>();
            try
            {
                var UserTypeId = _commonRepository.getUserTypeId(UserName);
                List = _context.Database.SqlQuery<OperatingRatioList>("SP_Get_Expense_Detail @StartDate = {0},@EndDate={1},@StoreID={2}", model.FromDate, model.ToDate, (model.StoreId == 0 ? null : model.StoreId)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ReportRepository - Get_Expense_Detail_Storewise - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return List;
        }
        public List<OperatingRatioList> GetTerminalTracedata(ReportModel model, ReportViewModel obj,string UserName)
        {
            List<OperatingRatioList> List = new List<OperatingRatioList>();
            try
            {
                var Userid = _commonRepository.getUserId(UserName);
                if (model.StoreId == 0)
                {
                    var UserTypeId = _commonRepository.getUserTypeId(UserName);
                    List = _context.Database.SqlQuery<OperatingRatioList>("SP_Get_Terminal_Trace_data_all @StartDate = {0},@EndDate={1},@ShiftId={2},@UserTypeId={3},@UserID={4}", model.FromDate, model.ToDate, model.ShiftId, UserTypeId, Userid).OrderByDescending(a => a.TotalSalPercentage).ThenBy(a => a.Status).ThenBy(a => a.Department).ToList();
                    List = List.Select(s => new OperatingRatioList
                    {
                        Department = s.Department,
                        Sales = s.Sales != null ? s.Sales : 0,
                        TotalSalPercentage = s.TotalSalPercentage != null ? s.TotalSalPercentage : 0,
                        COgs = s.COgs != null ? s.COgs : 0,
                        SalPercentage = s.SalPercentage != null ? s.SalPercentage : 0,
                        PDFAmount = s.PDFAmount != null ? s.PDFAmount : 0,
                        PDFPercentage = s.PDFPercentage != null ? s.PDFPercentage : 0,
                        PayrollDescription = s.PayrollDescription
                    }).ToList();                    
                }
                else
                {
                    List = _context.Database.SqlQuery<OperatingRatioList>("SP_Get_Terminal_Trace_data @StartDate = {0},@EndDate={1},@ShiftId={2},@StoreID = {3},@UserID={4}", model.FromDate, model.ToDate, model.ShiftId, Convert.ToInt32(model.StoreId), Userid).OrderByDescending(a => a.TotalSalPercentage).ThenBy(a=> a.Status).ThenBy(a => a.Department).ToList();
                    List = List.Select(s => new OperatingRatioList
                    {
                        Department = s.Department,
                        Sales = s.Sales != null ? s.Sales : 0,
                        TotalSalPercentage = s.TotalSalPercentage != null ? s.TotalSalPercentage : 0,
                        COgs = s.COgs != null ? s.COgs : 0,
                        SalPercentage = s.SalPercentage != null ? s.SalPercentage : 0,
                        PDFAmount = s.PDFAmount != null ? s.PDFAmount : 0,
                        PDFPercentage = s.PDFPercentage != null ? s.PDFPercentage : 0,
                        PayrollDescription = s.PayrollDescription
                    }).ToList();
                }
                
            }
            catch (Exception ex)
            {
                logger.Error("ReportRepository - GetTerminalTracedata - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return List;
        }

        public List<OperatingRatioList> GetTerminalTracedatadepartmentwise(ReportModel obj, ReportViewModel model, string UserName)
        {
            List<OperatingRatioList> List = new List<OperatingRatioList>();
            try
            {
                var UserTypeId = _commonRepository.getUserTypeId(UserName);
                List = _context.Database.SqlQuery<OperatingRatioList>("SP_Get_Terminal_Trace_data_department_wise @StartDate = {0},@EndDate={1},@ShiftId={2},@Department={3},@CheckFlag={4},@UserTypeId={5}", obj.FromDate, obj.ToDate, obj.ShiftId, obj.Department, obj.CheckFlag, UserTypeId).ToList();
            }
            catch (Exception ex)
            {

                logger.Error("ReportRepository - GetTerminalTracedatadepartmentwise - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return List;
        }

        public string GetSalesSammarieStartDate()
        {
            string StartDate = "";
            try
            {
                StartDate = Convert.ToString((from data in _context.SalesActivitySummaries.Where(s => s.StartDate < DateTime.Today && s.ShiftId != 0).OrderByDescending(a => a.SalesActivitySummaryId) select data.StartDate).FirstOrDefault());
            }
            catch (Exception ex)
            {

                logger.Error("ReportRepository - GetSalesSammarieStartDate - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return StartDate;
        }

        public void ShiftWiseTenderReport_Select(ref ShiftWiseTenderReport_Select shift,ref ReportViewModel viewModel, ReportModel model,string FromDate, string ToDate)
        {
            try
            {
                shift.shiftname = (from ShiftEnm e in Enum.GetValues(typeof(ShiftEnm))
                                                          select new ShiftList
                                                          {
                                                              Id = (int)e,
                                                              Name = e.ToString().Replace("_", " ")
                                                          }).ToList();

                shift.shiftnamelist = (from ShiftEnm e in Enum.GetValues(typeof(ShiftEnm))
                                                              select new ShiftNewList
                                                              {
                                                                  Id = (int)e,
                                                                  Name = e.ToString().Replace("_", " ")
                                                              }).ToList();

                foreach (var item in shift.shiftnamelist)
                {
                    decimal iCount = 0;
                    iCount = _context.Database.SqlQuery<GetShiftWiseTotal_Result>("GetShiftWiseTotal @StoreId = {0},@ShiftId = {1},@TransactionStartTime={2},@TransactionEndTime={3}", model.StoreId, item.Id, FromDate, ToDate).Count() > 0 ? _context.Database.SqlQuery<GetShiftWiseTotal_Result>("GetShiftWiseTotal @StoreId = {0},@ShiftId = {1},@TransactionStartTime={2},@TransactionEndTime={3}", model.StoreId, item.Id, FromDate, ToDate).FirstOrDefault().TotleByShift.Value : 0;
                    if (iCount > 0)
                    {
                        item.IsVisible = true;
                    }
                }
                int PaidOutCount = _context.Database.SqlQuery<int>("GetPaidOutCount @StartDate={0},@EndDate={1}", FromDate, ToDate).FirstOrDefault();
                shift.PaidOutCount = PaidOutCount;
                shift.PaidDistinctList = _context.Database.SqlQuery<string>("GetPaidOutTitleList @StartDate={0},@EndDate={1}", FromDate, ToDate).ToList();
                var aa = _context.Database.SqlQuery<string>("GetTitleList @TransactionStartTime={0},@TransactionEndTime={1},@StoreId={2}", FromDate, ToDate, model.StoreId).ToList();
                shift.TitleWiseTotal = aa.Select(x => new TitleWiseTotal() { Title = x.ToString() }).ToList();
                

                if (shift.TitleWiseTotal.Count == 0)
                {
                    viewModel.ErrorMessage = "NoItemFound";
                }
                else
                {
                    var idx = shift.TitleWiseTotal.FindIndex(x => x.Title.ToUpper().Contains("BOTTLE DEPOSIT FEES"));
                    if (idx >= 0)
                    {
                        var item1 = shift.TitleWiseTotal[idx];
                        shift.TitleWiseTotal.RemoveAt(idx);
                        shift.TitleWiseTotal.Insert(shift.TitleWiseTotal.Count, item1);
                    }
                }
                shift.CashTotal = _context.TenderInDrawers.Where(s => s.SalesActivitySummary.ShiftId != null && s.SalesActivitySummary.StoreId == model.StoreId && s.SalesActivitySummary.StartDate >= model.Sdate && s.SalesActivitySummary.StartDate <= model.Enddate && s.Title == "cash").Count() > 0 ? _context.TenderInDrawers.Where(s => s.SalesActivitySummary.ShiftId != null && s.SalesActivitySummary.StoreId == model.StoreId && s.SalesActivitySummary.StartDate >= model.Sdate && s.SalesActivitySummary.StartDate <= model.Enddate && s.Title == "cash").Sum(s => s.Amount) : 0;
                shift.OtherTotal = _context.TenderInDrawers.Where(s => s.SalesActivitySummary.ShiftId != null && s.SalesActivitySummary.StoreId == model.StoreId && s.SalesActivitySummary.StartDate >= model.Sdate && s.SalesActivitySummary.StartDate <= model.Enddate && s.Title == "cash").Count() > 0 ? _context.TenderInDrawers.Where(s => s.SalesActivitySummary.ShiftId != null && s.SalesActivitySummary.StoreId == model.StoreId && s.SalesActivitySummary.StartDate >= model.Sdate && s.SalesActivitySummary.StartDate <= model.Enddate && s.Title != "cash").Sum(s => s.Amount) : 0;
                var bb = _context.Database.SqlQuery<OtherDeposit>("GetotherdepositDetail @TransactionStartTime={0},@TransactionEndTime={1},@StoreId={2}", model.Sdate, model.Enddate, model.StoreId).ToList();

                List<OtherDepositeList> objList = new List<OtherDepositeList>();
                foreach (var item in bb)
                {
                    OtherDepositeList obj = new OtherDepositeList();
                    obj.Date = (item.CreateDate != null ? Convert.ToDateTime(item.CreateDate).ToString("MM/dd/yyyy") : "");
                    obj.Amount = item.Amount;
                    obj.Description = item.Name;
                    if (item.PaymentMethodId != null)
                    {
                        obj.PaymentMethod = _context.PaymentMethodMasters.Where(a => a.PaymentMethodId == item.PaymentMethodId) != null ? _context.PaymentMethodMasters.Where(a => a.PaymentMethodId == item.PaymentMethodId).FirstOrDefault().PaymentMethod : "";
                    }
                    if (item.OptionId != null)
                    {
                        obj.options = _context.OptionMasters.Where(a => a.OptionId == item.OptionId).FirstOrDefault().OptionName;
                    }
                    if (item.VendorId != null)
                    {
                        obj.Vendor = _context.VendorMasters.Where(a => a.VendorId == item.VendorId).FirstOrDefault().VendorName;
                    }
                    objList.Add(obj);
                    obj = null;
                }
                shift.OtherDepositeList = objList;
            }
            catch (Exception ex)
            {
                logger.Error("ReportRepository - ShiftWiseTenderReport_Select - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public List<TerminalMaster> terminalMasters()
        {
            List<TerminalMaster> TerminalMaster = new List<TerminalMaster>();
            try
            {
                TerminalMaster = _context.TerminalMasters.OrderBy(o => o.TerminalName).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ReportRepository - TerminalMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return TerminalMaster;
        }

        public IEnumerable GetData(int SearchRecords = 0, string startdate = "", string enddate = "", int Storeid = 0, int Terminalid = 0, string ShiftName = "", string UserName = "")
        {
            ReportModel model = new ReportModel();
            if (startdate == "")
            {
                startdate = AdminSiteConfiguration.GetDateformat(Convert.ToString(AdminSiteConfiguration.GetEasternTime(DateTime.Now).AddDays(-6)));
            }
            if (enddate == "")
            {
                enddate = AdminSiteConfiguration.GetDateformat(Convert.ToString(AdminSiteConfiguration.GetEasternTime(DateTime.Now)));
            }

            try
            {
                model.Sdate = Convert.ToDateTime(startdate).Date;
            }
            catch { }

            try
            {
                model.Enddate = Convert.ToDateTime(enddate).Date;
            }
            catch { }

            IEnumerable RtnData = null;
            var strStore = GetPOS_StoreList(Storeid,UserName);
            RtnData = _context.Database.SqlQuery<DailyPosFeeds>("SP_DailyPosFeeds @Storeid={0},@Terminalid={1},@TransactionStartTime={2},@TransactionEndTime={3}", strStore, Terminalid, startdate, enddate).ToList();

            if (RtnData != null)
            {
                if (ShiftName != "" && ShiftName != "0")
                {
                    try
                    {
                        RtnData = (from p in RtnData.Cast<DailyPosFeeds>() select p).Where(a => a.ShiftId == Convert.ToInt32(ShiftName))
                            .OrderByDescending(s => s.TransactionStartTime).OrderByDescending(s => s.TerminalName).ToList<DailyPosFeeds>();
                    }
                    catch (Exception ex) 
                    {
                        logger.Error("ReportRepository - GetData - " + DateTime.Now + " - " + ex.Message.ToString());
                    }
                }
            }
            return RtnData;
        }

        public string GetPOS_StoreList(int StoreId,string UserName)
        {
            string strList = "";
            try
            {
                if (!Roles.IsUserInRole("Administrator") || !Roles.IsUserInRole("ActivityLogSettings"))
                {
                    if (StoreId == 0)
                    {
                        var list = _commonRepository.GetStoreList_RoleWise(7, "ViewDailyPOSFeeds", UserName).ToList().Select(a => a.StoreId);
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
                logger.Error("ReportRepository - GetPOS_StoreList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return strList;
        }

        public void InsertStorewisePDFUpload(StorewisePDFUpload uploadPDF)
        {
            try
            {
                _context.StorewisePDFUploads.Add(uploadPDF);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("ReportRepository - InsertStorewisePDFUpload - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public List<ddllist> GetBindTerminalByStoreId(int StoreId = 0)
        {
            List<ddllist> Terminal = new List<ddllist>();
            try
            {
                if (StoreId == 0)
                {
                    Terminal = (from data in _context.TerminalMasters
                                orderby data.TerminalId ascending
                                select new ddllist
                                {
                                    Value = data.TerminalId.ToString(),
                                    Text = data.TerminalName.ToString()
                                }).ToList();
                }
                else
                {
                    Terminal = (from data in _context.StoreTerminals
                                where data.StoreId == StoreId && data.IsActive == true
                                orderby data.StoreTerminalId ascending
                                select new ddllist
                                {
                                    Value = data.TerminalId.ToString(),
                                    Text = data.TerminalMasters.TerminalName.ToString()
                                }).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error("ReportRepository - GetBindTerminalByStoreId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Terminal;
        }

        public void getDailyPosFeedsDetails(ref DailyPosFeedsDetails Storedetail, int id)
        {
            try
            {
                SalesActivitySummary Data = _context.SalesActivitySummaries.Find(id);
                Storedetail.StoreName = (from data1 in _context.StoreMasters
                                         where data1.StoreId == Data.StoreId
                                         select data1.Name).FirstOrDefault();
                Storedetail.TerminalName = (from data1 in _context.TerminalMasters
                                            where data1.TerminalId == (_context.StoreTerminals.Where(a => a.StoreTerminalId == Data.StoreTerminalId).FirstOrDefault().TerminalId)
                                            select data1.TerminalName).FirstOrDefault();
                Storedetail.StartDate = Data.StartDate;
                Storedetail.TransactionStartTime = Data.TransactionStartTime;
                Storedetail.TransactionEndTime = Data.TransactionEndTime;
                Storedetail.CustomerCount = Convert.ToInt32(Data.CustomerCount);
                Storedetail.NetSalesWithTax = Data.NetSalesWithTax;
                Storedetail.TotalTaxCollected = Data.TotalTaxCollected;
                Storedetail.AverageSale = Data.AverageSale;

                Storedetail.TenderList = (from a in _context.TenderInDrawers.Where(a => a.SalesActivitySummaryId == id)
                                          select new Tender
                                          {
                                              TendersTitle = a.Title,
                                              TendersAmount = a.Amount
                                          }).ToList<Tender>();
                Storedetail.DepartmentList = (from a in _context.DepartmentNetSales.Where(a => a.SalesActivitySummaryId == id && a.IsActive == true)
                                              select new Department
                                              {
                                                  DeptNetSalesTitle = a.Title,
                                                  DeptAmount = a.Amount
                                              }).ToList<Department>();
                Storedetail.Paidoutlist = (from a in _context.PaidOuts.Where(a => a.SalesActivitySummaryId == id && a.IsActive == true)
                                           select new PaidOuts
                                           {
                                               PaidOutTitle = a.Title,
                                               PaidOutAmount = a.Amount
                                           }).ToList<PaidOuts>();
            }
            catch (Exception ex)
            {
                logger.Error("ReportRepository - GetDailyPosFeedsDetails - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void DeleteDailyPOS(int Id = 0)
        {
            try
            {
                SalesActivitySummary Data = _context.SalesActivitySummaries.Find(Id);
                _context.DepartmentNetSales.RemoveRange(_context.DepartmentNetSales.Where(s => s.SalesActivitySummaryId == Id));
                _context.PaidOuts.RemoveRange(_context.PaidOuts.Where(s => s.SalesActivitySummaryId == Id));
                _context.TenderInDrawers.RemoveRange(_context.TenderInDrawers.Where(s => s.SalesActivitySummaryId == Id));
                _context.StorewisePDFUploads.RemoveRange(_context.StorewisePDFUploads.Where(s => s.FileName == Data.FileName));
                _context.creditcardDetails.RemoveRange(_context.creditcardDetails.Where(s => s.SalesActivitySummaryId == Id));
                _context.OtherDeposits.RemoveRange(_context.OtherDeposits.Where(s => s.SalesActivitySummaryId == Id));
                _context.PaidOutSettlements.RemoveRange(_context.PaidOutSettlements.Where(s => s.SalesActivitySummaryId == Id));

                var Invoices = _context.CashPaidoutInvoices.Where(s => s.SalesActivitySummaryId == Id).ToList();
                foreach (var item in Invoices)
                {
                    _context.Invoices.RemoveRange(_context.Invoices.Where(s => s.InvoiceId == item.InvoiceId));
                }
                _context.CashPaidoutInvoices.RemoveRange(_context.CashPaidoutInvoices.Where(s => s.SalesActivitySummaryId == Id));

                _context.SalesActivitySummaries.Remove(Data);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("ReportRepository - DeleteDailyPOS - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public IEnumerable PayrollExpenseGridGetData(int SearchRecords = 0, string SearchTitle = "", int StoreId = 0, string UserName = "", int UserID = 0, int? year = null)
        {
            IEnumerable RtnData = null;
            try
            {
                
                var strStore = GetPayrollExpense_StoreList(StoreId, UserName);
                RtnData = _context.Database.SqlQuery<PayrollExpenseSelect>("GetPayrollExpenseData @Storeid={0},@EndCheckYear={1}", strStore, year).ToList();
                if (year != null)
                {
                    clsActivityLog clsActivityLog = new clsActivityLog();
                    clsActivityLog.Action = "Click";
                    clsActivityLog.ModuleName = "Report";
                    clsActivityLog.PageName = "Payroll Expenses";
                    clsActivityLog.Message = "Payroll Expenses Generated for Year: " + year.ToString();
                    clsActivityLog.CreatedBy = UserID;
                    _synthesisApiRepository.CreateLog(clsActivityLog);
                }
            }
            catch (Exception ex)
            {
                AdminSiteConfiguration.WriteErrorLogs("Controller : Report Method : PayrollExpenseGridGetData Message:" + ex.Message + "Internal Message:" + ex.InnerException);
                logger.Error("ReportRepository - PayrollExpenseGridGetData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return RtnData;
        }

        public string GetPayrollExpense_StoreList(int StoreId,string UserName)
        {
            string strList = "";
            try
            {
                if (!Roles.IsUserInRole("Administrator") || !Roles.IsUserInRole("ActivityLogSettings"))
                {
                    if (StoreId == 0)
                    {
                        var list = _commonRepository.GetStoreList_RoleWise(6, "ViewPayrollReports", UserName).ToList().Select(a => a.StoreId);
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
                logger.Error("ReportRepository - GetPayrollExpense_StoreList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return strList;
        }

        public string GetNotConfigureAccount(int StoreID)
        {
            string Acc = "";
            try
            {
                var AccList = _context.Database.SqlQuery<PayrollAccount_Select>("SpPayrollCashAnalysis_Search @name = {0},@StoreId = {1}", "", StoreID).ToList().Where(a => a.DepartmentId == null || a.DepartmentId == 0).ToList();
                foreach (var item in AccList)
                {
                    Acc = (Acc == "" ? item.Name : Acc + ", " + item.Name);
                }
                var BankList = _context.PayrollBankAccounts.Where(a => a.StoreId == StoreID).FirstOrDefault();
                if (BankList != null)
                {
                    if (BankList.BankAccountId == null)
                    {
                        Acc = (Acc == "" ? "Bank Account not configured" : Acc + ", " + "Bank Account not configured");
                    }
                    if (BankList.VendorId == null)
                    {
                        Acc = (Acc == "" ? "Vendor not configured" : Acc + ", " + "Vendor not configured");
                    }
                }
            }
            catch (Exception ex)
            {
                AdminSiteConfiguration.WriteErrorLogs("Controller : Report Method : GetNotConfigureAccount Message:" + ex.Message + "Internal Message:" + ex.InnerException);
                logger.Error("ReportRepository - GetNotConfigureAccount - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Acc;
        }

        public void PayrollExpenseDetailss(ref PayrollExpenseDetailss Detail,int StoreID, int id)
        {
            try
            {
                Detail.PayrollExpenseDetail = _context.Database.SqlQuery<PayrollExpenseDetail>("GetPayrollExpense_Detail @Storeid={0},@PayrollId={1}", StoreID, id).ToList();
                var QBFlag = _context.QBOnlineConfigurations.Where(a => a.StoreId == StoreID).FirstOrDefault();
                if (QBFlag != null)
                {
                    if (QBFlag.IsTokenSuspend == 1)
                    {
                        if (Detail.PayrollExpenseDetail.Count > 0)
                        {
                            Detail.PayrollExpenseDetail.FirstOrDefault().QBStatusField = true;
                        }
                    }
                }
                foreach (var item in Detail.PayrollExpenseDetail)
                {
                    item.NotConfigureAccount = GetNotConfigureAccount(StoreID);
                }
            }
            catch (Exception ex)
            {
                logger.Error("ReportRepository - PayrollExpenseDetailss - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void PayrollCashAnalysisDetailAmount(ref PayrollExpenseDetailss posteddata)
        {
            try
            {
                if (posteddata.PayrollExpenseDetail.Count > 0)
                {
                    for (int i = 0; i < posteddata.PayrollExpenseDetail.Count; i++)
                    {
                        if (posteddata.PayrollExpenseDetail[i].Amount == null)
                        {
                            posteddata.PayrollExpenseDetail[i].Amount = 0;
                        }
                        if (posteddata.PayrollExpenseDetail[i].Amount != null && posteddata.PayrollExpenseDetail[i].PayrollCashAnalysisDetailId != 0)
                        {
                            if (posteddata.PayrollExpenseDetail[i].Amount != posteddata.PayrollExpenseDetail[i].OldTotalAmount)
                            {
                                if (posteddata.PayrollExpenseDetail[i].ValueIn == 2)
                                {
                                    posteddata.PayrollExpenseDetail[i].Amount = Convert.ToDecimal(posteddata.PayrollExpenseDetail[i].Amount) * -1;
                                }
                                _context.Database.ExecuteSqlCommand("SP_Payroll @Mode = {0},@Amount = {1},@PayrollCashAnalysisDetailId = {2}", "PayrollCashAnalysisDetailAmount", Convert.ToDecimal(posteddata.PayrollExpenseDetail[i].Amount), posteddata.PayrollExpenseDetail[i].PayrollCashAnalysisDetailId);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("ReportRepository - PayrollCashAnalysisDetailAmount - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public PayrollMaster PayrollMaster(int Id)
        {
            PayrollMaster master = new PayrollMaster();
            try
            {
                master = _context.PayrollMasters.Find(Id);
            }
            catch (Exception ex)
            {
                logger.Error("ReportRepository - PayrollMaster - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return master;
        }

        public string GetVendorListIDFromBankAccount(int StoreID)
        {
            string VendorListID = "";
            try
            {
                VendorListID = _context.Database.SqlQuery<string>("SP_VendorMaster @Mode ={0},@Storeid={1}", "GetListIDFromBankAccount", StoreID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("ReportRepository - GetVendorListIDFromBankAccount - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return VendorListID;
        }
        public string GetAccountListIDFromBankAccount(int StoreID)
        {
            string AccountID = "";
            try
            {
                AccountID = _context.Database.SqlQuery<string>("SP_DepartmentMaster @Mode ={0},@Storeid={1}", "GetListIDFromBankAccount", StoreID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("ReportRepository - GetAccountListIDFromBankAccount - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return AccountID;
        }

        public int InsertExpenseDetail(ref Expense obj, ref List<ExpenseDetail> details, int storeID, int Id)
        {
            int PayrollReportId = 0;
            try
            {
                var Details = _context.Database.SqlQuery<PayrollExpenseDetail>("GetPayrollExpense_Detail @Storeid={0},@PayrollId={1}", storeID, Id).ToList();
                //List<ExpenseDetail> Detail = new List<ExpenseDetail>();
                
                foreach (var item in Details)
                {
                    obj.TxnID = item.TxnID;
                    ExpenseDetail objE = new ExpenseDetail();
                    objE.Amount = item.Amount;
                    objE.DepartmentID = item.DepartmentListId;
                    objE.Description = item.Description;
                    objE.PayrollId = Convert.ToInt32(item.Payrollid);
                    objE.PayrollReportId = Convert.ToInt32(item.PayrollReportId);
                    PayrollReportId = Convert.ToInt32(item.PayrollReportId);
                    details.Add(objE);
                    objE = null;
                }
            }
            catch (Exception ex)
            {
                logger.Error("ReportRepository - InsertExpenseDetail - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PayrollReportId;
        }

        public List<int> GetPayrollReportIdList(int PayrollReportId)
        {
            List<int> vs = new List<int>();
            try
            {
                vs = _context.Database.SqlQuery<int>("SP_Payroll @Mode={0},@PayrollReportId={1}", "PayrollReportIdList", PayrollReportId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ReportRepository - GetPayrollReportIdList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return vs;
        }

        public PayrollReport UpdatePayrollReports(string ResponseID,int item)
        {
            PayrollReport PayrollReport = new PayrollReport();
            try
            {

                PayrollReport objR = _context.PayrollReports.Find(item);
                objR.TxnID = ResponseID;
                objR.IsSync = 1;
                objR.ApproveDate = DateTime.Now;
                AdminSiteConfiguration.WriteErrorLogs("Expense Entry Created : " + ResponseID);

                _context.Entry(objR).State = EntityState.Modified;
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                logger.Error("ReportRepository - UpdatePayrollReports - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PayrollReport;
        }

        public List<OperatingRatioList> GetTerminalTracedataDaily(ReportModel model, ReportViewModel obj, string UserName)
        {
            List<OperatingRatioList> List = new List<OperatingRatioList>();
            try
            {
                if (model.StoreId == 0)
                {
                    var UserTypeId = _commonRepository.getUserTypeId(UserName);
                    List = _context.Database.SqlQuery<OperatingRatioList>("SP_Get_Terminal_Trace_data_Daily_all @StartDate = {0},@EndDate={1},@UserTypeId={2}", model.FromDate, model.ToDate, UserTypeId).OrderByDescending(a => a.TotalSalPercentage).ThenBy(a => a.Department).ToList();
                }
                else
                {
                    List = _context.Database.SqlQuery<OperatingRatioList>("SP_Get_Terminal_Trace_data_Daily @StartDate = {0},@EndDate={1},@StoreID = {2}", model.FromDate, model.ToDate, Convert.ToInt32(model.StoreId)).OrderByDescending(a => a.TotalSalPercentage).ThenBy(a => a.Department).ToList();
                }

            }
            catch (Exception ex)
            {
                logger.Error("ReportRepository - GetTerminalTracedataDaily - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return List;
        }

        public List<OperatingRatioList> Get_Terminal_Trace_data_department_wise_Daily(ReportModel model, ReportViewModel obj, string UserName)
        {
            List<OperatingRatioList> List = new List<OperatingRatioList>();
            try
            {
                var UserTypeId = _commonRepository.getUserTypeId(UserName);
                List = _context.Database.SqlQuery<OperatingRatioList>("SP_Get_Terminal_Trace_data_department_wise_Daily @StartDate = {0},@EndDate={1},@Department={2},@CheckFlag={3},@UserTypeId={4}", model.FromDate, model.ToDate, model.Department, model.CheckFlag, UserTypeId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ReportRepository - Get_Terminal_Trace_data_department_wise_Daily - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return List;
        }

        public List<OperatingRatioList> getStoreWiseOperatingRationDailyAllTotal(ReportModel model, ReportViewModel obj, string UserName)
        {
            List<OperatingRatioList> List = new List<OperatingRatioList>();
            try
            {
                var UserTypeId = _commonRepository.getUserTypeId(UserName);
                List = _context.Database.SqlQuery<OperatingRatioList>("SP_Get_Terminal_Trace_data_department_wise_Daily @StartDate = {0},@EndDate={1},@Department={2},@CheckFlag={3},@UserTypeId={4}", model.FromDate, model.ToDate, model.Department, model.CheckFlag, UserTypeId).ToList();

            }
            catch (Exception ex)
            {
                logger.Error("ReportRepository - GetStoreWiseOperatingRationDailyAllTotal - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return List;
        }

        public List<PayrollAnalysisList> GetPayrollAnalysisdata(ReportModel model, ReportViewModel obj, string UserName)
        {
            List<PayrollAnalysisList> List = new List<PayrollAnalysisList>();
            try
            {
                if (model.StoreId == 0)
                {
                    var UserTypeId = _commonRepository.getUserTypeId(UserName);
                    List = _context.Database.SqlQuery<PayrollAnalysisList>("SP_Get_PayrollAnalysis_data_all @StartDate = {0},@EndDate={1},@UserTypeId = {2}", model.FromDate, model.ToDate, UserTypeId).OrderByDescending(a => a.PayrollAmount).ThenBy(a => a.DepartmentName).ToList();
                }
                else
                {
                    List = _context.Database.SqlQuery<PayrollAnalysisList>("SP_Get_PayrollAnalysis_data @StartDate = {0},@EndDate={1},@StoreID = {2}", model.FromDate, model.ToDate, Convert.ToInt32(model.StoreId)).OrderByDescending(a => a.PayrollAmount).ThenBy(a => a.DepartmentName).ToList();
                }

            }
            catch (Exception ex)
            {
                logger.Error("ReportRepository - GetPayrollAnalysisdata - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return List;
        }

        public List<PayrollAnalysisList> GetPayrollAnalysisdataDepartmentWise(ReportModel model, ReportViewModel obj, string UserName)
        {
            List<PayrollAnalysisList> List = new List<PayrollAnalysisList>();
            try
            {
                if (model.StoreId == 0)
                {
                    var UserTypeId = _commonRepository.getUserTypeId(UserName);
                    List = _context.Database.SqlQuery<PayrollAnalysisList>("SP_Get_PayrollAnalysis_data_DepartmentWise_ALL @StartDate = {0},@EndDate={1},@UserTypeId={2},@Department={3}", model.FromDate, model.ToDate, UserTypeId, model.Department).ToList();
                }
                else
                {
                    List = _context.Database.SqlQuery<PayrollAnalysisList>("SP_Get_PayrollAnalysis_data_DepartmentWise @StartDate = {0},@EndDate={1},@StoreId={2},@Department={3}", model.FromDate, model.ToDate, Convert.ToInt32(model.StoreId), model.Department).ToList();
                }

            }
            catch (Exception ex)
            {
                logger.Error("ReportRepository - GetPayrollAnalysisdataDepartmentWise - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return List;
        }

        public List<PayrollAnalysisTotal> Get_PayrollAnalysis_data_Total(ReportModel model, ReportViewModel obj, string UserName)
        {
            List<PayrollAnalysisTotal> List = new List<PayrollAnalysisTotal>();
            try
            {
                var UserTypeId = _commonRepository.getUserTypeId(UserName);
                List = _context.Database.SqlQuery<PayrollAnalysisTotal>("SP_Get_PayrollAnalysis_data_Total @StartDate = {0},@EndDate={1},@StoreId={2},@UserTypeId={3}", model.FromDate, model.ToDate, (model.StoreId == 0 ? null : model.StoreId), UserTypeId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ReportRepository - Get_PayrollAnalysis_data_Total - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return List;
        }
    }
}
