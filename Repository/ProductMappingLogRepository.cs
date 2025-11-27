using EntityModels.Models;
using NLog;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class ProductMappingLogRepository : IProductMappingLogRepository
    {
        private DBContext _context;
        Logger logger = LogManager.GetCurrentClassLogger();


        public ProductMappingLogRepository(DBContext context)
        {
            _context = context;
        }

        public List<InvoiceUserProductMapLogList> GetProductMappingLog()
        {
            List<InvoiceUserProductMapLogList> lstInvoiceUserProductMapLog = new List<InvoiceUserProductMapLogList>();
            try
            {
                lstInvoiceUserProductMapLog =  _context.Database.SqlQuery<InvoiceUserProductMapLogList>("SP_GetProductMappingLog @Mode = {0}", "GetProductMappingLog").ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingLogRepository - GetProductMappingLog - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lstInvoiceUserProductMapLog;
        }

        //item product log
        public List<ItemProductScanLog> GetItemProductLog(int StoreId, string Startdate, string Enddate)
        {
            List<ItemProductScanLog> lstInvoiceUserProductMapLog = new List<ItemProductScanLog>();
            try
            {
                lstInvoiceUserProductMapLog = _context.Database.SqlQuery<ItemProductScanLog>("SP_GetProductMappingLog @Mode = {0},@StoreId = {1},@StartDate = {2},@EndDate = {3}", "GetItemScannedLog", StoreId, Startdate, Enddate).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingLogRepository - GetProductMappingLog - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lstInvoiceUserProductMapLog;
        }

        public ItemProductScanLog GetItemScannedUsersCount()
        {
            ItemProductScanLog obj = new ItemProductScanLog();
            try
            {
                obj = _context.Database.SqlQuery<ItemProductScanLog>("SP_GetProductMappingLog @Mode = {0}", "GetItemScannedUsersCount").FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingLogRepository - GetItemScannedUsersCount - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<BarcodeLookupDetails> GetBarcodeLookupData(string searchproduct, int skip, int take)
        {
            List<BarcodeLookupDetails> obj = new List<BarcodeLookupDetails>();
            try
            {
                obj = _context.Database.SqlQuery<BarcodeLookupDetails>("SP_GetItemListBasedonItemCode @Mode = {0},@Search = {1},@Skip = {2},@Take = {3}", "GetBarcodeLookupData", searchproduct, skip, take).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingLogRepository - GetBarcodeLookupData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<DropdownViewModel> GetReportIssueDropDown()
        {
            List<DropdownViewModel> obj = new List<DropdownViewModel>();
            try
            {
                obj = _context.Database.SqlQuery<DropdownViewModel>("SP_GetItemListBasedonItemCode @Mode = {0}", "GetReportIssueDropdownValue").ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingLogRepository - GetReportIssueDropDown - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public void InsertReportIssue(int ProductId, int? selectedIssueId, string AdditionNotes, int UserId, string UserName)
        {
            try
            {
                _context.Database.ExecuteSqlCommand("SP_GetItemListBasedonItemCode @Mode={0},@ProductId={1},@ReportedOptionID={2},@AdditionalNotes={3},@UserID={4},@UserName={5}", "InsertReportIssue", ProductId, selectedIssueId, AdditionNotes, UserId, UserName);
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingLogRepository - InsertReportIssue - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public List<BarcodeLookupDetails> GetProductCount()
        {
            List<BarcodeLookupDetails> obj = new List<BarcodeLookupDetails>();
            try
            {
                obj = _context.Database.SqlQuery<BarcodeLookupDetails>("SP_GetItemListBasedonItemCode @Mode = {0}", "GetProductCount").ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingLogRepository - GetProductCount - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<DropdownViewModel> GetUserLogUserList()
        {
            List<DropdownViewModel> obj = new List<DropdownViewModel>();
            try
            {
                obj = _context.Database.SqlQuery<DropdownViewModel>("SP_GetCompetitorsScanLog @Mode = {0}", "GetUserLogUserList").ToList();
                obj = obj.OrderBy(x => x.Value == 0 ? 0 : 1).ThenBy(x => x.Text).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingLogRepository - GetUserLogUserList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<CompetitorsUserLog> GetCompetitorLogList(int? UserId, DateTime searchdate)
        {
            List<CompetitorsUserLog> obj = new List<CompetitorsUserLog>();
            try
            {
                obj = _context.Database.SqlQuery<CompetitorsUserLog>("SP_GetCompetitorsScanLog @Mode = {0},@UserId = {1},@SearchDate = {2}", "GetCompetitorLogList", UserId, searchdate).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingLogRepository - GetCompetitorLogList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }
    }
}
