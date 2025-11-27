using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisViewModal
{
    public class ddllistViewModal
    {
        public Nullable<bool> IsActive { get; set; }
        public string Store { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
        public int selectedvalue { get; set; }
        public bool selected_value { get; set; }
        public string ListID { get; set; }
    }
    public class DefaultButtonModels
    {
        public string content { get; set; }
        public bool isPrimary { get; set; }
    }
}
