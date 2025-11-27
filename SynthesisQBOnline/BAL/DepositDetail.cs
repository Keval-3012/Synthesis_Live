using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisQBOnline.BAL
{
    public class DepositDetail
    {
        public string id { get; set; }
        public int Gid { get; set; }
        public int storeid { get; set; }

        public string Description { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string EntityID { get; set; }
        public string EntityType { get; set; }
        public string AccountID { get; set; }

        public string PaymentMethod { get; set; }
        public string CheckNum { get; set; }

    }
}
