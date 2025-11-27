using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisViewModal.HRViewModel
{
    public class HRTerminationViewModel
    {
        public int EmployeeTerminationId { get; set; }
        public int EmployeeId { get; set; }
        public int EmployeeChildId { get; set; }
        public string DocFileName { get; set; }
        public int StoreId { get; set; }
        public DateTime? CreatedOn { get; set; }
    }

    public class HRTerminationList
    {
        public List<HRTerminationViewModel> terminationfile { get; set; }
    }
}
