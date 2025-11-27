using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModels.Models
{
    [Table("ApiKeyConfiguartion")]

    public class ApiKeyConfiguartion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int APIKeyId { get; set; }

        [MaxLength(200)]
        public string APIKeyName { get; set; }
        public bool IsDeleted { get; set; }
        public int Status { get; set; }
        public int BucketSize { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; } = false;
    }
}
