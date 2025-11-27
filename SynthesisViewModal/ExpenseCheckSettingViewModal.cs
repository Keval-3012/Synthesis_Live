using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisViewModal
{
    public class ExpenseCheckSettingViewModal
    {
        public int[] Type { get; set; } = null;
        public int[] IsActive { get; set; } = null;
        public int[] StoreID { get; set; } = null;
        public string[] DeptExcludeList { get; set; } = null;
        public string MSG { get; set; } = null;
        public decimal? AmtMinimum { get; set; } = null;
        public decimal? AmtMaximum { get; set; } = null;
        public int IsBindData { get; set; } = 1;
        public int currentPageIndex { get; set; } = 1;
        public string orderby { get; set; } = "TXnDate";
        public int IsAsc { get; set; } = 0;
        public int PageSize { get; set; } = 50;
        public int SearchRecords { get; set; } = 1;
        public string Alpha { get; set; } = "";
        public int storename { get; set; } = 0;
        public int? deptid { get; set; } = null;
        public string startdate { get; set; } = "";
        public string enddate { get; set; } = "";
        public string searchdashbord { get; set; } = "";
        public string SearchFlg { get; set; } = "";
        public string payment { get; set; } = "";
        public string[] radio { get; set; } = null;
        public string txtstartdate { get; set; } = "";
        public string txtenddate { get; set; } = "";
        public string ECLstStore { get; set; } = "";
        public string DrpLstdept { get; set; } = "";
        public int StoreId { get; set; } = 0;
    }
}
