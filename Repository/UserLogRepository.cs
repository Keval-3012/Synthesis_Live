using EntityModels.Models;
using NLog;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class UserLogRepository : IUserLogRepository
    {
        private DBContext _context;
        Logger logger = LogManager.GetCurrentClassLogger();

        public UserLogRepository(DBContext context)
        {
            _context = context;
        }

        public List<UserList> GetUserLogData(string[] Id)
        {
            List<UserList> userList = new List<UserList>();
            try
            {
                userList = _context.UserMasters.Where(s => Id.Contains(s.UserId.ToString())).Select(o => new UserList { UserId = o.UserId, UserName = o.FirstName }).OrderBy(o => o.UserName).ToList();
                userList.Insert(0, new UserList { UserName = "All Users" });
            }
            catch (Exception ex)
            {
                logger.Error("UserLogRepository - GetUserLogData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return userList;
        }
    }
}
