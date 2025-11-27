using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IWebRoleProviderRepository
    {
        string[] GetRolesForUser(string username);
        string[] GetRoleForUserStoreWise(string username, int? storeid);
        bool GetroleUserRoleWise(int UserTypeId, string RoleName, int? storeid);
    }
}
