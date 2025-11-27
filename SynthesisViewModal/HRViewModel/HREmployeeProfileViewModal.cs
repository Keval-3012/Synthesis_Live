using EntityModels.HRModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisViewModal.HRViewModel
{
    public class HREmployeeProfileViewModal
    {
        public int EmployeeId { get; set; }
        public DateTime DateofBirth { get; set; }
        public string sDateofBirth
        {
            get
            {
                return DateofBirth.ToString("MM/dd/yyyy");
            }
        }
        public string FullSSN { get; set; }
        public string SSV
        {
            get
            {
                return "XXX-XX-" + FullSSN.Replace(" ", "").Substring(FullSSN.Replace(" ", "").Length - 4, 4);
            }
        }
        public Gender Gender { get; set; }
        public string GenderName
        {
            get
            {
                return Enum.GetName(typeof(Gender), Gender);
            }
        }
        public MaritalStatus MaritalStatus { get; set; }
        public string MaritalStatusName
        {
            get
            {
                return Enum.GetName(typeof(MaritalStatus), MaritalStatus);
            }
        }
        public Language LanguageId { get; set; }
        public string LanguageIdName
        {
            get
            {
                return Enum.GetName(typeof(Language), LanguageId);
            }
        }
        
        public string Street { get; set; }
        public string City { get; set; }

        public string Address
        {
            get
            {
                return Street + " " + City;
            }
        }

        public string Email { get; set; }
        public string Phone { get; set; }
        public string MobileNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public string Designation { get; set; }
        public int? EmployeeChildId { get; set; }
    }

    public class HREmployeeProfileChild
    {
        public int EmployeeId { get; set; }
        public DateTime? HireDate { get; set; }
        public string sHireDate
        {
            get
            {
                if (HireDate.HasValue)
                {
                    // Return formatted string in "MM/dd/yyyy" format
                    return HireDate.Value.ToString("MM/dd/yyyy");
                }
                else
                {
                    return null; // or return null if you prefer
                }
            }
        }
        public DateTime? TerminationDate { get; set; }
        public string sTerminationDate
        {
            get
            {
                if (TerminationDate.HasValue)
                {
                    // Return formatted string in "MM/dd/yyyy" format
                    return TerminationDate.Value.ToString("MM/dd/yyyy");
                }
                else
                {
                    return null; // or return null if you prefer
                }
            }
        } 
        public int EmployeeChildId { get; set;}
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public EmployeementTypeStatus? EmployeementTypeStatus { get; set; }
        public string EmployeementTypeStatusName
        {
            get
            {
                if (EmployeementTypeStatus.HasValue)
                {
                    MemberInfo memberInfo = typeof(EmployeementTypeStatus).GetField(EmployeementTypeStatus.ToString());
                    DisplayAttribute displayAttribute = memberInfo.GetCustomAttribute<DisplayAttribute>();
                    if (displayAttribute != null)
                    {
                        return displayAttribute.Name;
                    }
                }
                return null;
            }
        }
        public string OfficeEmployeeID { get; set; }
    }
}
