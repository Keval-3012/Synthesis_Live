using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisViewModal
{
    public class ReportViewModel
    {
        public Array Arr { get; set; }
        public bool IsArray { get; set; }
        public string StatusMessage { get; set; } = "";
        public string ErrorMessage { get; set; } = "";
        public string InsertMessage { get; set; } = "";
        public string DeleteMessage { get; set; } = "";
        public string Editmessage { get; set; } = "";
        public bool IsFirst { get; set; } = true;
        public bool IsEdit { get; set; } = false;
        public int TotalDataCount { get; set; }
        public IEnumerable BindDailyPOSData { get; set; }
        public IEnumerable BindPayrollExpenseData { get; set; }
        public string sStoreID { get; set; }
        public string action { get; set;}
        public string Controller { get; set; }
    }

    public class ReportModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int? ShiftId { get; set; }
        public int? StoreId { get; set; }
        public string Department { get; set; }
        public string CheckFlag { get; set; } = "";

        public DateTime? Sdate { get; set; } = null;
        public DateTime? Enddate { get; set; } = null;
        public int startIndex { get; set; }
        public int endIndex { get; set; }
        public IEnumerable Data { get; set; } = null;
    }
}
