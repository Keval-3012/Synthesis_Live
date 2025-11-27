using Microsoft.Owin.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace SyntesisApi.Models
{
    public class Common
    {
        public static string DataTableToJSON(DataTable table)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var rows = new System.Collections.ArrayList();

            foreach (DataRow dr in table.Rows)
            {
                var rowData = new System.Collections.Generic.Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    rowData.Add(col.ColumnName, dr[col]);
                }
                rows.Add(rowData);
            }

            return serializer.Serialize(rows);
        }


        public static async Task<string> PosttData(string UserName, string Password, string Url)
        {
            string Response = "", accessToken = "";
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, Url + "/token");
                var collection = new List<KeyValuePair<string, string>>();
                collection.Add(new KeyValuePair<string, string>("username", UserName+"`1"));
                collection.Add(new KeyValuePair<string, string>("password", Password));
                collection.Add(new KeyValuePair<string, string>("grant_type", "password"));
                var content = new FormUrlEncodedContent(collection);
                request.Content = content;
                var response = await client.SendAsync(request);
                Response = await response.Content.ReadAsStringAsync();

                JObject jsonObject = JObject.Parse(Response);
                // Get the value of "access_token"
                accessToken = (string)jsonObject["access_token"];
            }
            catch (Exception ex)
            {
            }
            return accessToken;
        }

        public static async Task<string> PosttData2(string UserName, string Password, string Url)
        {
            string Response = "", accessToken = "";
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, Url + "/token");
                var collection = new List<KeyValuePair<string, string>>();
                collection.Add(new KeyValuePair<string, string>("username", UserName));
                collection.Add(new KeyValuePair<string, string>("password", Password));
                collection.Add(new KeyValuePair<string, string>("grant_type", "password"));
                var content = new FormUrlEncodedContent(collection);
                request.Content = content;
                var response = await client.SendAsync(request);
                Response = await response.Content.ReadAsStringAsync();

                JObject jsonObject = JObject.Parse(Response);
                // Get the value of "access_token"
                accessToken = (string)jsonObject["access_token"];
            }
            catch (Exception ex)
            {
            }
            return accessToken;
        }
    }
}