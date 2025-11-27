using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisViewModal
{
    public class ListWebcamHistoryViewModal
    {
        public int? StoreIds { get; set; }
        public int count { get; set; }
        public string RecordingDate { get; set; } = "";
        public string maxdate { get; set; }
    }
}
