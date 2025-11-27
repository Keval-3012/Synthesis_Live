using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace EntityModels.HRModels
{
    public enum Gender
    {
        Male = 1,
        Female = 2
    }
    public enum MaritalStatus
    {
        Single = 1,
        Married = 2,
        Divorced = 3,
        Widowed = 4,
    }
    public enum Language
    {
        English = 1,
        Español = 2
    }

    public enum State
    {
        NY = 1,
        NJ = 2,
        CT = 3,
        PA = 4
    }
    public enum Status
    {
        Active = 1,
        Terminated = 2,
        Deceased = 3,
        Resigned = 4
    }
    public enum EmployeementTypeStatus
    {
        [Display(Name = "Full Time")]
        FullTime = 1,
        [Display(Name = "Part Time")]
        PartTime = 2,
        [Display(Name = "Temporary")]
        Temporary = 3,
        [Display(Name = "Oncall")]
        Oncall = 4,

    }
    public enum PayType
    {
        Hourly = 1,
        Daily = 2
    }
    public enum TimeType
    {
        Used = 1,
        Awarded = 2
    }
    public enum EnrollmentStatus
    {
        Waived = 1,
        Daily = 2
    }


    public enum InputTypes
    {
        Checkbox = 1,
        Textbox = 2
    }

    public enum DocumentT
    {
        Consent = 1,
        ScheduleChange = 2
    }

    public enum Sign
    {
        Signed = 1,
        UnSigned = 2
    }

}
