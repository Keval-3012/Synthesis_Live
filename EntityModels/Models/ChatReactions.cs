using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModels.Models
{
    [Table("ChatReactions")]
    public class ChatReactions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int ReactionId { get; set; }
        public int ChatId { get; set; }
        [ForeignKey("ChatId")]
        public virtual ChatMessenger ChatMessenger { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual UserMaster UserMasters { get; set; }
        public string ReactionEmoji { get; set; }
        public DateTime? Timestamp { get; set; }

    }
}
