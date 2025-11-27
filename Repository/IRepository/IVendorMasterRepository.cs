using EntityModels.Models;
using Syncfusion.EJ2.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IVendorMasterRepository
    {
        List<VenodrMasterSelect> GetVendorMastersDataSP(int? StoreIds);

        VendorMaster getVendor(int VendorId);

        List<int> GetVendorDepartmentIds(int VendorId, int StoreId);

        string InsertVendor(CRUDModel<VendorMaster> VendorMaster);

        string UpdateVendor(CRUDModel<VendorMaster> VendorMaster);

        VendorMaster checkVendorExist(VendorMaster vendorMaster);

        VendorMaster getVendorByStoreId_LisId(int StoreId,string ListId);

        int SaveVendorMaster(VendorMaster vendorMaster);

        int SaveCopyVendorMaster(VendorMaster vendorMaster, int StoreId, string ListID);

        int UpdateVendorMaster(VendorMaster vendorMaster);

        void VendorDepartmentSave(int StoreId, VendorMaster vendorMaster);

        void DeleteVendor(int VendorId, string UserName);

        List<VenodrMasterCopy> GetStorevalueforVendor(string VendorName);

        void VendorDepartmentRelationMasterSave(int VendorId, int VendorCopyId, int StoreId, int UserId);

        List<VendorMaster> GetVendormaster(int VendorId);

        List<VendorMaster> GetVendormasterById();

        void UpdateVendorStatus(int VendorId, bool IsActive);

        List<GetMergeVendor> GetMergeVendor(string Mode,int StoreId);

        List<MergeVendorInvoiceList> GetMergeVendorInvoiceList(string Mode,string VendorId,int StoreId);

        void MergeInvoiceVendor(string Mode, string MasterVendorId, string VendorId, int StoreId);

        void UpdateUploadInvoice(string Mode,string InvoiceId,string UploadInvoice);
    }
}
