using SynthesisQBOnline.BAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SynthesisQBOnline.QBClass
{
    public static class QBDeposit
    {
        public static void CreateDepositeEntry(Deposit obj, List<DepositDetail> objDetail, ref QBResponse objRes, QBOnlineconfiguration objToken)
        {
            string XmlRequest = "";
            try
            {
                XmlRequest = GenerateXmlRequest_AddDeposit(obj, objDetail);
                string xmlResonse = QBRequest.LiveQBConnectionOperation_POST("POST", "/deposit", XmlRequest, objToken);

                if (xmlResonse.Contains("OK_"))
                {
                    objRes.ID = GetDepositId(xmlResonse.Replace("OK_", "")).ToString();
                    objRes.SyncToken = GetDepositSyncToken(xmlResonse.Replace("OK_", "")).ToString();
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
                    XmlNodeList oListBill = oDoc.GetElementsByTagName("Error");

                    foreach (XmlNode oNode in oListBill)
                    {
                        if (oNode["Message"] != null)
                        {
                            objRes.ID = "";
                            objRes.Status = "Deposit Create Error";
                            objRes.Message = oNode["Message"].InnerXml.ToString();
                            clsCommon.WriteErrorLogs("Create Deposit Request: " + XmlRequest);
                            clsCommon.WriteErrorLogs("Create Deposit Message: " + oNode["Message"].InnerXml);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objRes.Status = "Create Deposit. Message:" + ex.Message;
                objRes.Message = ex.Message;
                objRes.ID = "";
                clsCommon.WriteErrorLogs("Create Deposit Request: " + XmlRequest);
                clsCommon.WriteErrorLogs("Create Deposit. Message:" + ex.Message);
            }
        }

        private static string GenerateXmlRequest_AddDeposit(Deposit obj, List<DepositDetail> objDetail)
        {
            StringBuilder sbRequest = new StringBuilder();

            try
            {

                sbRequest.Append("<Deposit xmlns=\"http://schema.intuit.com/finance/v3\" domain=\"QBO\" sparse=\"false\">");


                if (obj.TxnDate != null)
                    sbRequest.Append("<TxnDate>" + clsCommon.ConvertDate_QbFormat(obj.TxnDate.ToString()) + "</TxnDate>");

                if (obj.Memo != null)
                    sbRequest.Append("<PrivateNote>" + obj.Memo.ToString() + "</PrivateNote>");

                if (obj.DepositAccID != null)
                    sbRequest.Append("<DepositToAccountRef>" + obj.DepositAccID.ToString() + "</DepositToAccountRef>");

                foreach (var strLine in objDetail)
                {

                    sbRequest.Append("<Line>");

                    if (strLine.Amount != null)
                        sbRequest.Append("<Amount>" + clsCommon.RemoveSpecialCharacters(strLine.Amount.ToString(), "Amount") + "</Amount>");
                    sbRequest.Append("<DetailType>DepositLineDetail</DetailType>");
                    if (strLine.Amount != null)
                        sbRequest.Append("<Description>" + clsCommon.RemoveSpecialCharacters(strLine.Description.ToString(), "Description") + "</Description>");


                    sbRequest.Append("<DepositLineDetail>");
                    if (strLine.EntityID != null)
                        sbRequest.Append("<Entity type=\"" + strLine.EntityType.ToString() + "\">" + strLine.EntityID.ToString() + "</Entity>");

                    if (strLine.AccountID != null)
                        sbRequest.Append("<AccountRef>" + strLine.AccountID.ToString() + "</AccountRef>");

                    if (strLine.PaymentMethod != null)
                        sbRequest.Append("<PaymentMethodRef>" + strLine.PaymentMethod.ToString() + "</PaymentMethodRef>");
                    if (strLine.CheckNum != null)
                        sbRequest.Append("<CheckNum>" + strLine.CheckNum + "</CheckNum>");

                    sbRequest.Append("</DepositLineDetail>");
                    sbRequest.Append("</Line>");
                }

                sbRequest.Append("</Deposit>");
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs("Build XML Request for Deposit. Message:" + ex.Message);
            }

            return sbRequest.ToString();
        }

        public static string GetDepositId(string Response)
        {
            string Id = "";

            try
            {
                XmlDocument oDoc = new XmlDocument();
                oDoc.LoadXml(Response);
                XmlNodeList oListJournalEntry = oDoc.GetElementsByTagName("Deposit");

                foreach (XmlNode oNode in oListJournalEntry)
                {
                    if (oNode["Id"] != null)
                    {
                        Id = oNode["Id"].InnerXml;
                    }
                }
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs("Check GetDepositEntryId Response Extract. Message:" + ex.Message);
            }

            return Id;
        }

        public static string GetDepositSyncToken(string Response)
        {
            string SyncToken = "";

            try
            {
                XmlDocument oDoc = new XmlDocument();
                oDoc.LoadXml(Response);
                XmlNodeList oListBill = oDoc.GetElementsByTagName("Deposit");

                foreach (XmlNode oNode in oListBill)
                {
                    if (oNode["SyncToken"] != null)
                    {
                        SyncToken = oNode["SyncToken"].InnerXml;
                    }
                }
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs("Check GetDepositSyncToken Response Extract. Message:" + ex.Message);
            }

            return SyncToken;
        }
    }
}
