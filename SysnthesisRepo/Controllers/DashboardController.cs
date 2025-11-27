using Aspose.Pdf.Operators;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using EntityModels.Models;
using HtmlAgilityPack;
using iTextSharp.text.html;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Newtonsoft.Json;
using NLog;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Repository;
using Repository.IRepository;
using Syncfusion.EJ2.Spreadsheet;
using SynthesisViewModal;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Xml;
using Utility;
using static Utility.AdminSiteConfiguration;

namespace SysnthesisRepo.Controllers
{
    public class DashboardController : Controller
    {
        DashboardViewModel data = new DashboardViewModel();
        private readonly IDashboardRepository _DashboardRepository;
        private readonly ISynthesisApiRepository _SynthesisApiRepository;
        private readonly ICommonRepository _CommonRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        public DashboardController()
        {
            this._DashboardRepository = new DashboardRepository(new DBContext(), new DBContextHR());
            this._SynthesisApiRepository = new SynthesisApiRepository();
            this._CommonRepository = new CommonRepository(new DBContext());
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        #region Daily Data Dashboard

        /// <summary>
        /// This method return Daily Dashboard.
        /// </summary>
        /// <param name="StateId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,ViewDashboardDailyDashboard,ViewDailyDashboard,ViewPeriodicDashboard")]
        public ActionResult Daily(int? StateId,int? GroupID)
        {
            ViewBag.Title = "Daily Dashboard - Synthesis";
            _CommonRepository.LogEntries();     //Harsh's code
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            //Db class for get user type Id.
            var UserTypeId = _CommonRepository.getUserTypeId(UserName);
            var UserId = _CommonRepository.getUserId(UserName);

            DashboardDaily obj = new DashboardDaily();
            try
            {
                int StoreID = 0;
                if (Session["storeid"] != null)
                {
                    StoreID = Convert.ToInt32(Convert.ToString(Session["storeid"]));
                }
                ViewBag.Storeidvalue = StoreID;
                if (UserTypeId != 1)
                {
                    if (StoreID == 0)
                    {
                        //Db class is Check Error by usertype id and StoreID.
                        var Roles = _DashboardRepository.CheckError(UserTypeId, StoreID);
                        if (!Roles.Contains("ViewDashboardDailyDashboard"))
                        {
                            return View("~/Views/Shared/Error.cshtml");
                            //goto Exit;
                        }
                    }
                    else
                    {
                        //Db class is Check Error by usertype id and StoreID.
                        var Roles = _DashboardRepository.CheckError(UserTypeId, StoreID);
                        if (!Roles.Contains("ViewDailyDashboard"))
                        {
                            return View("~/Views/Shared/Error.cshtml");
                            //goto Exit;
                        }
                    }
                }
                if (StateId == null)
                {
                    StateId = 0;
                }

                //Using this db class get startdate.
                var Startdate = _DashboardRepository.Startdate(StoreID);
                ViewBag.Startdate = Startdate;
                //Get Dashboard Daily Details using Startdate,storeId and StateId.
                obj = _DashboardRepository.Dashboard_Daily(Startdate, StoreID, StateId, GroupID);
                //List<DailyDashboardXML> dailyDashboardXML = new List<DailyDashboardXML>();
                //for (int i = 0; i < 24; i++)
                //{
                //    dailyDashboardXML.Add(new DailyDashboardXML
                //    {
                //        Hours = Convert.ToDateTime("03-30-2024").AddHours(i),
                //        dashboardDaily = obj
                //    });
                //}
                //string dashboardJson = JsonConvert.SerializeObject(dailyDashboardXML);
                //// To convert JSON text contained in string json into an XML node
                //XmlDocument doc = JsonConvert.DeserializeXmlNode(dashboardJson);
                ViewBag.StateId = new SelectList(_DashboardRepository.ViewBagStateId(), "StateId", "StateCode");
                
                ViewBag.GroupWiseStateStoreId = new SelectList(_DashboardRepository.ViewBagGroupId(UserId), "GroupWiseStateStoreId", "GroupName");

                //var groupList = _DashboardRepository.ViewBagGroupId(UserId);
                //var selectedGroupId = groupList.Select(s => s.GroupWiseStateStoreId).FirstOrDefault();
                //ViewBag.GroupWiseStateStoreId = selectedGroupId;
                //obj.GroupWiseStateStoreList = groupList.Select(g => new GroupOption
                //{
                //    Id = g.GroupWiseStateStoreId,
                //    Name = g.GroupName
                //}).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - Daily - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View(obj);
        }



        /// <summary>
        /// This is Get Daily Dashborad Data.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="StateId"></param>
        /// <returns></returns>
        public JsonResult getDashboardDailyData(string date, int? StateId, string StoreId, int? GroupID)
        {
            DashboardDaily obj = new DashboardDaily();
            try
            {
                string StoreID = "0";
                if (Session["storeid"] != null)
                {
                    StoreID = Convert.ToString(Session["storeid"]);
                }
                if (StoreId != null && StoreId != "")
                {
                    StoreID = StoreId;
                }
                if (StateId == null)
                {
                    StateId = 0;
                }
                if (GroupID == null)
                {
                    GroupID = 0;
                }
                ViewBag.Startdate = date;
                //Get Dashboard Daily data.
                obj = _DashboardRepository.getDashboardDailyData(date, StoreID, StateId, GroupID);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - getDashboardDailyData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getDashboardDailyDataStoreWise(string date, int? StateId, string StoreId, int? GroupID)
        {
            DashboardDaily obj = new DashboardDaily();
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
                string StoreID = "0";
                if (StoreId != null && StoreId != "")
                {
                    StoreID = StoreId;
                }
                ViewBag.Startdate = date;
                //Get Dashboard Daily data.
                obj = _DashboardRepository.getDashboardDailyData(date, StoreID, StateId,GroupID);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - getDashboardDailyDataStoreWise - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method Get Sales One Hour Worked Data.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="department"></param>
        /// <param name="StateId"></param>
        /// <returns></returns>
        public JsonResult GetSalesOneHourWorkedData(string date, string department, int? StateId, int? GroupID)
        {
            int StoreID = 0;
            List<SalesOneHourWorkedDataList> StoreHourlyWorkData = new List<SalesOneHourWorkedDataList>();
            try
            {
                if (Session["storeid"] != null)
                {
                    StoreID = Convert.ToInt32(Convert.ToString(Session["storeid"]));
                }
                if (StateId == null)
                {
                    StateId = 0;
                }
                if (GroupID == null)
                {
                    GroupID = 0;
                }
                //Get all sales One Hour Worked data.
                StoreHourlyWorkData = _DashboardRepository.GetSalesOneHourWorkedData(date, StoreID, department, StateId, GroupID);
                return Json(StoreHourlyWorkData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - GetSalesOneHourWorkedData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json("Error", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method returm Chart Data.
        /// </summary>
        /// <param name="StoreId"></param>
        /// <param name="Department"></param>
        /// <param name="Type"></param>
        /// <param name="Date"></param>
        /// <param name="StateId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetChartData(string StoreId, string Department, int Type, string Date, int? StateId, int? GroupID)
        {
            var result = new DashboardChartViewModel();
            //Using this db class get Chart data.
            result = _DashboardRepository.GetChartData(StoreId, Department, Type, Date, StateId,GroupID);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Get Sales Total Details Data.
        /// </summary>
        /// <param name="StoreId"></param>
        /// <param name="Department"></param>
        /// <param name="Type"></param>
        /// <param name="Date"></param>
        /// <param name="StateId"></param>
        /// <returns></returns>
        public JsonResult GetSalesTotalDetailsdata(string StoreId, string Department, int Type, string Date, int? StateId, int? GroupID)
        {
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
                //Get List of sales Total Details.
                List<DashboardDailyTotalData> Dashboard = _DashboardRepository.GetSalesTotalDetailsdata(StoreId, Department, Type, Date, StateId, GroupID);

                return Json(Dashboard, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - GetSalesTotalDetailsdata - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// This method is Get Weekly Sales Total Details data.
        /// </summary>
        /// <param name="StoreId"></param>
        /// <param name="Department"></param>
        /// <param name="Type"></param>
        /// <param name="WeeklyPeriodId"></param>
        /// <param name="StateId"></param>
        /// <returns></returns>
        public JsonResult GetWeeklySalesTotalDetailsdata(string StoreId, string Department, int Type, string WeeklyPeriodId, int? StateId)
        {
            try
            {
                if (StateId == null)
                {
                    StateId = 0;
                }
                //Get Weekly Sales total Details.
                List<DashboardDailyTotalData> Dashboard = _DashboardRepository.GetWeeklySalesTotalDetailsdata(StoreId, Department, Type, WeeklyPeriodId, StateId);
                return Json(Dashboard, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - GetWeeklySalesTotalDetailsdata - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// This method is Get  Weekly Sales Daily Total Details data.
        /// </summary>
        /// <param name="StoreId"></param>
        /// <param name="Department"></param>
        /// <param name="Type"></param>
        /// <param name="WeeklyPeriodId"></param>
        /// <param name="StateId"></param>
        /// <returns></returns>
        public JsonResult GetWeeklySalesDailyTotalDetailsdata(string StoreId, string Department, int Type, string WeeklyPeriodId, int? StateId)
        {
            try
            {
                if (Session["storeid"] != null)
                {
                    StoreId = Convert.ToString(Session["storeid"]);
                }
                if (StateId == null)
                {
                    StateId = 0;
                }
                //Get Weekly Sales daiky total Details.
                List<DashboardDailyTotalData> Dashboard = _DashboardRepository.GetWeeklySalesDailyTotalDetailsdata(StoreId, Department, Type, WeeklyPeriodId, StateId);
                return Json(Dashboard, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - GetWeeklySalesDailyTotalDetailsdata - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// This method is Get Hourly Chart Data.
        /// </summary>
        /// <param name="StoreId"></param>
        /// <param name="Department"></param>
        /// <param name="Type"></param>
        /// <param name="Date"></param>
        /// <param name="StateId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetHourlyChartData(string StoreId, string Department, int Type, string Date, int? StateId, int? GroupID)
        {
            var result = new HourlyChartViewModel();
            // Get Hourly chart data.
            try
            {
                result = _DashboardRepository.GetHourlyChartData(StoreId, Department, Type, Date, StateId, GroupID);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - GetHourlyChartData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Get Chart Data Doughnut.
        /// </summary>
        /// <param name="StoreId"></param>
        /// <param name="Department"></param>
        /// <param name="WeekNo"></param>
        /// <param name="Type"></param>
        /// <param name="StateId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetChartDataDoughnut(string StoreId, string Department, string WeekNo, int Type, int? StateId)
        {
            try
            {
                StoreId = "0";
                if (Session["storeid"] != null)
                {
                    StoreId = Convert.ToString(Session["storeid"]);
                }
                //Get Chart data Doughtnut.
                var result = _DashboardRepository.GetChartDataDoughnut(StoreId, Department, WeekNo, Type, StateId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - GetChartDataDoughnut - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// This method is Get Chart Data Bar.
        /// </summary>
        /// <param name="StoreId"></param>
        /// <param name="Department"></param>
        /// <param name="Type"></param>
        /// <param name="Date"></param>
        /// <param name="StateId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetChartDataBar(string StoreId, string Department, int Type, string Date, int? StateId, int? GroupID)
        {
            var result = new DashboardChartViewModel();
            result = _DashboardRepository.GetChartDataBar(StoreId, Department, Type, Date, StateId, GroupID);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Get Bank Account Details Value
        /// </summary>
        /// <returns></returns>
        public JsonResult BankAccountDetailValue()
        {
            try
            {
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                //Db class for get user type Id.
                var UserTypeId = _CommonRepository.getUserTypeId(UserName);
                int StoreID = 0;
                if (Session["storeid"] != null)
                {
                    StoreID = Convert.ToInt32(Convert.ToString(Session["storeid"]));
                }
                List<BankAccountDetail> banks = new List<BankAccountDetail>();
                string UserNm = Session["UserNm"].ToString();
                //Using this db class Get Bank Account Detals.
                banks = _DashboardRepository.BankAccountDetailValue(UserTypeId, StoreID, UserNm);
                return Json(banks, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - BankAccountDetailValue - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// This method is Get Bank account Details Value Refrensh.
        /// </summary>
        /// <returns></returns>
        public JsonResult BankAccountDetailValueRefresh()
        {
            try
            {
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                //Db class for get user type Id.
                var UserTypeId = _CommonRepository.getUserTypeId(UserName);
                int StoreID = 0;
                if (Session["storeid"] != null)
                {
                    StoreID = Convert.ToInt32(Convert.ToString(Session["storeid"]));
                }
                List<BankAccountDetail> banks = new List<BankAccountDetail>();
                //Using this db class Get Bank Account Detals Refresh.
                banks = _DashboardRepository.BankAccountDetailValueRefresh(UserTypeId, StoreID);
                return Json(banks, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - BankAccountDetailValueRefresh - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Weekly Data Dashboard
        /// <summary>
        /// This method is View of Weekly Dashborad.
        /// </summary>
        /// <param name="StateId"></param>
        /// <returns></returns>
        public ActionResult Weekly(int? StateId)
        {
            ViewBag.Title = "Weekly Dashboard - Synthesis";
            _CommonRepository.LogEntries();     //Harsh's code
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            //Db class for get user type Id.
            var UserTypeId = _CommonRepository.getUserTypeId(UserName);
            DashboardDaily obj = new DashboardDaily();
            try
            {
                if (StateId == null)
                {
                    StateId = 0;
                }
                if (Session["storeid"] == null)
                {
                    int StoreID = 0;
                    ViewBag.Storeidvalue = 0;
                    if (UserTypeId != 1)
                    {
                        if (StoreID == 0)
                        {
                            //Db class is Check Error by usertype id and StoreID.
                            var Roles = _DashboardRepository.CheckError(UserTypeId, StoreID);
                            if (!Roles.Contains("ViewDashboardWeeklyDashboard"))
                            {
                                return View("~/Views/Shared/Error.cshtml");
                                //goto Exit;
                            }
                        }
                    }
                }
                else
                {
                    int StoreID = 0;
                    if (Session["storeid"] != null)
                    {
                        StoreID = Convert.ToInt32(Convert.ToString(Session["storeid"]));
                    }
                    ViewBag.Storeidvalue = StoreID;

                    if (UserTypeId != 1)
                    {
                        //Db class is Check Error by usertype id and StoreID.
                        var Roles = _DashboardRepository.CheckError(UserTypeId, StoreID);
                        if (!Roles.Contains("ViewWeeklyDashboard"))
                        {
                            return View("~/Views/Shared/Error.cshtml");
                            //goto Exit;
                        }
                    }
                    //Using this db class get startdate.
                    var Startdate = _DashboardRepository.Startdate(StoreID);
                    ViewBag.Startdate = Startdate;
                    //Get Weekly data using Startdate,stroreId and stateid.
                    obj = _DashboardRepository.Weekly(Startdate, StoreID, StateId);

                }
                _DashboardRepository.Weekly_1(StateId, ref obj);
                DateTime dtStart = DateTime.Today.AddDays(-7);
                if (dtStart.Year != DateTime.Today.Year)
                {
                    dtStart = DateTime.Today;
                }
                //get Weekly Period Id.
                var WeeklyPeriodId = _DashboardRepository.WeeklyPeriodId(dtStart);
                //get All Chart Store Department Data.
                ViewBag.WeeklyPeriodId = new SelectList(_DashboardRepository.ChartStoreDepartmentData(StateId, dtStart), "Id", "Text", WeeklyPeriodId);
                //Get All View StateID.
                ViewBag.StateId = new SelectList(_DashboardRepository.ViewBagStateId(), "StateId", "StateCode");
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - Weekly - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View(obj);
        }

        /// <summary>
        /// This method is Get Dashborad Weekly data.
        /// </summary>
        /// <param name="WeeklyPeriodId"></param>
        /// <param name="StateId"></param>
        /// <returns></returns>
        public ActionResult getDashboardWeeklyData(int WeeklyPeriodId, int? StateId)
        {
            DashboardDaily obj = new DashboardDaily();
            try
            {
                if (StateId == null)
                {
                    StateId = 0;
                }

                int StoreID = 0;
                if (Session["storeid"] != null)
                {
                    StoreID = Convert.ToInt32(Convert.ToString(Session["storeid"]));
                }
                ViewBag.WeeklyPeriodId = WeeklyPeriodId;
                //using this db class Get dashboard Weekly data.
                obj = _DashboardRepository.getDashboardWeeklyData(WeeklyPeriodId, StateId, StoreID);

            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - getDashboardWeeklyData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Get Payroll Sales Boxes data.
        /// </summary>
        /// <param name="WeeklyPeriodId"></param>
        /// <param name="Department"></param>
        /// <param name="StateId"></param>
        /// <returns></returns>
        public ActionResult GetPayrollSalesBoxesData(int WeeklyPeriodId, string Department, int? StateId)
        {
            DashboardDaily obj = new DashboardDaily();
            try
            {
                int StoreID = 0;
                if (Session["storeid"] != null)
                {
                    StoreID = Convert.ToInt32(Convert.ToString(Session["storeid"]));
                }
                if (StateId == null)
                {
                    StateId = 0;
                }
                //Get payroll Sales Boxes data.
                obj = _DashboardRepository.GetPayrollSalesBoxesData(WeeklyPeriodId, Department, StateId, StoreID);

                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - GetPayrollSalesBoxesData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Get Weekly Chart data.
        /// </summary>
        /// <param name="StoreId"></param>
        /// <param name="Department"></param>
        /// <param name="Type"></param>
        /// <param name="WeeklyPeriodId"></param>
        /// <param name="StateId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetWeeklyChartData(string StoreId, string Department, int Type, int WeeklyPeriodId, int? StateId)
        {
            var result = new DashboardChartViewModel();
            try
            {
                if (StateId == null)
                {
                    StateId = 0;
                }
                //Get Weekly chart data.
                result = _DashboardRepository.GetWeeklyChartData(StoreId, Department, Type, WeeklyPeriodId, StateId);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - GetWeeklyChartData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Get Weekly Bar Chart data.
        /// </summary>
        /// <param name="StoreId"></param>
        /// <param name="Department"></param>
        /// <param name="Type"></param>
        /// <param name="WeeklyPeriodId"></param>
        /// <param name="StateId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetWeeklyBarChartData(string StoreId, string Department, int Type, int WeeklyPeriodId, int? StateId)
        {
            var result = new DashboardChartViewModel();
            try
            {
                if (Session["storeid"] != null)
                {
                    StoreId = Convert.ToString(Session["storeid"]);
                }
                if (StateId == null)
                {
                    StateId = 0;
                }
                //Using this db class get weekly bar chart data.
                result = _DashboardRepository.GetWeeklyBarChartData(StoreId, Department, Type, WeeklyPeriodId, StateId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - GetWeeklyBarChartData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// This method is Get Weekly Comparision Data.
        /// </summary>
        /// <param name="StoreId"></param>
        /// <param name="Department"></param>
        /// <param name="Type"></param>
        /// <param name="WeeklyPeriodId"></param>
        /// <param name="StateId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetWeeklyComparisionData(string StoreId, string Department, int Type, int WeeklyPeriodId, int? StateId)
        {
            WeeklyComparisionData objWeeklyComparisionData = new WeeklyComparisionData();
            try
            {
                if (Session["storeid"] != null)
                {
                    StoreId = Convert.ToString(Session["storeid"]);
                }
                if (StateId == null)
                {
                    StateId = 0;
                }
                //get Weekly Comparision data.
                objWeeklyComparisionData = _DashboardRepository.GetWeeklyComparisionData(StoreId, Department, Type, WeeklyPeriodId, StateId);

                return Json(objWeeklyComparisionData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - GetWeeklyComparisionData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(objWeeklyComparisionData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="StoreId"></param>
        /// <param name="Department"></param>
        /// <param name="Type"></param>
        /// <param name="WeeklyPeriodId"></param>
        /// <param name="StateId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetHourlyChartDataWeek(string StoreId, string Department, int Type, int WeeklyPeriodId, int? StateId)
        {
            var result = new HourlyChartViewModel();
            try
            {
                if (StateId == null)
                {
                    StateId = 0;
                }

                ////using this db class get hourly chart data Week.
                result = _DashboardRepository.GetHourlyChartDataWeek(StoreId, Department, Type, WeeklyPeriodId, StateId);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - GetHourlyChartDataWeek - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Get Sales one hour Worked data by Weekly.
        /// </summary>
        /// <param name="WeeklyPeriodId"></param>
        /// <param name="department"></param>
        /// <param name="StateId"></param>
        /// <returns></returns>
        public ActionResult GetSalesOneHourWorkedDataWeekly(int WeeklyPeriodId, string department, int? StateId)
        {
            List<SalesOneHourWorkedDataList> StoreHourlyWorkData = new List<SalesOneHourWorkedDataList>();
            try
            {
                int StoreID = 0;
                if (Session["storeid"] != null)
                {
                    StoreID = Convert.ToInt32(Convert.ToString(Session["storeid"]));
                }
                if (StateId == null)
                {
                    StateId = 0;
                }
                StoreHourlyWorkData = _DashboardRepository.GetSalesOneHourWorkedDataWeekly(WeeklyPeriodId, department, StateId, StoreID);
                //Get all Sales One Hour worked weekly data.
                return Json(StoreHourlyWorkData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - GetSalesOneHourWorkedDataWeekly - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(StoreHourlyWorkData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Get ToolTip data.
        /// </summary>
        /// <param name="StoreId"></param>
        /// <param name="Department"></param>
        /// <param name="Type"></param>
        /// <param name="Date"></param>
        /// <param name="Hours"></param>
        /// <param name="Mode"></param>
        /// <returns></returns>
        public JsonResult getToolTipData(string StoreId, string Department, int Type, string Date, string Hours, string Mode)
        {
            List<DashboardChartToolTipData> ToolTipData = new List<DashboardChartToolTipData>();
            try
            {
                //Get Tool Tip data.
                ToolTipData = _DashboardRepository.getToolTipData(StoreId, Department, Type, Date, Hours, Mode);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - getToolTipData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(ToolTipData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Get Hourly Chart Tool Tip data.
        /// </summary>
        /// <param name="StoreId"></param>
        /// <param name="Department"></param>
        /// <param name="Type"></param>
        /// <param name="Date"></param>
        /// <returns></returns>
        public JsonResult getHourlyChartToolTipData(string StoreId, string Department, int Type, string Date)
        {
            List<DashboardChartToolTipData> ToolTipData = new List<DashboardChartToolTipData>();
            try
            {
                if (Department != "ALL" && Type != 2)
                {
                    //using this db class get tool tip data.
                    ToolTipData = _DashboardRepository.getToolTipData(StoreId, Department, Type, Date, "", "1");
                }
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - getHourlyChartToolTipData - " + DateTime.Now + " - " + ex.Message.ToString());

            }
            return Json(ToolTipData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Get Weekly-Hourly Chart ToolTip data.
        /// </summary>
        /// <param name="StoreId"></param>
        /// <param name="Department"></param>
        /// <param name="Type"></param>
        /// <param name="WeeklyPeriodId"></param>
        /// <returns></returns>
        public JsonResult getWeeklyHourlyChartToolTipData(string StoreId, string Department, int Type, int WeeklyPeriodId)
        {
            List<DashboardChartToolTipData> ToolTipData = new List<DashboardChartToolTipData>();
            try
            {
                //Get Weekly Hourly chart tool tip.
                ToolTipData = _DashboardRepository.getWeeklyHourlyChartToolTipData(StoreId, Department, Type, WeeklyPeriodId);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - getToolTipData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(ToolTipData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Get Expenses Boxes data.
        /// </summary>
        /// <param name="WeeklyPeriodId"></param>
        /// <param name="StoreID"></param>
        /// <param name="StateId"></param>
        /// <returns></returns>
        public ActionResult GetExpensesBoxesData(int WeeklyPeriodId, string StoreID, int? StateId)
        {
            DashboardWeeklyExpenses obj = new DashboardWeeklyExpenses();
            try
            {
                if (StoreID == "")
                    StoreID = "0";
                if (StateId == null)
                {
                    StateId = 0;
                }
                // Get Expenses Boxes data.
                obj = _DashboardRepository.GetExpensesBoxesData(WeeklyPeriodId, StoreID, StateId);

                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - GetExpensesBoxesData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Periodic Data Dashboard
        /// <summary>
        /// This method is return view of Periodic dashborad.
        /// </summary>
        /// <returns></returns>
        public ActionResult Periodic()
        {
            ViewBag.Title = "Periodic Dashboard - Synthesis";
            _CommonRepository.LogEntries();     //Harsh's code
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;

            //Db class for get user type Id.
            var UserTypeId = _CommonRepository.getUserTypeId(UserName);
            DashboardDaily obj = new DashboardDaily();
            try
            {
                if (Session["storeid"] == "0" || Session["storeid"] == null)
                {
                    int StoreID = 0;
                    ViewBag.Storeidvalue = 0;
                    if (UserTypeId != 1)
                    {
                        if (StoreID == 0)
                        {
                            //Db class is Check Error by usertype id and StoreID.
                            var Roles = _DashboardRepository.CheckError(UserTypeId, StoreID);
                            if (!Roles.Contains("ViewDashboardPeriodicDashboard"))
                            {
                                return View("~/Views/Shared/Error.cshtml");
                                //goto Exit;
                            }
                        }
                    }
                    //Get End Date Weekly Period.
                    var enddate = _DashboardRepository.GetEndDateWeeklyPeriod();
                    ViewBag.Startdate = enddate.AddDays(-28).ToString("MM/dd/yyyy");
                    ViewBag.Enddate = enddate.ToString("MM/dd/yyyy");
                }
                else
                {
                    int StoreID = 0;
                    if (Session["storeid"] != null)
                    {
                        StoreID = Convert.ToInt32(Convert.ToString(Session["storeid"]));
                    }
                    ViewBag.Storeidvalue = StoreID;

                    if (UserTypeId != 1)
                    {
                        //Db class is Check Error by usertype id and StoreID.
                        var Roles = _DashboardRepository.CheckError(UserTypeId, StoreID);
                        if (!Roles.Contains("ViewPeriodicDashboard"))
                        {
                            return View("~/Views/Shared/Error.cshtml");
                            //goto Exit;
                        }
                    }
                    //Using this db class get startdate.
                    var Startdate = _DashboardRepository.Startdate(StoreID);
                    //Get Enddate.
                    var enddate = _DashboardRepository.EndDate();
                    ViewBag.Startdate = enddate.AddDays(-28).ToString("MM/dd/yyyy");
                    ViewBag.Enddate = enddate.ToString("MM/dd/yyyy");
                    //Using this class get periodic data with startdate and storeid.
                    obj = _DashboardRepository.Periodic(Startdate, StoreID);
                }
                DashboardDaily ddllist = new DashboardDaily();
                //Get list of Periodic.
                ddllist = _DashboardRepository.ddlPeriodic();
                obj.StoreList = ddllist.StoreList;
                obj.DepartmentList = ddllist.DepartmentList;
                obj.PayrollDepartmentList = ddllist.PayrollDepartmentList;
                //get All Weekly periodicId.
                ViewBag.WeeklyPeriodId = new SelectList(_DashboardRepository.ViewBagWeeklyPeriodId(), "Id", "Text");
                //Get all StateId.
                ViewBag.StateId = new SelectList(_DashboardRepository.ViewBagStateId(), "StateId", "StateCode");
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - Periodic - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View(obj);
        }
        /// <summary>
        /// This method is Get dashborad periodic data.
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="StateId"></param>
        /// <returns></returns>
        public ActionResult getDashboardPeriodicData(string StartDate, string EndDate, int? StateId)
        {
            DashboardDaily obj = new DashboardDaily();
            try
            {
                int StoreID = 0;
                if (Session["storeid"] != null)
                {
                    StoreID = Convert.ToInt32(Convert.ToString(Session["storeid"]));
                }

                DateTime Start = AdminSiteConfiguration.stringToDate(StartDate, DateFormats.MMddyyyy, DateFormats.MMddyyyy);
                DateTime End = AdminSiteConfiguration.stringToDate(EndDate, DateFormats.MMddyyyy, DateFormats.MMddyyyy);
                //Using this class Get dashboard Periodic data.
                obj = _DashboardRepository.getDashboardPeriodicData(StartDate, EndDate, StateId, Start, End, StoreID);
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - getDashboardPeriodicData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Get Sales One Hour Worked Data Periodic
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="department"></param>
        /// <param name="StateId"></param>
        /// <returns></returns>
        public ActionResult GetSalesOneHourWorkedDataPeriodic(string StartDate, string EndDate, string department, int? StateId)
        {
            List<SalesOneHourWorkedDataList> StoreHourlyWorkData = new List<SalesOneHourWorkedDataList>();
            try
            {
                int StoreID = 0;
                if (Session["storeid"] != null)
                {
                    StoreID = Convert.ToInt32(Convert.ToString(Session["storeid"]));
                }
                DateTime Start = StartDate == "" ? DateTime.Now : AdminSiteConfiguration.stringToDate(StartDate, DateFormats.MMddyyyy, DateFormats.MMddyyyy);
                DateTime End = EndDate == "" ? DateTime.Now : AdminSiteConfiguration.stringToDate(EndDate, DateFormats.MMddyyyy, DateFormats.MMddyyyy);
                //Get list of Periodic Sales One hour worked data.
                StoreHourlyWorkData = _DashboardRepository.GetSalesOneHourWorkedDataPeriodic(StartDate, EndDate, department, StateId, Start, End, StoreID);
                return Json(StoreHourlyWorkData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - GetSalesOneHourWorkedDataPeriodic - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(StoreHourlyWorkData, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// This method is Get Periodic Sales Total Details data.
        /// </summary>
        /// <param name="StoreId"></param>
        /// <param name="Department"></param>
        /// <param name="Type"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public JsonResult GetPeriodicSalesTotalDetailsdata(string StoreId, string Department, int Type, string StartDate, string EndDate)
        {
            //Get list of Periodic Sales Total details data.
            List<DashboardDailyTotalData> Dashboard = new List<DashboardDailyTotalData>();
            try
            {
                DateTime Start = StartDate == "" ? DateTime.Now : AdminSiteConfiguration.stringToDate(StartDate, DateFormats.MMddyyyy, DateFormats.MMddyyyy);
                DateTime End = EndDate == "" ? DateTime.Now : AdminSiteConfiguration.stringToDate(EndDate, DateFormats.MMddyyyy, DateFormats.MMddyyyy);
                Dashboard = _DashboardRepository.GetPeriodicSalesTotalDetailsdata(StoreId, Department, Type, Start, End);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - GetPeriodicSalesTotalDetailsdata - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(Dashboard, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// This method is Get Periodic sales daily total details data.
        /// </summary>
        /// <param name="StoreId"></param>
        /// <param name="Department"></param>
        /// <param name="Type"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public JsonResult GetPeriodicSalesDailyTotalDetailsdata(string StoreId, string Department, int Type, string StartDate, string EndDate)
        {
            if (Session["storeid"] != null)
            {
                StoreId = Convert.ToString(Session["storeid"]);
            }
            DateTime Start = StartDate == "" ? DateTime.Now : AdminSiteConfiguration.stringToDate(StartDate, DateFormats.MMddyyyy, DateFormats.MMddyyyy);
            DateTime End = EndDate == "" ? DateTime.Now : AdminSiteConfiguration.stringToDate(EndDate, DateFormats.MMddyyyy, DateFormats.MMddyyyy);
            //Get list of Periodic Sales Total details data.
            List<DashboardDailyTotalData> Dashboard = _DashboardRepository.GetPeriodicSalesTotalDetailsdata(StoreId, Department, Type, Start, End);
            return Json(Dashboard, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// This method is Get Periodic Chart Data.
        /// </summary>
        /// <param name="StoreId"></param>
        /// <param name="Department"></param>
        /// <param name="Type"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="StateId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetPeriodicChartData(string StoreId, string Department, int Type, string StartDate, string EndDate, int? StateId)
        {
            var result = new DashboardChartViewModel();
            try
            {
                DateTime Start = StartDate == "" ? DateTime.Now : AdminSiteConfiguration.stringToDate(StartDate, DateFormats.MMddyyyy, DateFormats.MMddyyyy);
                DateTime End = EndDate == "" ? DateTime.Now : AdminSiteConfiguration.stringToDate(EndDate, DateFormats.MMddyyyy, DateFormats.MMddyyyy);
                //Get list of Periodic chart data.
                result = _DashboardRepository.GetPeriodicChartData(StoreId, Department, Type, StateId, Start, End);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - GetPeriodicChartData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// This method is Get Periodic Chart data Doughnul
        /// </summary>
        /// <param name="StoreId"></param>
        /// <param name="Department"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="Type"></param>
        /// <param name="StateId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetChartDataDoughnutPeriodic(string StoreId, string Department, string StartDate, string EndDate, int Type, int? StateId)
        {
            var result = new DashboardChartViewModel();
            try
            {
                StoreId = "0";
                if (Session["storeid"] != null)
                {
                    StoreId = Convert.ToString(Session["storeid"]);
                }
                DateTime Start = StartDate == "" ? DateTime.Now : AdminSiteConfiguration.stringToDate(StartDate, DateFormats.MMddyyyy, DateFormats.MMddyyyy);
                DateTime End = EndDate == "" ? DateTime.Now : AdminSiteConfiguration.stringToDate(EndDate, DateFormats.MMddyyyy, DateFormats.MMddyyyy);
                //Get Periodic chart doughnut data.
                result = _DashboardRepository.GetChartDataDoughnutPeriodic(StoreId, Department, Type, StateId, Start, End);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - GetChartDataDoughnutPeriodic - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///This method is Get Periodic Bar Chart data.
        /// </summary>
        /// <param name="StoreId"></param>
        /// <param name="Department"></param>
        /// <param name="Type"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="StateId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetPeriodicBarChartData(string StoreId, string Department, int Type, string StartDate, string EndDate, int StateId)
        {
            var result = new DashboardChartViewModel();
            try
            {
                if (Session["storeid"] != null)
                {
                    StoreId = Convert.ToString(Session["storeid"]);
                }
                DateTime Start = StartDate == "" ? DateTime.Now : AdminSiteConfiguration.stringToDate(StartDate, DateFormats.MMddyyyy, DateFormats.MMddyyyy);
                DateTime End = EndDate == "" ? DateTime.Now : AdminSiteConfiguration.stringToDate(EndDate, DateFormats.MMddyyyy, DateFormats.MMddyyyy);
                //Get Periodic bar chart data.
                result = _DashboardRepository.GetPeriodicBarChartData(StoreId, Department, Type, StateId, Start, End);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - GetPeriodicBarChartData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Get periodic Comparision data.
        /// </summary>
        /// <param name="StoreId"></param>
        /// <param name="Department"></param>
        /// <param name="Type"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="StateId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetPeriodicComparisionData(string StoreId, string Department, int Type, string StartDate, string EndDate, int? StateId)
        {
            WeeklyComparisionData objWeeklyComparisionData = new WeeklyComparisionData();
            try
            {
                if (Session["storeid"] != null)
                {
                    StoreId = Convert.ToString(Session["storeid"]);
                }
                DateTime Start = StartDate == "" ? DateTime.Now : AdminSiteConfiguration.stringToDate(StartDate, DateFormats.MMddyyyy, DateFormats.MMddyyyy);
                DateTime End = EndDate == "" ? DateTime.Now : AdminSiteConfiguration.stringToDate(EndDate, DateFormats.MMddyyyy, DateFormats.MMddyyyy);
                //Get Periodic Comparision data.
                objWeeklyComparisionData = _DashboardRepository.GetPeriodicComparisionData(StoreId, Department, Type, StateId, Start, End);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - GetPeriodicComparisionData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(objWeeklyComparisionData, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// This method is Get periodic hourly Chart data.
        /// </summary>
        /// <param name="StoreId"></param>
        /// <param name="Department"></param>
        /// <param name="Type"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="StateId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetHourlyChartDataPeriodic(string StoreId, string Department, int Type, string StartDate, string EndDate, int? StateId)
        {
            var result = new HourlyChartViewModel();
            try
            {
                DateTime Start = StartDate == "" ? DateTime.Now : AdminSiteConfiguration.stringToDate(StartDate, DateFormats.MMddyyyy, DateFormats.MMddyyyy);
                DateTime End = EndDate == "" ? DateTime.Now : AdminSiteConfiguration.stringToDate(EndDate, DateFormats.MMddyyyy, DateFormats.MMddyyyy);
                //Get Periodic Hourly chart data.
                result = _DashboardRepository.GetHourlyChartDataPeriodic(StoreId, Department, Type, StateId, Start, End);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - GetHourlyChartDataPeriodic - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// This method is Get periodic hourly Chart tool tip data.
        /// </summary>
        /// <param name="StoreId"></param>
        /// <param name="Department"></param>
        /// <param name="Type"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public JsonResult getPeriodicHourlyChartToolTipData(string StoreId, string Department, int Type, string StartDate, string EndDate)
        {
            List<DashboardChartToolTipData> ToolTipData = new List<DashboardChartToolTipData>();
            try
            {
                DateTime Start = StartDate == "" ? DateTime.Now : AdminSiteConfiguration.stringToDate(StartDate, DateFormats.MMddyyyy, DateFormats.MMddyyyy);
                DateTime End = EndDate == "" ? DateTime.Now : AdminSiteConfiguration.stringToDate(EndDate, DateFormats.MMddyyyy, DateFormats.MMddyyyy);
                //get Periodic Hourly Chart tool tip data.
                ToolTipData = _DashboardRepository.getPeriodicHourlyChartToolTipData(StoreId, Department, Type, Start, End);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - getPeriodicHourlyChartToolTipData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(ToolTipData, JsonRequestBehavior.AllowGet); ;
        }
        /// <summary>
        /// This method is Get periodic Payroll sales Boxes data.
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="Department"></param>
        /// <param name="StateId"></param>
        /// <returns></returns>
        public ActionResult GetPayrollSalesBoxesDataPeriodic(string StartDate, string EndDate, string Department, int? StateId)
        {
            DashboardDaily obj = new DashboardDaily();
            try
            {
                int StoreID = 0;
                if (Session["storeid"] != null)
                {
                    StoreID = Convert.ToInt32(Convert.ToString(Session["storeid"]));
                }
                DateTime Start = StartDate == "" ? DateTime.Now : AdminSiteConfiguration.stringToDate(StartDate, DateFormats.MMddyyyy, DateFormats.MMddyyyy);
                DateTime End = EndDate == "" ? DateTime.Now : AdminSiteConfiguration.stringToDate(EndDate, DateFormats.MMddyyyy, DateFormats.MMddyyyy);
                //get Periodic payroll sales boxes data
                obj = _DashboardRepository.GetPayrollSalesBoxesDataPeriodic(Department, StateId, StoreID, Start, End);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - GetPayrollSalesBoxesDataPeriodic - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Get Periodic Expenses boxes data.
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="StoreID"></param>
        /// <param name="StateId"></param>
        /// <returns></returns>
        public ActionResult GetExpensesBoxesDataPeriodic(string StartDate, string EndDate, string StoreID, int? StateId)
        {
            if (StoreID == "")
                StoreID = "0";
            DashboardWeeklyExpenses obj = new DashboardWeeklyExpenses();
            DateTime Start = StartDate == "" ? DateTime.Now : AdminSiteConfiguration.stringToDate(StartDate, DateFormats.MMddyyyy, DateFormats.MMddyyyy);
            DateTime End = EndDate == "" ? DateTime.Now : AdminSiteConfiguration.stringToDate(EndDate, DateFormats.MMddyyyy, DateFormats.MMddyyyy);
            //Get Expenses Boxes data.
            obj = _DashboardRepository.GetExpensesBoxesDataPeriodic(StoreID, StateId, Start, End);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Get Week Date. 
        /// </summary>
        /// <param name="WeeklyPeriodId"></param>
        /// <returns></returns>
        public JsonResult GetWeekDate(string WeeklyPeriodId)
        {
            try
            {
                var StartDate = "";
                var EndDate = "";
                if (Convert.ToInt32(WeeklyPeriodId) != 0)
                {
                    //get Week date using Weekly periodic ID.
                    var WeeklyRange = _DashboardRepository.GetWeekDate(WeeklyPeriodId);
                    StartDate = WeeklyRange.StartDate.ToString("MM/dd/yyyy");
                    EndDate = WeeklyRange.EndDate.ToString("MM/dd/yyyy");
                }
                else
                {
                    StartDate = DateTime.Now.ToString("MM/dd/yyyy");
                    EndDate = DateTime.Now.ToString("MM/dd/yyyy");
                }
                return Json(new { StartDate = StartDate, EndDate = EndDate }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - GetWeekDate - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// This method is Get quarter Date.
        /// </summary>
        /// <param name="Qatar"></param>
        /// <returns></returns>
        public JsonResult GetQatarDate(string Qatar)
        {
            try
            {
                data = new DashboardViewModel();
                //Using this class Get Quarter date.
                data = _DashboardRepository.GetQatarDate(Qatar);
                return Json(new { StartDate = data.StartDate, EndDate = data.EndDate }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - GetQatarDate - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }
        #endregion


        #region Current Date Dashboard
        /// <summary>
        /// This method is return view of Yearly dashboard.
        /// </summary>
        /// <returns></returns>
        public ActionResult Yearly()
        {
            ViewBag.Title = "Yearly Dashboard - Synthesis";
            _CommonRepository.LogEntries();     //Harsh's code
            YearlySales sales = new YearlySales();
            if (Session["storeid"] == null)
            {
                int StoreID = 0;
                ViewBag.Storeidvalue = 0;
            }
            else
            {
                int StoreID = 0;
                if (Session["storeid"] != null)
                {
                    StoreID = Convert.ToInt32(Convert.ToString(Session["storeid"]));
                }
                ViewBag.Storeidvalue = StoreID;
            }
            //get StateID.
            ViewBag.StateId = new SelectList(_DashboardRepository.ViewBagStateId(), "StateId", "StateCode");

            sales = _DashboardRepository.DepartmentList();
            //Get All Period.
            ViewBag.period = _DashboardRepository.ViewBagPeriod();
            //using this class Get all Year.
            var YearList = _DashboardRepository.ViewbagYear();
            ViewBag.Year = new SelectList(YearList.Select(s => new { Year = s }), "Year", "Year");
            return View(sales);
        }

        /// <summary>
        /// This method is Get Yearly sales data.
        /// </summary>
        /// <param name="Departments"></param>
        /// <param name="StateId"></param>
        /// <param name="Year"></param>
        /// <returns></returns>
        public ActionResult GetYearlySalesData(string Departments, int? StateId, int Year)
        {
            data = new DashboardViewModel();
            List<ChartModel> charts = new List<ChartModel>();
            var YearList = _DashboardRepository.ViewbagYear();
            ViewBag.Year = new SelectList(YearList.Select(s => new { Year = s }), "Year", "Year");
            ViewBag.StateId = new SelectList(_DashboardRepository.ViewBagStateId(), "StateId", "StateCode");
            int StoreID = 0;
            if (Session["storeid"] == null)
            {
                ViewBag.Storeidvalue = 0;
            }
            else
            {
                StoreID = Convert.ToInt32(Convert.ToString(Session["storeid"]));
                ViewBag.Storeidvalue = StoreID;
            }
            //Get all Yearly sales data.
            data = _DashboardRepository.GetYearlySalesData(Departments, StateId, Year, StoreID);
            return Json(new { sales = data.sales, growt = data.growt, result = data, salesL = data.salesL }, JsonRequestBehavior.AllowGet);
        }
        #endregion


        /// <summary>
        /// This method is Get YTD Date.
        /// </summary>
        /// <param name="Qatar"></param>
        /// <returns></returns>
        public JsonResult GetYTDDate(string Qatar)
        {
            try
            {
                data = new DashboardViewModel();
                //Get all YTD date.
                data = _DashboardRepository.GetYTDDate(Qatar);
                return Json(new { StartDate = data.StartDate, EndDate = data.EndDate }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        //pdf export Himanshu
        public ActionResult DownloadDashboardDailyPDF(string date, int? StateId, int? GroupID, string StoreId, string SelectedStoreId, string Departments)
        {
            var obj = getDashboardDailyDataPDF(date, StateId, GroupID, StoreId, SelectedStoreId, Departments);

            string htmlContent = RenderViewToString("getDashboardDailyDataPDF", obj);

            byte[] pdfBytes = GeneratePdf(htmlContent.ToString());

            return File(pdfBytes, "application/pdf", "DailyDashboard.pdf");
        }

        //Himanshu
        private DashboardDaily getDashboardDailyDataPDF(string date, int? StateId, int? GroupID, string StoreId, string SelectedStoreId, string Departments)
        {
            DashboardDaily obj = new DashboardDaily();
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
                string StoreID = "0";
                if (StoreId != null && StoreId != "")
                {
                    StoreID = StoreId;
                    StoreID = StoreId.TrimEnd(',');

                }
                ViewBag.Startdate = date;
                string StoreName = "";
                string[] storeIdsArray = StoreID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string Id in storeIdsArray)
                {
                    if (int.TryParse(Id, out int storeIdInt))
                    {
                        StoreName += _CommonRepository.GetStoreNikeName(storeIdInt) + ",";
                    }
                }
                StoreName = StoreName.TrimEnd(',');
                ViewBag.StoreName = StoreName;
                ViewBag.StateName = _DashboardRepository.GetStateName(StateId);
                //Get Dashboard Daily data.
                obj = _DashboardRepository.getDashboardDailyData(date, StoreID, StateId, GroupID);

                SelectedStoreId = string.Join(",", SelectedStoreId.Split(',').Where(x => x != "0"));
                obj.dailychartlisttype1 = _DashboardRepository.GetChartDataForPDF(SelectedStoreId, Departments, 1, date, StateId, GroupID);
                ViewBag.AmountList = obj.dailychartlisttype1.Select(s => s.Amount).Distinct();
                ViewBag.HoursList = obj.dailychartlisttype1.Select(s => s.Hours).Distinct();
                ViewBag.StoreList = obj.dailychartlisttype1.Select(s => s.StoreId).Distinct();

                obj.dailychartlisttype2 = _DashboardRepository.GetChartDataForPDF(SelectedStoreId, Departments, 2, date, StateId, GroupID);
                obj.dailychartlisttype3 = _DashboardRepository.GetChartDataForPDF(SelectedStoreId, Departments, 3, date, StateId, GroupID);
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - getDashboardDailyDataPDF - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        //Himanshu
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

        //Himanshu
        protected byte[] GeneratePdf(string htmlContent)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                iTextSharp.text.Document document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                //PdfWriter.GetInstance(document, new FileStream(strFileName, FileMode.Create));
                document.Open();

                //string Content = System.IO.File.ReadAllText(Server.MapPath(".")+"//Dailypdf.html");


                iTextSharp.text.html.simpleparser.StyleSheet styles = new iTextSharp.text.html.simpleparser.StyleSheet();
                iTextSharp.text.html.simpleparser.HTMLWorker hw = new iTextSharp.text.html.simpleparser.HTMLWorker(document);
                hw.SetStyleSheet(styles);
                hw.Parse(new StringReader(htmlContent));

                //using (TextReader reader = new StringReader(htmlContent))
                //{
                //    XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, reader);
                //}

                document.Close();

                return ms.ToArray();
            }
        }


        //excel export Himanshu
        public ActionResult DownloadDashboardDailyExcel(string date, int? StateId, int? GroupID, string StoreId, string SelectedStoreId, string Departments)
        {
            var obj = getDashboardDailyDataPDF(date, StateId, GroupID, StoreId, SelectedStoreId, Departments);
            string htmlContent = RenderViewToString("getDashboardDailyDataExcel", obj);

            byte[] excelBytes = GenerateExcel(htmlContent.ToString());

            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DailyDashboard.xlsx");
        }

        //Himanshu
        protected byte[] GenerateExcel(string htmlContent)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (ExcelPackage package = new ExcelPackage(ms))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Dashboard Data");

                    LoadHtmlContentToWorksheet(htmlContent, worksheet);

                    worksheet.Cells.AutoFitColumns();

                    package.Save();
                }

                return ms.ToArray();
            }
        }

        //Himanshu
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

        //Himanshu 31-12-2024
        public ActionResult GetStoreBySelectedStateId(int? stateid,int? GroupID)
        {
            List<int> storeid = new List<int>();
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            var UserId = _CommonRepository.getUserId(UserName);
            try
            {
                if(stateid != null && GroupID == null)
                {
                    storeid = _DashboardRepository.GetStoreBySelectedStateId(stateid);
                }
                else
                {
                    storeid = _DashboardRepository.GetStoreBySelectedGroupId(GroupID, UserId);
                }
            }
            catch (Exception ex)
            {
                logger.Error("CustomerInformationController - UpdateCompanyName - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return Json(storeid, JsonRequestBehavior.AllowGet);
        }

        //Himanshu 03-02-2025
        public ActionResult ExportSalesExcelData(DateTime startdate, DateTime enddate, string storeids)
        {
            try
            {
                int StoreId = 0;
                StoreId = Convert.ToInt32(Session["storeid"]);
                if (StoreId != 0 && storeids == "")
                {
                    storeids = StoreId.ToString();
                }

                var salesData = _DashboardRepository.GetNetSalesData(startdate, enddate, storeids);

                if (salesData != null && salesData.Any())
                {
                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Net Sales Report");

                        // Set Headers
                        worksheet.Cells[1, 1].Value = "Date";
                        worksheet.Cells[1, 2].Value = "Store";

                        int columnIndex = 3;
                        var uniqueCategories = salesData.SelectMany(d => d.CategorySales.Keys).Distinct().ToList();

                        foreach (var category in uniqueCategories)
                        {
                            worksheet.Cells[1, columnIndex].Value = category;
                            columnIndex++;
                        }

                        worksheet.Cells[1, columnIndex].Value = "Total";
                        worksheet.Cells[1, columnIndex + 1].Value = "Customer Count";
                        worksheet.Cells[1, columnIndex + 2].Value = "Average Sale";

                        // Apply Header Styling
                        using (var headerRange = worksheet.Cells[1, 1, 1, columnIndex + 2])
                        {
                            headerRange.Style.Font.Bold = true;
                            headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                            headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            headerRange.Style.Font.Size = 12;
                        }

                        // Add Data
                        int rowIndex = 2;
                        foreach (var data in salesData)
                        {
                            worksheet.Cells[rowIndex, 1].Value = data.StartDate.ToString("MM/dd/yyyy");
                            worksheet.Cells[rowIndex, 2].Value = data.StoreName;

                            columnIndex = 3;
                            decimal total = 0;

                            foreach (var category in uniqueCategories)
                            {
                                decimal value = data.CategorySales.ContainsKey(category) ? data.CategorySales[category] : 0;
                                worksheet.Cells[rowIndex, columnIndex].Value = value;
                                total += value;
                                columnIndex++;
                            }

                            worksheet.Cells[rowIndex, columnIndex].Value = total; // Insert Total
                            worksheet.Cells[rowIndex, columnIndex + 1].Value = data.CustomerCount;
                            worksheet.Cells[rowIndex, columnIndex + 2].Value = data.AverageSale;

                            rowIndex++;
                        }

                        // AutoFit Columns
                        worksheet.Cells.AutoFitColumns();

                        // Return Excel file
                        var stream = new MemoryStream();
                        package.SaveAs(stream);
                        stream.Position = 0;
                        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "NetSalesReport.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("DashboardController - ExportSalesExcelData - " + DateTime.Now + " - " + ex.Message);
            }

            return RedirectToAction("Daily");
        }


    }
}