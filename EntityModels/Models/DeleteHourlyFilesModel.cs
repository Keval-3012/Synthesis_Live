using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    public class DeleteHourlyFilesModel
    {
        public string FileName { get; set; }
        public DateTime? StartDate { get; set; }
        public int HSequence { get; set; }
        public int Id { get; set; }
    }
}