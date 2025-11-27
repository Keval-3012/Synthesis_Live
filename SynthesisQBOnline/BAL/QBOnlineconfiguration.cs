using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisQBOnline.BAL
{
    public class QBOnlineconfiguration
    {
        public Nullable<int> StoreId { get; set; }
        public string URL { get; set; }
        public string RealmID { get; set; }
        public string ClientId { get; set; }
        public string ClientSecretKey { get; set; }
        public string QBToken { get; set; }
        public string QBRefreshToken { get; set; }
        public string LogStatus { get; set; }
    }
}
