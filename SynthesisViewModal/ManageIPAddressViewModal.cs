using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthesisViewModal
{
    public class ManageIPAddressViewModal
    {
        public string Message { get; set; }
        public string success { get; set; } = null;
        public string Error { get; set; } = null;
        public DateTime dt1 { get; set; }
        public DateTime dt2 { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int count { get; set; }
        public int IpAdressInfoID { get; set; }
        public string Location { get; set; }
        public int UserIdSt { get; set; }
        public string FromEmail { get; set; }
        public string Password { get; set; }
        public string ToEmail { get; set; }
        public string Port { get; set; }
        public string Smtp { get; set; }
        public string body { get; set; }
        public string startWeek { get; set; }
        public string endWeek { get; set; }
        public int days { get; set; }
    }

    public class UserDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
    public class LocationTypeVal
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
