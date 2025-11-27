using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    public class BankAccountSettingModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int BankAccountID { get; set; }

        public string ItemID { get; set; }

        public string AccessToken { get; set; }

        public int StoreID { get; set; }

        public string AccountNo { get; set; }

        public decimal? Balance { get; set; }

        public DateTime? LastSyncDate { get; set; }


        public DateTime? CreatedOn { get; set; }

        public int? CreatedBy { get; set; }
        [NotMapped]
        public string StoreName { get; set; }
    }
    public class BankAccountSettingDetail
    {
        public int BankAccountID { get; set; }
        public string ItemID { get; set; }
        public int StoreID { get; set; }
        public string AccountNo { get; set; }
        public string StoreName { get; set; }
    }
}