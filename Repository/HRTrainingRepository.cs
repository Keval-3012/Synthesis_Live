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
    public class HRTrainingRepository : IHRTrainingRepository
    {
        private DBContext _context;
        Logger logger = LogManager.GetCurrentClassLogger();
        public HRTrainingRepository(DBContext context) 
        {
            _context = context;
        }

        public List<HRTrainingViewModal> GetTrainingList(int store_idval)
        {
            List<HRTrainingViewModal> obj = new List<HRTrainingViewModal>();
            try
            {
                logger.Info("HRTrainingRepository - GetTrainingList - " + DateTime.Now);
                obj = _context.Database.SqlQuery<HRTrainingViewModal>("SP_GetAllResetTraining @Mode = {0},@StoreId = {1}", "Select",store_idval).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("HRTrainingRepository - GetTrainingList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return obj;
        }

        public void ResetTrainingCheck(List<int> selectedIds, int UserId)
        {
            try
            {
                logger.Info("HRTrainingRepository - ResetTrainingCheck - " + DateTime.Now);
                foreach (var id in selectedIds)
                {
                    var employee = _context.HREmployeeMaster.FirstOrDefault(e => e.EmployeeId == id);
                    if (employee != null)
                    {
                        HREmployeeTrainingHistory onjcer = new HREmployeeTrainingHistory();
                        onjcer.EmployeeId = id;
                        onjcer.TraningFilePath = employee.TraningFilePath;
                        onjcer.TrainingContent = employee.TraningContent;
                        onjcer.TrainingCompletedDateTime = employee.TrainingCompletedDateTime;
                        onjcer.IsTrainingCompleted = true;
                        onjcer.CreatedOn = DateTime.Now;
                        onjcer.CreatedBy = UserId;                        
                        _context.HREmployeeTrainingHistory.Add(onjcer);
                        _context.SaveChanges();

                        //for reset training in HR_EmployeeMaster table
                        _context.Database.ExecuteSqlCommand("SP_HREmployeeTrainingHistory @Mode = {0},@EmployeeId = {1},@ModifiedBy = {2}", "ResetTraining", id, UserId);

                        //employee.TraningFilePath = null;
                        //employee.TraningContent = null;
                        //employee.TrainingCompletedDateTime = null;
                        //employee.LastSlidename = null;
                        //employee.IsTraningCompleted = false;
                        //employee.ModifiedOn = DateTime.Now;
                        //employee.ModifiedBy = UserId;
                        //_context.SaveChanges();
                    }
                    employee = null;
                }
            }
            catch (Exception ex)
            {
                logger.Error("HRTrainingRepository - ResetTrainingCheck - " + DateTime.Now + " - " + ex.Message.ToString());
            }    
        }
    }
}
