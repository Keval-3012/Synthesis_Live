using EntityModels.Models;
using NLog;
using Repository;
using Repository.IRepository;
using Syncfusion.EJ2.Base;
using Syncfusion.EJ2.Popups;
using SynthesisQBOnline;
using SynthesisViewModal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using Utility;
using static Utility.AdminSiteConfiguration;

namespace SynthesisRepo.Controllers
{
    public class ManageIPAddressController : Controller
    {

        private readonly IManageIPAddressRepository _manageIPAddresseRepository;
        private readonly ISynthesisApiRepository _synthesisApiRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly IUserActivityLogRepository _activityLogRepository;
        Logger logger = LogManager.GetCurrentClassLogger();

        ManageIPAddressViewModal MainManageIPAddressView = new ManageIPAddressViewModal();
        public ManageIPAddressController()
        {
            this._manageIPAddresseRepository = new ManageIPAddressRepository(new DBContext());
            this._synthesisApiRepository = new SynthesisApiRepository();
            this._commonRepository = new CommonRepository(new DBContext());
            this._activityLogRepository = new UserActivityLogRepository(new DBContext());
        }
        /// <summary>
        /// This method is return view of User Time Hourse
        /// </summary>
        /// <returns></returns>
        public ActionResult UserTimeHourse()
        {
            ViewBag.Title = "Employee Timecards - Synthesis";
            _commonRepository.LogEntries();     //Harsh's code
            ViewBag.EmailSend = _commonRepository.GetMessageValue("ETESS", "Email Send Successfully!");
            ViewBag.OutGreatIn = _commonRepository.GetMessageValue("ETOGI", "Out Time must be greater than to In Time");
            ViewBag.BelongstoOther = _commonRepository.GetMessageValue("ETEBOR", "You have entered time is already belongs to other record.");
            try
            {
                //Get Pay period Setting list
                ViewBag.Day = (int)_manageIPAddresseRepository.PayPeriodSettingsList().Select(m => m.Day).FirstOrDefault();
                DateTime dt = Common.StartOfWeek(DateTime.Now, (DayOfWeek)ViewBag.Day);
                ViewBag.Date = dt;
                //Get Drp Week List
                var WeekData = _manageIPAddresseRepository.DrpWeekList();
                int i = 0;
                foreach (var item in WeekData)
                {
                    if (i == 0)
                    {
                        item.Id = item.Id + " (Current Period)";
                        item.Text = item.Text + " (Current Period)";
                        break;
                    }
                }

                ViewBag.DrpWeekRange = new SelectList(WeekData, "Id", "Text");
                Session["weekrange"] = dt.ToString("MM/dd/yyyy").Replace("-", "/") + " - " + dt.AddDays(6).ToString("MM/dd/yyyy").Replace("-", "/") + " (Current Period)";
                Session["arrow"] = "1";

                List<DialogDialogButton> buttons = new List<DialogDialogButton>() { };
                buttons.Add(new DialogDialogButton() { Click = "dlgemailButtonClick", ButtonModel = new DefaultButtonModels() { content = "Send Mail", isPrimary = true } });
                buttons.Add(new DialogDialogButton() { Click = "CancelBouttonClick", ButtonModel = new DefaultButtonModels() { content = "Cancel" } });
                ViewBag.DefaultButtons = buttons;

            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressController - UserTimeHourse - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           
            return View();
        }
        /// <summary>
        /// This method is return partial view of User grid value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public ActionResult UserGridValue(string[] value, int method)
        {
            string date = value[0].ToString().Trim();
            try
            {
                MainManageIPAddressView.dt1 = AdminSiteConfiguration.stringToDate(date, DateFormats.MMddyyyy, DateFormats.MMddyyyy);
                MainManageIPAddressView.dt2 = MainManageIPAddressView.dt1.AddDays(6);
                //Get Pay period Setting list
                ViewBag.Day = (int)_manageIPAddresseRepository.PayPeriodSettingsList().Select(m => m.Day).FirstOrDefault();
                DateTime dt = Common.StartOfWeek(DateTime.Now, (DayOfWeek)ViewBag.Day);

                if (method == 1)
                {
                    MainManageIPAddressView.dt1 = MainManageIPAddressView.dt1.AddDays(-7);
                    MainManageIPAddressView.dt2 = MainManageIPAddressView.dt1.AddDays(6);
                    Session["weekrange"] = MainManageIPAddressView.dt1.ToString("MM/dd/yyyy").Replace("-", "/") + " - " + MainManageIPAddressView.dt2.ToString("MM/dd/yyyy").Replace("-", "/");

                }
                else if (method == 2)
                {
                    DateTime dts = MainManageIPAddressView.dt1;
                    DateTime dte = MainManageIPAddressView.dt1.AddDays(6);

                    MainManageIPAddressView.dt1 = MainManageIPAddressView.dt1.AddDays(7);
                    MainManageIPAddressView.dt2 = MainManageIPAddressView.dt1.AddDays(6);
                    Session["weekrange"] = MainManageIPAddressView.dt1.ToString("MM/dd/yyyy").Replace("-", "/") + " - " + MainManageIPAddressView.dt2.ToString("MM/dd/yyyy").Replace("-", "/");

                    if (MainManageIPAddressView.dt1 > dt)
                    {
                        dte = dts.AddDays(6);
                        Session["weekrange"] = dts.ToString("MM/dd/yyyy").Replace("-", "/") + " - " + dte.ToString("MM/dd/yyyy").Replace("-", "/") + " (Current Period)";
                        Session["arrow"] = "1";
                        MainManageIPAddressView.dt1 = dts;
                        MainManageIPAddressView.dt2 = dte;
                    }
                }
                else if (method == 4)
                {
                    MainManageIPAddressView.dt2 = MainManageIPAddressView.dt1.AddDays(6);
                    Session["weekrange"] = value[0].Trim().Replace("-", "/") + " - " + value[1].Trim().Replace("-", "/");
                }
                if (dt == MainManageIPAddressView.dt1)
                {
                    MainManageIPAddressView.dt2 = MainManageIPAddressView.dt1.AddDays(6);
                    Session["weekrange"] = MainManageIPAddressView.dt1.ToString("MM/dd/yyyy").Replace("-", "/") + " - " + MainManageIPAddressView.dt2.ToString("MM/dd/yyyy").Replace("-", "/") + " (Current Period)";
                    Session["arrow"] = "1";
                }
                else
                {
                    Session["arrow"] = "0";
                }
                Session["Date1"] = MainManageIPAddressView.dt1;
                Session["Date2"] = MainManageIPAddressView.dt2;

            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressController - UserGridValue - " + DateTime.Now + " - " + ex.Message.ToString());
            }
          
            return PartialView("UserActivityList");
        }

        /// <summary>
        /// This method is get url Data Source Value
        /// </summary>
        /// <param name="dm"></param>
        /// <returns></returns>
        public ActionResult UrlDatasourceValue(DataManagerRequest dm)
        {
            MainManageIPAddressView.dt1 = Convert.ToDateTime(Session["Date1"]);
            MainManageIPAddressView.dt2 = Convert.ToDateTime(Session["Date2"]);
            List<UserActivityList> list = new List<UserActivityList>();
            //this class is User Activity List
            list = _manageIPAddresseRepository.UserActivityList(0, MainManageIPAddressView.dt1, MainManageIPAddressView.dt2);
            IEnumerable DataSource = list;
            DataOperations operation = new DataOperations();
            MainManageIPAddressView.count = DataSource.Cast<UserActivityList>().Count();
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
                logger.Error("ManageIPAddressController - UrlDatasourceValue - " + DateTime.Now + " - " + ex.Message.ToString());
            }
          
            return dm.RequiresCounts ? Json(new { result = DataSource, count = MainManageIPAddressView.count }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet) : Json(DataSource, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Get user time track
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ActionResult GetUserTimeTrack(UserActivityList value)
        {
            MainManageIPAddressView.dt1 = Convert.ToDateTime(Session["Date1"]);
            MainManageIPAddressView.dt2 = Convert.ToDateTime(Session["Date2"]);
            ViewBag.UserName = value.FirstName;
            ViewBag.UserId = value.UserId;
            //Get Ip And Location by location and IpAdressInfoID
            ViewBag.Loction = _manageIPAddresseRepository.GetIpAndLocation().Select(s => new { s.Location, s.IpAdressInfoID }).ToList();
            ViewBag.StartDate = MainManageIPAddressView.dt1.ToString("MM/dd/yyyy hh:mm:ss");
            ViewBag.EndDate = MainManageIPAddressView.dt2.ToString("MM/dd/yyyy hh:mm:ss");

            ViewBag.Showdate = "(" + MainManageIPAddressView.dt1.ToString("MM/dd/yyyy").Replace("-", "/") + " - " + MainManageIPAddressView.dt2.ToString("MM/dd/yyyy").Replace("-", "/") + ")";
            return PartialView("UserTimeInAndOut");
        }
        /// <summary>
        /// This method is Url data Source for User time
        /// </summary>
        /// <param name="dm"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public ActionResult UrlDatasourceUserTime(DataManagerRequest dm, int UserId)
        {
            MainManageIPAddressView.dt1 = Convert.ToDateTime(Session["Date1"]);
            MainManageIPAddressView.dt2 = Convert.ToDateTime(Session["Date2"]);
            List<GetUserTrackTime> list = new List<GetUserTrackTime>();
            //Get User Wise Attendance
            list = _manageIPAddresseRepository.GetUserwiseAttendance(UserId, MainManageIPAddressView.dt1, MainManageIPAddressView.dt2);
            IEnumerable DataSource = list;
            DataOperations operation = new DataOperations();
            MainManageIPAddressView.count = DataSource.Cast<GetUserTrackTime>().Count();
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
                logger.Error("ManageIPAddressController - UrlDatasourceUserTime - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            
          
           
            return dm.RequiresCounts ? Json(new { result = DataSource, count = MainManageIPAddressView.count }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet) : Json(DataSource, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// This method is Insert User Time
        /// </summary>
        /// <param name="GetUserTrackTime"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public ActionResult InsertUserTime(CRUDModel<GetUserTrackTime> GetUserTrackTime, int UserId)
        {
            try
            {
                var date = Convert.ToDateTime(GetUserTrackTime.Value.InTime);
                System.DateTime dateTime = Convert.ToDateTime(Convert.ToDateTime(GetUserTrackTime.Value.dt).Date.ToString("yyyy-MM-dd") + ' ' + GetUserTrackTime.Value.InTime);

                clsCommon.WriteErrorLogs("InTime :  " + dateTime);

                System.DateTime dateTime1 = new System.DateTime();
                if (GetUserTrackTime.Value.OutTime != null)
                {
                    dateTime1 = Convert.ToDateTime(Convert.ToDateTime(GetUserTrackTime.Value.dt).Date.ToString("yyyy-MM-dd") + ' ' + GetUserTrackTime.Value.OutTime);
                }
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                //Db class is get user Id by username.
                MainManageIPAddressView.UserIdSt = _commonRepository.getUserId(UserName);
                IpAdressInfo ipAdress = new IpAdressInfo();
                UserTimeTrackInfo obj = new UserTimeTrackInfo();
                UserTimeTrackInfo obj1 = new UserTimeTrackInfo();
                //Db class is Get Inlocation
                ipAdress = _manageIPAddresseRepository.GetInLocation(GetUserTrackTime.Value.InLocation);
                if (GetUserTrackTime.Value.OutTime != null)
                {
                    obj.UserId = UserId;
                    obj.StartTime = dateTime;
                    obj.Location = ipAdress.IpAdressInfoID.ToString();
                    obj.IpAddress = String.IsNullOrEmpty(ipAdress.StaticIp) ? ipAdress.StartIP : ipAdress.StaticIp;
                    obj.CreatedOn = DateTime.Now;
                    obj.CreatedBy = MainManageIPAddressView.UserIdSt;
                    obj.InOutType = CheckInOutType.CheckIn;
                    obj.IsTimeCardEntry = true;
                    //This db class is Save User Time Track
                    _manageIPAddresseRepository.SaveUserTimeTrack(obj);

                    obj1.UserId = UserId;
                    obj1.StartTime = dateTime1;
                    obj1.Location = ipAdress.IpAdressInfoID.ToString();
                    obj1.IpAddress = String.IsNullOrEmpty(ipAdress.StaticIp) ? ipAdress.StartIP : ipAdress.StaticIp;
                    obj1.CreatedOn = DateTime.Now;
                    obj1.CreatedBy = MainManageIPAddressView.UserIdSt;
                    obj1.InOutType = CheckInOutType.CheckOut;
                    obj1.IsTimeCardEntry = true;
                    //This db class is Save User Time Track
                    _manageIPAddresseRepository.SaveUserTimeTrack(obj1);
                    //MainManageIPAddressView.success = "User In/Out time successfully Created.";
                    MainManageIPAddressView.success = _commonRepository.GetMessageValue("EMTC", "User In/Out time successfully Created.");
                }
                else
                {
                    obj.UserId = UserId;
                    obj.StartTime = dateTime;
                    obj.Location = ipAdress.IpAdressInfoID.ToString();
                    obj.IpAddress = String.IsNullOrEmpty(ipAdress.StaticIp) ? ipAdress.StartIP : ipAdress.StaticIp;
                    obj.CreatedOn = DateTime.Now;
                    obj.CreatedBy = MainManageIPAddressView.UserIdSt;
                    obj.InOutType = CheckInOutType.CheckIn;
                    obj.IsTimeCardEntry = true;
                    //This db class is Save User Time Track
                    _manageIPAddresseRepository.SaveUserTimeTrack(obj);
                    //MainManageIPAddressView.success = "User In/Out time successfully Created.";
                    MainManageIPAddressView.success = _commonRepository.GetMessageValue("EMTC", "User In/Out time successfully Created.");
                }
                obj = null;
                obj1 = null;
                GetUserTrackTime.Value.dt = (Convert.ToDateTime(GetUserTrackTime.Value.dt)).ToString("MM/dd/yyyy");
                GetUserTrackTime.Value.InTime = (Convert.ToDateTime(GetUserTrackTime.Value.InTime)).ToString("hh:mmtt");
                if (GetUserTrackTime.Value.OutTime != null)
                {
                    var date2 = Convert.ToDateTime(GetUserTrackTime.Value.OutTime);
                    GetUserTrackTime.Value.OutTime = (Convert.ToDateTime(GetUserTrackTime.Value.OutTime)).ToString("hh:mmtt");
                    //Get Time duration
                    decimal duration = _manageIPAddresseRepository.GetTimeDuration(0, date, date2);
                    GetUserTrackTime.Value.TotalHours = duration;
                }
                else
                {
                    GetUserTrackTime.Value.TotalHours = 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressController - InsertUserTime - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = GetUserTrackTime.Value, success = MainManageIPAddressView.success, Error = MainManageIPAddressView.Error });
        }
        /// <summary>
        /// This method is Update User Time
        /// </summary>
        /// <param name="GetUserTrackTime"></param>
        /// <returns></returns>
        public ActionResult UpdateUserTime(CRUDModel<GetUserTrackTime> GetUserTrackTime)
        {
            try
            {
                System.DateTime dateTime = Convert.ToDateTime(Convert.ToDateTime(GetUserTrackTime.Value.dt).Date.ToString("yyyy-MM-dd") + ' ' + GetUserTrackTime.Value.InTime);
                DateTime dateTime1 = new DateTime();
                if (GetUserTrackTime.Value.OutTime != null)
                {
                    dateTime1 = Convert.ToDateTime(Convert.ToDateTime(GetUserTrackTime.Value.dt).Date.ToString("yyyy-MM-dd") + ' ' + GetUserTrackTime.Value.OutTime);
                }
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                //Db class is get user Id by username.
                MainManageIPAddressView.UserId = _commonRepository.getUserId(UserName);
                IpAdressInfo ipAdress = new IpAdressInfo();
                UserTimeTrackInfo obj = new UserTimeTrackInfo();
                UserTimeTrackInfo obj1 = new UserTimeTrackInfo();
                if (GetUserTrackTime.Value.OutTime != null)
                {
                    if (GetUserTrackTime.Value.InTimeId != null)
                    {
                        if (GetUserTrackTime.Value.OutTimeId != null && GetUserTrackTime.Value.OutTimeId != 0)
                        {
                            //Db class is Get Inlocation
                            ipAdress = _manageIPAddresseRepository.GetInLocation(GetUserTrackTime.Value.InLocation);
                            obj.UserId = GetUserTrackTime.Value.UserId;
                            obj.StartTime = dateTime;
                            obj.Location = ipAdress.IpAdressInfoID.ToString();
                            obj.IpAddress = String.IsNullOrEmpty(ipAdress.StaticIp) ? ipAdress.StartIP : ipAdress.StaticIp;
                            obj.InOutType = CheckInOutType.CheckIn;
                            obj.UpdatedOn = DateTime.Now;
                            obj.UpdatedBy = MainManageIPAddressView.UserId;
                            obj.UserTimeTrackInfoID = (int)GetUserTrackTime.Value.InTimeId;
                            //This db class is Update User Time Track
                            _manageIPAddresseRepository.UpdateUserTrackTime(obj);

                            obj1.UserId = GetUserTrackTime.Value.UserId;
                            obj1.StartTime = dateTime1;
                            obj1.Location = ipAdress.IpAdressInfoID.ToString();
                            obj1.IpAddress = String.IsNullOrEmpty(ipAdress.StaticIp) ? ipAdress.StartIP : ipAdress.StaticIp;
                            obj1.UpdatedOn = DateTime.Now;
                            obj1.UpdatedBy = MainManageIPAddressView.UserId;
                            obj1.InOutType = CheckInOutType.CheckOut;
                            obj1.UserTimeTrackInfoID = (int)GetUserTrackTime.Value.OutTimeId;
                            obj1.IsSystemEntry = false;
                            obj1.IsTimeCardEntry = true;
                            //This db class is Update User Time Track
                            _manageIPAddresseRepository.UpdateUserTrackTime(obj1);
                            //MainManageIPAddressView.success = "User In/Out time successfully Updated.";
                            MainManageIPAddressView.success = _commonRepository.GetMessageValue("EMTE", "User In/Out time successfully Updated.");
                        }
                        else
                        {
                            //Db class is Get Inlocation
                            ipAdress = _manageIPAddresseRepository.GetInLocation(GetUserTrackTime.Value.InLocation);
                            //Db class is Get User Time Track Infos
                            UserTimeTrackInfo obj2 = _manageIPAddresseRepository.GetUserTimeTrackInfos((int)GetUserTrackTime.Value.InTimeId);

                            obj.UserId = GetUserTrackTime.Value.UserId;
                            obj.StartTime = dateTime;
                            obj.Location = ipAdress.IpAdressInfoID.ToString();
                            obj.IpAddress = String.IsNullOrEmpty(ipAdress.StaticIp) ? ipAdress.StartIP : ipAdress.StaticIp;
                            obj.CreatedOn = obj2.CreatedOn;
                            obj.CreatedBy = obj2.CreatedBy;
                            obj.InOutType = CheckInOutType.CheckIn;
                            obj.IsTimeCardEntry = obj2.IsTimeCardEntry;
                            //This db class is Update User Time Track
                            _manageIPAddresseRepository.UpdateUserTrackTime(obj);

                            obj1.UserId = GetUserTrackTime.Value.UserId;
                            obj1.StartTime = dateTime1;
                            obj1.Location = ipAdress.IpAdressInfoID.ToString();
                            obj1.IpAddress = String.IsNullOrEmpty(ipAdress.StaticIp) ? ipAdress.StartIP : ipAdress.StaticIp;
                            obj1.CreatedOn = DateTime.Now;
                            obj1.CreatedBy = MainManageIPAddressView.UserId;
                            obj1.InOutType = CheckInOutType.CheckOut;
                            obj1.IsSystemEntry = false;
                            obj1.IsTimeCardEntry = true;
                            //This db class is Update User Time Track
                            _manageIPAddresseRepository.UpdateUserTrackTime(obj1);
                            //MainManageIPAddressView.success = "User In/Out time successfully Created.";
                            MainManageIPAddressView.success = _commonRepository.GetMessageValue("EMTC", "User In/Out time successfully Created.");
                        }
                    }
                    else
                    {
                        //Db class is Get Inlocation
                        ipAdress = _manageIPAddresseRepository.GetInLocation(GetUserTrackTime.Value.InLocation);
                        obj.UserId = GetUserTrackTime.Value.UserId;
                        obj.StartTime = dateTime;
                        obj.Location = ipAdress.IpAdressInfoID.ToString();
                        obj.IpAddress = String.IsNullOrEmpty(ipAdress.StaticIp) ? ipAdress.StartIP : ipAdress.StaticIp;
                        obj.CreatedOn = DateTime.Now;
                        obj.CreatedBy = MainManageIPAddressView.UserId;
                        obj.InOutType = CheckInOutType.CheckIn;
                        obj.IsTimeCardEntry = true;
                        //This db class is Update User Time Track
                        _manageIPAddresseRepository.UpdateUserTrackTime(obj);

                        obj1.UserId = GetUserTrackTime.Value.UserId;
                        obj1.StartTime = dateTime1;
                        obj1.Location = ipAdress.IpAdressInfoID.ToString();
                        obj1.IpAddress = String.IsNullOrEmpty(ipAdress.StaticIp) ? ipAdress.StartIP : ipAdress.StaticIp;
                        obj1.CreatedOn = DateTime.Now;
                        obj1.CreatedBy = MainManageIPAddressView.UserId;
                        obj1.InOutType = CheckInOutType.CheckOut;
                        obj1.IsTimeCardEntry = true;
                        //This db class is Update User Time Track
                        _manageIPAddresseRepository.UpdateUserTrackTime(obj1);
                        //MainManageIPAddressView.success = "User In/Out time successfully Created.";
                        MainManageIPAddressView.success = _commonRepository.GetMessageValue("EMTC", "User In/Out time successfully Created.");
                    }
                }
                else
                {
                    if (GetUserTrackTime.Value.InTimeId != null)
                    {
                        //Db class is Get Inlocation
                        ipAdress = _manageIPAddresseRepository.GetInLocation(GetUserTrackTime.Value.InLocation);

                        obj.UserId = GetUserTrackTime.Value.UserId;
                        obj.StartTime = dateTime;
                        obj.Location = ipAdress.IpAdressInfoID.ToString();
                        obj.IpAddress = String.IsNullOrEmpty(ipAdress.StaticIp) ? ipAdress.StartIP : ipAdress.StaticIp;
                        obj.InOutType = CheckInOutType.CheckIn;
                        obj.UpdatedOn = DateTime.Now;
                        obj.UpdatedBy = MainManageIPAddressView.UserId;
                        obj.UserTimeTrackInfoID = (int)GetUserTrackTime.Value.InTimeId;
                        //This db class is Update User Time Track
                        _manageIPAddresseRepository.UpdateUserTrackTime(obj);
                        //MainManageIPAddressView.success = "User In/Out time successfully Updated.";
                        MainManageIPAddressView.success = _commonRepository.GetMessageValue("EMTE", "User In/Out time successfully Updated.");
                    }
                    else
                    {
                        //Db class is Get Inlocation
                        ipAdress = _manageIPAddresseRepository.GetInLocation(GetUserTrackTime.Value.InLocation);
                        obj.UserId = GetUserTrackTime.Value.UserId;
                        obj.StartTime = dateTime;
                        obj.Location = ipAdress.IpAdressInfoID.ToString();
                        obj.IpAddress = String.IsNullOrEmpty(ipAdress.StaticIp) ? ipAdress.StartIP : ipAdress.StaticIp;
                        obj.CreatedOn = DateTime.Now;
                        obj.CreatedBy = MainManageIPAddressView.UserId;
                        obj.InOutType = CheckInOutType.CheckIn;
                        obj.IsTimeCardEntry = true;
                        //This db class is Update User Time Track
                        _manageIPAddresseRepository.UpdateUserTrackTime(obj);
                        //MainManageIPAddressView.success = "User In/Out time successfully Created.";
                        MainManageIPAddressView.success = _commonRepository.GetMessageValue("EMTC", "User In/Out time successfully Created.");
                    }
                }
                obj = null;
                obj1 = null;
                GetUserTrackTime.Value.dt = (Convert.ToDateTime(GetUserTrackTime.Value.dt)).ToString("MM/dd/yyyy");
                GetUserTrackTime.Value.InTime = (Convert.ToDateTime(GetUserTrackTime.Value.InTime)).ToString("hh:mmtt");

                if (GetUserTrackTime.Value.OutTime != null)
                {
                    //GetUserTrackTime.Value.OutTime = (Convert.ToDateTime(dateTime1)).ToString("hh:mmtt");
                    decimal duration = _manageIPAddresseRepository.GetTimeDuration(0, dateTime, dateTime1);
                    GetUserTrackTime.Value.TotalHours = duration;
                }
                else
                {
                    GetUserTrackTime.Value.TotalHours = 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressController - UpdateUserTime - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = GetUserTrackTime.Value, success = MainManageIPAddressView.success, Error = MainManageIPAddressView.Error });
        }

        /// <summary>
        /// This method is Send Email for User Weekly Timecard
        /// </summary>
        /// <param name="value"></param>
        /// <param name="toemail"></param>
        /// <returns></returns>
        public ActionResult SendEmail(List<UserActivityList> value, string toemail)
        {
            string message = "";
            try
            {
                MainManageIPAddressView.dt1 = Convert.ToDateTime(Session["Date1"]);
                MainManageIPAddressView.dt2 = Convert.ToDateTime(Session["Date2"]);
                List<GetUserTrackTime> list = new List<GetUserTrackTime>();
                //Get User Wise Attendance
                list = _manageIPAddresseRepository.GetUserwiseAttendance(value[0].UserId, MainManageIPAddressView.dt1, MainManageIPAddressView.dt2);

                MainManageIPAddressView.FromEmail = "wsmanagementnyc@gmail.com";
                MainManageIPAddressView.Password = "idfhuseptwavhoov";
                MainManageIPAddressView.ToEmail = toemail;
                MainManageIPAddressView.Port = "587";
                MainManageIPAddressView.Smtp = "smtp.gmail.com";
                MainManageIPAddressView.body = "";
                MainManageIPAddressView.body = "<body style='color:black;'><span style='opacity: 0'>" + DateTime.Now + "</span>";
                MainManageIPAddressView.body += "<div style='display:flex;width:100%;justify-content:center;align-items:center;padding:1rem;'>";
                MainManageIPAddressView.body += "<div style='font-family:arial;width:clamp(320px, 650px, 80vw);padding:2rem;border-radius:20px;text-align:center;border: 2px solid rgb(200,200,200);'>";
                MainManageIPAddressView.body += "<div style='background-color:rgb(29,29,29);width:120px;height:120px;border-radius:60px;margin:auto'>" +
                                     "<img src ='https://hrwestsidemarket.com/images/ws-logo.png' style ='width:120px;'></div>";

                MainManageIPAddressView.body += "<h1 style='margin:auto;font-size:2rem;font-weight: 900;color:red;margin-bottom:0rem;margin-top:2rem;text-align:center;justify-content:center;'>" +
                "Hi " + value[0].FirstName + "!" + "</h1>";
                MainManageIPAddressView.body += "<h1 style='margin:auto;margin-bottom:0rem;font-size:2rem;text-align:center;justify-content:center;color:black;'>Hope you're having a <br/> fantastic week!</h1>";

                MainManageIPAddressView.body += "<h1 style='margin:auto;margin-bottom:0rem;margin-top:2rem;font-size:20px;text-align:center;justify-content:center;font-weight: 600;color:black;'>Here are the hours you worked during the pay period</h1>";

                MainManageIPAddressView.body += "<h1 style='font-size:20px;margin:auto;display:flex;width:fit-content;color:rgb(82, 118, 173);margin-bottom:0rem;text-align:center;justify-content:center;margin-top:15px;'>" + Session["weekrange"].ToString().Replace("(Current Period)", "") + "</h1>";

                MainManageIPAddressView.body += "<div style='margin-top:2rem;text-align:center;margin-bottom:1rem;'><h3 style='color:black;'>Hours Summary</h3>" +
                    "<div><ul style='display:flex;list-style:none;margin:0px;'>" +
                                 "<li style ='width:fit-content;padding:10px;margin-right:5px;background:rgb(242, 204, 129);border-radius:5px;'>" +
                                      "<span style ='font-size:12px;margin-bottom:12px;'>Total Hours Worked</span><span style ='font-weight:bold;font-size:1.25rem;color:rgb(94, 54, 5);display:block;margin-top:15px;'>" + value[0].TotalHoursWorked + " </span>" +
                                  "</li>" +
                                  "<li style='width:fit-content;padding:10px;margin-right:5px;background:rgb(209, 209, 209);border-radius:5px;'>" +
                                      "<span style ='font-size:12px;margin-bottom:12px;'> Regular </span><span style ='font-weight:bold;font-size:1.25rem;display:block;margin-top:15px;'>" + value[0].Regular + "</span>" +
                                  "</li>" +
                                  "<li style = 'width:fit-content;padding:10px;margin-right:5px;background:rgb(209, 209, 209);border-radius:5px;'>" +
                                      "<span style ='font-size:12px;margin-bottom:12px;'> Overtime </span><span style ='font-weight:bold;font-size:1.25rem;display:block;margin-top:15px;'>" + value[0].OverTime + "</span>" +
                                  "</li>" +
                                  "<li style ='width:fit-content;padding:10px;margin-right:5px;background:rgb(100, 216, 234);border-radius:5px;'>" +
                                  "<span style ='font-size:12px;margin-bottom:12px;'> Office Work </span><span style ='font-weight:bold;font-size:1.25rem;color:rgb(21, 68, 112);display:block;margin-top:15px;'>" + (value[0].OfficeHours == null ? 0 : value[0].OfficeHours) + "</span>" +
                                  "</li>" +
                                  "<li style ='width:fit-content;padding:10px;margin-right:5px;background:rgb(225, 193, 245);border-radius:5px;'>" +
                                  "<span style ='font-size:12px;margin-bottom:12px;'> Remote Work </span><span style = 'font-weight:bold;font-size:1.25rem;color:rgb(103, 35, 103);display:block;margin-top:15px;'> " + (value[0].RemoteHours == null ? 0 : value[0].RemoteHours) + " </span>" +
                                  "</li>" +
                                  "</ul></div>" +
                                  "</div>";


                MainManageIPAddressView.body += "<div style='width:100%;text-align:center;margin:auto;'>" +
                        "<h3 style='text-align:center;color:black;margin:auto;margin-bottom:1rem;'> Time Card Details</h3>" +
                        "<table cellpadding='0' cellspacing='0' style='width:100%;margin-top:20px;margin:auto;'>" +
                         "<thead style='background-color:rgb(196, 196, 196);height:40px;'>" +
                            "<tr style ='padding:20px;'>" +
                                   "<td style ='font-weight:bolder;width:22%;font-size:12px;border-top-left-radius:10px;border-right:1px solid rgb(180, 180, 180);text-align:center;'>Date</td>" +
                                   "<td style ='font-weight:bolder;font-size:12px;border-right:1px solid rgb(180, 180, 180);text-align:center;'>In Time</td>" +
                                   "<td style ='font-weight:bolder;font-size:12px;border-right:1px solid rgb(180, 180, 180);text-align:center;'>Out Time</td>" +
                                   "<td style ='font-weight:bolder;font-size:12px;border-right:1px solid rgb(180, 180, 180);text-align:center;'>Location In</td>" +
                                   "<td style ='font-weight:bolder;font-size:12px;border-right:1px solid rgb(180, 180, 180);text-align:center;'>Location Out</td>" +
                                   "<td style ='font-weight:bolder;font-size:12px;border-top-right-radius:10px;text-align:center;'>Total Hours</td>" +
                            "</tr></thead><tbody>";
                foreach (var lst in list)
                {
                    MainManageIPAddressView.body += "<tr style='background: rgb(255, 255, 255);height: 30px;'>" +
                           "<td style='width:22%;font-size:12px;border-bottom:1px solid rgb(207, 207, 207);text-align:center;'>" + lst.dt + "</td>" +
                           "<td style='font-size:12px;border-bottom:1px solid rgb(207, 207, 207);text-align:center;'>" + lst.InTime + "</td>" +
                           "<td style='font-size:12px;border-bottom:1px solid rgb(207, 207, 207);text-align:center;'>" + lst.OutTime + "</td>" +
                           "<td style='font-size:12px;border-bottom:1px solid rgb(207, 207, 207);text-align:center;'>" + lst.InLocation + "</td>" +
                           "<td style='font-size:12px;border-bottom:1px solid rgb(207, 207, 207);text-align:center;'>" + lst.OutLocation + "</td>" +
                           "<td style='font-size:12px;border-bottom:1px solid rgb(207, 207, 207);text-align:center;'>" + lst.TotalHours + "</td>" +
                           "</tr>";
                }
                MainManageIPAddressView.body += "</tbody></table>";
                MainManageIPAddressView.body += "<p style='margin:0px; width:100%;font-size:11px;background-color:rgb(196, 196, 196);height:40px;text-align:center;border-bottom-left-radius:10px;border-bottom-right-radius:10px;color:black; display:flex;margin:auto;'><span style='margin: auto;'>If you have any questions, please don't hesitate to contact us at <a href='mailto:mzuleta@wmarketnyc.com' style='font-weight:bold;margin:auto;' target='_blank'>mzuleta@wmarketnyc.com</a></span></p></div></div><span style='opacity: 0'>" + DateTime.Now + "</span></body>";

                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(MainManageIPAddressView.Smtp.ToString());
                mail.Sender = new MailAddress(MainManageIPAddressView.FromEmail);
                mail.From = new MailAddress(MainManageIPAddressView.FromEmail); //you have to provide your gmail address as from address
                mail.To.Add(MainManageIPAddressView.ToEmail);
                mail.Subject = "WESTSIDE MARKET - Your Weekly Timecard (Pay Period " + Session["weekrange"].ToString().Replace("(Current Period)", "") + ")";
                mail.Body = String.Format(MainManageIPAddressView.body);
                mail.IsBodyHtml = true;
                SmtpServer.UseDefaultCredentials = true;
                SmtpServer.Port = Convert.ToInt32(MainManageIPAddressView.Port.ToString());
                SmtpServer.Credentials = new System.Net.NetworkCredential(MainManageIPAddressView.FromEmail.ToString(), MainManageIPAddressView.Password.ToString()); //you have to provide you gamil username and password
                SmtpServer.EnableSsl = true;
                try
                {
                    SmtpServer.Send(mail);
                    message = "Success";
                }
                catch (Exception ex)
                {
                    logger.Error("ManageIPAddressController - SendEmail - " + DateTime.Now + " - " + ex.Message.ToString());
                }
            }
            catch (Exception Ex)
            {
                logger.Error("ManageIPAddressController - SendEmail - " + DateTime.Now + " - " + Ex.Message.ToString());
            }
            return Json(message);
        }

        /// <summary>
        /// This method is Is mail send allow or not
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult IsMailSendAllow(string value)
        {
            string message = "";
            try
            {
                //This class is update Is Mail Send allow 
                _manageIPAddresseRepository.UpdateIsMailSendAllow(value);
                message = "Success";
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressController - IsMailSendAllow - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(message);
        }
        /// <summary>
        /// This method is Remove user Time Info 
        /// </summary>
        /// <param name="Info"></param>
        /// <returns></returns>
        public ActionResult RemoveUserTime(CRUDModel<GetUserTrackTime> Info)
        {
            UserTimeTrackInfo obj = new UserTimeTrackInfo();
            try
            {
                _manageIPAddresseRepository.RemoveUserTime(Info);

                MainManageIPAddressView.success = "Successfully Deleted!";
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressController - RemoveUserTime - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { success = MainManageIPAddressView.success, Error = MainManageIPAddressView.Error });
        }
        /// <summary>
        /// This methos is return Pay Period Settings List
        /// </summary>
        /// <returns></returns>
        public ActionResult PayPeriodSettings()
        {
            ViewBag.Title = "Pay Period Settings - Synthesis";
            _commonRepository.LogEntries();     //Harsh's code
            ViewBag.EmailSuccess = _commonRepository.GetMessageValue("PPES", "Email Configuration save Successfully!");

            //Get Pay period Setting list
            var a = _manageIPAddresseRepository.PayPeriodSettingsList();
            try
            {
                if (a != null)
                {
                    if (a.Count > 0)
                    {
                        ViewBag.ismail = a[0].IsMailSendAllow;
                    }
                    else
                    {
                        ViewBag.ismail = "false";
                    }
                }
                else
                {
                    ViewBag.ismail = "false";
                }
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressController - PayPeriodSettings - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           
            //Get Pay period Setting list
            return View(_manageIPAddresseRepository.PayPeriodSettingsList());
        }

        /// <summary>
        /// This method is return view of Add Pay period settings
        /// </summary>
        /// <returns></returns>
        public ActionResult AddPayPeriodSettings()
        {

            return View();
        }
        /// <summary>
        /// This method is Save Pay Period Settings
        /// </summary>
        /// <param name="Obj"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPayPeriodSettings(PayPeriodSettings Obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                    Obj.CreatedDate = DateTime.Now;
                    //Db class is get user Id by username.
                    Obj.CreatedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                    //This db class is Save Pay Period setting
                    _manageIPAddresseRepository.SavePayPeriodSetting(Obj);
                }

            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressController - AddPayPeriodSettings - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return RedirectToAction("PayPeriodSettings");

        }
        /// <summary>
        /// This method is get Editpay Period setting by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditPayPeriodSettings(int? id)
        {
            //Get pay Period Id
            PayPeriodSettings model = _manageIPAddresseRepository.GetPayPeiodId(id);
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
               
                if (model == null)
                {
                    return HttpNotFound();
                }
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressController - EditPayPeriodSettings - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           

            return View(model);
        }
        /// <summary>
        ///  This method is Update Period settings data
        /// </summary>
        /// <param name="Obj"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPayPeriodSettings(PayPeriodSettings Obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    MainManageIPAddressView.startWeek = Enum.GetName(typeof(Week), Obj.Day);
                    var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                    var end = (int)Obj.Day == 0 ? 6 : (int)Obj.Day - 1;
                    MainManageIPAddressView.endWeek = Enum.GetName(typeof(Week), end);
                    DateTime weekRanges = WeekDays(MainManageIPAddressView.startWeek, MainManageIPAddressView.endWeek, DateTime.Now.Year.ToString());
                    DateTime startdate = new DateTime();
                    DateTime enddate = new DateTime();
                    //This db class is clear Table Week masters
                    _manageIPAddresseRepository.ClearTableWeekMasters();

                    for (int i = 1; i <= 52; i++)
                    {
                        WeekMaster master = new WeekMaster();
                        startdate = weekRanges;
                        enddate = startdate.AddDays(6);
                        master.StartWeek = (int)Obj.Day;
                        master.EndWeek = end;
                        master.StartDate = startdate;
                        master.EndDate = enddate;
                        master.Year = DateTime.Now.Year;
                        master.WeekNo = i;
                        //This class is save Week Master
                        _manageIPAddresseRepository.SaveWeekMaster(master);
                        weekRanges = enddate.AddDays(1);
                    }
                    Obj.UpdatedDate = DateTime.Now;
                    //Db class is get user Id by username.
                    Obj.UpdatedBy = Convert.ToInt32(_commonRepository.getUserId(UserName));
                    //This class is Update Pay Period Setting
                    _manageIPAddresseRepository.UpdatePayPeriodSetting(Obj);
                }
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressController - EditPayPeriodSettings - " + DateTime.Now + " - " + ex.Message.ToString());
            }
          
            return RedirectToAction("PayPeriodSettings");
        }
        /// <summary>
        /// This method is get All Dates In Month
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public IEnumerable<DateTime> AllDatesInMonth(int year, int month)
        {
            MainManageIPAddressView.days = DateTime.DaysInMonth(year, month);
            for (int day = 1; day <= MainManageIPAddressView.days; day++)
            {
                Session["days"] = day;
                yield return new DateTime(year, month, day);
            }
        }
        /// <summary>
        ///  This method is get Week days
        /// </summary>
        /// <param name="startMeek"></param>
        /// <param name="endWeek"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public DateTime WeekDays(string startMeek, string endWeek, string year)
        {
            var AllDate = AllDatesInMonth(Convert.ToInt32(year), 1);
            var startDate = new DateTime();
            try
            {
                if (startMeek == "Monday")
                {
                    startDate = AllDate.Where(i => i.DayOfWeek == DayOfWeek.Monday).FirstOrDefault();
                }
                else if (startMeek == "Tuesday")
                {
                    startDate = AllDate.Where(i => i.DayOfWeek == DayOfWeek.Tuesday).FirstOrDefault();
                }
                else if (startMeek == "Wednesday")
                {
                    startDate = AllDate.Where(i => i.DayOfWeek == DayOfWeek.Wednesday).FirstOrDefault();
                }
                else if (startMeek == "Thursday")
                {
                    startDate = AllDate.Where(i => i.DayOfWeek == DayOfWeek.Thursday).FirstOrDefault();
                }
                else if (startMeek == "Friday")
                {
                    startDate = AllDate.Where(i => i.DayOfWeek == DayOfWeek.Friday).FirstOrDefault();
                }
                else if (startMeek == "Saturday")
                {
                    startDate = AllDate.Where(i => i.DayOfWeek == DayOfWeek.Saturday).FirstOrDefault();
                }
                else if (startMeek == "Sunday")
                {
                    startDate = AllDate.Where(i => i.DayOfWeek == DayOfWeek.Sunday).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                logger.Error("ManageIPAddressController - WeekDays - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           
            return startDate;
        }
    }
}