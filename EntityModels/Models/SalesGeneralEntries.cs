using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("SalesGeneralEntries")]
    public class SalesGeneralEntries
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int SalesGeneralId { get; set; }

        public int StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual UserMaster UserMasters { get; set; }

        public DateTime? SalesDate { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? Status { get; set; }

        public int? SyncStatus { get; set; }

        public decimal TotalAmount { get; set; }

        public int? NoOfPos { get; set; }

        [MaxLength(50)]
        public string TxnId { get; set; }

        public int? IsSync { get; set; }
        [NotMapped]
        public string UserName { get; set; }

        [NotMapped]
        public string StoreName { get; set; }

        public virtual ICollection<SalesChildEntries> salesChildEntries { get; set; }
        public virtual ICollection<SalesOtherDeposite> SalesOtherDeposites { get; set; }
    }
    [Table("SalesChildEntries")]
    public class SalesChildEntries
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]

        public int SalesChildId { get; set; }

        public int SalesGeneralId { get; set; }
        [ForeignKey("SalesGeneralId")]
        public virtual SalesGeneralEntries salesGeneralEntries { get; set; }

        public int? GroupAccountId { get; set; }
        
        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual DepartmentMaster DepartmentMasters { get; set; }

        public decimal Amount { get; set; }

        [MaxLength(500)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string Memo { get; set; }

        public int? TypeId { get; set; }
    }

    [Table("SalesGeneralEntriesHistory")]
    public class SalesGeneralEntriesHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int SalesGeneralHistoryId { get; set; }

        public int StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual UserMaster UserMasters { get; set; }

        public DateTime? SalesDate { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? Status { get; set; }

        public int? SyncStatus { get; set; }

        public decimal TotalAmount { get; set; }
    }

    [Table("SalesOtherDeposite")]
    public class SalesOtherDeposite
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]

        public int SalesOtherDepositeId { get; set; }

        public int SalesGeneralId { get; set; }
        [ForeignKey("SalesGeneralId")]
        public virtual SalesGeneralEntries salesGeneralEntries { get; set; }

        public int StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        public int OtherDepositId { get; set; }
        [ForeignKey("OtherDepositId")]
        public virtual OtherDeposit OtherDeposits { get; set; }

        public DateTime? OtherDepositDate { get; set; }

        [MaxLength(50)]
        public string TxnId { get; set; }

        public int? IsSync { get; set; }

        public int? Status { get; set; }
    }

    public class GeneralEntriesDetail
    {
        public decimal? EditTotalAmount { get; set; }

        public List<JournalEntriesList> JournalEntriesLists { get; set; }
    }

    public class JournalEntriesList
    {
        public int id { get; set; }
        public DateTime? CloseOutDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? validationflag { get; set; }
        public string DeptName { get; set; }
        public string GroupName { get; set; }
        public int? noofpos { get; set; }
        public int? Typeid { get; set; }
        public string Memo { get; set; }
        public int? ChildSalesId { get; set; }
        public string CloseOutDateformat { get; set; }
        public string Storename { get; set; }
        public string Title { get; set; }
        public string TxnID { get; set; }
        public Nullable<int> IsSync { get; set; }
        public bool QBStatusField { get; set; }
        public int? StoreID { get; set; }
        public string NotConfigureAccount { get; set; }
        public int Sign { get; set; }
    }

    public class ChildOtherDepositeList
    {
        public int SalesOtherDepositeId { get; set; }
        public int SalesGeneralId { get; set; }
        public int StoreId { get; set; }
        public int OtherDepositId { get; set; }
        public DateTime? OtherDepositDate { get; set; }
        public string TxnId { get; set; }
        public int? IsSync { get; set; }
        public int? Status { get; set; }
        public string Memo { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string VendorListID { get; set; }
        public string DepartmentListId { get; set; }
        public string BankAccountID { get; set; }
        public string PaymentTypeID { get; set; }
    }

    public class JournalEntryDetail
    {
        public int SalesChildId { get; set; }
        public int SalesGeneralId { get; set; }
        public int? GroupAccountId { get; set; }
        public int? DepartmentId { get; set; }
        public decimal Amount { get; set; }
        public string Title { get; set; }
        public string Memo { get; set; }
        public int? TypeId { get; set; }
        public string DepartmentName { get; set; }
        public DateTime? SalesDate { get; set; }
        public string DepartmentListID { get; set; }
        public string EntityID { get; set; }
        public string EntityName { get; set; }
        public string EntityType { get; set; }
    }

    public class SalesGeneralEntriesSyncfusion
    {
        public int SalesGeneralId { get; set; }
        public int StoreId { get; set; }
        public int UserId { get; set; }
        public DateTime? SalesDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? Status { get; set; }
        public int? SyncStatus { get; set; }
        public decimal TotalAmount { get; set; }
        public int? NoOfPos { get; set; }
        public string TxnId { get; set; }
        public int? IsSync { get; set; }
        public string UserName { get; set; }
        public string StoreName { get; set; }
    }
}