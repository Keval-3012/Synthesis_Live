using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface INotificationReportRepository
    {
        List<Invoice_Notification> getNotificationList(int UserId);
    }
}
