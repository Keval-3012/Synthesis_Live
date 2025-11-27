using EntityModels.Models;
using NLog;
using Repository.IRepository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CompanyGroupsRepository : ICompanyGroupsRepository
    {
        private DBContext _context;
        Logger logger = LogManager.GetCurrentClassLogger();
        public CompanyGroupsRepository(DBContext context)
        {
            _context = context;
        }

        public void DeleteGroupMaster(int Id)
        {
            try
            {
                GroupMaster groups = _context.GroupMasters.Find(Id);
                _context.GroupMasters.Remove(groups);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("CompanyGroupsRepository - DeleteGroupMaster - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
       
        public List<GroupMaster> GetGroupDataList()
        {
            List<GroupMaster> GroupDatalist = new List<GroupMaster>();
            try
            {
                GroupDatalist =  _context.GroupMasters.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("CompanyGroupsRepository - GetGroupDataList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return GroupDatalist;
        }
       
        public List<GroupMaster> GetGroupDataforCount(GroupMaster groups)
        {
            List<GroupMaster> GroupDatalist = new List<GroupMaster>();
            try
            {
                GroupDatalist = _context.GroupMasters.Where(s => s.GroupId != groups.GroupId && s.Name == groups.Name).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("CompanyGroupsRepository - GetGroupDataforCount - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return GroupDatalist;
        }

        public GroupMaster GetGroupMasterByID(int ID)
        {
            GroupMaster groupmaster = new GroupMaster();
            try
            {
                groupmaster = _context.GroupMasters.Find(ID);
            }
            catch (Exception ex)
            {
                logger.Error("CompanyGroupsRepository - GetGroupMasterByID - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return groupmaster;
        }

        public IQueryable GetGroupMasterData()
        {
            IQueryable GroupMasterData = null;
            try
            {
                GroupMasterData = _context.GroupMasters.AsQueryable(); 
            }
            catch (Exception ex)
            {
                logger.Error("CompanyGroupsRepository - GetGroupMasterData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return GroupMasterData;
        }

        public string GetGroupName(int Id)
        {
            string GroupName = "";
            try
            {
                GroupMaster Groupmaster = _context.GroupMasters.Find(Id);
                GroupName = Groupmaster.Name;
            }
            catch (Exception ex)
            {
                logger.Error("CompanyGroupsRepository - GetGroupName - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return GroupName;
        }

        public void SaveGroupMaster(GroupMaster groupmaster)
        {
            try
            {
                _context.GroupMasters.Add(groupmaster);
                _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.Error("CompanyGroupsRepository - SaveGroupMaster - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void UpdateGroupMaster(GroupMaster groupmaster)
        {
            try
            {
                _context.Entry(groupmaster).State = EntityState.Modified;
                _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.Error("CompanyGroupsRepository - UpdateGroupMaster - " + DateTime.Now + " - " + ex.Message.ToString());
            }

        }
    }
}
