using EntityModels.HRModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisViewModal.HRViewModel
{
    public class HRNoteViewModal
    {
        public int EmployeeNoteId { get; set; }

        public int? EmployeeId { get; set; }

        public int? EmployeeChildId { get; set; }

        public string Notes { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? CreatedOn { get; set; } = DateTime.Now;

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; } = DateTime.Now;

        public int? ModifiedBy { get; set; }

        public string CreatedbyName { get; set; }
    }
}
