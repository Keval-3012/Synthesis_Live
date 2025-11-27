using EntityModels.Models;
using SynthesisViewModal.HRViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IUserMastersRepository
    {
        Task<UserMaster> Register(UserMaster UserMasters);
        Task<UserMaster> GetUserMasterDetails(int Id);
        List<UserMaster> GetBindData(int? GroupId = null);
        List<UserMaster> GetUserMasterData(int? StoreId);
        List<UserMaster> GetRightsStoresData();
        List<GroupMaster> GetGroupMasters();
        List<StoreMaster> GetStoreMasters(int? GroupId);
        List<UserTypeMaster> GetUserTypeMasters(UserMaster userMaster);
        List<UserTypeMaster> GetUserTypeMastersbyGroupId(int GroupId);
        List<StoreMaster> GetStoreMastersbyGroupId(int GroupId);
        List<UserRoles> GetuserRoles(int? StoreId);
        Task<UserMaster> GetFindUserMasters(int? id);
        bool CheckExist(UserMaster modelObj);
        bool CheckUserPasswordExist(UserMaster modelObj, int UserId);
        void SaveUserDetails(UserMaster UserMaster);
        void UpdateUserDetails(UserMaster UserMaster);
        void Resetpassword(User_Resetpassword PostedData);
        void UpdateUserPassword(UserMaster PostedData,int UserId);
        UserMaster DeleteUserDetails(int id);
         bool IsValidUser(Admin_Login model);
        bool IsFirstLogin(Admin_Login model);
        Login AddLogin(Login login);
        IpAdressInfo SP_IpAddressInfo(string myIP);
        UserMaster GetUserId(string UserId);
        UserIpInfo isuserAllow(int UserId,int IpAdressInfoID);
        UserTimeTrackInfo lastStatus(int UserId);
        List<UserRoles> ViewDashboardDailyDashboard(int UserTypeId);
        void SP_UserMaster(string Password,int UserId);
        string GetCurrentPassword(int UserId);
        void SP_UserTimeTrackInfo_Proc(int UserId, string ip, int Types);
        bool IsYourLoginStillTrue(string userId, string sid);
        UserTimeTrackInfo UserTimeTrac(string Ip, int UserId, string Location);
        UserTimeTrackInfo AddUserTimeTrackInfo(UserTimeTrackInfo userTimeTrackInfo);
        List<UserMaster> GetAllUserMasters();
        void UpdateUserManagerStatus(UserMaster UM);
        void UpdateUserManagerTrackStatus(UserMaster UM);
        void ResetPasswordUserManager(UserMaster UM);
        List<UserRoleStoreList> GetUserRoleStoreList(object UserId);
        List<UserRoleStoreList> GetUserRoleRightsList(int StoreId,int UserId);
        List<StoreMaster> GetStoreMastersLists();
        List<GroupWiseStateStore> GetGroupWiseStateStores();
        List<CompaniesCompetitorsList> GetCompaniesCompetitorsList();
        List<GroupMaster> GetAllGroups();
    }
}
