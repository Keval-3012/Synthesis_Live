using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("DepartmentMaster")]
    public class DepartmentMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = " ")]
        [Display(Name = "Department Name")]
        [MaxLength(300)]
        public string DepartmentName { get; set; }

        [MaxLength(50)]
        public string AccountNumber { get; set; }

        public int AccountTypeId { get; set; }
        [ForeignKey("AccountTypeId")]
        public virtual AccountTypeMaster AccountTypeMasters { get; set; }

        public int AccountDetailTypeId { get; set; }
        [ForeignKey("AccountDetailTypeId")]
        public virtual AccountDetailTypeMaster AccountDetailTypeMasters { get; set; }

        public int? StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        [MaxLength(100)]
        public string IsSubAccount { get; set; }

        public int? IsSync { get; set; }
        public DateTime? SyncDate { get; set; }

        public string Description { get; set; }

        [MaxLength(50)]
        public string ListId { get; set; }

        [MaxLength(50)]
        public string DListId { get; set; }

        public Boolean IsActive { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public int? ModifiedBy { get; set; }

        //Added by Dani on 02-04-2025
        public DateTime? LastModifiedOn { get; set; }

        [NotMapped]
        public string[] MultiStoreId { get; set; }

        [NotMapped]
        public string AccountType { get; set; }

        [NotMapped]
        public string AccountDetailType { get; set; }

        [NotMapped]
        public string QBDetailType { get; set; }
        [NotMapped]
        public string Status { get; set; }

        public virtual ICollection<InvoiceDepartmentDetail> InvoiceDepartmentDetails { get; set; }
        public virtual ICollection<Configuration> Configurations { get; set; }
        public virtual ICollection<ConfigurationGroup> ConfigurationGroups { get; set; }
        public virtual ICollection<Departmentconfiguration> Departmentconfigurations { get; set; }
        public virtual ICollection<SalesChildEntries> salesChildEntries { get; set; }
        public virtual ICollection<OtherDeposit> OtherDeposits { get; set; }
        public virtual ICollection<OtherDepositeSetting> OtherDepositeSettings { get; set; }
        public virtual ICollection<PayrollCashAnalysis> PayrollCashAnalyses { get; set; }
        public virtual ICollection<PayrollBankAccount> PayrollBankAccounts { get; set; }
        public virtual ICollection<RightsStore> RightsStores { get; set; }
        public virtual ICollection<ExpenseCheck_Setting> ExpenseCheck_Settings { get; set; }
        public virtual ICollection<ExpenseCheck> ExpenseCheck { get; set; }
        public virtual ICollection<ExpenseCheckDetail> ExpenseCheckDetails { get; set; }

        public virtual ICollection<VendorDepartmentRelationMaster> VendorDepartmentRelationMasters { get; set; }
        //public virtual ICollection<HomeExpenseWeeklySalesSetting> HomeExpenseDepartmentMasters { get; set; }
    }

    public class DepartmentMasterList
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string AccountType { get; set; }
        public string Status { get; set; }
        public string StoreName { get; set; }
        public string AccountDetailType { get; set; }
        
    }
    public class IsSubAccountsSelect
    {
        public string ListId { get; set; }
        public string DepartmentName { get; set; }
    }

    public class DepartmetItemList
    {
        public string DepartmentName { get; set; }
    }
}