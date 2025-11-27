using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisViewModal.HRViewModel
{
    public class HRDepartmentViewModel
    {
        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? CreatedOn { get; set; } = DateTime.Now;
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; } = DateTime.Now;
        public int? ModifiedBy { get; set; }
        public string Status { get; set; }

    }
}
