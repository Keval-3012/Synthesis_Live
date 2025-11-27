using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IProductPriceRepository
    {
        List<VendorNameList> GetsalePriceList();
        DataTable getProductPriceList(string Upccode, string FromDate, string ToDate);
        List<ProductPriceModel> getlinechart(string Upccode, string startDate, string endDate, string storeId);
        List<VendorPiChartModel> VendorgetPiechart(string Upccode);
        List<StorePieChartModel> StoregetPiechart(string Upccode);
        List<LineItemsLookUp> LineItemsLookup(string Upccode, int StoreIds, string InvoiceNumber);
        List<InvoiceLookup> InvoiceLookup(string Upccode);
        List<StoreWiseLookup> StoreWiseLookup(string Upccode);
        List<VendorNameList> ViewBagVendor();
        List<DepartmentMaster> ViewBagDepartment();
        DataTable ProductPriceGrid(string[] value, string Upccode, string vendorname, string ColorFilter, int Radio);
        void Unlink_ItemLines(LineItemList value);
    }
}
