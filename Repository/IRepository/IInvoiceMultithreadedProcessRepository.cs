using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IInvoiceMultithreadedProcessRepository
    {
        void SaveKey(string apikey, int UserId);
        List<ApiKeyConfiguartion> GetKeyConfiguartions();
        ApiKeyConfiguartion GetFindApiKey(int id);
        void UpdateKey(int apikeyid, string apikey, int UserId);
        void DeleteKey(int? id, int UserId);
        BucketSizeMaster GetBucketSize();
        void UpdateBucketSize(int bucketid, int bucketsize);
    }
}
