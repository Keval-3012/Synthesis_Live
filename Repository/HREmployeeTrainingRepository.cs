using EntityModels.HRModels;
using EntityModels.Models;
using NLog;
using Repository.IRepository;
using SynthesisViewModal.HRViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Repository
{
    public class HREmployeeTrainingRepository : IHREmployeeTrainingRepository
    {
        private DBContext _context;
        Logger logger = LogManager.GetCurrentClassLogger();
        public HREmployeeTrainingRepository(DBContext context) 
        {
            _context = context;
        }

        public HREmployeeMaster HREmployeeMaster(int EmployeeId)
        {
            HREmployeeMaster master = new HREmployeeMaster();
            try
            {
                logger.Info("HREmployeeTrainingRepository - HREmployeeMaster - " + DateTime.Now);
                master = _context.HREmployeeMaster.Where(b => b.EmployeeId == EmployeeId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeTrainingRepository - HREmployeeMaster - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return master;
        }

        public void SaveLanguageselection(HREmployeeMaster master)
        {
            try
            {
                logger.Info("HREmployeeTrainingRepository - SaveLanguageselection - " + DateTime.Now);
                master.IsLanguageSelect = true;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeTrainingRepository - SaveLanguageselection - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            
        }

        public void UpdateLastSlide(int UserId, string LastSlideName)
        {
            try
            {
                _context.Database.ExecuteSqlCommand("SP_HRApiTraining @Mode = {0}, @EmployeeId = {1}, @LastSlideName = {2}, @Ids = {3} OUTPUT", "UpdateLastSlide", UserId, LastSlideName,1);
                //var objEmp = _context.HREmployeeMaster.Where(b => b.EmployeeId == UserId).FirstOrDefault();
                //if (objEmp != null)
                //{

                //    objEmp.LastSlidename = LastSlideName;
                //    objEmp.ModifiedOn = DateTime.Now;
                //    objEmp.ModifiedBy = UserId;
                //    _context.SaveChanges();
                //}
            }
            catch (Exception ex)
            {
                logger.Error("HREmployeeTrainingRepository - UpdateLastSlide - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
    }
}
