using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisViewModal.HRViewModel
{
    public class HRWarningViewModel
    {
        public int EmployeeWarningId { get; set; }
        public int EmployeeId { get; set; }
        public int EmployeeChildId { get; set; }
        public string DocFileName { get; set; }
        public string Warning { get; set; }
        public int StoreId { get; set; }
        public DateTime? CreatedOn { get; set; }
    }

    public class HRWarningList
    {
        public List<HRWarningViewModel> firstwarningfile { get; set; }
        public List<HRWarningViewModel> secondwarningfile { get; set; }
        public List<HRWarningViewModel> finalwarningfile { get; set; }
    }
}
