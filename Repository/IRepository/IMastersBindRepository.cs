using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IMastersBindRepository
    {
        List<DepartmentMaster> GetDepartmentMasters(int storeid);
        List<DepartmentMaster> GetAllDepartmentMasters();
        List<TypicalBalanceMaster> GetTypicalbalance();
        List<VendorMaster> GetVendorMaster(int StoreId);

        List<CustomerMaster> GetCustomerMasters(int StoreId);
        List<StateMaster> GetStateMasters();

        List<AccountTypeMaster> GetAccountTypeMasters();

        List<StoreMaster> GetStoreMasters();
        List<DiscountTypeMaster> GetDiscountTypeMaster();
        List<InvoiceTypeMaster> GetInvoiceTypeMaster();
        List<PaymentTypeMaster> GetPaymentTypeMaster();

        string GetVendorName(int VendorId);
        string GetVendorRemoveSpecialCarecters(int VendorId);
        bool? IsHRAdmin(string UserName);
        bool? IsHRSuperAdmin(string UserName);
        List<EmployeeMaster> GetEmployeeMaster(int StoreId);
    }
}
