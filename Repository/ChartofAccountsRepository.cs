using EntityModels.Models;
using NLog;
using Repository.IRepository;
using SynthesisViewModal;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Repository
{
    public class ChartofAccountsRepository : IChartofAccountsRepository
    {
        private DBContext _context;
        Logger logger = LogManager.GetCurrentClassLogger();

        public ChartofAccountsRepository(DBContext context)
        {
            _context = context;
        }

        public List<StoreMaster> StoreMasterList()
        {
            List<StoreMaster> obj = new List<StoreMaster>();
            try
            {
                obj = _context.StoreMasters.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsRepository - StoreMasterList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<AccountDetailTypeMaster> AccountDetailTypeList()
        {
            List<AccountDetailTypeMaster> obj = new List<AccountDetailTypeMaster>();
            try
            {
                obj = _context.AccountDetailTypeMasters.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsRepository - AccountDetailTypeList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<AccountTypeMaster> AccountTypeList()
        {
            List<AccountTypeMaster> obj = new List<AccountTypeMaster>();
            try
            {
                obj = _context.AccountTypeMasters.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsRepository - AccountTypeList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<DepartmentMaster> DepartmentMasterList()
        {
            List<DepartmentMaster> obj = new List<DepartmentMaster>();
            try
            {
                obj = _context.DepartmentMasters.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsRepository - DepartmentMasterList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<DepartmentMaster> SelectByStoreID_Name(int StoreId, string DepartmentName)
        {
            List<DepartmentMaster> obj = new List<DepartmentMaster>();
            try
            {
                obj = _context.Database.SqlQuery<DepartmentMaster>("SP_DepartmentMaster @Mode = {0},@StoreId = {1},@DepartmentName = {2}", "SelectByStoreID_Name", StoreId, DepartmentName).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsRepository - SelectByStoreID_Name - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<DepartmentMaster> SelectForUpdate(int? StoreId, string DepartmentName, int DepartmentId)
        {
            List<DepartmentMaster> obj = new List<DepartmentMaster>();
            try
            {
                obj = _context.Database.SqlQuery<DepartmentMaster>("SP_DepartmentMaster @Mode = {0},@StoreId = {1},@DepartmentName = {2},@DepartmentId ={3}", "SelectForUpdate", StoreId, DepartmentName, DepartmentId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsRepository - SelectForUpdate - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public DepartmentMaster SelectByStoreID_DepartmentId(int StoreId, int iDeptID)
        {
            DepartmentMaster obj = new DepartmentMaster();
            try
            {
                obj = _context.Database.SqlQuery<DepartmentMaster>("SP_DepartmentMaster @Mode = {0},@StoreId = {1},@DepartmentId = {2}", "SelectByStoreID_DepartmentId", StoreId, iDeptID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsRepository - SelectByStoreID_DepartmentId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<AccountTypeMaster> GetAccountType(string AccType)
        {
            List<AccountTypeMaster> obj = new List<AccountTypeMaster>();
            try
            {
                obj = _context.Database.SqlQuery<AccountTypeMaster>("SP_AccountTypeMaster @Mode,@AccountType,@Flag", new SqlParameter("@Mode", "GetAccountType"), new SqlParameter("@AccountType", AccType), new SqlParameter("@Flag", "O")).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsRepository - GetAccountType - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<AccountDetailTypeMaster> GetQBDetailType(string DetailsType)
        {
            List<AccountDetailTypeMaster> obj = new List<AccountDetailTypeMaster>();
            try
            {
                obj = _context.Database.SqlQuery<AccountDetailTypeMaster>("SP_AccountDetailTypeMaster @Mode,@QBDetailType", new SqlParameter("@Mode", "GetQBDetailType"), new SqlParameter("@QBDetailType", DetailsType)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsRepository - GetQBDetailType - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public DepartmentMaster SelectByStoreID_DepId(int? StoreId, int DepartmentId)
        {
            DepartmentMaster obj = new DepartmentMaster();
            try
            {
                obj = _context.Database.SqlQuery<DepartmentMaster>("SP_DepartmentMaster @Mode = {0},@StoreId = {1},@DepartmentId = {2}", "SelectByStoreID_DepartmentId", StoreId, DepartmentId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsRepository - SelectByStoreID_DepId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public List<AccountTypeMaster> GetAccountType1(string QBAccountType)
        {
            List<AccountTypeMaster> obj = new List<AccountTypeMaster>();
            try
            {
                obj = _context.Database.SqlQuery<AccountTypeMaster>("SP_AccountTypeMaster @Mode,@AccountType,@Flag", new SqlParameter("@Mode", "GetAccountType"), new SqlParameter("@AccountType", QBAccountType), new SqlParameter("@Flag", "D")).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsRepository - GetAccountType1 - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public DepartmentMaster SelectByStoreID_Name1(int StoreId, string DepartmentName)
        {
            DepartmentMaster obj = new DepartmentMaster();
            try
            {
                obj = _context.Database.SqlQuery<DepartmentMaster>("SP_DepartmentMaster @Mode = {0},@StoreId = {1},@DepartmentName = {2}", "SelectByStoreID_Name", StoreId, DepartmentName).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsRepository - SelectByStoreID_Name1 - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public void SaveAccountTypeMasters(AccountTypeMaster objAcc)
        {
            try
            {
                _context.AccountTypeMasters.Add(objAcc);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsRepository - SaveAccountTypeMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void SaveAccountDetailTypeMasters(AccountDetailTypeMaster objDetail)
        {
            try
            {
                _context.AccountDetailTypeMasters.Add(objDetail);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsRepository - SaveAccountDetailTypeMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void UpdateDepartmentMaster(DepartmentMaster objDept)
        {
            try
            {
                _context.Entry(objDept).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsRepository - UpdateDepartmentMaster - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void SaveDepartmentMaster(DepartmentMaster objDept)
        {
            try
            {
                _context.DepartmentMasters.Add(objDept);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsRepository - SaveDepartmentMaster - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public List<DepartmentMaster> SelectByStoreID_ListID(int? StoreId, string ID)
        {
            List<DepartmentMaster> obj = new List<DepartmentMaster>();
            try
            {
                obj = _context.Database.SqlQuery<DepartmentMaster>("SP_departmentMaster.Value @Mode = {0},@StoreId = {1},@ListId = {2}", "SelectByStoreID_ListID", StoreId, ID).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsRepository - SelectByStoreID_ListID - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public async Task<DepartmentMaster> GetDepartmentMasterId(int? id)
        {
            DepartmentMaster obj = new DepartmentMaster();
            try
            {
                obj = await _context.DepartmentMasters.FindAsync(id);

            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsRepository - GetDepartmentMasterId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public async void DeleteConfirmed(DepartmentMaster departmentMaster)
        {
            try
            {
                _context.DepartmentMasters.Remove(departmentMaster);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsRepository - DeleteConfirmed - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void ActiveStatus(int deptid)
        {
            try
            {
                DepartmentMaster obj = new DepartmentMaster();
                obj = _context.DepartmentMasters.Find(deptid);
                obj.IsActive = true;
                obj.LastModifiedOn = DateTime.Now;
                _context.Entry(obj).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsRepository - ActiveStatus - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void InActiveStatus(int deptid)
        {
            try
            {
                DepartmentMaster obj = new DepartmentMaster();
                obj = _context.DepartmentMasters.Find(deptid);
                obj.IsActive = false;
                obj.LastModifiedOn = DateTime.Now;
                _context.Entry(obj).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("ChartofAccountsRepository - InActiveStatus - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
    }
}
