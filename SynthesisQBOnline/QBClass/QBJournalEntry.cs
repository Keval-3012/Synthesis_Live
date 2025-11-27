using SynthesisQBOnline;
using SynthesisQBOnline.BAL;
using SynthesisQBOnline.QBClass;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

public static class QBJournalEntry
{

    public static void CreateJournalEntry(JournalEntry obj, List<JournalDetail> objDetail, ref QBResponse objRes, QBOnlineconfiguration objToken)
    {
        string XmlRequest = "";
        try
        {
            XmlRequest = GenerateXmlRequest_AddJournalEntry(obj, objDetail);
            string xmlResonse = QBRequest.LiveQBConnectionOperation_POST("POST", "/journalentry", XmlRequest, objToken);

            if (xmlResonse.Contains("OK_"))
            {
                objRes.ID = GetJournalEntryId(xmlResonse.Replace("OK_", "")).ToString();
                objRes.SyncToken = GetJournalSyncToken(xmlResonse.Replace("OK_", "")).ToString();
                objRes.ID = objRes.ID.ToString();
                objRes.Status = "Done";
                objRes.Message = "";
                clsCommon.WriteErrorLogs("Create JournalEntry. JournalEntry ID :" + obj.ID + " TxnID :" + objRes.ID.ToString());
            }
            else if (xmlResonse.Contains("Token Expired") || xmlResonse.Contains("AuthenticationFailed") || xmlResonse.Contains("Authorization"))
            {
                objRes.ID = "0";
                objRes.Status = "AuthenticationFailed";
                objRes.Message = "AuthenticationFailed";
                clsCommon.WriteErrorLogs("Create JournalEntry. Message:" + objRes.Message);
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
                        objRes.Status = "JournalEntry Create Error";
                        objRes.Message = oNode["Message"].InnerXml.ToString();
                        clsCommon.WriteErrorLogs("Create JournalEntry Request: " + XmlRequest);
                        clsCommon.WriteErrorLogs("Create JournalEntry Error Message: " + oNode["Message"].InnerXml);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            objRes.Status = "Create JournalEntry Error. Message:" + ex.Message;
            objRes.Message = ex.Message;
            objRes.ID = "";
            clsCommon.WriteErrorLogs("Create JournalEntry Request: " + XmlRequest);
            clsCommon.WriteErrorLogs("Create JournalEntry Error. Message:" + ex.Message);
        }
    }


    private static string GenerateXmlRequest_AddJournalEntry(JournalEntry obj, List<JournalDetail> objDetail)
    {
        StringBuilder sbRequest = new StringBuilder();

        try
        {
            sbRequest.Append("<JournalEntry xmlns=\"http://schema.intuit.com/finance/v3\" domain=\"QBO\" sparse=\"false\">");

            if (obj.salesdate != null)
                sbRequest.Append("<TxnDate>" + clsCommon.ConvertDate_QbFormat(obj.salesdate.ToString()) + "</TxnDate>");

            foreach (var strLine in objDetail)
            {

                sbRequest.Append("<Line>");
                if (strLine.Memo != null)
                    sbRequest.Append("<Description>" + clsCommon.RemoveSpecialCharacters(strLine.Memo.ToString(), "Description") + "</Description>");
                if (strLine.Amount != null)
                    sbRequest.Append("<Amount>" + clsCommon.RemoveSpecialCharacters(strLine.Amount.ToString(), "Amount") + "</Amount>");
                sbRequest.Append("<DetailType>JournalEntryLineDetail</DetailType>");
                sbRequest.Append("<JournalEntryLineDetail>");
                if (strLine.Typeid != null)
                {
                    if (strLine.Typeid == 2)
                    {
                        sbRequest.Append("<PostingType>Debit</PostingType>");
                    }
                    else
                    {
                        sbRequest.Append("<PostingType>Credit</PostingType>");
                    }
                }
                if (strLine.EntityID != null)
                {
                    sbRequest.Append("<Entity>");
                    sbRequest.Append("<Type>" + strLine.EntityType.ToString() + "</Type>");
                    sbRequest.Append("<EntityRef>" + strLine.EntityID.ToString() + "</EntityRef>");
                    sbRequest.Append("</Entity>");
                }

                if (strLine.ListID != null)
                    sbRequest.Append("<AccountRef>" + strLine.ListID.ToString() + "</AccountRef>");

                sbRequest.Append("</JournalEntryLineDetail>");
                sbRequest.Append("</Line>");
            }

            sbRequest.Append("</JournalEntry>");
        }
        catch (Exception ex)
        {
            clsCommon.WriteErrorLogs("Build XML Request for JournalEntry. Message:" + ex.Message);
        }

        return sbRequest.ToString();
    }

    public static string GetJournalEntryId(string Response)
    {
        string Id = "";

        try
        {
            XmlDocument oDoc = new XmlDocument();
            oDoc.LoadXml(Response);
            XmlNodeList oListJournalEntry = oDoc.GetElementsByTagName("JournalEntry");

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
            clsCommon.WriteErrorLogs("Check GetJournalEntryId Response Extract. Message:" + ex.Message);
        }

        return Id;
    }

    public static string GetJournalSyncToken(string Response)
    {
        string SyncToken = "";

        try
        {
            XmlDocument oDoc = new XmlDocument();
            oDoc.LoadXml(Response);
            XmlNodeList oListBill = oDoc.GetElementsByTagName("JournalEntry");

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
            clsCommon.WriteErrorLogs("Check GetJournalSyncToken Response Extract. Message:" + ex.Message);
        }

        return SyncToken;
    }
}

