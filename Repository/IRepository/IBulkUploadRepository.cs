using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IBulkUploadRepository
    {
        void SaveFileChanges(UploadPdf uploadPDF);
        void RemoveFileChanges(UploadPdf uploadPDF);
        List<UploadFile> StoreProcess(string Search, int StoreId);
        List<UploadFile> WithoutStoreProcess(string Search);
        List<UploadFile> WithStoreProcess(int StoreId);
        List<UploadFile> WithStoreProcessing();
        void UploadFileChanges(int? id);
        List<UploadFile> GetUploadFiles(int StoreID, int userid, List<StoreMaster> StoreList, List<UserMaster> UserMaster);
    }
}
