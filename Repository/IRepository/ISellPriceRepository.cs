using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface ISellPriceRepository
    {
        List<VendorNameList> GetsalePriceList();
        List<DepartmetItemList> ViewbagDepartment();
        DataTable SellPriceGrid(string[] value, string Upccode, string Departmentname, string ColorFilter);

    }
}
