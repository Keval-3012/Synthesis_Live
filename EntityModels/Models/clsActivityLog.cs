using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModels.Models
{
    public class clsActivityLog
    {
        public string ModuleName { get; set; }
        public string PageName { get; set; }
        public string Message { get; set; }
        public int CreatedBy { get; set; }
        public string Action { get; set; }
    }
    public class UserLogDetails
    {
        public string UserID { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string ModuleName { get; set; }
        public string ActionName { get; set; }
    }

    public class ActionList
    {
        public string ActionName { get; set; }
    }

    public class ModuleList
    {
        public string ModuleName { get; set; }
    }

    public class LogUserList
    {
        public string ModuleName { get; set; }
        public string LandingDate { get; set; }
        public string Action { get; set; }
        public string Message { get; set; }
        public string InvoiceID { get; set; }
        public string ProductId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string TotalTimeSpent { get; set; }
        public string AllData { get; set; }
    }
}
