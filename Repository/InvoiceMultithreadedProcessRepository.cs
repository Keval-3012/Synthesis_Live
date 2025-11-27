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
    public class InvoiceMultithreadedProcessRepository : IInvoiceMultithreadedProcessRepository
    {
        private DBContext _context;
        Logger logger = LogManager.GetCurrentClassLogger();

        public InvoiceMultithreadedProcessRepository(DBContext context)
        {
            _context = context;
        }
        public void SaveKey(string apikey, int UserId)
        {
            try
            {              
                _context.Database.ExecuteSqlCommand("SP_APIKeyConfiguration @Mode={0}, @APIKeyName={1}, @CreatedBy={2}",  "Insert" , apikey, UserId);
            } 
            catch (Exception ex)
            {
                logger.Error("InvoiceMultithreadedProcessRepository - SaveKey - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void UpdateKey(int apikeyid ,string apikey, int UserId)
        {
            try
            {
                _context.Database.ExecuteSqlCommand("SP_APIKeyConfiguration @Mode={0},@APIKeyName={1},@APIKeyId={2},@ModifiedBy={3}", "Update", apikey, apikeyid, UserId);            
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceMultithreadedProcessRepository - UpdateKey - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void DeleteKey(int? id, int UserId)
        {
            try
            {
                _context.Database.ExecuteSqlCommand("SP_APIKeyConfiguration @Mode={0},@APIKeyId={1},@ModifiedBy={2}", "Delete", id, UserId);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceMultithreadedProcessRepository - DeleteKey - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public List<ApiKeyConfiguartion> GetKeyConfiguartions()
        {
            List<ApiKeyConfiguartion> ApiKeyConfiguartions = new List<ApiKeyConfiguartion>();
            try
            {
                ApiKeyConfiguartions = _context.Database.SqlQuery<ApiKeyConfiguartion>("SP_APIKeyConfiguration @Mode={0}", "GetAllAPIKey").ToList();
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceMultithreadedProcessRepository - GetKeyConfiguartions - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return ApiKeyConfiguartions;
        }

        public ApiKeyConfiguartion GetFindApiKey(int Id)
        {
            ApiKeyConfiguartion ApiKeyConfiguartions = new ApiKeyConfiguartion();
            try
            {
                ApiKeyConfiguartions = _context.ApiKeyConfiguartions.Find(Id);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceMultithreadedProcessRepository - GetFindApiKey - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return ApiKeyConfiguartions;
        }

        public BucketSizeMaster GetBucketSize()
        {
            BucketSizeMaster bucketSize = new BucketSizeMaster();
            try
            {
                bucketSize = _context.BucketSizeMasters.Count() > 0 ? _context.BucketSizeMasters.FirstOrDefault() :  null;
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceMultithreadedProcessRepository - GetBucketSize - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return bucketSize;
        }

        public void UpdateBucketSize(int bucketid, int bucketsize)
        {
            try
            {
                _context.Database.ExecuteSqlCommand("SP_APIKeyConfiguration @Mode={0},@BucketSize={1},@BucketId={2}", "UpdateBucketSize", bucketsize,bucketid );
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceMultithreadedProcessRepository - UpdateKey - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
    }
}
