using EntityModels.Models;
using NLog;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CompaniesRepository : ICompaniesRepository
    {
        private DBContext db;
        List<StoreMaster> StoreMasters = new List<StoreMaster>();
        StoreMaster StoreMaster = new StoreMaster();
        Logger logger = LogManager.GetCurrentClassLogger();
        public CompaniesRepository(DBContext context)
        {
            db = context;
        }
        public StoreMaster GetStoreMastersbyID(int id)
        {
            try
            {
                StoreMaster = db.StoreMasters.Find(Convert.ToInt32(id));
            }
            catch (Exception ex)
            {
                logger.Error("CompaniesRepository - GetStoreMastersbyID - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return StoreMaster;
        }
        public List<StoreMaster> GetAllStoreMasters(int? GroupId = null)
        {
            try
            {
                // Base query for stores
                var storeQuery = db.StoreMasters.AsQueryable();

                // Apply GroupId filter ONLY if provided and not 0
                if (GroupId.HasValue && GroupId.Value > 0)
                {
                    storeQuery = storeQuery.Where(s => s.GroupId == GroupId.Value);
                }
                // If GroupId is null or 0, don't apply filter - return all stores

                StoreMasters = storeQuery.ToList();

                logger.Info($"CompaniesRepository - GetAllStoreMasters - Retrieved {StoreMasters.Count} stores for GroupId: {GroupId ?? 0}");
            }
            catch (Exception ex)
            {
                logger.Error("CompaniesRepository - GetAllStoreMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return StoreMasters;
        }
        public List<GroupMaster> GetGroupMasters()
        {
            List<GroupMaster> groupMasters = new List<GroupMaster>();
            try
            {
                groupMasters = db.GroupMasters.Where(s => s.IsActive == true).OrderBy(o => o.Name).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("CompaniesRepository - GetGroupMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return groupMasters;
        }
        public List<StateMaster> GetStateMasters()
        {
            List<StateMaster> stateMasters = new List<StateMaster>();
            try
            {
                stateMasters = db.StateMasters.OrderBy(o => o.StateName).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("CompaniesRepository - GetStateMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return stateMasters;
        }
        public void AddStoreMasters(StoreMaster storeMaster)
        {
            try
            {
                db.StoreMasters.Add(storeMaster);
                db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.Error("CompaniesRepository - AddStoreMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public void UpdatestoreMaster(StoreMaster storeMaster)
        {
            try
            {
                db.Database.ExecuteSqlCommand("SP_StoreMaster @Mode = {0},@GroupId = {1},@Name = {2},@Address1 = {3},@Address2 ={4},@StoreNo = {5},@FaxNo = {6},@IsActive = {7},@EmailId = {8},@NickName = {9},@StateID = {10},@ModifiedBy = {11},@Longitude = {12},@Latitude = {13},@Radius = {14}, @StoreId = {15}", "UpdateStoreDetail", storeMaster.GroupId, storeMaster.Name, storeMaster.Address1, storeMaster.Address2, storeMaster.StoreNo, storeMaster.FaxNo, storeMaster.IsActive, storeMaster.EmailId, storeMaster.NickName, storeMaster.StateID, storeMaster.ModifiedBy, storeMaster.Longitude, storeMaster.Latitude, storeMaster.Radius, storeMaster.StoreId);
                db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.Error("CompaniesRepository - UpdatestoreMaster - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public void DeleteStoreMaster(int ID)
        {
            try
            {
                StoreMaster storeMaster = db.StoreMasters.Find(ID);
                db.StoreMasters.Remove(storeMaster);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("CompaniesRepository - DeleteStoreMaster - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public string GetStoreName(int ID)
        {
            string storeName = "";
            try
            {
                StoreMaster storeMaster = db.StoreMasters.Find(ID);
                storeName = storeMaster.Name;
            }
            catch (Exception ex)
            {
                logger.Error("CompaniesRepository - GetStoreName - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return storeName;
        }
        public List<StoreMaster> GetStoreMastersforCount(StoreMaster storeMaster)
        {
            try
            {
                StoreMasters = db.StoreMasters.Where(s => s.StoreId != storeMaster.StoreId && s.Name == storeMaster.Name).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("CompaniesRepository - GetStoreMastersforCount - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return StoreMasters;
        }
        //Updated by Dani on 07-08-2025        
        public bool InsertQuickBookStorewiseToken(int storeId, string clientId, string clientSecret)
        {
            try
            {
                var tokenEntry = new QuickBooksStorewiseToken
                {
                    StoreID = storeId,
                    ClientID = clientId,
                    ClientSecret = clientSecret,
                    CreatedDate = DateTime.Now
                };

                db.QuickBooksStorewiseToken.Add(tokenEntry);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("CompaniesRepository - InsertQuickBookStorewiseToken - " + DateTime.Now + " - " + ex.Message.ToString());
                return false;
            }
        }
    }
}