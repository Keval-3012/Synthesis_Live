using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IProductMappingLogRepository
    {
        List<InvoiceUserProductMapLogList> GetProductMappingLog();
        List<ItemProductScanLog> GetItemProductLog(int StoreId,string Startdate,string Enddate);
        ItemProductScanLog GetItemScannedUsersCount();
        List<BarcodeLookupDetails> GetBarcodeLookupData(string searchproduct,int skip,int take);
        List<DropdownViewModel> GetReportIssueDropDown();
        void InsertReportIssue(int ProductId,int? selectedIssueId,string AdditionNotes,int UserId,string UserName);
        List<BarcodeLookupDetails> GetProductCount();
        List<DropdownViewModel> GetUserLogUserList();
        List<CompetitorsUserLog> GetCompetitorLogList(int? UserId, DateTime searchdate);

    }
}
