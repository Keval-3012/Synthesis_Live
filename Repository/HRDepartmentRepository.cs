using EntityModels.HRModels;
using EntityModels.Models;
using NLog;
using Repository.IRepository;
using SynthesisViewModal.HRViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class HRDepartmentRepository : IHRDepartmentRepository
    {
        private DBContext _context;
        Logger logger = LogManager.GetCurrentClassLogger();
        public HRDepartmentRepository(DBContext context) 
        {
            _context = context;
        }

        public List<HRDepartmentViewModel> GetHRDepartmentList()
        {
            List<HRDepartmentViewModel> obj = new List<HRDepartmentViewModel>();
            try
            {
                logger.Info("HRDepartmentRepository - GetHRDepartmentList - " + DateTime.Now);
                obj = _context.Database.SqlQuery<HRDepartmentViewModel>("SP_GetAllHRDepartment @Mode = {0}", "Select").ToList();
            }
            catch (Exception ex)
            {
                logger.Error("HRDepartmentRepository - GetHRDepartmentList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public bool CheckDepartmentExistOrNot(HRDepartmentMaster hR)
        {
            bool CheckEorN = false;
            try
            {
                logger.Info("HRDepartmentRepository - CheckDepartmentExistOrNot - " + DateTime.Now);
                if (_context.HRDepartmentMasters.Where(p => p.DepartmentName.Equals(hR.DepartmentName) && p.IsActive == true).Count() > 0)
                {
                    CheckEorN = true;
                }
                else
                {
                    CheckEorN = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("HRDepartmentRepository - CheckDepartmentExistOrNot - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return CheckEorN;
        }

        public HRDepartmentMaster InsertHRDepartment(HRDepartmentMaster hR)
        {
            try
            {
                logger.Info("HRDepartmentRepository - InsertHRDepartment - " + DateTime.Now);
                _context.HRDepartmentMasters.Add(hR);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("HRDepartmentRepository - InsertHRDepartment - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return hR;
        }

        public bool UpdateTimeCheckDepartmentExistOrNot(HRDepartmentMaster hR)
        {
            bool CheckEorN = false;
            try
            {
                logger.Info("HRDepartmentRepository - UpdateTimeCheckDepartmentExistOrNot - " + DateTime.Now);
                HRDepartmentMaster obj = _context.HRDepartmentMasters.SingleOrDefault(b => b.DepartmentId == hR.DepartmentId);
                if (_context.HRDepartmentMasters.Where(p => p.DepartmentName.Equals(hR.DepartmentName, StringComparison.CurrentCultureIgnoreCase) && !p.DepartmentId.Equals(hR.DepartmentId) && p.IsActive == true).Count() > 0)
                {
                    CheckEorN = true;
                }
                else
                {
                    CheckEorN = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("HRDepartmentRepository - UpdateTimeCheckDepartmentExistOrNot - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return CheckEorN;
        }

        public HRDepartmentMaster UpdateHRDepartment(HRDepartmentMaster hR)
        {
            try
            {
                logger.Info("HRDepartmentRepository - UpdateHRDepartment - " + DateTime.Now);
                _context.Database.ExecuteSqlCommand("SP_GetAllHRDepartment @Mode={0},@IsActive={1},@DepartmentName={2},@DepartmentId={3},@ModifiedBy = {4},@ModifiedOn = {5}", "Update", hR.IsActive, hR.DepartmentName, hR.DepartmentId, hR.ModifiedBy, hR.ModifiedOn);
            }
            catch (Exception ex)
            {
                logger.Error("HRDepartmentRepository - UpdateHRDepartment - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return hR;
        }

        public HRDepartmentMaster RemoveHRDepartment(int DepartmentId)
        {
            HRDepartmentMaster hr = new HRDepartmentMaster();
            try
            {
                logger.Info("HRDepartmentRepository - RemoveHRDepartment - " + DateTime.Now);
                hr = _context.HRDepartmentMasters.Find(DepartmentId);
                _context.HRDepartmentMasters.Remove(hr);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("HRDepartmentRepository - RemoveHRDepartment - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return hr;
        }

        public void UpdateDepartmentStatus(HRDepartmentViewModel HR)
        {
            try
            {
                logger.Info("HRDepartmentRepository - UpdateDepartmentStatus - " + DateTime.Now);
                _context.Database.ExecuteSqlCommand("SP_GetAllHRDepartment @Mode = {0},@DepartmentId = {1},@IsActive = {2},@ModifiedBy = {3},@ModifiedOn = {4}", "UpdateDepartmentStatus", HR.DepartmentId, HR.IsActive, HR.ModifiedBy, HR.ModifiedOn);
            }
            catch (Exception ex)
            {
                logger.Error("VendorMasterRepository - UpdateDepartmentStatus - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public HRDepartmentMaster GetHRDepartmentById(int DepartmentId)
        {
            HRDepartmentMaster hr = new HRDepartmentMaster();
            try
            {
                logger.Info("HRDepartmentRepository - GetHRDepartmentById - " + DateTime.Now);
                hr = _context.HRDepartmentMasters.Find(DepartmentId);
            }
            catch (Exception ex)
            {
                logger.Error("HRDepartmentRepository - GetHRDepartmentById - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return hr;
        }
    }
}
