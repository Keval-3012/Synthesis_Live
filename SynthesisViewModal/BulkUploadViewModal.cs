using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisViewModal
{
    public class BulkUploadViewModal
    {
        public int IsBindData { get; set; } = 1; 
        public int currentPageIndex { get; set; } = 1; 
        public string orderby { get; set; } = "UploadPdfId"; 
        public int IsAsc { get; set; } = 0; 
        public int PageSize { get; set; } = 20; 
        public int SearchRecords { get; set; } = 1; 
        public string Alpha { get; set; } = ""; 
        public int deptname { get; set; } = 0; 
        public string startdate { get; set; } = ""; 
        public string enddate { get; set; } = ""; 
        public string payment { get; set; } = ""; 
        public string Store_val { get; set; } = ""; 
        public string searchdashbord { get; set; } = ""; 
        public string AmtMaximum { get; set; } = "0"; 
        public string AmtMinimum { get; set; } = "0";
        public bool ShowMyInvoice { get; set; } = false;
    }
}
