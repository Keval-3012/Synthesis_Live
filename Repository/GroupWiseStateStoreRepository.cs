using EntityModels.Models;
using NLog;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class GroupWiseStateStoreRepository : IGroupWiseStateStoreRepository
    {
        private DBContext _Context;
        Logger logger = LogManager.GetCurrentClassLogger();

        public GroupWiseStateStoreRepository(DBContext context)
        {
            _Context = context;
        }

        public GroupWiseStateStore InsertInformation(GroupWiseStateStore Gm)
        {
            try
            {
                _Context.Database.ExecuteSqlCommand("SP_GroupWiseStateStore @Mode={0},@GroupName={1},@StoreName={2},@CreatedBy={3}", "Insert", Gm.GroupName, Gm.StoreName, Gm.CreatedBy);
            }
            catch (Exception ex)
            {
                logger.Error("GroupWiseStateStoreRepository - InsertInformation - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Gm;
        }

        public GroupWiseStateStore UpdateInformation(GroupWiseStateStore Gm)
        {
            try
            {
                _Context.Database.ExecuteSqlCommand("SP_GroupWiseStateStore @Mode={0}, @GroupWiseStateStoreId={1}, @GroupName={2}, @StoreName={3}, @ModifiedBy={4}", "Update", Gm.GroupWiseStateStoreId, Gm.GroupName, Gm.StoreName, Gm.ModifiedBy);
            }
            catch (Exception ex)
            {
                logger.Error("GroupWiseStateStoreRepository - UpdateInformation - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Gm;
        }

        public GroupWiseStateStore DeleteInformation(int GroupWiseStateStoreId)
        {
            GroupWiseStateStore Gm = new GroupWiseStateStore();
            try
            {
                Gm = _Context.GroupWiseStateStores.Find(GroupWiseStateStoreId);
                _Context.GroupWiseStateStores.Remove(Gm);
                _Context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("GroupWiseStateStoreRepository - DeleteInformation - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Gm;
        }

        public List<GroupWiseStateStoreSelect> GetInformation()
        {
            List<GroupWiseStateStoreSelect> Gm = new List<GroupWiseStateStoreSelect>();
            try
            {
                Gm = _Context.Database.SqlQuery<GroupWiseStateStoreSelect>("SP_GroupWiseStateStore @Mode={0}", "GetAllData").ToList();
            }
            catch (Exception ex)
            {
                logger.Error("GroupWiseStateStoreRepository - GetInformation - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Gm;
        }

        public async Task<GroupWiseStateStore> GetFindGroupStateWiseStores(int? Id)
        {
            GroupWiseStateStore groupWiseStateStore = new GroupWiseStateStore();
            try
            {
                groupWiseStateStore = await _Context.GroupWiseStateStores.FindAsync(Id);
            }
            catch (Exception ex)
            {
                logger.Error("GroupWiseStateStoreRepository - GetFindGroupStateWiseStores - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return groupWiseStateStore;
        }

        //public List<UserMaster> GetUserMasters()
        //{
        //    List<UserMaster> UserMaster = new List<UserMaster>();
        //    try
        //    {
        //        UserMaster = _Context.UserMasters.Where(s => s.IsActive == true).OrderBy(s => s.FirstName).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("GroupWiseStateStoreRepository - GetUserMasters - " + DateTime.Now + " - " + ex.Message.ToString());
        //    }
        //    return UserMaster;
        //}
    }
}
