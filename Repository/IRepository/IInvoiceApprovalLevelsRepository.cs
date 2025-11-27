using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IInvoiceApprovalLevelsRepository
    {
        List<LevelsApprover> GetBindData();
        bool CheckExist(LevelsApprover modelObj);
        void SaveUserDetails(LevelsApprover LevelsApprover);
        void UpdateUserDetails(LevelsApprover LevelsApprover);
        Task<LevelsApprover> GetFindApproveMaster(int? id);
        LevelsApprover DeleteUserDetails(int id);
    }
}
