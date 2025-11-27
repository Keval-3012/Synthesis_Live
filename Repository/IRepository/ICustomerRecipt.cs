using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public  interface ICustomerRecipt
    {
        CustomersReceiveablesReceipts InsertRecipt(CustomersReceiveablesReceipts _Cm);
        List<CustomersReceiveablesReceiptsListModel> GetCustomerRecieptList(int? StoreId);
        CustomersReceiveablesReceipts getcustomerreciptdata(int customerreceiptid);
        CustomersReceiveablesManagement getcustomerdata(int customerreceiptid);
        void UpdateIsEmailTriggered(int customerreceiptid);
        void DeleteCustomerReceipt(int? id);
        void UpdateCompanyNameCustomerReceipt(int CustomersReceiveablesReceiptsId, int CompanyNameId);

    }
}
