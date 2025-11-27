using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisViewModal
{
    public class PayrollAccountViewModal
    {
        public int IsBindData { get; set; } = 1;
        public int currentPageIndex { get; set; } = 1;
        public string orderby { get; set; } = "id";
        public int IsAsc { get; set; } = 0;
        public int PageSize { get; set; } = 50;
        public int SearchRecords { get; set; } = 1;
        public string Alpha { get; set; } = "";
        public string SearchTitle { get; set; } = "";
        public int startIndex { get; set; }
        public int endIndex { get; set; }
    }
    public class UpdatePRAccount
    {
        public int ID { get; set; } 
        public string QBAccountid { get; set; } 
        public string Description { get; set; } 
        public string ValueIn { get; set; } 
        public string IsActive { get; set; } 
        public int? NewSortingNo { get; set; } 
    }
    public class UpdateBankAccount_Setting
    {
        public int ID { get; set; }
        public string QBAccountid { get; set; }
        public string VendorId { get; set; }
    }
}
