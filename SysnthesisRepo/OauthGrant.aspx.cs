using Newtonsoft.Json;
using SysnthesisRepo.QBAuth;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace wwwroot
{
    public partial class OauthGrant : System.Web.UI.Page
    {
        private string redirectURI, discoveryURI, qboBaseUrl, logPath; //clientID, clientSecret,
        private string scopeValC2QB;//, scopeValOpenId, scopeValSIWI;

        string incoming_state = "";
        string realmId = "";
        string code = "";
        static string authorizationEndpoint;
        static string tokenEndpoint;
        static string userinfoEndPoint;
        static string revokeEndpoint;
        static string issuerUrl;
        static string jwksEndpoint;
        static string mod;
        static string expo;

        protected void Page_Load(object sender, EventArgs e)
        {
            redirectURI = ConfigurationManager.AppSettings["redirectURI"];
            discoveryURI = ConfigurationManager.AppSettings["discoveryURI"];
            qboBaseUrl = ConfigurationManager.AppSettings["qboBaseUrl"];
            logPath = "~/Log/";
            if (!System.IO.Directory.Exists(Server.MapPath(logPath)))
            {
                System.IO.Directory.CreateDirectory(Server.MapPath(logPath));
            }
            // Fetch ClientId and ClientSecret from SP_QBOnlineConfiguration            
            string clientId = null;
            string clientSecret = null;

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_QBOnlineConfiguration", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Mode", "GetQBTokenStorewise");
                    //cmd.Parameters.AddWithValue("@StoreID", storeId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            clientId = reader["ClientID"].ToString();
                            clientSecret = reader["ClientSecret"].ToString();
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret))
            {
                QBRequest.AuObj.ClientId = clientId;
                QBRequest.AuObj.ClientSecretKey = clientSecret;
            }
            else
            {
                output("QuickBooks Client ID/Secret not found in DB for StoreID: ");
                return;
            }
            scopeValC2QB = System.Uri.EscapeDataString(ConfigurationManager.AppSettings["scopeValC2QB"]);

            if (Session["accessToken"] == null)
            {
                if (Request.QueryString.Count > 0)
                {
                    List<string> queryKeys = new List<string>(Request.QueryString.AllKeys);
                   
                    // Check for errors.
                    if (queryKeys.Contains("error") == true)
                    {
                        output(String.Format("OAuth Authorization Error: {0}.", Request.QueryString["error"].ToString()));
                        return;
                    }
                    if (queryKeys.Contains("code") == false
                        || queryKeys.Contains("state") == false)
                    {
                        output("Malformed Authorization Response.");
                        return;
                    }

                    //extracts the state
                    if (Request.QueryString["state"] != null)
                    {
                        incoming_state = Request.QueryString["state"].ToString();
                        if (Session["CSRF"] != null)
                        {
                            //match incoming state with the saved State in your DB from DoOAuth function and then execute the below steps
                            if (Session["CSRF"].ToString() == incoming_state)
                            {
                                if (Request.QueryString["realmId"] != null)
                                {
                                    realmId = Request.QueryString["realmId"].ToString();
                                    Session["realmId"] = realmId;
                                }

                                //extract the code
                                if (Request.QueryString["code"] != null)
                                {
                                    code = Request.QueryString["code"].ToString();
                                    output("Authorization code obtained.");
                                    //start the code exchange at the Token Endpoint.
                                    //this call will fail with 'invalid grant' error if application is not stopped after testing one button click flow as code is not renewed
                                    PerformCodeExchange(code, redirectURI, realmId);
                                }

                                if (Session["realmId"] != null)
                                {
                                    if (Session["accessToken"] != null && Session["refreshToken"] != null)
                                    {
                                        //call QBO api
                                        QboApiCall(Session["accessToken"].ToString(), Session["refreshToken"].ToString(), Session["realmId"].ToString());

                                        QBRequest.AuObj.RealmId = Session["realmId"].ToString();
                                        QBRequest.AuObj.QBRefreshToken = Session["refreshToken"].ToString();
                                        QBRequest.AuObj.QBToken = Session["accessToken"].ToString();

                                        Session["accessToken"] = null;
                                        Session["realmId"] = null;
                                        Session["refreshToken"] = null;
                                        Session["CSRF"] = null;
                                        // Truncate QuickBooksStorewiseToken table
                                        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString))
                                        {
                                            using (SqlCommand cmd = new SqlCommand("SP_QBOnlineConfiguration", conn))
                                            {
                                                cmd.CommandType = CommandType.StoredProcedure;
                                                cmd.Parameters.AddWithValue("@Mode", "TruncateQBTokenStorewise");
                                                conn.Open();
                                                cmd.ExecuteNonQuery();
                                            }
                                        }

                                        //Save_Data();
                                        Response.Redirect("/QBConfiguration/SaveData");
                                    }
                                }
                                else
                                {
                                    output("SIWI call does not returns realm for QBO qbo api call.");
                                }
                            }
                            else
                            {
                                output("Invalid State");
                                Session.Clear();
                                Session.Abandon();
                            }
                        }
                    }
                }
                else
                {
                    GetDiscoveryData();
                    //get JWKS keys
                    GetJWKSkeys();
                    //DoOAuth for Connect to Quickbooks button
                    DoOAuth("C2QB");
                }
            }


        }

        public void output(string logMsg)
        {
            //Console.WriteLine(logMsg);
            System.IO.StreamWriter sw = System.IO.File.AppendText(Server.MapPath(logPath) + "OAuth2SampleAppLogs.txt");
            try
            {
                string logLine = System.String.Format(
                    "{0:G}: {1}.", System.DateTime.Now, logMsg);
                sw.WriteLine(logLine);
            }
            finally
            {
                sw.Close();
            }
        }

        public static string randomDataBase64url(uint length)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[length];
            rng.GetBytes(bytes);
            return Base64urlencodeNoPadding(bytes);
        }

        public static string Base64urlencodeNoPadding(byte[] buffer)
        {
            string base64 = Convert.ToBase64String(buffer);

            // Converts base64 to base64url.
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            // Strips padding.
            base64 = base64.Replace("=", "");
            return base64;
        }

        public string GetLogPath()
        {
            try
            {
                if (logPath == "")
                {
                    logPath = System.Environment.GetEnvironmentVariable("TEMP");
                    if (!logPath.EndsWith("\\")) logPath += "\\";
                }
            }
            catch
            {
                output("Log error path not found.");
            }
            return logPath;
        }

        #region get Discovery data
        private void GetDiscoveryData()
        {
            output("Fetching Discovery Data.");
            DiscoveryData discoveryDataDecoded;

            // build the request    
            HttpWebRequest discoveryRequest = (HttpWebRequest)WebRequest.Create(discoveryURI);
            discoveryRequest.Method = "GET";
            discoveryRequest.Accept = "application/json";

            try
            {
                //call Discovery endpoint
                HttpWebResponse discoveryResponse = (HttpWebResponse)discoveryRequest.GetResponse();
                using (var discoveryDataReader = new StreamReader(discoveryResponse.GetResponseStream()))
                {
                    //read response
                    string responseText = discoveryDataReader.ReadToEnd();

                    // converts to dictionary
                    discoveryDataDecoded = JsonConvert.DeserializeObject<DiscoveryData>(responseText);
                }

                //Authorization endpoint url
                authorizationEndpoint = discoveryDataDecoded.Authorization_endpoint;

                //Token endpoint url
                tokenEndpoint = discoveryDataDecoded.Token_endpoint;

                //UseInfo endpoint url
                userinfoEndPoint = discoveryDataDecoded.Userinfo_endpoint;

                //Revoke endpoint url
                revokeEndpoint = discoveryDataDecoded.Revocation_endpoint;

                //Issuer endpoint Url 
                issuerUrl = discoveryDataDecoded.Issuer;

                //Json Web Key Store Url
                jwksEndpoint = discoveryDataDecoded.JWKS_uri;

                output("Discovery Data obtained.");
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {

                        output("HTTP Status: " + response.StatusCode);
                        var exceptionDetail = response.GetResponseHeader("WWW-Authenticate");
                        if (exceptionDetail != null && exceptionDetail != "")
                        {
                            output(exceptionDetail);
                        }
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            // read response body
                            string responseText = reader.ReadToEnd();
                            if (responseText != null && responseText != "")
                            {
                                output(responseText);
                            }
                        }
                    }
                }
                else
                {
                    output(ex.Message);
                }
            }
        }
        #endregion
        private void GetJWKSkeys()
        {
            output("Making Get JWKS Keys Call.");

            JWKS jwksEndpointDecoded;
            // send the JWKS request
            HttpWebRequest jwksRequest = (HttpWebRequest)WebRequest.Create(jwksEndpoint);
            jwksRequest.Method = "GET";
            jwksRequest.Accept = "application/json";
            // get the response
            HttpWebResponse jwksResponse = (HttpWebResponse)jwksRequest.GetResponse();

            using (var jwksReader = new StreamReader(jwksResponse.GetResponseStream()))
            {
                //read response
                string responseText = jwksReader.ReadToEnd();

                //Decode userInfo response
                jwksEndpointDecoded = JsonConvert.DeserializeObject<JWKS>(responseText);
            }

            //get mod and exponent value
            foreach (var key in jwksEndpointDecoded.Keys)
            {
                if (key.N != null)
                {
                    mod = key.N;
                }
                if (key.E != null)
                {
                    expo = key.E;
                }

            }
            output("JWKS Keys obtained.");
        }

        #region OAuth2 calls
        private void DoOAuth(string callMadeBy)
        {
            output("Intiating OAuth2 call to get code.");
            string authorizationRequest = "";
            string scopeVal = "";

            //Generate the state and save this in DB to match it against the incoming_state value after this call is completed
            //Statecan be a unique Id, campaign id, tracking id or CSRF token
            output("Session Start.");
            string stateVal = randomDataBase64url(32);
            if (Session["CSRF"] == null)
            {
                Session["CSRF"] = stateVal;
            }
            output("Session Get." + "-" + stateVal);
            output("Session Completed.");

            //Decide scope based on which flow was initiated
            output("CallMadeBy Start.");
            if (callMadeBy == "C2QB") //C2QB scopes
            {
                Session["callMadeBy"] = "C2QB";
                scopeVal = scopeValC2QB;
            }
            output("CallMadeBy Start." + "-" + scopeVal);

            output("authorizationEndpoint Start.");
            if (authorizationEndpoint != "" && authorizationEndpoint != null)
            {
                output("authorizationEndpoint if condition executed.");
                //Create the OAuth 2.0 authorization request.
                authorizationRequest = string.Format("{0}?client_id={1}&response_type=code&scope={2}&redirect_uri={3}&state={4}",
                    authorizationEndpoint,
                    QBRequest.AuObj.ClientId,
                    scopeVal,
                    System.Uri.EscapeDataString(redirectURI),
                    stateVal);
                output("authorizationEndpoint if condition executed." + "-" + authorizationRequest);
                Response.Redirect(authorizationRequest);
            }
            else
            {
                output("Missing authorizationEndpoint url!");
            }
            output("DoOAuth Function Execution Completed.");

        }

        private void PerformCodeExchange(string code, string redirectURI, string realmId)
        {
            output("Exchanging code for tokens.");

            string id_token = "";
            string refresh_token = "";
            string access_token = "";
            bool isTokenValid = false;
            string sub = "";
            string email = "";
            string emailVerified = "";
            string givenName = "";
            string familyName = "";
            string phoneNumber = "";
            string phoneNumberVerified = "";
            string streetAddress = "";
            string locality = "";
            string region = "";
            string postalCode = "";
            string country = "";

            string cred = string.Format("{0}:{1}", QBRequest.AuObj.ClientId, QBRequest.AuObj.ClientSecretKey);
            string enc = Convert.ToBase64String(Encoding.ASCII.GetBytes(cred));
            string basicAuth = string.Format("{0} {1}", "Basic", enc);

            // build the  request            
            string accesstokenRequestBody = string.Format("grant_type=authorization_code&code={0}&redirect_uri={1}",
                code,
                System.Uri.EscapeDataString(redirectURI)
                );

            // send the Token request
            HttpWebRequest accesstokenRequest = (HttpWebRequest)WebRequest.Create(tokenEndpoint);
            accesstokenRequest.Method = "POST";
            accesstokenRequest.ContentType = "application/x-www-form-urlencoded";
            accesstokenRequest.Accept = "application/json";
            accesstokenRequest.Headers[HttpRequestHeader.Authorization] = basicAuth;//Adding Authorization header

            byte[] _byteVersion = Encoding.ASCII.GetBytes(accesstokenRequestBody);
            accesstokenRequest.ContentLength = _byteVersion.Length;
            Stream stream = accesstokenRequest.GetRequestStream();
            stream.Write(_byteVersion, 0, _byteVersion.Length);//verify
            stream.Close();

            try
            {
                // get the response
                HttpWebResponse accesstokenResponse = (HttpWebResponse)accesstokenRequest.GetResponse();
                using (var accesstokenReader = new StreamReader(accesstokenResponse.GetResponseStream()))
                {
                    //read response
                    string responseText = accesstokenReader.ReadToEnd();
                    //decode response
                    Dictionary<string, string> accesstokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);
                    if (accesstokenEndpointDecoded.ContainsKey("id_token"))
                    {

                        id_token = accesstokenEndpointDecoded["id_token"];
                        //validate idToken
                        isTokenValid = isIdTokenValid(id_token);
                    }

                    if (accesstokenEndpointDecoded.ContainsKey("refresh_token"))
                    {
                        //save the refresh token in persistent store so that it can be used to refresh short lived access tokens
                        refresh_token = accesstokenEndpointDecoded["refresh_token"];
                        Session["refreshToken"] = refresh_token;

                        if (accesstokenEndpointDecoded.ContainsKey("access_token"))
                        {
                            output("Access token obtained.");
                            access_token = accesstokenEndpointDecoded["access_token"];
                            Session["accessToken"] = access_token;

                            //get userinfo
                            UserInfo userdata = GetUserInfo(access_token, refresh_token);

                            //read userinfo endpoint details
                            sub = userdata.Sub;
                            email = userdata.Email;
                            emailVerified = userdata.EmailVerified;
                            givenName = userdata.GivenName;
                            familyName = userdata.FamilyName;
                            phoneNumber = userdata.PhoneNumber;
                            phoneNumberVerified = userdata.PhoneNumberVerified;
                            userdata.Address = new Address();
                            streetAddress = userdata.Address.StreetAddress;
                            locality = userdata.Address.Locality;
                            region = userdata.Address.Region;
                            postalCode = userdata.Address.PostalCode;
                            country = userdata.Address.Country;

                        }
                    }

                    if (Session["callMadeby"].ToString() == "OpenId")
                    {
                        if (Request.Url.Query == "")
                        {
                            Response.Redirect(Request.RawUrl);
                        }
                        else
                        {
                            Response.Redirect(Request.RawUrl.Replace(Request.Url.Query, ""));
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
                        output("HTTP Status: " + response.StatusCode);
                        var exceptionDetail = response.GetResponseHeader("WWW-Authenticate");
                        if (exceptionDetail != null && exceptionDetail != "")
                        {
                            output(exceptionDetail);
                        }
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            // read response body
                            string responseText = reader.ReadToEnd();
                            if (responseText != null && responseText != "")
                            {
                                output(responseText);
                            }
                        }
                    }
                }
            }
        }
        private void PerformRefreshToken(string refresh_token)
        {
            output("Exchanging refresh token for access token.");//refresh token is valid for 100days and access token for 1hr
            string access_token = "";
            string cred = string.Format("{0}:{1}", QBRequest.AuObj.ClientId, QBRequest.AuObj.ClientSecretKey);
            string enc = Convert.ToBase64String(Encoding.ASCII.GetBytes(cred));
            string basicAuth = string.Format("{0} {1}", "Basic", enc);

            // build the  request
            string refreshtokenRequestBody = string.Format("grant_type=refresh_token&refresh_token={0}",
                refresh_token
                );

            // send the Refresh Token request
            HttpWebRequest refreshtokenRequest = (HttpWebRequest)WebRequest.Create(tokenEndpoint);
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
                            output(String.Format("OAuth token refresh error: {0}.", refreshtokenEndpointDecoded["error"]));
                            return;
                        }
                    }
                    else
                    {
                        //if no error
                        if (refreshtokenEndpointDecoded.ContainsKey("refresh_token"))
                        {

                            refresh_token = refreshtokenEndpointDecoded["refresh_token"];
                            Session["refreshToken"] = refresh_token;


                            if (refreshtokenEndpointDecoded.ContainsKey("access_token"))
                            {
                                //save both refresh token and new access token in permanent store
                                access_token = refreshtokenEndpointDecoded["access_token"];
                                Session["accessToken"] = access_token;



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

                        output("HTTP Status: " + response.StatusCode);
                        var exceptionDetail = response.GetResponseHeader("WWW-Authenticate");
                        if (exceptionDetail != null && exceptionDetail != "")
                        {
                            output(exceptionDetail);
                        }
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            // read response body
                            string responseText = reader.ReadToEnd();
                            if (responseText != null && responseText != "")
                            {
                                output(responseText);
                            }
                        }
                    }

                }
            }

            output("Access token refreshed.");
        }
        private void PerformRefreshToken(string access_token, string refresh_token)
        {
            output("Performing Revoke tokens.");

            string cred = string.Format("{0}:{1}", QBRequest.AuObj.ClientId, QBRequest.AuObj.ClientSecretKey);
            string enc = Convert.ToBase64String(Encoding.ASCII.GetBytes(cred));
            string basicAuth = string.Format("{0} {1}", "Basic", enc);

            // build the request
            string tokenRequestBody = "{\"token\":\"" + refresh_token + "\"}";

            // send the Revoke token request
            HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(revokeEndpoint);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/json";
            tokenRequest.Accept = "application/json";
            //Add Authorization header
            tokenRequest.Headers[HttpRequestHeader.Authorization] = basicAuth;

            byte[] _byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
            tokenRequest.ContentLength = _byteVersion.Length;
            Stream stream = tokenRequest.GetRequestStream();
            stream.Write(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            try
            {
                //get the response
                HttpWebResponse response = (HttpWebResponse)tokenRequest.GetResponse();

                //here you should handle status code and take action based on that
                if (response.StatusCode == HttpStatusCode.OK)//200
                {
                    output("Successful Revoke!");
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)//400
                {
                    output("One or more of BearerToken, RefreshToken, ClientId or, ClientSecret are incorrect.");
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)//401
                {
                    output("Bad authorization header or no authorization header sent.");
                }
                else if (response.StatusCode == HttpStatusCode.InternalServerError)//500
                {
                    output("Intuit server internal error, not the fault of the developer.");
                }

                //We are removing all sessions and qerystring here even if we get error on revoke. 
                //In your code, you can choose to handle the errors and then delete sessions and querystring
                Session.Clear();
                Session.Abandon();
                if (Request.Url.Query == "")
                {
                    Response.Redirect(Request.RawUrl);
                }
                else
                {
                    Response.Redirect(Request.RawUrl.Replace(Request.Url.Query, ""));
                }
            }
            catch (WebException ex)
            {
                Session.Clear();
                Session.Abandon();
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        output("HTTP Status: " + response.StatusCode);
                        var exceptionDetail = response.GetResponseHeader("WWW-Authenticate");
                        if (exceptionDetail != null && exceptionDetail != "")
                        {
                            output(exceptionDetail);
                        }
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            // read response body
                            string responseText = reader.ReadToEnd();
                            if (responseText != null && responseText != "")
                            {
                                output(responseText);
                            }
                        }
                    }
                }
            }

            output("Token revoked.");
        }
        private bool isIdTokenValid(string id_token)
        {
            output("Making IsIdToken Valid Call.");
            string idToken = id_token;
            string[] splitValues = idToken.Split('.');
            if (splitValues[0] != null)
            {

                //decode header 
                var headerJson = Encoding.UTF8.GetString(FromBase64Url(splitValues[0].ToString()));
                IdTokenHeader headerData = JsonConvert.DeserializeObject<IdTokenHeader>(headerJson);

                //Verify if the key id of the key used to sign the payload is not null
                if (headerData.Kid == null)
                {
                    return false;
                }

                //Verify if the hashing alg used to sign the payload is not null
                if (headerData.Alg == null)
                {
                    return false;
                }

            }
            if (splitValues[1] != null)
            {
                //decode payload
                var payloadJson = Encoding.UTF8.GetString(FromBase64Url(splitValues[1].ToString()));

                IdTokenPayload payloadData = JsonConvert.DeserializeObject<IdTokenPayload>(payloadJson);

                //verify aud matches clientId
                if (payloadData.Aud != null)
                {
                    if (payloadData.Aud[0].ToString() != QBRequest.AuObj.ClientId)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

                //verify authtime matches the time the ID token was authorized.                
                if (payloadData.Auth_time == null)
                {
                    return false;
                }

                //verify exp matches the time the ID token expires, represented in Unix time (integer seconds).                
                if (payloadData.Exp != null)
                {
                    ulong expiration = Convert.ToUInt64(payloadData.Exp);

                    TimeSpan epochTicks = new TimeSpan(new DateTime(1970, 1, 1).Ticks);
                    TimeSpan unixTicks = new TimeSpan(DateTime.UtcNow.Ticks) - epochTicks;
                    ulong unixTime = Convert.ToUInt64(unixTicks.Milliseconds);
                    //Verify the expiration time with what you expiry time have calculated and saved in your application
                    if ((expiration - unixTime) <= 0)
                    {

                        return false;
                    }
                }
                else
                {
                    return false;
                }

                //Verify iat matches the time the ID token was issued, represented in Unix time (integer seconds).            
                if (payloadData.Iat == null)
                {
                    return false;
                }

                //verify iss matches the  issuer identifier for the issuer of the response.     
                if (payloadData.Iss != null)
                {
                    if (payloadData.Iss.ToString() != issuerUrl)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
                if (payloadData.Sub == null)
                {

                    return false;
                }
            }

            //verify Siganture matches the sigend concatenation of the encoded header and the encoded payload with the specified algorithm
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            //Read values of n and e from discovery document.
            rsa.ImportParameters(
              new RSAParameters()
              {
                  //Read values from discovery document
                  Modulus = FromBase64Url(mod),
                  Exponent = FromBase64Url(expo)
              });

            //verify using RSA signature
            SHA256 sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(splitValues[0] + '.' + splitValues[1]));

            RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
            rsaDeformatter.SetHashAlgorithm("SHA256");
            if (rsaDeformatter.VerifySignature(hash, FromBase64Url(splitValues[2])))
            {
                output("IdToken Signature is verified.");
                output("IsIdToken Valid Call completed.");
                return true;
            }
            else
            {
                output("Signature is compromised.");
                output("IsIdToken Valid Call completed.");
                return false;
            }
        }
        private UserInfo GetUserInfo(string access_token, string refresh_token)
        {
            output("Making Get User Info Call.");
            // send the UserInfo endpoint request
            HttpWebRequest userinfoRequest = (HttpWebRequest)WebRequest.Create(userinfoEndPoint);
            userinfoRequest.Method = "GET";
            userinfoRequest.Headers.Add(string.Format("Authorization: Bearer {0}", access_token));
            userinfoRequest.Accept = "application/json";

            // get the response
            HttpWebResponse userinfoResponse = (HttpWebResponse)userinfoRequest.GetResponse();
            UserInfo userinfoEndpointDecoded;
            using (var userinfoReader = new StreamReader(userinfoResponse.GetResponseStream()))
            {
                //read response
                string responseText = userinfoReader.ReadToEnd();
                userinfoEndpointDecoded = JsonConvert.DeserializeObject<UserInfo>(responseText);
            }
            output("Get User Info Call completed.");
            return userinfoEndpointDecoded;
        }
        static byte[] FromBase64Url(string base64Url)
        {
            string padded = base64Url.Length % 4 == 0
                ? base64Url : base64Url + "====".Substring(base64Url.Length % 4);
            string base64 = padded.Replace("_", "/")
                                  .Replace("-", "+");
            return Convert.FromBase64String(base64);
        }
        #endregion

        #region qbo calls
        private void QboApiCall(string access_token, string refresh_token, string realmId)
        {
            try
            {
                if (realmId != "")
                {
                    output("Making QBO API Call.");

                    string query = "select * from CompanyInfo";
                    // build the  request
                    string encodedQuery = WebUtility.UrlEncode(query);

                    //add qbobase url and query
                    string uri = string.Format("https://{0}/v3/company/{1}/query?query={2}", qboBaseUrl, realmId, encodedQuery);

                    // send the request
                    HttpWebRequest qboApiRequest = (HttpWebRequest)WebRequest.Create(uri);
                    qboApiRequest.Method = "GET";
                    qboApiRequest.Headers.Add(string.Format("Authorization: Bearer {0}", access_token));
                    qboApiRequest.ContentType = "application/json;charset=UTF-8";
                    qboApiRequest.Accept = "*/*";

                    // get the response
                    HttpWebResponse qboApiResponse = (HttpWebResponse)qboApiRequest.GetResponse();
                    if (qboApiResponse.StatusCode == HttpStatusCode.Unauthorized)//401
                    {
                        output("Invalid/Expired Access Token.");
                        //if you get a 401 token expiry then perform token refresh
                        PerformRefreshToken(refresh_token);

                        //Retry QBO API call again with new tokens
                        if (Session["accessToken"] != null && Session["refreshToken"] != null && Session["realmId"] != null)
                        {
                            QboApiCall(Session["accessToken"].ToString(), Session["refreshToken"].ToString(), Session["realmId"].ToString());
                        }
                    }
                    else
                    {
                        //read qbo api response
                        using (var qboApiReader = new StreamReader(qboApiResponse.GetResponseStream()))
                        {
                            string responseText = qboApiReader.ReadToEnd();
                            output("QBO call successful.");
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
                        output("HTTP Status: " + response.StatusCode);
                        var exceptionDetail = response.GetResponseHeader("WWW-Authenticate");
                        if (exceptionDetail != null && exceptionDetail != "")
                        {
                            output(exceptionDetail);
                        }
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            // read response body
                            string responseText = reader.ReadToEnd();
                            if (responseText != null && responseText != "")
                            {
                                output(responseText);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        //private bool Save_Data()
        //{
        //    bool Save_Record = false;
        //    try
        //    {
        //        DataTable dt = new BALQBAPIKey().SelectbyAPPId(new BOLQBAPIKey() { AppId = Convert.ToInt32(Session["AppId"].ToString()), UserId = Convert.ToInt32(Session["UserId"].ToString()) });
        //        if (dt.Rows.Count <= 0)
        //        {
        //            new BALQBAPIKey().Insert(new BOLQBAPIKey()
        //            {
        //                UserId = Convert.ToInt32(Session["UserId"].ToString()),
        //                AppId = Convert.ToInt32(Session["AppId"].ToString()),
        //                CompanyRealmID = QBRequest.AuObj.RealmID.ToString(),
        //                ClientId = QBRequest.AuObj.ClientId.ToString(),
        //                ClientSecretKey = QBRequest.AuObj.SecretKey.ToString(),
        //                QBToken = QBRequest.AuObj.QbToken.ToString(),
        //                QBRefreshToken = QBRequest.AuObj.RefreshToekn.ToString()
        //            });

        //            Save_Record = true;
        //        }
        //        else
        //        {
        //            new BALQBAPIKey().Update(new BOLQBAPIKey()
        //            {
        //                Id = Convert.ToInt32(Session["UserId"].ToString()),
        //                UserId = Convert.ToInt32(Session["UserId"].ToString()),
        //                AppId = Convert.ToInt32(Session["AppId"].ToString()),
        //                CompanyRealmID = QBRequest.AuObj.RealmID.ToString(),
        //                ClientId = QBRequest.AuObj.ClientId.ToString(),
        //                ClientSecretKey = QBRequest.AuObj.SecretKey.ToString(),
        //                QBToken = QBRequest.AuObj.QbToken.ToString(),
        //                QBRefreshToken = QBRequest.AuObj.RefreshToekn.ToString()
        //            });
        //            Save_Record = true;
        //        }
        //    }
        //    catch (Exception ex) { ex.ToString(); }
        //    return Save_Record;
        //}
    }
}