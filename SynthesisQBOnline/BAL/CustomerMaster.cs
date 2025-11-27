using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisQBOnline.BAL
{
    public class CustomerMaster
    {
        public string ID { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        public string DisplayName { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        public string LastName { get; set; }
        public string PrimaryEmailAddr { get; set; }
        public string FirstName { get; set; }
        public string PrimaryPhone { get; set; }
        public string Active { get; set; }

        public string PrintOnCheckName { get; set; }

        public string MiddleName { get; set; }
        public string CompanyName { get; set; }
        public string Notes { get; set; }
        public string Balance { get; set; }
        public string SyncToken { get; set; }


        public string BAddress1 { get; set; }
        public string BAddress2 { get; set; }
        public string BCity { get; set; }
        public string BState { get; set; }
        public string BCountry { get; set; }
        public string BZipCode { get; set; }

        public string SAddress1 { get; set; }
        public string SAddress2 { get; set; }
        public string SCity { get; set; }
        public string SState { get; set; }
        public string SCountry { get; set; }
        public string SZipCode { get; set; }
    }
}
