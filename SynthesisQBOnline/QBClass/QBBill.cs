using SynthesisQBOnline;
using SynthesisQBOnline.BAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace SynthesisQBOnline.QBClass
{
    public static class QBBill
    {
        // GET Bill
        public static List<Bill> GetBill_All(QBOnlineconfiguration obj, ref QBResponse objRes)
        {
            List<Bill> ListBill = new List<Bill>();
            try
            {
                Int32 PageNumber = 1;
                Int32 StartPosition = 1;
                NextPage:
                clsCommon.WriteErrorLogs("Retrieve All Bill from QuickBooks. Read Page Number :" + PageNumber);
                string SrResponse = "";
                SrResponse = QBRequest.LiveQBConnectionOperation_GET("GET", "Select%20%2A%20from%20Bill%20Where%20Metadata.LastUpdatedTime%20%3E%20%272020-02-01%27%20STARTPOSITION%20" + StartPosition + "%20MAXRESULTS%20500%20", obj);
                //SrResponse = QBRequest.LiveQBConnectionOperation_GET("GET", "Select%20%2A%20from%20Bill%20Where%20Metadata.CreateTime%20%3E%20%272020-02-01%27%20%20STARTPOSITION%20" + StartPosition + "%20MAXRESULTS%20500%20", obj);

                if (SrResponse.Contains("OK_"))
                {
                    objRes.Status = "Get Bill";
                    objRes.Message = "";
                    ListBill = ExtractResponse(SrResponse.Replace("OK_", ""), "Bill", ref StartPosition, ref PageNumber);
                    if (PageNumber != -1)
                        goto NextPage;
                }
                else if (SrResponse.Contains("Token Expired") || SrResponse.Contains("AuthenticationFailed") || SrResponse.Contains("Authorization"))
                {
                    objRes.ID = "0";
                    objRes.Status = "AuthenticationFailed";
                    objRes.Message = "AuthenticationFailed";
                    clsCommon.WriteErrorLogs("GetBill_All Read : AuthenticationFailed");
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
                            objRes.Status = "Get Bill Message:" + oNode["Message"].InnerXml;
                            objRes.Message = oNode["Message"].InnerXml.ToString();
                            clsCommon.WriteErrorLogs("Get Bill Message:" + oNode["Message"].InnerXml);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs("GetBill_All Error: " + ex.InnerException.ToString() + "  Message: " + ex.Message.ToString());
                objRes.Message = ex.Message;
                objRes.Status = "Error: Bill not found";
            }

            return ListBill;
        }

        public static List<Bill> ExtractResponse(string Response, string Mode, ref Int32 StartPosition, ref Int32 PageNumber)
        {
            clsCommon.Clear_Intialize_Table("INV");
            List<Bill> LstBill = new List<Bill>();
            try
            {
                XmlDocument oDoc = new XmlDocument();
                oDoc.LoadXml(Response);

                switch (Mode)
                {
                    case "Bill":
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
                        XmlNodeList oListBill = oDoc.GetElementsByTagName("Bill");
                        foreach (XmlNode oNode in oListBill)
                        {
                            Bill objBill = new Bill();
                            objBill.ID = (oNode["Id"] == null ? "0" : oNode["Id"].InnerXml);
                            objBill.SyncToken = (oNode["SyncToken"] == null ? "" : oNode["SyncToken"].InnerXml);
                            objBill.CreatedTime = (oNode["MetaData"]["CreateTime"] == null ? "" : oNode["MetaData"]["CreateTime"].InnerXml);
                            objBill.LastModifiedTime = (oNode["MetaData"]["LastUpdatedTime"] == null ? "" : oNode["MetaData"]["LastUpdatedTime"].InnerXml);
                            objBill.RefNumber = (oNode["DocNumber"] == null ? "" : oNode["DocNumber"].InnerXml);
                            objBill.InvoiceDate = (oNode["TxnDate"] == null ? "" : oNode["TxnDate"].InnerXml);
                            objBill.Notes = (oNode["PrivateNote"] == null ? "" : oNode["PrivateNote"].InnerXml);
                            objBill.VendorID = (oNode["VendorRef"] == null ? "" : oNode["VendorRef"].InnerXml);
                            objBill.VendorName = (oNode["VendorRef"].Attributes[0] == null ? "" : oNode["VendorRef"].Attributes[0].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                            objBill.Amount = Convert.ToDecimal(oNode["TotalAmt"] == null ? "" : oNode["TotalAmt"].InnerXml);
                            objBill.DueDate = oNode["DueDate"] == null ? "" : oNode["DueDate"].InnerXml;

                            LstBill.Add(objBill);
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
                            objBill = null;
                        }
                        break;

                }
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs("Extract Resonse. Mode:" + Mode + ". Message:" + ex.Message);
            }
            return LstBill;
        }

        // Check Bill
        public static void CheckBillID(string ID, QBOnlineconfiguration obj, ref QBResponse objRes)
        {
            try
            {
                clsCommon.WriteErrorLogs("Retrieve Bill from QuickBooks.");
                string SrResponse = "";
                SrResponse = QBRequest.LiveQBConnectionOperation_GET("GET", "select%20*%20from%20Bill%20where%20ID%3d%27" + ID + "%27&minorversion=4", obj);

                if (SrResponse.Contains("OK_"))
                {
                    objRes.Status = "Get Bill";
                    objRes.Message = "";
                    objRes.ID = GetBillId(SrResponse.Replace("OK_", "")).ToString();
                    objRes.SyncToken = GetBillSyncToken(SrResponse.Replace("OK_", "")).ToString();
                }
                else if (SrResponse.Contains("Token Expired") || SrResponse.Contains("AuthenticationFailed") || SrResponse.Contains("Authorization"))
                {
                    objRes.ID = "0";
                    objRes.Status = "AuthenticationFailed";
                    objRes.Message = "AuthenticationFailed";
                    clsCommon.WriteErrorLogs("CheckBillID : AuthenticationFailed");
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
                            objRes.Status = "Get Bill Message:" + oNode["Message"].InnerXml;
                            objRes.Message = oNode["Message"].InnerXml.ToString();
                            clsCommon.WriteErrorLogs("CheckBillID:" + oNode["Message"].InnerXml);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs("CheckBillID Error: " + ex.InnerException.ToString() + "  Message: " + ex.Message.ToString());
                objRes.Message = ex.Message;
                objRes.Status = "Error: Bill not found";
            }
        }

        public static string GetBillSyncToken(string Response)
        {
            string SyncToken = "";

            try
            {
                XmlDocument oDoc = new XmlDocument();
                oDoc.LoadXml(Response);
                XmlNodeList oListBill = oDoc.GetElementsByTagName("Bill");

                foreach (XmlNode oNode in oListBill)
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
                clsCommon.WriteErrorLogs("Check Bill Response Extract. Message:" + ex.Message);
            }

            return SyncToken;
        }

        public static void CreateBill(Bill obj, List<BillDetail> objDetail, ref QBResponse objRes, QBOnlineconfiguration objToken)
        {
            string XmlRequest = "";
            try
            {
                XmlRequest = GenerateXmlRequest_Bill(obj, objDetail);
                string xmlResonse = QBRequest.LiveQBConnectionOperation_POST("POST", "/bill", XmlRequest, objToken);

                if (xmlResonse.Contains("OK_"))
                {
                    objRes.ID = GetBillId(xmlResonse.Replace("OK_", "")).ToString();
                    objRes.SyncToken = GetBillSyncToken(xmlResonse.Replace("OK_", "")).ToString();
                    objRes.ID = objRes.ID.ToString();
                    objRes.Status = "Done";
                    objRes.Message = "";
                }
                else if (xmlResonse.Contains("Token Expired") || xmlResonse.Contains("AuthenticationFailed") || xmlResonse.Contains("Authorization"))
                {
                    objRes.ID = "0";
                    objRes.Status = "AuthenticationFailed";
                    objRes.Message = "AuthenticationFailed";
                    clsCommon.WriteErrorLogs("CreateBill Error : AuthenticationFailed");
                }
                else if (xmlResonse.Contains("Error") || xmlResonse.Contains("error"))
                {
                    XmlDocument oDoc = new XmlDocument();
                    oDoc.LoadXml(xmlResonse.Replace("Error:", "").Replace("error:", ""));
                    XmlNodeList oListBill = oDoc.GetElementsByTagName("Error");

                    foreach (XmlNode oNode in oListBill)
                    {
                        if (oNode["Message"] != null)
                        {
                            objRes.ID = "";
                            objRes.Status = "Bill Create Error";
                            objRes.Message = oNode["Message"].InnerXml.ToString();
                            clsCommon.WriteErrorLogs("Create bill Request: " + XmlRequest);
                            clsCommon.WriteErrorLogs("Create bill Message: " + oNode["Message"].InnerXml);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objRes.Status = "Create bill. Message:" + ex.Message;
                objRes.Message = ex.Message;
                objRes.ID = "";
                clsCommon.WriteErrorLogs("Create bill Request: " + XmlRequest);
                clsCommon.WriteErrorLogs("CreateBill Error: " + ex.InnerException.ToString() + "  Message: " + ex.Message.ToString());
            }
        }

        private static string GenerateXmlRequest_Bill(Bill obj, List<BillDetail> objDetail)
        {
            StringBuilder sbRequest = new StringBuilder();

            try
            {
                sbRequest.Append("<Bill xmlns=\"http://schema.intuit.com/finance/v3\" domain=\"QBO\" sparse=\"false\">");

                sbRequest.Append("<VendorRef>" + obj.VendorID.ToString() + "</VendorRef>");
                if (obj.InvoiceDate != null)
                    sbRequest.Append("<TxnDate>" + clsCommon.ConvertDate_QbFormat(obj.InvoiceDate.ToString()) + "</TxnDate>");
                if (obj.RefNumber != null)
                    sbRequest.Append("<DocNumber>" + clsCommon.RemoveSpecialCharacters(obj.RefNumber.ToString(), "State") + "</DocNumber>");
                if (obj.Notes != null)
                    sbRequest.Append("<PrivateNote>" + clsCommon.RemoveSpecialCharacters(obj.Notes.ToString(), "Memo") + "</PrivateNote>");

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

                    sbRequest.Append("</AccountBasedExpenseLineDetail>");
                    sbRequest.Append("</Line>");
                }

                sbRequest.Append("</Bill>");
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs("GenerateXmlRequest_Bill :  Error: " + ex.InnerException.ToString() + "  Message: " + ex.Message.ToString());
            }

            return sbRequest.ToString();
        }

        public static string GetBillId(string Response)
        {
            string Id = "";

            try
            {
                XmlDocument oDoc = new XmlDocument();
                oDoc.LoadXml(Response);
                XmlNodeList oListBill = oDoc.GetElementsByTagName("Bill");

                foreach (XmlNode oNode in oListBill)
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
                clsCommon.WriteErrorLogs("Check Bill Response Extract. Error: " + ex.InnerException.ToString() + "  Message: " + ex.Message.ToString());
            }

            return Id;
        }

        public static void ModBill(Bill obj, List<BillDetail> objDetail, ref QBResponse objRes, QBOnlineconfiguration objToken)
        {
            string XmlRequest = "";
            try
            {
                XmlRequest = GenerateXmlRequest_ModBill(obj, objDetail);
                string xmlResonse = QBRequest.LiveQBConnectionOperation_POST("POST", "/bill", XmlRequest, objToken);

                if (xmlResonse.Contains("OK_"))
                {
                    objRes.ID = GetBillId(xmlResonse.Replace("OK_", "")).ToString();
                    objRes.SyncToken = GetBillSyncToken(xmlResonse.Replace("OK_", "")).ToString();
                    objRes.ID = objRes.ID.ToString();
                    objRes.Status = "Done";
                    objRes.Message = "";
                }
                else if (xmlResonse.Contains("Token Expired") || xmlResonse.Contains("AuthenticationFailed") || xmlResonse.Contains("Authorization"))
                {
                    objRes.ID = "0";
                    objRes.Status = "AuthenticationFailed";
                    objRes.Message = "AuthenticationFailed";
                    clsCommon.WriteErrorLogs("ModBill : AuthenticationFailed ");
                }
                else if (xmlResonse.Contains("Error") || xmlResonse.Contains("error"))
                {
                    XmlDocument oDoc = new XmlDocument();
                    oDoc.LoadXml(xmlResonse.Replace("Error:", "").Replace("error:", ""));
                    XmlNodeList oListBill = oDoc.GetElementsByTagName("Error");

                    foreach (XmlNode oNode in oListBill)
                    {
                        if (oNode["Message"] != null)
                        {
                            objRes.ID = "";
                            objRes.Status = "Bill Update Error";
                            objRes.Message = oNode["Message"].InnerXml.ToString();
                            clsCommon.WriteErrorLogs("ModBill Request: " + XmlRequest);
                            clsCommon.WriteErrorLogs("ModBill Message: " + oNode["Message"].InnerXml);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objRes.Status = "Update bill. Message:" + ex.Message;
                objRes.Message = ex.Message;
                objRes.ID = "";
                clsCommon.WriteErrorLogs("ModBill Request: " + XmlRequest);
                clsCommon.WriteErrorLogs("ModBill. Error: " + ex.InnerException.ToString() + "  Message: " + ex.Message.ToString());
            }
        }

        private static string GenerateXmlRequest_ModBill(Bill obj, List<BillDetail> objDetail)
        {
            StringBuilder sbRequest = new StringBuilder();

            try
            {
                sbRequest.Append("<Bill xmlns=\"http://schema.intuit.com/finance/v3\" domain=\"QBO\" sparse=\"false\">");

                sbRequest.Append("<Id>" + obj.ID.ToString() + "</Id>");
                sbRequest.Append("<SyncToken>" + obj.SyncToken.ToString() + "</SyncToken>");

                sbRequest.Append("<VendorRef>" + obj.VendorID.ToString() + "</VendorRef>");
                if (obj.InvoiceDate != null)
                    sbRequest.Append("<TxnDate>" + clsCommon.ConvertDate_QbFormat(obj.InvoiceDate.ToString()) + "</TxnDate>");
                if (obj.RefNumber != null)
                    sbRequest.Append("<DocNumber>" + clsCommon.RemoveSpecialCharacters(obj.RefNumber.ToString(), "State") + "</DocNumber>");
                if (obj.Notes != null)
                    sbRequest.Append("<PrivateNote>" + clsCommon.RemoveSpecialCharacters(obj.Notes.ToString(), "Memo") + "</PrivateNote>");

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

                    sbRequest.Append("</AccountBasedExpenseLineDetail>");
                    sbRequest.Append("</Line>");
                }

                sbRequest.Append("</Bill>");
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs("Build XML Request for Mod Bill. Message:" + ex.Message);
            }

            return sbRequest.ToString();
        }
    }
}
