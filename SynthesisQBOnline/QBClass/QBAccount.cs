using SynthesisQBOnline.BAL;
using SynthesisQBOnline;
using SynthesisQBOnline.QBClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SynthesisQBOnline
{
    public static class QBAccount
    {
        public static void CheckDeapartmentID(string ID, QBOnlineconfiguration obj, ref QBResponse objRes)
        {
            try
            {
                clsCommon.WriteErrorLogs("Retrieve Deapartment from QuickBooks.");
                string SrResponse = "";
                SrResponse = QBRequest.LiveQBConnectionOperation_GET("GET", "select%20*%20from%20account%20where%20ID%3d%27" + ID + "%27&minorversion=4", obj);

                if (SrResponse.Contains("OK_"))
                {
                    objRes.Status = "Get Deapartment";
                    objRes.Message = "";
                    objRes.ID = GetDepartmentId(SrResponse.Replace("OK_", "")).ToString();
                    objRes.SyncToken = GetDepartmentSyncToken(SrResponse.Replace("OK_", "")).ToString();
                }
                else if (SrResponse.Contains("Token Expired") || SrResponse.Contains("AuthenticationFailed") || SrResponse.Contains("Authorization"))
                {
                    objRes.ID = "0";
                    objRes.Status = "AuthenticationFailed";
                    objRes.Message = "AuthenticationFailed";
                }
                else if (SrResponse.Contains("Error") || SrResponse.Contains("error"))
                {
                    XmlDocument oDoc = new XmlDocument();
                    oDoc.LoadXml(SrResponse.Replace("Error:", "").Replace("error:", ""));
                    XmlNodeList oListAcc = oDoc.GetElementsByTagName("Error");

                    foreach (XmlNode oNode in oListAcc)
                    {
                        if (oNode["Message"] != null)
                        {
                            objRes.ID = "0";
                            objRes.Status = "Get Deapartment Message:" + oNode["Message"].InnerXml;
                            objRes.Message = oNode["Message"].InnerXml.ToString();
                            clsCommon.WriteErrorLogs("Get Deapartment Message:" + oNode["Message"].InnerXml);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs(ex.Message);
                objRes.Message = ex.Message;
                objRes.Status = "Error: Deapartment not found";
            }
        }

        public static string GetDepartmentSyncToken(string Response)
        {
            string SyncToken = "";
            try
            {
                XmlDocument oDoc = new XmlDocument();
                oDoc.LoadXml(Response);
                XmlNodeList oListAccount = oDoc.GetElementsByTagName("Account");

                foreach (XmlNode oNode in oListAccount)
                {
                    if (oNode["SyncToken"] != null)
                    {
                        //ID = Convert.ToInt32(oNode["Id"].InnerXml);
                        SyncToken = oNode["SyncToken"].InnerXml;
                    }
                }
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs("Check Account Response Extract. Message:" + ex.Message);
            }
            return SyncToken;
        }

        public static void CreateDepartment(AccountMaster obj, ref QBResponse objRes, QBOnlineconfiguration objToken)
        {
            try
            {
                string XmlRequest = GenerateXmlRequest_Department(obj);
                string xmlResonse = QBRequest.LiveQBConnectionOperation_POST("POST", "/account", XmlRequest, objToken);

                if (xmlResonse.Contains("OK_"))
                {
                    objRes.ID = GetDepartmentId(xmlResonse.Replace("OK_", "")).ToString();
                    objRes.SyncToken = GetDepartmentSyncToken(xmlResonse.Replace("OK_", "")).ToString();
                    objRes.ID = objRes.ID.ToString();
                    objRes.Status = "Done";
                    objRes.Message = "";
                }
                else if (xmlResonse.Contains("Token Expired") || xmlResonse.Contains("AuthenticationFailed") || xmlResonse.Contains("Authorization"))
                {
                    objRes.ID = "0";
                    objRes.Status = "AuthenticationFailed";
                    objRes.Message = "AuthenticationFailed";
                }
                else if (xmlResonse.Contains("Error") || xmlResonse.Contains("error"))
                {
                    XmlDocument oDoc = new XmlDocument();
                    oDoc.LoadXml(xmlResonse.Replace("Error:", "").Replace("error:", ""));
                    XmlNodeList oListDep = oDoc.GetElementsByTagName("Error");

                    foreach (XmlNode oNode in oListDep)
                    {
                        if (oNode["Message"] != null)
                        {
                            objRes.ID = "0";
                            objRes.Status = "Create Department Message:" + oNode["Message"].InnerXml;
                            objRes.Message = oNode["Message"].InnerXml.ToString();
                            clsCommon.WriteErrorLogs("Create Department Message:" + oNode["Message"].InnerXml);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs("Create Department. Message:" + ex.Message);
                objRes.Status = "Create Department. Message:" + ex.Message;
                objRes.Message = ex.Message;
                objRes.ID = "";
            }
        }

        private static string GenerateXmlRequest_Department(AccountMaster obj)
        {
            StringBuilder sbRequest = new StringBuilder();

            try
            {
                sbRequest.Append("<Account xmlns=\"http://schema.intuit.com/finance/v3\" domain=\"QBO\" sparse=\"false\">");

                if (obj.Department != null)
                {
                    sbRequest.Append("<Name>" + clsCommon.RemoveSpecialCharacters(obj.Department.ToString(), "Customer") + "</Name>");
                }
                if (obj.AccountType != null)
                {
                    sbRequest.Append("<AccountType>" + obj.AccountType.ToString() + "</AccountType>");
                }
                if (obj.Description != null)
                {
                    sbRequest.Append("<Description>" + clsCommon.RemoveSpecialCharacters(obj.Description.ToString(), "Address") + " </Description>");
                }
                if (obj.AccountNumber != null)
                {
                    sbRequest.Append("<AcctNum>" + obj.AccountNumber.ToString() + "</AcctNum>");
                }
                if (obj.DetailType != null)
                {
                    sbRequest.Append("<AccountSubType>" + obj.DetailType.ToString() + "</AccountSubType>");
                }
                if (obj.SubAccount != null)
                {
                    sbRequest.Append("<SubAccount>true</SubAccount>");
                    sbRequest.Append("<ParentRef>" + clsCommon.RemoveSpecialCharacters(obj.SubAccount.ToString(), "Customer") + "</ParentRef>");
                }
                //else
                //{
                //    sbRequest.Append("<SubAccount>true</SubAccount>");
                //    sbRequest.Append("<ParentRef>Customer</ParentRef>");
                //}
                sbRequest.Append("</Account>");
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs("Build XML Request for Account. Message:" + ex.Message);
            }

            return sbRequest.ToString();
        }

        public static string GetDepartmentId(string Response)
        {
            string Id = "";
            try
            {
                XmlDocument oDoc = new XmlDocument();
                oDoc.LoadXml(Response);
                XmlNodeList oListDep = oDoc.GetElementsByTagName("Account");

                foreach (XmlNode oNode in oListDep)
                {
                    if (oNode["Id"] != null)
                    {
                        Id = oNode["Id"].InnerXml;
                    }

                    //if (oNode["SyncToken"] != null)
                    //{
                    //    SyncToken = oNode["SyncToken"].InnerXml;
                    //}
                }
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs("Check Account Response Extract. Message:" + ex.Message);
            }
            return Id;
        }

        public static List<AccountMaster> GetDept_All(QBOnlineconfiguration obj, ref QBResponse objRes)
        {
            List<AccountMaster> AccountList = new List<AccountMaster>();
            try
            {
                Int32 PageNumber = 1;
                Int32 StartPosition = 1;

                NextPage:
                clsCommon.WriteErrorLogs("Retrieve All Account from QuickBooks. Read Page Number :" + PageNumber);
                string SrResponse = "";
                SrResponse = QBRequest.LiveQBConnectionOperation_GET("GET", "select%20*%20FROM%20account%20%20STARTPOSITION%20" + StartPosition + "%20MAXRESULTS%20500%20", obj);

                if (SrResponse.Contains("OK_"))
                {
                    objRes.Status = "Get Account";
                    objRes.Message = "";
                    AccountList = ExtractResponse(SrResponse.Replace("OK_", ""), "Account", ref StartPosition, ref PageNumber);
                    if (PageNumber != -1)
                        goto NextPage;
                }
                else if (SrResponse.Contains("Token Expired") || SrResponse.Contains("AuthenticationFailed") || SrResponse.Contains("Authorization"))
                {
                    objRes.ID = "0";
                    objRes.Status = "AuthenticationFailed";
                    objRes.Message = "AuthenticationFailed";
                }
                else if (SrResponse.Contains("Error") || SrResponse.Contains("error"))
                {
                    XmlDocument oDoc = new XmlDocument();
                    oDoc.LoadXml(SrResponse.Replace("Error:", "").Replace("error:", ""));
                    XmlNodeList oListDep = oDoc.GetElementsByTagName("Error");

                    foreach (XmlNode oNode in oListDep)
                    {
                        if (oNode["Message"] != null)
                        {
                            objRes.ID = "0";
                            objRes.Status = "Get Department Message:" + oNode["Message"].InnerXml;
                            objRes.Message = oNode["Message"].InnerXml.ToString();
                            clsCommon.WriteErrorLogs("Get Department Message:" + oNode["Message"].InnerXml);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs(ex.Message);
                objRes.Message = ex.Message;
                objRes.Status = "Error: Account not found";
            }

            return AccountList;
        }

        public static List<AccountMaster> ExtractResponse(string Response, string Mode, ref Int32 StartPosition, ref Int32 PageNumber)
        {
            List<AccountMaster> LstAccount = new List<AccountMaster>();
            try
            {
                XmlDocument oDoc = new XmlDocument();
                oDoc.LoadXml(Response);

                switch (Mode)
                {
                    case "Account":
                        // Int32 StartPosition = 0;
                        XmlNode oMain = oDoc["IntuitResponse"]["QueryResponse"];
                        if (oMain.Attributes.Count > 0)
                        {
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
                        }
                        else
                        {
                            PageNumber = -1;
                        }
                        XmlNodeList oListAcc = oDoc.GetElementsByTagName("Account");
                        foreach (XmlNode oNode in oListAcc)
                        {
                            AccountMaster objAccount = new AccountMaster();
                            objAccount.ID = (oNode["Id"] == null ? "0" : oNode["Id"].InnerXml);
                            objAccount.SyncToken = (oNode["SyncToken"] == null ? "" : oNode["SyncToken"].InnerXml);
                            objAccount.Department = (oNode["FullyQualifiedName"] == null ? "" : oNode["FullyQualifiedName"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                            objAccount.AccountType = (oNode["AccountType"] == null ? "" : oNode["AccountType"].InnerXml);
                            objAccount.IsActive = (oNode["Active"] == null ? "" : oNode["Active"].InnerXml);
                            objAccount.Description = (oNode["Description"] == null ? "" : oNode["Description"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                            objAccount.DetailType = (oNode["AccountSubType"] == null ? "" : oNode["AccountSubType"].InnerXml);
                            objAccount.AccountNumber = (oNode["AcctNum"] == null ? "" : oNode["AcctNum"].InnerXml);
                            if (Convert.ToBoolean(oNode["SubAccount"].InnerXml) == true)
                            {
                                string[] StrName = oNode["FullyQualifiedName"].InnerXml.ToString().Split(':');
                                objAccount.ParentRefID = (oNode["ParentRef"] == null ? "" : oNode["ParentRef"].InnerXml);
                                objAccount.SubAccount = (oNode["FullyQualifiedName"] == null ? "" : StrName.Length > 1 ? StrName[0].ToString().Replace("&amp;", "&").Replace("&apos;", "'") : "");
                            }

                            LstAccount.Add(objAccount);
                            objAccount = null;
                        }
                        break;

                }
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs("Extract Resonse. Mode:" + Mode + ". Message:" + ex.Message);
            }
            return LstAccount;
        }

        public static void ModDeapartment(AccountMaster obj, ref QBResponse objRes, QBOnlineconfiguration objToken)
        {
            try
            {
                string XmlRequest = GenerateXmlRequest_ModDepartment(obj);
                string xmlResonse = QBRequest.LiveQBConnectionOperation_POST("POST", "/account", XmlRequest, objToken);

                if (xmlResonse.Contains("OK_"))
                {
                    objRes.ID = GetDepartmentId(xmlResonse.Replace("OK_", "")).ToString();
                    objRes.SyncToken = GetDepartmentSyncToken(xmlResonse.Replace("OK_", "")).ToString();
                    objRes.ID = objRes.ID.ToString();
                    objRes.Status = "Done";
                    objRes.Message = "";
                }
                else if (xmlResonse.Contains("Token Expired") || xmlResonse.Contains("AuthenticationFailed") || xmlResonse.Contains("Authorization"))
                {
                    objRes.ID = "0";
                    objRes.Status = "AuthenticationFailed";
                    objRes.Message = "AuthenticationFailed";
                }
                else if (xmlResonse.Contains("Error") || xmlResonse.Contains("error"))
                {
                    XmlDocument oDoc = new XmlDocument();
                    oDoc.LoadXml(xmlResonse.Replace("Error:", "").Replace("error:", ""));
                    XmlNodeList oListAcc = oDoc.GetElementsByTagName("Error");

                    foreach (XmlNode oNode in oListAcc)
                    {
                        if (oNode["Message"] != null)
                        {
                            objRes.ID = "";
                            objRes.Status = "Deapartment Update Error";
                            objRes.Message = oNode["Message"].InnerXml.ToString();
                            clsCommon.WriteErrorLogs("Create Deapartment Message: " + oNode["Message"].InnerXml);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objRes.Status = "Update Deapartment. Message:" + ex.Message;
                objRes.Message = ex.Message;
                objRes.ID = "";
                clsCommon.WriteErrorLogs("Update Deapartment. Message:" + ex.Message);
            }
        }

        private static string GenerateXmlRequest_ModDepartment(AccountMaster obj)
        {
            StringBuilder sbRequest = new StringBuilder();

            try
            {
                sbRequest.Append("<Account xmlns=\"http://schema.intuit.com/finance/v3\" domain=\"QBO\" sparse=\"false\">");

                sbRequest.Append("<Id>" + obj.ID.ToString() + "</Id>");
                sbRequest.Append("<SyncToken>" + obj.SyncToken.ToString() + "</SyncToken>");
                if (obj.Department != null)
                {
                    sbRequest.Append("<Name>" + clsCommon.RemoveSpecialCharacters(obj.Department.ToString(), "Customer") + "</Name>");
                }
                if (obj.AccountType != null)
                {
                    sbRequest.Append("<AccountType>" + obj.AccountType.ToString() + "</AccountType>");
                }
                if (obj.Description != null)
                {
                    sbRequest.Append("<Description>" + clsCommon.RemoveSpecialCharacters(obj.Description.ToString(), "Address") + "</Description>");
                }
                if (obj.AccountNumber != null)
                {
                    sbRequest.Append("<AcctNum>" + obj.AccountNumber.ToString() + "</AcctNum>");
                }
                if (obj.DetailType != null)
                {
                    sbRequest.Append("<AccountSubType>" + obj.DetailType.ToString() + "</AccountSubType>");
                }
                if (obj.SubAccount != null)
                {
                    sbRequest.Append("<SubAccount>true</SubAccount>");
                    sbRequest.Append("<ParentRef>" + clsCommon.RemoveSpecialCharacters(obj.SubAccount.ToString(), "Customer") + "</ParentRef>");
                }
                sbRequest.Append("</Account>");
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs("Build XML Request for Account. Message:" + ex.Message);
            }

            return sbRequest.ToString();
        }
    }
}
