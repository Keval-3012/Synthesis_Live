using EntityModels.Models;
using NLog;
using Repository;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace SysnthesisRepo
{
    public class WebRoleProvider : RoleProvider
    {
        private readonly IWebRoleProviderRepository _webRoleProviderRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        public WebRoleProvider()
        {
            this._webRoleProviderRepository = new WebRoleProviderRepository(new DBContext());
        }
        public override string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            string[] result = null;
            try
            {
            result= _webRoleProviderRepository.GetRolesForUser(username);
            }
            catch (Exception ex)
            {
                logger.Error("WebRoleProvider - GetRolesForUser - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return result;

        }

        public string[] GetRoleForUserStoreWise(string username, int? storeid)
        {
            string[] result = null;
            try
            {
                _webRoleProviderRepository.GetRoleForUserStoreWise(username,storeid);
            }
            catch (Exception ex)
            {
                logger.Error("WebRoleProvider - GetRoleForUserStoreWise - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return result;
        }

        public bool GetroleUserRoleWise(int UserTypeId, string RoleName, int? storeid)
        {
            bool result = true;
            try
            {
                _webRoleProviderRepository.GetroleUserRoleWise(UserTypeId, RoleName, storeid);
            }
            catch (Exception ex)
            {
                logger.Error("WebRoleProvider - GetroleUserRoleWise - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return result;
        }
    }
}