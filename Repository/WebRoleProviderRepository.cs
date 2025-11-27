using EntityModels.Models;
using NLog;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Repository
{
    public class WebRoleProviderRepository : IWebRoleProviderRepository
    {
        private DBContext _context;
        Logger logger = LogManager.GetCurrentClassLogger();

        public WebRoleProviderRepository(DBContext context)
        {
            _context = context;
        }
        public string[] GetRolesForUser(string username)
        {
            string[] result = null;
            try
            {
                var UserData = _context.UserMasters.Where(s => s.UserName.ToLower() == username.ToLower()).FirstOrDefault();
                if (UserData.UserTypeMasters.UserType == "Administrator")
                {
                     result = _context.userRoles.Where(s => s.UserTypeId == UserData.UserTypeId).Select(s => s.Role).ToArray();
                }
                else
                {
                    int? SID = null;
                    if (HttpContext.Current.Session["storeid"] != null && HttpContext.Current.Session["storeid"] != "0")
                    {
                        SID = Convert.ToInt32(HttpContext.Current.Session["storeid"].ToString());
                    }
                    if (SID != null && SID > 0)
                    {
                        result = _context.userRoles.Where(s => s.UserTypeId == UserData.UserTypeId && s.StoreId == SID).Select(s => s.Role).ToArray();
                    }
                    else
                    {
                        result = _context.userRoles.Where(s => s.UserTypeId == UserData.UserTypeId).Select(s => s.Role).ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("WebRoleProviderRepository - GetUserMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return result;
        }
        public string[] GetRoleForUserStoreWise(string username, int? storeid)
        {
            string[] result = null;
            try
            {
                var UserData = _context.UserMasters.Where(s => s.UserName.ToLower() == username.ToLower()).FirstOrDefault();
                result = _context.userRoles.Where(s => s.UserTypeId == UserData.UserTypeId && s.StoreId == storeid).Select(s => s.Role).ToArray();
            }
            catch (Exception ex)
            {
                logger.Error("WebRoleProviderRepository - GetRoleForUserStoreWise - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return result;
        }
        public bool GetroleUserRoleWise(int UserTypeId, string RoleName, int? storeid)
        {
            bool result = true;
            try
            {
                var Result = _context.userRoles.Where(s => s.UserTypeId == UserTypeId && s.Role == RoleName && s.StoreId == storeid && s.ModuleId == 1).Select(s => s.Role).FirstOrDefault();
                if (Result == null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("WebRoleProviderRepository - GetroleUserRoleWise - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return result;
        }
    }
}
