using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisViewModal.HRViewModel
{
    public class HRTrainingViewModal
    {
        public string EmployeeUserName { get; set; }
        public string StoreNikeName { get; set; }
        public int EmployeeId { get; set; }
        public string DOB { get; set; }
        public string HireDate { get; set; }
        public string SSN { get; set; }
        public string TrainingFilePath { get; set; }
        public string TrainingContent { get; set; }
        public DateTime TrainingCompletedDateTime { get; set; }
    }

    public class CompleteyTraining
    {
        public string Data { get; set; }
        public int? EmployeeId { get; set; }
        public string TrainingDate { get; set; }
        public string TrainingTime { get; set; }
        public string LastSlidename { get; set; }
    }

    public class ResetTraining
    {
        public int EmployeeId { get; set; }
    }
    public class ResponseModel
    {
        public string responseStatus { get; set; }
        public DataTable responseData { get; set; }
        public string message { get; set; }
    }
}
