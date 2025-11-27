using EntityModels.Models;
using NLog;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using SyntesisApi;

namespace Repository
{
    public class NotificationReportRepository : INotificationReportRepository
    {
        private DBContext _context;
        Logger logger = LogManager.GetCurrentClassLogger();
        public NotificationReportRepository(DBContext context)
        {
            _context = context;
        }
        public List<Invoice_Notification> getNotificationList(int UserId)
        {
            List<Invoice_Notification> NotificationList = new List<Invoice_Notification>();
            List<int> ContainUserList = new List<int>();
            try
            {
                var CurrentUserTypeLevel = _context.UserMasters.Where(s => s.UserId == UserId).Select(s => s.UserTypeMasters.LevelsApproverId).FirstOrDefault();
                var UserTypeList = _context.UserTypeModuleApprovers.Where(s => s.LevelsApproverId == CurrentUserTypeLevel).Select(s => s.UserTypeId).ToList();
                var UserList = _context.UserMasters.Where(s => UserTypeList.Contains(s.UserTypeId)).Select(s => new { s.UserId, s.UserName }).ToList();
                ContainUserList = UserList.Select(s => s.UserId).ToList();

                var store = _context.Database.SqlQuery<int>("SP_GetStore_ForDashboard @Mode={0},@UserTypeId={1}", "GetStore_ForNotification", UserModule.getUserTypeId());

                NotificationList = _context.Invoices.Where(s => s.StatusValue == InvoiceStatusEnm.Pending && store.Contains(s.StoreId) && ContainUserList.Contains(s.CreatedBy.Value)).Select(s => new { InvoiceId = s.InvoiceId, StoreName = s.StoreMasters.Name, StoreId = s.StoreId, s.CreatedBy, s.NotificationColor, s.InvoiceNumber, s.InvoiceDate }).ToList().Select(k => new Invoice_Notification
                {
                    StoreName = k.StoreName,
                    StoreId = k.StoreId,
                    InvoiceId = k.InvoiceId,
                    NotificationColor = k.NotificationColor,
                    InvoiceNumber = k.InvoiceNumber,
                    InvoiceDate = k.InvoiceDate,
                    UserName = UserList.Where(n => n.UserId == k.CreatedBy).FirstOrDefault().UserName
                }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("NotificationReportRepository - getNotificationList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return NotificationList;
        }
    }
}
