using Newtonsoft.Json.Linq;
using SyntesisApi.BAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml.Linq;

namespace SyntesisApi.Models
{
    public class UserRepository
    {
        public User ValidateUser(string username, string password)
        {
            User user = new User();
            DataTable Dt = new DataTable();
            Dt = new BALAccount().GetData(username);
            if (username.Contains("`1"))
            {
                username = username.Replace("`1", "");
            }

            if (Dt.Rows.Count > 0)
            {
                if (!String.IsNullOrEmpty(Dt.Rows[0]["Password"].ToString()))
                {
                    if (Dt.Rows[0]["Password"].ToString().ToLower() == password.Trim().ToLower())
                    {
                        user.UserId = Convert.ToInt32(Dt.Rows[0]["UserId"].ToString());
                        user.UserName = username;
                    }
                    else
                    {
                        user = null;
                    }
                }
            }
            else
            {
                user = null;
            }
            return user;
        }
    }
}