using EntityModels.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface ICompanyGroupsRepository
    {
        GroupMaster GetGroupMasterByID(int ID);
        void UpdateGroupMaster(GroupMaster groupmaster);
        IQueryable GetGroupMasterData();
        List<GroupMaster> GetGroupDataList();
        List<GroupMaster> GetGroupDataforCount(GroupMaster GroupMaster);
        void SaveGroupMaster(GroupMaster groupmaster);
        void DeleteGroupMaster(int Id);
        string GetGroupName(int Id);
    }
}
