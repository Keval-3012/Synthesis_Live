using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisViewModal
{
    public class DepartmentMastersViewModal
    {
        public int? StoreIds { get; set; }
        public int StoreId { get; set; }
        public int count { get; set; }
        public string error { get; set; } = null;
        public string Sucsses { get; set; } = null;
    }
}
