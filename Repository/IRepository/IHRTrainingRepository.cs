using EntityModels.HRModels;
using SynthesisViewModal.HRViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IHRTrainingRepository
    {
        List<HRTrainingViewModal> GetTrainingList(int store_idval);
        void ResetTrainingCheck(List<int> selectedIds, int UserId);
    }
}
