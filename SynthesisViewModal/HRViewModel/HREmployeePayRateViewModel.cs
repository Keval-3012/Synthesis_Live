using EntityModels.HRModels;
using System;
using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisViewModal.HRViewModel
{
    public class HREmployeePayRateViewModel
    {
        public int EmployeePayRateId { get; set; }
        public int? EmployeeId { get; set; }
        public int? EmployeeChildId { get; set; }
        public DateTime? PayRateDate { get; set; }
        public string sPayRateDate
        {
            get
            {
                if (PayRateDate.HasValue)
                {
                    // Return formatted string in "MM/dd/yyyy" format
                    return PayRateDate.Value.ToString("MM/dd/yyyy");
                }
                else
                {
                    return null; // or return null if you prefer
                }
            }
        }
        public int PayType { get; set; }
        public string PayTypeName
        {
            get
            {
                return Enum.GetName(typeof(PayType), PayType);
            }
        }
        public decimal? PayRate { get; set; }

        public string Comments { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? CreatedOn { get; set; } = DateTime.Now;

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; } = DateTime.Now;

        public int? ModifiedBy { get; set; }

    }

    public class promptButtonModel
    {
        public string content { get; set; }
        public bool isPrimary { get; set; }
    }

    public class promptButtonSSNModel
    {
        public string content { get; set; }
        public bool isPrimary { get; set; }
    }

    public class ValidPayRate
    {
        public int EmployeePayRateId { get; set; }
        public decimal? PayRate { get; set; }
        public string Password { get; set; }
    }

    public class ValidPassForSSN
    {
        public int EmployeeId { get; set; }
        public string SSN { get; set; }
        public string Password { get; set; }
    }
}
