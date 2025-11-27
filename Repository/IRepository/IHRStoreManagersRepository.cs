using EntityModels.HRModels;
using EntityModels.Models;
using SynthesisViewModal.HRViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IHRStoreManagersRepository
    {
        List<HRStoreList> GetHRStoreManagerList();
        void UpdateStoreManagerStatus(HRStoreManagerViewModel SM);
        List<int> GetStorebyId(int StoreId);
        List<int> GetUserbyId(int? StoreManageId);
        List<UserMaster> GetAllUserMasters(int? StoreManageId);
        List<StoreMaster> GetStoreMasters();
        List<UserMaster> GetAllUserMastersForAdd();
        HRStoreManagers InsertHRStoreManager(HRStoreManagers hRStoreManagers);
        HRStoreManagers UpdateHRStoreManager(HRStoreManagers hRStoreManagers);
        HRStoreManagers GetHRStoreManagersByID(int StoreManagerId);
    }
}
