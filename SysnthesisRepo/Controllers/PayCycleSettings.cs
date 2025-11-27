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
    public class PayCycleSettingsController : Controller
    {

        private readonly IManageIPAddressRepository _manageIPAddresseRepository;
        private readonly ISynthesisApiRepository _synthesisApiRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly IUserActivityLogRepository _activityLogRepository;
        Logger logger = LogManager.GetCurrentClassLogger();

        ManageIPAddressViewModal MainManageIPAddressView = new ManageIPAddressViewModal();
        public PayCycleSettingsController()
        {
            this._manageIPAddresseRepository = new ManageIPAddressRepository(new DBContext());
            this._synthesisApiRepository = new SynthesisApiRepository();
            this._commonRepository = new CommonRepository(new DBContext());
            this._activityLogRepository = new UserActivityLogRepository(new DBContext());
        }
        /// <summary>
        /// This method is return Pay Period Settings List
        /// </summary>
        /// <returns></returns>
        public ActionResult PayCycleSettings()
        {
            ViewBag.Title = "Pay Cycle Settings - Synthesis";
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
                logger.Error("PayCycleSettingsController - PayCycleSettings - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           
            //Get Pay period Setting list
            return View(_manageIPAddresseRepository.PayPeriodSettingsList());
        }

        /// <summary>
        /// This method is return view of Add Pay period settings
        /// </summary>
        /// <returns></returns>
        public ActionResult AddPayCycleSettings()
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
        public ActionResult AddPayCycleSettings(PayPeriodSettings Obj)
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
                logger.Error("PayCycleSettingsController - AddPayPeriodSettings - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return RedirectToAction("PayCycleSettings");

        }
        /// <summary>
        /// This method is get Editpay Period setting by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditPayCycleSettings(int? id)
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
                logger.Error("PayCycleSettingsController - EditPayPeriodSettings - " + DateTime.Now + " - " + ex.Message.ToString());
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
        public ActionResult EditPayCycleSettings(PayPeriodSettings Obj)
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
                logger.Error("PayCycleSettingsController - EditPayPeriodSettings - " + DateTime.Now + " - " + ex.Message.ToString());
            }          
            return RedirectToAction("PayCycleSettings");
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
                logger.Error("PayCycleSettingsController - WeekDays - " + DateTime.Now + " - " + ex.Message.ToString());
            }           
            return startDate;
        }
    }
}