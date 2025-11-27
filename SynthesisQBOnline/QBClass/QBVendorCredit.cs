using SynthesisQBOnline.BAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace SynthesisQBOnline.QBClass
{
    public static class QBVendorCredit
    {
        // GET VendorCredit
        public static List<VendorCredit> GetVendorCredit_All(QBOnlineconfiguration obj, ref QBResponse objRes)
        {
            List<VendorCredit> ListCredit = new List<VendorCredit>();
            try
            {
                Int32 PageNumber = 1;
                Int32 StartPosition = 1;

                NextPage:
                clsCommon.WriteErrorLogs("Retrieve All VendorCredit from QuickBooks. Read Page Number :" + PageNumber);
                string SrResponse = "";
                SrResponse = QBRequest.LiveQBConnectionOperation_GET("GET", "Select%20%2A%20from%20VendorCredit%20Where%20Metadata.LastUpdatedTime%20%3E%20%272020-02-01%27%20STARTPOSITION%20" + StartPosition + "%20MAXRESULTS%20500%20", obj);
               
                if (SrResponse.Contains("OK_"))
                {
                    objRes.Status = "Get VendorCredit";
                    objRes.Message = "";
                    ListCredit = ExtractResponse(SrResponse.Replace("OK_", ""), "VendorCredit", ref StartPosition, ref PageNumber);
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
                    XmlNodeList oListBill = oDoc.GetElementsByTagName("Error");

                    foreach (XmlNode oNode in oListBill)
                    {
                        if (oNode["Message"] != null)
                        {
                            objRes.ID = "0";
                            objRes.Status = "Get VendorCredit Message:" + oNode["Message"].InnerXml;
                            objRes.Message = oNode["Message"].InnerXml.ToString();
                            clsCommon.WriteErrorLogs("Get VendorCredit Message:" + oNode["Message"].InnerXml);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs(ex.Message);
                objRes.Message = ex.Message;
                objRes.Status = "Error: VendorCredit not found";
            }

            return ListCredit;
        }

        public static List<VendorCredit> ExtractResponse(string Response, string Mode, ref Int32 StartPosition, ref Int32 PageNumber)
        {
            clsCommon.Clear_Intialize_Table("VenCredit");
            List<VendorCredit> LstCredit = new List<VendorCredit>();
            try
            {
                XmlDocument oDoc = new XmlDocument();
                oDoc.LoadXml(Response);

                switch (Mode)
                {
                    case "VendorCredit":
                        // Int32 StartPosition = 0;
                        XmlNode oMain = oDoc["IntuitResponse"]["QueryResponse"];
                        if (oMain.Attributes.Count > 0)
                        {
                            if (oMain.Attributes[0] != null && oMain.Attributes[1] != null)
                            {
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
                        XmlNodeList oListCredit = oDoc.GetElementsByTagName("Bill");
                        foreach (XmlNode oNode in oListCredit)
                        {
                            VendorCredit objCredit = new VendorCredit();
                            objCredit.ID = (oNode["Id"] == null ? "0" : oNode["Id"].InnerXml);
                            objCredit.SyncToken = (oNode["SyncToken"] == null ? "" : oNode["SyncToken"].InnerXml);
                            objCredit.CreatedTime = (oNode["MetaData"]["CreateTime"] == null ? "" : oNode["MetaData"]["CreateTime"].InnerXml);
                            objCredit.LastModifiedTime = (oNode["MetaData"]["LastUpdatedTime"] == null ? "" : oNode["MetaData"]["LastUpdatedTime"].InnerXml);
                            objCredit.RefNumber = (oNode["DocNumber"] == null ? "" : oNode["DocNumber"].InnerXml);
                            objCredit.TxnDate = Convert.ToDateTime(oNode["TxnDate"] == null ? "" : oNode["TxnDate"].InnerXml);
                            objCredit.Notes = (oNode["PrivateNote"] == null ? "" : oNode["PrivateNote"].InnerXml);
                            objCredit.VendorID = (oNode["VendorRef"] == null ? "" : oNode["VendorRef"].InnerXml);
                            objCredit.VendorName = (oNode["VendorRef"].Attributes[0] == null ? "" : oNode["VendorRef"].Attributes[0].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                            objCredit.Amount = Convert.ToDecimal(oNode["TotalAmt"] == null ? "" : oNode["TotalAmt"].InnerXml);

                            LstCredit.Add(objCredit);
                            objCredit = null;
                            if (oNode["Line"] != null)
                            {
                                XmlNodeList oInnerList = oDoc.GetElementsByTagName("Line");
                                foreach (XmlNode oInner in oInnerList)
                                {
                                    DataRow drInner = clsCommon.dtQBChild.NewRow();
                                    drInner["TxnID"] = (oNode["Id"] == null ? "0" : oNode["Id"].InnerXml);
                                    drInner["LineId"] = (oInner["Id"] == null ? "" : oInner["Id"].InnerXml);
                                    drInner["LineNum"] = (oInner["LineNum"] == null ? "" : oInner["LineNum"].InnerXml);
                                    drInner["Description"] = (oInner["Description"] == null ? "" : oInner["Description"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                    drInner["Amount"] = Convert.ToDecimal(oInner["Amount"] == null ? "" : oInner["Amount"].InnerXml);
                                    if (oInner["AccountBasedExpenseLineDetail"] != null)
                                    {
                                        drInner["DeptName"] = (oInner["AccountBasedExpenseLineDetail"]["AccountRef"].Attributes[0].InnerXml == null ? "" : oInner["AccountBasedExpenseLineDetail"]["AccountRef"].Attributes[0].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                        drInner["DeptID"] = (oInner["AccountBasedExpenseLineDetail"]["AccountRef"] == null ? "" : oInner["AccountBasedExpenseLineDetail"]["AccountRef"].InnerXml);
                                    }
                                    clsCommon.dtQBChild.Rows.Add(drInner);
                                }
                            }
                        }
                        break;

                }
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs("Extract Resonse. Mode:" + Mode + ". Message:" + ex.Message);
            }
            return LstCredit;
        }

        public static void CheckVendorCreditID(string ID, QBOnlineconfiguration obj, ref QBResponse objRes)
        {
            try
            {
                clsCommon.WriteErrorLogs("Retrieve VendorCredit from QuickBooks.");
                string SrResponse = "";
                SrResponse = QBRequest.LiveQBConnectionOperation_GET("GET", "select%20*%20from%20VendorCredit%20where%20ID%3d%27" + ID + "%27&minorversion=4", obj);

                if (SrResponse.Contains("OK_"))
                {
                    objRes.Status = "Get VendorCredit";
                    objRes.Message = "";
                    objRes.ID = GetVendorCreditId(SrResponse.Replace("OK_", "")).ToString();
                    objRes.SyncToken = GetVendorCreditSyncToken(SrResponse.Replace("OK_", "")).ToString();
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
                    XmlNodeList oListVen= oDoc.GetElementsByTagName("Error");

                    foreach (XmlNode oNode in oListVen)
                    {
                        if (oNode["Message"] != null)
                        {
                            objRes.ID = "0";
                            objRes.Status = "Get VendorCredit Message:" + oNode["Message"].InnerXml;
                            objRes.Message = oNode["Message"].InnerXml.ToString();
                            clsCommon.WriteErrorLogs("Get VendorCredit Message:" + oNode["Message"].InnerXml);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                clsCommon.WriteErrorLogs(ex.Message);
                objRes.Message = ex.Message;
                objRes.Status = "Error: VendorCredit not found";
            }
        }

        public static string GetVendorCreditSyncToken(string Response)
        {
            string SyncToken = "";

            try
            {
                XmlDocument oDoc = new XmlDocument();
                oDoc.LoadXml(Response);
                XmlNodeList oListVendor = oDoc.GetElementsByTagName("VendorCredit");

                foreach (XmlNode oNode in oListVendor)
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
                clsCommon.WriteErrorLogs("Check VendorCredit Response Extract. Message:" + ex.Message);
            }

            return SyncToken;
        }

        public static void CreateVendorCredit(VendorCredit obj, List<VendorCredirDetail> objDetail, ref QBResponse objRes, QBOnlineconfiguration objToken)
        {
            string XmlRequest = "";
            try
            {
                clsCommon.WriteErrorLogs("CreateVendorCredit:" + obj.RefNumber);
                XmlRequest = GenerateXmlRequest_VendorCredit(obj, objDetail);
                string xmlResonse = QBRequest.LiveQBConnectionOperation_POST("POST", "/vendorcredit", XmlRequest,objToken);

                if (xmlResonse.Contains("OK_"))
                {
                    objRes.ID = GetVendorCreditId(xmlResonse.Replace("OK_", "")).ToString();
                    objRes.SyncToken = GetVendorCreditSyncToken(xmlResonse.Replace("OK_", "")).ToString();
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
                    XmlNodeList oListVen = oDoc.GetElementsByTagName("Error");

                    foreach (XmlNode oNode in oListVen)
                    {
                        if (oNode["Message"] != null)
                        {
                            objRes.ID = "";
                            objRes.Status = "VendorCredit Create Error";
                            objRes.Message = oNode["Message"].InnerXml.ToString();
                            clsCommon.WriteErrorLogs("Create VendorCredit Request: " + XmlRequest);
                            clsCommon.WriteErrorLogs("Create VendorCredit Message: " + oNode["Message"].InnerXml);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objRes.Status = "Create VendorCredit. Message:" + ex.Message;
                objRes.Message = ex.Message;
                objRes.ID = "";
                clsCommon.WriteErrorLogs("Create VendorCredit Request: " + XmlRequest);
                clsCommon.WriteErrorLogs("Create VendorCredit. Message:" + ex.Message);
            }
        }

        private static string GenerateXmlRequest_VendorCredit(VendorCredit obj, List<VendorCredirDetail> objDetail)
        {
            StringBuilder sbRequest = new StringBuilder();

            try
            {
                sbRequest.Append("<VendorCredit xmlns=\"http://schema.intuit.com/finance/v3\" domain=\"QBO\" sparse=\"false\">");

                sbRequest.Append("<VendorRef>" + obj.VendorID.ToString() + "</VendorRef>");
                if (obj.TxnDate != null)
                    sbRequest.Append("<TxnDate>" + clsCommon.ConvertDate_QbFormat(obj.TxnDate.ToString()) + "</TxnDate>");
                if (obj.RefNumber != null)
                    sbRequest.Append("<DocNumber>" + clsCommon.RemoveSpecialCharacters(obj.RefNumber.ToString(), "State") + "</DocNumber>");
                if (obj.Notes != null)
                    sbRequest.Append("<PrivateNote>" + clsCommon.RemoveSpecialCharacters(obj.Notes.ToString(), "Memo") + "</PrivateNote>");
                if (obj.Amount != null)
                    sbRequest.Append("<TotalAmt>" + clsCommon.RemoveSpecialCharacters(obj.Amount.ToString(), "Amount") + "</TotalAmt>");

                foreach (var strLine in objDetail)
                {
                    sbRequest.Append("<Line>");
                    if (strLine.Description != null)
                        sbRequest.Append("<Description>" + clsCommon.RemoveSpecialCharacters(strLine.Description.ToString(), "Description") + "</Description>");
                    if (strLine.Amount != null)
                        sbRequest.Append("<Amount>" + clsCommon.RemoveSpecialCharacters(strLine.Amount.ToString(), "Amount") + "</Amount>");
                    sbRequest.Append("<DetailType>AccountBasedExpenseLineDetail</DetailType>");
                    sbRequest.Append("<AccountBasedExpenseLineDetail>");
                    if (strLine.DepartmentID != null)
                        sbRequest.Append("<AccountRef>" + strLine.DepartmentID.ToString() + "</AccountRef>");

                    if (strLine.CustomerID != null)
                        sbRequest.Append("<CustomerRef>" + strLine.CustomerID.ToString() + "</CustomerRef>");

                    sbRequest.Append("</AccountBasedExpenseLineDetail>");
                    sbRequest.Append("</Line>");
                }

                sbRequest.Append("</VendorCredit>");
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs("Build XML Request for VendorCredit. Message:" + ex.Message);
            }

            return sbRequest.ToString();
        }

        public static string GetVendorCreditId(string Response)
        {
            string Id = "";

            try
            {
                XmlDocument oDoc = new XmlDocument();
                oDoc.LoadXml(Response);
                XmlNodeList oListVendor = oDoc.GetElementsByTagName("VendorCredit");

                foreach (XmlNode oNode in oListVendor)
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
                clsCommon.WriteErrorLogs("Check VendorCredit Response Extract. Message:" + ex.Message);
            }

            return Id;
        }

        public static void ModVendorCredit(VendorCredit obj, List<VendorCredirDetail> objDetail, ref QBResponse objRes, QBOnlineconfiguration objToken)
        {
            string XmlRequest = "";
            try
            {
                XmlRequest = GenerateXmlRequest_ModVendorCredit(obj, objDetail);
                string xmlResonse = QBRequest.LiveQBConnectionOperation_POST("POST", "/vendorcredit", XmlRequest, objToken);

                if (xmlResonse.Contains("OK_"))
                {
                    objRes.ID = GetVendorCreditId(xmlResonse.Replace("OK_", "")).ToString();
                    objRes.SyncToken = GetVendorCreditSyncToken(xmlResonse.Replace("OK_", "")).ToString();
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
                    XmlNodeList oListVen = oDoc.GetElementsByTagName("Error");

                    foreach (XmlNode oNode in oListVen)
                    {
                        if (oNode["Message"] != null)
                        {
                            objRes.ID = "";
                            objRes.Status = "VendorCredit Update Error";
                            objRes.Message = oNode["Message"].InnerXml.ToString();
                            clsCommon.WriteErrorLogs("Edit VendorCredit Request: " + XmlRequest);
                            clsCommon.WriteErrorLogs("Create VendorCredit Message: " + oNode["Message"].InnerXml);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objRes.Status = "Update VendorCredit. Message:" + ex.Message;
                objRes.Message = ex.Message;
                objRes.ID = "";
                clsCommon.WriteErrorLogs("Edit VendorCredit Request: " + XmlRequest);
                clsCommon.WriteErrorLogs("Update VendorCredit. Message:" + ex.Message);
            }
        }

        private static string GenerateXmlRequest_ModVendorCredit(VendorCredit obj, List<VendorCredirDetail> objDetail)
        {
            StringBuilder sbRequest = new StringBuilder();

            try
            {
                sbRequest.Append("<VendorCredit xmlns=\"http://schema.intuit.com/finance/v3\" domain=\"QBO\" sparse=\"false\">");

                sbRequest.Append("<Id>" + obj.ID.ToString() + "</Id>");
                sbRequest.Append("<SyncToken>" + obj.SyncToken.ToString() + "</SyncToken>");

                sbRequest.Append("<VendorRef>" + obj.VendorID.ToString() + "</VendorRef>");
                if (obj.TxnDate != null)
                    sbRequest.Append("<TxnDate>" + clsCommon.ConvertDate_QbFormat(obj.TxnDate.ToString()) + "</TxnDate>");
                if (obj.RefNumber != null)
                    sbRequest.Append("<DocNumber>" + clsCommon.RemoveSpecialCharacters(obj.RefNumber.ToString(), "State") + "</DocNumber>");
                if (obj.Notes != null)
                    sbRequest.Append("<PrivateNote>" + clsCommon.RemoveSpecialCharacters(obj.Notes.ToString(), "Memo") + "</PrivateNote>");
                if (obj.Amount != null)
                    sbRequest.Append("<TotalAmt>" + clsCommon.RemoveSpecialCharacters(obj.Amount.ToString(), "Amount") + "</TotalAmt>");

                foreach (var strLine in objDetail)
                {
                    sbRequest.Append("<Line>");
                    if (strLine.Description != null)
                        sbRequest.Append("<Description>" + clsCommon.RemoveSpecialCharacters(strLine.Description.ToString(), "Description") + "</Description>");
                    if (strLine.Amount != null)
                        sbRequest.Append("<Amount>" + clsCommon.RemoveSpecialCharacters(strLine.Amount.ToString(), "Amount") + "</Amount>");
                    sbRequest.Append("<DetailType>AccountBasedExpenseLineDetail</DetailType>");
                    sbRequest.Append("<AccountBasedExpenseLineDetail>");
                    if (strLine.DepartmentID != null)
                        sbRequest.Append("<AccountRef>" + strLine.DepartmentID.ToString() + "</AccountRef>");

                    if (strLine.CustomerID != null)
                        sbRequest.Append("<CustomerRef>" + strLine.CustomerID.ToString() + "</CustomerRef>");

                    sbRequest.Append("</AccountBasedExpenseLineDetail>");
                    sbRequest.Append("</Line>");
                }

                sbRequest.Append("</VendorCredit>");
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs("Build XML Request for VendorCredit. Message:" + ex.Message);
            }

            return sbRequest.ToString();
        }
    }
}
