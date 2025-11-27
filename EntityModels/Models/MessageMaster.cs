using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModels.Models
{
    public class MessageMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int MessageId { get; set; }
        public string KeyNum { get; set; }
        public string ValueStr { get; set; }
        public string Description { get; set; }
        public string ModuleName { get; set; }
    }
}
