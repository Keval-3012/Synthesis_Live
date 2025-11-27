using EntityModels.Migrations;
using EntityModels.Models;
using NLog;
using Repository.IRepository;
using Syncfusion.EJ2.Base;
using SynthesisQBOnline;
using SynthesisQBOnline.BAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using VendorDepartmentRelationMaster = EntityModels.Models.VendorDepartmentRelationMaster;
using VendorMaster = EntityModels.Models.VendorMaster;

namespace Repository
{
    public class VendorMasterRepository : IVendorMasterRepository
    {
        private DBContext db;
        UserActivityLogRepository activityLogRepository = null;
        CommonRepository commonRepository = null;
        Logger logger = LogManager.GetCurrentClassLogger();

        public VendorMasterRepository(DBContext context)
        {
            db = context;
            activityLogRepository = new UserActivityLogRepository(context);
            commonRepository = new CommonRepository(context);
        }

        public VendorMaster getVendor(int VendorId)
        {
            VendorMaster vendorMaster = new VendorMaster();
            try
            {
                vendorMaster = db.VendorMasters.Where(s => s.VendorId == VendorId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("VendorMasterRepository - GetVendor - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return vendorMaster;
        }

        public List<VenodrMasterSelect> GetVendorMastersDataSP(int? StoreIds)
        {
            List<VenodrMasterSelect> vendorMasters = new List<VenodrMasterSelect>();
            try
            {
                vendorMasters = db.Database.SqlQuery<VenodrMasterSelect>("SP_VendorMaster @Mode = {0}, @StoreId = {1}", "SelectVendor", StoreIds).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("VendorMasterRepository - GetVendorMastersDataSP - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return vendorMasters;
        }

        public List<int> GetVendorDepartmentIds(int VendorId, int StoreId)
        {
            List<int> VendorDepartments = new List<int>();
            try
            {
                VendorDepartments = db.VendorDepartmentRelationMasters.Where(s => s.VendorId == VendorId && s.StoreId == StoreId).Select(s => s.DepartmentId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("VendorMasterRepository - GetVendorDepartmentIds - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return VendorDepartments;
        }

        public string InsertVendor(CRUDModel<VendorMaster> VendorMaster)
        {
            string SuccessOrError = "";
            try
            {
                string PhoneNumber = null;

                if (!String.IsNullOrEmpty(VendorMaster.Value.PhoneNumber))
                {
                    PhoneNumber = Convert.ToInt64(VendorMaster.Value.PhoneNumber).ToString("(###) ###-####");
                }
                VendorMaster.Value.PhoneNumber = PhoneNumber;
                if (VendorMaster.Value.MultiDepartmentId != null)
                {
                    var Data = VendorMaster.Value.MultiDepartmentId.Select(s => s).ToList();
                    var DepartmentName = db.DepartmentMasters.Where(s => Data.Contains(s.DepartmentId.ToString())).Select(s => s.DepartmentName).ToList();
                    VendorMaster.Value.Departments = String.Join(", ", DepartmentName);
                }
                List<VendorDepartmentRelationMaster> lstVD = new List<VendorDepartmentRelationMaster>();
                try
                {
                    VendorMaster vendorMaster = new VendorMaster();
                    if (VendorMaster.Value.State == "Select State")
                    {
                        VendorMaster.Value.State = null;
                    }
                    vendorMaster = VendorMaster.Value;
                    vendorMaster.State = vendorMaster.State == "Select State" ? null : vendorMaster.State;

                    foreach (var item in VendorMaster.Value.MultiStoreId)
                    {
                        lstVD = new List<VendorDepartmentRelationMaster>();
                        int iVendorId = 0;
                        vendorMaster.StoreId = Convert.ToInt32(item);
                        int StoreId = Convert.ToInt32(vendorMaster.StoreId);
                        if (StoreId > 0)
                        {
                            string VendorName = vendorMaster.VendorName?.Trim();
                            var objVendorExist = db.VendorMasters.ToList().Where(p => p.VendorName == VendorName && p.StoreId == vendorMaster.StoreId).FirstOrDefault();
                            if (objVendorExist == null)
                            {
                                db.Database.ExecuteSqlCommand("SPVendor_Web @Spara = {0},@StoreId = {1},@VendorId ={2},@VendorName={3},@Address={4},@PhoneNumber={5},@CompanyName={6},@PrintOnCheck={7},@IsActive={8},@City={9},@State={10},@Country={11},@PostalCode={12},@EMail={13},@Instruction={14},@AccountNumber={15},@ModifiedBy={16},@CreatedBy={17},@IsSync={18}", "Insert",
                         vendorMaster.StoreId, 0, VendorName, vendorMaster.Address, vendorMaster.PhoneNumber, vendorMaster.CompanyName, vendorMaster.PrintOnCheck, vendorMaster.IsActive, vendorMaster.City, vendorMaster.State, vendorMaster.Country, vendorMaster.PostalCode, vendorMaster.EMail, vendorMaster.Instruction, vendorMaster.AccountNumber, null, UserModule.getUserId(), false);
                                SuccessOrError = "success";
                            }
                            else
                            {
                                SuccessOrError = "error";
                                goto Exit;
                            }
                        }
                        else
                        {
                            SuccessOrError = "success";
                        }
                        int UserId = UserModule.getUserId();
                        if (vendorMaster.MultiDepartmentId != null)
                        {
                            var DepartmentList = db.DepartmentMasters.ToList().Where(s => s.StoreId == Convert.ToInt32(item) && vendorMaster.MultiDepartmentId.Contains(s.DepartmentId.ToString())).Select(m => m.DepartmentId).ToList();
                            if (DepartmentList.Count() > 0)
                            {
                                foreach (var item1 in DepartmentList)
                                {
                                    VendorDepartmentRelationMaster objVD = new VendorDepartmentRelationMaster();
                                    objVD.VendorId = iVendorId;
                                    objVD.StoreId = StoreId;
                                    objVD.DepartmentId = item1;
                                    objVD.CreatedBy = UserId;
                                    objVD.CreatedOn = DateTime.Now;
                                    lstVD.Add(objVD);
                                }

                            }
                            if (lstVD.Count() > 0)
                            {
                                foreach (var itemqw in lstVD)
                                {
                                    db.VendorDepartmentRelationMasters.Add(itemqw);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    AdminSiteConfiguration.WriteErrorLogs("Controller : VendorMaster Method : VendorCreate Message:" + ex.Message + "Internal Message:" + ex.InnerException);
                }
            }
            catch (Exception ex)
            {
                logger.Error("VendorMasterRepository - InsertVendor - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        Exit:;
            return SuccessOrError;
        }

        public string UpdateVendor(CRUDModel<VendorMaster> VendorMaster)
        {
            string PhoneNumber = null;
            string SuccessOrError = "";
            try
            {
                VendorMaster vendorMaster = new VendorMaster();
                if (VendorMaster.Value.State == "Select State")
                {
                    VendorMaster.Value.State = null;
                }
                vendorMaster = VendorMaster.Value;
                vendorMaster.State = vendorMaster.State == "Select State" ? null : vendorMaster.State;

                if (!String.IsNullOrEmpty(VendorMaster.Value.PhoneNumber))
                {
                    PhoneNumber = Convert.ToInt64(VendorMaster.Value.PhoneNumber).ToString("(###) ###-####");
                }
                VendorMaster.Value.PhoneNumber = PhoneNumber;
                if (VendorMaster.Value.MultiDepartmentId != null)
                {
                    var Data = VendorMaster.Value.MultiDepartmentId.Select(s => s).ToList();
                    var DepartmentName = db.DepartmentMasters.Where(s => Data.Contains(s.DepartmentId.ToString())).Select(s => s.DepartmentName).ToList();
                    VendorMaster.Value.Departments = String.Join(", ", DepartmentName);
                }
                int StoreId = Convert.ToInt32(vendorMaster.StoreId);
                vendorMaster.IsSync = false;
                //if (vendorMaster.IsActive == true)
                //{
                //    vendorMaster.IsSync = true;
                //}
                //else
                //{
                //    vendorMaster.IsSync = false;
                //}
                string VendorName = vendorMaster.VendorName?.Trim();
                var objVendorExist = db.VendorMasters.ToList().Where(p => p.VendorId != vendorMaster.VendorId && p.VendorName == VendorName && p.StoreId == vendorMaster.StoreId).FirstOrDefault();
                if (objVendorExist == null)
                {
                    db.Database.ExecuteSqlCommand("SPVendor_Web @Spara = {0},@StoreId = {1},@VendorId ={2},@VendorName={3},@Address={4},@PhoneNumber={5},@CompanyName={6},@PrintOnCheck={7},@IsActive={8},@City={9},@State={10},@Country={11},@PostalCode={12},@EMail={13},@Instruction={14},@AccountNumber={15},@IsSync={16},@ModifiedBy={17},@CreatedBy={18}", "Update",
                        vendorMaster.StoreId, vendorMaster.VendorId, VendorName, vendorMaster.Address, vendorMaster.PhoneNumber, vendorMaster.CompanyName, vendorMaster.PrintOnCheck, vendorMaster.IsActive, vendorMaster.City, vendorMaster.State, vendorMaster.Country, vendorMaster.PostalCode, vendorMaster.EMail, vendorMaster.Instruction, vendorMaster.AccountNumber, vendorMaster.IsSync, UserModule.getUserId(), UserModule.getUserId());
                    SuccessOrError = "success";
                }
                else
                {
                    SuccessOrError = "error";
                }
                int UserId = UserModule.getUserId();
                db.VendorDepartmentRelationMasters.RemoveRange(db.VendorDepartmentRelationMasters.Where(s => s.StoreId == StoreId && s.VendorId == vendorMaster.VendorId));
                List<VendorDepartmentRelationMaster> lstVD = new List<VendorDepartmentRelationMaster>();
                if (vendorMaster.MultiDepartmentId != null)
                {
                    var DepartmentList = db.DepartmentMasters.ToList().Where(s => s.StoreId == StoreId && vendorMaster.MultiDepartmentId.Contains(s.DepartmentId.ToString())).Select(m => m.DepartmentId).ToList();
                    if (DepartmentList.Count() > 0)
                    {
                        foreach (var item1 in DepartmentList)
                        {
                            VendorDepartmentRelationMaster objVD = new VendorDepartmentRelationMaster();
                            objVD.VendorId = vendorMaster.VendorId;
                            objVD.StoreId = StoreId;
                            objVD.DepartmentId = item1;
                            objVD.CreatedBy = UserId;
                            objVD.CreatedOn = DateTime.Now;
                            lstVD.Add(objVD);
                        }

                    }

                    if (lstVD.Count() > 0)
                    {
                        foreach (var item in lstVD)
                        {
                            db.VendorDepartmentRelationMasters.Add(item);
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    db.SaveChanges();
                }
                VendorMaster.Value.StoreName = db.StoreMasters.Where(w => w.StoreId == VendorMaster.Value.StoreId).FirstOrDefault().NickName;
            }
            catch (Exception ex)
            {
                logger.Error("VendorMasterRepository - UpdateVendor - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return SuccessOrError;
        }

        public VendorMaster checkVendorExist(VendorMaster vendorMaster)
        {
            VendorMaster vendorMasterObj = new VendorMaster();
            try
            {
                vendorMasterObj = db.Database.SqlQuery<VendorMaster>("SP_VendorMaster @Mode = {0},@StoreId = {1},@VendorName = {2},@VendorId ={3}", "SelectForUpdate", vendorMaster.StoreId, vendorMaster.VendorName, vendorMaster.VendorId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("VendorMasterRepository - checkVendorExist - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return vendorMasterObj;
        }

        public VendorMaster getVendorByStoreId_LisId(int StoreId, string ListId)
        {
            VendorMaster vendorMasterObj = new VendorMaster();
            try
            {
                vendorMasterObj = db.Database.SqlQuery<VendorMaster>("SP_VendorMaster @Mode = {0},@StoreId = {1},@ListId = {2}", "SelectByStoreID_ListID", StoreId, ListId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("VendorMasterRepository - getVendorByStoreId_LisId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return vendorMasterObj;
        }

        public int SaveVendorMaster(VendorMaster vendorMaster)
        {
            int iVendorId = 0;
            try
            {
                VendorMaster objVendor = new VendorMaster();
                objVendor.ListId = vendorMaster.ListId;
                objVendor.VendorName = vendorMaster.VendorName == null ? null : vendorMaster.VendorName;
                objVendor.PhoneNumber = vendorMaster.PhoneNumber == null ? null : vendorMaster.PhoneNumber;
                objVendor.CompanyName = vendorMaster.CompanyName == null ? null : vendorMaster.CompanyName;
                objVendor.PrintOnCheck = vendorMaster.PrintOnCheck == null ? null : vendorMaster.PrintOnCheck;
                objVendor.IsActive = Convert.ToBoolean(vendorMaster.IsActive);
                objVendor.Address = vendorMaster.Address == null ? null : vendorMaster.Address;
                objVendor.City = vendorMaster.City == null ? null : vendorMaster.City;
                objVendor.State = vendorMaster.State == null ? null : vendorMaster.State;
                objVendor.Country = vendorMaster.Country == null ? null : vendorMaster.Country;
                objVendor.PostalCode = vendorMaster.PostalCode == null ? null : vendorMaster.PostalCode;
                objVendor.EMail = vendorMaster.EMail == null ? null : vendorMaster.EMail;
                objVendor.Instruction = vendorMaster.Instruction == null ? null : vendorMaster.Instruction;
                objVendor.AccountNumber = vendorMaster.AccountNumber == null ? null : vendorMaster.AccountNumber;
                objVendor.CreatedBy = vendorMaster.CreatedBy;
                objVendor.CreatedOn = DateTime.Now;
                objVendor.StoreId = vendorMaster.StoreId;
                objVendor.LastModifiedOn = DateTime.Now;
                objVendor.IsSync = true;
                objVendor.SyncDate = DateTime.Now;
                db.VendorMasters.Add(objVendor);
                db.SaveChanges();
                objVendor = null;
            }
            catch (Exception ex)
            {
                logger.Error("VendorMasterRepository - SaveVendorMaster - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return iVendorId;
        }

        public int SaveCopyVendorMaster(VendorMaster vendorMaster, int StoreId, string ListID)
        {
            int iVendorId = 0;
            int StoreID = Convert.ToInt32(StoreId);
            try
            {
                VendorMaster objVendor = new VendorMaster();
                objVendor.ListId = ListID;
                objVendor.VendorName = vendorMaster.VendorName == null ? null : vendorMaster.VendorName;
                objVendor.PhoneNumber = vendorMaster.PhoneNumber == null ? null : vendorMaster.PhoneNumber;
                objVendor.CompanyName = vendorMaster.CompanyName == null ? null : vendorMaster.CompanyName;
                objVendor.PrintOnCheck = vendorMaster.PrintOnCheck == null ? null : vendorMaster.PrintOnCheck;
                objVendor.IsActive = Convert.ToBoolean(vendorMaster.IsActive);
                objVendor.Address = vendorMaster.Address == null ? null : vendorMaster.Address;
                objVendor.City = vendorMaster.City == null ? null : vendorMaster.City;
                objVendor.State = vendorMaster.State == null ? null : vendorMaster.State;
                objVendor.Country = vendorMaster.Country == null ? null : vendorMaster.Country;
                objVendor.PostalCode = vendorMaster.PostalCode == null ? null : vendorMaster.PostalCode;
                objVendor.EMail = vendorMaster.EMail == null ? null : vendorMaster.EMail;
                objVendor.Instruction = vendorMaster.Instruction == null ? null : vendorMaster.Instruction;
                objVendor.AccountNumber = vendorMaster.AccountNumber == null ? null : vendorMaster.AccountNumber;
                objVendor.CreatedBy = vendorMaster.CreatedBy;
                objVendor.CreatedOn = DateTime.Now;
                objVendor.StoreId = StoreID;
                objVendor.LastModifiedOn = DateTime.Now;
                objVendor.IsSync = true;
                objVendor.SyncDate = DateTime.Now;
                db.VendorMasters.Add(objVendor);
                db.SaveChanges();
                objVendor = null;
            }
            catch (Exception ex)
            {
                logger.Error("VendorMasterRepository - SaveVendorMaster - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return iVendorId;
        }

        public int UpdateVendorMaster(VendorMaster vendorMaster)
        {
            int iVendorId = 0;
            try
            {
                var objVendor = db.VendorMasters.Where(a => a.VendorId == vendorMaster.VendorId && a.StoreId == vendorMaster.StoreId).FirstOrDefault();
                objVendor.ListId = vendorMaster.ListId;
                objVendor.VendorName = vendorMaster.VendorName == null ? null : vendorMaster.VendorName;
                objVendor.PhoneNumber = vendorMaster.PhoneNumber == null ? null : vendorMaster.PhoneNumber;
                objVendor.CompanyName = vendorMaster.CompanyName == null ? null : vendorMaster.CompanyName;
                objVendor.PrintOnCheck = vendorMaster.PrintOnCheck == null ? null : vendorMaster.PrintOnCheck;
                objVendor.IsActive = Convert.ToBoolean(vendorMaster.IsActive);
                objVendor.Address = vendorMaster.Address == null ? null : vendorMaster.Address;
                objVendor.City = vendorMaster.City == null ? null : vendorMaster.City;
                objVendor.State = vendorMaster.State == null ? null : vendorMaster.State;
                objVendor.Country = vendorMaster.Country == null ? null : vendorMaster.Country;
                objVendor.PostalCode = vendorMaster.PostalCode == null ? null : vendorMaster.PostalCode;
                objVendor.EMail = vendorMaster.EMail == null ? null : vendorMaster.EMail;
                objVendor.Instruction = vendorMaster.Instruction == null ? null : vendorMaster.Instruction;
                objVendor.AccountNumber = vendorMaster.AccountNumber == null ? null : vendorMaster.AccountNumber;
                objVendor.ModifiedBy = vendorMaster.ModifiedBy;
                objVendor.ModifiedOn = DateTime.Now;
                objVendor.LastModifiedOn = DateTime.Now;
                objVendor.StoreId = vendorMaster.StoreId;
                objVendor.IsSync = true;
                db.Entry(objVendor).State = EntityState.Modified;
                db.SaveChanges();
                iVendorId = objVendor.VendorId;
                objVendor = null;
            }
            catch (Exception ex)
            {
                logger.Error("VendorMasterRepository - UpdateVendorMaster - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return iVendorId;
        }

        public void VendorDepartmentSave(int StoreId, VendorMaster vendorMaster)
        {
            try
            {
                db.VendorDepartmentRelationMasters.RemoveRange(db.VendorDepartmentRelationMasters.Where(s => s.StoreId == StoreId && s.VendorId == vendorMaster.VendorId));
                List<VendorDepartmentRelationMaster> lstVD = new List<VendorDepartmentRelationMaster>();
                if (vendorMaster.MultiDepartmentId != null)
                {
                    var DepartmentList = db.DepartmentMasters.ToList().Where(s => s.StoreId == StoreId && vendorMaster.MultiDepartmentId.Contains(s.DepartmentId.ToString())).Select(m => m.DepartmentId).ToList();
                    if (DepartmentList.Count() > 0)
                    {
                        foreach (var item1 in DepartmentList)
                        {
                            VendorDepartmentRelationMaster objVD = new VendorDepartmentRelationMaster();
                            objVD.VendorId = vendorMaster.VendorId;
                            objVD.StoreId = StoreId;
                            objVD.DepartmentId = item1;
                            objVD.CreatedBy = vendorMaster.CreatedBy;
                            objVD.CreatedOn = DateTime.Now;
                            lstVD.Add(objVD);
                        }

                    }

                    if (lstVD.Count() > 0)
                    {
                        foreach (var item in lstVD)
                        {
                            db.VendorDepartmentRelationMasters.Add(item);
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logger.Error("VendorMasterRepository - VendorDepartmentSave - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void DeleteVendor(int VendorId, string UserName)
        {
            try
            {
                VendorMaster vendorMaster = db.VendorMasters.Find(VendorId);                
                db.VendorMasters.Remove(vendorMaster);
                db.SaveChanges();

                ActivityLog ActLog = new ActivityLog();
                ActLog.Action = 3;
                ActLog.Comment = "Vendor " + vendorMaster.VendorName + " Deleted by " + commonRepository.getUserFirstName(UserName) + " on " + AdminSiteConfiguration.GetDate(AdminSiteConfiguration.GetEasternTime(DateTime.Now).ToString());
                activityLogRepository.ActivityLogInsert(ActLog);
            }
            catch (Exception ex)
            {
                logger.Error("VendorMasterRepository - DeleteVendor - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public List<VenodrMasterCopy> GetStorevalueforVendor(string VendorName)
        {
            List<VenodrMasterCopy> list = new List<VenodrMasterCopy>();
            try
            {
                list = db.Database.SqlQuery<VenodrMasterCopy>("[SP_VendorMaster] @Mode={0},@vendorName={1}", "SelectStorelist", VendorName.Trim()).ToList(); //.Replace("&amp;","&").Replace("&apos;","'")
            }
            catch (Exception ex)
            { 
                logger.Error("VendorMasterRepository - GetStorevalueforVendor - " + DateTime.Now + " - " + ex.Message.ToString()); 
            }
            return list;
        }

        public void VendorDepartmentRelationMasterSave(int VendorId, int VendorCopyId, int StoreId, int UserId)
        {
            try
            {
                var DepartmentList = db.VendorDepartmentRelationMasters.Where(s => s.VendorId == VendorId).Select(m => m.DepartmentId).ToList();
                if (DepartmentList.Count() > 0)
                {
                    foreach (var item1 in DepartmentList)
                    {
                        VendorDepartmentRelationMaster objVD = new VendorDepartmentRelationMaster();
                        objVD.VendorId = VendorCopyId;
                        objVD.StoreId = StoreId;
                        objVD.DepartmentId = item1;
                        objVD.CreatedBy = UserId;
                        objVD.CreatedOn = DateTime.Now;
                        db.VendorDepartmentRelationMasters.Add(objVD);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex) 
            { 
                logger.Error("VendorMasterRepository - VendorDepartmentRelationMasterSave - " + DateTime.Now + " - " + ex.Message.ToString()); 
            }
            
        }

        public List<VendorMaster> GetVendormaster(int VendorId)
        {
            List<VendorMaster> vendorMasters = new List<VendorMaster>();
            try
            {
                vendorMasters = db.VendorMasters.Where(x => x.VendorId == VendorId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("VendorMasterRepository - GetVendormaster - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return vendorMasters;
        }

        public List<VendorMaster> GetVendormasterById()
        {
            List<VendorMaster> obj = new List<VendorMaster>();
            try
            {
                obj = db.VendorMasters.ToList();
            }
            catch (Exception ex) 
            { 
                logger.Error("VendorMasterRepository - GetVendormasterById - " + DateTime.Now + " - " + ex.Message.ToString()); 
            }
            return obj;
        }

        public void UpdateVendorStatus(int VendorId, bool IsActive)
        {
            try
            {
                db.Database.ExecuteSqlCommand("SP_VendorMaster @Mode = {0},@VendorId = {1},@IsActive = {2}", "UpdateVendorStatus", VendorId, IsActive);
            }
            catch (Exception ex)
            {
                logger.Error("VendorMasterRepository - UpdateVendorStatus - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public List<GetMergeVendor> GetMergeVendor(string Mode, int StoreId)
        {
            List<GetMergeVendor> obj = new List<GetMergeVendor>();
            try
            {
                obj = db.Database.SqlQuery<GetMergeVendor>("SP_VendorMaster @Mode={0},@StoreId={1}", Mode, StoreId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("VendorMasterRepository - GetMergeVendor - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<MergeVendorInvoiceList> GetMergeVendorInvoiceList(string Mode, string VendorId, int StoreId)
        {
            List<MergeVendorInvoiceList> obj = new List<MergeVendorInvoiceList>();
            try
            {
                obj = db.Database.SqlQuery<MergeVendorInvoiceList>("SP_Invoice @Mode={0},@VendorId={1},@StoreId={2}", "GetAllInvoiceVendorId", VendorId, 1).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("VendorMasterRepository - MergeVendorInvoiceList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public void MergeInvoiceVendor(string Mode, string MasterVendorId, string VendorId, int StoreId) 
        {
            try
            {
                db.Database.ExecuteSqlCommand("SP_Invoice @Mode={0},@MasterVendorId={1},@VendorId={2},@StoreId={3}", Mode, MasterVendorId, VendorId, StoreId);
            }
            catch (Exception ex)
            {
                logger.Error("VendorMasterRepository - MergeInvoiceVendor - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void UpdateUploadInvoice(string Mode, string InvoiceId, string UploadInvoice)
        {
            try
            {
                db.Database.ExecuteSqlCommand("SP_Invoice @Mode={0},@InvoiceId={1},@UploadInvoice={2}", Mode, InvoiceId, UploadInvoice);
                //db.Database.ExecuteSqlCommand("SP_Invoice @Mode={0},@MasterVendorId={1},@VendorId={2},@StoreId={3}", Mode, MasterVendorId, VendorId, StoreId);
            }
            catch (Exception ex)
            {
                logger.Error("VendorMasterRepository - UpdateUploadInvoice - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
    }
}
