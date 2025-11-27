using EntityModels.Models;
using SynthesisViewModal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IPayrollAccountRepository
    {
        List<PayrollAccount_Select> GetBindData(string SearchTitle,int StoreId);
        void UpdatePRAccount(UpdatePRAccount updatePRAccount);
        List<PayrollBankAccount> GetPayrollBankAccounts(int storeid);
        void UpdatePayrollBankAccounts(UpdateBankAccount_Setting updateBankAccount_Setting, int storeid);
    }
}
