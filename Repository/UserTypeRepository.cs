using EntityModels.Models;
using NLog;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class UserTypeRepository : IUserTypeRepository
    {
        private DBContext _context;
        Logger logger = LogManager.GetCurrentClassLogger();

        public UserTypeRepository(DBContext context)
        {
            _context = context;
        }
        public async Task<UserTypeMaster> Register(UserTypeMaster userTypeMaster)
        {
            try
            {
                userTypeMaster.IsActive = true;
                userTypeMaster.IsViewInvoiceOnly = true;
                _context.UserTypeMasters.Add(userTypeMaster);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.Error("UserTypeRepository - Register - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return userTypeMaster;
        }
        public List<UserTypeMaster> UserTypeBindData()
        {
            List<UserTypeMaster> UserTypeMaster = new List<UserTypeMaster>();
            try
            {
                UserTypeMaster = _context.UserTypeMasters.Where(s => s.UserType != "Administrator").Select(s => new { UserTypeId = s.UserTypeId, Name = s.GroupMasters.Name, LevelName = s.LevelsApprovers != null ? s.LevelsApprovers.LevelName : "", LevelSortName = s.LevelsApprovers != null ? s.LevelsApprovers.LevelSortName : "", UserType = s.UserType }).ToList().Select(s => new UserTypeMaster { UserTypeId = s.UserTypeId, Name = s.Name, LevelName = s.LevelName, LevelSortName = s.LevelSortName, UserType = s.UserType }).AsQueryable().OfType<UserTypeMaster>().ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserTypeRepository - Register - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return UserTypeMaster;
        }
        public async void SaveUserDetails(UserTypeMaster UserTypeMaster)
        {
            try
            {
                _context.UserTypeMasters.Add(UserTypeMaster);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.Error("UserTypeRepository - SaveUserDetails - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public async Task<UserTypeMaster> GetFindUserTypeMasters(int? id)
        {
            UserTypeMaster UserTypeMaster = new UserTypeMaster();
            try
            {
                UserTypeMaster = await _context.UserTypeMasters.FindAsync(id);
            }
            catch (Exception ex)
            {
                logger.Error("UserTypeRepository - GetFindUserMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return UserTypeMaster;
        }
        public List<LevelsApprover> GetUserTypeMasters(UserTypeMaster userTypeMaster)
        {
            List<LevelsApprover> LevelsApprover = new List<LevelsApprover>();
            try
            {
                LevelsApprover = _context.levelsApprovers.Where(s => s.IsActive == true).ToList();                
                //LevelsApprover = _context.levelsApprovers.Where(s => s.IsActive == true).Select(s => new LevelsApprover { LevelsApproverId = s.LevelsApproverId, LevelSortName = s.LevelSortName }).OrderBy(o => o.LevelSortName).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("UserTypeRepository - GetUserTypeMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return LevelsApprover;
        }
        public void UpdateUserTypeMasterDetails(UserTypeMaster userTypeMaster)
        {
            try
            {
                _context.Entry(userTypeMaster).State = EntityState.Modified;
                  _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.Error("UserTypeRepository - UpdateUserTypeMasterDetails - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public UserTypeMaster DeleteUserDetails(int id)
        {
            UserTypeMaster UserTypeMaster = new UserTypeMaster();
            try
            {
                UserTypeMaster = _context.UserTypeMasters.Find(id);
                _context.UserTypeMasters.Remove(UserTypeMaster);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("UserTypeRepository - DeleteUserDetails - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return UserTypeMaster;
        }
    }
}
