using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IGroupWiseStateStoreRepository
    {
        GroupWiseStateStore InsertInformation(GroupWiseStateStore Gm);

        GroupWiseStateStore UpdateInformation(GroupWiseStateStore Gm);

        GroupWiseStateStore DeleteInformation(int GroupWiseStateStoreId);

        List<GroupWiseStateStoreSelect> GetInformation();

        Task<GroupWiseStateStore> GetFindGroupStateWiseStores(int? id);

        //List<UserMaster> GetUserMasters();        
    }
}
