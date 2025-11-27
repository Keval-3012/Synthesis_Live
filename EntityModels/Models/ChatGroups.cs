using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModels.Models
{
    [Table("ChatGroups")]
    public class ChatGroups
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int Createdby { get; set; }
        public DateTime CreatedDate { get; set; }
        public virtual ICollection<ChatMessenger> ChatMessengers { get; set; }

        public virtual ICollection<ChatGroupMembers> chatGroupMembers { get; set; }
    }
}
