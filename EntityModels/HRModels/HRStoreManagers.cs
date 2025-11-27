using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModels.HRModels
{
    [Table("HR_StoreManagers")]
    public class HRStoreManagers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int StoreManagerId { get; set; }
        public int StoreId { get; set; }
        public string FirstName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; } = DateTime.Now;
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; } = DateTime.Now;
        public int? ModifiedBy { get; set; }
        //[NotMapped]
        //public string StoreName { get; set; }
    }
    public class HRStoreList
    {
        public int StoreManagerId { get; set; }
        public int StoreId { get; set; }
        public string FirstName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; } = DateTime.Now;
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; } = DateTime.Now;
        public int? ModifiedBy { get; set; }
        public string StoreName { get; set; }
    }
}
