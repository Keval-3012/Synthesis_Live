using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModels.Models
{
    public class UserWiseNoteManage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int NoteId { get; set; }
        [MaxLength(250)]
        public string NoteName { get; set; }
        [MaxLength]
        public string NoteDescription { get; set; }
        public int Createdby { get; set; }
        public DateTime? CreatedOn { get; set; } = DateTime.Now;
        public DateTime? ModifiedOn { get; set; } = DateTime.Now;
        public bool Isdeleted { get; set; } = false;
        public int? InvoiceId { get; set; }
    }
}
