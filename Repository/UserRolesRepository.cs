//using EntityModels.Migrations;
using EntityModels.Models;
using NLog;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Repository
{
    public class UserRolesRepository : IUserRolesRepository
    {
        private DBContext db;
        Logger logger = LogManager.GetCurrentClassLogger();
        public UserRolesRepository(DBContext context)
        {
            db = context;
        }
        public List<UserRoles> GetUserTypeMasters()
        {
            List<UserRoles> lstUserRoles = new List<UserRoles>();
            try
            {
                lstUserRoles = (db.UserTypeMasters.Where(s => s.UserType != "Administrator" && s.IsActive == true).Select(s => new { s.UserTypeId, UserType = s.UserType }).ToList()).Select(k => new UserRoles { UserType = k.UserType, UserTypeId = k.UserTypeId }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesRepository - GetUserTypeMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lstUserRoles;
        }
        public List<UserTypeMaster> GetListUserTypeMasters(int GroupID)
        {
            List<UserTypeMaster> lstUserTypeMaster = new List<UserTypeMaster>();
            try
            {
                lstUserTypeMaster = db.UserTypeMasters.Where(s => s.IsActive == true && s.UserType != "Administrator" && s.GroupId == GroupID).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesRepository - GetListUserTypeMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lstUserTypeMaster;
        }
        public List<UserRoles> GetUserRolesByUserType(int UserTypeID)
        {
            List<UserRoles> lstUserRoles = new List<UserRoles>();
            try
            {
                lstUserRoles = db.userRoles.Where(s => s.UserTypeId == UserTypeID).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesRepository - GetUserRolesByUserType - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lstUserRoles;
        }
        public List<GroupMaster> GetGroupMasters()
        {
            List<GroupMaster> lstGroupMaster = new List<GroupMaster>();
            try
            {
                lstGroupMaster = db.GroupMasters.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesRepository - GetGroupMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lstGroupMaster;
        }
        public List<UserTypeModuleApprover> GetUserTypeModuleApprovers()
        {
            List<UserTypeModuleApprover> lstUserTypeModuleApprover = new List<UserTypeModuleApprover>();
            try
            {
                lstUserTypeModuleApprover = db.UserTypeModuleApprovers.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesRepository - GetUserTypeModuleApprovers - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lstUserTypeModuleApprover;
        }
        public void RemoveUserRolesData(List<UserRoles> UserRoles)
        {
            try
            {
                db.userRoles.RemoveRange(UserRoles);
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesRepository - RemoveUserRolesData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public void RemoveRightsStoresByUserType(int UserTypeID)
        {
            try
            {
                var UserRightsStoreData = db.RightsStores.Where(s => s.UserTypeId == UserTypeID).ToList();
                db.RightsStores.RemoveRange(UserRightsStoreData);
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesRepository - RemoveRightsStoresByUserType - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public void RemoveUserTypeModuleApprovers(UserTypeModuleApprover UserTypeModuleApprover)
        {
            try
            {
                db.UserTypeModuleApprovers.Remove(UserTypeModuleApprover);
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesRepository - RemoveUserTypeModuleApprovers - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public void AddUserRolesList(List<UserRoles> lstUserRoles)
        {
            try
            {
                db.userRoles.AddRange(lstUserRoles);
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesRepository - AddUserRolesList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public void AddRightsStoresList(List<RightsStore> lstRightsStore)
        {
            try
            {
                db.RightsStores.AddRange(lstRightsStore);
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesRepository - AddRightsStoresList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public void AddUserTypeModuleApprovers(UserTypeModuleApprover UserTypeModuleApprover)
        {
            try
            {
                db.UserTypeModuleApprovers.Add(UserTypeModuleApprover);
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesRepository - AddUserTypeModuleApprovers - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public int GetModuleMasterID()
        {
            int ModuleMasterID = 0;
            try
            {
                ModuleMasterID = db.ModuleMasters.Where(s => s.ModuleName == "Invoice").FirstOrDefault().ModuleId;
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesRepository - GetModuleMasterID - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return ModuleMasterID;
        }
        public List<DepartmentMaster> GetDepartmentMasters()
        {
            List<DepartmentMaster> departmentMasters = new List<DepartmentMaster>();
            try
            {
                departmentMasters = db.DepartmentMasters.Where(s => s.IsActive).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesRepository - GetDepartmentMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return departmentMasters;
        }
        public List<DepartmentList> GetFullListDepartmentMasters()
        {
            List<DepartmentList> departmentMasters = new List<DepartmentList>();
            try
            {
                departmentMasters = db.DepartmentMasters.Where(s => s.IsActive == true).Select(s => new DepartmentList { DepartmentId = s.DepartmentId, DepartmentName = s.DepartmentName, StoreId = s.StoreId.Value }).OrderBy(o => o.DepartmentName).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesRepository - GetFullListDepartmentMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return departmentMasters;
        }
        public List<int> GetStoreMastersByGroupID(int GroupID)
        {
            List<int> lst = new List<int>();
            try
            {
                lst = db.StoreMasters.OrderBy(o => o.NickName).Where(s => s.GroupId == GroupID && s.IsActive == true).Select(s => s.StoreId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesRepository - GetStoreMastersByGroupID - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lst;
        }
        public List<string> GetStoreMastersNicNamebyGroupID(int GroupID)
        {
            List<string> lst = new List<string>();
            try
            {
                lst = db.StoreMasters.Where(s => s.GroupId == GroupID && s.IsActive == true).Select(s => s.NickName).OrderBy(o => o).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesRepository - GetStoreMastersNicNamebyGroupID - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lst;
        }
        public string GetUserTypeNamebyUserTypeId(int UserTypeId)
        {
            string lst = "";
            try
            {
                lst = db.UserTypeMasters.Find(UserTypeId).UserType;
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesRepository - GetUserTypeNamebyUserTypeId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lst;
        }
        public List<StoreMaster> GetStoreMasters(int GroupID)
        {
            List<StoreMaster> lstStoreMaster = new List<StoreMaster>();
            try
            {
                lstStoreMaster = db.StoreMasters.Where(s => s.GroupId == GroupID && s.IsActive == true).Select(s => new { s.StoreId, s.NickName }).OrderBy(o => o).ToList().Select(s => new StoreMaster { StoreId = s.StoreId, NickName = s.NickName }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesRepository - GetStoreMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lstStoreMaster;
        }
        public List<LevelsApprover> GetlevelsApprovers()
        {
            List<LevelsApprover> lstLevelsApprover = new List<LevelsApprover>();
            try
            {
                lstLevelsApprover = db.levelsApprovers.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesRepository - GetlevelsApprovers - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lstLevelsApprover;
        }
        public List<UserTypeMaster> GetUserTypeMastersByGroupID(int GroupId)
        {
            List<UserTypeMaster> lstUserTypeMaster = new List<UserTypeMaster>();
            try
            {
                lstUserTypeMaster = db.UserTypeMasters.Where(s => s.IsActive == true && s.UserType != "Administrator" && s.GroupId == GroupId).Select(s => new UserTypeMaster { UserTypeId = s.UserTypeId, UserType = s.UserType }).OrderBy(o => o.UserType).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesRepository - GetUserTypeMastersByGroupID - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lstUserTypeMaster;
        }
        public List<ModuleMaster> GetModuleMasters()
        {
            List<ModuleMaster> ModuleMasters = new List<ModuleMaster>();
            try
            {
                ModuleMasters = db.ModuleMasters.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesRepository - GetModuleMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return ModuleMasters;
        }
        public List<RightsStore> GetRightsStores()
        {
            List<RightsStore> RightsStores = new List<RightsStore>();
            try
            {
                RightsStores = db.RightsStores.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesRepository - GetRightsStores - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return RightsStores;
        }
        public List<UserTypeMaster> GetAllUserTypeMasters()
        {
            List<UserTypeMaster> UserTypeMaster = new List<UserTypeMaster>();
            try
            {
                UserTypeMaster = db.UserTypeMasters.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesRepository - GetAllUserTypeMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return UserTypeMaster;
        }
        public bool GetUserTypeMastersByUserTypeId(int UserTypeId)
        {
            bool isRecordExists = false;
            try
            {
                isRecordExists = db.UserTypeMasters.Any(s => s.UserTypeId == UserTypeId && s.IsViewInvoiceOnly == true);
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesRepository - GetUserTypeMastersByUserTypeId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return isRecordExists;
        }
        public void AddUserTypeMasters(UserTypeMaster UserTypeMaster)
        {
            try
            {
                db.UserTypeMasters.Add(UserTypeMaster);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesRepository - AddUserTypeMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            } 
        }

        public void DbSuccesfullySubmit()
        {
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("UserRolesRepository - DbSuccesfullySubmit - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
    }
}
