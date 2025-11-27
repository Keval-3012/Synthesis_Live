using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisQBOnline.BAL
{
    public class ExpenseDetail
    {
        public string TxnID { get; set; }
        public string ID { get; set; }
        public string LineNum { get; set; }
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Description { get; set; }
        public Nullable<int> PayrollId { get; set; }
        public Nullable<int> PayrollReportId { get; set; }
    }
}
