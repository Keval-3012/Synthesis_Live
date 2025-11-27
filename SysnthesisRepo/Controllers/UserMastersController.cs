using EntityModels.HRModels;
using EntityModels.Models;
using NLog;
using Repository;
using Repository.IRepository;
using Syncfusion.EJ2.Base;
using SynthesisViewModal;
using SynthesisViewModal.HRViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Utility;

namespace SysnthesisRepo.Controllers
{

    public class UserMastersController : Controller
    {
        private readonly IUserMastersRepository _UserMastersRepository;
        private readonly ICommonRepository _CommonRepository;
        private readonly IUserActivityLogRepository _ActivityLogRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        protected static string InsertMessage = "";
        protected static string DeleteMessage = "";
        protected static string Editmessage = "";
        protected static string ActivityLogMessage = "";

        protected static Array Arr;
        protected static bool IsArray;
        protected static IEnumerable BindData;
        protected static bool IsEdit = false;
        protected static string StatusMessage = "";
        protected static int TotalDataCount;
        protected static string LockMessage = "";
        protected static string UnLockMessage = "";
        protected static string StatusMessageString = "";
        string UserName = System.Web.HttpContext.Current.User.Identity.Name;
        public UserMastersController()
        {
            this._UserMastersRepository = new UserMastersRepository(new DBContext());
            this._CommonRepository = new CommonRepository(new DBContext());
            this._ActivityLogRepository = new UserActivityLogRepository(new DBContext());
        }
        /// <summary>
        /// This method is Get All User Master Details.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,UsersSettings")]
        public async Task<ActionResult> Details(int Id = 0)
        {
            UserMaster UserMaster = new UserMaster();
            //Get all User Master details with id.
            UserMaster = await _UserMastersRepository.GetUserMasterDetails(Id);
            return View(UserMaster);
        }
        //[Authorize(Roles = "Administrator,UsersSettings")]
        // GET: UserMasters
        /// <summary>
        /// This method is return View with Users details.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index()
        {
            ViewBag.Title = "Users - Synthesis";
            return View();
        }
        /// <summary>
        /// This methos is Grid with user details.
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
        public ActionResult Grid(int IsBindData = 1, int currentPageIndex = 1, string orderby = "UserId", int IsAsc = 0, int PageSize = 100, int SearchRecords = 1, string Alpha = "", string SearchTitle = "")
        {
            #region MyRegion_BindData
            int startIndex = ((currentPageIndex - 1) * PageSize) + 1;
            int endIndex = startIndex + PageSize - 1;
            IEnumerable Data = null;
            try
            {
                if (IsBindData == 1 || IsEdit == true)
                {
                    //Get all Bind data With search title.
                    BindData = _UserMastersRepository.GetBindData().OfType<UserMaster>().ToList();
                    if (SearchTitle != "")
                    {
                        BindData = BindData.OfType<UserMaster>().ToList().Where(a => a.FirstName.ToString().ToLower().Contains(SearchTitle.ToLower()) || a.UserType.ToString().ToLower().Contains(SearchTitle.ToLower()) ||
                            a.UserName.ToString().ToLower().Contains(SearchTitle.ToLower())).ToList();
                    }
                    TotalDataCount = BindData.OfType<UserMaster>().ToList().Count();
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
                ViewBag.StatusMessageString = StatusMessageString;
                ViewBag.UserDeleted = _CommonRepository.GetMessageValue("UMD", "User Deleted Successfully..");

                if (TotalDataCount < endIndex)
                {
                    ViewBag.endIndex = TotalDataCount;
                }
                else
                {
                    ViewBag.endIndex = endIndex;
                }
                ViewBag.TotalDataCount = TotalDataCount;
                var ColumnName = typeof(UserMaster).GetProperties().Where(p => p.Name == orderby).FirstOrDefault();



                if (IsAsc == 1)
                {
                    ViewBag.AscVal = 0;
                    Data = BindData.OfType<UserMaster>().ToList().OrderBy(n => ColumnName.GetValue(n, null)).Skip(startIndex - 1).Take(PageSize);
                }
                else
                {
                    ViewBag.AscVal = 1;
                    Data = BindData.OfType<UserMaster>().ToList().OrderByDescending(n => ColumnName.GetValue(n, null)).Skip(startIndex - 1).Take(PageSize);
                }
                StatusMessage = "";
                StatusMessageString = "";
                //Get All Group Masters.
                ViewBag.GroupId = new SelectList(_UserMastersRepository.GetGroupMasters(), "GroupId", "Name");
                ViewBag.StoreId = new SelectList("");
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersController - Grid - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return View(Data);
            #endregion
        }

        /// <summary>
        /// This method is Create view.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,UsersSettings")]
        // GET: UserMasters/Create
        public ActionResult Create()
        {
            //Get All Group Masters.
            ViewBag.GroupId = new SelectList(_UserMastersRepository.GetGroupMasters(), "GroupId", "Name");
            ViewBag.UserTypeId = new SelectList("");
            return View();
        }
        /// <summary>
        /// This function is check Existing User.
        /// </summary>
        /// <param name="modelObj"></param>
        /// <returns></returns>
        public bool CheckExist(UserMaster modelObj)
        {
            try
            {
                if (modelObj.GroupId == null)
                {
                    ModelState.AddModelError("GroupId", " ");
                    return false;
                }
                //This class is check Exist User.
                if (_UserMastersRepository.CheckExist(modelObj))
                {
                    ModelState.AddModelError("UserName", "Already Exist...");
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersController - CheckExist - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return true;
        }
        /// <summary>
        /// This method is create new user.
        /// </summary>
        /// <param name="userMaster"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserMaster userMaster) //
        {
            if (CheckExist(userMaster))
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        //This class is Save user details.
                        _UserMastersRepository.SaveUserDetails(userMaster);

                        //StatusMessageString = "User created successfully..";
                        StatusMessageString = _CommonRepository.GetMessageValue("UMC", "User created successfully..");
                        var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                        //This  class is Get user id.
                        int UserId = _CommonRepository.getUserId(UserName);
                        ActivityLog ActLog = new ActivityLog();
                        ActLog.UserId = UserId;
                        ActLog.CreatedBy = UserId;
                        ActLog.Action = 1;
                        //This Db class is used for get user firstname.
                        ActLog.Comment = "User " + "<a href='/UserMasters/Details/" + userMaster.UserId + "'>" + userMaster.UserName + "</a> Created by " + _CommonRepository.getUserFirstName(UserName) + " on " + Common.GetDate(Common.GetEasternTime(DateTime.Now).ToString());
                        //This  class is used for Activity Log Insert.
                        _ActivityLogRepository.ActivityLogInsert(ActLog);
                        //StatusMessageString = "User created successfully..";
                        StatusMessageString = _CommonRepository.GetMessageValue("UMC", "User created successfully..");
                        ViewBag.StatusMessageString = StatusMessageString;
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.StatusMessage = "Error1";
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("UserMastersController - Create - " + DateTime.Now + " - " + ex.Message.ToString());

                }

            }
            ViewBag.StatusMessage = StatusMessage;
            ViewBag.Insert = InsertMessage;
            StatusMessage = "";
            //Get All Group Masters.
            ViewBag.GroupId = new SelectList(_UserMastersRepository.GetGroupMasters(), "GroupId", "Name", userMaster.GroupId);
            ViewBag.UserTypeId = new SelectList(_UserMastersRepository.GetUserTypeMasters(userMaster), "UserTypeId", "UserType", userMaster.UserTypeId);

            return View("Create");
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
        /// This method is Find User With Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,UsersSettings")]
        // GET: UserMasters/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            UserMaster userMaster = await _UserMastersRepository.GetFindUserMasters(id);
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
                }

                userMaster.ConfirmPassword = userMaster.Password;
                if (userMaster == null)
                {
                    return HttpNotFound();
                }
                //Get All Group Masters.
                ViewBag.GroupId = new SelectList(_UserMastersRepository.GetGroupMasters(), "GroupId", "Name", userMaster.GroupId);
                ViewBag.UserTypeId = new SelectList(_UserMastersRepository.GetUserTypeMasters(userMaster), "UserTypeId", "UserType", userMaster.UserTypeId);
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersController - Edit - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return View(userMaster);
        }
        /// <summary>
        /// This Method is update user details.
        /// </summary>
        /// <param name="userMaster"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserMaster userMaster)
        {
            ModelState.Remove("Password");
            try
            {
                if (ModelState.IsValid)
                {
                    if (CheckExist(userMaster))
                    {
                        var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                        //This  class is Get user id.
                        int UserId = _CommonRepository.getUserId(UserName);
                        userMaster.ModifiedOn = DateTime.Today;
                        _UserMastersRepository.UpdateUserDetails(userMaster);
                        ActivityLog ActLog = new ActivityLog();
                        ActLog.UserId = UserId;
                        ActLog.CreatedBy = UserId;
                        ActLog.Action = 2;
                        //This Db class is used for get user firstname.
                        ActLog.Comment = "User " + "<a href='/UserMasters/Details/" + userMaster.UserId + "'>" + userMaster.UserName + "</a> Edited by " + _CommonRepository.getUserFirstName(UserName) + " on " + Common.GetDate(Common.GetEasternTime(DateTime.Now).ToString());
                        //This  class is used for Activity Log Insert.
                        _ActivityLogRepository.ActivityLogInsert(ActLog);
                        //StatusMessageString = "User updated successfully..";
                        StatusMessageString = _CommonRepository.GetMessageValue("UME", "User updated successfully..");
                        ViewBag.StatusMessageString = StatusMessageString;
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersController - Edit - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            //Get All Group Masters.
            ViewBag.GroupId = new SelectList(_UserMastersRepository.GetGroupMasters(), "GroupId", "Name", userMaster.GroupId);
            //Get All User type masters.
            ViewBag.UserTypeId = new SelectList(_UserMastersRepository.GetUserTypeMasters(userMaster), "UserTypeId", "UserType", userMaster.UserTypeId);
            return View(userMaster);
        }

        /// <summary>
        /// This method is Find User With Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,UsersSettings")]
        // GET: UserMasters/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            //Get find user master.
            UserMaster userMaster = await _UserMastersRepository.GetFindUserMasters(id);
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
                }
                if (userMaster == null)
                {
                    return HttpNotFound();
                }
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersController - Delete - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return View(userMaster);
        }

        // POST: UserMasters/Delete/5

        /// <summary>
        /// This method is delete User details.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteConfirmed(int id)
        {
            StatusMessageString = "";
            StatusMessage = "Error";
            string name = "";

            try
            {
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                //This  class is Get user id.
                int UserId = _CommonRepository.getUserId(UserName);
                //Delete User details.
                UserMaster userMaster = _UserMastersRepository.DeleteUserDetails(id);
                ActivityLog ActLog = new ActivityLog();
                ActLog.UserId = UserId;
                ActLog.CreatedBy = UserId;
                ActLog.Action = 3;
                //This Db class is used for get user firstname.
                ActLog.Comment = "User " + userMaster.UserName + "</a> Deleted by " + _CommonRepository.getUserFirstName(UserName) + " on " + Common.GetDate(Common.GetEasternTime(DateTime.Now).ToString());
                //This  class is used for Activity Log Insert.
                _ActivityLogRepository.ActivityLogInsert(ActLog);
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersController - Delete - " + DateTime.Now + " - " + ex.Message.ToString());
                string Msg = "";
                if (ex.InnerException.ToString().Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                {
                    //Msg = "This user can not deleted. because it is used some where..";
                    Msg = _CommonRepository.GetMessageValue("UMD_E", "This user can not deleted. because it is used some where..");
                }
                return Json(Msg, JsonRequestBehavior.AllowGet);
            }

            StatusMessage = "Delete";
            //DeleteMessage = name + " deleted successfully.";
            DeleteMessage = _CommonRepository.GetMessageValue("UMDD", "##name## deleted successfully.").Replace("##name##", name);
            ViewBag.Delete = DeleteMessage;
            //StatusMessageString = "User Deleted Successfully..";
            StatusMessageString = _CommonRepository.GetMessageValue("UMD", "User Deleted Successfully..");
            return Json(StatusMessageString, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Reset user password. 
        /// </summary>
        /// <param name="Reg_userid"></param>
        /// <returns></returns>
        public ActionResult ResetPassword(int Reg_userid = 0)
        {
            User_Resetpassword objUserMst = new User_Resetpassword();
            objUserMst.Reg_userid = Reg_userid;
            ViewBag.PassRequired = _CommonRepository.GetMessageValue("UPR", "Password is required");
            ViewBag.ConPassRequired = _CommonRepository.GetMessageValue("UCPR", "ConfirmPassword is required");
            ViewBag.PassNotMatch = _CommonRepository.GetMessageValue("UPCPM", "Password and current password not match.");
            return PartialView(objUserMst);
        }

        /// <summary>
        ///  This method is Reset user password
        /// </summary>
        /// <param name="PostedData"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(User_Resetpassword PostedData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //This class is reset password for user.
                    _UserMastersRepository.Resetpassword(PostedData);

                    //StatusMessageString = "Password Reset successfully..";
                    StatusMessageString = _CommonRepository.GetMessageValue("UMRP", "Password Reset successfully..");
                    ViewBag.StatusMessageString = StatusMessageString;
                }
                catch (Exception ex)
                {
                    //StatusMessageString = "Reset Again Something Went Wrong..";
                    StatusMessageString = _CommonRepository.GetMessageValue("UMRP_E", "Reset Again Something Went Wrong.." + ex.Message);
                    ViewBag.StatusMessageString = StatusMessageString;
                }
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        ///  This method is return Change user password view.
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePassword()
        {
            ViewBag.Title = "Change Password - Synthesis";
            try
            {
                if (TempData["StatusMessage"] != "")
                {
                    ViewBag.StatusMessage = TempData["StatusMessage"];
                    TempData["StatusMessage"] = "";
                }
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersController - ChangePassword - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return View();
        }

        /// <summary>
        ///  This method is Update user password.
        /// </summary>
        /// <param name="PostedData"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(UserMaster PostedData)
        {

            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            //This  class is Get user id.
            int UserId = _CommonRepository.getUserId(UserName);
            if (UserId > 0)
            {
                try
                {
                    if (PostedData.OldPassword != null && PostedData.Password != null && PostedData.ConfirmPassword != null)
                    {
                        //Check User if password Exists.
                        if (_UserMastersRepository.CheckUserPasswordExist(PostedData, UserId))
                        {
                            //Update user password.
                            _UserMastersRepository.UpdateUserPassword(PostedData, UserId);
                            TempData["StatusMessage"] = "SuccessChangePassword";
                            //StatusMessageString = "Password Change successfully..";
                            StatusMessageString = _CommonRepository.GetMessageValue("UMCP", "Password Change successfully..");
                            ViewBag.StatusMessageString = StatusMessageString;
                        }
                        else
                        {
                            ModelState.AddModelError("OldPassword", "Enter Current Password Correct");
                            return View(PostedData);
                        }
                    }
                    else
                    {
                        if (PostedData.OldPassword == null)
                        {
                            ModelState.AddModelError("OldPassword", "Please Enter Old Password");
                        }
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
                catch (Exception ex)
                {
                    logger.Error("UserMastersController - ChangePassword - " + DateTime.Now + " - " + ex.Message.ToString());
                }

            }

            return RedirectToAction("ChangePassword");
        }
        /// <summary>
        /// This method is get All User Type.
        /// </summary>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        public ActionResult getUserTypeData(int GroupId)
        {
            //var UserTypeData = db.UserTypeMasters.Where(s => s.GroupId == GroupId && s.IsActive == true).Select(s => new { s.UserTypeId, s.UserType }).ToList();
            //Get user type by groupid.
            var UserTypeData = _UserMastersRepository.GetUserTypeMastersbyGroupId(GroupId).Select(s => new { s.UserTypeId, s.UserType }).ToList();
            return Json(UserTypeData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Get All Store data.
        /// </summary>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        public ActionResult getStoreData(int GroupId)
        {
            //Get Store master by group Id
            var UserTypeData = _UserMastersRepository.GetStoreMastersbyGroupId(GroupId).Select(s => new { s.StoreId, s.Name }).ToList();
            return Json(UserTypeData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Get User data using Storeid and groupid.
        /// </summary>
        /// <param name="StoreId"></param>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        public async Task<ActionResult> getUserData(int? StoreId, int? GroupId)
        {
            List<UserMaster> UserMasterData = new List<UserMaster>();
            try
            {
                if (StoreId != null)
                {
                    //GEt User Master data.
                    UserMasterData = _UserMastersRepository.GetUserMasterData(StoreId);
                    TotalDataCount = BindData.OfType<UserMaster>().ToList().Count();
                }
                else
                {
                    //get Rights Store data.
                    UserMasterData = _UserMastersRepository.GetRightsStoresData();
                    TotalDataCount = BindData.OfType<UserMaster>().ToList().Count();
                }
                //Get All Group Masters.
                ViewBag.GroupId = new SelectList(_UserMastersRepository.GetGroupMasters(), "GroupId", "Name", GroupId);
                //Get all Store master d
                ViewBag.StoreId = new SelectList(_UserMastersRepository.GetStoreMasters(GroupId), "StoreId", "Name", StoreId);

                ViewBag.IsBindData = 0;
                ViewBag.CurrentPageIndex = 0;
                ViewBag.LastPageIndex = 0;
                ViewBag.OrderByVal = "UserId";
                ViewBag.IsAscVal = "1";
                ViewBag.PageSize = "10";
                ViewBag.Alpha = "";
                ViewBag.SearchRecords = "";
                ViewBag.SearchTitle = "";
                ViewBag.StatusMessage = StatusMessage;
                ViewBag.Insert = InsertMessage;
                ViewBag.Edit = Editmessage;
                ViewBag.Lock = LockMessage;
                ViewBag.UnLock = UnLockMessage;
                ViewBag.startindex = 1;

                if (TotalDataCount < 100)
                {
                    ViewBag.endIndex = TotalDataCount;
                }
                else
                {
                    ViewBag.endIndex = 100;
                }
                ViewBag.TotalDataCount = TotalDataCount;
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersController - getUserData - " + DateTime.Now + " - " + ex.Message.ToString());
            }



            return PartialView("Grid", UserMasterData);
        }


        #region Synfusion Manager User Code
        public ActionResult ManagerUserList()
        {
            try
            {
                logger.Info("UserMastersRepository - ManagerUserList - " + DateTime.Now);

                // Get all groups from repository               
                var groupList = _UserMastersRepository.GetAllGroups();

                if (groupList == null || !groupList.Any())
                {
                    logger.Warn("UserMastersController - ManagerUserList - No groups found");
                    groupList = new List<GroupMaster>();
                }

                // Create dropdown list
                var groupDropdownList = groupList.Select(g => new
                {
                    GroupId = g.GroupId,
                    Name = g.Name
                }).ToList();

                // Add "All Groups" option at the beginning
                groupDropdownList.Insert(0, new { GroupId = 0, Name = "All Groups" });

                ViewBag.GroupList = groupDropdownList;
                // Set default to "All Groups" (0)
                ViewBag.DefaultGroupId = 0;
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersController - ManagerUserList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View();
        }

        public ActionResult UrlDatasource(DataManagerRequest dm, int? GroupId = null)
        {
            List<UserMaster> HrDeVm = new List<UserMaster>();
            IEnumerable DataSource = new List<UserMaster>();
            int Count = 0;
            try
            {
                logger.Info("UserMastersController - UrlDatasource - " + DateTime.Now);
                // If GroupId is null or 0, pass null to get all users
                // Otherwise pass the specific GroupId
                int? filterGroupId = (GroupId.HasValue && GroupId.Value > 0) ? GroupId : null;

                //Using this db class Get Web cam details by storeid.
                HrDeVm = _UserMastersRepository.GetBindData(filterGroupId).OfType<UserMaster>().ToList();
                DataSource = HrDeVm;

                DataOperations operation = new DataOperations();
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    string search = dm.Search[0].Key.ToString().Trim();
                    DataSource = HrDeVm.ToList().Where(x => x.FirstName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 || x.Group.Contains(search) || x.UserType.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 || x.UserName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                    //DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
                }
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                }
                Count = DataSource.Cast<UserMaster>().Count();
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
                logger.Error("UserMastersController - UrlDatasource - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return dm.RequiresCounts ? Json(new { result = DataSource, count = Count }) : Json(DataSource);
        }

        public ActionResult AddPartial()
        {
            UserMaster obj = new UserMaster();
            try
            {
                logger.Info("UserMastersController - AddPartial - " + DateTime.Now);
                obj.FirstName = obj.FirstName;
                obj.UserName = obj.UserName;
                obj.Password = obj.Password;
                obj.ConfirmPassword = obj.ConfirmPassword;
                obj.EmailAddress = obj.EmailAddress;
                obj.IsActive = obj.IsActive;
                obj.TrackHours = obj.TrackHours;
                obj.PhoneNumber = obj.PhoneNumber;
                obj.ForWhatsAppNotification = obj.ForWhatsAppNotification;
                obj.IsProductScanApp = obj.IsProductScanApp;
                obj.ProductImageUpload = obj.ProductImageUpload;
                obj.IsAbleExpiryChange = obj.IsAbleExpiryChange;
                obj.UpdateProductDetails = obj.UpdateProductDetails;
                obj.UserRightsforStoreAccess = obj.UserRightsforStoreAccess;
                obj.StoreAccess = obj.StoreAccess;
                obj.IsCustomCompetitors = obj.IsCustomCompetitors;
                ViewBag.GroupId = new SelectList(_UserMastersRepository.GetGroupMasters(), "GroupId", "Name");
                ViewBag.UserTypeId = new SelectList(_UserMastersRepository.GetUserTypeMasters(obj), "UserTypeId", "UserType");
                ViewBag.GroupWiseStateStoreId = new SelectList(_UserMastersRepository.GetGroupWiseStateStores(), "GroupWiseStateStoreId", "GroupName");
                ViewBag.DesignatedStore = new SelectList(_UserMastersRepository.GetStoreMastersLists(), "StoreId", "NickName");
                ViewBag.MultipleStorer = new SelectList(_UserMastersRepository.GetStoreMastersLists(), "StoreId", "NickName");
                ViewBag.MultipleStoreres = new SelectList(_UserMastersRepository.GetStoreMastersLists(), "StoreId", "NickName");
                ViewBag.CompaniesCompetitorsId = new SelectList(_UserMastersRepository.GetCompaniesCompetitorsList(), "CompaniesCompetitorsId", "CompetitorsName");
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersController - AddPartial - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView("_DialogUserAddpartial");
        }

        public async Task<ActionResult> EditPartial(UserMaster value)
        {
            UserMaster obj = new UserMaster();
            try
            {
                logger.Info("UserMastersController - EditPartial - " + DateTime.Now);
                obj = await _UserMastersRepository.GetFindUserMasters(value.UserId);
                ViewBag.GroupId = new SelectList(_UserMastersRepository.GetGroupMasters(), "GroupId", "Name", obj.GroupId);
                ViewBag.UserTypeId = new SelectList(_UserMastersRepository.GetUserTypeMasters(obj), "UserTypeId", "UserType", obj.UserTypeId);
                ViewBag.GroupWiseStateStoreId = new SelectList(_UserMastersRepository.GetGroupWiseStateStores(), "GroupWiseStateStoreId", "GroupName", obj.GroupWiseStateStoreId);
                ViewBag.DesignatedStore = new SelectList(_UserMastersRepository.GetStoreMastersLists(), "StoreId", "NickName", obj.DesignatedStore);
                if (obj.StoreAccess == null)
                {
                    ViewBag.MultipleStorer = new SelectList(_UserMastersRepository.GetStoreMastersLists(), "StoreId", "NickName", obj.MuiltiStoreAccess);
                }
                else
                {
                    obj.MuiltiStoreAccess = obj.StoreAccess.Split(',');
                    ViewBag.MultipleStorer = new SelectList(_UserMastersRepository.GetStoreMastersLists(), "StoreId", "NickName", obj.MuiltiStoreAccess);
                }
                if (obj.DataStoreAccess == null)
                {
                    ViewBag.MultipleStoreres = new SelectList(_UserMastersRepository.GetStoreMastersLists(), "StoreId", "NickName", obj.MuiltiStoreAccessData);
                }
                else
                {
                    obj.MuiltiStoreAccessData = obj.DataStoreAccess.Split(',');
                    ViewBag.MultipleStoreres = new SelectList(_UserMastersRepository.GetStoreMastersLists(), "StoreId", "NickName", obj.MuiltiStoreAccessData);
                }
                if (obj.CompetitorsId == null)
                {
                    ViewBag.CompaniesCompetitorsId = new SelectList(_UserMastersRepository.GetCompaniesCompetitorsList(), "CompaniesCompetitorsId", "CompetitorsName", obj.CompaniesCompetitors);
                }
                else
                {
                    obj.CompaniesCompetitors = obj.CompetitorsId.Split(',');
                    ViewBag.CompaniesCompetitorsId = new SelectList(_UserMastersRepository.GetCompaniesCompetitorsList(), "CompaniesCompetitorsId", "CompetitorsName", obj.CompaniesCompetitors);
                }

            }
            catch (Exception ex)
            {
                logger.Error("UserMastersController - EditPartial - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView("_DialogUserEditpartial", obj);
        }        

        public ActionResult InsertUserManager(UserMaster value)
        {
            string SuccessMessage = null;
            string ErrorMessage = null;
            try
            {
                if (value.FirstName != null && value.UserName != null && value.Password != null && value.ConfirmPassword != null && value.GroupId != null && value.UserTypeId != 0)
                {
                    logger.Info("UserMastersController - InsertUserManager - " + DateTime.Now);
                    var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                    int UserId = _CommonRepository.getUserId(UserName);
                    value.CreatedBy = UserId;
                    value.CreatedOn = DateTime.Today;
                    if (value.MuiltiStoreAccess == null)
                    {
                        value.StoreAccess = null;
                    }
                    else
                    {
                        value.StoreAccess = string.Join(",", value.MuiltiStoreAccess);
                    }
                    if (value.MuiltiStoreAccessData == null)
                    {
                        value.DataStoreAccess = null;
                    }
                    else
                    {
                        value.DataStoreAccess = string.Join(",", value.MuiltiStoreAccessData);
                    }
                    if (value.CompaniesCompetitors == null)
                    {
                        value.CompetitorsId = null;
                    }
                    else
                    {
                        value.CompetitorsId = string.Join(",", value.CompaniesCompetitors);
                    }
                    _UserMastersRepository.SaveUserDetails(value);

                    ActivityLog ActLog = new ActivityLog();
                    ActLog.UserId = UserId;
                    ActLog.CreatedBy = UserId;
                    ActLog.Action = 1;
                    //This Db class is used for get user firstname.
                    ActLog.Comment = "User " + "<a href='/UserMasters/Details/" + value.UserId + "'>" + value.UserName + "</a> Created by " + _CommonRepository.getUserFirstName(UserName) + " on " + Common.GetDate(Common.GetEasternTime(DateTime.Now).ToString());
                    //This  class is used for Activity Log Insert.
                    _ActivityLogRepository.ActivityLogInsert(ActLog);
                    SuccessMessage = "User Created Successfully.";
                }
                else
                {
                    ErrorMessage = "Please Insert Required Fields...";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("UserMastersController - InsertUserManager - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = value, success = SuccessMessage, Error = ErrorMessage });
        }

        public ActionResult UpdateUserManager(UserMaster value)
        {
            string SuccessMessage = null;
            string ErrorMessage = null;
            try
            {
                if(value.FirstName != null && value.UserName != null && value.GroupId != null && value.UserTypeId != 0)
                {
                    logger.Info("UserMastersController - UpdateUserManager - " + DateTime.Now);
                    var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                    int UserId = _CommonRepository.getUserId(UserName);
                    value.ModifiedBy = UserId;
                    value.ModifiedOn = DateTime.Today;
                    if (value.MuiltiStoreAccess == null)
                    {
                        value.StoreAccess = null;
                    }
                    else
                    {
                        value.StoreAccess = string.Join(",", value.MuiltiStoreAccess);
                    }
                    if (value.MuiltiStoreAccessData == null)
                    {
                        value.DataStoreAccess = null;
                    }
                    else
                    {
                        value.DataStoreAccess = string.Join(",", value.MuiltiStoreAccessData);
                    }
                    if (value.CompaniesCompetitors == null)
                    {
                        value.CompetitorsId = null;
                    }
                    else
                    {
                        value.CompetitorsId = string.Join(",", value.CompaniesCompetitors);
                    }
                    _UserMastersRepository.UpdateUserDetails(value);

                    ActivityLog ActLog = new ActivityLog();
                    ActLog.UserId = UserId;
                    ActLog.CreatedBy = UserId;
                    ActLog.Action = 2;
                    //This Db class is used for get user firstname.
                    ActLog.Comment = "User " + "<a href='/UserMasters/Details/" + value.UserId + "'>" + value.UserName + "</a> Edited by " + _CommonRepository.getUserFirstName(UserName) + " on " + Common.GetDate(Common.GetEasternTime(DateTime.Now).ToString());
                    //This  class is used for Activity Log Insert.
                    _ActivityLogRepository.ActivityLogInsert(ActLog);
                    SuccessMessage = "User Updated Successfully.";
                }
                else
                {
                    ErrorMessage = "Please Insert Required Fields...";
                }

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Error("UserMastersController - UpdateUserManager - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = value, success = SuccessMessage, Error = ErrorMessage });
        }

        public JsonResult UpdateStatusUserManager(int UserId, bool IsActive)
        {
            string Message = "";
            try
            {
                logger.Info("UserMastersController - UpdateStatusUserManager - " + DateTime.Now);
                UserMaster UM = new UserMaster();
                UM.UserId = UserId;
                UM.IsActive = IsActive;
                UM.ModifiedBy = _CommonRepository.getUserId(UserName);
                UM.ModifiedOn = DateTime.Now;
                _UserMastersRepository.UpdateUserManagerStatus(UM);
                Message = "Success";
            }
            catch (Exception ex)
            {
                Message = "Error";
                logger.Error("UserMastersController - UpdateStatusUserManager - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(Message);
        }

        public JsonResult UpdateTrackStatusUserManager(int UserId, bool TrackHours)
        {
            string Message = "";
            try
            {
                logger.Info("UserMastersController - UpdateTrackStatusUserManager - " + DateTime.Now);
                UserMaster UM = new UserMaster();
                UM.UserId = UserId;
                UM.TrackHours = TrackHours;
                UM.ModifiedBy = _CommonRepository.getUserId(UserName);
                UM.ModifiedOn = DateTime.Now;
                _UserMastersRepository.UpdateUserManagerTrackStatus(UM);
                Message = "Success";
            }
            catch (Exception ex)
            {
                Message = "Error";
                logger.Error("UserMastersController - UpdateTrackStatusUserManager - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(Message);
        }

        public JsonResult UserManagerResetPassowrd(int UserId, string Password)
        {
            string Message = "";
            try
            {
                logger.Info("UserMastersController - UserManagerResetPassowrd - " + DateTime.Now);
                UserMaster UM = new UserMaster();
                UM.UserId = UserId;
                UM.Password = Password;
                UM.ModifiedBy = _CommonRepository.getUserId(UserName);
                UM.ModifiedOn = DateTime.Now;
                _UserMastersRepository.ResetPasswordUserManager(UM);
                Message = "Success";
            }
            catch (Exception ex)
            {
                Message = "Error";
                logger.Error("UserMastersController - UserManagerResetPassowrd - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(Message);
        }

        public ActionResult UrlDatasourceChildList(DataManagerRequest dm)
        {
            List<UserRoleStoreList> HrDeVm = new List<UserRoleStoreList>();
            IEnumerable DataSource = new List<UserRoleStoreList>();
            int Count = 0;
            try
            {
                logger.Info("UserMastersController - UrlDatasourceChildList - " + DateTime.Now);
                //Using this db class Get Web cam details by storeid.

                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    HrDeVm = _UserMastersRepository.GetUserRoleStoreList(dm.Where[0].value);
                    //HrDeVm = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                }
                DataSource = HrDeVm;
                DataOperations operation = new DataOperations();
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    string search = dm.Search[0].Key.ToString().Trim();
                    //DataSource = HrDeVm.ToList().Where(x => x.FirstName.Contains(search) || x.Group.Contains(search) || x.UserType.Contains(search) || x.UserName.Contains(search)).ToList();
                    //DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
                }
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                }
                Count = DataSource.Cast<UserRoleStoreList>().Count();

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
                logger.Error("UserMastersController - UrlDatasourceChildList - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return dm.RequiresCounts ? Json(new { result = DataSource, count = Count }) : Json(DataSource);
        }

        public ActionResult UrlDatasourceGrandChildList(DataManagerRequest dm)
        {
            List<UserRoleStoreList> HrDeVm = new List<UserRoleStoreList>();
            IEnumerable DataSource = new List<UserRoleStoreList>();
            int Count = 0;
            try
            {
                logger.Info("UserMastersController - UrlDatasourceGrandChildList - " + DateTime.Now);
                //Using this db class Get Web cam details by storeid.

                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    var filterValue = dm.Where[0].value.ToString();
                    var values = filterValue.Split(',');

                    if (values.Length == 2)
                    {
                        int storeId;
                        int userId;
                        if (int.TryParse(values[0], out storeId) && int.TryParse(values[1], out userId))
                        {
                            HrDeVm = _UserMastersRepository.GetUserRoleRightsList(storeId, userId);
                        }
                    }
                }
                DataSource = HrDeVm;
                DataOperations operation = new DataOperations();
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    string search = dm.Search[0].Key.ToString().Trim();
                    //DataSource = HrDeVm.ToList().Where(x => x.FirstName.Contains(search) || x.Group.Contains(search) || x.UserType.Contains(search) || x.UserName.Contains(search)).ToList();
                    //DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
                }
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                }

                Count = DataSource.Cast<UserRoleStoreList>().Count();
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
                logger.Error("UserMastersController - UrlDatasourceGrandChildList - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return dm.RequiresCounts ? Json(new { result = DataSource, count = Count }) : Json(DataSource);
        }
        #endregion
    }
}