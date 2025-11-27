using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("PayrollReport")]
    public class PayrollReport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int PayrollReportId { get; set; }

        public int StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        [MaxLength(200)]
        public string FileName { get; set; }

        public DateTime UploadDate { get; set; }

        public bool IsRead { get; set; }

        [MaxLength(50)]
        public string TxnID { get; set; }

        public int? IsSync { get; set; }

        [MaxLength(50)]
        public string TransactionNo { get; set; }

        public int? FIleNo { get; set; }
        public virtual ICollection<PayrollMaster> PayrollMasters { get; set; }

        public DateTime? ApproveDate { get; set; }

    }

    [Table("PayrollMaster")]
    public class PayrollMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int PayrollId { get; set; }

        public int StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int PayrollReportId { get; set; }
        [ForeignKey("PayrollReportId")]
        public virtual PayrollReport PayrollReports { get; set; }

        public DateTime? EndCheckDate { get; set; }
        public virtual ICollection<PayrollDepartmentDetails> payrollDepartmentDetails { get; set; }
        public virtual ICollection<PayrollCashAnalysisDetail> PayrollCashAnalysisDetails { get; set; }
    }

    [Table("PayrollDepartment")]
    public class PayrollDepartment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int PayrollDepartmentId { get; set; }

        public int StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        [MaxLength(100)]
        public string DepartmentName { get; set; }
        public virtual ICollection<PayrollDepartmentDetails> payrollDepartmentDetails { get; set; }

        public virtual ICollection<PayrollHoursDetails> payrollHoursDetails { get; set; }
    }
    [Table("PayrollDepartmentDetails")]
    public class PayrollDepartmentDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int PayrollDepartmentDetId { get; set; }

        public int PayrollDepartmentId { get; set; }
        [ForeignKey("PayrollDepartmentId")]
        public virtual PayrollDepartment PayrollDepartments { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        public decimal Value { get; set; }

        public int PayrollId { get; set; }
        [ForeignKey("PayrollId")]
        public virtual PayrollMaster PayrollMasters { get; set; }
    }
    
     public class PayrollFileList
    {
        public string FileName { get; set; }
        public string UploadDate { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string ProcessDate { get; set; }
        public int PayrollReportID { get; set; }
        public string EndCheckDate { get; set; }
        public int StoreID { get; set; }
        public bool IsRead { get; set; }
    }

    public class PDFTable
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public string Name2 { get; set; }
        public string Value2 { get; set; }

        public string Name3 { get; set; }
        public string Value3 { get; set; }

        public string Name4 { get; set; }
        public string Value4 { get; set; }

        public string Name5 { get; set; }
        public string Value5 { get; set; }

    }

    public class PayrollDetails
    {
        public string DepartmentName { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }

    }
    public class PayrollAccount_Select
    {
        public int PayrollCashAnalysisId { get; set; }
        public int? StoreId { get; set; }
        public string Name { get; set; }
        public int? DepartmentId { get; set; }
        public int? ValueIn { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int? NewSortingNo { get; set; }
        public int? Exist { get; set; }
        public int SectionGroup { get; set; }
        public string QbAccount { get; set; }
        public int? QbAccountid { get; set; }
        public string ValueInstr { get; set; }
        public string IsActivestr { get; set; }

        public List<DrpList> DeptList { get; set; }
        public List<DrpList> ValueInList { get; set; }
    }
    public class Payroll_Setting_Select
    {
        public int id { get; set; }
        public int? Exist { get; set; }
        public Nullable<int> StoreId { get; set; }
        public int BankAccountID { get; set; }
        public string BankAccountName { get; set; }
        public int VendorID { get; set; }
        public string VendorName { get; set; }
    }

    public class PayrollManualModel
    {
        public string FileName { get; set; }
        public int FileType { get; set; }
        public int StoreId { get; set; }
        public string TransactionNo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime EndCheckDate { get; set; }
        public decimal? CHILDSUPPORT { get; set; }
        public decimal? STDIS { get; set; }
        public decimal? CSDispNY { get; set; }
        public decimal? EENYPFL { get; set; }
        public decimal? GROSSPAY { get; set; }
        public decimal? AFLAC { get; set; }
        public decimal? K2401 { get; set; }
        public decimal? LIFEINS { get; set; }
        public decimal? SDINY { get; set; }
        public decimal? FederalUnemployment { get; set; }
        public decimal? MedicareEmployer { get; set; }
        public decimal? NewYorkUnemployment { get; set; }
        public decimal? PAYROLLTAXES { get; set; }
        public decimal? SocialSecurityEmployer { get; set; }
        public decimal? NewYorkMetroCommutZone { get; set; }
        public decimal? Employeepayments { get; set; }
        public decimal? NYReemplSvc { get; set; }
        public decimal? NJERWorkDev { get; set; }
        public decimal? NYMCTMTTDMSC { get; set; }
        public decimal? PayrollFee { get; set; }
        public decimal? NewJerseyUnemployment { get; set; }
        public decimal? NJDisability { get; set; }
        public decimal? Dental { get; set; }
        public decimal? LifeInsurance { get; set; }
        public decimal? EmployerSocialSecurity { get; set; }
        public decimal? GrossWages { get; set; }
        public decimal? EmployerMedicare { get; set; }
        public virtual ICollection<PayrollManualDetailsModel> PayrollManualDetailsModel { get; set; }
    }

    public class PayrollManualDetailsModel
    {
        public int PayrollDepartmentDetId { get; set; }
        public int PayrollDepartmentId { get; set; }
        public decimal? HOURLY { get; set; }
        public decimal? REGULAR { get; set; }
        public decimal? RETRO { get; set; }
        public decimal? SALARY { get; set; }
        public decimal? SPREADOFHOURS { get; set; }
        public decimal? TRAINING { get; set; }
        public decimal? HOLIDAYWORKED { get; set; }
        public decimal? OVERTIME { get; set; }
        public decimal? BONUS { get; set; }
        public decimal? CREDITCARDTIPS { get; set; }
        public decimal? HOLIDAY { get; set; }
        public decimal? SICK { get; set; }
        public decimal? VACATION { get; set; }
    }
}