using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisViewModal
{
    public class InvoiceReportViewModal
    {
        public int IsBindData { get; set; } = 1;
        public int currentPageIndex { get; set; } = 1;
        public string orderby { get; set; } = "InvoiceId";  
        public int IsAsc { get; set; } = 0;  
        public int PageSize { get; set; } = 100;  
        public int SearchRecords { get; set; } = 1;  
        public string Alpha { get; set; } = "";  
        public int deptname { get; set; } = 0;  
        public string startdate { get; set; } = "";  
        public string enddate { get; set; } = "";  
        public string payment { get; set; } = "";
        public string Store_val { get; set; } = "";  
        public int Status { get; set; } = 0;  
        public int VendorName { get; set; } = 0;  
        public decimal AmtMaximum { get; set; } = 0;
        public decimal AmtMinimum { get; set; } = 0;
        public int storeid { get; set; } = 0;
        public int istoreID { get; set; } = 0;
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
    }
}
