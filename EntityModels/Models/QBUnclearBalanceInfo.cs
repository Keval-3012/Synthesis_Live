using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    public class QBUnclearBalanceInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int QBUnclearBalanceInfoId { get; set; }

        public int StoreID { get; set; }
        public decimal Balance { get; set; }
        public DateTime LastSyncDate { get; set; }

    }
}