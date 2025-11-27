using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisQBOnline.BAL
{
    public class VendorMaster
    {
        public string ID { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        public string DisplayName { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        public string PrintOnCheckas { get; set; }
        public string Title { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Suffix { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string IsActive { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Fax { get; set; }
        public string Website { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Notes { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string SyncToken { get; set; }
        public string AcctNum { get; set; }
    }
}
