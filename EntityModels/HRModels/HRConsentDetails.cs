using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModels.HRModels
{
    [Table("HR_ConsentDetails")]
    public class HRConsentDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int ConsentDetailId { get; set; }

        public int ConsentId { get; set; }
        [ForeignKey("ConsentId")]
        public virtual HRConsentMaster HRConsentMaster { get; set; }

        public Language? LanguageId { get; set; }

        public InputTypes? TypeId { get; set; }

        public string Description { get; set; }

        public DateTime? CreatedOn { get; set; } = DateTime.Now;

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; } = DateTime.Now;

        public int? ModifiedBy { get; set; }
    }
}
