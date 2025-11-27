using EntityModels.Models;
using NLog;
using Repository.IRepository;
using SynthesisViewModal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class DashboardRepository : IDashboardRepository
    {
        private DBContext _context;
        private DBContextHR _context2;
        private readonly ICommonRepository _ICommonRepositoryRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        DashboardViewModel data = new DashboardViewModel();

        public DashboardRepository(DBContext context, DBContextHR context2)
        {
            _context = context;
            _context2 = context2;
        }

        public List<string> CheckError(int UserTypeID, int StoreID)
        {
            List<string> list = new List<string>();
            try
            {
                if (UserTypeID != 1)
                {
                    if (StoreID == 0)
                    {
                        list = _context.userRoles.Where(s => s.UserTypeId == UserTypeID).Select(s => s.Role).ToList();

                    }
                    else
                    {
                        list = _context.userRoles.Where(s => s.UserTypeId == UserTypeID && s.StoreId == StoreID).Select(s => s.Role).ToList();

                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - CheckError - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return list;
        }

        /// <summary>
        /// Get accessible stores from UserRoles table by UserTypeId
        /// </summary>
        /// <param name="UserTypeId">UserType ID</param>
        /// <returns>List of accessible store IDs</returns>
        public List<int> GetUserAccessibleStoresByUserType(int UserTypeId)
        {
            List<int> accessibleStores = new List<int>();
            try
            {
                // Get distinct store IDs from UserRoles table for this UserTypeId
                accessibleStores = _context.userRoles
                    .Where(s => s.UserTypeId == UserTypeId && s.StoreId != null && s.StoreId > 0)
                    .Select(s => s.StoreId.Value)
                    .Distinct()
                    .ToList();
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetUserAccessibleStoresByUserType - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return accessibleStores;
        }

        public string Startdate(int StoreID)
        {
            string startdt = "";
            try
            {
                startdt = _context.SalesActivitySummaryHourlies.Where(s => s.StoreId == StoreID).Count() > 0 ? _context.SalesActivitySummaryHourlies.Where(s => s.StoreId == StoreID).Max(s => s.StartDate).Value.ToString("MM-dd-yyyy") : DateTime.Today.ToString("MM-dd-yyyy");
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - Startdate - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return startdt;
        }

        public DashboardDaily Dashboard_Daily(string Startdate, int StoreID, int? StateId, int? GroupID)
        {
            DashboardDaily obj = new DashboardDaily();
            try
            {
                List<DashboardDailySelect> Dashboard = _context.Database.SqlQuery<DashboardDailySelect>("SP_Dashboard_Daily @Date = {0},@StoreID={1},@StateId={2},@GroupID={3}", Startdate, StoreID, StateId, GroupID).ToList();

                if (Dashboard.Count() > 0)
                {
                    DashboardDailySelect obj2 = new DashboardDailySelect();
                    obj2 = Dashboard.FirstOrDefault();
                    obj.AllVoidsAmt = obj2.AllVoidsAmt;
                    obj.ItemCorrectsAmt = obj2.ItemCorrectsAmt;
                    obj.ItemReturnsAmt = obj2.ItemReturnsAmt;
                    obj.AllVoids = obj2.AllVoids;
                    obj.ItemCorrects = obj2.ItemCorrects;
                    obj.ItemReturns = obj2.ItemReturns;
                    obj.CashPayout = obj2.CashPayout;
                    obj.Over = obj2.Over;
                    obj.TotalSalesCurrentDay = obj2.TotalSalesCurrentDay;
                    obj.SalesGrowth = obj2.SalesGrowth;
                    obj.CustomerCountCurrentDay = obj2.CustomerCountCurrentDay;
                    obj.CustomerCountGrowth = obj2.CustomerCountGrowth;
                    obj.AverageSaleCurrentDay = obj2.AverageSaleCurrentDay;
                    obj.AveragesalesGrowth = obj2.AveragesalesGrowth;
                    obj.TotalSalesLastWeek = obj2.TotalSalesLastWeek;
                    obj.CustomerCountLastWeek = obj2.CustomerCountLastWeek;
                    obj.AverageSalesLastWeek = obj2.AverageSalesLastWeek;
                    obj.TotalCash = obj2.TotalCash;
                    obj.Offline = obj2.Offline;
                    obj.Online = obj2.Online;
                    obj.InvoicesAdded = obj2.InvoicesAdded;
                    obj.InvoicesApproved = obj2.InvoicesApproved;
                    obj.DayName = obj2.DayName;
                    obj.HourTime = obj2.HourTime;
                }

                obj.StoreList = _context.Database.SqlQuery<DropdownList>("SP_ChartStoreDepartmentData @Mode={0},@StateId={1}", "1", StateId).ToList();

                obj.DepartmentList = _context.Database.SqlQuery<DropdownDepartmentList>("SP_ChartStoreDepartmentData @Mode={0},@StateId={1}", "2", StateId).ToList();
                if (obj.DepartmentList != null)
                {
                    var index = obj.DepartmentList.FindAll(item => item.Text.ToUpper() == "ICE CREAM");
                    if (index != null)
                    {
                        if (index.Count > 0)
                        {
                            var itemtoremove = obj.DepartmentList.Where(item => item.Text.ToUpper() == "ICE CREAM").First();
                            obj.DepartmentList.Remove(itemtoremove);
                        }
                    }

                    var index1 = obj.DepartmentList.FindAll(item => item.Text.ToUpper() == "CATERING");
                    if (index1 != null)
                    {
                        if (index1.Count > 0)
                        {
                            var itemtoremove = obj.DepartmentList.Where(item => item.Text.ToUpper() == "CATERING").First();
                            obj.DepartmentList.Remove(itemtoremove);
                        }
                    }
                }
                obj.PayrollDepartmentList = _context.Database.SqlQuery<DropdownDepartmentList>("SP_ChartStoreDepartmentData @Mode={0},@StateId={1}", "3", StateId).ToList();

            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - Dashboard_Daily - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<StateMaster> ViewBagStateId()
        {
            List<StateMaster> list = new List<StateMaster>();
            try
            {
                var StoreStateIds = _context.StoreMasters.Select(s => s.StateID).ToList();
                list = _context.StateMasters.Where(s => StoreStateIds.Contains(s.StateId)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - ViewBagStateId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return list;
        }

        public DashboardDaily getDashboardDailyData(string date, string StoreID, int? StateId, int? GroupID)
        {
            DashboardDaily obj = new DashboardDaily();
            try
            {
                List<DashboardDailySelect> Dashboard = _context.Database.SqlQuery<DashboardDailySelect>("SP_Dashboard_Daily @Date = {0},@StoreID={1},@StateId={2},@GroupID={3}", date, StoreID, StateId, GroupID).ToList();

                DashboardDailySelect obj2 = new DashboardDailySelect();
                if (Dashboard.Count() > 0)
                {

                    obj2 = Dashboard.FirstOrDefault();
                    obj.CustomerCountCurrentDay = obj2.CustomerCountCurrentDay;
                    obj.AllVoids = obj2.AllVoids;
                    obj.ItemCorrects = obj2.ItemCorrects;
                    obj.ItemReturns = obj2.ItemReturns;
                    obj.TotalCash = obj2.TotalCash;
                    obj.AverageSaleCurrentDay = obj2.AverageSaleCurrentDay;
                    obj.CashPayout = obj2.CashPayout;
                    obj.Over = obj2.Over;
                    obj.TotalSalesCurrentDay = obj2.TotalSalesCurrentDay;
                    obj.TotalSalesLastWeek = obj2.TotalSalesLastWeek;
                    obj.SalesGrowth = obj2.SalesGrowth;
                    obj.Offline = obj2.Offline;
                    obj.Online = obj2.Online;
                    obj.InvoicesAdded = obj2.InvoicesAdded;
                    obj.InvoicesApproved = obj2.InvoicesApproved;
                    obj.AllVoidsAmt = obj2.AllVoidsAmt;
                    obj.ItemCorrectsAmt = obj2.ItemCorrectsAmt;
                    obj.ItemReturnsAmt = obj2.ItemReturnsAmt;
                    obj.DayName = obj2.DayName;
                    obj.HourTime = obj2.HourTime;
                    obj.CustomerCountGrowth = obj2.CustomerCountGrowth;
                    obj.AveragesalesGrowth = obj2.AveragesalesGrowth;
                    obj.CustomerCountLastWeek = obj2.CustomerCountLastWeek;
                    obj.AverageSalesLastWeek = obj2.AverageSalesLastWeek;
                }
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetDashboardDailyData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<SalesOneHourWorkedDataList> GetSalesOneHourWorkedData(string date, int StoreID, string department, int? StateId, int? GroupID)
        {
            List<SalesOneHourWorkedDataList> list = new List<SalesOneHourWorkedDataList>();
            try
            {
                list = _context.Database.SqlQuery<SalesOneHourWorkedDataList>("SP_ChartStoreDepartmentData  @Mode={0},@Date = {1},@StoreID={2},@Department={3},@StateId={4},@GroupID={5}", "4", date, StoreID, department, StateId, GroupID).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetSalesOneHourWorkedData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return list;
        }

        public DashboardChartViewModel GetChartData(string StoreId, string Department, int Type, string Date, int? StateId, int? GroupID)
        {
            DashboardChartViewModel result = new DashboardChartViewModel();
            try
            {
                if (StateId == null)
                {
                    StateId = 0;
                }
                if (GroupID == null)
                {
                    GroupID = 0;
                }
                List<DashboardDailySelectChart> Dashboard = _context.Database.SqlQuery<DashboardDailySelectChart>("SP_Dashboard_Daily_Chart @StoreID = {0},@Type={1},@Deaprtment={2},@Date={3},@StateId={4},@GroupID={5}", StoreId, Type, Department, Date, StateId, GroupID).ToList();

                var HoursList = Dashboard.Select(s => s.Hours).Distinct();
                foreach (var item in HoursList)
                {
                    result.Labels.Add(item.ToString());
                    result.Labelss.Add("[a,b]");
                }

                var rnd = new Random();
                var StoreDisctinctList = Dashboard.Select(s => s.StoreId).Distinct();
                var store = _context.StoreMasters.ToList().Where(a => StoreDisctinctList.Contains(a.StoreId)).ToList();
                store.ForEach(a =>
                {
                    var results = new DatsetList();
                    results.code = a.StoreId.ToString();
                    results.label = a.Name;
                    if (a.StoreId == 1) /*7484*/
                    {
                        results.backgroundColor = "#f3db27";
                        results.borderColor = "#f3db27";

                    }
                    else if (a.StoreId == 5) /*2589*/
                    {
                        results.backgroundColor = "#7f0126";
                        results.borderColor = "#7f0126";
                    }
                    else if (a.StoreId == 6) /*14 th street*/
                    {
                        results.backgroundColor = "#11fa30";
                        results.borderColor = "#11fa30";
                    }
                    else if (a.StoreId == 2) /*maywood*/
                    {
                        results.backgroundColor = "#949494";
                        results.borderColor = "#949494";
                    }
                    else if (a.StoreId == 7)/*2840*/
                    {
                        results.backgroundColor = "#2d45ff";
                        results.borderColor = "#2d45ff";
                    }
                    else if (a.StoreId == 3) /*1407*/
                    {
                        results.backgroundColor = "#a581ff";
                        results.borderColor = "#a581ff";
                    }
                    else if (a.StoreId == 4) /*180*/
                    {
                        results.backgroundColor = "#e2601d";
                        results.borderColor = "#e2601d";
                    }
                    else/*170*/
                    {
                        results.backgroundColor = "#339db4";
                        results.borderColor = "#339db4";
                    }
                    results.radius = 4;
                    results.fill = false;
                    result.DatsetLists.Add(results);
                });

                decimal Total = 0;
                result.DatsetLists.ForEach(a =>
                {
                    foreach (var item in Dashboard)
                    {
                        if (a.code == item.StoreId.ToString())
                        {

                            if (item.Amount < 0)
                            {
                                a.data.Add(null);
                            }
                            else
                            {
                                a.data.Add(item.Amount.ToString());
                                Total = Total + item.Amount;
                            }
                        }
                    }
                });
                result.Total = Total;
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetChartData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return result;
        }

        public List<DashboardDailyTotalData> GetSalesTotalDetailsdata(string StoreId, string Department, int Type, string Date, int? StateId, int? GroupID)
        {
            List<DashboardDailyTotalData> list = new List<DashboardDailyTotalData>();
            try
            {
                list = _context.Database.SqlQuery<DashboardDailyTotalData>("SP_Dashboard_Daily_Totals @StoreID = {0},@Type={1},@Department={2},@Date={3},@StateId={4},@GroupID={5}", StoreId, Type, Department, Date, StateId, GroupID).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetSalesTotalDetailsdata - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return list;
        }

        public List<DashboardDailyTotalData> GetWeeklySalesTotalDetailsdata(string StoreId, string Department, int Type, string WeeklyPeriodId, int? StateId)
        {
            List<DashboardDailyTotalData> list = new List<DashboardDailyTotalData>();
            try
            {
                list = _context.Database.SqlQuery<DashboardDailyTotalData>("SP_Dashboard_Weekly_Totals @StoreID = {0},@Type={1},@Department={2},@WeekNo={3},@StateId={4}", StoreId, Type, Department, WeeklyPeriodId, StateId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetWeeklySalesTotalDetailsdata - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return list;
        }

        public List<DashboardDailyTotalData> GetWeeklySalesDailyTotalDetailsdata(string StoreId, string Department, int Type, string WeeklyPeriodId, int? StateId)
        {
            List<DashboardDailyTotalData> list = new List<DashboardDailyTotalData>();
            try
            {
                list = _context.Database.SqlQuery<DashboardDailyTotalData>("SP_Dashboard_Weekly_Totals @StoreID = {0},@Type={1},@Department={2},@WeekNo={3},@StateId={4}", StoreId, Type, Department, WeeklyPeriodId, StateId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetWeeklySalesDailyTotalDetailsdata - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return list;
        }

        public HourlyChartViewModel GetHourlyChartData(string StoreId, string Department, int Type, string Date, int? StateId, int? GroupID)
        {
            HourlyChartViewModel result = new HourlyChartViewModel();
            try
            {
                if (StateId == null)
                {
                    StateId = 0;
                }
                if (GroupID == null)
                {
                    GroupID = 0;
                }
                List<DashboardDailySelectChart> Dashboard = _context.Database.SqlQuery<DashboardDailySelectChart>("SP_Dashboard_Daily_Chart @StoreID = {0},@Type={1},@Deaprtment={2},@Date={3},@StateId={4},@GroupID={5}", StoreId, Type, Department, Date, StateId, GroupID).ToList();

                var HoursList = Dashboard.Select(s => s.Hours).Distinct();
                foreach (var item in Dashboard)
                {
                    result.Labels.Add(item.StoreId.ToString());
                    var results = new HourlyDatsetList();

                }
                var rnd = new Random();
                int i = 0;
                foreach (var item in Dashboard)
                {
                    var results = new HourlyDatsetList();
                    results.label = item.StoreId.ToString();
                    if (item.StoreId == 1) /*7484*/
                    {
                        results.backgroundColor = "#f3db27";
                    }
                    else if (item.StoreId == 5) /*2589*/
                    {
                        results.backgroundColor = "#7f0126";
                    }
                    else if (item.StoreId == 6) /*14 th street*/
                    {
                        results.backgroundColor = "#11fa30";
                    }
                    else if (item.StoreId == 2) /*maywood*/
                    {
                        results.backgroundColor = "#949494";
                    }
                    else if (item.StoreId == 7)/*2840*/
                    {
                        results.backgroundColor = "#2d45ff";
                    }
                    else if (item.StoreId == 3) /*1407*/
                    {
                        results.backgroundColor = "#a581ff";
                    }
                    else if (item.StoreId == 4) /*180*/
                    {
                        results.backgroundColor = "#e2601d";
                    }
                    else/*170*/
                    {
                        results.backgroundColor = "#339db4";
                    }
                    results.radius = 6;
                    result.HourlyDatsetList.Add(results);
                    i += 1;
                }


                result.HourlyDatsetList.ForEach(a =>
                {
                    foreach (var item in Dashboard)
                    {
                        if (a.label.ToString() == item.StoreId.ToString())
                        {
                            a.data.Add(new hourlydata() { x = item.Hours != null ? item.Hours.ToString() : "", y = item.Amount.ToString() });
                        }
                    }

                });
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetHourlyChartData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return result;
        }

        public DashboardChartViewModel GetChartDataDoughnut(string StoreId, string Department, string WeekNo, int Type, int? StateId)
        {
            DashboardChartViewModel result = new DashboardChartViewModel();
            try
            {
                if (StateId == null)
                {
                    StateId = 0;
                }
                List<DashboardDougWeeklySelectChart> Dashboard = _context.Database.SqlQuery<DashboardDougWeeklySelectChart>("SP_Dashboard_Doug_Weekly_Chart @StoreID = {0},@Deaprtment={1},@Weekno={2},@StateId={3}", StoreId, Department, WeekNo, StateId).ToList();
                Dashboard = Dashboard.Where(s => s.Type == Type).ToList();

                foreach (var item in Dashboard)
                {
                    result.Labels.Add(item.PercentData.ToString());
                    if (item.Weeks == "C")
                    {
                        result.Labels.Add((100 - item.PercentData).ToString("#.##"));
                    }
                }

                var rnd = new Random();
                var results = new DatsetListDoughnut();
                results.label = "1";

                if (StoreId == "1") /*7484*/
                {
                    results.backgroundColor.Add("#f3db27");
                    results.borderColor = "#f3db27";
                }
                else if (StoreId == "5") /*2589*/
                {
                    results.backgroundColor.Add("#7f0126");
                    results.borderColor = "#7f0126";
                }
                else if (StoreId == "6") /*14 th street*/
                {
                    results.backgroundColor.Add("#11fa30");
                    results.borderColor = "#11fa30";
                }
                else if (StoreId == "2") /*maywood*/
                {
                    results.backgroundColor.Add("#949494");
                    results.borderColor = "#949494";
                }
                else if (StoreId == "7")/*2840*/
                {
                    results.backgroundColor.Add("#2d45ff");
                    results.borderColor = "#2d45ff";
                }
                else if (StoreId == "3") /*1407*/
                {
                    results.backgroundColor.Add("#a581ff");
                    results.borderColor = "#a581ff";
                }
                else if (StoreId == "4") /*180*/
                {
                    results.backgroundColor.Add("#e2601d");
                    results.borderColor = "#e2601d";
                }
                else/*170*/
                {
                    results.backgroundColor.Add("#339db4");
                    results.borderColor = "#339db4";
                }
                results.backgroundColor.Add("#dddddd");
                results.borderColor = "#dddddd";

                foreach (var item in Dashboard)
                {
                    results.data.Add(item.PercentData.ToString());
                    if (item.Weeks == "C")
                    {
                        results.data.Add((100 - item.PercentData).ToString("#.##"));
                    }
                }
                result.DatsetListDoughnutLists.Add(results);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetChartDataDoughnut - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return result;
        }

        public DashboardChartViewModel GetChartDataBar(string StoreId, string Department, int Type, string Date, int? StateId, int? GroupID)
        {
            DashboardChartViewModel result = new DashboardChartViewModel();
            try
            {
                result.Labels.Add("1".ToString());
                result.Labels.Add("2".ToString());
                result.Labels.Add("3".ToString());


                var rnd = new Random();
                var results = new DatsetList();
                results.code = "1".ToString();
                results.label = "1";

                results.backgroundColor = ColorTranslator.ToHtml(Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256)));
                results.borderColor = ColorTranslator.ToHtml(Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256)));
                results.fill = false;
                results.data.Add("65");
                result.DatsetLists.Add(results);

                results = new DatsetList();
                results.code = "2".ToString();
                results.label = "2";

                results.backgroundColor = ColorTranslator.ToHtml(Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256)));
                results.borderColor = ColorTranslator.ToHtml(Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256)));
                results.fill = false;
                results.data.Add("25");
                result.DatsetLists.Add(results);

                results = new DatsetList();
                results.code = "3".ToString();
                results.label = "3";

                results.backgroundColor = ColorTranslator.ToHtml(Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256)));
                results.borderColor = ColorTranslator.ToHtml(Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256)));
                results.fill = false;
                results.data.Add("45");
                result.DatsetLists.Add(results);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetChartDataBar - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return result;
        }

        public List<BankAccountDetail> BankAccountDetailValue(int UserTypeId, int StoreID, string UserNm)
        {
            List<BankAccountDetail> banks = new List<BankAccountDetail>();
            try
            {
                string st = "";
                if (UserTypeId != 1)
                {
                    var StoreList = _context.userRoles.Where(s => s.UserTypeId == UserTypeId && s.ModuleId == 19 && s.Role == "ViewBAIFDailyDashboard").Select(s => s.StoreId).ToList();

                    if (StoreID != 0)
                    {
                        if (StoreList.Count != 0)
                        {
                            foreach (var item in StoreList)
                            {
                                if (item == StoreID)
                                {
                                    banks = _context.Database.SqlQuery<BankAccountDetail>("SP_BankAccountSetting @Spara={0},@StoreIDList={1}", "SelectAmount", item.ToString()).ToList();
                                    if (UserNm == "pzoitas" || UserNm == "dbelesis" || UserNm == "jzoitas")
                                    {
                                        if (banks.Count > 0)
                                            banks[0].IsVisible = 0;
                                    }
                                    else
                                    {
                                        if (banks.Count > 0)
                                            banks[0].IsVisible = 1;
                                    }
                                    break;
                                }
                            }

                        }
                    }
                    else
                    {
                        st = String.Join(", ", StoreList);
                        banks = _context.Database.SqlQuery<BankAccountDetail>("SP_BankAccountSetting @Spara={0},@StoreIDList={1}", "SelectAmount", st).ToList();
                        if (UserNm == "pzoitas" || UserNm == "dbelesis" || UserNm == "jzoitas")
                        {
                            if (banks.Count > 0)
                                banks[0].IsVisible = 0;
                        }
                        else
                        {
                            if (banks.Count > 0)
                                banks[0].IsVisible = 1;
                        }
                    }
                }
                else
                {
                    if (StoreID != 0)
                    {
                        banks = _context.Database.SqlQuery<BankAccountDetail>("SP_BankAccountSetting @Spara={0},@StoreIDList={1}", "SelectAmount", StoreID.ToString()).ToList();
                    }
                    else
                    {
                        banks = _context.Database.SqlQuery<BankAccountDetail>("SP_BankAccountSetting @Spara={0},@StoreIDList={1}", "SelectAmount", '0').ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - BankAccountDetailValue - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return banks;
        }

        public List<BankAccountDetail> BankAccountDetailValueRefresh(int UserTypeId, int StoreID)
        {
            List<BankAccountDetail> banks = new List<BankAccountDetail>();
            try
            {
                List<BankAccountDetailRefresh> banksRefresh = new List<BankAccountDetailRefresh>();
                NameValueCollection obj = new NameValueCollection();
                string Response = "";
                if (UserTypeId != 1)
                {
                    var StoreList = _context.userRoles.Where(s => s.UserTypeId == UserTypeId && s.ModuleId == 19 && s.Role == "ViewBAIFDailyDashboard").Select(s => s.StoreId).ToList();
                    string stRefresh = String.Join(",", StoreList);
                    banksRefresh = _context.Database.SqlQuery<BankAccountDetailRefresh>("SP_BankAccountSetting @Spara={0},@StoreIDList={1}", "SelectListID", stRefresh).ToList();

                    foreach (var item in banksRefresh)
                    {
                        Response = clsPlaidAPIOperations.GetBankBalance(item.AccessToken);
                        if (Response.Contains("error_message") == false)
                        {
                            clsPlaidAPIOperations.ExtractResponse(Response, ref obj);
                            item.AccountNo = obj["AccNo"].ToString();
                            item.Balance = Convert.ToDecimal(obj["Balance"].ToString());
                            _context.Database.ExecuteSqlCommand("SP_BankAccountSetting @Spara = {0},@ID = {1},@StoreID = {2},@Balance = {3},@AccountNo = {4}", "UpdateBalance", item.BankAccountId, item.StoreID, item.Balance, item.AccountNo);
                        }
                    }
                }
                else
                {
                    banksRefresh = _context.Database.SqlQuery<BankAccountDetailRefresh>("SP_BankAccountSetting @Spara={0}", "Select").ToList();
                    foreach (var item in banksRefresh)
                    {
                        Response = clsPlaidAPIOperations.GetBankBalance(item.AccessToken);
                        if (Response.Contains("error_message") == false)
                        {
                            clsPlaidAPIOperations.ExtractResponse(Response, ref obj);
                            item.AccountNo = obj["AccNo"].ToString();
                            item.Balance = Convert.ToDecimal(obj["Balance"].ToString());
                            _context.Database.ExecuteSqlCommand("SP_BankAccountSetting @Spara = {0},@ID = {1},@StoreID = {2},@Balance = {3},@AccountNo = {4}", "UpdateBalance", item.BankAccountId, item.StoreID, item.Balance, item.AccountNo);
                        }
                    }
                }

                string st = "";
                if (UserTypeId != 1)
                {
                    var StoreList = _context.userRoles.Where(s => s.UserTypeId == UserTypeId && s.ModuleId == 19 && s.Role == "ViewBAIFDailyDashboard").Select(s => s.StoreId).ToList();

                    if (StoreID != 0)
                    {
                        if (StoreList.Count != 0)
                        {
                            foreach (var item in StoreList)
                            {
                                if (item == StoreID)
                                {
                                    banks = _context.Database.SqlQuery<BankAccountDetail>("SP_BankAccountSetting @Spara={0},@StoreIDList={1}", "SelectAmount", item.ToString()).ToList();
                                    break;
                                }
                            }

                        }
                    }
                    else
                    {
                        st = String.Join(", ", StoreList);
                        banks = _context.Database.SqlQuery<BankAccountDetail>("SP_BankAccountSetting @Spara={0},@StoreIDList={1}", "SelectAmount", st).ToList();
                    }
                }
                else
                {
                    if (StoreID != 0)
                    {
                        banks = _context.Database.SqlQuery<BankAccountDetail>("SP_BankAccountSetting @Spara={0},@StoreIDList={1}", "SelectAmount", StoreID.ToString()).ToList();
                    }
                    else
                    {
                        banks = _context.Database.SqlQuery<BankAccountDetail>("SP_BankAccountSetting @Spara={0},@StoreIDList={1}", "SelectAmount", '0').ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - BankAccountDetailValueRefresh - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return banks;
        }

        public DashboardDaily Weekly(string Startdate, int StoreID, int? StateId)
        {
            DashboardDaily obj = new DashboardDaily();
            try
            {
                List<DashboardDailySelect> Dashboard = _context.Database.SqlQuery<DashboardDailySelect>("SP_Dashboard_Daily @Date = {0},@StoreID={1},@StateId={2}", Startdate, StoreID, StateId).ToList();

                if (Dashboard.Count() > 0)
                {
                    DashboardDailySelect obj2 = new DashboardDailySelect();
                    obj2 = Dashboard.FirstOrDefault();
                    obj.AllVoidsAmt = obj2.AllVoidsAmt;
                    obj.ItemCorrectsAmt = obj2.ItemCorrectsAmt;
                    obj.ItemReturnsAmt = obj2.ItemReturnsAmt;
                    obj.AllVoids = obj2.AllVoids;
                    obj.ItemCorrects = obj2.ItemCorrects;
                    obj.ItemReturns = obj2.ItemReturns;
                    obj.CashPayout = obj2.CashPayout;
                    obj.Over = obj2.Over;
                    obj.TotalSalesCurrentDay = obj2.TotalSalesCurrentDay;
                    obj.SalesGrowth = obj2.SalesGrowth;
                    obj.CustomerCountCurrentDay = obj2.CustomerCountCurrentDay;
                    obj.CustomerCountGrowth = obj2.CustomerCountGrowth;
                    obj.AverageSaleCurrentDay = obj2.AverageSaleCurrentDay;
                    obj.AveragesalesGrowth = obj2.AveragesalesGrowth;
                    obj.TotalSalesLastWeek = obj2.TotalSalesLastWeek;
                    obj.CustomerCountLastWeek = obj2.CustomerCountLastWeek;
                    obj.AverageSalesLastWeek = obj2.AverageSalesLastWeek;
                    obj.TotalCash = obj2.TotalCash;
                    obj.Offline = obj2.Offline;
                    obj.Online = obj2.Online;
                    obj.InvoicesAdded = obj2.InvoicesAdded;
                    obj.InvoicesApproved = obj2.InvoicesApproved;
                }

                obj.StoreList = _context.Database.SqlQuery<DropdownList>("SP_ChartStoreDepartmentData @Mode={0},@StateId={1}", "1", StateId).ToList();

                obj.DepartmentList = _context.Database.SqlQuery<DropdownDepartmentList>("SP_ChartStoreDepartmentData @Mode={0},@StateId={1}", "2", StateId).ToList();
                obj.PayrollDepartmentList = _context.Database.SqlQuery<DropdownDepartmentList>("SP_ChartStoreDepartmentData @Mode={0},@StateId={1}", "3", StateId).ToList();

            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - Weekly - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }
        public DashboardDaily Weekly_1(int? StateId,ref DashboardDaily obj)
        {
            try
            {
                obj.StoreList = _context.Database.SqlQuery<DropdownList>("SP_ChartStoreDepartmentData @Mode={0},@StateId={1}", "1", StateId).ToList();

                obj.DepartmentList = _context.Database.SqlQuery<DropdownDepartmentList>("SP_ChartStoreDepartmentData @Mode={0},@StateId={1}", "2", StateId).ToList();
                obj.PayrollDepartmentList = _context.Database.SqlQuery<DropdownDepartmentList>("SP_ChartStoreDepartmentData @Mode={0},@StateId={1}", "3", StateId).ToList();

            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - Weekly - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }
        public List<DropdownList> ChartStoreDepartmentData(int? StateId, DateTime dtStart)
        {
            List<DropdownList> list = new List<DropdownList>();
            try
            {
                list = _context.Database.SqlQuery<DropdownList>("SP_ChartStoreDepartmentData @Mode={0},@StateId={1}", "6", StateId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - ChartStoreDepartmentData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return list;
        }

        public int WeeklyPeriodId(DateTime dtStart)
        {
            int WeekendID = 0;
            try
            {
                WeekendID = _context.weeklyPeriods.ToList().Where(s => s.StartDate <= dtStart && s.EndDate >= dtStart) != null ? _context.weeklyPeriods.Where(s => s.StartDate <= dtStart && s.EndDate >= dtStart).FirstOrDefault().WeeklyPeriodId : 0;
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - WeeklyPeriodId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return WeekendID;
        }

        public DashboardDaily getDashboardWeeklyData(int WeeklyPeriodId, int? StateId, int StoreID)
        {
            DashboardDaily obj = new DashboardDaily();
            try
            {
                List<DashboardWeeklySelect> Dashboard = _context.Database.SqlQuery<DashboardWeeklySelect>("SP_Dashboard_Weekly @Weekno = {0},@StoreID={1},@StateId={2}", WeeklyPeriodId, StoreID, StateId).ToList();
                var WeeklyRange = _context.weeklyPeriods.Find(WeeklyPeriodId);
                var SignedCount = _context2.Database.SqlQuery<int>("SPEmployeeDocument_Dashboard @Spara = {0},@FromDate={1},@ToDate={2},@StoreId={3},@StateId={4}", "SelectSignedByDate", WeeklyRange.StartDate, WeeklyRange.EndDate, StoreID, StateId).Count() > 0 ? _context2.Database.SqlQuery<int>("SPEmployeeDocument_Dashboard @Spara = {0},@FromDate={1},@ToDate={2},@StoreId={3},@StateId={4}", "SelectSignedByDate", WeeklyRange.StartDate, WeeklyRange.EndDate, StoreID, StateId).FirstOrDefault() : 0;
                var UnSignedCount = _context2.Database.SqlQuery<int>("SPEmployeeDocument_Dashboard @Spara = {0},@FromDate={1},@ToDate={2},@StoreId={3},@StateId={4}", "SelectUnSignedByDate", WeeklyRange.StartDate, WeeklyRange.EndDate, StoreID, StateId).Count() > 0 ? _context2.Database.SqlQuery<int>("SPEmployeeDocument_Dashboard @Spara = {0},@FromDate={1},@ToDate={2},@StoreId={3},@StateId={4}", "SelectUnSignedByDate", WeeklyRange.StartDate, WeeklyRange.EndDate, StoreID, StateId).FirstOrDefault() : 0;
                DashboardWeeklySelect obj2 = new DashboardWeeklySelect();

                if (Dashboard.Count() > 0)
                {
                    obj2 = Dashboard.FirstOrDefault();
                    obj.CustomerCountCurrentDay = obj2.CustomerCountCurrentDay;
                    obj.AllVoids = obj2.AllVoids;
                    obj.ItemCorrects = obj2.ItemCorrects;
                    obj.ItemReturns = obj2.ItemReturns;
                    obj.TotalCash = obj2.TotalCash;
                    obj.AverageSaleCurrentDay = obj2.AverageSaleCurrentDay;
                    obj.CashPayout = obj2.CashPayout;
                    obj.Over = obj2.Over;
                    obj.TotalSalesCurrentDay = obj2.TotalSalesCurrentDay;
                    obj.TotalSalesLastWeek = obj2.TotalSalesLastWeek;
                    obj.SalesGrowth = obj2.SalesGrowth;
                    obj.Offline = obj2.Offline;
                    obj.Online = obj2.Online;
                    obj.InvoicesAdded = obj2.InvoicesAdded;
                    obj.InvoicesApproved = obj2.InvoicesApproved;
                    obj.AllVoidsAmt = obj2.AllVoidsAmt;
                    obj.ItemCorrectsAmt = obj2.ItemCorrectsAmt;
                    obj.ItemReturnsAmt = obj2.ItemReturnsAmt;
                    obj.SignedCount = SignedCount;
                    obj.UnSignedCount = UnSignedCount;
                }
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetDashboardWeeklyData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public DashboardDaily GetPayrollSalesBoxesData(int WeeklyPeriodId, string Department, int? StateId, int StoreID)
        {
            DashboardDaily obj = new DashboardDaily();
            try
            {
                List<DashboardWeeklySelect> Dashboard = _context.Database.SqlQuery<DashboardWeeklySelect>("SP_Dashboard_PayrollSalesBoxesData @Weekno = {0},@StoreID={1},@Department={2},@StateId={3}", WeeklyPeriodId, StoreID, Department, StateId).ToList();

                DashboardWeeklySelect obj2 = new DashboardWeeklySelect();
                if (Dashboard.Count() > 0)
                {
                    obj2 = Dashboard.FirstOrDefault();

                    obj.PayrollOverTime = obj2.PayrollOverTime.Value;
                    obj.PayrollSickPay = obj2.PayrollSickPay.Value;
                    obj.PayrollVacation = obj2.PayrollVacation.Value;
                    obj.PayrollHolidays = obj2.PayrollHolidays.Value;
                    obj.PayrollSalary = obj2.PayrollSalary == null ? 0 : obj2.PayrollVacation.Value;
                    obj.PayrollBonus = obj2.PayrollBonus.Value;
                    obj.PayrollRegularPay = obj2.PayrollRegularPay.Value;
                    obj.SalesRegularPay = obj2.SalesRegularPay.Value;
                    obj.SalesOverTime = obj2.SalesOverTime.Value;
                    obj.SalesSalary = obj2.SalesSalary.Value;
                    obj.SalesOtherpay = obj2.SalesOtherpay.Value;
                }
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetPayrollSalesBoxesData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public DashboardChartViewModel GetWeeklyChartData(string StoreId, string Department, int Type, int WeeklyPeriodId, int? StateId)
        {
            DashboardChartViewModel result = new DashboardChartViewModel();
            try
            {
                List<DashboardWeeklySelectChart> Dashboard = _context.Database.SqlQuery<DashboardWeeklySelectChart>("SP_Dashboard_Weekly_Chart @StoreID = {0},@Type={1},@Deaprtment={2},@Weekno={3},@StateId={4}", StoreId, Type, Department, WeeklyPeriodId, StateId).ToList();

                var HoursList = Dashboard.Select(s => s.Hours).Distinct();
                foreach (var item in HoursList)
                {
                    result.Labels.Add(item.ToString());
                }

                var rnd = new Random();
                var StoreDisctinctList = Dashboard.Select(s => s.StoreId).Distinct();
                var store = _context.StoreMasters.ToList().Where(a => StoreDisctinctList.Contains(a.StoreId)).ToList();
                store.ForEach(a =>
                {
                    var results = new DatsetList();
                    results.code = a.StoreId.ToString();
                    results.label = a.Name;
                    if (a.StoreId == 1) /*7484*/
                    {
                        results.backgroundColor = "#f3db27";
                        results.borderColor = "#f3db27";
                    }
                    else if (a.StoreId == 5) /*2589*/
                    {
                        results.backgroundColor = "#7f0126";
                        results.borderColor = "#7f0126";
                    }
                    else if (a.StoreId == 6) /*14 th street*/
                    {
                        results.backgroundColor = "#11fa30";
                        results.borderColor = "#11fa30";
                    }
                    else if (a.StoreId == 2) /*maywood*/
                    {
                        results.backgroundColor = "#949494";
                        results.borderColor = "#949494";
                    }
                    else if (a.StoreId == 7)/*2840*/
                    {
                        results.backgroundColor = "#2d45ff";
                        results.borderColor = "#2d45ff";
                    }
                    else if (a.StoreId == 3) /*1407*/
                    {
                        results.backgroundColor = "#a581ff";
                        results.borderColor = "#a581ff";
                    }
                    else if (a.StoreId == 4) /*180*/
                    {
                        results.backgroundColor = "#e2601d";
                        results.borderColor = "#e2601d";
                    }
                    else/*170*/
                    {
                        results.backgroundColor = "#339db4";
                        results.borderColor = "#339db4";
                    }

                    results.radius = 4;
                    results.fill = false;
                    result.DatsetLists.Add(results);
                });

                decimal Total = 0;
                result.DatsetLists.ForEach(a =>
                {
                    foreach (var item in Dashboard)
                    {
                        if (a.code == item.StoreId.ToString())
                        {
                            a.data.Add(item.Amount.ToString());
                            Total = Total + item.Amount;
                        }

                    }
                });
                result.Total = Total;
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetWeeklyChartData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return result;
        }

        public DashboardChartViewModel GetWeeklyBarChartData(string StoreId, string Department, int Type, int WeeklyPeriodId, int? StateId)
        {
            DashboardChartViewModel result = new DashboardChartViewModel();
            try
            {
                List<DashboardWeeklySelectChart> Dashboard = _context.Database.SqlQuery<DashboardWeeklySelectChart>("SP_Dashboard_Weekly_Chart @StoreID = {0},@Type={1},@Deaprtment={2},@Weekno={3},@StateId={4}", StoreId, Type, Department, WeeklyPeriodId, StateId).ToList();

                var rnd = new Random();
                var StoreDisctinctList = Dashboard.Select(s => s.StoreId).Distinct();
                var store = _context.StoreMasters.ToList().Where(a => StoreDisctinctList.Contains(a.StoreId)).ToList();
                int i = 1;
                foreach (var item in Dashboard)
                {
                    var results = new DatsetList();
                    results.code = item.StoreId.ToString();
                    results.label = item.Hours;
                    if (i == 1)
                    {
                        results.backgroundColor = "#f3db27";
                        results.borderColor = "#f3db27";

                    }
                    else if (i == 2)
                    {

                        results.backgroundColor = "#CDDC39";
                        results.borderColor = "#CDDC39";
                    }
                    else
                    {
                        results.backgroundColor = "#689F38";
                        results.borderColor = "#689F38";

                    }

                    results.fill = false;
                    result.DatsetLists.Add(results);
                    i += 1;
                };

                i = 1;
                result.DatsetLists.ForEach(a =>
                {
                    for (int j = 0; j < Dashboard.Count; j++)
                    {
                        if (j == 0 && i == 1)
                        {
                            a.data.Add(Dashboard[j].Amount.ToString());
                            break;
                        }
                        else if (j == 1 && i == 2)
                        {
                            a.data.Add(Dashboard[j].Amount.ToString());
                            break;
                        }
                        else if (j == 2 && i == 3)
                        {
                            a.data.Add(Dashboard[j].Amount.ToString());
                            break;
                        }
                    }
                    i += 1;
                });
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetWeeklyBarChartData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return result;
        }

        public WeeklyComparisionData GetWeeklyComparisionData(string StoreId, string Department, int Type, int WeeklyPeriodId, int? StateId)
        {
            WeeklyComparisionData objWeeklyComparisionData = new WeeklyComparisionData();
            try
            {
                List<WeeklyComparisionData> DashboardWeeklyData = _context.Database.SqlQuery<WeeklyComparisionData>("SP_Dashboard_Weekly_Comparison @StoreID = {0},@Type={1},@Deaprtment={2},@Weekno={3},@StateId={4}", StoreId, Type, Department, WeeklyPeriodId, StateId).ToList();

                if (DashboardWeeklyData.Count() > 0)
                {
                    objWeeklyComparisionData = DashboardWeeklyData.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetWeeklyComparisionData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return objWeeklyComparisionData;
        }

        public HourlyChartViewModel GetHourlyChartDataWeek(string StoreId, string Department, int Type, int WeeklyPeriodId, int? StateId)
        {
            HourlyChartViewModel result = new HourlyChartViewModel();
            try
            {
                List<DashboardWeeklySelectChart> Dashboard = _context.Database.SqlQuery<DashboardWeeklySelectChart>("SP_Dashboard_Weekly_Chart @StoreID = {0},@Type={1},@Deaprtment={2},@Weekno={3},@StateId={4}", StoreId, Type, Department, WeeklyPeriodId, StateId).ToList();

                var HoursList = Dashboard.Select(s => s.Hours).Distinct();
                foreach (var item in Dashboard)
                {
                    result.Labels.Add(item.StoreId.ToString());
                    var results = new HourlyDatsetList();

                }
                var rnd = new Random();
                int i = 0;
                foreach (var item in Dashboard)
                {
                    var results = new HourlyDatsetList();
                    results.label = item.StoreId.ToString();
                    if (item.StoreId == 1) /*7484*/
                    {
                        results.backgroundColor = "#f3db27";
                    }
                    else if (item.StoreId == 5) /*2589*/
                    {
                        results.backgroundColor = "#7f0126";
                    }
                    else if (item.StoreId == 6) /*14 th street*/
                    {
                        results.backgroundColor = "#11fa30";
                    }
                    else if (item.StoreId == 2) /*maywood*/
                    {
                        results.backgroundColor = "#949494";
                    }
                    else if (item.StoreId == 7)/*2840*/
                    {
                        results.backgroundColor = "#2d45ff";
                    }
                    else if (item.StoreId == 3) /*1407*/
                    {
                        results.backgroundColor = "#a581ff";
                    }
                    else if (item.StoreId == 4) /*180*/
                    {
                        results.backgroundColor = "#e2601d";
                    }
                    else/*170*/
                    {
                        results.backgroundColor = "#339db4";
                    }
                    results.radius = 4;
                    result.HourlyDatsetList.Add(results);
                    i += 1;
                }

                result.HourlyDatsetList.ForEach(a =>
                {
                    foreach (var item in Dashboard)
                    {
                        if (a.label.ToString() == item.StoreId.ToString())
                        {
                            a.data.Add(new hourlydata() { x = item.Hours == null ? "" : item.Hours.ToString(), y = item.Amount.ToString() });
                        }
                    }

                });
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetHourlyChartDataWeek - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return result;
        }

        public List<SalesOneHourWorkedDataList> GetSalesOneHourWorkedDataWeekly(int WeeklyPeriodId, string department, int? StateId, int StoreID)
        {
            List<SalesOneHourWorkedDataList> StoreHourlyWorkData = new List<SalesOneHourWorkedDataList>();
            try
            {
                StoreHourlyWorkData = _context.Database.SqlQuery<SalesOneHourWorkedDataList>("SP_ChartStoreDepartmentData  @Mode={0},@Date = {1},@StoreID={2},@Department={3},@Weekno={4},@StateId={5}", "5", null, StoreID, department, WeeklyPeriodId, StateId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetSalesOneHourWorkedDataWeekly - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return StoreHourlyWorkData;
        }

        public List<DashboardChartToolTipData> getToolTipData(string StoreId, string Department, int Type, string Date, string Hours, string Mode)
        {
            List<DashboardChartToolTipData> ToolTipData = new List<DashboardChartToolTipData>();
            try
            {
                if (Department != "ALL" && Type != 2)
                {
                    ToolTipData = _context.Database.SqlQuery<DashboardChartToolTipData>("Sp_getToolTipData @StoreID = {0},@Type={1},@Department={2},@Date={3},@ChartType={4},@Hours={5}", StoreId, Type, Department, Date, Mode, Hours).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetToolTipData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return ToolTipData;
        }

        public List<DashboardChartToolTipData> getWeeklyHourlyChartToolTipData(string StoreId, string Department, int Type, int WeeklyPeriodId)
        {
            List<DashboardChartToolTipData> ToolTipData = new List<DashboardChartToolTipData>();
            try
            {
                ToolTipData = _context.Database.SqlQuery<DashboardChartToolTipData>("Sp_getToolTipData @StoreID = {0},@Type={1},@Department={2},@Date={3},@ChartType={4},@Hours={5},@WeeklyPeriodId={6}", StoreId, Type, Department, null, "", "1", WeeklyPeriodId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetWeeklyHourlyChartToolTipData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return ToolTipData;
        }

        public DashboardWeeklyExpenses GetExpensesBoxesData(int WeeklyPeriodId, string StoreID, int? StateId)
        {
            DashboardWeeklyExpenses obj = new DashboardWeeklyExpenses();
            try
            {
                List<ExpensesWeeklyBox> Dashboard = _context.Database.SqlQuery<ExpensesWeeklyBox>("ExpensesList_ForDashboard_Proc @Weekno = {0},@StoreID={1},@StateId={2}", WeeklyPeriodId, StoreID, StateId).ToList();

                obj.spDelivery = "DELIVERY EXPENSE";
                obj.spOffice = "OFFICE EXPENSES";
                obj.spProduce = "PRODUCE BUYER";
                obj.spRepairs = "REPAIRS & MAINTENANCE";
                obj.spSupplies = "SUPPLIES";
                obj.spExterminator = "EXTERMINATOR";
                obj.spManagement = "MANAGEMENT FEES";

                obj.ExpDelivery = "0.00%";
                obj.ExpOffice = "0.00%";
                obj.ExpProduce = "0.00%";
                obj.ExpRepairs = "0.00%";
                obj.ExpSupplies = "0.00%";
                obj.ExpExterminator = "0.00%";
                obj.ExpManagement = "0.00%";

                ExpensesWeeklyBox obj2 = new ExpensesWeeklyBox();
                if (Dashboard.Count() > 0)
                {


                    foreach (var r in Dashboard)
                    {
                        switch (r.DepartmentName)
                        {
                            case "DELIVERY EXPENSE":

                                obj.spvDelivery = r.Amount;
                                obj.ExpDelivery = r.ExpPercentage;
                                break;
                            case "OFFICE EXPENSES":

                                obj.spvOffice = r.Amount;
                                obj.ExpOffice = r.ExpPercentage;
                                break;
                            case "PRODUCE BUYER":

                                obj.spvProduce = r.Amount;
                                obj.ExpProduce = r.ExpPercentage;
                                break;
                            case "REPAIRS & MAINTENANCE":
                                obj.spvRepairs = r.Amount;
                                obj.ExpRepairs = r.ExpPercentage;
                                break;
                            case "SUPPLIES":

                                obj.spvSupplies = r.Amount;
                                obj.ExpSupplies = r.ExpPercentage;
                                break;
                            case "EXTERMINATOR":

                                obj.spvExterminator = r.Amount;
                                obj.ExpExterminator = r.ExpPercentage;
                                break;
                            case "MANAGEMENT FEES":

                                obj.spvManagement = r.Amount;
                                obj.ExpManagement = r.ExpPercentage;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetExpensesBoxesData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public DateTime GetEndDateWeeklyPeriod()
        {
            DateTime time = DateTime.Now;
            try
            {
                time = (_context.weeklyPeriods.Where(x => x.Year == DateTime.Now.Year && x.EndDate <= DateTime.Today) == null) ? _context.weeklyPeriods.Where(x => x.Year == DateTime.Now.Year && x.EndDate <= DateTime.Today).Max(x => x.EndDate) : DateTime.Today;
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetEndDateWeeklyPeriod - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return time;
        }

        public DateTime EndDate()
        {
            DateTime time = DateTime.Now;
            try
            {
                time = _context.weeklyPeriods.Where(x => x.Year == DateTime.Now.Year && x.EndDate <= DateTime.Today).Max(x => x.EndDate);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - EndDate - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return time;
        }

        public List<DropdownList> ViewBagWeeklyPeriodId()
        {
            List<DropdownList> list = new List<DropdownList>();
            try
            {
                list = _context.Database.SqlQuery<DropdownList>("SP_ChartStoreDepartmentData @Mode={0}", "6").ToList();
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - ViewBagWeeklyPeriodId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return list;
        }

        public DashboardDaily Periodic(string Startdate, int StoreID)
        {
            DashboardDaily obj = new DashboardDaily();
            try
            {
                List<DashboardDailySelect> Dashboard = _context.Database.SqlQuery<DashboardDailySelect>("SP_Dashboard_Daily @Date = {0},@StoreID={1}", Startdate, StoreID).ToList();

                if (Dashboard.Count() > 0)
                {
                    DashboardDailySelect obj2 = new DashboardDailySelect();
                    obj2 = Dashboard.FirstOrDefault();
                    obj.AllVoidsAmt = obj2.AllVoidsAmt;
                    obj.ItemCorrectsAmt = obj2.ItemCorrectsAmt;
                    obj.ItemReturnsAmt = obj2.ItemReturnsAmt;
                    obj.AllVoids = obj2.AllVoids;
                    obj.ItemCorrects = obj2.ItemCorrects;
                    obj.ItemReturns = obj2.ItemReturns;
                    obj.CashPayout = obj2.CashPayout;
                    obj.Over = obj2.Over;
                    obj.TotalSalesCurrentDay = obj2.TotalSalesCurrentDay;
                    obj.SalesGrowth = obj2.SalesGrowth;
                    obj.CustomerCountCurrentDay = obj2.CustomerCountCurrentDay;
                    obj.CustomerCountGrowth = obj2.CustomerCountGrowth;
                    obj.AverageSaleCurrentDay = obj2.AverageSaleCurrentDay;
                    obj.AveragesalesGrowth = obj2.AveragesalesGrowth;
                    obj.TotalSalesLastWeek = obj2.TotalSalesLastWeek;
                    obj.CustomerCountLastWeek = obj2.CustomerCountLastWeek;
                    obj.AverageSalesLastWeek = obj2.AverageSalesLastWeek;
                    obj.TotalCash = obj2.TotalCash;
                    obj.Offline = obj2.Offline;
                    obj.Online = obj2.Online;
                    obj.InvoicesAdded = obj2.InvoicesAdded;
                    obj.InvoicesApproved = obj2.InvoicesApproved;
                }
                obj.StoreList = _context.Database.SqlQuery<DropdownList>("SP_ChartStoreDepartmentDataPeriodic @Mode={0}", "1").ToList();
                obj.DepartmentList = _context.Database.SqlQuery<DropdownDepartmentList>("SP_ChartStoreDepartmentDataPeriodic @Mode={0}", "2").ToList();
                obj.PayrollDepartmentList = _context.Database.SqlQuery<DropdownDepartmentList>("SP_ChartStoreDepartmentDataPeriodic @Mode={0}", "3").ToList();
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - Periodic - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public DashboardDaily ddlPeriodic()
        {
            DashboardDaily obj = new DashboardDaily();
            try
            {
                obj.StoreList = _context.Database.SqlQuery<DropdownList>("SP_ChartStoreDepartmentDataPeriodic @Mode={0}", "1").ToList();
                obj.DepartmentList = _context.Database.SqlQuery<DropdownDepartmentList>("SP_ChartStoreDepartmentDataPeriodic @Mode={0}", "2").ToList();
                obj.PayrollDepartmentList = _context.Database.SqlQuery<DropdownDepartmentList>("SP_ChartStoreDepartmentDataPeriodic @Mode={0}", "3").ToList();
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - ddlPeriodic - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public DashboardDaily getDashboardPeriodicData(string StartDate, string EndDate, int? StateId, DateTime Start, DateTime End, int StoreID)
        {
            DashboardDaily obj = new DashboardDaily();
            try
            {
                List<DashboardWeeklySelect> Dashboard = _context.Database.SqlQuery<DashboardWeeklySelect>("SP_Dashboard_Periodic @StartDate = {0},@EndDate = {1},@StoreID={2},@StateId={3}", Start, End, StoreID, (StateId == null ? 0 : StateId)).ToList();

                DashboardWeeklySelect obj2 = new DashboardWeeklySelect();
                if (Dashboard.Count() > 0)
                {
                    obj2 = Dashboard.FirstOrDefault();
                    obj.CustomerCountCurrentDay = obj2.CustomerCountCurrentDay;
                    obj.AllVoids = obj2.AllVoids;
                    obj.ItemCorrects = obj2.ItemCorrects;
                    obj.ItemReturns = obj2.ItemReturns;
                    obj.TotalCash = obj2.TotalCash;
                    obj.AverageSaleCurrentDay = obj2.AverageSaleCurrentDay;
                    obj.CashPayout = obj2.CashPayout;
                    obj.Over = obj2.Over;
                    obj.TotalSalesCurrentDay = obj2.TotalSalesCurrentDay;
                    obj.TotalSalesLastWeek = obj2.TotalSalesLastWeek;
                    obj.SalesGrowth = obj2.SalesGrowth;
                    obj.Offline = obj2.Offline;
                    obj.Online = obj2.Online;
                    obj.InvoicesAdded = obj2.InvoicesAdded;
                    obj.InvoicesApproved = obj2.InvoicesApproved;
                    obj.AllVoidsAmt = obj2.AllVoidsAmt;
                    obj.ItemCorrectsAmt = obj2.ItemCorrectsAmt;
                    obj.ItemReturnsAmt = obj2.ItemReturnsAmt;
                }
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetDashboardPeriodicData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<SalesOneHourWorkedDataList> GetSalesOneHourWorkedDataPeriodic(string StartDate, string EndDate, string department, int? StateId, DateTime Start, DateTime End, int StoreID)
        {
            List<SalesOneHourWorkedDataList> StoreHourlyWorkData = new List<SalesOneHourWorkedDataList>();
            try
            {
                StoreHourlyWorkData = _context.Database.SqlQuery<SalesOneHourWorkedDataList>("SP_ChartStoreDepartmentDataPeriodic  @Mode={0},@Date = {1},@StoreID={2},@Department={3},@StartDate={4},@EndDate={5},@StateId={6}", "5", null, StoreID, department, Start, End, (StateId == null ? 0 : StateId)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetSalesOneHourWorkedDataPeriodic - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return StoreHourlyWorkData;
        }

        public List<DashboardDailyTotalData> GetPeriodicSalesTotalDetailsdata(string StoreId, string Department, int Type, DateTime Start, DateTime End)
        {
            List<DashboardDailyTotalData> Dashboard = new List<DashboardDailyTotalData>();
            try
            {
                Dashboard = _context.Database.SqlQuery<DashboardDailyTotalData>("SP_Dashboard_Periodic_Totals @StoreID = {0},@Type={1},@Department={2},@StartDate={3},@EndDate={4}", StoreId, Type, Department, Start, End).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetPeriodicSalesTotalDetailsdata - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Dashboard;
        }

        public DashboardChartViewModel GetPeriodicChartData(string StoreId, string Department, int Type, int? StateId, DateTime Start, DateTime End)
        {
            DashboardChartViewModel result = new DashboardChartViewModel();
            try
            {
                List<DashboardWeeklySelectChart> Dashboard = _context.Database.SqlQuery<DashboardWeeklySelectChart>("SP_Dashboard_Periodic_Chart @StoreID = {0},@Type={1},@Deaprtment={2},@StartDate={3},@EndDate={4},@StateId={5}", StoreId, Type, Department, Start, End, StateId == null ? 0 : StateId).ToList();

                var HoursList = Dashboard.Select(s => s.Hours).Distinct();
                foreach (var item in HoursList)
                {
                    result.Labels.Add(item.ToString());
                }

                var rnd = new Random();
                var StoreDisctinctList = Dashboard.Select(s => s.StoreId).Distinct();
                var store = _context.StoreMasters.ToList().Where(a => StoreDisctinctList.Contains(a.StoreId)).ToList();
                store.ForEach(a =>
                {
                    var results = new DatsetList();
                    results.code = a.StoreId.ToString();
                    results.label = a.Name;
                    if (a.StoreId == 1) /*7484*/
                    {
                        results.backgroundColor = "#f3db27";
                        results.borderColor = "#f3db27";
                    }
                    else if (a.StoreId == 5) /*2589*/
                    {
                        results.backgroundColor = "#7f0126";
                        results.borderColor = "#7f0126";
                    }
                    else if (a.StoreId == 6) /*14 th street*/
                    {
                        results.backgroundColor = "#11fa30";
                        results.borderColor = "#11fa30";
                    }
                    else if (a.StoreId == 2) /*maywood*/
                    {
                        results.backgroundColor = "#949494";
                        results.borderColor = "#949494";
                    }
                    else if (a.StoreId == 7)/*2840*/
                    {
                        results.backgroundColor = "#2d45ff";
                        results.borderColor = "#2d45ff";
                    }
                    else if (a.StoreId == 3) /*1407*/
                    {
                        results.backgroundColor = "#a581ff";
                        results.borderColor = "#a581ff";
                    }
                    else if (a.StoreId == 4) /*180*/
                    {
                        results.backgroundColor = "#e2601d";
                        results.borderColor = "#e2601d";
                    }
                    else/*170*/
                    {
                        results.backgroundColor = "#339db4";
                        results.borderColor = "#339db4";
                    }
                    results.radius = 4;
                    results.fill = false;
                    result.DatsetLists.Add(results);
                });

                decimal Total = 0;
                result.DatsetLists.ForEach(a =>
                {
                    foreach (var item in Dashboard)
                    {
                        if (a.code == item.StoreId.ToString())
                        {
                            a.data.Add(item.Amount.ToString());
                            Total = Total + item.Amount;
                        }

                    }
                });
                result.Total = Total;
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetPeriodicChartData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return result;
        }

        public DashboardChartViewModel GetChartDataDoughnutPeriodic(string StoreId, string Department, int Type, int? StateId, DateTime Start, DateTime End)
        {
            DashboardChartViewModel result = new DashboardChartViewModel();
            try
            {
                List<DashboardDougWeeklySelectChart> Dashboard = new List<DashboardDougWeeklySelectChart>();
              
                try
                {
                    Dashboard = _context.Database.SqlQuery<DashboardDougWeeklySelectChart>("SP_Dashboard_Doug_Periodic_Chart @StoreID = {0},@Deaprtment={1},@StartDate={2},@EndDate={3},@StateId={4}", StoreId, Department, Start, End, (StateId == null ? 0 : StateId)).ToList();
                    Dashboard = Dashboard.Where(s => s.Type == Type).ToList();
                    result = new DashboardChartViewModel();

                    foreach (var item in Dashboard)
                    {
                        result.Labels.Add(item.PercentData.ToString());
                        if (item.Weeks == "C")
                        {
                            result.Labels.Add((100 - item.PercentData).ToString("#.##"));
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("DashboardRepository - GetChartDataDoughnutPeriodic1 - " + DateTime.Now + " - " + ex.Message.ToString());
                }
                var rnd = new Random();
                var results = new DatsetListDoughnut();
                results.label = "1";

                if (StoreId == "1") /*7484*/
                {
                    results.backgroundColor.Add("#f3db27");
                    results.borderColor = "#f3db27";
                }
                else if (StoreId == "5") /*2589*/
                {
                    results.backgroundColor.Add("#7f0126");
                    results.borderColor = "#7f0126";
                }
                else if (StoreId == "6") /*14 th street*/
                {
                    results.backgroundColor.Add("#11fa30");
                    results.borderColor = "#11fa30";
                }
                else if (StoreId == "2") /*maywood*/
                {
                    results.backgroundColor.Add("#949494");
                    results.borderColor = "#949494";
                }
                else if (StoreId == "7")/*2840*/
                {
                    results.backgroundColor.Add("#2d45ff");
                    results.borderColor = "#2d45ff";
                }
                else if (StoreId == "3") /*1407*/
                {
                    results.backgroundColor.Add("#a581ff");
                    results.borderColor = "#a581ff";
                }
                else if (StoreId == "4") /*180*/
                {
                    results.backgroundColor.Add("#e2601d");
                    results.borderColor = "#e2601d";
                }
                else/*170*/
                {
                    results.backgroundColor.Add("#339db4");
                    results.borderColor = "#339db4";
                }
                results.backgroundColor.Add("#dddddd");
                results.borderColor = "#dddddd";

                foreach (var item in Dashboard)
                {
                    results.data.Add(item.PercentData.ToString());
                    if (item.Weeks == "C")
                    {
                        results.data.Add((100 - item.PercentData).ToString("#.##"));
                    }
                }
                result.DatsetListDoughnutLists.Add(results);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetChartDataDoughnutPeriodic2 - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return result;
        }

        public DashboardChartViewModel GetPeriodicBarChartData(string StoreId, string Department, int Type, int StateId, DateTime Start, DateTime End)
        {
            DashboardChartViewModel result = new DashboardChartViewModel();
            try
            {
                List<DashboardWeeklySelectChart> Dashboard = _context.Database.SqlQuery<DashboardWeeklySelectChart>("SP_Dashboard_Periodic_Chart @StoreID = {0},@Type={1},@Deaprtment={2},@StartDate={3},@EndDate={4},@StateId={5}", StoreId, Type, Department, Start, End, (StateId == null ? 0 : StateId)).ToList();

                var rnd = new Random();
                var StoreDisctinctList = Dashboard.Select(s => s.StoreId).Distinct();
                var store = _context.StoreMasters.ToList().Where(a => StoreDisctinctList.Contains(a.StoreId)).ToList();
                int i = 1;
                foreach (var item in Dashboard)
                {
                    var results = new DatsetList();
                    results.code = item.StoreId.ToString();
                    results.label = item.Hours;
                    if (i == 1)
                    {
                        results.backgroundColor = "#f3db27";
                        results.borderColor = "#f3db27";

                    }
                    else if (i == 2)
                    {

                        results.backgroundColor = "#CDDC39";
                        results.borderColor = "#CDDC39";
                    }
                    else
                    {
                        results.backgroundColor = "#689F38";
                        results.borderColor = "#689F38";

                    }

                    results.fill = false;
                    result.DatsetLists.Add(results);
                    i += 1;
                };

                i = 1;
                result.DatsetLists.ForEach(a =>
                {
                    for (int j = 0; j < Dashboard.Count; j++)
                    {
                        if (j == 0 && i == 1)
                        {
                            a.data.Add(Dashboard[j].Amount.ToString());
                            break;
                        }
                        else if (j == 1 && i == 2)
                        {
                            a.data.Add(Dashboard[j].Amount.ToString());
                            break;
                        }
                        else if (j == 2 && i == 3)
                        {
                            a.data.Add(Dashboard[j].Amount.ToString());
                            break;
                        }
                    }
                    i += 1;

                });
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetPeriodicBarChartData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return result;
        }

        public HourlyChartViewModel GetHourlyChartDataPeriodic(string StoreId, string Department, int Type, int? StateId, DateTime Start, DateTime End)
        {
            HourlyChartViewModel result = new HourlyChartViewModel();
            try
            {
                List<DashboardWeeklySelectChart> Dashboard = _context.Database.SqlQuery<DashboardWeeklySelectChart>("SP_Dashboard_Periodic_Chart @StoreID = {0},@Type={1},@Deaprtment={2},@StartDate={3},@EndDate={4},@StateId={5}", StoreId, Type, Department, Start, End, (StateId == null ? 0 : StateId)).ToList();

                var HoursList = Dashboard.Select(s => s.Hours).Distinct();
                foreach (var item in Dashboard)
                {
                    result.Labels.Add(item.StoreId.ToString());
                    var results = new HourlyDatsetList();
                }
                var rnd = new Random();
                int i = 0;
                foreach (var item in Dashboard)
                {
                    var results = new HourlyDatsetList();
                    results.label = item.StoreId.ToString();
                    if (item.StoreId == 1) /*7484*/
                    {
                        results.backgroundColor = "#f3db27";
                    }
                    else if (item.StoreId == 5) /*2589*/
                    {
                        results.backgroundColor = "#7f0126";
                    }
                    else if (item.StoreId == 6) /*14 th street*/
                    {
                        results.backgroundColor = "#11fa30";
                    }
                    else if (item.StoreId == 2) /*maywood*/
                    {
                        results.backgroundColor = "#949494";
                    }
                    else if (item.StoreId == 7)/*2840*/
                    {
                        results.backgroundColor = "#2d45ff";
                    }
                    else if (item.StoreId == 3) /*1407*/
                    {
                        results.backgroundColor = "#a581ff";
                    }
                    else if (item.StoreId == 4) /*180*/
                    {
                        results.backgroundColor = "#e2601d";
                    }
                    else/*170*/
                    {
                        results.backgroundColor = "#339db4";
                    }
                    results.radius = 4;
                    result.HourlyDatsetList.Add(results);
                    i += 1;
                }
                result.HourlyDatsetList.ForEach(a =>
                {
                    foreach (var item in Dashboard)
                    {
                        if (a.label.ToString() == item.StoreId.ToString())
                        {
                            a.data.Add(new hourlydata() { x = item.Hours == null ? "" : item.Hours.ToString(), y = item.Amount.ToString() });
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetHourlyChartDataPeriodic - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return result;
        }

        public List<DashboardChartToolTipData> getPeriodicHourlyChartToolTipData(string StoreId, string Department, int Type, DateTime Start, DateTime End)
        {
            List<DashboardChartToolTipData> ToolTipData = new List<DashboardChartToolTipData>();
            try
            {
                ToolTipData = _context.Database.SqlQuery<DashboardChartToolTipData>("Sp_getToolTipDataPeriodic @StoreID = {0},@Type={1},@Department={2},@Date={3},@ChartType={4},@Hours={5},@StartDate={6},@EndDate={7}", StoreId, Type, Department, null, "", "1", Start, End).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetPeriodicHourlyChartToolTipData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return ToolTipData;
        }

        public DashboardDaily GetPayrollSalesBoxesDataPeriodic(string Department, int? StateId, int StoreID, DateTime Start, DateTime End)
        {
            DashboardDaily obj = new DashboardDaily();
            try
            {
                List<DashboardWeeklySelect> Dashboard = _context.Database.SqlQuery<DashboardWeeklySelect>("SP_Dashboard_PayrollSalesBoxesData_Periodic @StartDate={0},@EndDate={1},@StoreID={2},@Department={3},@StateId={4}", Start, End, StoreID, Department, (StateId == null ? 0 : StateId)).ToList();

                DashboardWeeklySelect obj2 = new DashboardWeeklySelect();
                if (Dashboard.Count() > 0)
                {
                    obj2 = Dashboard.FirstOrDefault();
                    obj.PayrollOverTime = obj2.PayrollOverTime.Value;
                    obj.PayrollSickPay = obj2.PayrollSickPay.Value;
                    obj.PayrollVacation = obj2.PayrollVacation.Value;
                    obj.PayrollSalary = obj2.PayrollSalary == null ? 0 : obj2.PayrollVacation.Value;
                    obj.PayrollHolidays = obj2.PayrollHolidays.Value;
                    obj.PayrollBonus = obj2.PayrollBonus.Value;
                    obj.PayrollRegularPay = obj2.PayrollRegularPay.Value;
                    obj.SalesRegularPay = obj2.SalesRegularPay.Value;
                    obj.SalesOverTime = obj2.SalesOverTime.Value;
                    obj.SalesSalary = obj2.SalesSalary.Value;
                    obj.SalesOtherpay = obj2.SalesOtherpay.Value;
                }
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetPayrollSalesBoxesDataPeriodic - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public DashboardWeeklyExpenses GetExpensesBoxesDataPeriodic(string StoreID, int? StateId, DateTime Start, DateTime End)
        {
            DashboardWeeklyExpenses obj = new DashboardWeeklyExpenses();
            try
            {
                List<ExpensesWeeklyBox> Dashboard = _context.Database.SqlQuery<ExpensesWeeklyBox>("ExpensesList_ForDashboard_Proc_Periodic @StartDate={0},@EndDate={1},@StoreID={2},@StateId={3}", Start, End, StoreID, (StateId == null ? 0 : StateId)).ToList();

                obj.spDelivery = "DELIVERY EXPENSE";
                obj.spOffice = "OFFICE EXPENSES";
                obj.spProduce = "PRODUCE BUYER";
                obj.spRepairs = "REPAIRS & MAINTENANCE";
                obj.spSupplies = "SUPPLIES";
                obj.spExterminator = "EXTERMINATOR";
                obj.spManagement = "MANAGEMENT FEES";

                obj.ExpDelivery = "0.00%";
                obj.ExpOffice = "0.00%";
                obj.ExpProduce = "0.00%";
                obj.ExpRepairs = "0.00%";
                obj.ExpSupplies = "0.00%";
                obj.ExpExterminator = "0.00%";
                obj.ExpManagement = "0.00%";

                ExpensesWeeklyBox obj2 = new ExpensesWeeklyBox();
                if (Dashboard.Count() > 0)
                {
                    foreach (var r in Dashboard)
                    {
                        switch (r.DepartmentName)
                        {
                            case "DELIVERY EXPENSE":

                                obj.spvDelivery = r.Amount;
                                obj.ExpDelivery = r.ExpPercentage;
                                break;
                            case "OFFICE EXPENSES":

                                obj.spvOffice = r.Amount;
                                obj.ExpOffice = r.ExpPercentage;
                                break;
                            case "PRODUCE BUYER":

                                obj.spvProduce = r.Amount;
                                obj.ExpProduce = r.ExpPercentage;
                                break;
                            case "REPAIRS & MAINTENANCE":
                                obj.spvRepairs = r.Amount;
                                obj.ExpRepairs = r.ExpPercentage;
                                break;
                            case "SUPPLIES":

                                obj.spvSupplies = r.Amount;
                                obj.ExpSupplies = r.ExpPercentage;
                                break;
                            case "EXTERMINATOR":

                                obj.spvExterminator = r.Amount;
                                obj.ExpExterminator = r.ExpPercentage;
                                break;
                            case "MANAGEMENT FEES":

                                obj.spvManagement = r.Amount;
                                obj.ExpManagement = r.ExpPercentage;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetExpensesBoxesDataPeriodic - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public WeeklyPeriod GetWeekDate(string WeeklyPeriodId)
        {
            WeeklyPeriod WeeklyRange = new WeeklyPeriod();
            try
            {
                WeeklyRange = _context.weeklyPeriods.Find(Convert.ToInt32(WeeklyPeriodId));
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetWeekDate - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return WeeklyRange;
        }

        public WeeklyComparisionData GetPeriodicComparisionData(string StoreId, string Department, int Type, int? StateId, DateTime Start, DateTime End)
        {
            WeeklyComparisionData objWeeklyComparisionData = new WeeklyComparisionData();
            try
            {
                List<WeeklyComparisionData> DashboardWeeklyData = _context.Database.SqlQuery<WeeklyComparisionData>("SP_Dashboard_Periodic_Comparison @StoreID = {0},@Type={1},@Deaprtment={2},@StartDate={3},@EndDate={4},@StateId={5}", StoreId, Type, Department, Start, End, (StateId == null ? 0 : StateId)).ToList();

                if (DashboardWeeklyData.Count() > 0)
                {
                    objWeeklyComparisionData = DashboardWeeklyData.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetPeriodicComparisionData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return objWeeklyComparisionData;
        }

        public string ViewBagPeriod()
        {
            string dates = "";
            try
            {
                var SDate = _context.weeklyPeriods.Where(x => x.WNumber == 1 && x.Year == DateTime.Now.Year).Select(x => x.StartDate).FirstOrDefault();
                var EDate = (_context.weeklyPeriods.Where(x => x.Year == DateTime.Now.Year && x.EndDate <= DateTime.Today).Count() > 0) ? _context.weeklyPeriods.Where(x => x.Year == DateTime.Now.Year && x.EndDate <= DateTime.Today).Max(x => x.EndDate) : DateTime.Today;
                dates = SDate.ToString("MM/dd/yyyy") + " To " + EDate.ToString("MM/dd/yyyy");
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - ViewBagPeriod - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return dates;
        }

        public YearlySales DepartmentList()
        {
            YearlySales sales = new YearlySales();
            try
            {
                sales.DepartmentList = _context.Database.SqlQuery<DropdownDepartmentList>("SP_ChartStoreDepartmentData @Mode={0}", "2").ToList();
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - DepartmentList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return sales;
        }

        public List<int> ViewbagYear()
        {
            List<int> list = new List<int>();
            try
            {
                list = _context.weeklyPeriods.Select(s => s.Year).Distinct().OrderByDescending(o => o).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - ViewbagYear - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return list;
        }

        public DashboardViewModel GetYearlySalesData(string Departments, int? StateId, int Year, int StoreID)
        {
            data = new DashboardViewModel();
            try
            {
                if (StoreID == 0)
                {
                    data.sales = _context.Database.SqlQuery<YearlySales>("SP_Dashboard_YTD @Type = {0},@StoreID={1} ,@Department={2},@StateId={3},@Year={4}", 1, StoreID, Departments, StateId, Year).FirstOrDefault();
                    data.growt = _context.Database.SqlQuery<YearlyGrowt>("SP_Dashboard_YTD @Type = {0},@StoreID={1} ,@Department={2},@StateId={3},@Year={4}", 2, StoreID, Departments, StateId, Year).ToList();
                    data.charts = _context.Database.SqlQuery<ChartModel>("SP_Dashboard_YTD @Type = {0},@StoreID={1} ,@Department={2},@StateId={3},@Year={4}", 3, StoreID, Departments, StateId, Year).ToList();
                    data.salesL = _context.Database.SqlQuery<YearlySales>("SP_Dashboard_YTD @Type = {0},@StoreID={1} ,@Department={2},@StateId={3},@Year={4}", 4, StoreID, Departments, StateId, Year).FirstOrDefault();
                }
                else
                {
                    data.sales = _context.Database.SqlQuery<YearlySales>("SP_Dashboard_YTD @Type = {0},@StoreID={1},@Department={2},@StateId={3},@Year={4}", 1, StoreID, Departments, StateId, Year).FirstOrDefault();
                    data.growt = _context.Database.SqlQuery<YearlyGrowt>("SP_Dashboard_YTD @Type = {0},@StoreID={1} ,@Department={2},@StateId={3},@Year={4}", 2, StoreID, Departments, StateId, Year).ToList();
                    data.charts = _context.Database.SqlQuery<ChartModel>("SP_Dashboard_YTD @Type = {0},@StoreID={1} ,@Department={2},@StateId={3},@Year={4}", 3, StoreID, Departments, StateId, Year).ToList();
                    data.salesL = _context.Database.SqlQuery<YearlySales>("SP_Dashboard_YTD @Type = {0},@StoreID={1} ,@Department={2},@StateId={3},@Year={4}", 4, StoreID, Departments, StateId, Year).FirstOrDefault();
                }
               
                var code1 = 0;
                var code2 = 0;
                var code3 = 0;
                foreach (var item in data.charts)
                {
                    data.Labels.Add(item.WeekNo.ToString());
                    data.Data.Add(item.Amount.ToString());
                    code1 = 53;
                    code2 = 227;
                    code3 = 203;
                    data.backgroundColor.Add("rgba(" + code1 + "," + code2 + "," + code3 + "," + 0.2 + ")");
                    data.borderColor.Add("rgba(" + code1 + "," + code2 + "," + code3 + "," + 1 + ")");
                }
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetYearlySalesData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return data;
        }

        public DashboardViewModel GetQatarDate(string Qatar)
        {
            data = new DashboardViewModel();
            try
            {
                var StartDate = "";
                var EndDate = "";
                int[] weekno;
                if (Convert.ToInt32(Qatar) == 1)
                {
                    weekno = new int[] { 1, 13 };
                    var WeeklyRange = _context.weeklyPeriods.Where(w => weekno.Contains(w.WNumber) && w.Year == DateTime.Now.Year).OrderBy(o => o.WNumber).ToList();
                    foreach (var item in WeeklyRange)
                    {
                        if (item.WNumber == 1)
                        {
                            StartDate = item.StartDate.ToString("MM/dd/yyyy");
                        }
                        else
                        {
                            EndDate = item.EndDate.ToString("MM/dd/yyyy");
                        }
                    }
                }
                else if (Convert.ToInt32(Qatar) == 2)
                {
                    weekno = new int[] { 14, 26 };
                    var WeeklyRange = _context.weeklyPeriods.Where(w => weekno.Contains(w.WNumber) && w.Year == DateTime.Now.Year).OrderBy(o => o.WNumber).ToList();
                    foreach (var item in WeeklyRange)
                    {
                        if (item.WNumber == 14)
                        {
                            StartDate = item.StartDate.ToString("MM/dd/yyyy");
                        }
                        else
                        {
                            EndDate = item.EndDate.ToString("MM/dd/yyyy");
                        }
                    }
                }
                else if (Convert.ToInt32(Qatar) == 3)
                {
                    weekno = new int[] { 27, 39 };
                    var WeeklyRange = _context.weeklyPeriods.Where(w => weekno.Contains(w.WNumber) && w.Year == DateTime.Now.Year).OrderBy(o => o.WNumber).ToList();
                    foreach (var item in WeeklyRange)
                    {
                        if (item.WNumber == 27)
                        {
                            StartDate = item.StartDate.ToString("MM/dd/yyyy");
                        }
                        else
                        {
                            EndDate = item.EndDate.ToString("MM/dd/yyyy");
                        }
                    }
                }
                else
                {
                    weekno = new int[] { 40, 52 };
                    var WeeklyRange = _context.weeklyPeriods.Where(w => weekno.Contains(w.WNumber) && w.Year == DateTime.Now.Year).OrderBy(o => o.WNumber).ToList();
                    foreach (var item in WeeklyRange)
                    {
                        if (item.WNumber == 40)
                        {
                            StartDate = item.StartDate.ToString("MM/dd/yyyy");
                        }
                        else
                        {
                            EndDate = item.EndDate.ToString("MM/dd/yyyy");
                        }
                    }
                }
                data.StartDate = StartDate;
                data.EndDate = EndDate;
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetQatarDate - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return data;
        }

        public DashboardViewModel GetYTDDate(string Qatar)
        {
            data = new DashboardViewModel();
            try
            {
                var StartDate = "";
                var EndDate = "";
                if (Qatar == "YTD")
                {
                    var SDate = _context.weeklyPeriods.Where(x => x.WNumber == 1 && x.Year == DateTime.Now.Year).Select(x => x.StartDate).FirstOrDefault();
                    StartDate = SDate.ToString("MM/dd/yyyy");
                    var EDate = _context.weeklyPeriods.Where(x => x.Year == DateTime.Now.Year && x.EndDate <= DateTime.Today).Max(x => x.EndDate);

                    EndDate = EDate.ToString("MM/dd/yyyy");
                }
                if (Qatar == "LFY")
                {
                    var SDate = _context.weeklyPeriods.Where(x => x.WNumber == 1 && x.Year == DateTime.Now.Year - 1).Select(x => x.StartDate).FirstOrDefault();
                    StartDate = SDate.ToString("MM/dd/yyyy");
                    var EDate = _context.weeklyPeriods.Where(x => x.WNumber == 52 && x.Year == DateTime.Now.Year - 1).Select(x => x.EndDate).FirstOrDefault();
                    if (EDate.Year == DateTime.Now.Year)
                    {
                        EndDate = "12/31/" + (DateTime.Now.Year - 1).ToString();
                    }
                    else
                    {
                        EndDate = EDate.ToString("MM/dd/yyyy");
                    }
                }
                data.StartDate = StartDate;
                data.EndDate = EndDate;
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetYTDDate - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return data;
        }

        public string GetStateName(int? StateId)
        {
            string StoreName = "";
            try
            {
                if(StateId != 0)
                {
                    StoreName = _context.StateMasters.Where(a => a.StateId == StateId).FirstOrDefault().StateCode;
                }
            }
            catch (Exception ex)
            {
                logger.Error("CommonRepository - GetStateName - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return StoreName;
        }

        //Himanshu
        public List<DashboardDailySelectChart> GetChartDataForPDF(string StoreId, string Department, int Type, string Date, int? StateId, int? GroupID)
        {
            List<DashboardDailySelectChart> Dashboard = new List<DashboardDailySelectChart>();
            try
            {
                if (StateId == null)
                {
                    StateId = 0;
                }
                Dashboard = _context.Database.SqlQuery<DashboardDailySelectChart>("SP_Dashboard_Daily_Chart @StoreID = {0},@Type={1},@Deaprtment={2},@Date={3},@StateId={4},@GroupID={5}", StoreId, Type, Department, Date, StateId, GroupID).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetChartData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Dashboard;


        }

        public List<int> GetStoreBySelectedStateId(int? stateid)
        {
            List<int> list = new List<int>();
            try
            {
                list = _context.StoreMasters.Where(s => s.StateID == stateid).Select(s => s.StoreId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetStoreBySelectedStateId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return list;
        }

        public List<SalesDataModel> GetNetSalesData(DateTime startdate, DateTime enddate, string storeids)
        {
            List<SalesDataModel> salesData = new List<SalesDataModel>();

            try
            {
                var rawData = _context.Database.SqlQuery<ExportSalesDataResult>("SP_Dashboard_NetSaleExport @Startdate = {0}, @Enddate = {1}, @StoreIds = {2}",startdate, enddate, storeids).ToList();

                // Pivot data
                var groupedData = rawData.GroupBy(x => new { x.StartDate,x.StoreName })
                                         .Select(g => new SalesDataModel
                                         {
                                             StartDate = g.Key.StartDate,
                                             StoreName = g.Key.StoreName,
                                             CustomerCount = g.First().CustomerCount,
                                             AverageSale = Math.Round(g.First().AverageSale, 2),
                                             CategorySales = g.ToDictionary(k => k.MergedDepartment, v => v.TotalAmount)
                                         }).OrderBy(x=>x.StartDate).ThenBy(x=>x.StoreName).ToList();

                return groupedData;
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetNetSalesData - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return salesData;
        }

        public List<GroupWiseStateStore> ViewBagGroupId(int UserId)
        {
            List<GroupWiseStateStore> list = new List<GroupWiseStateStore>();
            try
            {
                var GroupId = _context.UserMasters.Where(s => s.UserId == UserId).Select(s => s.GroupWiseStateStoreId).FirstOrDefault();
                if (GroupId != null)
                {
                    list = _context.GroupWiseStateStores.Where(s => s.GroupWiseStateStoreId == GroupId).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - ViewBagStateId - " + DateTime.Now + " - " + ex.Message.ToString());
            }   
            return list;
        }

        public List<int> GetStoreBySelectedGroupId(int? GroupID, int UserId)
        {
            List<int> list = new List<int>();
            try
            {
                var GroupId = _context.UserMasters.Where(s => s.UserId == UserId).Select(s => s.GroupWiseStateStoreId).FirstOrDefault();
                var storenames = _context.GroupWiseStateStores.Where(s => s.GroupWiseStateStoreId == GroupId).Select(s => s.StoreName).FirstOrDefault();
                if (!string.IsNullOrEmpty(storenames))
                {
                    list = storenames.Split(',').Select(s => int.Parse(s.Trim())).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error("DashboardRepository - GetStoreBySelectedStateId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return list;
        }
    }
}
