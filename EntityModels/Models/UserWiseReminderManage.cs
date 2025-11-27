using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModels.Models
{
    public class UserWiseReminderManage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int ReminderId { get; set; }
        [MaxLength(250)]
        public string ReminderName { get; set; }
        [MaxLength]
        public string ReminderDescription { get; set; }
        public DateTime ReminderDate { get; set; }
        public int Createdby { get; set; }
        public DateTime? CreatedOn { get; set; } = DateTime.Now;
        public DateTime? ModifiedOn { get; set; } = DateTime.Now;
        public bool Isdeleted { get; set; } = false;
        public bool ForWhatsAppNotification { get; set; } = false;
    }
}
