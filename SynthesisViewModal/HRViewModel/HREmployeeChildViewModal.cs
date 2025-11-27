using EntityModels.HRModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisViewModal.HRViewModel
{
    public class HREmployeeChildViewModal
    {
        public int? EmployeeChildId { get; set; }
        public int? EmployeeId { get; set; }
        public int? SrNo { get; set; }
        public string OfficeEmployeeID { get; set; }
        public DateTime? HireDate { get; set; }
        public string sHireDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public string sTerminationDate { get; set; }
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
        public EmployeementTypeStatus? EmployeementTypeStatus { get; set; }
        public string EmployeementTypeStatusName
        {
            get
            {
                if (EmployeementTypeStatus.HasValue)
                {
                    return Enum.GetName(typeof(EmployeementTypeStatus), EmployeementTypeStatus.Value);
                }
                return null;
            }
        }
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }
}
