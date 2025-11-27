using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisViewModal.HRViewModel
{
    public class HRFileDetailViewModel
    {
        public int DocId { get; set; }
        public int EmployeeId { get; set; }
        public int EmployeeChildId { get; set; }
        public string FileName { get; set; }
        public int StoreId { get; set; }
        public string DocType { get; set; }
        public int DocumentType { get; set; } 
        public string CreatedOn { get; set; }
    }

    public class HRAllFileList
    {
        public List<HRFileDetailViewModel> File401KPlan { get; set; }
        public List<HRFileDetailViewModel> MobileInsurance { get; set; }
        public List<HRFileDetailViewModel> EssentialDocuments { get; set; }
        public List<HRFileDetailViewModel> SignedForms { get; set; }
        public List<HRFileDetailViewModel> VaccinationInfo { get; set; }
        public List<HRFileDetailViewModel> Warning { get; set; }
        public List<HRFileDetailViewModel> Termination { get; set; }
    }

}
