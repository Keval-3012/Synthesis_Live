using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IGroupAccountMasterRepository
    {
        List<ConfigurationGroup> GetConfigurationGroup(int storeid);
        ConfigurationGroup GetConfigurationGroupDataById(int Id);
        void DeleteConfigGroupById(int Id);
        string SaveGroupMaster(string Name, string QBAccountid, string Typicalbalid, string memo, string EntityVal, int storeid);
        string UpdateGroupMaster(int ID, string Name, string QBAccountid, string Typicalbalid, string memo, string EntityVal, int storeid);

        List<OtherDepositeSetting> OtherDepositeAccount(int StoreId);

        void SyncVendors(List<SynthesisQBOnline.BAL.VendorMaster> dtVendor,int StoreId,int UserId);

        void SyncDepartment(List<SynthesisQBOnline.BAL.CustomerMaster> dtCustomer, int StoreId, int UserId);

        string UpdateOtherDeposite_Setting(int ID, int QBAccountid,int StoreId);

        List<ddllist> GetCustomerVendor(string QBId, int StoreId);

    }
}
