using EntityModels.HRModels;
using EntityModels.Models;
using Microsoft.Win32.SafeHandles;
using NLog;
using Repository.IRepository;
using SynthesisViewModal.HRViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class HRStoreManagersRepository :IHRStoreManagersRepository
    {
        private DBContext _context;
        Logger logger = LogManager.GetCurrentClassLogger();
        public HRStoreManagersRepository(DBContext context)
        {
            _context = context;
        }
        public List<HRStoreList> GetHRStoreManagerList()
        {
            List<HRStoreList> obj = new List<HRStoreList>();
            try
            {
                logger.Info("HRStoreManagersRepository - GetHRStoreManagerList - " + DateTime.Now);
                obj = _context.Database.SqlQuery<HRStoreList>("SP_GetAllHRStoreManager @Mode = {0}", "Select").ToList();
            }
            catch (Exception ex)
            {
                logger.Error("HRStoreManagersRepository - GetHRStoreManagerList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }
        public HRStoreManagers InsertHRStoreManager(HRStoreManagers hRStoreManagers)
        {
            try
            {
                logger.Info("HRStoreManagersRepository - UpdateHRStoreManager - " + DateTime.Now);
                _context.Database.ExecuteSqlCommand("SP_GetAllHRStoreManager @Mode={0},@StoreId = {1},@FirstName={2},@UserName={3},@Password={4},@ConfirmPassword={5},@IsActive = {6},@CreatedOn = {7},@CreatedBy = {8}", "InsertHRStoreManager", hRStoreManagers.StoreId,hRStoreManagers.FirstName, hRStoreManagers.UserName,hRStoreManagers.Password, hRStoreManagers.ConfirmPassword, hRStoreManagers.IsActive,hRStoreManagers.CreatedOn, hRStoreManagers.CreatedBy);
            }
            catch (Exception ex)
            {
                logger.Error("HRStoreManagersRepository - InsertHRStoreManager - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return hRStoreManagers;
        }
        public HRStoreManagers UpdateHRStoreManager(HRStoreManagers hRStoreManagers)
        {
            try
            {
                logger.Info("HRStoreManagersRepository - UpdateHRStoreManager - " + DateTime.Now);
                _context.Database.ExecuteSqlCommand("SP_GetAllHRStoreManager @Mode={0},@StoreId = {1},@FirstName={2},@UserName={3},@Password={4},@ConfirmPassword = {5},@ModifiedOn = {6},@ModifiedBy = {7},@StoreManagerId = {8}", "UpdateHRStoreManager",  hRStoreManagers.StoreId, hRStoreManagers.FirstName, hRStoreManagers.UserName, hRStoreManagers.Password, hRStoreManagers.ConfirmPassword, hRStoreManagers.ModifiedOn, hRStoreManagers.ModifiedBy, hRStoreManagers.StoreManagerId);
            }
            catch (Exception ex)
            {
                logger.Error("HRStoreManagersRepository - UpdateHRStoreManager - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return hRStoreManagers;
        }
        public HRStoreManagers GetHRStoreManagersByID(int StoreManagerId)
        {
            HRStoreManagers hR = new HRStoreManagers();
            try
            {
                logger.Info("HRStoreManagersRepository - GetHRStoreManagersByID - " + DateTime.Now);
                hR = _context.HRStoreManagers.Find(StoreManagerId);
            }
            catch (Exception ex)
            {
                logger.Error("HRStoreManagersRepository - GetHRStoreManagersByID - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return hR;
        }
        public void UpdateStoreManagerStatus(HRStoreManagerViewModel SM)
        {
            try
            {
                logger.Info("HRStoreManagersRepository - UpdateStoreManagerStatus - " + DateTime.Now);
                _context.Database.ExecuteSqlCommand("SP_GetAllHRStoreManager @Mode = {0},@StoreId = {1},@IsActive = {2},@ModifiedBy = {3},@ModifiedOn = {4}", "UpdateStoreManagerStatus", SM.StoreId, SM.IsActive, SM.ModifiedBy, SM.ModifiedOn);
            }
            catch (Exception ex)
            {
                logger.Error("HRStoreManagersRepository - UpdateStoreManagerStatus - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public List<int> GetStorebyId(int StoreId)
        {
            List<int> store = new List<int>();
            try
            {
                logger.Info("HRStoreManagersRepository - GetStorebyId - " + DateTime.Now);
                store = _context.StoreMasters.Where(s => s.StoreId == StoreId).Select(s => s.StoreId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("HRStoreManagersRepository - GetStorebyId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return store;
        }
        public List<int> GetUserbyId(int? StoreManageId)
        {
            List<int> store = new List<int>();
            try
            {
                logger.Info("HRStoreManagersRepository - GetUserbyId - " + DateTime.Now);
                store = _context.UserMasters.Where(s => s.UserId == StoreManageId).Select(s => s.UserId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("HRStoreManagersRepository - GetUserbyId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return store;
        }
        public List<UserMaster> GetAllUserMasters(int? StoreManageId)
        {
            List<UserMaster> UserMasters = new List<UserMaster>();
            try
            {
                logger.Info("HRStoreManagersRepository - GetAllUserMasters - " + DateTime.Now);
                UserMasters = _context.UserMasters.Where(u => u.IsActive == true && (!_context.StoreMasters.Any(s => s.StoreManageId == u.UserId) || u.UserId == StoreManageId)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("HRStoreManagersRepository - GetAllUserMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return UserMasters;
        }
        public List<UserMaster> GetAllUserMastersForAdd()
        {
            List<UserMaster> UserMasters = new List<UserMaster>();
            try
            {
                logger.Info("HRStoreManagersRepository - GetAllUserMastersForAdd - " + DateTime.Now);
                UserMasters = _context.UserMasters.Where(u => !_context.StoreMasters.Any(s => s.StoreManageId == u.UserId) && u.IsActive == true).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("HRStoreManagersRepository - GetAllUserMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return UserMasters;
        }
        public List<StoreMaster> GetStoreMasters()
        {
            List<StoreMaster> storeMasters = new List<StoreMaster>();
            try
            {
                logger.Info("HRStoreManagersRepository - GetStoreMasters - " + DateTime.Now);
                storeMasters = _context.StoreMasters.Where(s => s.IsActive ==  true).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("HRStoreManagersRepository - GetStoreMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return storeMasters;
        }
    }
}
