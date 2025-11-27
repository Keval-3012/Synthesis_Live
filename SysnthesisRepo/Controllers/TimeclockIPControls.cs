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
    public class TimeclockIPControlsController : Controller
    {

        private readonly IManageIPAddressRepository _manageIPAddresseRepository;
        private readonly ISynthesisApiRepository _synthesisApiRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly IUserActivityLogRepository _activityLogRepository;
        Logger logger = LogManager.GetCurrentClassLogger();

        ManageIPAddressViewModal MainManageIPAddressView = new ManageIPAddressViewModal();
        public TimeclockIPControlsController()
        {
            this._manageIPAddresseRepository = new ManageIPAddressRepository(new DBContext());
            this._synthesisApiRepository = new SynthesisApiRepository();
            this._commonRepository = new CommonRepository(new DBContext());
            this._activityLogRepository = new UserActivityLogRepository(new DBContext());
        }
        // GET: ManageIPAddress
        /// <summary>
        /// This method is retun view of IP Restriction
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.Title = "Timeclock IP Controls - Synthesis";
            _commonRepository.LogEntries();  
            ViewBag.ValidIPAdd = _commonRepository.GetMessageValue("IPPIA", "Please enter valid IP Address");
            ViewBag.LeastOneUser = _commonRepository.GetMessageValue("IPSLO", "Please select at least One User");
            ViewBag.StartEndDiff = _commonRepository.GetMessageValue("IPSEDR", "Start Ip and End Ip in Different Range");
            ViewBag.StartGreatEnd = _commonRepository.GetMessageValue("IPSGE", "Start Ip greater than End Ip");
            return View();
        }
        /// <summary>
        /// This method is return partial view of add
        /// </summary>
        /// <returns></returns>
        public ActionResult AddPartial()
        {
            //Get List of All users
            ViewBag.Users = _manageIPAddresseRepository.GetUsers();
            //get List of Location type value
            var enumData = _manageIPAddresseRepository.LocationTypeValue();
            ViewBag.EnumList = new SelectList(enumData, "ID", "Name");
            return PartialView("_DialogAddpartial");
        }

        /// <summary>
        /// This method is return partial view of Edit 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ActionResult Editpartial(IpAdressInfo value)
        {
            //get Ip Info address Id
            IpAdressInfo ipAdress = _manageIPAddresseRepository.GetIpInfoaddressid(value);
            //Get List of All users
            ViewBag.Users = _manageIPAddresseRepository.GetUsers();
            //get List of Location type value
            var enumData = _manageIPAddresseRepository.LocationTypeValue();
            ViewBag.EnumList = new SelectList(enumData, "ID", "Name");
            ViewBag.LocationTypeID = ipAdress.LocationType;
            //get List of User Info Id
            ViewBag.UserID = _manageIPAddresseRepository.GetUserInfoId(ipAdress).Select(s => s.UserId).ToList();
            return PartialView("_DialogEditpartial", ipAdress);
        }
        /// <summary>
        /// This method is Save Ip AddressInfo
        /// </summary>
        /// <param name="InfoAddressModel"></param>
        /// <returns></returns>
        public ActionResult Insert(CRUDModel<IpAdressInfo> InfoAddressModel)
        {
            try
            {
                if (InfoAddressModel.Value.MultiUserId != null)
                {
                    //Get List of All userName by Info addreass model
                    var user = _manageIPAddresseRepository.GetUserName(InfoAddressModel);
                    InfoAddressModel.Value.Users = String.Join(", ", user);
                }
                bool flag = true;
                if (InfoAddressModel.Value.StartIP == null && InfoAddressModel.Value.EndIP == null && InfoAddressModel.Value.StaticIp == null)
                {
                    //MainManageIPAddressView.Message = "Please Enter Static Ip or Ip Range";
                    MainManageIPAddressView.Message = _commonRepository.GetMessageValue("IPESR", "Please Enter Static Ip or Ip Range");
                    MainManageIPAddressView.Error = MainManageIPAddressView.Message;
                    flag = false;
                }

                if (InfoAddressModel.Value.StartIP != null)
                {
                    if (InfoAddressModel.Value.EndIP == null)
                    {
                        //MainManageIPAddressView.Message = "Please Enter End Ip";
                        MainManageIPAddressView.Message = _commonRepository.GetMessageValue("IPEE", "Please Enter End Ip");
                        MainManageIPAddressView.Error = MainManageIPAddressView.Message;
                        flag = false;
                    }
                }
                if (InfoAddressModel.Value.EndIP != null)
                {
                    if (InfoAddressModel.Value.StartIP == null)
                    {
                        //MainManageIPAddressView.Message = "Please Enter Start Ip";
                        MainManageIPAddressView.Message = _commonRepository.GetMessageValue("IPES", "Please Enter Start Ip");
                        MainManageIPAddressView.Error = MainManageIPAddressView.Message;
                        flag = false;
                    }
                }
                if (flag == true)
                {
                    if (ModelState.IsValid)
                    {
                        IpAdressInfo vData = null;
                        if (InfoAddressModel.Value.StaticIp != null)
                        {
                            //Get All Start End Ip
                            vData = _manageIPAddresseRepository.GetStartEndIp(InfoAddressModel.Value.StartIP, InfoAddressModel.Value.EndIP, InfoAddressModel.Value.IpAdressInfoID);

                        }
                        else if (InfoAddressModel.Value.StartIP != null && InfoAddressModel.Value.EndIP != null)
                        {
                            //this class isSelect For Update Range
                            vData = _manageIPAddresseRepository.SelectForUpdaterange(InfoAddressModel.Value.StartIP, InfoAddressModel.Value.EndIP, InfoAddressModel.Value.IpAdressInfoID);
                        }

                        if (vData != null)
                        {
                            //MainManageIPAddressView.Message = "Ip Address/Range Already Exist";
                            MainManageIPAddressView.Message = _commonRepository.GetMessageValue("IPC_E", "Ip Address/Range Already Exist");
                            MainManageIPAddressView.Error += vData.StaticIp + ":" + MainManageIPAddressView.Message;
                        }
                        else
                        {
                            if (InfoAddressModel.Value.IP == "Static")
                            {
                                InfoAddressModel.Value.StartIP = null;
                                InfoAddressModel.Value.EndIP = null;
                            }
                            if (InfoAddressModel.Value.IP == "Range")
                            {
                                InfoAddressModel.Value.StaticIp = null;

                            }
                            IpAdressInfo ipAdress = new IpAdressInfo();
                            var UserName = System.Web.HttpContext.Current.User.Identity.Name;

                            ipAdress.StaticIp = InfoAddressModel.Value.StaticIp;
                            ipAdress.StartIP = InfoAddressModel.Value.StartIP;
                            ipAdress.EndIP = InfoAddressModel.Value.EndIP;
                            ipAdress.IsActive = Convert.ToBoolean(InfoAddressModel.Value.IsActive);
                            ipAdress.Location = InfoAddressModel.Value.Location;
                            //Db class is get user Id by username.
                            ipAdress.CreatedBy = _commonRepository.getUserId(UserName);
                            ipAdress.LocationType = InfoAddressModel.Value.LocationType;
                            ipAdress.CreatedOn = DateTime.Now;
                            //this db class is save Ip Address info
                            _manageIPAddresseRepository.SaveIpAddressInfo(ipAdress);
                            var iIpaddressId = ipAdress.IpAdressInfoID;
                            if (iIpaddressId != 0)
                            {
                                //MainManageIPAddressView.success = "Ip Address Save Successfully..";
                                MainManageIPAddressView.success = _commonRepository.GetMessageValue("IPC", "Ip Address Save Successfully..");
                            }
                            ipAdress = null;
                            foreach (var item in InfoAddressModel.Value.MultiUserId)
                            {
                                UserIpInfo userIp = new UserIpInfo();
                                userIp.IpAdressInfoID = iIpaddressId;
                                userIp.UserID = Convert.ToInt32(item);
                                //this db class is save User Ip info
                                _manageIPAddresseRepository.SaveUserIpInfo(userIp);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("TimeclockIPControlsController - Insert - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            InfoAddressModel.Value.StaticIp = InfoAddressModel.Value.StaticIp != null ? InfoAddressModel.Value.StaticIp : InfoAddressModel.Value.StartIP + "-" + InfoAddressModel.Value.EndIP;
            InfoAddressModel.Value.Status = InfoAddressModel.Value.IsActive == true ? "Active" : "Inactive";
            return Json(new { data = InfoAddressModel.Value, success = MainManageIPAddressView.success, Error = MainManageIPAddressView.Error });
        }
        /// <summary>
        /// This method is Update Ip AddressInfo
        /// </summary>
        /// <param name="InfoAddressModel"></param>
        /// <returns></returns>
        public ActionResult Update(CRUDModel<IpAdressInfo> InfoAddressModel)
        {
            try
            {
                if (InfoAddressModel.Value.MultiUserId != null)
                {
                    //Get List of All userName by Info addreass model
                    var user = _manageIPAddresseRepository.GetUserName(InfoAddressModel);
                    InfoAddressModel.Value.Users = String.Join(", ", user);
                }

                if (ModelState.IsValid)
                {
                    IpAdressInfo vData = null;
                    if (InfoAddressModel.Value.StaticIp != null)
                    {
                        //Get All Start End Ip
                        vData = _manageIPAddresseRepository.GetStartEndIp(InfoAddressModel.Value.StartIP, InfoAddressModel.Value.EndIP, InfoAddressModel.Value.IpAdressInfoID);

                    }
                    else if (InfoAddressModel.Value.StartIP != null && InfoAddressModel.Value.EndIP != null)
                    {
                        //this class isSelect For Update Range
                        vData = _manageIPAddresseRepository.SelectForUpdaterange(InfoAddressModel.Value.StartIP, InfoAddressModel.Value.EndIP, InfoAddressModel.Value.IpAdressInfoID);

                    }
                    if (vData != null)
                    {
                        MainManageIPAddressView.Message = _commonRepository.GetMessageValue("IPC_E", "Ip Address/Range Already Exist");
                        MainManageIPAddressView.Error += vData.StaticIp + ":" + MainManageIPAddressView.Message;
                    }
                    else
                    {
                        if (InfoAddressModel.Value.IP == "Static")
                        {
                            InfoAddressModel.Value.StartIP = null;
                            InfoAddressModel.Value.EndIP = null;
                        }
                        if (InfoAddressModel.Value.IP == "Range")
                        {
                            InfoAddressModel.Value.StaticIp = null;

                        }
                        //Db class is get IP address Info by Id.
                        var ipAdress = _manageIPAddresseRepository.GetIpAdressInfoID(InfoAddressModel.Value.IpAdressInfoID);
                        var UserName = System.Web.HttpContext.Current.User.Identity.Name;

                        ipAdress.StaticIp = InfoAddressModel.Value.StaticIp;
                        ipAdress.StartIP = InfoAddressModel.Value.StartIP;
                        ipAdress.EndIP = InfoAddressModel.Value.EndIP;
                        ipAdress.IsActive = Convert.ToBoolean(InfoAddressModel.Value.IsActive);
                        ipAdress.Location = InfoAddressModel.Value.Location;
                        ipAdress.LocationType = InfoAddressModel.Value.LocationType;
                        //Db class is get user Id by username.
                        ipAdress.UpdatedBy = _commonRepository.getUserId(UserName);
                        ipAdress.UpdatedOn = DateTime.Now;
                        //this db class is update Ip Address info
                        _manageIPAddresseRepository.UpdateIpAddressInfo(ipAdress);
                        var iIpaddressId = ipAdress.IpAdressInfoID;
                        ipAdress = null;
                        if (iIpaddressId != 0)
                        {
                            //MainManageIPAddressView.success = "Ip Address Update Successfully..";
                            MainManageIPAddressView.success = _commonRepository.GetMessageValue("IPE", "Ip Address Update Successfully..");
                        }
                        //Db class is get user IP Info.
                        List<UserIpInfo> userIp1 = _manageIPAddresseRepository.GetUserIpInfo(iIpaddressId);
                        foreach (var item in InfoAddressModel.Value.MultiUserId)
                        {
                            UserIpInfo userIp = new UserIpInfo();
                            userIp.IpAdressInfoID = iIpaddressId;
                            userIp.UserID = Convert.ToInt32(item);
                            //this db class is save User Ip info
                            _manageIPAddresseRepository.SaveUserIpInfo(userIp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("TimeclockIPControlsController - Update - " + DateTime.Now + " - " + ex.Message.ToString());
                AdminSiteConfiguration.WriteErrorLogs("Controller : IpAddress Method : IpAddressUpdated Message:" + ex.Message + "Internal Message:" + ex.InnerException);
            }
            InfoAddressModel.Value.StaticIp = InfoAddressModel.Value.StaticIp != null ? InfoAddressModel.Value.StaticIp : InfoAddressModel.Value.StartIP + "-" + InfoAddressModel.Value.EndIP;
            InfoAddressModel.Value.Status = InfoAddressModel.Value.IsActive == true ? "Active" : "Inactive";
            return Json(new { data = InfoAddressModel.Value, success = MainManageIPAddressView.success, Error = MainManageIPAddressView.Error });

        }
        /// <summary>
        /// This method is Remove Ip Info
        /// </summary>
        /// <param name="IpAdressInfo"></param>
        /// <returns></returns>
        public ActionResult Remove(CRUDModel<IpAdressInfo> IpAdressInfo)
        {
            try
            {
                //this db class is Remove Ip info
                _manageIPAddresseRepository.RemoveIpinfo(IpAdressInfo);
                //MainManageIPAddressView.success = "Successfully Deleted!";
                MainManageIPAddressView.success = _commonRepository.GetMessageValue("IPD", "Successfully Deleted!");
            }
            catch (Exception ex)
            {
                logger.Error("TimeclockIPControlsController - Remove - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = IpAdressInfo.Deleted, success = MainManageIPAddressView.success, Error = MainManageIPAddressView.Error });
        }
        /// <summary>
        /// This method is get url data source. 
        /// </summary>
        /// <param name="dm"></param>
        /// <returns></returns>
        public ActionResult UrlDatasource(DataManagerRequest dm)
        {
            List<IpAdressInfoSelect> ipAdressInfos = new List<IpAdressInfoSelect>();
            //Get List of Address
            ipAdressInfos = _manageIPAddresseRepository.SelectAddress();
            IEnumerable DataSource = ipAdressInfos;
            DataOperations operation = new DataOperations();
            MainManageIPAddressView.count = DataSource.Cast<IpAdressInfoSelect>().Count();
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
                count = DataSource.Cast<VenodrMasterSelect>().Count();
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
                logger.Error("TimeclockIPControlsController - UrlDatasource - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }       
    }
}