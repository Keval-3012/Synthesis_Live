using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SyntesisApi.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
    public class CheckLogin
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FCMTokenApp { get; set; }
        public string PlayerId { get; set; }
        public string DeviceType { get; set; }        
    }
}