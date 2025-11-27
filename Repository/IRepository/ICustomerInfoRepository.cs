using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public  interface ICustomerInfoRepository
    {
        CustomersReceiveablesManagement InsertInformation(CustomersReceiveablesManagement Cm, int StoreId);

        CustomersReceiveablesManagement UpdateInformation(CustomersReceiveablesManagement Cm);

        CustomersReceiveablesManagement DeleteInformation(int CustomerID);

        List<CustomersReceiveablesManagement> GetInformation(int? StoreId);

       
    }
}
