using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisViewModal
{
    public class LineItemViewModel
    {

        public decimal Qty { get; set; }
        public string Name { get; set; }
        public int StoreId { get; set; }
        public string NickName { get; set; }
        public string InvoiceNumber { get; set; }
        public int InvoiceId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal UnitPrice { get; set; }
        public string VendorName { get; set; }
        public string Description { get; set; }
    }
}
