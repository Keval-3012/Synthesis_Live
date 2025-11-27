using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("QBWebhook")]
    public class QBWebhook
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int QBWebhookId { get; set; }

        
        public int QBOnlineId { get; set; }

        public string RealmId { get; set; }

        [MaxLength(50)]
        public string Type { get; set; }

        [MaxLength(50)]
        public string TXNId { get; set; }

        public string Operation { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}