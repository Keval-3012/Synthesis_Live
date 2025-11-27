using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IBankAccountsRepository
    {
        List<BankAccountSettingDetail> GetBankDetail(int StoreID);
        void SaveBankDetails(BankAccountSettingModel BankAccDetails);
        BankAccountSettingModel GetBankDetailsByID(int? ID);
        StoreMaster GetStoreMastersdata(BankAccountSettingModel bankAccountSettingModel);
        void DeleteBankDetails(BankAccountSettingModel bankdetails);
    }
}
