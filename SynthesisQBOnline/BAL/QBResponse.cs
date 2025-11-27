using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisQBOnline.BAL
{
    public class QBResponse
    {
        public string ID { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string SyncToken { get; set; }
        public string QBStatus { get; set; }
    }
}
