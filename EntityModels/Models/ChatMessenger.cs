using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModels.Models
{
    [Table("ChatMessenger")]
    public class ChatMessenger
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int ChatId { get; set; }
        public long ConversationId { get; set; }
        public int SenderId { get; set; }
        [ForeignKey("SenderId")]
        public virtual UserMaster UserMastersSender { get; set; }
        public int? ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        public virtual UserMaster UserMastersReceiver { get; set; }
        public int? GroupId { get; set; }
        [ForeignKey("GroupId")]
        public virtual ChatGroups ChatGroups { get; set; }
        public string Message { get; set; }
        public DateTime? Timestamp { get; set; }
        public string DeletedByUsers { get; set; }
        public int? DeletedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string ConversationType { get; set; }
        public string UploadFile { get; set; }
        public string IsRead { get; set; }

        public virtual ICollection<ChatReactions> chatReactions { get; set; }
    }
}
