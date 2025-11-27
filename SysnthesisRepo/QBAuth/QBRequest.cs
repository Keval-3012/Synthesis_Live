using Newtonsoft.Json;
using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;

namespace SysnthesisRepo.QBAuth
{
    public class QBRequest
    {
        public static QBOnlineConfiguration AuObj = new QBOnlineConfiguration();
        public static string LiveQBConnectionOperation_GET(string Method, string Query)
        {

            string Results = "";
            HttpWebRequest WebRequestObject = null;
            HttpWebResponse WebResponseObject = null;
        ReRun:
            try
            {
                string uri = string.Format("https://{0}/v3/company/{1}/query?query={2}", ConfigurationManager.AppSettings["qboBaseUrl"].ToString(), AuObj.RealmId, Query);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                WebRequestObject = WebRequest.Create(uri) as HttpWebRequest;
                WebRequestObject.Method = "GET";
                WebRequestObject.Accept = "application/xml";
                WebRequestObject.Headers.Add(string.Format("Authorization: Bearer {0}", AuObj.QBToken));
                WebRequestObject.ContentType = "application/xml";
                WebResponseObject = (HttpWebResponse)WebRequestObject.GetResponse();
                using (Stream data = WebResponseObject.GetResponseStream())
                    Results = "OK_" + new StreamReader(data).ReadToEnd();

            }
            catch (WebException ex2)
            {
                using (WebResponse response = ex2.Response)
                {
                    var httpResponse = (HttpWebResponse)response;

                    using (Stream data = response.GetResponseStream())
                    {
                        StreamReader sr2 = new StreamReader(data);
                        Results = "Error:" + sr2.ReadToEnd();

                        if (Results.Contains("Token expired") && Results.Contains("AuthenticationFailed"))
                        {
                            if (PerformRefreshToken(AuObj.QBRefreshToken.ToString()))
                            {
                                goto ReRun;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("bad"))
                {
                }
                AuObj.LogStatus = "Bad Request";
            }
            finally
            {
                try
                {
                    WebResponseObject.Close();
                    WebRequestObject.Abort();
                }
                catch
                {
                }
            }
            return Results;
        }

        public static bool PerformRefreshToken(string refresh_token)
        {
            bool PerformRefreshToken = false;
            string access_token = "";
            string cred = string.Format("{0}:{1}", QBRequest.AuObj.ClientId.ToString(), QBRequest.AuObj.ClientSecretKey.ToString());
            string enc = Convert.ToBase64String(Encoding.ASCII.GetBytes(cred));
            string basicAuth = string.Format("{0} {1}", "Basic", enc);

            // build the  request
            string refreshtokenRequestBody = string.Format("grant_type=refresh_token&refresh_token={0}",
                refresh_token
                );

            // send the Refresh Token request
            HttpWebRequest refreshtokenRequest = (HttpWebRequest)WebRequest.Create("https://oauth.platform.intuit.com/oauth2/v1/tokens/bearer");
            refreshtokenRequest.Method = "POST";
            refreshtokenRequest.ContentType = "application/x-www-form-urlencoded";
            refreshtokenRequest.Accept = "application/json";
            //Adding Authorization header
            refreshtokenRequest.Headers[HttpRequestHeader.Authorization] = basicAuth;

            byte[] _byteVersion = Encoding.ASCII.GetBytes(refreshtokenRequestBody);
            refreshtokenRequest.ContentLength = _byteVersion.Length;
            Stream stream = refreshtokenRequest.GetRequestStream();
            stream.Write(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            try
            {
                //get response
                HttpWebResponse refreshtokenResponse = (HttpWebResponse)refreshtokenRequest.GetResponse();
                using (var refreshTokenReader = new StreamReader(refreshtokenResponse.GetResponseStream()))
                {
                    //read response
                    string responseText = refreshTokenReader.ReadToEnd();

                    // decode response
                    Dictionary<string, string> refreshtokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

                    if (refreshtokenEndpointDecoded.ContainsKey("error"))
                    {
                        // Check for errors.
                        if (refreshtokenEndpointDecoded["error"] != null)
                        {
                            //output(String.Format("OAuth token refresh error: {0}.", refreshtokenEndpointDecoded["error"]));
                            return PerformRefreshToken;
                        }
                    }
                    else
                    {
                        //if no error
                        if (refreshtokenEndpointDecoded.ContainsKey("refresh_token"))
                        {

                            refresh_token = refreshtokenEndpointDecoded["refresh_token"];
                            AuObj.QBRefreshToken = refresh_token;


                            if (refreshtokenEndpointDecoded.ContainsKey("access_token"))
                            {
                                //save both refresh token and new access token in permanent store
                                access_token = refreshtokenEndpointDecoded["access_token"];
                                AuObj.QBToken = access_token;
                                PerformRefreshToken = true;

                            }
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {

                        // output("HTTP Status: " + response.StatusCode);
                        var exceptionDetail = response.GetResponseHeader("WWW-Authenticate");
                        if (exceptionDetail != null && exceptionDetail != "")
                        {
                            // output(exceptionDetail);
                        }
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            // read response body
                            string responseText = reader.ReadToEnd();
                            if (responseText != null && responseText != "")
                            {
                                //  output(responseText);
                            }
                        }
                    }

                }
            }

            try
            {
            }
            catch (Exception ex)
            {

            }
            //output("Access token refreshed.");
            return PerformRefreshToken;
        }

        public static string LiveQBConnectionOperation_POST(string Method, string Query, string Post)
        {
            //    Int32 Retry = 0;
            //ReRun:
            HttpWebRequest WebRequestObject = null;
            HttpWebResponse WebResponseObject = null;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string Results = "";
        ReRun:;
            try
            {
                string uri2 = string.Format("https://{0}/v3/company/{1}{2}", ConfigurationSettings.AppSettings["qboBaseUrl"], AuObj.RealmId, Query);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                // string uri2 = string.Format(ConfigurationManager.AppSettings["qboBaseUrl"].ToString(), AuObj.RealmID.ToString(), Query);
                WebRequestObject = (HttpWebRequest)WebRequest.Create(uri2);
                WebRequestObject.Method = "POST";
                WebRequestObject.Accept = "application/xml";
                WebRequestObject.Method = WebRequestMethods.Http.Post;
                WebRequestObject.Headers.Add(string.Format("Authorization: Bearer {0}", AuObj.QBToken));
                WebRequestObject.ContentType = "application/xml";

                UTF8Encoding encoding = new UTF8Encoding();
                byte[] byte1 = encoding.GetBytes(Post.ToString());
                WebRequestObject.ContentLength = byte1.Length;
                Stream newStream = WebRequestObject.GetRequestStream();

                newStream.Write(byte1, 0, byte1.Length);
                HttpWebResponse httpWebResponse = WebRequestObject.GetResponse() as HttpWebResponse;
                using (Stream data = httpWebResponse.GetResponseStream())
                {
                    Results = "OK_" + new StreamReader(data).ReadToEnd();
                }

            }
            catch (WebException ex)
            {
                using (WebResponse response = ex.Response)
                {
                    var httpResponse = (HttpWebResponse)response;

                    using (Stream data = response.GetResponseStream())
                    {
                        StreamReader sr2 = new StreamReader(data);
                        Results = "Error:" + sr2.ReadToEnd();

                        if (Results.Contains("Token expired") && Results.Contains("AuthenticationFailed"))
                        {
                            if (PerformRefreshToken(AuObj.QBRefreshToken.ToString()))
                            {
                                goto ReRun;
                            }
                        }
                    }
                }
            }
            finally
            {
                try
                {
                    WebResponseObject.Close();
                    WebRequestObject.Abort();
                }
                catch
                {
                }
            }
            return Results;
        }


        public static void ExtractResponse(string Response, ref DataTable dtResult, string Mode, ref Int32 StartPosition, ref Int32 PageNumber)
        {
            try
            {
                XmlDocument oDoc = new XmlDocument();
                oDoc.LoadXml(Response);

                switch (Mode)
                {
                    case "VENDOR":
                        // Int32 StartPosition = 0;
                        XmlNode oMain1 = oDoc["IntuitResponse"]["QueryResponse"];
                        if (oMain1.Attributes[0] != null && oMain1.Attributes[1] != null)
                        {
                            // StartPosition = Convert.ToInt32(oMain.Attributes[0].Value.ToString());
                            Int32 MaxResult = Convert.ToInt32(oMain1.Attributes[1].Value.ToString());
                            if (MaxResult % 500 == 0)
                            {
                                StartPosition = (MaxResult * PageNumber) + 1;
                                PageNumber += 1;
                            }
                            else
                            {
                                PageNumber = -1;
                            }
                        }
                        XmlNodeList oListItm1 = oDoc.GetElementsByTagName("Vendor");
                        foreach (XmlNode oNode in oListItm1)
                        {
                            DataRow dr = dtResult.NewRow();
                            dr[0] = (oNode["Id"] == null ? "0" : oNode["Id"].InnerXml);
                            dr[1] = (oNode["Name"] == null ? "" : oNode["Name"].InnerXml);
                            dr[2] = (oNode["Type"] == null ? "" : oNode["Type"].InnerXml);
                            dr[3] = (oNode["Description"] == null ? "" : oNode["Description"].InnerXml);
                            dtResult.Rows.Add(dr);
                        }
                        break;
                    case "ACC":
                        XmlNodeList oList = oDoc.GetElementsByTagName("Account");
                        foreach (XmlNode oNode in oList)
                        {
                            DataRow dr = dtResult.NewRow();
                            dr[0] = (oNode["Id"] == null ? "0" : oNode["Id"].InnerXml);
                            dr[1] = (oNode["Name"] == null ? "" : oNode["Name"].InnerXml);
                            dr[2] = (oNode["AccountType"] == null ? "" : oNode["AccountType"].InnerXml);
                            dtResult.Rows.Add(dr);
                        }
                        break;


                    case "ITM":
                        // Int32 StartPosition = 0;
                        XmlNode oMain = oDoc["IntuitResponse"]["QueryResponse"];
                        if (oMain.Attributes[0] != null && oMain.Attributes[1] != null)
                        {
                            // StartPosition = Convert.ToInt32(oMain.Attributes[0].Value.ToString());
                            Int32 MaxResult = Convert.ToInt32(oMain.Attributes[1].Value.ToString());
                            if (MaxResult % 500 == 0)
                            {
                                StartPosition = (MaxResult * PageNumber) + 1;
                                PageNumber += 1;
                            }
                            else
                            {
                                PageNumber = -1;
                            }
                        }
                        XmlNodeList oListItm = oDoc.GetElementsByTagName("Item");
                        foreach (XmlNode oNode in oListItm)
                        {
                            DataRow dr = dtResult.NewRow();
                            dr[0] = (oNode["Id"] == null ? "0" : oNode["Id"].InnerXml);
                            dr[1] = (oNode["Name"] == null ? "" : oNode["Name"].InnerXml);
                            dr[2] = (oNode["Type"] == null ? "" : oNode["Type"].InnerXml);
                            dr[3] = (oNode["Description"] == null ? "" : oNode["Description"].InnerXml);
                            dtResult.Rows.Add(dr);
                        }
                        break;

                }
            }
            catch (Exception ex)
            {
                //clsCommon.WriteErrorLogs("Extract Resonse. Mode:" + Mode + ". Message:" + ex.Message);
            }
        }
    }
}