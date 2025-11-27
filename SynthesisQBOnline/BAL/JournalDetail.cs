using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisQBOnline.BAL
{
  public  class JournalDetail
    {
        
        public string id { get; set; }
        public int Gid { get; set; }
        public int storeid { get; set; }

        public string Groupname { get; set; }
        public int groupid { get; set; }

        public Nullable<decimal> Amount { get; set; }
        public string Title { get; set; }
        public string Memo { get; set; }
        public int Typeid { get; set; }
        public string ListID { get; set; }
        public string EntityID { get; set; }
        public string EntityType { get; set; }
    }
}
