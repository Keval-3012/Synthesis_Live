using EntityModels.Models;
using SynthesisViewModal.HRViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface ISynthesisApiRepository
    {
        Task<string> PosttData(string UserName, string Password);
        void CreateLog(clsActivityLog clsActivityLog);
        clsActivityLog GetActivityDetails(string controllerName, string actionName);
        void LogEntry(string controller, string action, bool? IsReload = null);
        Task<DataTable> GetUserLogDetails(UserLogDetails UserLogDetails);
        Task<string> GetUser();
        Task<DataSet> GetModuleandAction();
        Task<DataTable> GetUserLogDetailsNew(UserLogDetails UserLogDetails);
        Task<DataTable> GetUserLogDetailsNewChild(UserLogDetails UserLogDetails);
		Task<string> PosttDataEmployeeLogin(string UserName, string Password);
        Task<string> CompleteTraining(CompleteyTraining data);
        Task<string> ResetTraining(ResetTraining data);
        Task<string> GetNoteTitleFromGPTAsync(string content);
    }
}
