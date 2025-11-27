using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisQBOnline.BAL
{
   public class JournalEntry
    {
        public string ID { get; set; }
        public int storeid { get; set; }
        public int userid { get; set; }
        public DateTime? salesdate { get; set; }
        public DateTime? Createddate { get; set; }
        public int status { get; set; }
        public int syncstatus { get; set; }
        public decimal? totalamount { get; set; }
        public int noofpos { get; set; }
        public string TxnID { get; set; }
        public int IsSync { get; set; }
    }
}
