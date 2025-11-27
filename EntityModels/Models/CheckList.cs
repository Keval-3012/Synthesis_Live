using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("CheckList")]
    public class CheckList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int ChecklistId { get; set; }
        [Column(TypeName= "nvarchar")]
        [MaxLength(50)]
        public string Is_cleared { get; set; }
        [Column(TypeName = "nvarchar")]
        [MaxLength(500)]
        public string Account_name { get; set; }
        public int AccountId { get; set; }
        public DateTime Tx_Date { get; set; }
        [Column(TypeName = "nvarchar")]
        [MaxLength(500)]
        public string Doc_no { get; set; }
        [Column(TypeName = "nvarchar")]
        [MaxLength(500)]
        public string EntityName { get; set; }
        public int EntityId { get; set; }
        [Column(TypeName = "nvarchar")]
        [MaxLength(100)]
        public string Pmt_method { get; set; }
        [Column(TypeName = "nvarchar")]
        [MaxLength(100)]
        public string Txn_type { get; set; }
        public int StoreId { get; set; }
        public int MailSent { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? TxnId { get; set; }

        public int? UserID { get; set; }
        public DateTime? UpdateDate { get; set; }
    }

    public class CheckIndexList
    {
        //public int ID { get; set; }
        //public int? StoreID { get; set; }
        public int ChecklistId { get; set; }
        public DateTime Tx_Date { get; set; }
        public string Doc_no { get; set; }
        public string Account_name { get; set; }
        public string Txn_type { get; set; }
        public string EntityName { get; set; }
        public decimal Amount { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string Is_cleared { get; set; }
        public int MailSent { get; set; }
        public string UpdateDate { get; set; }
        public string MailsentBool { get; set; }

        public string FirstName { get; set; }
    }
}