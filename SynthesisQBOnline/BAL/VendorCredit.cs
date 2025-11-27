using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisQBOnline.BAL
{
    public class VendorCredit
    {
        public string ID { get; set; }
        public string SyncToken { get; set; }
        public string CreatedTime { get; set; }
        public string LastModifiedTime { get; set; }
        public string RefNumber { get; set; }
        public string VendorID { get; set; }
        public string VendorName { get; set; }
        public string Address { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<System.DateTime> TxnDate { get; set; }
        public string Notes { get; set; }
    }
}
