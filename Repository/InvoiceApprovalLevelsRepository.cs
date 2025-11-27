using EntityModels.Models;
using NLog;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Repository
{
    public class InvoiceApprovalLevelsRepository : IInvoiceApprovalLevelsRepository
    {
        private DBContext _context;
        Logger logger = LogManager.GetCurrentClassLogger();

        public InvoiceApprovalLevelsRepository(DBContext context)
        {
            _context = context;
        }
        public List<LevelsApprover> GetBindData()
        {
            List<LevelsApprover> levelsApprovers = new List<LevelsApprover>();
            try
            {
                levelsApprovers = _context.levelsApprovers.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceApprovalLevelsRepository - GetBindData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return levelsApprovers;
        }
        public bool CheckExist(LevelsApprover modelObj)
        {
            bool status = false;
            try
            {
                if (_context.levelsApprovers.Where(s => s.LevelsApproverId != modelObj.LevelsApproverId && s.LevelName.ToLower().Trim() == modelObj.LevelName.ToLower().Trim()).Count() > 0)
                {
                    status = true;
                }
                else
                {
                    status = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceApprovalLevelsRepository - CheckExist - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return status;
        }
        public void SaveUserDetails(LevelsApprover LevelsApprover)
        {
            try
            {
                _context.levelsApprovers.Add(LevelsApprover);
                _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceApprovalLevelsRepository - SaveUserDetails - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public LevelsApprover DeleteUserDetails(int id)
        {
            LevelsApprover levelsApprover = new LevelsApprover();
            try
            {
                levelsApprover = _context.levelsApprovers.Find(id);
                _context.levelsApprovers.Remove(levelsApprover);    
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceApprovalLevelsRepository - DeleteUserDetails - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return levelsApprover;
        }
        public async Task<LevelsApprover> GetFindApproveMaster(int? Id)
        {
            LevelsApprover levelsApprover = new LevelsApprover();
            try
            {
                levelsApprover = await _context.levelsApprovers.FindAsync(Id);
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceApprovalLevelsRepository - GetFindApproveMaster - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return levelsApprover;
        }
        public void UpdateUserDetails(LevelsApprover LevelsApprover)
        {
            try
            {
                _context.Entry(LevelsApprover).State = EntityState.Modified;
                _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.Error("InvoiceApprovalLevelsRepository - UpdateUserDetails - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
    }
}
