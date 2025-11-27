using EntityModels.HRModels;
using SynthesisViewModal.HRViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IHREmployeeTrainingRepository
    {
        void SaveLanguageselection(HREmployeeMaster master);

        HREmployeeMaster HREmployeeMaster(int EmployeeId);
        void UpdateLastSlide(int UserId, string LastSlideName);
    }
}
