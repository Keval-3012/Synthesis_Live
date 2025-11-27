using EntityModels.Models;
using NLog;
using Repository.IRepository;
using SynthesisQBOnline.BAL;
using SynthesisQBOnline.QBClass;
using SynthesisViewModal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using System.Xml.Linq;
using Utility;

namespace Repository
{
    public class TerminalRepository : ITerminalRepository
    {
        private DBContext _context;
        private IQBRepository _qBRepository;
        private IUserActivityLogRepository _activityLogRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        public TerminalRepository(DBContext context)
        {
            _context = context;
            _qBRepository = new QBRepository(new DBContext());
            _activityLogRepository = new UserActivityLogRepository(new DBContext());
        }

        public DateTime GetTransactionStartTimeDesc()
        {
            DateTime TransactionStart = new DateTime();
            try
            {
                TransactionStart = Convert.ToDateTime(_context.SalesActivitySummaries.Where(s => s.TransactionStartTime < DateTime.Today).OrderByDescending(o => o.TransactionStartTime).FirstOrDefault().TransactionStartTime);
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - GetTransactionStartTimeDesc - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return TransactionStart;
        }

        public List<OtherDepositData> GetOtherDepositDatas(TerminalViewModel MainTerminalView)
        {
            List<OtherDepositData> otherDepositDatas = new List<OtherDepositData>();
            try
            {
                otherDepositDatas = _context.OtherDeposits.Where(s => s.StoreId == MainTerminalView.StoreId && s.CreateDate == MainTerminalView.SDate).ToList().Select(a => new OtherDepositData
                {
                    name = a.Name,
                    amount = a.Amount,
                    payment = a.PaymentMethodMasters.PaymentMethod,
                    storeid = a.StoreId,
                    date = a.CreateDate,
                    options = a.OptionMasters != null ? a.OptionMasters.OptionName : "",
                    Vendor = a.VendorMasters != null ? a.VendorMasters.VendorName : ""
                }).DefaultIfEmpty().ToList();
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - GetOtherDepositDatas - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return otherDepositDatas;
        }

        public void GetTotalAvrage(ref Terminal_Select terminal_Select, TerminalViewModel terminal, int StoreID)
        {
            try
            {
                List<SalesActivitySummary> salesActivityData = _context.Database.SqlQuery<SalesActivitySummary>("GetSalesActivitySummary_Data @Storeid={0},@TransactionStartTime={1}", StoreID, terminal.SDate).ToList();
                terminal_Select.Coustomercount = (int)salesActivityData.Sum(s => s.CustomerCount);

                terminal_Select.totalsalesamount = salesActivityData.Sum(s => s.NetSalesWithTax);
                decimal totalavgsales = salesActivityData.Count() > 0 ? salesActivityData.Sum(s => s.AverageSale) / salesActivityData.Count() : 0;
                terminal_Select.totalavgsales = (totalavgsales == 0 ? 0 : Convert.ToDecimal(totalavgsales.ToString("#.##")));
                decimal totalcash = _context.TenderInDrawers.Where(s => s.SalesActivitySummary.StoreId == terminal.StoreId && s.SalesActivitySummary.TransactionStartTime == terminal.SDate && s.SalesActivitySummary.IsActive == true && s.Title == "cash").Count() > 0 ? _context.TenderInDrawers.Where(s => s.SalesActivitySummary.StoreId == terminal.StoreId && s.SalesActivitySummary.TransactionStartTime == terminal.SDate && s.SalesActivitySummary.IsActive == true && s.Title == "cash").Sum(s => s.Amount) : 0;
                terminal_Select.totalcash = totalcash;

                if (terminal_Select.OtherDepositList != null)
                {
                    if (terminal_Select.OtherDepositList.Count > 0)
                    {
                        if (terminal_Select.OtherDepositList[0] != null)
                        {
                            terminal_Select.totalcash = totalcash + Convert.ToDecimal(terminal_Select.OtherDepositList.Sum(a => a.amount));
                        }
                    }
                }
                terminal_Select.NotConfigureAccount = "";

                var AccList = _context.Database.SqlQuery<TenderAccountsStoreWise>("SP_GetTenderAccountStoreWise @Storeid={0}", terminal.StoreId).ToList().Where(a => a.GroupId == null && a.DepartmentId == null && a.Flag == 1).ToList();
                foreach (var item in AccList)
                {
                    terminal_Select.NotConfigureAccount = (terminal_Select.NotConfigureAccount == "" ? item.Title : terminal_Select.NotConfigureAccount + ", " + item.Title);
                }

                terminal_Select.totalOverShort = _context.Database.SqlQuery<decimal>("GetOverShortAmount @TransactionStartTime={0},@StoreId={1}", terminal.SDate, terminal.StoreId).FirstOrDefault();


                SetOptionList(ref terminal_Select);

                GetSelectedVendorList(StoreID, ref terminal_Select);
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - GetTotalAvrage - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void GetSelectedVendorList(int StoreID, ref Terminal_Select terminal_Select)
        {
            try
            {
                string Stores = _qBRepository.GetStoreOnlineDesktop(StoreID);
                if (Stores == "Online")
                {
                    terminal_Select.SelectVendorList = (from a in _context.VendorMasters
                                                        where a.StoreId.ToString() == StoreID.ToString() && a.IsActive == true && a.ListId != null
                                                        orderby a.VendorName ascending
                                                        select new ddllist
                                                        {
                                                            Value = a.VendorName,
                                                            Text = a.VendorName,
                                                            ListID = a.ListId.ToString()
                                                        }).ToList().Where(a => !a.ListID.Contains("-")).ToList();

                    var DeptList = _context.Database.SqlQuery<DepartmentMaster>("SP_DepartmentMaster @Mode = {0}", "SelectExpense_Department").ToList();
                    terminal_Select.SelectDepartmentList = DeptList.Select(s => new ddllist
                    {
                        Value = s.DepartmentId.ToString(),
                        Text = s.DepartmentName,
                        selectedvalue = s.DepartmentId,
                        ListID = s.ListId,
                        Store = s.StoreId.ToString()
                    }).Where(b => b.Store == StoreID.ToString().ToString() && (b.ListID != null && !b.ListID.ToString().Contains("-"))).ToList();
                }
                else
                {
                    terminal_Select.SelectVendorList = (from a in _context.VendorMasters
                                                        where a.StoreId.ToString() == StoreID.ToString() && a.IsActive == true && a.ListId != null
                                                        orderby a.VendorName ascending
                                                        select new ddllist
                                                        {
                                                            Value = a.VendorName,
                                                            Text = a.VendorName,
                                                            ListID = a.ListId.ToString()
                                                        }).ToList().Where(a => a.ListID.Contains("-")).ToList();

                    var DeptList = _context.Database.SqlQuery<DepartmentMaster>("SP_DepartmentMaster @Mode = {0}", "SelectExpense_Department").ToList();
                    terminal_Select.SelectDepartmentList = DeptList.Select(s => new ddllist
                    {
                        Value = s.DepartmentId.ToString(),
                        Text = s.DepartmentName,
                        selectedvalue = s.DepartmentId,
                        ListID = s.ListId,
                        Store = s.StoreId.ToString()
                    }).Where(b => b.Store == StoreID.ToString() && (b.ListID != null && b.ListID.ToString().Contains("-"))).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - GetSelectedVendorList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void GetTerminalData(ref Terminal_Select obj, TerminalViewModel model)
        {
            try
            {

                obj.TerminalData = _context.Database.SqlQuery<BindTerminaldata>("GetSalesActivitySummary_Terminalwiselist @Storeid={0},@StartDate={1}", model.StoreId, model.CDate)
                                            .Select(s => new BindTerminaldata
                                            {
                                                TerminalId = s.TerminalId,
                                                TerminalName = s.TerminalName
                                            }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - GetTerminalData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public int GetUnassignShiftCount(int StoreID, string date)
        {
            int Count = 0;
            try
            {
                Count = _context.Database.SqlQuery<int>("GetUnassignShiftCount @StartDate={0},@Storeid={1}", date, StoreID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - GetUnassignShiftCount - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Count;
        }
        public DayCloseOutStatus GetDayCloseOutStatus_Data(int StoreID, string date)
        {
            DayCloseOutStatus outStatus = new DayCloseOutStatus();
            try
            {
                outStatus = _context.Database.SqlQuery<DayCloseOutStatus>("GetDayCloseOutStatus_Data @StartDate={0},@Storeid={1}", date, StoreID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - GetDayCloseOutStatus_Data - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return outStatus;
        }

        public void SetShiftDataGridValue(ref Terminal_Select terminal_Select, TerminalViewModel model)
        {
            try
            {
                List<SalesActivitySummary> salesActivitySummaries = new List<SalesActivitySummary>();
                salesActivitySummaries = _context.Database.SqlQuery<SalesActivitySummary>("GetSalesActivitySummary_ShiftList @TransactionStartTime={0},@StoreId={1},@TerminalId={2}", model.CDate, model.StoreId, Convert.ToInt32(model.TerminalId)).ToList();
                var GroupByData = salesActivitySummaries.GroupBy(s => new { s.StoreTerminalId }, (s, g) => new { s.StoreTerminalId, NetSalesWithTax = g.Sum(k => k.NetSalesWithTax), TotalTaxCollected = g.Sum(k => k.TotalTaxCollected), CustomerCount = g.Sum(k => k.CustomerCount) }).ToList();
                terminal_Select.ShiftData = GroupByData.Select(s => new BindShiftdata
                {
                    terminalid = s.StoreTerminalId,
                    SalesAmount = s.NetSalesWithTax,
                    CustomerCount = (int?)s.CustomerCount,
                    ShiftdataList = (from a1 in salesActivitySummaries
                                     select new ddlShiftList
                                     {
                                         //Id = (a1.ShiftId == null ? 0 : a1.ShiftId.Value),
                                         Id = a1.SalesActivitySummaryId,
                                         //(a1.ShiftId == null ? 0 : db.ShiftMasters.Where(a => a.ShiftId == a1.ShiftId).FirstOrDefault().ShiftId),
                                         ShiftName = (a1.ShiftId == null ? "Shift#" : _context.ShiftMasters.Where(a => a.ShiftId == a1.ShiftId).FirstOrDefault().ShiftName),
                                         //(a1.ShiftMasters == null? "Shift#" : a1.ShiftMasters.ShiftName),
                                         SalesActivitySummaryId = a1.SalesActivitySummaryId
                                     }).ToList<ddlShiftList>()
                }).ToList();
                salesActivitySummaries = new List<SalesActivitySummary>();
                salesActivitySummaries = _context.Database.SqlQuery<SalesActivitySummary>("GetSalesActivitySummary_Detail @TransactionStartTime={0},@StoreId={1}", model.CDate, model.StoreId).ToList();
                terminal_Select.Coustomercount = (int)salesActivitySummaries.Sum(s => s.CustomerCount);

                terminal_Select.totalsalesamount = salesActivitySummaries.Sum(s => s.NetSalesWithTax);
                terminal_Select.totalavgsales = ((terminal_Select.Coustomercount == 0) ? 0 : (terminal_Select.totalsalesamount / terminal_Select.Coustomercount));
                terminal_Select.totalavgsales = Math.Round(Convert.ToDecimal(terminal_Select.totalavgsales), 2);

                terminal_Select.totalcash = _context.Database.SqlQuery<decimal>("GetSalesActivitySummary_TotalCash @TransactionStartTime={0},@StoreId={1}", model.CDate, model.StoreId).FirstOrDefault();
                terminal_Select.totalOverShort = _context.Database.SqlQuery<decimal>("GetOverShortAmount @TransactionStartTime={0},@StoreId={1}", model.CDate, model.StoreId).FirstOrDefault();

                List<OtherDeposit> OtherDepositeData = new List<OtherDeposit>();
                OtherDepositeData = _context.OtherDeposits.Where(s => s.StoreId == model.StoreId && s.CreateDate == model.CDate).ToList();

                if (OtherDepositeData.Count() > 0)
                {
                    terminal_Select.OtherDepositList = OtherDepositeData.Select(s => new OtherDepositData
                    {
                        name = s.Name,
                        amount = s.Amount,
                        payment = s.PaymentMethodMasters != null ? s.PaymentMethodMasters.PaymentMethod : "",
                        storeid = s.StoreId,
                        date = s.CreateDate,
                        options = s.OptionMasters.OptionName,
                        Vendor = s.VendorMasters != null ? s.VendorMasters.VendorName : "",
                    }).ToList();
                }

                GetSelectedVendorList(model.StoreId, ref terminal_Select);
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - SetShiftDataGridValue - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public string GetStoreTerminalName(TerminalViewModel model)
        {
            try
            {
                model.TerminalName = _context.StoreTerminals.Where(a => a.StoreTerminalId == model.terminal_Id).Select(a => a.TerminalMasters.TerminalName).FirstOrDefault().ToString();
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - GetStoreTerminalName - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return model.TerminalName;
        }

        public void GetShiftWisetenderData(ref Terminal_Select terminal_Select, TerminalViewModel model)
        {
            try
            {
                terminal_Select.ShiftWisetenderData = _context.SalesActivitySummaries.Where(s => s.StoreId == model.StoreId && s.IsActive == true && s.StoreTerminalId == model.terminal_Id && s.SalesActivitySummaryId == model.shift_Id).ToList()
                                                              .Select(a1 => new Bindshiftwisetenderlist
                                                              {
                                                                  Id = a1.SalesActivitySummaryId,
                                                                  CustomerCount = a1.CustomerCount,
                                                                  Terminalname = a1.StoreTerminals.TerminalMasters.TerminalName.ToString().ToUpper(),
                                                                  StartTime = a1.TransactionStartTime.Value.ToString("hh:mm tt"),
                                                                  EndTime = a1.TransactionEndTime.Value.ToString("hh:mm tt"),
                                                                  SalesAmount = a1.NetSalesWithTax,
                                                                  AvgSales = a1.AverageSale,
                                                                  CashierNegative = a1.CashierNegative,
                                                                  TotalTaxAmount = a1.TotalTaxCollected,
                                                                  ShiftName = a1.ShiftMasters == null ? "Shift#" : a1.ShiftMasters.ShiftName,
                                                                  CashierName = a1.CashierName,
                                                                  Notes = a1.Notes,
                                                                  Paidoutamount = _context.PaidOuts.Where(s => s.SalesActivitySummaryId == a1.SalesActivitySummaryId).Count() > 0 ? _context.PaidOuts.Where(s => s.SalesActivitySummaryId == a1.SalesActivitySummaryId).Sum(s => s.Amount) : 0,
                                                                  FileName = a1.FileName,
                                                                  ShiftNameList = (from data1 in _context.ShiftMasters
                                                                                   where data1.ShiftName != null || data1.ShiftName != ""
                                                                                   select new ShiftNameList
                                                                                   {
                                                                                       Text = data1.ShiftName.ToString(),
                                                                                       Value = data1.ShiftId.ToString()
                                                                                   }).ToList<ShiftNameList>(),

                                                                  paidoutLists = _context.PaidOuts.Where(K => K.SalesActivitySummaryId == a1.SalesActivitySummaryId && K.IsActive == true).GroupBy(w => new { w.SalesActivitySummaryId, w.IsActive }, (s, g) => new { g.FirstOrDefault().PaidOutId, s.SalesActivitySummaryId, s.IsActive, Amount = g.Sum(k => k.Amount) }).ToList().Select(j => new PaidoutList
                                                                  {
                                                                      Amount = j.Amount,
                                                                      Title = "Paid Out",
                                                                      Id = j.PaidOutId,
                                                                      ListName = "PaidOut"
                                                                  }).ToList()
                                                              }).ToList();
                foreach (var aa in terminal_Select.ShiftWisetenderData)
                {
                    aa.TenderList = _context.Database.SqlQuery<tbl_TenderinDraw_ById_Result>("tbl_TenderInDraw_Byid @id={0}", aa.Id).ToList()
                                                                                .Select(a2 => new TenderList
                                                                                {
                                                                                    Amount = a2.Amount,
                                                                                    Id = a2.Id,
                                                                                    Name = a2.Title,
                                                                                    ListName = "Activity",
                                                                                    IsManually = false
                                                                                }).ToList();
                    foreach (var bb in aa.paidoutLists)
                    {
                        var CashPaidOutInvoiceData = _context.Database.SqlQuery<BindCase_PaidOut_Invoice>("GetCashPaidOut_Invoice @StoreId={0},@TerminalId={1},@ShiftID={2},@PaidOutId={3}", model.StoreId, model.terminal_Id, model.shift_Id, bb.Id).ToList();
                        bb.BindCase_PaidOut_Invoice = CashPaidOutInvoiceData;
                        var SettlementData = _context.PaidOutSettlements.Where(s => s.SalesActivitySummaryId == aa.Id).ToList();
                        bb.PaidOut_SettlementList = SettlementData.Select(s => new PaidOut_Settlement
                        {
                            Id = s.PaidOutSettlementId,
                            ActivitySalesSummaryID = s.SalesActivitySummaryId,
                            IsActive = s.IsActive,
                            Title = s.Title,
                            Amount = s.Amount
                        }).ToList();

                    }

                    foreach (var aa1 in aa.TenderList)
                    {
                        aa1.IsManually = Convert.ToBoolean(_context.TenderInDrawers.Where(a => a.TenderInDrawerId == aa1.Id).Select(a => a.IsManual).FirstOrDefault());
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - GetShiftWisetenderData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void SetCraditCardDetail(ref Terminal_Select terminal_Select, TerminalViewModel model)
        {
            try
            {
                var CreditcardDetails = _context.Database.SqlQuery<CreditcardDetailsData>("GetCreditCardDetails_New @storeid={0},@ShiftId={1},@date={2},@TerminalId={3}", model.StoreId, model.shiftid, model.date, model.terminal_Id).ToList();
                if (CreditcardDetails.Count() > 0)
                {
                    foreach (var item in CreditcardDetails)
                    {
                        terminal_Select.AMEX = item.Amount_AMEX;
                        terminal_Select.Discover = item.Amount_Discover;
                        terminal_Select.Visa = item.Amount_Visa;
                        terminal_Select.Master = item.Amount_Master;
                        terminal_Select.CCOffline = item.Amount_CCOffline;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - SetCraditCardDetail - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void SetOtherDepositeData(ref Terminal_Select terminal_Select, TerminalViewModel model)
        {
            try
            {
                List<OtherDeposit> OtherDepositeData = new List<OtherDeposit>();
                if (model.shift_Id == 0)
                {
                    if (model.date != "")
                    {
                        DateTime chkDate = Convert.ToDateTime(model.date);
                        OtherDepositeData = _context.OtherDeposits.Where(s => s.StoreId == model.StoreId && s.StoreTerminalId == model.terminal_Id && s.CreateDate == chkDate).ToList();
                    }
                }
                if (OtherDepositeData.Count() > 0)
                {
                    terminal_Select.OtherDepositList = OtherDepositeData.Select(s => new OtherDepositData
                    {
                        Id = s.OtherDepositId,
                        name = s.Name,
                        amount = s.Amount,
                        payment = s.PaymentMethodMasters != null ? s.PaymentMethodMasters.PaymentMethod : "",
                        storeid = s.StoreId,
                        date = s.CreateDate,
                        options = s.OptionMasters.OptionName,
                        Vendor = s.VendorMasters != null ? s.VendorMasters.VendorName : "",
                        iTerminalId = s.StoreTerminalId,
                        UploadDocument = s.UploadDocument,
                        TerminalName = s.StoreTerminals.TerminalMasters.TerminalName
                    }).ToList();
                }


                SetOptionList(ref terminal_Select);
                GetSelectedVendorList(model.StoreId, ref terminal_Select);

            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - SetOtherDepositeData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public bool CheckSalesActivitySummariesExistOrNot(ref Terminal_Select Posteddata, ref TerminalViewModel terminal)
        {
            TerminalViewModel model = new TerminalViewModel();
            var exist = _context.SalesActivitySummaries.Any(s => s.StoreId == model.StoreId && s.StartDate == model.SDate && s.StoreTerminalId == model.terminal_Id && s.SalesActivitySummaryId != model.SalesActivitySummaryId);
            try
            {
                model.StoreId = terminal.StoreId;
                model.SalesActivitySummaryId = Posteddata.ShiftWisetenderData[0].Id;
                model.date = terminal.date = Convert.ToString(_context.SalesActivitySummaries.Where(s => s.SalesActivitySummaryId == model.SalesActivitySummaryId).FirstOrDefault().StartDate);
                var DayCount = _context.Database.SqlQuery<DayCloseOutStatus>("GetDayCloseOutStatus_Data @StartDate={0},@Storeid={1}", model.date, model.StoreId).FirstOrDefault();
                if (DayCount != null)
                {
                    DayCloseOutStatus obj = _context.dayCloseOutStatuses.Find(DayCount.DayCloseOutId);
                    if (obj != null)
                    {
                        try
                        {
                            obj.DayCloseOutCount = 2;
                            _context.Entry(obj).State = EntityState.Modified;
                            _context.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }
                        finally
                        {
                            obj = null;
                        }
                    }
                }
                model.SDate = terminal.SDate = Convert.ToDateTime(model.date);
                model.terminal_Id = terminal.terminal_Id = Convert.ToInt32(Posteddata.terminal_id);
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - CheckSalesActivitySummariesExistOrNot - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return exist;
        }

        public void UpdateCreditcardDetails(ref Terminal_Select Posteddata, ref TerminalViewModel terminal)
        {
            TerminalViewModel model = new TerminalViewModel();
            try
            {
                model.CashierName = Posteddata.ShiftWisetenderData[0].CashierName;
                model.ShiftName = Posteddata.ShiftWisetenderData[0].ShiftName;
                model.Notes = Posteddata.ShiftWisetenderData[0].Notes;
                model.SalesActivitySummaryId = Posteddata.ShiftWisetenderData[0].Id;
                model.StoreId = terminal.StoreId;
                model.date = terminal.date;
                if (!String.IsNullOrEmpty(model.ShiftName))
                {
                    model.shift_Id_N = _context.ShiftMasters.Where(s => s.ShiftName == model.ShiftName).FirstOrDefault().ShiftId;
                }

                if (String.IsNullOrEmpty(model.ShiftName))
                {
                    model.ShiftName = "Shift#";
                }

                _context.Database.ExecuteSqlCommand("SP_POSTerminal @Mode = {0},@ShiftId = {1},@CashierName = {2},@Notes = {3},@SalesActivitySummaryId ={4}", "UpdateCashierName", model.shift_Id_N, model.CashierName, model.Notes, model.SalesActivitySummaryId);
                if (Posteddata.ShiftWisetenderData[0].TenderList != null)
                {
                    if (Posteddata.ShiftWisetenderData[0].TenderList.Count > 0)
                    {
                        for (int i = 0; i < Posteddata.ShiftWisetenderData[0].TenderList.Count; i++)
                        {
                            decimal amountval = Convert.ToDecimal(Posteddata.ShiftWisetenderData[0].TenderList[i].Amount);
                            int id = Convert.ToInt32(Posteddata.ShiftWisetenderData[0].TenderList[i].Id);
                            _context.Database.ExecuteSqlCommand("SP_POSTerminal @Mode = {0},@Amount = {1},@TenderInDrawerId = {2}", "UpdateTenderInDrawerAmount", amountval, id);
                        }
                    }
                }
                int j = 0;
                string amt = "";
                string nameval = "";
                if (terminal.Title != null)
                {
                    foreach (var val_id in terminal.Title)
                    {
                        TenderInDrawer dataDept = new TenderInDrawer();
                        if (val_id != string.Empty)
                        {
                            nameval = val_id;
                        }
                        else
                        {
                            nameval = "Shift#";
                        }
                        amt = terminal.Amount[j];
                        decimal amotval = Convert.ToDecimal(amt);
                        var successDept = _context.Database.ExecuteSqlCommand("insert into TenderInDrawer values({0},{1},{2},getdate(),1)", model.SalesActivitySummaryId, nameval, amotval);
                        j++;
                    }
                }
                if (Posteddata.AMEX != null || Posteddata.Discover != null || Posteddata.Master != null || Posteddata.Visa != null || Posteddata.CCOffline != null)
                {
                    var Exist = _context.creditcardDetails.Any(s => s.StoreId == model.StoreId && s.SalesActivitySummaryId == model.SalesActivitySummaryId);
                    if (Exist == false)
                    {
                        CreditcardDetails creditcardDetails = new CreditcardDetails();
                        creditcardDetails.ShiftId = null;
                        creditcardDetails.StoreId = model.StoreId;
                        creditcardDetails.Date = Convert.ToDateTime(model.date);
                        creditcardDetails.Amount_AMEX = Posteddata.AMEX ?? 0.00m;
                        creditcardDetails.Amount_Discover = Posteddata.Discover ?? 0.00m;
                        creditcardDetails.Amount_Master = Posteddata.Master ?? 0.00m;
                        creditcardDetails.Amount_Visa = Posteddata.Visa ?? 0.00m;
                        creditcardDetails.Amount_CCOffline = Posteddata.CCOffline ?? 0.00m;
                        creditcardDetails.StoreTerminalId = terminal.terminal_Id;
                        _context.creditcardDetails.Add(creditcardDetails);
                        _context.SaveChanges();
                    }
                    else
                    {
                        int ID = _context.creditcardDetails.Where(s => s.StoreId == model.StoreId && s.SalesActivitySummaryId == model.SalesActivitySummaryId).FirstOrDefault().CreditcardDetailId;
                        var creditcardDetails = _context.creditcardDetails.Find(ID);
                        creditcardDetails.ShiftId = null;
                        creditcardDetails.StoreId = model.StoreId;
                        creditcardDetails.Date = Convert.ToDateTime(model.date);
                        creditcardDetails.Amount_AMEX = Posteddata.AMEX ?? 0.00m;
                        creditcardDetails.Amount_Discover = Posteddata.Discover ?? 0.00m;
                        creditcardDetails.Amount_Master = Posteddata.Master ?? 0.00m;
                        creditcardDetails.Amount_Visa = Posteddata.Visa ?? 0.00m;
                        creditcardDetails.Amount_CCOffline = Posteddata.CCOffline ?? 0.00m;
                        creditcardDetails.StoreTerminalId = terminal.terminal_Id;
                        _context.Entry(creditcardDetails).State = EntityState.Modified;
                        _context.SaveChanges();
                    }
                }

                CreditcardDetails tbl_Creditcard_Details = _context.creditcardDetails.Where(a => a.SalesActivitySummaryId == model.SalesActivitySummaryId).FirstOrDefault();
                if (tbl_Creditcard_Details != null)
                {
                    var shiftid = _context.ShiftMasters.Where(a => a.ShiftName == model.ShiftName).FirstOrDefault();
                    if (shiftid != null)
                    {
                        tbl_Creditcard_Details.ShiftId = Convert.ToInt32(shiftid.ShiftId);
                        _context.Entry(tbl_Creditcard_Details).State = EntityState.Modified;
                        _context.SaveChanges();
                    }
                }

                if (Posteddata.ShiftWisetenderData[0].paidoutLists != null)
                {
                    if (Posteddata.ShiftWisetenderData[0].paidoutLists.Count > 0)
                    {
                        for (int i = 0; i < Posteddata.ShiftWisetenderData[0].paidoutLists.Count; i++)
                        {
                            int? Id = Posteddata.ShiftWisetenderData[0].paidoutLists[i].Id;
                            int UserID = UserModule.getUserId();
                            decimal? AMT = Posteddata.ShiftWisetenderData[0].paidoutLists[i].Amount;
                            if (AMT != null)
                            {
                                PaidOut paidOut = _context.PaidOuts.Find(Id);
                                if (paidOut != null)
                                {
                                    paidOut.Amount = Convert.ToDecimal(AMT);
                                    _context.Entry(paidOut).State = EntityState.Modified;
                                    _context.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - UpdateCreditcardDetails - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void SaveOtherDeposit(ref TerminalViewModel model)
        {
            try
            {
                TerminalViewModel terminalViewData = model as TerminalViewModel;
                OtherDeposit obj = new OtherDeposit();
                obj.Name = terminalViewData.name;
                List<PaymentMethodMaster> PaymentMethodData = _context.PaymentMethodMasters.ToList();
                terminalViewData.payment = String.IsNullOrEmpty(terminalViewData.payment) ? "null" : terminalViewData.payment;
                obj.PaymentMethodId = PaymentMethodData.Where(s => s.PaymentMethodId == Convert.ToInt32(terminalViewData.payment)).FirstOrDefault().PaymentMethodId;
                obj.Amount = terminalViewData.AmountValue;
                obj.CreateDate = Convert.ToDateTime(terminalViewData.date);
                obj.StoreId = terminalViewData.sid;
                obj.OptionId = _context.OptionMasters.Where(s => s.OptionName == terminalViewData.options).FirstOrDefault().OptionId;
                obj.VendorId = terminalViewData.vendor != "null" ? _context.VendorMasters.Where(s => s.VendorName == terminalViewData.vendor && s.StoreId == terminalViewData.sid).FirstOrDefault().VendorId : (int?)null;
                int DeptID = Convert.ToInt32(terminalViewData.Department);
                obj.DepartmentId = terminalViewData.Department != "null" ? _context.DepartmentMasters.Where(s => s.DepartmentId == DeptID && s.StoreId == terminalViewData.sid).FirstOrDefault().DepartmentId : DeptID;
                obj.StoreTerminalId = terminalViewData.Terminal != 0 ? terminalViewData.Terminal : (int?)null;
                obj.ShiftId = terminalViewData.Shift == 0 ? null : terminalViewData.Shift;
                obj.UploadDocument = terminalViewData.UploadFile_Title;
                obj.SalesActivitySummaryId = terminalViewData.ActivitySalesSummuryId != 0 ? terminalViewData.ActivitySalesSummuryId : (int?)null;
                _context.OtherDeposits.Add(obj);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - SaveOtherDeposit - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void UpdateOtherDeposit(ref TerminalViewModel model)
        {
            try
            {
                TerminalViewModel terminalViewData = model as TerminalViewModel;
                if (terminalViewData.flag == 1)
                {
                    _context.Database.ExecuteSqlCommand("SP_POSTerminal @Mode = {0},@UploadDocument = {1},@OtherDepositId = {2}", "UpdateOtherDepositeDocument", terminalViewData.UploadFile_Title, terminalViewData.Id);
                }
                OtherDeposit deposit = _context.OtherDeposits.SingleOrDefault(b => b.OtherDepositId == terminalViewData.Id);
                if (deposit != null)
                {
                    int? NullValues = null;
                    var PaymentMethodData = _context.PaymentMethodMasters.ToList();
                    deposit.PaymentMethodId = PaymentMethodData.Where(s => s.PaymentMethod == terminalViewData.payment).FirstOrDefault().PaymentMethodId;
                    deposit.Name = terminalViewData.name;
                    deposit.Amount = terminalViewData.AmountValue;
                    deposit.OptionId = _context.OptionMasters.Where(s => s.OptionName == terminalViewData.options).FirstOrDefault().OptionId;
                    deposit.VendorId = terminalViewData.vendor != "" ? _context.VendorMasters.Where(s => s.VendorName == terminalViewData.vendor && s.StoreId == deposit.StoreId).FirstOrDefault().VendorId : NullValues;
                    deposit.DepartmentId = terminalViewData.Department != "" ? _context.DepartmentMasters.Where(s => s.DepartmentName == terminalViewData.Department && s.StoreId == deposit.StoreId).FirstOrDefault().DepartmentId : NullValues;
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - UpdateOtherDeposit - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public int SetDepositeData(ref Terminal_Select terminal_Select, TerminalViewModel terminal)
        {
            int Count = 0;
            List<OtherDeposit> OtherDepositeData = new List<OtherDeposit>();
            try
            {
                if (terminal.SalesActivityId != 0)
                {
                    OtherDepositeData = _context.OtherDeposits.Where(s => s.SalesActivitySummaryId == terminal.SalesActivityId).ToList();
                }
                else if (terminal.StoreId == 0)
                {
                    terminal.CDate = Convert.ToDateTime(terminal.date);

                    OtherDepositeData = _context.OtherDeposits.Where(s => s.StoreId == terminal.StoreId && s.StoreTerminalId == terminal.terminal_Id && s.CreateDate == terminal.CDate).ToList();
                }

                List<ShiftMaster> ShiftMaster = _context.ShiftMasters.ToList();
                if (OtherDepositeData.Count() > 0)
                {
                    terminal_Select.OtherDepositList = OtherDepositeData.Select(s => new OtherDepositData
                    {
                        Id = s.OtherDepositId,
                        name = s.Name,
                        amount = s.Amount,
                        payment = s.PaymentMethodMasters != null ? s.PaymentMethodMasters.PaymentMethod : "",
                        storeid = s.StoreId,
                        date = s.CreateDate,
                        options = s.OptionMasters.OptionName,
                        Vendor = s.VendorMasters != null ? s.VendorMasters.VendorName : "",
                        Department = s.DepartmentMaster != null ? s.DepartmentMaster.DepartmentName : "",
                        iTerminalId = s.StoreTerminalId,
                        Shift = s.ShiftId,
                        ShiftName = s.ShiftId != null ? ShiftMaster.Where(k => k.ShiftId == s.ShiftId).FirstOrDefault().ShiftName : "",
                        UploadDocument = s.UploadDocument,
                        TerminalName = s.StoreTerminals.TerminalMasters.TerminalName
                    }).ToList();
                }
                Count = OtherDepositeData.Count();
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - SetDepositeData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Count;
        }

        public void SetOptionList(ref Terminal_Select terminal_Select)
        {
            try
            {
                terminal_Select.SelectOptionList = (from a in _context.OptionMasters
                                                    select new ddllist
                                                    {
                                                        Value = a.OptionName,
                                                        Text = a.OptionName
                                                    }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - SetOptionList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void SetShiftNameList(ref Terminal_Select terminal_Select)
        {
            try
            {
                terminal_Select.ShiftNameList = (from a1 in _context.ShiftMasters
                                                 select new ddllist
                                                 {
                                                     Value = a1.ShiftId.ToString(),
                                                     Text = a1.ShiftName,
                                                 }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - SetShiftNameList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void Deleteotherdepositdata(TerminalViewModel terminal)
        {
            try
            {
                OtherDeposit otherdeposit = _context.OtherDeposits.Find(terminal.Id);
                _context.SalesOtherDeposites.RemoveRange(_context.SalesOtherDeposites.Where(s => s.OtherDepositId == terminal.Id));
                _context.OtherDeposits.Remove(otherdeposit);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - Deleteotherdepositdata - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public string SavePaidOutSettlement(TerminalSettlement terminal)
        {
            try
            {
                PaidOutSettlement paidOutSettlement = new PaidOutSettlement();
                paidOutSettlement.SalesActivitySummaryId = terminal.SettlementID;
                paidOutSettlement.Amount = terminal.Amount;
                paidOutSettlement.Title = terminal.Title;
                paidOutSettlement.IsActive = true;
                paidOutSettlement.CreatedBy = UserModule.getUserId();
                paidOutSettlement.CreatedOn = DateTime.Today;
                _context.PaidOutSettlements.Add(paidOutSettlement);
                _context.SaveChanges();
                if (paidOutSettlement.PaidOutSettlementId != 0)
                {
                    terminal.ErrorMessage = "sucess";
                }
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - SavePaidOutSettlement - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return terminal.ErrorMessage;
        }

        public void DeleteSettlementEntry(TerminalViewModel terminal)
        {
            try
            {
                PaidOutSettlement paidOutSettlement = _context.PaidOutSettlements.Find(terminal.Id);
                _context.PaidOutSettlements.Remove(paidOutSettlement);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - DeleteSettlementEntry - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public List<ddllist> GetPayDetailList(string Id)
        {
            List<ddllist> paymethodlist = new List<ddllist>();
            try
            {
                if (Id == "Rebate" || Id == "Other")
                {
                    paymethodlist = (from data in _context.PaymentMethodMasters.Where(s => s.PaymentMethodId == 2 || s.PaymentMethodId == 1)
                                     select new ddllist
                                     {
                                         Value = data.PaymentMethodId.ToString(),
                                         Text = data.PaymentMethod
                                     }).ToList();
                }
                else
                {
                    paymethodlist = (from data in _context.PaymentMethodMasters
                                     select new ddllist
                                     {
                                         Value = data.PaymentMethodId.ToString(),
                                         Text = data.PaymentMethod
                                     }).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - GetPayDetailList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return paymethodlist;
        }

        public string SetDayCloseOut(TerminalViewModel terminal)
        {
            try
            {
                CloseOut closeOut = new CloseOut();
                closeOut.PendingDayShift = _context.Database.SqlQuery<string>("Get_Pending_Shift_For_CloseOut @Storeid={0},@StartDate={1}", terminal.StoreId, terminal.date_val).FirstOrDefault();
                closeOut.TotalSalesamount = _context.Database.SqlQuery<decimal>("GetTotalAmount_DayCloseOut @StoreId={0},@StartDate={1}", terminal.StoreId, terminal.date_val).FirstOrDefault();
                List<DailyCloseOutSalesList> SalesList = _context.Database.SqlQuery<DailyCloseOutSalesList>("GetTilteWiseSaleList_DayCloseOut @StoreId={0},@SaleDate={1}", terminal.StoreId, terminal.date_val).ToList().Where(a => a.ConfigurationGroupId != null).ToList();
                if (SalesList.Count > 0 && closeOut.PendingDayShift == "")
                {
                    closeOut.SalesGeneralEntries = _context.Database.SqlQuery<int>("SalesGeneralEntries_Insert @StoreId={0},@Salesdate={1},@userid={2},@totalamount={3}", terminal.StoreId, terminal.date_val, terminal.UserID, Convert.ToDecimal(closeOut.TotalSalesamount)).FirstOrDefault();
                    if (closeOut.SalesGeneralEntries > 0)
                    {
                        List<SalesActivitySummarySalesList> BTData = _context.Database.SqlQuery<SalesActivitySummarySalesList>("GetSalesActivitySummary_Terminalwiselist @StoreId={0},@StartDate={1}", terminal.StoreId, terminal.date_val).ToList();
                        closeOut.POSCount = BTData.Count();
                        _context.Database.ExecuteSqlCommand("SP_POSTerminal @Mode = {0},@SalesGeneralID = {1},@NoOfPos = {2}", "UpdateNoOfPos", closeOut.SalesGeneralEntries, closeOut.POSCount);

                        closeOut.SalesTotal = Convert.ToDecimal(BTData.Sum(a => a.NetSalesWithTax));
                        closeOut.TotalTaxAmount = Convert.ToDecimal(BTData.Sum(a => a.TotalTaxCollected));
                        closeOut.PaidOutTotal = _context.Database.SqlQuery<decimal>("GetPaidOutTotal_ByTerminalId @TransactionStartTime={0},@StoreId={1},@TerminalId={2}", terminal.date_val, terminal.StoreId, 0).FirstOrDefault();
                        closeOut.OverShortVal = closeOut.SalesTotal - (closeOut.SalesTotal + closeOut.PaidOutTotal);
                        foreach (var item in SalesList)
                        {
                            SalesChildEntries objChild = new SalesChildEntries();
                            objChild.SalesGeneralId = closeOut.SalesGeneralEntries;
                            objChild.GroupAccountId = item.ConfigurationGroupId.GetValueOrDefault();
                            objChild.DepartmentId = item.DepartmentId;
                            objChild.Amount = (item.AccountType == "D" ? Convert.ToDecimal(item.TotalAmount) : Math.Abs(Convert.ToDecimal(item.TotalAmount)));
                            objChild.Title = item.Title;
                            objChild.Memo = item.Memo;
                            objChild.TypeId = item.TypicalBalanceId.GetValueOrDefault();
                            _context.salesChildEntries.Add(objChild);
                            _context.SaveChanges();
                            objChild = null;
                        }

                        var OtherDepositList = _context.Database.SqlQuery<OtherDeposit>("GetSalesActivitySummary_OtherDeposite @TransactionStartTime={0},@StoreId={1}", terminal.date_val, terminal.StoreId).ToList();

                        List<int> drpOtherList = (from a in _context.SalesOtherDeposites where a.SalesGeneralId == closeOut.SalesGeneralEntries select a.SalesOtherDepositeId).ToList();
                        for (int i = 0; i < drpOtherList.Count; i++)
                        {
                            SalesOtherDeposite otherdata = _context.SalesOtherDeposites.Find(drpOtherList[i]);
                            _context.SalesOtherDeposites.Remove(otherdata);
                            _context.SaveChanges();
                        }
                        if (OtherDepositList.Count() > 0)
                        {
                            if (OtherDepositList[0] != null)
                            {
                                foreach (var item in OtherDepositList)
                                {
                                    SalesOtherDeposite obj = new SalesOtherDeposite();
                                    obj.SalesGeneralId = Convert.ToInt32(closeOut.SalesGeneralEntries);
                                    obj.StoreId = terminal.StoreId;
                                    obj.OtherDepositId = item.OtherDepositId;
                                    obj.OtherDepositDate = Convert.ToDateTime(terminal.date_val);
                                    obj.Status = 0;
                                    _context.SalesOtherDeposites.Add(obj);
                                    _context.SaveChanges();
                                }
                            }
                        }

                        terminal.ErrorMessage = "Day Close Out Successful";
                        var DayCount = _context.Database.SqlQuery<DayCloseOutStatus>("GetDayCloseOutStatus_Data @StartDate={0},@Storeid={1}", terminal.date_val, Convert.ToInt32(terminal.StoreId)).FirstOrDefault();
                        if (DayCount != null)
                        {
                            DayCloseOutStatus obj = _context.dayCloseOutStatuses.Find(DayCount.DayCloseOutId);
                            if (obj != null)
                            {
                                obj.DayCloseOutCount = 1;
                                _context.Entry(obj).State = EntityState.Modified;
                                _context.SaveChanges();
                                obj = null;
                            }
                            else
                            {
                                DayCloseOutStatus objDay = new DayCloseOutStatus();
                                objDay.StartDate = Convert.ToDateTime(terminal.date_val);
                                objDay.StoreId = terminal.StoreId;
                                objDay.DayCloseOutCount = 1;
                                _context.dayCloseOutStatuses.Add(objDay);
                                _context.SaveChanges();
                            }
                        }
                        else
                        {
                            DayCloseOutStatus objDay = new DayCloseOutStatus();
                            objDay.StartDate = Convert.ToDateTime(terminal.date_val);
                            objDay.StoreId = terminal.StoreId;
                            objDay.DayCloseOutCount = 1;
                            _context.dayCloseOutStatuses.Add(objDay);
                            _context.SaveChanges();
                        }

                        List<JournalEntriesList> JournalEntriesLists = _context.salesChildEntries.Where(s => s.SalesGeneralId == closeOut.SalesGeneralEntries).ToList().
                                     Select(dt => new JournalEntriesList
                                     {
                                         id = closeOut.SalesGeneralEntries,
                                         TotalAmount = (dt.Amount < 0 ? dt.Amount * -1 : dt.Amount),
                                         Typeid = (dt.Amount < 0 ? (Convert.ToInt32(dt.TypeId) == 1 ? 2 : 1) : dt.TypeId),
                                         Sign = (dt.Amount < 0 ? 2 : 1)
                                     }).ToList();


                        decimal? Debited = JournalEntriesLists.Where(o => (o.Typeid == 2) || (o.Sign == 2 && o.Typeid != 1)).Sum(s => s.TotalAmount);
                        decimal? Creadited = JournalEntriesLists.Where(o => (o.Typeid == 1)).Sum(s => s.TotalAmount);
                        decimal done = Convert.ToDecimal(Debited - Creadited);
                        if (done == 0)
                        {
                            SalesGeneralEntries Data = _context.salesGeneralEntries.Find(closeOut.SalesGeneralEntries);

                            terminal.Store = _qBRepository.GetStoreOnlineDesktop(Convert.ToInt32(Data.StoreId));
                            terminal.StoreFlag = _qBRepository.GetStoreOnlineDesktopFlag(Convert.ToInt32(Data.StoreId));
                            if (terminal.Store != "")
                            {
                                if (terminal.Store == "Online" && terminal.StoreFlag == 1)
                                {
                                    if (Data.Status == null || Data.Status == 0)
                                    {
                                        JournalEntry MainDetail = new JournalEntry();
                                        MainDetail.ID = Data.SalesGeneralId.ToString();
                                        MainDetail.storeid = Convert.ToInt32(Data.StoreId);
                                        MainDetail.userid = Convert.ToInt32(Data.UserId);
                                        MainDetail.salesdate = Data.SalesDate;
                                        MainDetail.Createddate = Data.CreatedDate;
                                        MainDetail.status = Convert.ToInt32(Data.Status);
                                        MainDetail.syncstatus = Convert.ToInt32(Data.SyncStatus);
                                        MainDetail.totalamount = Data.TotalAmount;
                                        MainDetail.noofpos = Convert.ToInt32(Data.NoOfPos);

                                        List<JournalDetail> Detail = new List<JournalDetail>();
                                        var Det = _context.Database.SqlQuery<JournalEntryDetail>("GeneralEntryDetail @SalesGeneralId={0}", Data.SalesGeneralId);
                                        if (Det != null)
                                        {
                                            foreach (var item in Det)
                                            {
                                                JournalDetail obj = new JournalDetail();
                                                obj.id = item.SalesChildId.ToString();
                                                obj.Gid = Data.SalesGeneralId;
                                                obj.storeid = Data.StoreId;
                                                obj.groupid = Convert.ToInt32(item.GroupAccountId);
                                                obj.Groupname = item.DepartmentName;
                                                obj.Amount = (item.Amount < 0 ? item.Amount * -1 : item.Amount);
                                                obj.Typeid = (item.Amount < 0 ? (Convert.ToInt32(item.TypeId) == 1 ? 2 : 1) : Convert.ToInt32(item.TypeId));
                                                obj.Memo = item.Memo;
                                                obj.Title = item.Title;
                                                obj.ListID = item.DepartmentListID;
                                                obj.EntityID = item.EntityID;
                                                obj.EntityType = item.EntityType;
                                                Detail.Add(obj);
                                                obj = null;
                                            }
                                        }

                                        QBResponse objResponse = new QBResponse();
                                        QBOnlineconfiguration objOnlieDetail = _qBRepository.GetConfigDetail(Convert.ToInt32(Data.StoreId));
                                        QBJournalEntry.CreateJournalEntry(MainDetail, Detail, ref objResponse, objOnlieDetail);
                                        if (objResponse.ID != "0" || objResponse.Status == "Done")
                                        {
                                            Data.TxnId = objResponse.ID;
                                            Data.IsSync = 1;
                                            Data.Status = 3;
                                            Common.WriteErrorLogs("Journal Entry Created : " + objResponse.ID);
                                            _context.Entry(Data).State = EntityState.Modified;
                                            _context.SaveChanges();
                                        }
                                        else
                                        {
                                            Common.WriteErrorLogs(objResponse.Status + ": " + objResponse.Message);
                                        }

                                        var datas = _context.Database.SqlQuery<ChildOtherDepositeList>("GetChildOtherDeposite @SalesGeneralId={0}", closeOut.SalesGeneralEntries);
                                        if (datas != null)
                                        {
                                            foreach (var item in datas)
                                            {
                                                Deposit objDep = new Deposit();
                                                objDep.TxnDate = item.OtherDepositDate;
                                                objDep.Memo = item.Memo;
                                                objDep.DepositAccID = item.BankAccountID;

                                                List<DepositDetail> objDetailList = new List<DepositDetail>();
                                                DepositDetail objDetail = new DepositDetail();
                                                objDetail.Amount = item.Amount;
                                                objDetail.Description = item.Memo;
                                                objDetail.EntityID = item.VendorListID;
                                                objDetail.AccountID = item.DepartmentListId;
                                                objDetail.PaymentMethod = item.PaymentTypeID;
                                                objDetail.EntityType = "Vendor";
                                                objDetailList.Add(objDetail);
                                                QBResponse objResponse1 = new QBResponse();
                                                QBDeposit.CreateDepositeEntry(objDep, objDetailList, ref objResponse1, objOnlieDetail);
                                                objDetailList = null;
                                                if (objResponse1.ID != "0" || objResponse1.Status == "Done" || objResponse1.ID != "")
                                                {
                                                    DBContext db1 = new DBContext();
                                                    SalesOtherDeposite Datas = db1.SalesOtherDeposites.Find(item.SalesOtherDepositeId);
                                                    Datas.TxnId = objResponse1.ID;
                                                    Datas.IsSync = 1;
                                                    Datas.Status = 3;
                                                    AdminSiteConfiguration.WriteErrorLogs("Deposite Entry Created : " + objResponse1.ID);

                                                    db1.Entry(Datas).State = EntityState.Modified;
                                                    db1.SaveChanges();
                                                    db1 = null;
                                                }
                                                else
                                                {
                                                    AdminSiteConfiguration.WriteErrorLogs(objResponse.Status + ": " + objResponse1.Message);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
                else
                {
                    terminal.ErrorMessage = "Group Name is not Configured.";
                    if (closeOut.PendingDayShift != "")
                    {
                        terminal.ErrorMessage += "<br/>" + closeOut.PendingDayShift;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - SetDayCloseOut - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return terminal.ErrorMessage;
        }

        public List<TenderAccountsStoreWise> getTenderAccountsStoreWise(TerminalViewModel terminal)
        {
            List<TenderAccountsStoreWise> AccList = new List<TenderAccountsStoreWise>();
            try
            {
                AccList = _context.Database.SqlQuery<TenderAccountsStoreWise>("SP_GetTenderAccountStoreWise @Storeid={0}", terminal.StoreId).ToList().Where(a => a.GroupId == null && a.DepartmentId == null && a.Flag == 1).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - getTenderAccountsStoreWise - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return AccList;
        }

        public void DeleteTenderEntry(TerminalViewModel terminal)
        {
            try
            {
                TenderInDrawer Data = _context.TenderInDrawers.Find(terminal.Id);
                _context.TenderInDrawers.Remove(Data);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - DeleteTenderEntry - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void DeleteotherdepositFile(TerminalViewModel terminal)
        {
            try
            {
                OtherDeposit otherdeposit = _context.OtherDeposits.Find(terminal.Id);
                if (otherdeposit != null)
                {
                    otherdeposit.UploadDocument = "";
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - DeleteotherdepositFile - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public string GetSalesGeneralEntries_Data(TerminalViewModel terminal)
        {
            SalesGeneralEntries Data = new SalesGeneralEntries();
            try
            {
                Data = _context.Database.SqlQuery<SalesGeneralEntries>("GetSalesGeneralEntries_Data @Storeid={0},@SalesDate={1}", terminal.StoreId, terminal.SDate).FirstOrDefault();
                if (Data != null)
                {
                    terminal.ErrorMessage = Data.Status.ToString();
                }
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - GetSalesGeneralEntries_Data - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return terminal.ErrorMessage;
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
                logger.Error("TerminalRepository - GetStoreList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return StoreList;
        }
        public CreateInvoiceDetail SetCaseInvoice(TerminalViewModel terminal)
        {
            CreateInvoiceDetail invoiceDetai = new CreateInvoiceDetail();
            try
            {
                int StoreId = terminal.StoreId;
                invoiceDetai.DepartmentMasters = _context.DepartmentMasters.Where(s => s.IsActive == true && s.DepartmentId == 0).ToList();
                if (terminal.StoreId != 0)
                {
                    invoiceDetai.StoreMasters = GetStoreList(1);
                    invoiceDetai.VendorMasters = _context.VendorMasters.Where(s => s.StoreId == StoreId && s.IsActive == true).OrderBy(o => o.VendorName).ToList();
                    invoiceDetai.DepartmentMasters_store = _context.DepartmentMasters.Where(s => s.IsActive == true && s.StoreId == StoreId).OrderBy(o => o.DepartmentName).ToList();
                }
                else
                {
                    invoiceDetai.StoreMasters = GetStoreList(1);
                }
                invoiceDetai.DiscountTypeMasters = _context.DiscountTypeMasters.ToList();
                invoiceDetai.InvoiceTypeMasters = _context.InvoiceTypeMasters.ToList();
                invoiceDetai.PaymentTypeMasters = _context.PaymentTypeMasters.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - SetCaseInvoice - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return invoiceDetai;
        }

        public CreateInvoiceDetail UploadFileExtantionInvalid(TerminalViewModel terminal)
        {
            CreateInvoiceDetail invoiceDetai = new CreateInvoiceDetail();
            try
            {
                invoiceDetai.DepartmentMasters = _context.Database.SqlQuery<DepartmentMaster>("SP_DepartmentMaster @Mode = {0},@StoreId = {1}", "SelectExpense_Department", terminal.invoice.StoreId).ToList().Where(s => s.StoreId == terminal.invoice.StoreId).ToList();
                invoiceDetai.StoreMasters = GetStoreList(1);
                invoiceDetai.VendorMasters = _context.VendorMasters.Where(s => s.StoreId == terminal.invoice.StoreId && s.IsActive == true).Select(s => new EntityModels.Models.VendorMaster { VendorId = s.VendorId, VendorName = s.VendorName }).ToList();
                invoiceDetai.DepartmentMasters_store = _context.DepartmentMasters.Where(s => s.IsActive == true && s.StoreId == terminal.StoreId).OrderBy(o => o.DepartmentName).ToList();
                invoiceDetai.DiscountTypeMasters = _context.DiscountTypeMasters.ToList();
                invoiceDetai.InvoiceTypeMasters = _context.InvoiceTypeMasters.ToList();
                invoiceDetai.PaymentTypeMasters = _context.PaymentTypeMasters.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - UploadFileExtantionInvalid - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return invoiceDetai;
        }

        public async Task<TerminalViewModel> CreateInvoice(TerminalViewModel terminal)
        {
            TerminalViewModel model = terminal;
            try
            {
                if (model.Store == "Online" && model.StoreFlag == 1 && model.invoice.PaymentTypeId == 2) //Check/ACH
                {
                    if (model.invoice.QBTransfer == false)
                    {
                        try
                        {
                            if (model.invoice.strInvoiceDate != null)
                            {
                                model.invoice.InvoiceDate = Convert.ToDateTime(model.invoice.strInvoiceDate);
                            }
                            model.invoice.StatusValue = InvoiceStatusEnm.Approved;
                            model.invoice.CreatedBy = _context.UserMasters.Where(s => s.UserName == model.name).FirstOrDefault().UserId;
                            model.invoice.CreatedOn = DateTime.Now;
                            model.invoice.ApproveRejectBy = _context.UserMasters.Where(s => s.UserName == model.name).FirstOrDefault().UserId;
                            model.invoice.ApproveRejectDate = DateTime.Now;
                            model.invoice.IsActive = true;
                            _context.Invoices.Add(model.invoice);
                            await _context.SaveChangesAsync();
                            model.InvoiceId = model.invoice.InvoiceId;
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }
                    }
                    else
                    {
                        try
                        {
                            model.invoice.CreatedBy = _context.UserMasters.Where(s => s.UserName == model.name).FirstOrDefault().UserId;
                            model.invoice.CreatedOn = DateTime.Now;
                            model.invoice.Source = "WEB";
                            model.invoice.StatusValue = InvoiceStatusEnm.Approved;
                            model.invoice.ApproveRejectBy = _context.UserMasters.Where(s => s.UserName == model.name).FirstOrDefault().UserId;
                            model.invoice.ApproveRejectDate = DateTime.Now;
                            model.invoice.IsActive = true;
                            _context.Invoices.Add(model.invoice);
                            await _context.SaveChangesAsync();
                            model.InvoiceId = model.invoice.InvoiceId;
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                        }
                    }
                }
                else
                {
                    try
                    {
                        model.invoice.CreatedBy = _context.UserMasters.Where(s => s.UserName == model.name).FirstOrDefault().UserId;
                        model.invoice.CreatedOn = DateTime.Now;
                        model.invoice.Source = "WEB";
                        model.invoice.StatusValue = InvoiceStatusEnm.Approved;
                        model.invoice.ApproveRejectBy = _context.UserMasters.Where(s => s.UserName == model.name).FirstOrDefault().UserId;
                        model.invoice.ApproveRejectDate = DateTime.Now;
                        model.invoice.IsActive = true;
                        _context.Invoices.Add(model.invoice);
                        await _context.SaveChangesAsync();
                        model.InvoiceId = model.invoice.InvoiceId;
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - CreateInvoice - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return model;
        }

        public async Task<TerminalViewModel> CreateInvoiceWithoutQuickInvoice(TerminalViewModel terminal)
        {
            TerminalViewModel model = terminal;
            try
            {
                model.invoice.CreatedBy = _context.UserMasters.Where(s => s.UserName == model.name).FirstOrDefault().UserId;
                model.invoice.CreatedOn = DateTime.Now;
                model.invoice.Source = "WEB";
                model.invoice.StatusValue = InvoiceStatusEnm.Pending;
                model.invoice.IsActive = true;
                _context.Invoices.Add(model.invoice);
                await _context.SaveChangesAsync();
                model.InvoiceId = model.invoice.InvoiceId;
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - CreateInvoiceWithoutQuickInvoice - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return model;
        }

        public async Task<TerminalViewModel> UpdateInvoice(TerminalViewModel terminal)
        {
            TerminalViewModel model = terminal;
            try
            {
                SalesActivitySummary salesActivity = _context.SalesActivitySummaries.Find(model.ShiftID);
                CashPaidoutInvoice objC = new CashPaidoutInvoice();
                objC.InvoiceId = Convert.ToInt32(model.InvoiceId);
                objC.StoreTerminalId = Convert.ToInt32(model.TerminalId);
                objC.ShiftId = salesActivity.ShiftId;
                objC.PaidOutId = model.PaidOutID;
                objC.StoreId = model.invoice.StoreId;
                objC.CreatedDate = DateTime.Now;
                objC.SalesActivitySummaryId = model.ShiftID;
                _context.CashPaidoutInvoices.Add(objC);
                await _context.SaveChangesAsync();
                objC = null;

                if (model.ChildDepartmentId != null)
                {
                    int j = 0;
                    foreach (var val_id in model.ChildDepartmentId)
                    {
                        if (!String.IsNullOrEmpty(model.ChildAmount[j].ToString()))
                        {
                            InvoiceDepartmentDetail deptDetail = new InvoiceDepartmentDetail();
                            deptDetail.InvoiceId = model.InvoiceId;
                            deptDetail.DepartmentId = Convert.ToInt32(val_id);
                            deptDetail.Amount = Convert.ToDecimal(model.ChildAmount[j]);
                            _context.InvoiceDepartmentDetails.Add(deptDetail);
                            await _context.SaveChangesAsync();
                        }
                        j++;
                    }
                }

                if (model.invoice.QuickInvoice == "1")
                {
                    model.ActivityLogMessage = "Quick Invoice Number " + "<a href='/Invoices/Details/" + model.invoice.InvoiceId + "'>" + model.invoice.InvoiceNumber + "</a> Created by " + UserModule.getUserFirstName() + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                }
                else
                {
                    model.ActivityLogMessage = "Invoice Number " + "<a href='/Invoices/Details/" + model.invoice.InvoiceId + "'>" + model.invoice.InvoiceNumber + "</a> Created by " + UserModule.getUserFirstName() + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                }

                ActivityLog ActLog1 = new ActivityLog();
                ActLog1.Action = 1;
                ActLog1.Comment = model.ActivityLogMessage;
                _activityLogRepository.ActivityLogInsert(ActLog1);
                if (model.invoice.QuickInvoice == "1")
                {
                    model.StatusMessage = "Success1";
                }
                else
                {
                    model.StatusMessage = "Success";
                }
                if (model.invoice.DiscountTypeId != 1)
                {
                    _context.Database.ExecuteSqlCommand("SP_InvoiceDiscount @Mode = {0},@InvoiceId = {1},@DiscountTypeId = {2},@DiscountAmount = {3}, @DiscountPercent = {4}", "UpdateDiscountDetail", model.InvoiceId, model.invoice.DiscountTypeId, (model.invoice.DiscountAmount == null ? 0 : model.invoice.DiscountAmount), (model.invoice.DiscountPercent == null ? 0 : model.invoice.DiscountPercent));

                    int iiInvoiceID = 0;

                    if (model.UploadInvoice != null)
                    {
                        if (model.UploadInvoice.ContentLength > 0)
                        {
                            var allowedExtensions = new[] { ".pdf" };
                            var extension = Path.GetExtension(model.UploadInvoice.FileName);
                            var Ext = Convert.ToString(extension).ToLower();
                            if (!allowedExtensions.Contains(Ext))
                            {
                                model.StatusMessage = "InvalidImage";
                                return model;
                            }
                            else
                            {
                                model.DSacn_Title = AdminSiteConfiguration.GetRandomNo() + Path.GetFileName(model.UploadInvoice.FileName);
                                model.DSacn_Title = AdminSiteConfiguration.RemoveSpecialCharacter(model.DSacn_Title);
                                var path1 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "UserFiles\\Invoices" + "\\" + model.DSacn_Title;
                                model.UploadInvoice.SaveAs(path1);
                            }
                        }
                    }

                    string CRdiscount = "";
                    string Credit_Invoiceno = model.invoice.InvoiceNumber + "_cr";
                    if (model.invoice.DiscountTypeId == 2)
                    {
                        CRdiscount = model.invoice.DiscountPercent.ToString();
                        Credit_Invoiceno = Credit_Invoiceno + CRdiscount + "%";
                    }

                    if (model.invoice.QuickInvoice == "1" && (Roles.IsUserInRole("Administrator") || Roles.IsUserInRole("ApproveInvoice")))
                    {
                        if (model.Store != "")
                        {
                            if (model.Store == "Online" && model.StoreFlag == 1 && model.invoice.PaymentTypeId == 2)
                            {
                                try
                                {
                                    Invoice InvObj = _context.Invoices.AsNoTracking().FirstOrDefault(a => a.InvoiceId == model.InvoiceId);
                                    InvObj.strInvoiceDate = InvObj.InvoiceDate.ToString("dd/MM/yyyy");
                                    InvObj.CreatedBy = _context.UserMasters.Where(s => s.UserName == model.name).FirstOrDefault().UserId;
                                    InvObj.CreatedOn = DateTime.Now;
                                    InvObj.Source = "WEB";
                                    InvObj.StatusValue = InvoiceStatusEnm.Approved;
                                    model.invoice.ApproveRejectBy = _context.UserMasters.Where(s => s.UserName == model.name).FirstOrDefault().UserId;
                                    model.invoice.ApproveRejectDate = DateTime.Now;
                                    InvObj.RefInvoiceId = model.InvoiceId;
                                    InvObj.InvoiceTypeId = 2; //CreditMemo
                                    InvObj.PaymentTypeId = 2; //Check/ACH
                                    InvObj.DiscountTypeId = 1;
                                    InvObj.StatusValue = InvoiceStatusEnm.Approved;

                                    InvObj.TotalAmount = InvObj.DiscountAmount != null ? model.invoice.DiscountAmount.Value : InvObj.TotalAmount;
                                    InvObj.DiscountAmount = 0;
                                    InvObj.DiscountPercent = 0;
                                    InvObj.InvoiceNumber = Credit_Invoiceno;
                                    model.invoice.IsActive = true;
                                    InvObj.UploadInvoice = model.DSacn_Title;
                                    _context.Invoices.Add(InvObj);
                                    await _context.SaveChangesAsync();
                                    iiInvoiceID = InvObj.InvoiceId;

                                    if (Convert.ToInt32(iiInvoiceID) > 0)
                                    {
                                        InvoiceDepartmentDetail deptDetail = new InvoiceDepartmentDetail();
                                        deptDetail.InvoiceId = iiInvoiceID;
                                        deptDetail.DepartmentId = Convert.ToInt32(model.invoice.Disc_Dept_id);
                                        deptDetail.Amount = Convert.ToDecimal(InvObj.DiscountAmount != null ? model.invoice.DiscountAmount.Value : InvObj.TotalAmount);
                                        _context.InvoiceDepartmentDetails.Add(deptDetail);
                                        await _context.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        Invoice iiInvoice = _context.Invoices.Find(iiInvoiceID);
                                        iiInvoice.Source = "WEB";
                                        _context.Entry(iiInvoice).State = EntityState.Modified;
                                        await _context.SaveChangesAsync();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logger.Error("TerminalRepository - UpdateInvoicePartially - " + DateTime.Now + " - " + ex.Message.ToString());
                                }
                            }
                            else
                            {
                                Invoice CreditMemo = new Invoice();
                                CreditMemo = _context.Invoices.AsNoTracking().FirstOrDefault(a => a.InvoiceId == model.InvoiceId);
                                CreditMemo.strInvoiceDate = CreditMemo.InvoiceDate.ToString("dd/MM/yyyy");
                                CreditMemo.InvoiceTypeId = 2; //CreditMemo
                                CreditMemo.StatusValue = InvoiceStatusEnm.Pending;
                                CreditMemo.PaymentTypeId = 2; //Check/ACH
                                CreditMemo.DiscountTypeId = 1; //N/A
                                CreditMemo.TotalAmount = model.invoice.DiscountAmount != null ? model.invoice.DiscountAmount.Value : model.invoice.TotalAmount;
                                CreditMemo.DiscountAmount = 0;
                                CreditMemo.DiscountPercent = 0;
                                CreditMemo.RefInvoiceId = model.InvoiceId;
                                CreditMemo.UploadInvoice = model.DSacn_Title;
                                CreditMemo.InvoiceNumber = Credit_Invoiceno;
                                CreditMemo.CreatedBy = _context.UserMasters.Where(s => s.UserName == model.name).FirstOrDefault().UserId;
                                CreditMemo.CreatedOn = DateTime.Now;
                                CreditMemo.Source = "WEB";
                                CreditMemo.IsActive = true;
                                _context.Invoices.Add(CreditMemo);
                                await _context.SaveChangesAsync();

                                if (CreditMemo.InvoiceId > 0)
                                {
                                    InvoiceDepartmentDetail deptDetail1 = new InvoiceDepartmentDetail();
                                    deptDetail1.InvoiceId = CreditMemo.InvoiceId;
                                    deptDetail1.DepartmentId = Convert.ToInt32(model.invoice.Disc_Dept_id);
                                    deptDetail1.Amount = Convert.ToDecimal(model.invoice.DiscountAmount != null ? model.invoice.DiscountAmount.Value : model.invoice.TotalAmount);
                                    _context.InvoiceDepartmentDetails.Add(deptDetail1);
                                    await _context.SaveChangesAsync();
                                }
                            }
                        }
                        else
                        {
                            model.invoice.InvoiceTypeId = 2;
                            model.invoice.StatusValue = InvoiceStatusEnm.Pending;
                            model.invoice.PaymentTypeId = 2; //Check/ACH
                            model.invoice.DiscountTypeId = 1; //N/A
                            model.invoice.InvoiceNumber = Credit_Invoiceno;
                            model.invoice.CreatedBy = _context.UserMasters.Where(s => s.UserName == model.name).FirstOrDefault().UserId;
                            model.invoice.CreatedOn = DateTime.Now;
                            model.invoice.RefInvoiceId = model.InvoiceId;
                            model.invoice.Source = "WEB";
                            model.invoice.IsActive = true;
                            _context.Invoices.Add(model.invoice);
                            await _context.SaveChangesAsync();
                            iiInvoiceID = model.invoice.InvoiceId;
                        }
                    }
                    else
                    {
                        Invoice InvObj = _context.Invoices.AsNoTracking().FirstOrDefault(a => a.InvoiceId == model.InvoiceId);
                        InvObj.strInvoiceDate = model.invoice.InvoiceDate.ToString("dd/MM/yyyy");
                        InvObj.InvoiceTypeId = 2;
                        InvObj.PaymentTypeId = 2; //Check/ACH
                        InvObj.InvoiceNumber = Credit_Invoiceno;
                        InvObj.StatusValue = InvoiceStatusEnm.Pending;
                        InvObj.DiscountTypeId = 1; //N/A
                        InvObj.CreatedBy = _context.UserMasters.Where(s => s.UserName == model.name).FirstOrDefault().UserId;
                        InvObj.CreatedOn = DateTime.Now;
                        InvObj.RefInvoiceId = model.InvoiceId;
                        InvObj.Source = "WEB";
                        InvObj.IsActive = true;
                        InvObj.TotalAmount = Convert.ToDecimal(model.invoice.DiscountAmount != null ? model.invoice.DiscountAmount.Value : model.invoice.TotalAmount);
                        _context.Invoices.Add(InvObj);
                        await _context.SaveChangesAsync();
                        iiInvoiceID = InvObj.InvoiceId;

                        InvoiceDepartmentDetail deptDetail = new InvoiceDepartmentDetail();
                        deptDetail.InvoiceId = iiInvoiceID;
                        deptDetail.DepartmentId = Convert.ToInt32(model.invoice.Disc_Dept_id);
                        deptDetail.Amount = Convert.ToDecimal(model.invoice.DiscountAmount != null ? model.invoice.DiscountAmount.Value : InvObj.TotalAmount);
                        _context.InvoiceDepartmentDetails.Add(deptDetail);
                        await _context.SaveChangesAsync();
                    }

                    if (iiInvoiceID > 0)
                    {
                        if (model.invoice.QuickInvoice == "1")
                        {
                            model.ActivityLogMessage = "Quick Invoice Number " + "<a href='/Invoices/Details/" + model.invoice.InvoiceId + "'>" + model.invoice.InvoiceNumber + "</a> Created by " + UserModule.getUserFirstName() + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                        }
                        else
                        {
                            model.ActivityLogMessage = "Invoice Number " + "<a href='/Invoices/Details/" + model.invoice.InvoiceId + "'>" + model.invoice.InvoiceNumber + "</a> Created by " + UserModule.getUserFirstName() + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                        }

                        ActivityLog ActLog2 = new ActivityLog();
                        ActLog2.Action = 1;
                        ActLog2.Comment = model.ActivityLogMessage;
                        _activityLogRepository.ActivityLogInsert(ActLog2);
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - UpdateInvoice - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return model;
        }

        public string GetVendorName(int VendorId)
        {
            string Name = "";
            try
            {
                Name = _context.VendorMasters.Find(VendorId).VendorName;
                Name = AdminSiteConfiguration.RemoveSpecialCharacterInvoice(Name);
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - GetVendorName - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Name;
        }

        public string DeleteCashInvoice(int Id)
        {
            string message = "";
            try
            {
                _context.Database.ExecuteSqlCommand("SP_Invoice @Mode = {0},@InvoiceId = {1}", "DeleteFromCashPaidoutInvoice", Id);
                Invoice invoice = _context.Invoices.Find(Id);
                string InvoinceNo = invoice.InvoiceNumber;
                _context.Invoices.Remove(invoice);
                _context.SaveChangesAsync();

                ActivityLog ActLog = new ActivityLog();
                ActLog.Action = 3;
                ActLog.Comment = "CashInvoice " + InvoinceNo + " Deleted by " + UserModule.getUserFirstName() + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                _activityLogRepository.ActivityLogInsert(ActLog);
                message = "sucess";
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - DeleteCashInvoice - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return message;
        }

        public Invoice GetInvoiceByID(int InvoiceID)
        {
            Invoice invoice = new Invoice();
            try
            {
                invoice = _context.Invoices.Where(a => a.InvoiceId == InvoiceID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - GetInvoiceByID - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return invoice;
        }

        public SalesOtherDeposite GetSalesOtherDepositeByID(int OtherDepositeID)
        {
            SalesOtherDeposite salesOther = new SalesOtherDeposite();
            try
            {
                salesOther = _context.SalesOtherDeposites.Where(a => a.OtherDepositId == OtherDepositeID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - GetSalesOtherDepositeByID - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return salesOther;
        }

        public DateTime? GetPreviousDayCount(TerminalViewModel terminal)
        {
            int Count = 0;
            DateTime previousDate = terminal.SDate.AddDays(-1);
            try
            {
                Count = _context.salesGeneralEntries.Where(x => x.SalesDate == previousDate && x.StoreId == terminal.StoreId).Count();
                if(Count > 0)
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - GetPreviousDayCount - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return previousDate;
        }

        public string GetPaidOutAmountMessage(TerminalViewModel terminal)
        {
            string result = "";
            try
            {
                var data = _context.Database.SqlQuery<TerminalViewModel>("SP_CheckPaidOutInvoiceAmount @Mode={0}, @StoreTerminalIds={1}, @StoreId={2},@StartDate={3}", "GetMessagePaidAmountCheck", terminal.Terminalid_val, terminal.StoreId,terminal.SDate).FirstOrDefault();
                if (data != null) {
                    result = data.ErrorMessage;
                }
                else
                {
                    result = "";
                }
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - GetPaidOutAmountMessage - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return result;
        }

        public string GetCCOfflineAmountMessage(TerminalViewModel terminal)
        {
            string result = "";
            try
            {
                var data = _context.Database.SqlQuery<TerminalViewModel>("SP_CheckCCOfflineAmount @Mode={0}, @StoreTerminalIds={1}, @StoreId={2},@StartDate={3}", "GetMessageCCOfflineAmountCheck", terminal.Terminalid_val, terminal.StoreId, terminal.SDate).FirstOrDefault();
                if (data != null)
                {
                    result = data.ErrorMessage;
                }
                else
                {
                    result = "";
                }
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - GetCCOfflineAmountMessage - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return result;
        }

        public string GetLastThirtydaysClosedOut(TerminalViewModel terminal)
        {
            string result = "";
            try
            {
                var data = _context.Database.SqlQuery<TerminalViewModel>("SP_CheckCCOfflineAmount @Mode={0}, @StoreId={1},@StartDate={2}", "GetLastThirtydaysClosedOutDate", terminal.StoreId, terminal.SDate).FirstOrDefault();
                if (data.ErrorMessage != null)
                {
                    result = data.ErrorMessage;
                }
                else
                {
                    result = "";
                }
            }
            catch (Exception ex)
            {
                logger.Error("TerminalRepository - GetLastThirtydaysClosedOut - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return result;
        }


    }
}
