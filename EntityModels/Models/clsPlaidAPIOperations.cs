//using Newtonsoft.Json.Linq;
//using RestSharp;
//using SynthesisCF.Models;
//using System;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.Configuration;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using Utility;

namespace EntityModels.Models
{
    public class clsPlaidAPIOperations
    {
        public static string GetBankBalance(string AccessToken)
        {
            string Resonse = "";                  
            try
            {
                //ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                var client = new RestClient("https://production.plaid.com/transactions/get");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");                
                request.AddParameter("application/json", "{\"client_id\":\"" + ConfigurationManager.AppSettings["ClientIDBank"].ToString() + "\",\"secret\":\"" + ConfigurationManager.AppSettings["SecretBank"].ToString() + "\",\"access_token\":\"" + AccessToken + "\",\"start_date\":\"" + Common.GetDateFormat(DateTime.Now, -6) + "\",\"end_date\":\"" + Common.GetDateFormat(DateTime.Now, 0) + "\"}", ParameterType.RequestBody);
                //request.AddParameter("Accept", "application/json"); 
                IRestResponse response = client.Execute(request);
                Resonse= response.Content;
            }
            catch (Exception ex)
            {
                Common.WriteErrorLogs("Retrieve Bank Balance. Message:" + ex.Message);
            }
            return Resonse;
        }

        public static void ExtractResponse(string Response,ref NameValueCollection nvcData)
        {    
            nvcData.Clear();
            try
            {                
                JObject Obj = JObject.Parse(Response);
                Obj = JObject.Parse(Obj["accounts"][0].ToString());
                
                nvcData.Add("AccNo", Obj["mask"].ToString());
                nvcData.Add("Balance", Obj["balances"]["current"].ToString());

            }
            catch (Exception ex)
            {
                Common.WriteErrorLogs("Extract Bank Response. Message:" + ex.Message);
            }            
        }

    }
}
