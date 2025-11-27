using EntityModels.Models;
using NLog;
using Repository.IRepository;
using SynthesisViewModal.HRViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class HRAdminsRepository : IHRAdminsRepository
    {
        private DBContext _context;
        Logger logger = LogManager.GetCurrentClassLogger();
        public HRAdminsRepository(DBContext context)
        {
            _context = context;
        }
        public List<HRAdmins> GetHRAdminList()
        {
            List<HRAdmins> data = new List<HRAdmins>();
            try
            {
                logger.Info("HRAdminsRepository - GetHRAdminList - " + DateTime.Now);
                data = (from result in _context.UserMasters.Where(s => s.IsHRAdmin == true)
                        select new HRAdmins
                        {
                            UserId = result.UserId,
                            UserName = result.UserName,
                            Name = result.FirstName
                        }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("HRAdminsRepository - GetHRAdminList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return data;
        }
    }
}
