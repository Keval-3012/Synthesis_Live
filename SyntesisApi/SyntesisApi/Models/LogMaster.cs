using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SyntesisApi.Models
{
    public class LogMaster
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
}