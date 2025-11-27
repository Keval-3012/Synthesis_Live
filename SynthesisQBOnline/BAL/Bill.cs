using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisQBOnline.BAL
{
    public class Bill
    {
        public string ID { get; set; }
        public string SyncToken { get; set; }
        public string RefNumber { get; set; }
        public string VendorID { get; set; }
        public string VendorName { get; set; }
        public string Address { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string InvoiceDate { get; set; }
        public string DueDate { get; set; }
        public string Notes { get; set; }
        public string BillID { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string CreatedTime { get; set; }
        public string LastModifiedTime { get; set; }
    }
}
