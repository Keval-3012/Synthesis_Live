using EntityModels.Models;
using NLog;
using Repository;
using Repository.IRepository;
using SynthesisQBOnline.BAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SysnthesisRepo.Controllers
{
    public class GroupAccountMastersController : Controller
    {
        // GET: Admin/User
        protected static string StatusMessage = "";
        protected static string InsertMessage = "";
        protected static string LockMessage = "";
        protected static string UnLockMessage = "";
        protected static string Editmessage = "";
        protected static string ActivityLogMessage = "";
        private const int FirstPageIndex = 1;
        protected static int TotalDataCount;
        protected static Array Arr;
        protected static bool IsArray;
        protected static IEnumerable BindData;
        protected static bool IsEdit = false;
        protected static string EmployeeCode;
        private readonly IGroupAccountMasterRepository _GroupAccountMasterRepository;
        private readonly IMastersBindRepository _MastersBindRepository;
        private readonly ICommonRepository _CommonRepository;
        private readonly IQBRepository _qBRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        public GroupAccountMastersController()
        {
            this._GroupAccountMasterRepository = new GroupAccountMasterRepository(new DBContext());
            this._MastersBindRepository = new MastersBindRepository(new DBContext());
            this._CommonRepository = new CommonRepository(new DBContext());
            this._qBRepository = new QBRepository(new DBContext());
        }
        // GET: GroupAccountMasters
        /// <summary>
        /// This method is return view of group account master
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.Title = "Group Account Master - Synthesis";
            ViewBag.CreateSuccess = _CommonRepository.GetMessageValue("GAC", "Group Account Name Created Successfully");
            ViewBag.DeleteSuccess = _CommonRepository.GetMessageValue("GAD", "Group Account Name Deleted Successfully");
            ViewBag.UpdateSuccess = _CommonRepository.GetMessageValue("GAE", "Group Account Name Updated Successfully");
            ViewBag.AlreadyExists = _CommonRepository.GetMessageValue("GAA", "Group Account Name Is Already Exists.");
            ViewBag.NoFound = _CommonRepository.GetMessageValue("GMGAF", "No Group Account Found");
            int storeid = 0;
            try
            {
                string SID = Convert.ToString(Session["storeid"]);
                if (SID != "0" && SID != "")
                {
                    storeid = Convert.ToInt32(Session["storeid"]);
                    ViewBag.StatusMessage = "";
                }
                else
                {
                    ViewBag.StatusMessage = "NoStoreSelected";
                }
            }
            catch (Exception ex)
            {
                logger.Error("GroupAccountMastersController - GetDepartmentMasters - " + DateTime.Now + " - " + ex.Message.ToString());
               
            }
           
            return View();
        }

        /// <summary>
        ///  This method is return view of grid group account master
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
        public ActionResult Grid(int IsBindData = 1, int currentPageIndex = 1, string orderby = "id", int IsAsc = 0, int PageSize = 50, int SearchRecords = 1, string Alpha = "", string SearchTitle = "")
        {
            #region MyRegion_Array
            try
            {

                if (IsArray == true)
                {
                    foreach (string a1 in Arr)
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
                        if (a1.Split(':')[0].ToString() == "SearchTitle")
                        {
                            SearchTitle = Convert.ToString(a1.Split(':')[1].ToString());
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                logger.Error("GroupAccountMastersController - Grid - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            IsArray = false;
            Arr = new string[]
            {  "IsBindData:" + IsBindData
                ,"currentPageIndex:" + currentPageIndex
                ,"orderby:" + orderby
                ,"IsAsc:" + IsAsc
                ,"PageSize:" + PageSize
                ,"SearchRecords:" + SearchRecords
                ,"Alpha:" + Alpha
                ,"SearchTitle:" + SearchTitle

            };
            #endregion

            #region MyRegion_BindData
            int startIndex = ((currentPageIndex - 1) * PageSize) + 1;
            int endIndex = startIndex + PageSize - 1;
            IEnumerable Data = null;
            try
            {
                if (IsBindData == 1 || IsEdit == true)
                {
                    BindData = GetData(SearchRecords, SearchTitle).OfType<ConfigurationGroup>().ToList();
                    TotalDataCount = BindData.OfType<ConfigurationGroup>().ToList().Count();
                }


                if (TotalDataCount == 0)
                {
                    StatusMessage = "NoItem";
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
                ViewBag.StatusMessage = StatusMessage;
                ViewBag.Insert = InsertMessage;
                ViewBag.Edit = Editmessage;
                ViewBag.Lock = LockMessage;
                ViewBag.UnLock = UnLockMessage;
                ViewBag.startindex = startIndex;
                ViewBag.CreateSuccess = _CommonRepository.GetMessageValue("GAC", "Group Account Name Created Successfully");
                ViewBag.DeleteSuccess = _CommonRepository.GetMessageValue("GAD", "Group Account Name Deleted Successfully");
                ViewBag.UpdateSuccess = _CommonRepository.GetMessageValue("GAE", "Group Account Name Updated Successfully");
                ViewBag.AlreadyExists = _CommonRepository.GetMessageValue("GAE", "Group Account Name Is Already Exists.");
                ViewBag.EmailExists = _CommonRepository.GetMessageValue("GAEA", "Email Is Already Exists With Another Store.");
                ViewBag.EntUpdate = _CommonRepository.GetMessageValue("GMEUS", "Entity Updated Successfully");
            
                if (TotalDataCount < endIndex)
                {
                    ViewBag.endIndex = TotalDataCount;
                }
                else
                {
                    ViewBag.endIndex = endIndex;
                }
                ViewBag.TotalDataCount = TotalDataCount;
                var ColumnName = typeof(ConfigurationGroup).GetProperties().Where(p => p.Name == orderby).FirstOrDefault();


                if (IsAsc == 1)
                {
                    ViewBag.AscVal = 0;
                    Data = BindData.OfType<ConfigurationGroup>().ToList().OrderBy(n => ColumnName.GetValue(n, null)).Skip(startIndex - 1).Take(PageSize);
                }
                else
                {
                    ViewBag.AscVal = 1;

                    Data = BindData.OfType<ConfigurationGroup>().ToList().Skip(startIndex - 1).Take(PageSize);
                }
                StatusMessage = "";
                var StoreId = Convert.ToInt32(Session["storeid"].ToString());
                //This class is Get department master
                ViewBag.QBAccount = new SelectList(_MastersBindRepository.GetDepartmentMasters(StoreId).OrderBy(o => o.DepartmentName).ToList(), "DepartmentId", "DepartmentName");
                //This class is get All Typical Balance
                ViewBag.TypicalBalance = new SelectList(_MastersBindRepository.GetTypicalbalance().OrderBy(o => o.TypicalBalanceId).ToList(), "TypicalBalanceId", "TypicalBalanceName");
                //Get all vendor master by store id
                ViewBag.VendorList = (from dataUser in _MastersBindRepository.GetVendorMaster(StoreId)
                                      select new SelectListItem
                                      {
                                          Value = dataUser.VendorId.ToString(),
                                          Text = dataUser.VendorName,
                                      }).ToList();
                //Get all Customer master
                ViewBag.CustomerList = (from dataUser in _MastersBindRepository.GetCustomerMasters(StoreId)
                                        select new SelectListItem
                                        {
                                            Value = dataUser.CustomerId.ToString(),
                                            Text = dataUser.DisplayName
                                        }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("GroupAccountMastersController - grid - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           
            return View(Data);
            #endregion
        }

        /// <summary>
        /// This class is used to get last page index for totaldatacount.
        /// </summary>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        private int getLastPageIndex(int PageSize)
        {
            int lastPageIndex = Convert.ToInt32(TotalDataCount) / PageSize;
            if (TotalDataCount % PageSize > 0)
                lastPageIndex += 1;
            return lastPageIndex;
        }

        /// <summary>
        /// This method is get data of Configuration Group
        /// </summary>
        /// <param name="SearchRecords"></param>
        /// <param name="SearchTitle"></param>
        /// <returns></returns>
        private IEnumerable GetData(int SearchRecords = 0, string SearchTitle = "")
        {
            IEnumerable RtnData = null;
            try
            {
                int storeid = 0;
                storeid = Convert.ToInt32(Session["storeid"]);
                //Get Configuration Group by store Id
                RtnData = _GroupAccountMasterRepository.GetConfigurationGroup(storeid);
            }
            catch (Exception e) {
                logger.Error("GroupAccountMastersController - GetData - " + DateTime.Now + " - " + e.Message.ToString());
            }
            return RtnData;
        }

        /// <summary>
        /// This method is Delete Configuration Group By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Delete(int Id = 0)
        {
            try
            {
                //Delete Configuration group By Id
                _GroupAccountMasterRepository.DeleteConfigGroupById(Id);
            }
            catch (Exception ex)
            {
                logger.Error("GroupAccountMastersController - Delete - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            StatusMessage = "Delete";
            ViewBag.StatusMessage = "Delete";
            return null;
        }

        /// <summary>
        /// This method is get Qb Account
        /// </summary>
        /// <returns></returns>
        public JsonResult GetQBAccount()
        {
            try
            {
                var StoreId = Convert.ToInt32(Session["storeid"].ToString());
                //This class is Get department master
                var LstQBAccount = (from a in _MastersBindRepository.GetDepartmentMasters(StoreId)
                                    orderby a.DepartmentName ascending
                                    select new
                                    {
                                        Value = a.DepartmentId.ToString(),
                                        Text = a.DepartmentName
                                    }).ToList();
                //var Dt = db.DepartmentMasters.ToList();
                return Json(new SelectList(LstQBAccount.ToArray(), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e) {
                logger.Error("GroupAccountMastersController - GetQBAccount - " + DateTime.Now + " - " + e.Message.ToString());
            };
            return Json(JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is save Group Master data
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="QBAccountid"></param>
        /// <param name="Typicalbalid"></param>
        /// <param name="memo"></param>
        /// <param name="EntityVal"></param>
        /// <returns></returns>
        public JsonResult SaveGroupMaster(string Name, string QBAccountid, string Typicalbalid, string memo, string EntityVal)
        {
            string message = "";
            try
            {
                int storeid = 0;
                storeid = Convert.ToInt32(Session["storeid"]);
                //This class is save Group master data
                message = _GroupAccountMasterRepository.SaveGroupMaster(Name, QBAccountid, Typicalbalid, memo, EntityVal, storeid);
            }
            catch (Exception Ex)
            {
                logger.Error("GroupAccountMastersController - SaveGroupMaster - " + DateTime.Now + " - " + Ex.Message.ToString());
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is update Group Master data
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Name"></param>
        /// <param name="QBAccountid"></param>
        /// <param name="Typicalbalid"></param>
        /// <param name="memo"></param>
        /// <param name="EntityVal"></param>
        /// <returns></returns>
        public JsonResult UpdateGroupMaster(int ID, string Name, string QBAccountid, string Typicalbalid, string memo, string EntityVal)
        {
            string message = "";
            try
            {
                int storeid = 0;
                storeid = Convert.ToInt32(Session["storeid"]);
                //This class is update group master
                message = _GroupAccountMasterRepository.UpdateGroupMaster(ID, Name, QBAccountid, Typicalbalid, memo, EntityVal, storeid);
            }
            catch (Exception Ex)
            {
                logger.Error("GroupAccountMastersController - UpdateGroupMaster - " + DateTime.Now + " - " + Ex.Message.ToString());
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// This method is get other Deposite Account 
        /// </summary>
        /// <returns></returns>
        public ActionResult OtherDepositeAccount()
        {
            ViewBag.SettingCreate = _CommonRepository.GetMessageValue("GMSCS", "Setting Created Successfully");
            ViewBag.SettingDelete = _CommonRepository.GetMessageValue("GMSDS", "Setting Deleted Successfully");
            ViewBag.SettingUpdate = _CommonRepository.GetMessageValue("GMSUS", "Setting Updated Successfully");
            ViewBag.SettingExists = _CommonRepository.GetMessageValue("GMSAE", "Setting Is Already Exists.");
            List<OtherDepositeSetting> RtnData = new List<OtherDepositeSetting>();
            try
            {
                int storeid = 0;
                storeid = Convert.ToInt32(Session["storeid"]);
                //Get all Department masters by account type id
                var DeptList = _MastersBindRepository.GetAllDepartmentMasters().Where(a => a.AccountTypeId == 3 || a.AccountTypeId == 19 && a.IsActive == true).ToList();
                //Get Store Online desktop data
                string Stores = _qBRepository.GetStoreOnlineDesktop(Convert.ToInt32(storeid));
                if (Stores == "Online")
                {
                    ViewBag.DepartmentList = (DeptList.Select(s => new ddllist
                    {
                        Value = s.DepartmentId.ToString(),
                        Text = s.DepartmentName,
                        selectedvalue = s.DepartmentId,
                        ListID = s.ListId
                    }).ToList()).Where(a => !a.ListID.Contains("-")).ToList();
                }
                else if (Stores == "Desktop")
                {
                    ViewBag.DepartmentList = (DeptList.Select(s => new ddllist
                    {
                        Value = s.DepartmentId.ToString(),
                        Text = s.DepartmentName,
                        selectedvalue = s.DepartmentId,
                        ListID = s.ListId
                    }).ToList()).Where(a => !a.ListID.Contains("-")).ToList();
                }
                //Get Other Deposite Account by Storeid
                RtnData = _GroupAccountMasterRepository.OtherDepositeAccount(storeid);
            }
            catch (Exception ex) 
            {
                logger.Error("GroupAccountMastersController - OtherDepositeAccount - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            ViewBag.StatusMessage = "";
            return View(RtnData);
        }


        /// <summary>
        /// This method is get QB Sync Vendor Dept 
        /// </summary>
        /// <returns></returns>
        public ActionResult QBSyncVendor_Dept()
        {
            string message = "";
            try
            {
                int StoreID = 0;
                StoreID = Convert.ToInt32(Session["storeid"]);
                QBOnlineconfiguration objOnline = new QBOnlineconfiguration();
                //This class is get Config Details
                QBOnlineconfiguration objOnlieDetail = _qBRepository.GetConfigDetail(Convert.ToInt32(StoreID));
                QBResponse objResponse = new QBResponse();

                List<SynthesisQBOnline.BAL.VendorMaster> dtVendor = SynthesisQBOnline.QBClass.QBVendor.GetVendor_All(objOnline, ref objResponse);
                if (dtVendor.Count > 0)
                {
                    //Db class is  get user Id by username.
                    _GroupAccountMasterRepository.SyncVendors(dtVendor, StoreID, _CommonRepository.getUserId(System.Web.HttpContext.Current.User.Identity.Name));
                }


                List<SynthesisQBOnline.BAL.CustomerMaster> dtCustomer = SynthesisQBOnline.QBClass.QBCustomer.GetCustomer_All(objOnline, ref objResponse);
                if (dtCustomer.Count > 0)
                {
                    //Db class is  get user Id by username.
                    _GroupAccountMasterRepository.SyncDepartment(dtCustomer, StoreID, _CommonRepository.getUserId(System.Web.HttpContext.Current.User.Identity.Name));
                }
            }
            catch (Exception Ex)
            {
                logger.Error("GroupAccountMastersController - QBSyncVendor_Dept - " + DateTime.Now + " - " + Ex.Message.ToString());
            }


            message = "Success";
            return Json(message, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// This method isUpdate Other deposite Setting  
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="QBAccountid"></param>
        /// <returns></returns>
        public JsonResult UpdateOtherDeposite_Setting(int ID, int QBAccountid)
        {
            string message = "";
            try
            {
                int storeid = 0;
                storeid = Convert.ToInt32(Session["storeid"]);
                //Update Other Depodite Setting
                message = _GroupAccountMasterRepository.UpdateOtherDeposite_Setting(ID, QBAccountid, storeid);
            }
            catch (Exception ex)
            {
                logger.Error("GroupAccountMastersController - UpdateOtherDeposite_Setting - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is get Deatils of account by Qb id
        /// </summary>
        /// <param name="QBId"></param>
        /// <returns></returns>
        public JsonResult GetDetailAccountId(string QBId)
        {
            string Id = "0";
            try
            {
                int QBIdval = Convert.ToInt32(QBId);
                int storeid = 0;
                storeid = Convert.ToInt32(Session["storeid"]);
                //This class is Get department master
                Id = (from a in _MastersBindRepository.GetDepartmentMasters(storeid)
                      where a.DepartmentId == QBIdval
                      select a.AccountDetailTypeId).FirstOrDefault().ToString();
            }
            catch (Exception ex)
            {
                logger.Error("GroupAccountMastersController - GetDetailAccountId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           
            return Json(Id, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is get Customer vendor by Qb Id
        /// </summary>
        /// <param name="QBId"></param>
        /// <returns></returns>
        public JsonResult GetCustomerVendor(string QBId)
        {
            List<ddllist> LstQBAccount = new List<ddllist>();
            try
            {
                int storeid = 0;
                storeid = Convert.ToInt32(Session["storeid"]);
                //This db class is Get Customer Vendor data
                LstQBAccount = _GroupAccountMasterRepository.GetCustomerVendor(QBId, storeid);
            }
            catch (Exception ex)
            {
                logger.Error("GroupAccountMastersController - GetCustomerVendor - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           
           
            return Json(new SelectList(LstQBAccount.ToArray(), "Value", "Text"), JsonRequestBehavior.AllowGet);
        }
    }
}