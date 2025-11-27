using EntityModels.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IUserActivityLogRepository
    {
        IEnumerable GetData(int SearchRecords = 0, string SearchTitle = "");

        int getLastPageIndex(int PageSize, int TotalDataCount);
        ActivityLog ActivityLogInsert(ActivityLog activityLog);
    }
}
