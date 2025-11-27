using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisQBOnline.BAL
{
    public class AccountMaster
    {
        public string ID { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        public string Department { get; set; }
        public string Description { get; set; }
        public string SyncToken { get; set; }
        public string AccountType { get; set; }
        public string DetailType { get; set; }
        public string AccountNumber { get; set; }
        public string ParentRefID { get; set; }
        public string SubAccount { get; set; }
        public string TaxCode { get; set; }
        public string IsActive { get; set; }
    }
}
