using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisViewModal
{
    public class PDFReadViewModal
    {
        protected static string StatusMessage { get; set; } = "";
        protected static Array Arr { get; set; }
        protected static bool IsArray { get; set; }
        protected static IEnumerable BindData { get; set; }
        protected static bool IsEdit { get; set; } = false;
        protected static int TotalDataCount { get; set; }
    }
}
