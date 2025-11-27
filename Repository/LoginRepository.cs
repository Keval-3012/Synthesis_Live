using EntityModels.HRModels;
using EntityModels.Models;
using Repository.IRepository;
using SynthesisViewModal.HRViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class LoginRepository : ILoginRepository
    {
        private DBContext _context;
        public LoginRepository(DBContext context)
        {
            _context = context;
        }

        public string CheckLogin(string username, string password)
        {
            string Message = "";
            try
            {
                Message = _context.Database.SqlQuery<string>("SP_HREmployee_Login @Mode = {0},@UserName = {1},@Password = {2}", "CheckLogin", username, password).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw;
            }
            return Message;
        }

        public HREmployeeMaster LoginEmployee(string username, string password)
        {
            HREmployeeMaster master = new HREmployeeMaster();
            try
            {
                master = _context.Database.SqlQuery<HREmployeeMaster>("SP_HREmployee_Login @Mode = {0},@UserName = {1},@Password = {2}", "EmpData", username, password).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw;
            }
            return master;
        }

        public void ModifyPasswordUpdate(int EmployeeId, string Password)
        {
            try
            {
                _context.Database.ExecuteSqlCommand("SP_HREmployee_Login @Mode = {0},@EmployeeId = {1},@Password = {2}", "UpdateFirstLogin", EmployeeId, Password);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
