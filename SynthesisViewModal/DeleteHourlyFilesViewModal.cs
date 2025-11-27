using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisViewModal
{
    public class DeleteHourlyFilesViewModal
    {
        public int istoreID { get; set; }
        public int storeid { get; set; }
        public string StatusMessage { get; set; } = "";
        public string DeleteMessage { get; set; } = "";
        public string StatusMessageString { get; set; } = "";
        public string FileName { get; set; }
        public DateTime? StartDate { get; set; }
        public int HSequence { get; set; }
        public int Id { get; set; }
        public DateTime? start_date { get; set; }
    }
}
