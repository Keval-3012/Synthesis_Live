using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface ICompaniesRepository
    {
        StoreMaster GetStoreMastersbyID (int id);
        List<StoreMaster> GetAllStoreMasters(int? GroupId = null);
        List<GroupMaster> GetGroupMasters();
        List<StateMaster> GetStateMasters();
        void AddStoreMasters(StoreMaster storeMaster);
        void UpdatestoreMaster(StoreMaster storeMaster);
        void DeleteStoreMaster(int ID);
        string GetStoreName(int ID);
        List<StoreMaster> GetStoreMastersforCount(StoreMaster stores);
        //Updated by Dani on 07-08-2025
        bool InsertQuickBookStorewiseToken(int storeId, string clientId, string clientSecret);
    }
}
