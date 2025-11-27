using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisViewModal
{
    public class LevelApproversViewModal
    {
        public int IsBindData { get; set; } = 1;
        public int currentPageIndex { get; set; } = 1;
        public string orderby { get; set; } = "LevelsApproverId";
        public int IsAsc { get; set; } = 0;
        public int PageSize { get; set; } = 100;
        public int SearchRecords { get; set; } = 1;
        public string Alpha { get; set; } = "";
        public string SearchTitle { get; set; } = "";
        public int startIndex {  get; set; }
        public int endIndex {  get; set; }
    }
}
