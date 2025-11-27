using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModels.HRModels
{
    [Table("HR_EmployeeMaster")]
    public class HREmployeeMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int EmployeeId { get; set; }

        public int LoginUserId { get; set; }

        [Required(ErrorMessage = "Employee User Name is Required.")]
        [MaxLength(100)]
        public string EmployeeUserName { get; set; }

        [Required(ErrorMessage = "Password is Required.")]
        [MaxLength(50)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is Required.")]
        [MaxLength(50)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "First Name is Required.")]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [MaxLength(100)]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Last Name is Required.")]
        [MaxLength(100)]
        public string LastName { get; set; }

        [MaxLength(100)]
        public string AdditionalLastName { get; set; }

        [Required(ErrorMessage = "Date of Birth is Required.")]
        public DateTime DateofBirth { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "DOB is Required.")]
        [RegularExpression(@"((0[1-9]|1[0-2])\/((0|1)[0-9]|2[0-9]|3[0-1])\/((19|20)\d\d))$", ErrorMessage = "DOB is Required.")]
        public string sDateofBirth { get; set; }

        [MaxLength(50)]
        public string SSN { get; set; }

        public Gender Gender { get; set; }

        public MaritalStatus MaritalStatus { get; set; }

        public Language LanguageId { get; set; }

        public int? EthnicityId { get; set; }
        [ForeignKey("EthnicityId")]
        public virtual HREthnicityMaster HREthnicityMaster { get; set; }


        //[EmailAddress(ErrorMessage = "Invalid Email Address")]
        [RegularExpression(@"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Invalid Email Address")]
        [MaxLength(50)]
        public string Email { get; set; }

        [MaxLength(50)]
        public string Phone { get; set; }

        [MaxLength(50)]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = "Street is Required.")]
        [MaxLength(100)]
        public string Street { get; set; }

        [MaxLength(20)]
        public string BuildingNo { get; set; }

        [Required(ErrorMessage = "City is Required.")]
        [MaxLength(20)]
        public string City { get; set; }

        public State State { get; set; }

        [Required(ErrorMessage = "Zip Code is Required.")]
        [MaxLength(20)]
        public string ZipCode { get; set; }

        public string Designation { get; set; }
        
        public string TraningFilePath { get; set; }
       
        public string TraningContent { get; set; }

        public DateTime? TrainingCompletedDateTime { get; set; }

        [DataType(DataType.Time)]
        public DateTime? TrainingCompletedTime { get; set; }

        [MaxLength(50)]
        public string LastSlidename { get; set; }

        [MaxLength(50)]
        public string FullSSN { get; set; }

        public bool IsTraningCompleted { get; set; } = false;

        public bool UseEmailAsLogin { get; set; } = false;

        public bool IsLocked { get; set; } = false;

        public bool IsDeleted { get; set; } = false;

        public bool IsLanguageSelect { get; set; } = false;

        public bool IsFirstLogin { get; set; } = true;

        public DateTime? ModifiedPasswordDate { get; set; } = DateTime.Now;

        public DateTime? CreatedOn { get; set; } = DateTime.Now;

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; } = DateTime.Now;

        public int? ModifiedBy { get; set; }
        [NotMapped]
        public string Message { get; set; }

        public virtual ICollection<HREmployeeChild> HREmployeeChild { get; set; }

        public virtual ICollection<HREmployeePayRate> HREmployeePayRates { get; set; }

        public virtual ICollection<HREmployeeSickTimes> HREmployeeSickTimes { get; set; }

        public virtual ICollection<HREmployeeVacationTime> HREmployeeVacationTimes { get; set; }

        public virtual ICollection<HREmployeeInsurance> HREmployeeInsurances { get; set; }

        public virtual ICollection<HREmployeeDocument> HREmployeeDocuments { get; set; }

        public virtual ICollection<HREmployeeNotes> HREmployeeNotes { get; set; }

        public virtual ICollection<HREmployeeTrainingHistory> HREmployeeTrainingHistories { get; set; }

        public virtual ICollection<HREmployeeRetirementInfo> HREmployeeRetirementInfos { get; set; }

        public virtual ICollection<HREmployeeHealthBenefitInfo> HREmployeeHealthBenefitInfos { get; set; }

        public virtual ICollection<HREmployeeVaccineCertificateInfo> HREmployeeVaccineCertificateInfos { get; set; }
    }

}
