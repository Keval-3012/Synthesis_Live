using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("QBDesktopConfiguration")]
    public class QBDesktopConfiguration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int QBDesktopId { get; set; }

        public int WebCompanyID { get; set; }

        public string QBCompanyPath { get; set; }

        [MaxLength(50)]
        public string UserName { get; set; }

        [MaxLength(50)]
        public string Password { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public DateTime? LastSyncDate { get; set; }

        public Guid OwnerID { get; set; }

        public Guid FileID { get; set; }

        [MaxLength(50)]
        public string AppName { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public int StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        public int Flag { get; set; }

        public string BankAccID { get; set; }

        [NotMapped]
        public string StoreName { get; set; }
    }
}