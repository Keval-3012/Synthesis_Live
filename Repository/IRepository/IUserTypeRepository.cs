using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IUserTypeRepository
    {
        Task<UserTypeMaster> Register(UserTypeMaster userTypeMaster);
        List<UserTypeMaster> UserTypeBindData();
        void SaveUserDetails(UserTypeMaster UserTypeMaster);
        Task<UserTypeMaster> GetFindUserTypeMasters(int? id);
        List<LevelsApprover> GetUserTypeMasters(UserTypeMaster userTypeMaster);
        void UpdateUserTypeMasterDetails(UserTypeMaster userTypeMaster);
        UserTypeMaster DeleteUserDetails(int id);
    }
}
