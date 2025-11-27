using EntityModels.HRModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface ILoginRepository
    {
        string CheckLogin(string username, string password);
        HREmployeeMaster LoginEmployee(string username, string password);

        void ModifyPasswordUpdate(int EmployeeId, string Password);
    }
}
