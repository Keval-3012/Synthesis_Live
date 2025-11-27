//using EntityModels.Migrations;
using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IUserRolesRepository
    {
        List<UserRoles> GetUserTypeMasters();
        List<UserTypeMaster> GetListUserTypeMasters(int GroupID);
        List<UserRoles> GetUserRolesByUserType(int UserTypeID);
        List<GroupMaster> GetGroupMasters();
        List<UserTypeModuleApprover> GetUserTypeModuleApprovers();
        void RemoveUserRolesData(List<UserRoles> UserRoles);
        void RemoveRightsStoresByUserType(int UserTypeID);
        void RemoveUserTypeModuleApprovers(UserTypeModuleApprover UserTypeModuleApprover);
        void AddUserRolesList(List<UserRoles> lstUserRoles);
        void AddUserTypeModuleApprovers(UserTypeModuleApprover UserTypeModuleApprover);
        int GetModuleMasterID();
        List<DepartmentMaster> GetDepartmentMasters();
        void AddRightsStoresList(List<RightsStore> lstRightsStore);
        List<ModuleMaster> GetModuleMasters();
        bool GetUserTypeMastersByUserTypeId(int UserTypeId);
        List<RightsStore> GetRightsStores();
        List<DepartmentList> GetFullListDepartmentMasters();
        List<int> GetStoreMastersByGroupID(int GroupID);
        List<StoreMaster> GetStoreMasters(int GroupID);
        List<string> GetStoreMastersNicNamebyGroupID(int UserTypeId);
        List<LevelsApprover> GetlevelsApprovers();
        List<UserTypeMaster> GetUserTypeMastersByGroupID(int GroupId);
        List<UserTypeMaster> GetAllUserTypeMasters();
        void AddUserTypeMasters(UserTypeMaster UserTypeMaster);
        void DbSuccesfullySubmit();
    }
}
