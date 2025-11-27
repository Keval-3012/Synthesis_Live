using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("GroupWiseStateStore")]
    public class GroupWiseStateStore
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int GroupWiseStateStoreId { get; set; }

        public string GroupName { get; set; }

        public string StoreName { get; set; }        

        public int? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [NotMapped]
        public string[] MuiltiStoreAccess { get; set; }        
    }

    public class GroupWiseStateStoreSelect
    {
        public int GroupWiseStateStoreId { get; set; }

        public string GroupName { get; set; }

        public string StoreName { get; set; }        
    }
}
