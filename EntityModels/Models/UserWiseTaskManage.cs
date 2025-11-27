using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModels.Models
{
    public class UserWiseTaskManage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int TaskId { get; set; }
        [MaxLength(250)]
        public string TaskName { get; set; }
        [MaxLength]
        public string TaskDescription { get; set; }
        public int PriorityId { get; set; }
        public int AssignTo { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; } = false;
        public int Createdby { get; set; }
        public DateTime? CreatedOn { get; set; } = DateTime.Now;
        public DateTime? ModifiedOn { get; set; } = DateTime.Now;
        public int? InvoiceId { get; set; }
        public bool Isdeleted { get; set; } = false;
    }
}
