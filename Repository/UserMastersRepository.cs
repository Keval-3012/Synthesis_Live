using EntityModels.Models;
using NLog;
using Repository.IRepository;
using SynthesisViewModal.HRViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Repository
{
    public class UserMastersRepository : IUserMastersRepository
    {
        private DBContext _context;
        protected static bool IsArray;
        Logger logger = LogManager.GetCurrentClassLogger();
        public UserMastersRepository(DBContext context)
        {
            _context = context;
        }
        public async Task<UserMaster> Register(UserMaster UserMaster)
        {
            try
            {
                UserMaster.IsActive = true;
                _context.UserMasters.Add(UserMaster);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - Register - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return UserMaster;
        }
        public async Task<UserMaster> GetUserMasterDetails(int Id)
        {
            UserMaster UserMaster = new UserMaster();
            try
            {
                IsArray = true;
                UserMaster = _context.UserMasters.Find(Id);
                if (UserMaster.CreatedOn != null)
                {
                    UserMaster.CreatedOn = Convert.ToDateTime(Common.GetEasternTime(UserMaster.CreatedOn.Value));
                }
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - GetUserMasterDetails - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return UserMaster;
        }
        public async Task<UserMaster> GetFindUserMasters(int? Id)
        {
            UserMaster UserMaster = new UserMaster();
            try
            {
                UserMaster = await _context.UserMasters.FindAsync(Id);
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - GetFindUserMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return UserMaster;
        }
        public List<UserMaster> GetBindData(int? GroupId = null)
        {
            List<UserMaster> UserMasterList = new List<UserMaster>();
            List<UserRoles> UserRolesList = new List<UserRoles>();
            try
            {
                var UserId = UserModule.getUserId();
                var UserWiseRightsData = _context.UserWiseRights.Where(x => x.UserId == UserId).ToList();

                if (UserWiseRightsData != null && UserWiseRightsData.Count > 0)
                {
                    UserRolesList = UserWiseRightsData.Select(x => new UserRoles
                    {
                        GroupId = x.GroupId,
                        ModuleId = x.ModuleId,
                        Role = x.Role,
                        StoreId = x.StoreId,
                        UserRoleId = x.UserWiseRightsId,
                        UserTypeId = x.UserTypeId,
                        UserForms = x.UserForms,
                        RightsStoreLists = x.RightsStoreLists,
                        UserType = x.UserType,
                    }).ToList();
                }
                else
                {
                    UserRolesList = _context.userRoles.ToList();
                }

                // Base query for users
                var userQuery = _context.UserMasters.AsQueryable();

                // Apply GroupId filter if provided (and not 0/"All Groups")
                if (GroupId.HasValue && GroupId.Value != 0)
                {
                    userQuery = userQuery.Where(s => s.GroupId == GroupId.Value);
                }

                // Fetch user data with filter applied
                UserMasterList = userQuery
                    .Select(s => new
                    {
                        UserId = s.UserId,
                        FirstName = s.FirstName,
                        UserName = s.UserName,
                        Group = s.GroupMasters.Name,
                        GroupId = s.GroupId,
                        UserTypeId = s.UserTypeId,
                        UserType = s.UserTypeMasters.UserType,
                        EmailAddress = s.EmailAddress,
                        PhoneNumber = s.PhoneNumber,
                        ForWhatsAppNotification = s.ForWhatsAppNotification,
                        IsActive = s.IsActive,
                        TrackHours = s.TrackHours,
                        IsOnline = s.IsOnline
                    })
                    .ToList()
                    .Select(k => new UserMaster
                    {
                        UserId = k.UserId,
                        FirstName = k.FirstName,
                        UserName = k.UserName,
                        GroupId = k.GroupId,
                        Group = k.Group,
                        UserTypeId = k.UserTypeId,
                        UserType = k.UserType,
                        EmailAddress = k.EmailAddress,
                        PhoneNumber = k.PhoneNumber,
                        ForWhatsAppNotification = k.ForWhatsAppNotification,
                        IsActive = k.IsActive,
                        TrackHours = k.TrackHours,
                        IsOnline = k.IsOnline,
                        StoreName = UserRolesList
                            .Where(s => s.UserTypeId == k.UserTypeId)
                            .Select(s => s.StoreMasters != null ? s.StoreMasters.Name : "Unknown")
                            .Distinct()
                            .ToArray()
                    })
                    .OrderByDescending(u => u.IsActive)
                    .ThenBy(u => u.FirstName)
                    .ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - GetBindData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return UserMasterList;
        }
        public List<UserMaster> GetUserMasterData(int? StoreId)
        {
            List<UserMaster> UserMaster = new List<UserMaster>();
            List<UserRoles> UserRoles = new List<UserRoles>();
            try
            {
                UserRoles = _context.userRoles.Where(s => s.StoreId == StoreId).ToList();
                var UserTypeList = UserRoles.Select(s => s.UserTypeId).ToList();

                UserMaster = _context.UserMasters.Where(s => s.UserTypeMasters.UserType != "Administrator" && UserTypeList.Contains(s.UserTypeId))
                 .Select(s => new
                 {
                     UserId = s.UserId,
                     FirstName = s.FirstName,
                     UserName = s.UserName,
                     Group = s.GroupMasters.Name,
                     UserTypeId = s.UserTypeId,
                     UserType = s.UserTypeMasters.UserType,
                 }).ToList().Select(k => new UserMaster
                 {
                     UserId = k.UserId,
                     FirstName = k.FirstName,
                     UserName = k.UserName,
                     Group = k.Group,
                     UserTypeId = k.UserTypeId,
                     UserType = k.UserType,
                     StoreName = UserRoles.Where(s => s.UserTypeId == k.UserTypeId).Select(s => s.StoreMasters.Name).Distinct().ToArray()
                 }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - GetUserMasterData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return UserMaster;
        }
        public List<UserMaster> GetRightsStoresData()
        {
            List<UserMaster> UserMaster = new List<UserMaster>();
            try
            {
                var StoreList = _context.RightsStores.ToList();

                UserMaster = _context.UserMasters.Where(s => s.UserTypeMasters.UserType != "Administrator")
                .Select(s => new
                {
                    UserId = s.UserId,
                    FirstName = s.FirstName,
                    UserName = s.UserName,
                    Group = s.GroupMasters.Name,
                    UserTypeId = s.UserTypeId,
                    UserType = s.UserTypeMasters.UserType,
                }).ToList().Select(k => new UserMaster
                {
                    UserId = k.UserId,
                    FirstName = k.FirstName,
                    UserName = k.UserName,
                    Group = k.Group,
                    UserTypeId = k.UserTypeId,
                    UserType = k.UserType,
                    StoreName = StoreList.Where(s => s.UserTypeId == k.UserTypeId).Select(s => s.StoreMasters.Name).Distinct().ToArray()
                }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - GetRightsStoresData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return UserMaster;
        }
        public List<GroupMaster> GetGroupMasters()
        {
            List<GroupMaster> GroupMaster = new List<GroupMaster>();
            try
            {
                GroupMaster = _context.GroupMasters.Where(s => s.IsActive == true).OrderBy(o => o.Name).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - GetGroupMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return GroupMaster;
        }
        public List<StoreMaster> GetStoreMastersLists()
        {
            List<StoreMaster> StoreMaster = new List<StoreMaster>();
            try
            {
                var excludedStoreNames = new List<string> { "SMG", "Test" };
                StoreMaster = _context.StoreMasters
                    .Where(s => s.IsActive == true && !excludedStoreNames.Contains(s.NickName))
                    .OrderBy(o => o.NickName)
                    .ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - GetStoreMastersLists - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return StoreMaster;
        }
        public List<StoreMaster> GetStoreMasters(int? GroupId)
        {
            List<StoreMaster> StoreMaster = new List<StoreMaster>();
            try
            {
                StoreMaster = _context.StoreMasters.Where(s => s.IsActive == true && s.GroupId == GroupId).OrderBy(o => o.Name).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - GetStoreMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return StoreMaster;
        }
        public List<UserTypeMaster> GetUserTypeMasters(UserMaster userMaster)
        {
            List<UserTypeMaster> UserTypeMaster = new List<UserTypeMaster>();
            try
            {
                UserTypeMaster = _context.UserTypeMasters.Where(s => s.IsActive == true && s.GroupId == userMaster.GroupId).OrderBy(o => o.UserType).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - GetUserTypeMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return UserTypeMaster;
        }
        public List<UserTypeMaster> GetUserTypeMastersbyGroupId(int GroupId)
        {
            List<UserTypeMaster> UserTypeMaster = new List<UserTypeMaster>();
            try
            {
                UserTypeMaster = _context.UserTypeMasters.Where(s => s.GroupId == GroupId && s.IsActive == true).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - GetUserTypeMastersbyGroupId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return UserTypeMaster;
        }
        public List<StoreMaster> GetStoreMastersbyGroupId(int GroupId)
        {
            List<StoreMaster> StoreMaster = new List<StoreMaster>();
            try
            {
                StoreMaster = _context.StoreMasters.Where(s => s.GroupId == GroupId && s.IsActive == true).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - GetStoreMastersbyGroupId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return StoreMaster;
        }
        public List<UserRoles> GetuserRoles(int? StoreId)
        {
            List<UserRoles> UserRoles = new List<UserRoles>();
            try
            {
                UserRoles = _context.userRoles.Where(s => s.StoreId == StoreId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - GetuserRoles - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return UserRoles;
        }
        public bool CheckExist(UserMaster modelObj)
        {
            try
            {
                return _context.UserMasters.Any(s => s.UserId != modelObj.UserId && s.UserName == modelObj.UserName);
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - CheckExist - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return false;
        }
        public bool CheckUserPasswordExist(UserMaster PostedData, int UserId)
        {
            try
            {
                return _context.UserMasters.Any(s => s.UserId == UserId && s.Password == PostedData.OldPassword);
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - CheckUserPasswordExist - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return false;
        }
        public void SaveUserDetails(UserMaster UserMaster)
        {
            try
            {
                string PhoneNumber = null;

                if (!String.IsNullOrEmpty(UserMaster.PhoneNumber))
                {
                    PhoneNumber = Convert.ToInt64(UserMaster.PhoneNumber).ToString("(###) ###-####");
                }
                UserMaster.PhoneNumber = PhoneNumber;
                _context.UserMasters.Add(UserMaster);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - SaveUserDetails - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public void UpdateUserDetails(UserMaster userMaster)
        {
            try
            {
                string PhoneNumber = null;
                if (!String.IsNullOrEmpty(userMaster.PhoneNumber))
                {
                    PhoneNumber = Convert.ToInt64(userMaster.PhoneNumber).ToString("(###) ###-####");
                }
                userMaster.PhoneNumber = PhoneNumber;
                _context.Database.ExecuteSqlCommand("SP_UserMaster @Mode = {0},@FirstName = {1},@UserName = {2}, @GroupId = {3}, @UserTypeId = {4}, @ModifiedBy = {5}, @IsActive = {6}, @UserId = {7},@TrackHours = {8},@EmailAddress={9},@IsHRAdmin={10},@IsHRSuperAdmin={11},@PhoneNumber={12},@ForWhatsAppNotification={13},@IsProductScanApp={14},@UserRightsforStoreAccess={15},@StoreAccess={16},@DataStoreAccess={17},@GroupWiseStateStoreId={18},@ProductImageUpload={19},@UpdateProductDetails={20},@IsAbleExpiryChange={21},@DesignatedStore={22},@LanguageId={23},@DesignatedPageId={24},@IsCustomCompetitors={25},@CompetitorsId={26}", "UpdateUserDetail", userMaster.FirstName, userMaster.UserName, userMaster.GroupId, userMaster.UserTypeId, userMaster.ModifiedBy, userMaster.IsActive, userMaster.UserId, userMaster.TrackHours, userMaster.EmailAddress, userMaster.IsHRAdmin, userMaster.IsHRSuperAdmin, userMaster.PhoneNumber, userMaster.ForWhatsAppNotification, userMaster.IsProductScanApp, userMaster.UserRightsforStoreAccess, userMaster.StoreAccess, userMaster.DataStoreAccess, userMaster.GroupWiseStateStoreId, userMaster.ProductImageUpload, userMaster.UpdateProductDetails, userMaster.IsAbleExpiryChange, userMaster.DesignatedStore, userMaster.LanguageId, userMaster.DesignatedPageId, userMaster.IsCustomCompetitors, userMaster.CompetitorsId);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - UpdateUserDetails - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public void UpdateUserPassword(UserMaster PostedData, int UserId)
        {
            try
            {
                _context.Database.ExecuteSqlCommand("SP_UserMaster @Mode = {0},@Password = {1},@UserId = {2},@OldPassword = {3}", "UpdateOldPassword", PostedData.Password, UserId, PostedData.OldPassword);
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - UpdateUserPassword - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public void Resetpassword(User_Resetpassword PostedData)
        {
            try
            {
                _context.Database.ExecuteSqlCommand("SP_UserMaster @Mode = {0},@Password = {1},@UserId = {2}", "UpdateUserPassword", PostedData.Password, PostedData.Reg_userid);
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - Resetpassword - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public UserMaster DeleteUserDetails(int id)
        {
            UserMaster userMaster = new UserMaster();
            try
            {
                userMaster = _context.UserMasters.Find(id);
                _context.UserMasters.Remove(userMaster);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - DeleteUserDetails - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return userMaster;
        }
        public bool IsValidUser(Admin_Login admin_Login)
        {
            return _context.UserMasters.Any(s => s.UserName.ToLower() == admin_Login.UserName && s.Password == admin_Login.Password.ToLower() && s.IsActive == true);
        }
        public bool IsFirstLogin(Admin_Login model)
        {
            return _context.UserMasters.Any(s => s.UserName.ToLower() == model.UserName && s.Password == model.Password.ToLower() && s.IsActive == true && s.IsFirstLogin == false);
        }
        public Login AddLogin(Login login)
        {
            try
            {
                _context.Logins.Add(login);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - AddLogin - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return login;
        }
        public IpAdressInfo SP_IpAddressInfo(string myIP)
        {
            IpAdressInfo isIpContain = null;
            try
            {
                isIpContain = _context.Database.SqlQuery<IpAdressInfo>("SP_IpAddressInfo @Mode = {0},@IpAddress = {1}", "SelectCheckIP", myIP).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - SP_IpAddressInfo - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return isIpContain;
        }
        public UserMaster GetUserId(string UserId)
        {
            return _context.UserMasters.Where(s => s.UserName == UserId).FirstOrDefault();
        }
        public UserIpInfo isuserAllow(int UserId, int IpAdressInfoID)
        {
            return _context.UserIpInfos.Where(s => s.UserID == UserId && s.IpAdressInfoID == IpAdressInfoID).FirstOrDefault();
        }
        public UserTimeTrackInfo lastStatus(int UserId)
        {
            return _context.UserTimeTrackInfos.Where(a => a.UserId == UserId).ToArray().OrderByDescending(a => a.StartTime).FirstOrDefault();
        }
        public List<UserRoles> ViewDashboardDailyDashboard(int UserTypeId)
        {
            return _context.userRoles.Where(s => s.UserTypeId == UserTypeId && s.Role == "ViewDashboardDailyDashboard").ToList();
        }
        public void SP_UserMaster(string Password, int UserId)
        {
            try
            {
                _context.Database.ExecuteSqlCommand("SP_UserMaster @Mode = {0},@Password = {1},@UserId = {2}", "UpdateUserPasswordFirstLogin", Password, UserId);
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - SP_UserMaster - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public string GetCurrentPassword(int UserId)
        {
            return _context.UserMasters.Where(u => u.UserId == UserId).Select(u => u.Password).FirstOrDefault();
        }
        public void SP_UserTimeTrackInfo_Proc(int UserId, string ip, int Types)
        {
            try
            {
                _context.Database.ExecuteSqlCommand("SP_UserTimeTrackInfo_Proc @UserID={0}, @StartTime={1},@IpAddress={2},@Type={3},@CreatedBy={4}", UserId, DateTime.Now, ip, Types, UserId);
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - SP_UserTimeTrackInfo_Proc - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public bool IsYourLoginStillTrue(string userId, string sid)
        {
            IEnumerable<Login> logins = (from i in _context.Logins
                                         where i.LoggedIn == true && i.UserId == userId && i.SessionId == sid
                                         select i).AsEnumerable();
            return logins.Any();
        }
        public UserTimeTrackInfo UserTimeTrac(string IP, int UserId, string Location)
        {
            return _context.UserTimeTrackInfos.Where(a => a.IpAddress == IP && a.UserId == UserId && a.Location == Location).ToArray().LastOrDefault();
        }
        public UserTimeTrackInfo AddUserTimeTrackInfo(UserTimeTrackInfo userTimeTrackInfo)
        {
            try
            {
                _context.UserTimeTrackInfos.Add(userTimeTrackInfo);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - AddUserTimeTrackInfo - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return userTimeTrackInfo;
        }
        public List<UserMaster> GetAllUserMasters()
        {
            List<UserMaster> UserMasters = new List<UserMaster>();
            try
            {
                UserMasters = _context.UserMasters.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - AddUserTimeTrackInfo - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return UserMasters;
        }
        public void UpdateUserManagerStatus(UserMaster UM)
        {
            try
            {
                logger.Info("UserMastersRepository - UpdateUserManagerStatus - " + DateTime.Now);
                _context.Database.ExecuteSqlCommand("SP_UserMaster @Mode = {0},@UserId = {1},@IsActive = {2},@ModifiedBy = {3},@ModifiedOn = {4}", "UpdateUserManagerStatus", UM.UserId, UM.IsActive, UM.ModifiedBy, UM.ModifiedOn);
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - UpdateUserManagerStatus - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public void UpdateUserManagerTrackStatus(UserMaster UM)
        {
            try
            {
                logger.Info("UserMastersRepository - UpdateUserManagerTrackStatus - " + DateTime.Now);
                _context.Database.ExecuteSqlCommand("SP_UserMaster @Mode = {0},@UserId = {1},@TrackHours = {2},@ModifiedBy = {3},@ModifiedOn = {4}", "UpdateUserManagerTrackStatus", UM.UserId, UM.TrackHours, UM.ModifiedBy, UM.ModifiedOn);
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - UpdateUserManagerTrackStatus - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public void ResetPasswordUserManager(UserMaster UM)
        {
            try
            {
                logger.Info("UserMastersRepository - ResetPasswordUserManager - " + DateTime.Now);
                _context.Database.ExecuteSqlCommand("SP_UserMaster @Mode = {0},@UserId = {1},@Password = {2},@ModifiedBy = {3},@ModifiedOn = {4}", "UpdateUserPassword", UM.UserId, UM.Password, UM.ModifiedBy, UM.ModifiedOn);
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - ResetPasswordUserManager - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public List<UserRoleStoreList> GetUserRoleStoreList(object UserId)
        {
            List<UserRoleStoreList> storelist = new List<UserRoleStoreList>();
            try
            {
                storelist = _context.Database.SqlQuery<UserRoleStoreList>("SP_UserMaster @Mode = {0}, @UserId = {1}", "GetUserRoleStoreName", UserId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - GetUserRoleStoreList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return storelist;
        }
        public List<UserRoleStoreList> GetUserRoleRightsList(int StoreId, int UserId)
        {
            List<UserRoleStoreList> storelist = new List<UserRoleStoreList>();
            try
            {
                storelist = _context.Database.SqlQuery<UserRoleStoreList>("SP_UserMaster @Mode = {0}, @StoreId = {1}, @UserId = {2}", "GetUserRoleRightsList", StoreId, UserId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - GetUserRoleRightsList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return storelist;
        }
        public List<GroupWiseStateStore> GetGroupWiseStateStores()
        {
            List<GroupWiseStateStore> GroupWiseStateStore = new List<GroupWiseStateStore>();
            try
            {
                GroupWiseStateStore = _context.GroupWiseStateStores.OrderBy(o => o.GroupName).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - GetGroupWiseStateStores - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return GroupWiseStateStore;
        }
        public List<CompaniesCompetitorsList> GetCompaniesCompetitorsList()
        {
            List<CompaniesCompetitorsList> Competitors = new List<CompaniesCompetitorsList>();
            try
            {
                Competitors = _context.Database.SqlQuery<CompaniesCompetitorsList>("SP_UserMaster @Mode = {0}", "GetAllCompetitorsList").ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - GetFormattedCompaniesCompetitorsList - " + DateTime.Now + " - " + ex.Message);
            }
            return Competitors;
        }

        //Added by Dani on 06-11-2025 for Group calling all.
        public List<GroupMaster> GetAllGroups()
        {
            List<GroupMaster> groupList = new List<GroupMaster>();
            try
            {
                groupList = _context.GroupMasters.Where(g => g.IsActive == true).OrderBy(g => g.Name).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserMastersRepository - GetAllGroups - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return groupList;
        }
    }
}
