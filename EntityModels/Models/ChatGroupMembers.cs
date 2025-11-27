using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModels.Models
{
    [Table("ChatGroupMembers")]
    public class ChatGroupMembers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int GroupMemberId { get; set; }
        public int GroupId { get; set; }
        [ForeignKey("GroupId")]
        public virtual ChatGroups ChatGroups { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual UserMaster UserMasters { get; set; }
        public bool IsAdmin { get; set; }
    }
}
