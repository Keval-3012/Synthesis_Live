using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisViewModal
{
    public class DocumentsViewModal
    {
        public string Cloudurl { get; set; }
        public string StatusMessage { get; set; } = "";
        public string ActivityLogMessage { get; set; } = "";
        public string DeleteMessage { get; set; } = "";
        public int FirstPageIndex { get; set; } = 1;
        public int TotalDataCount { get; set; }
        public int RtnDatalistcount { get; set; }
        public Array Arr { get; set; }
        public bool IsArray { get; set; }
        public bool IsEdit { get; set; } = false;
        public string IsFilter { get; set; } = "0";
        public string paymethod { get; set; } = "";
        public int currentPageIndex { get; set; }
        public string strdashbordsuccess { get; set; }
        public string ExistCode { get; set; } = "";
    }
}
