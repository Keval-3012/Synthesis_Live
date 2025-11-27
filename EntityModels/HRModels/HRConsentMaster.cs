using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModels.HRModels
{
    [Table("HR_ConsentMaster")]
    public class HRConsentMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int ConsentId { get; set; }

        public int FormTypeId { get; set; }
        [ForeignKey("FormTypeId")]
        public virtual HRFormTypeMaster HRFormTypeMaster { get; set; }

        public DateTime? CreatedOn { get; set; } = DateTime.Now;

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; } = DateTime.Now;

        public int? ModifiedBy { get; set; }

        public virtual ICollection<HRConsentDetails> HRConsentDetails { get; set; }
    }
}
