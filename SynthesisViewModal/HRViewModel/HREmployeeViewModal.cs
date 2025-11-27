using EntityModels.HRModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisViewModal.HRViewModel
{
    public class HREmployeeViewModal
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string sDateofBirth { get; set; }
        public DateTime? DateofBirth { get; set; }
        public string SSN { get; set; }
        public string OfficeEmployeeID { get; set; }
        public string NickName { get; set; }
        public bool IsTraningCompleted { get; set; }
        public bool IsLocked { get; set; }
        public Status? Status { get; set; }
        public string StatusName
        {
            get
            {
                if (Status.HasValue)
                {
                    return Enum.GetName(typeof(Status), Status.Value);
                }
                return null;
            }
        }
        public int? OptStatus { get; set; }
        public string OptStatusValue { get; set; }
        public string HelathBenefitUsed { get; set; }
        public string TraningFilePath { get; set; }
        public string TraningContent { get; set; }
        public string LastSlidename { get; set; }
        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set;}
        public DateTime? HireDate { get; set;}
        public string sHireDate { get;set; }
        public string EmployeeUserName { get;set; }
        
    }

    public class EnumDropDown
    {
        public int value { get; set; }
        public string text { get; set; } = string.Empty;
    }

    public class ToastModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string CssClass { get; set; }
        public string Icon { get; set; }
    }

    public class EmployeeFilterModel
    {
        public string HiringFromDate { get; set; }
        public string HiringToDate { get; set; }
        public int? DepartmentId { get; set; }
        public bool TStatusComplete { get; set; }
        public bool TStatusPending { get; set; }
        public bool EStatusDeceased { get; set; }
        public bool EStatusActive { get; set; }
        public bool EStatusOnTerminated { get; set; }
        public bool EStatusResigned { get; set; }
        public bool PStatusNotUsed { get; set; }
        public bool PStatusOptIn { get; set; }
        public bool PStatusOptOut { get; set; }
        public bool MIStatusNotUsed { get; set; }
        public bool MIStatusUsed { get; set; }
    }


}
