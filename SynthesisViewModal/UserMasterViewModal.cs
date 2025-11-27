using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisViewModal
{
    public class UserMasterViewModal
    {
        public int IsBindData { get; set; }
        public int currentPageIndex { get; set; }
        public string orderby { get; set; } = "UserId";
        public int IsAsc { get; set; } = 0;
        public int PageSize { get; set; } = 100;
        public int SearchRecords { get; set; } = 1;
        public string SearchTitle { get; set; } = "";
        public string Alpha { get; set; } = "";
    }
}
