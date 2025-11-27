using EntityModels.Models;
using Microsoft.Extensions.Logging;
using NLog;
using Repository;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Utility;

namespace SysnthesisRepo.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        protected static string StatusMessage = "";
        private readonly IUserMastersRepository _iUserMastersRepository;
        private readonly IUserActivityLogRepository _activityLogRepository;
        private readonly ISynthesisApiRepository _synthesisApiRepository;
        private readonly ICommonRepository _commonRepository;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public LoginController()
        {
            this._iUserMastersRepository = new UserMastersRepository(new DBContext());
            this._activityLogRepository = new UserActivityLogRepository(new DBContext());
            this._synthesisApiRepository = new SynthesisApiRepository();
            this._commonRepository = new CommonRepository(new DBContext());
        }
        /// <summary>
        /// This method is return Index view With Login User.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()

        {
            logger.Info("LoginController - Index(Method Called) - " + DateTime.Now);
            ViewBag.statusmessage = StatusMessage;
            try
            {
                if (!string.IsNullOrEmpty(StatusMessage))
                {
                    if (StatusMessage == "t")
                    {
                        ViewBag.ErrorMessage = "Invalid username or password.";
                    }
                    if (StatusMessage == "t1")
                    {
                        ViewBag.ErrorMessage = "Login failed. Please contact System Administrator.";
                    }
                }
                StatusMessage = "";

                if (Request.Cookies["AdminLoginCoockie"] != null)
                {
                    try
                    {
                        ViewBag.ReturnUrl = Request.QueryString["ReturnUrl"].ToString();
                    }
                    catch { }

                    try
                    {
                        HttpCookie AdminLoginCoockie = Request.Cookies.Get("AdminLoginCoockie");

                        string AdminUsername = AdminLoginCoockie.Values["AdminUsername"].ToString();
                        string AdminPassword = AdminLoginCoockie.Values["AdminPassword"].ToString();
                        string AdminRememberme = AdminLoginCoockie.Values["AdminRememberme"].ToString();

                        if (AdminRememberme == true.ToString())
                        {
                            ViewBag.AdminUsername = AdminUsername;
                            ViewBag.AdminPassword = AdminPassword;
                            ViewBag.AdminRememberme = AdminRememberme;
                        }
                    }
                    catch { }
                }

            }
            catch (Exception ex)
            {
                logger.Error("LoginController - Index - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return View();
        }

        /// <summary>
        /// This method is return Get All IP Address.
        /// </summary>
        /// <returns></returns>
        public string GetIPAddress()
        {
            //string IPAddress = Request.UserHostAddress;
            string IPAddress = "";
            try
            {
                if (ConfigurationManager.AppSettings["IPAddress"].ToString() == "")
                {
                    IPAddress = Request.UserHostAddress;
                }
                else
                {
                    IPAddress = "49.36.68.47";
                }
            }
            catch (Exception ex)
            {
                logger.Error("LoginController - GetIPAddress - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return IPAddress;
        }

        /// <summary>
        /// This method is used to Login user .
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ReturnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(Admin_Login model, string ReturnUrl = "")
        {
            try
            {


                if (ModelState.IsValid)
                {
                    //This Db class is used for Valid User.
                    bool IsValidUser = _iUserMastersRepository.IsValidUser(model);
                    //string response = SynthesisApi.PosttData(path, "");
                    if (IsValidUser)
                    {
                        Session.Clear();
                        //Session.Abandon();
                        //This Db class is used for User First Login.
                        if (_iUserMastersRepository.IsFirstLogin(model))
                        {
                            FormsAuthentication.SetAuthCookie(model.UserName, false);
                            return RedirectToAction("ModifyPassword", "Login");
                        }
                        else
                        {
                            // Get API TOken



                            string AccessTokan = await _synthesisApiRepository.PosttData(model.UserName, model.Password);


                            string myIP = GetIPAddress();
                            FormsAuthentication.SetAuthCookie(model.UserName, false);
                            Session["storeid"] = "0";
                            Login login = new Login();

                            login.UserId = model.UserName;
                            login.SessionId = System.Web.HttpContext.Current.Session.SessionID;
                            login.LoggedIn = true;
                            Session["UserNm"] = model.UserName;

                            //This Db class is used for Add User Login.
                            _iUserMastersRepository.AddLogin(login);
                            Session["CurrentSession"] = login.SessionId;
                            Session["UserAccessTokan"] = AccessTokan;
                            IpAdressInfo isIpContain = null;
                            //This Db class is used for Get Ip Address Information.
                            isIpContain = _iUserMastersRepository.SP_IpAddressInfo(myIP);

                            //This Db class is used for get user by userid.
                            UserMaster User = _iUserMastersRepository.GetUserId(login.UserId);
                            clsActivityLog clsActivityLog = new clsActivityLog();
                            clsActivityLog.ModuleName = "Login";
                            clsActivityLog.PageName = "Login";
                            clsActivityLog.Message = model.UserName + " Login Successfully ";
                            clsActivityLog.CreatedBy = User.UserId;
                            clsActivityLog.Action = "Login";
                            _synthesisApiRepository.CreateLog(clsActivityLog);
                            Session["UserId"] = User.UserId;
                            if (User.TrackHours == true)
                            {
                                Session["CheckInOutTrack"] = 1;
                            }
                            else
                            {
                                Session["CheckInOutTrack"] = 0;
                            }
                            if (isIpContain != null)
                            {
                                //This Db class is used for user Allow.
                                UserIpInfo isuserAllow = _iUserMastersRepository.isuserAllow(User.UserId, isIpContain.IpAdressInfoID);

                                if (isuserAllow != null)
                                {
                                    Session["CheckInOut"] = 1;
                                }
                                else
                                {
                                    Session["CheckInOut"] = 0;
                                }
                                //This Db class is used for Get user last Status.
                                UserTimeTrackInfo LastStatus = _iUserMastersRepository.lastStatus(User.UserId);
                                if (LastStatus == null)
                                {
                                    Session["CheckInOutStatus"] = (long)CheckInOutType.CheckIn;
                                    ViewBag.CheckInOutStatusForClass = (long)CheckInOutType.CheckIn;

                                }
                                else if (LastStatus.InOutType == CheckInOutType.CheckIn)
                                {
                                    Session["CheckInOutStatus"] = (long)CheckInOutType.CheckOut;
                                    ViewBag.CheckInOutStatusForClass = (long)CheckInOutType.CheckIn;
                                }
                                else if (LastStatus.InOutType == CheckInOutType.CheckOut)
                                {
                                    Session["CheckInOutStatus"] = (long)CheckInOutType.CheckIn;
                                    ViewBag.CheckInOutStatusForClass = (long)CheckInOutType.CheckOut;
                                }
                                Session["IpAdressInfoID"] = isIpContain.IpAdressInfoID;
                            }

                            var UserTypeId = UserModule.getLoginUserTypeId(model.UserName);
                            //This Db class is used for view dashborad daily dashborad.
                            var Roles = _iUserMastersRepository.ViewDashboardDailyDashboard(UserTypeId);
                            if (Roles.Count() > 0 || UserTypeId == 1)
                            {

                                return RedirectToAction("Daily", "Dashboard");
                                //return RedirectToAction("Index", "UserMasters");
                            }
                            else
                            {
                                return RedirectToAction("IndexBeta", "Invoices");
                            }

                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Message", "Invalid User Name or Password");
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error("LoginController - Index - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View(model);
        }

        /// <summary>
        /// This method is return ModifyPassword view.
        /// </summary>
        /// <returns></returns>
        public ActionResult ModifyPassword()
        {
            TempData["StatusMessage"] = "Your password has expired, please change your password.";
            try
            {
                if (!string.IsNullOrEmpty(TempData["StatusMessage"]?.ToString()))
                {
                    ViewBag.StatusMessage = TempData["StatusMessage"];
                    TempData["StatusMessage"] = "Your password has expired, please change your password.";
                }
            }
            catch (Exception ex)
            {
                logger.Error("LoginController - ModifyPassword - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return View();
        }

        /// <summary>
        /// This method is set Modified password.
        /// </summary>
        /// <param name="PostedData"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModifyPassword(UserMaster PostedData)
        {
            try
            {
                var UserId = UserModule.getUserId();
                if (UserId > 0)
                {
                    if (PostedData.Password != null && PostedData.ConfirmPassword != null)
                    {
                        var currentPassword = _iUserMastersRepository.GetCurrentPassword(UserId);

                        if (currentPassword == PostedData.Password)
                        {
                            ModelState.AddModelError("Password", "New password should not be same as your existing password");
                            ModelState.AddModelError("ConfirmPassword", "New password should not be same as your existing password");
                            return View(PostedData);
                        }

                        _iUserMastersRepository.SP_UserMaster(PostedData.Password, UserId);
                        Session["storeid"] = "0";
                        return RedirectToAction("IndexBeta", "Invoices");
                    }
                    else
                    {
                        if (PostedData.Password == null)
                        {
                            ModelState.AddModelError("Password", "Please Enter Password");
                        }
                        if (PostedData.ConfirmPassword == null)
                        {
                            ModelState.AddModelError("ConfirmPassword", "Please Enter Password");
                        }
                        return View(PostedData);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("LoginController - ModifyPassword - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return RedirectToAction("ModifyPassword");
        }

        //public ActionResult ModifyPassword(UserMaster PostedData)
        //{
        //    try
        //    {
        //        var UserId = UserModule.getUserId();
        //        if (UserId > 0)
        //        {
        //            if (PostedData.Password != null && PostedData.ConfirmPassword != null)
        //            {
        //                _iUserMastersRepository.SP_UserMaster(PostedData.Password, UserId);
        //                ViewBag.Message = "Password Updated Successfully.";                        
        //                Session["storeid"] = "0";
        //                return RedirectToAction("IndexBeta", "Invoices");
        //            }
        //            else
        //            {
        //                if (PostedData.Password == null)
        //                {
        //                    ModelState.AddModelError("Password", "Please Enter Password");
        //                }
        //                if (PostedData.ConfirmPassword == null)
        //                {
        //                    ModelState.AddModelError("ConfirmPassword", "Please Enter Password");
        //                }

        //                return View(PostedData);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("LoginController - ModifyPassword - " + DateTime.Now + " - " + ex.Message.ToString());
        //    }
        //    return RedirectToAction("ModifyPassword");
        //}

        /// <summary>
        /// This method is used to Logout user.
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }

        /// <summary>
        /// This method is Update user CheckoInOut.
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public JsonResult UpdateCheckInOut(int Type = 0)
        {
            Common.WriteErrorLogsCheckInOut("**************************Start**************************");

            string message = "";
            int UserId = UserModule.getUserId();
            if (UserId != 0)
            {
                Common.WriteErrorLogsCheckInOut("Function:UpdateCheckInOut;Time:" + DateTime.Now + ";UserId:" + UserId + ";Type:" + Type);
                var ip = GetIPAddress();
                try
                {
                    if (Type == (long)CheckInOutType.CheckIn)
                    {
                        Session["CheckInOut"] = (long)CheckInOutType.CheckOut;
                    }
                    else if (Type == (long)CheckInOutType.CheckOut)
                    {
                        Session["CheckInOut"] = (long)CheckInOutType.CheckIn;
                    }
                    //This Db class is used for User Time Track Information.
                    _iUserMastersRepository.SP_UserTimeTrackInfo_Proc(UserId, ip, Type);
                    Common.WriteErrorLogsCheckInOut("Function:UpdateCheckInOut; CheckInOut successfully done...!!!");
                }
                catch (Exception ex)
                {
                    logger.Error("LoginController - UpdateCheckInOut - " + DateTime.Now + " - " + ex.Message.ToString());
                }



                Common.WriteErrorLogsCheckInOut("**************************End**************************");
                return Json(new { result = true, message = message }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Common.WriteErrorLogsCheckInOut("Function:UpdateCheckInOut;Time:" + DateTime.Now + ";Type:" + Type + "; UserId : " + UserId);
                Common.WriteErrorLogsCheckInOut("**************************End**************************");
                ActivityLog ActLog = new ActivityLog();
                ActLog.Action = 2;
                ActLog.Comment = "Function:UpdateCheckInOut;Time:" + DateTime.Now + ";UserId:" + UserId + ";Type:" + Type;
                //This DB class is used for Activity Log Insert
                _activityLogRepository.ActivityLogInsert(ActLog);

                return Json(new { result = false, message = message }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// This method is Update user CheckoInOut Status.
        /// </summary>
        /// <returns></returns>
        public JsonResult UpdateCheckInOutStatus()
        {
            int UserId = UserModule.getUserId();


            if (Session["CurrentSession"] == null)
                Session["CurrentSession"] = "empty";

            //Db class is used to check to see if your ID in the Logins table has LoggedIn = true - if so, continue, otherwise, redirect to Login page.

            if (_iUserMastersRepository.IsYourLoginStillTrue(System.Web.HttpContext.Current.User.Identity.Name, Session["CurrentSession"].ToString()))
            {
                //Db class is used to check to see if your user ID is being used elsewhere under a different session ID
                if (!_iUserMastersRepository.IsYourLoginStillTrue(System.Web.HttpContext.Current.User.Identity.Name, Session["CurrentSession"].ToString()))
                {
                    return Json(new { data = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // if it is being used elsewhere, update all their Logins records to LoggedIn = false, except for your session ID
                    LogEveryoneElseOut(System.Web.HttpContext.Current.User.Identity.Name, Session["CurrentSession"].ToString());
                    return Json(new { data = true }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                try
                {
                    //CheckOut
                    if (Session["IpAdressInfoID"] != null)
                    {
                        int UserID = UserModule.getUserId();
                        string IP = GetIPAddress();
                        string Location = Session["IpAdressInfoID"].ToString();
                        //Db class is used to get User Time Tracker
                        UserTimeTrackInfo LastStatus = _iUserMastersRepository.UserTimeTrac(IP, UserID, Location);
                        if (LastStatus != null)
                        {
                            if (LastStatus.InOutType == CheckInOutType.CheckIn)
                            {
                                UserTimeTrackInfo obj = new UserTimeTrackInfo();
                                obj.UserId = UserID;
                                obj.StartTime = DateTime.Now;
                                if (Session["IpAdressInfoID"] != null)
                                {
                                    obj.Location = Session["IpAdressInfoID"].ToString();
                                    obj.IpAddress = IP;
                                    obj.CreatedOn = DateTime.Now;
                                    obj.CreatedBy = UserID;
                                    obj.InOutType = CheckInOutType.CheckOut;
                                    //Db class is used to Add user time track information.
                                    _iUserMastersRepository.AddUserTimeTrackInfo(obj);
                                    FormsAuthentication.SignOut();
                                    Session.Clear();
                                    Session.Abandon();
                                    return Json(new { data = false }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            return Json(new { data = true }, JsonRequestBehavior.AllowGet);
                        }
                        return Json(new { data = true }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("LoginController - UpdateCheckInOutStatus - " + DateTime.Now + " - " + ex.Message.ToString());
                }

                return Json(new { data = true }, JsonRequestBehavior.AllowGet);
            }

        }

        //public static bool IsYourLoginStillTrue(string userId, string sid)
        //{
        //    DBContext context = new DBContext();

        //    IEnumerable<Login> logins = (from i in context.Logins
        //                                 where i.LoggedIn == true && i.UserId == userId && i.SessionId == sid
        //                                 select i).AsEnumerable();
        //    return logins.Any();
        //}

        //public static bool IsUserLoggedOnElsewhere(string userId, string sid)
        //{
        //    DBContext context = new DBContext();

        //    IEnumerable<Login> logins = (from i in context.Logins
        //                                 where i.LoggedIn == true && i.UserId == userId && i.SessionId != sid
        //                                 select i).AsEnumerable();
        //    return logins.Any();
        //}

        /// <summary>
        /// This class is LogEveryoneElseOut.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sid"></param>
        public static void LogEveryoneElseOut(string userId, string sid)
        {
            try
            {
                DBContext context = new DBContext();

                IEnumerable<Login> logins = (from i in context.Logins
                                             where i.LoggedIn == true && i.UserId == userId && i.SessionId != sid // need to filter by user ID
                                             select i).AsEnumerable();

                foreach (Login item in logins)
                {
                    item.LoggedIn = false;
                }

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("LoginController - LogEveryoneElseOut - " + DateTime.Now + " - " + ex.Message.ToString());
            }

        }

    }
}