//using EntityModels.Migrations;
using EntityModels.Models;
using NLog;
using Repository;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SysnthesisRepo.Controllers
{
    [Authorize(Roles = "Administrator,UserRightsSettings")]
    public class UserRolesController : Controller
    {
        protected static string StatusMessageString = "";
        private readonly IUserRolesRepository _UserRolesRepository;
        private readonly ICommonRepository _CommonRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        public UserRolesController()
        {
            this._UserRolesRepository = new UserRolesRepository(new DBContext());
            this._CommonRepository = new CommonRepository(new DBContext());
        }
        // GET: UserRoles
        /// <summary>
        /// This method is Get All user type.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index()
        {
            //Get User Type Master.
            var Users = _UserRolesRepository.GetUserTypeMasters();
            ViewBag.deleteSure = _CommonRepository.GetMessageValue("URSDR", "Are you sure you want to Delete this record?");
            return View(Users);
        }

        // GET: UserRoles/Create
        /// <summary>
        /// This method is Get group Masters.
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            ViewBag.Title = "User Rights - Synthesis";
            ViewBag.LevelRequired = _CommonRepository.GetMessageValue("URLRR", "Level Is Required.");
            ViewBag.URoleRequired = _CommonRepository.GetMessageValue("URURR", "User Role Is Required.");
            UserRoles obj = new UserRoles();
            try
            {
                if (StatusMessageString != "")
                {
                    ViewBag.StatusMessageString = StatusMessageString;
                    StatusMessageString = "";
                }
                List<GroupMaster> lstGroupMaster = new List<GroupMaster>();
                //get group Masters.
                lstGroupMaster = _UserRolesRepository.GetGroupMasters();
                int GroupId = lstGroupMaster.Where(s => s.IsActive == true).Count() > 0 ? lstGroupMaster.Where(s => s.IsActive == true).FirstOrDefault().GroupId : 0;
                if (Session["GroupId"] != null)
                {
                    GroupId = Convert.ToInt32(Session["GroupId"]);
                }
                ViewBag.GroupId = new SelectList(lstGroupMaster.Where(s => s.IsActive == true).Select(s => new { s.GroupId, s.Name }).OrderBy(o => o.Name).ToList(), "GroupId", "Name", GroupId);
                //Get List of user type with groupid.
                ViewBag.UserTypeId = new SelectList(_UserRolesRepository.GetListUserTypeMasters(GroupId).Select(s => new UserTypeMaster { UserTypeId = s.UserTypeId, UserType = s.UserType + " " + s.LevelsApprovers.LevelSortName }).OrderBy(o => o.UserType), "UserTypeId", "UserType");

                //ViewBag.LevelsApproverId = new SelectList(db.levelsApprovers.Where(s => s.IsActive == true).Select(s => new { s.LevelsApproverId, s.LevelSortName }).OrderBy(o => o.LevelSortName).ToList(), "LevelsApproverId", "LevelSortName");
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesController - Create - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           
            return View(obj);
        }

        // POST: UserRoles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// This methos is userroles,Rightstorelists.
        /// </summary>
        /// <param name="userRoles"></param>
        /// <param name="RightsStoreLists"></param>
        /// <param name="SelectedRoles"></param>
        /// <param name="RoleLevelId"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserRoles userRoles, List<RightsStore> RightsStoreLists, string[] SelectedRoles, int? RoleLevelId)
        {
            try
            {
                StatusMessageString = "";
                if (userRoles.UserTypeId != 0)
                {
                    int Flg = 0;
                    //Get user roles by user type.
                    var UserRolesData = _UserRolesRepository.GetUserRolesByUserType(userRoles.UserTypeId);
                    if (UserRolesData.Count() > 0)
                    {
                        Flg = 1;
                        //Remove user roles data.
                        _UserRolesRepository.RemoveUserRolesData(UserRolesData);
                    }
                    //Remove RIght Stores by user type.
                    _UserRolesRepository.RemoveRightsStoresByUserType(userRoles.UserTypeId);

                    List<UserRoles> UserRolesList = new List<UserRoles>();
                    if (SelectedRoles != null)
                    {
                        try
                        {
                            SelectedRoles = SelectedRoles.Where(s => s != null).ToArray();

                            foreach (var item in SelectedRoles)
                            {
                                string[] RoleArray = item.Split('_');

                                UserRolesList.Add(new UserRoles { UserTypeId = userRoles.UserTypeId, UserRoleId = userRoles.UserRoleId, Role = RoleArray[0], StoreId = Convert.ToInt32(RoleArray[1]), ModuleId = Convert.ToInt32(RoleArray[2]) });
                            }
                            //Add User Roles.
                            _UserRolesRepository.AddUserRolesList(UserRolesList);

                            if (RoleLevelId != null)
                            {
                                UserTypeModuleApprover userTypeModuleApprover = new UserTypeModuleApprover();

                                userTypeModuleApprover.UserTypeId = userRoles.UserTypeId;
                                //Get Module masterid.
                                userTypeModuleApprover.ModuleId = _UserRolesRepository.GetModuleMasterID();
                                userTypeModuleApprover.LevelsApproverId = RoleLevelId.Value;
                                List<UserTypeModuleApprover> userTypeModuleApprovers = new List<UserTypeModuleApprover>();
                                //Get User Type Module Approvers.
                                userTypeModuleApprovers = _UserRolesRepository.GetUserTypeModuleApprovers();
                                if (userTypeModuleApprovers.Any(s => s.UserTypeId == userRoles.UserTypeId && s.ModuleId == userTypeModuleApprover.ModuleId))
                                {
                                    //Remove User type module approvers.
                                    _UserRolesRepository.RemoveUserTypeModuleApprovers(userTypeModuleApprovers.Where(s => s.UserTypeId == userRoles.UserTypeId && s.ModuleId == userTypeModuleApprover.ModuleId).FirstOrDefault());
                                }
                                //Add User type module approvers
                                _UserRolesRepository.AddUserTypeModuleApprovers(userTypeModuleApprover);
                            }
                            else
                            {
                                UserTypeModuleApprover userTypeModuleApprover = new UserTypeModuleApprover();

                                userTypeModuleApprover.UserTypeId = userRoles.UserTypeId;
                                //Get Module Master.
                                userTypeModuleApprover.ModuleId = _UserRolesRepository.GetModuleMasterID();
                                List<UserTypeModuleApprover> userTypeModuleApprovers = new List<UserTypeModuleApprover>();
                                //Get User Type Module Approvers.
                                userTypeModuleApprovers = _UserRolesRepository.GetUserTypeModuleApprovers();
                                if (userTypeModuleApprovers.Any(s => s.UserTypeId == userRoles.UserTypeId && s.ModuleId == userTypeModuleApprover.ModuleId))
                                {
                                    //Remove User type module approvers.
                                    _UserRolesRepository.RemoveUserTypeModuleApprovers(userTypeModuleApprovers.Where(s => s.UserTypeId == userRoles.UserTypeId && s.ModuleId == userTypeModuleApprover.ModuleId).FirstOrDefault());
                                }
                            }
                            if (RightsStoreLists != null)
                            {
                                List<RightsStore> RightsStores = new List<RightsStore>();
                                //Get Department masters.
                                var DepartmentList = _UserRolesRepository.GetDepartmentMasters().Select(s => new  {  s.DepartmentId, s.StoreId }).ToList();
                                foreach (var item in RightsStoreLists)
                                {
                                    if (item.DepartmentIds != null)
                                    {
                                        foreach (var item1 in item.DepartmentIds)
                                        {
                                            var StoreId = DepartmentList.Where(s => s.DepartmentId == Convert.ToInt32(item1)).FirstOrDefault().StoreId;
                                            RightsStores.Add(new RightsStore { ModuleId = 1, StoreId = StoreId.Value, UserTypeId = userRoles.UserTypeId, DepartmentId = Convert.ToInt32(item1) });
                                        }
                                    }
                                }
                                //Add Rights store list.
                                _UserRolesRepository.AddRightsStoresList(RightsStores);
                            }
                            //StatusMessageString = "Rights Save Successfully..";
                            _UserRolesRepository.DbSuccesfullySubmit();
                            StatusMessageString = _CommonRepository.GetMessageValue("URC", "Rights Save Successfully..");
                            ViewBag.StatusMessageString = StatusMessageString;
                        }
                        catch (Exception ex)
                        {
                            logger.Error("UserRolesController - Create - " + DateTime.Now + " - " + ex.Message.ToString());
                        }
                      
                    }
                    else if (Flg == 1)
                    {
                        _UserRolesRepository.DbSuccesfullySubmit();
                        //StatusMessageString = "Rights Removed Successfully..";
                        StatusMessageString = _CommonRepository.GetMessageValue("URR", "Rights Removed Successfully..");
                        ViewBag.StatusMessageString = StatusMessageString;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesController - Create - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            Session["GroupId"] = userRoles.GroupId;
            return RedirectToAction("Create");
        }

        /// <summary>
        /// This method Get Store Wise Roles.
        /// </summary>
        /// <param name="UserTypeId"></param>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        public async Task<ActionResult> GetStoreWiseRoles(int UserTypeId, int GroupId)
        {
            List<RightsStore> RightsStoreList = new List<RightsStore>();
            if (UserTypeId > 0)
            {
                try
                {
                    var ListData = _UserRolesRepository.GetModuleMasters();
                    //get user type by id.
                    if (_UserRolesRepository.GetUserTypeMastersByUserTypeId(UserTypeId))
                    {
                        ListData = ListData.Where(s => s.ModuleNo == 1).ToList();
                        //get All Right Stores.
                        var RightsStoreData = _UserRolesRepository.GetRightsStores();
                        if (RightsStoreData.Count > 0)
                        {
                            //Get Full List department.
                            var DepartmentList = _UserRolesRepository.GetFullListDepartmentMasters();
                            foreach (var item in ListData)
                            {
                                string[] StoreIds = RightsStoreData.Where(s => s.ModuleId == item.ModuleNo && s.UserTypeId == UserTypeId).Select(s => s.DepartmentId.ToString()).ToArray();
                                RightsStoreList.Add(new RightsStore { ModuleName = item.ModuleName, ModuleDispName = item.DisplayName, ModuleId = item.ModuleId, UserTypeId = UserTypeId, DepartmentLists = DepartmentList, DepartmentIds = StoreIds });
                            }
                        }
                        else
                        {
                            //Get Full List department.
                            var DepartmentList = _UserRolesRepository.GetFullListDepartmentMasters();
                            foreach (var item in ListData)
                            {
                                RightsStoreList.Add(new RightsStore { ModuleName = item.ModuleName, ModuleDispName = item.DisplayName, ModuleId = item.ModuleId, UserTypeId = UserTypeId, DepartmentLists = DepartmentList });
                            }
                        }
                        ////Get Full List department.
                        ViewBag.DepartmentIds = new MultiSelectList(_UserRolesRepository.GetFullListDepartmentMasters(), "DepartmentId", "DepartmentName");
                        //Get user role by usertype.
                        ViewBag.userRoles = _UserRolesRepository.GetUserRolesByUserType(UserTypeId).Select(s => s.Role + s.StoreId).ToList();
                        //Get Store master by group Id
                        ViewBag.StoresIds = _UserRolesRepository.GetStoreMastersByGroupID(GroupId);
                        //Get all store.
                        ViewBag.Stores = _UserRolesRepository.GetStoreMasters(GroupId);
                        //This db class is get full List of department.
                        ViewBag.DepartmentId = new SelectList(_UserRolesRepository.GetFullListDepartmentMasters(), "DepartmentId", "DepartmentName");
                        return PartialView("_RolesDepartmentList", RightsStoreList);
                    }
                    else
                    {
                        //get all Right Stores.
                        var RightsStoreData = _UserRolesRepository.GetRightsStores();
                        if (RightsStoreData.Count > 0)
                        {
                            foreach (var item in ListData)
                            {
                                string[] StoreIds = RightsStoreData.Where(s => s.ModuleId == item.ModuleNo && s.UserTypeId == UserTypeId).Select(s => s.StoreId.ToString()).ToArray();
                                RightsStoreList.Add(new RightsStore { ModuleName = item.ModuleName, ModuleDispName = item.DisplayName, ModuleId = item.ModuleId, UserTypeId = UserTypeId, StoreIds = StoreIds });
                            }
                        }
                        else
                        {
                            foreach (var item in ListData)
                            {
                                RightsStoreList.Add(new RightsStore { ModuleName = item.ModuleName, ModuleDispName = item.DisplayName, ModuleId = item.ModuleId, UserTypeId = UserTypeId });
                            }
                        }
                    }
                    //Get user role by usertype.
                    ViewBag.userRoles = _UserRolesRepository.GetUserRolesByUserType(UserTypeId).Select(s => s.Role + s.StoreId).ToList();
                    //Get Module Master
                    var ModuleId = _UserRolesRepository.GetModuleMasterID();
                    List<UserTypeModuleApprover> lstUserTypeModuleApprover = new List<UserTypeModuleApprover>();
                    //Get all user type module approver.
                    lstUserTypeModuleApprover = _UserRolesRepository.GetUserTypeModuleApprovers();
                    var UserTypelevelid = lstUserTypeModuleApprover.Where(s => s.UserTypeId == UserTypeId && s.ModuleId == ModuleId).Count() > 0 ? lstUserTypeModuleApprover.Where(s => s.UserTypeId == UserTypeId && s.ModuleId == ModuleId).FirstOrDefault().LevelsApproverId : 0;
                    ViewBag.Stores = _UserRolesRepository.GetStoreMastersNicNamebyGroupID(GroupId);
                    //Get Store type by groupid.
                    ViewBag.StoresIds = _UserRolesRepository.GetStoreMastersByGroupID(GroupId);
                    //Get user type by user type.
                    ViewBag.UserTypeName = _UserRolesRepository.GetUserTypeMastersByUserTypeId(UserTypeId);
                    //Get Level Approvers.
                    ViewBag.RoleLevelId = new SelectList(_UserRolesRepository.GetlevelsApprovers().Where(s => s.IsActive == true).Select(s => new { s.LevelsApproverId, s.LevelSortName }).OrderBy(o => o.LevelSortName).ToList(), "LevelsApproverId", "LevelSortName", UserTypelevelid);
                   
                }
                catch (Exception  ex)
                {
                    logger.Error("UserRolesController - GetStoreWiseRoles - " + DateTime.Now + " - " + ex.Message.ToString());
                }
                //get Module Master.
                return PartialView("_RolesList", RightsStoreList);
            }
            else
            {
                //Get user type by groupid.
                ViewBag.UserTypeId = new SelectList(_UserRolesRepository.GetUserTypeMastersByGroupID(GroupId), "UserTypeId", "UserType");
                //Get Level Approvers.
                ViewBag.RoleLevelId = new SelectList(_UserRolesRepository.GetlevelsApprovers().Where(s => s.IsActive == true).Select(s => new { s.LevelsApproverId, s.LevelSortName }).OrderBy(o => o.LevelSortName).ToList(), "LevelsApproverId", "LevelSortName");
                return PartialView("_RolesList", RightsStoreList);
            }
        }

        /// <summary>
        /// This method is get Group Wise Store.
        /// </summary>
        /// <param name="GroupId"></param>
        /// <param name="UserTypeId"></param>
        /// <returns></returns>
        public async Task<ActionResult> GetGroupWiseStore(int GroupId, int UserTypeId)
        {
            //Get Module Masters.
            var ListData = _UserRolesRepository.GetModuleMasters();
            List<RightsStore> RightsStoreList = new List<RightsStore>();
            try
            {
                //Get Rights Store.
                var RightsStoreData = _UserRolesRepository.GetRightsStores();
                if (RightsStoreData.Count > 0)
                {

                    foreach (var item in ListData)
                    {

                        string[] StoreIds = RightsStoreData.Where(s => s.ModuleId == item.ModuleId && s.UserTypeId == UserTypeId).Select(s => s.StoreId.ToString()).ToArray();
                        RightsStoreList.Add(new RightsStore { ModuleName = item.ModuleName, ModuleDispName = item.DisplayName, ModuleId = item.ModuleId, UserTypeId = UserTypeId, StoreIds = StoreIds });
                    }
                }
                else
                {
                    foreach (var item in ListData)
                    {
                        RightsStoreList.Add(new RightsStore { ModuleName = item.ModuleName, ModuleDispName = item.DisplayName, ModuleId = item.ModuleId, UserTypeId = UserTypeId });
                    }
                }
                //Get user role by usertype.
                ViewBag.userRoles = _UserRolesRepository.GetUserRolesByUserType(UserTypeId).Select(s => s.Role).ToList();
                //Get All Store master.
                ViewBag.StoreIds = new MultiSelectList(_UserRolesRepository.GetStoreMasters(GroupId).OrderBy(o => o.NickName).ToList(), "StoreId", "NickName");
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesController - GetGroupWiseStore - " + DateTime.Now + " - " + ex.Message.ToString());
            }
          
            return PartialView("_RolesList", RightsStoreList);
        }

        /// <summary>
        /// This method is Add User Type.
        /// </summary>
        /// <param name="In"></param>
        /// <returns></returns>
        public ActionResult AddUserType(string In)
        {
            UserTypeMaster obj = new UserTypeMaster();
            try
            {
                obj.UserType = In;
                obj.CreatedOn = DateTime.Now;
                obj.IsViewInvoiceOnly = false;
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                //This  class is Get user id.
                obj.CreatedBy = _CommonRepository.getUserId(UserName);
                //Get Level Approvers.
                ViewBag.LevelsApproverId = new SelectList(_UserRolesRepository.GetlevelsApprovers().Where(s => s.IsActive == true).Select(s => new { s.LevelsApproverId, s.LevelSortName }).OrderBy(o => o.LevelSortName).ToList(), "LevelsApproverId", "LevelSortName");
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesController - AddUserType - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return PartialView("_CreateUserType", obj);
        }
        /// <summary>
        /// This method is Add User Type Data.
        /// </summary>
        /// <param name="UserType"></param>
        /// <param name="GroupId"></param>
        /// <param name="LevelsApproverId"></param>
        /// <param name="IsViewInvoiceOnly"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddUserTypeData(string UserType, int GroupId, int LevelsApproverId, bool IsViewInvoiceOnly)
        {
            try
            {
                // //Get All user Type.
                int UserTypeId = _UserRolesRepository.GetAllUserTypeMasters().Where(s => s.UserType.Trim() == UserType.Trim() && s.GroupId == GroupId).Count() > 0 ? _UserRolesRepository.GetAllUserTypeMasters().Where(s => s.UserType.Trim() == UserType.Trim() && s.GroupId == GroupId).FirstOrDefault().UserTypeId : 0;
                if (UserTypeId == 0)
                {
                    UserTypeMaster obj = new UserTypeMaster();
                    if (IsViewInvoiceOnly == true)
                    {
                        UserType = "ViewOnly-" + UserType;
                    }
                    obj.UserType = UserType;
                    obj.IsActive = true;
                    obj.GroupId = GroupId;
                    obj.LevelsApproverId = LevelsApproverId;
                    obj.IsViewInvoiceOnly = IsViewInvoiceOnly;
                    obj.CreatedOn = DateTime.Now;
                    var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                    //This  class is Get user id.
                    obj.CreatedBy = _CommonRepository.getUserId(UserName);
                    //Add user Type.
                    _UserRolesRepository.AddUserTypeMasters(obj);
                    ModelState.Clear();
                    //StatusMessageString = "User Role Added Successfully..";
                    StatusMessageString = _CommonRepository.GetMessageValue("URA", "User Role Added Successfully..");
                    ViewBag.StatusMessageString = StatusMessageString;
                    //Get All user Type.
                    var IName = _UserRolesRepository.GetAllUserTypeMasters().Where(w => w.UserTypeId == obj.UserTypeId).Select(s => s.UserType).FirstOrDefault();
                    var data = _UserRolesRepository.GetAllUserTypeMasters().Select(s => new { UserTypeId = s.UserTypeId, UserType = s.UserType, IID = obj.UserTypeId, IName = UserType }).OrderBy(o => o.UserType).ToList();
                    //Get All user Type.
                    ViewBag.UserTypeId = new SelectList(_UserRolesRepository.GetAllUserTypeMasters().Where(s => s.IsActive == true && s.UserType != "Administrator" && s.GroupId == GroupId).Select(s => new { s.UserTypeId, s.UserType }).OrderBy(o => o.UserType).ToList(), "UserTypeId", "UserType");
                    ViewBag.UserTypeLatestId = obj.UserTypeId;
                    //Get Level Approvers.
                    ViewBag.LevelsApproverId = new SelectList(_UserRolesRepository.GetlevelsApprovers().Where(s => s.IsActive == true).Select(s => new { s.LevelsApproverId, s.LevelSortName }).OrderBy(o => o.LevelSortName).ToList(), "LevelsApproverId", "LevelSortName", LevelsApproverId);
                }
                else
                {
                    ViewBag.StatusMessage = "Exist";
                    //Get All user Type.
                    ViewBag.UserTypeId = new SelectList(_UserRolesRepository.GetAllUserTypeMasters().Where(s => s.IsActive == true && s.UserType != "Administrator" && s.GroupId == GroupId).Select(s => new { s.UserTypeId, s.UserType }).OrderBy(o => o.UserType).ToList(), "UserTypeId", "UserType");
                    ViewBag.UserTypeLatestId = UserTypeId;
                    //Get Level Approvers.
                    ViewBag.LevelsApproverId = new SelectList(_UserRolesRepository.GetlevelsApprovers().Where(s => s.IsActive == true).Select(s => new { s.LevelsApproverId, s.LevelSortName }).OrderBy(o => o.LevelSortName).ToList(), "LevelsApproverId", "LevelSortName", LevelsApproverId);
                }
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesController - AddUserTypeData - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            //GetRoleList();
            ViewBag.Exists = _CommonRepository.GetMessageValue("URRUAE", "User Role Already Exist..");
            return PartialView("_UserRolesList", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Get Role List.
        /// </summary>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        public async Task<ActionResult> GetRoleList(int GroupId)
        {
            //Get All User Type.
            ViewBag.UserTypeId = new SelectList(_UserRolesRepository.GetAllUserTypeMasters().Where(s => s.IsActive == true && s.UserType != "Administrator" && s.GroupId == GroupId).Select(s => new { s.UserTypeId, UserType = s.UserType + " " + s.LevelsApprovers.LevelSortName }).OrderBy(o => o.UserType).ToList(), "UserTypeId", "UserType");
            ViewBag.Exists = _CommonRepository.GetMessageValue("URRUAE", "User Role Already Exist..");
            return PartialView("_UserRolesList");
        }
    }
}