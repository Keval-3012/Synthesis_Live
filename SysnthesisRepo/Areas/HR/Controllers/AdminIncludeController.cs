using EntityModels.Models;
using Microsoft.Extensions.Logging;
using NLog;
using Repository;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SysnthesisRepo.Areas.HR.Controllers
{
    public class AdminIncludeController : Controller
    {
        private DBContext db = new DBContext();
        private readonly ICommonRepository _CommonRepository;
        private readonly IMastersBindRepository _mastersBindRepository;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public AdminIncludeController()
        {
            this._CommonRepository = new CommonRepository(new DBContext());
            this._mastersBindRepository = new MastersBindRepository(new DBContext());
        }
        // GET: HR/AdminInclude
        public ActionResult Header()
        {

            //
            Header obj = new Header();
            try
            {
                obj.FirstName = Session["EmployeeUserName"].ToString();
                obj.UserRole = db.UserMasters.Where(s => s.UserName == User.Identity.Name).Count() > 0 ? db.UserMasters.Where(s => s.UserName == User.Identity.Name).FirstOrDefault().UserTypeMasters.UserType : "";
                if (Roles.IsUserInRole("Administrator"))
                {
                    var StoreData = db.StoreMasters.Where(s => s.IsActive == true).Select(s => new { s.StoreId, s.NickName }).OrderBy(o => o.NickName).ToList();
                    if (StoreData.Count() > 1)
                    {
                        ViewBag.HeaderStoreId = new SelectList(StoreData, "StoreId", "NickName", Session["StoreId"]);
                    }
                    else
                    {
                        var SID = StoreData.Count() > 0 ? StoreData.FirstOrDefault().StoreId : 0;
                        Session["StoreId"] = SID;
                        ViewBag.HeaderStoreId = new SelectList(StoreData, "StoreId", "NickName", SID);
                    }
                    ViewBag.StoreCount = StoreData.Count();
                }
                else
                {
                    var StoreList = _CommonRepository.GetHeaderStoreList(1);
                    if (StoreList.Count() > 1)
                    {
                        if (Convert.ToInt32(Session["StoreId"]) > 0)
                        {
                            ViewBag.HeaderStoreId = new SelectList(StoreList, "StoreId", "NickName", Session["StoreId"]);
                        }
                        else
                        {
                            ViewBag.HeaderStoreId = new SelectList(StoreList, "StoreId", "NickName");
                        }
                    }
                    else
                    {
                        //var SID = StoreList.Count() > 0 ? StoreList.FirstOrDefault().StoreId : 0;
                        //Session["StoreId"] = SID;
                        ViewBag.HeaderStoreId = new SelectList(StoreList, "StoreId", "NickName");
                    }
                    ViewBag.StoreCount = StoreList.Count();

                }
                //string myIP = GetIPAddress();
                //IpAdressInfo isIpContain = null;
                //isIpContain = db.Database.SqlQuery<IpAdressInfo>("SP_IpAddressInfo @Mode = {0},@IpAddress = {1}", "SelectCheckIP", myIP).FirstOrDefault();
                //var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                //var Userid = _CommonRepository.getUserId(UserName);
                //UserMaster user = db.UserMasters.Where(s => s.UserId == Userid).FirstOrDefault();
                //if (user.TrackHours == true)
                ////Db class for get user Id.
                //{
                //    Session["CheckInOutTrack"] = 1;
                //}
                //else
                //{
                //    Session["CheckInOutTrack"] = 0;
                //}
                //if (isIpContain != null)
                //{


                //    UserIpInfo isuserAllow = db.UserIpInfos.Where(s => s.UserID == Userid && s.IpAdressInfoID == isIpContain.IpAdressInfoID).FirstOrDefault();

                //    if (isuserAllow != null)
                //    {
                //        Session["CheckInOut"] = 1;
                //    }
                //    else
                //    {
                //        Session["CheckInOut"] = 0;
                //    }
                //    UserTimeTrackInfo LastStatus = db.UserTimeTrackInfos.Where(a => a.UserId == Userid).ToArray().OrderByDescending(a => a.StartTime).FirstOrDefault();
                //    if (LastStatus == null)
                //    {
                //        Session["CheckInOutStatus"] = (long)CheckInOutType.CheckIn;
                //        ViewBag.CheckInOutStatusForClass = (long)CheckInOutType.CheckIn;

                //    }
                //    else if (LastStatus.InOutType == CheckInOutType.CheckIn)
                //    {
                //        Session["CheckInOutStatus"] = (long)CheckInOutType.CheckOut;
                //        ViewBag.CheckInOutStatusForClass = (long)CheckInOutType.CheckIn;
                //    }
                //    else if (LastStatus.InOutType == CheckInOutType.CheckOut)
                //    {
                //        Session["CheckInOutStatus"] = (long)CheckInOutType.CheckIn;
                //        ViewBag.CheckInOutStatusForClass = (long)CheckInOutType.CheckOut;
                //    }
                //    Session["IpAdressInfoID"] = isIpContain.IpAdressInfoID;
                //}
            }
            catch (Exception ex)
            {
                logger.Error("AdminIncludeController - Header - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return PartialView(obj);
        }

        public ActionResult SideMenu()
        {
            return View();
        }
    }
}