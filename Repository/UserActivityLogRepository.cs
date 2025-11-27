using EntityModels.Models;
using NLog;
using Repository.IRepository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class UserActivityLogRepository : IUserActivityLogRepository
    {
        private DBContext _context;
        Logger logger = LogManager.GetCurrentClassLogger();

        public UserActivityLogRepository(DBContext context)
        {
            _context = context;
        }
        public IEnumerable GetData(int SearchRecords = 0, string SearchTitle = "")
        {
            IEnumerable RtnData = null;
            try
            {
               RtnData = _context.ActivityLogs.OrderByDescending(o => o.ActivityLogId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ActivityLogRepository - GetData - " + DateTime.Now + " - "+ ex.Message.ToString());
            }
            return RtnData;
        }
        public int getLastPageIndex(int PageSize, int TotalDataCount)
        {
            int lastPageIndex = Convert.ToInt32(TotalDataCount) / PageSize;
            try
            {
                if (TotalDataCount % PageSize > 0)
                    lastPageIndex += 1;
            }
            catch (Exception ex)
            {
                logger.Error("ActivityLogRepository - getLastPageIndex - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lastPageIndex;
        }
        public ActivityLog ActivityLogInsert(ActivityLog activityLog)
        {
            try
            {
                _context.ActivityLogs.Add(activityLog);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("ActivityLogRepository - ActivityLogInsert - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return activityLog;
        }
    }
}
