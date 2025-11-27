using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("QBOnlineConfiguration")]
    public class QBOnlineConfiguration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int QBOnlineId { get; set; }

        public string RealmId { get; set; }

        public string ClientId { get; set; }

        public string ClientSecretKey { get; set; }

        public string QBToken { get; set; }

        public string QBRefreshToken { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public DateTime? LastSyncDate { get; set; }

        public bool IsActive { get; set; }

        public int StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        public int Flag { get; set; }

        public int BankAccID { get; set; }

        public int IsTokenSuspend { get; set; }

        public string CompanyName { get; set; }

        [NotMapped]
        public string LogStatus { get; set; }
    }

    public class QBOnlineConfiguration1
    {
        public int StoreId { get; set; }
        public string Name { get; set; }
        public string QBType { get; set; }
        public int QBOnlineFlag { get; set; }
        public int QBWebFlag { get; set; }

        public string Flag { get; set; }
        public string DesktopTrue { get; set; }
    }
}