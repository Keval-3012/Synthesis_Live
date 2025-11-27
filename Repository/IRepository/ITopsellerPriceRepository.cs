using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface ITopsellerPriceRepository
    {
        DataTable TopSellerPriceGrid(string[] value, string Upccode, string Departmentname, string ColorFilter);
        List<VendorNameList> ViewBagVendor();
        List<DepartmetItemList> ViewBagDepartment();
    }
}
