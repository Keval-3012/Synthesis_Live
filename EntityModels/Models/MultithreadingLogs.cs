using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModels.Models
{
    [Table("MultithreadingInvoiceLogs")]

    public class MultithreadingInvoiceLogs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int MultithreadingInvoiceLogId { get; set;}
        public int? UploadPdfAutomationId { get; set; }
        public string UploadStartFinish { get; set;}
        public DateTime? UploadStartFinishDate { get; set;}
        public string MovetoConsole { get; set;}
        public DateTime? MovetoConsoleDate { get; set;}
        public string SentAWSExtract { get; set;}
        public DateTime? SentAWSExtractDate { get;set;}
        public string ReceivedAWSExtract { get; set; }
        public DateTime? ReceivedAWSExtractDate { get; set; }
        public string JSONtoCHATGPT { get; set; }
        public DateTime? JSONtoCHATGPTDate { get; set; }
        public string ResponseCHATGPT { get; set; }
        public DateTime? ResponseCHATGPTDate { get; set; }
        public string InvoiceApproveStatus { get; set; }
        public DateTime? InvoiceApproveStatusDate { get; set; }
        public string InvoiceCreatedStatus { get; set; }
        public DateTime? InvoiceCreatedStatusDate { get; set; }

    }
}
