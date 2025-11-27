using EntityModels.Migrations;
using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisViewModal
{
    public class InvoicesViewModel
    {
        public string Message {  get; set; }    
        public string idval { get; set; }    
        public string InvoiceNumber { get; set; }
        public string InvDate { get; set; }
    }
}
