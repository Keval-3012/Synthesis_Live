using Microsoft.Extensions.Logging;
using NLog;
using SynthesisQBOnline.BAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SynthesisQBOnline.QBClass
{
    public static class QBExpense
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public static void Create_Expense(Expense obj, List<ExpenseDetail> objDetail, ref QBResponse objRes, QBOnlineconfiguration objToken)
        {
            logger.Info("QBExpense - Create_Expense Start - " + DateTime.Now);
            string XmlRequest = "";
            try
            {
                XmlRequest = GenerateXmlRequest_Expense(obj, objDetail);
                string xmlResonse = QBRequest.LiveQBConnectionOperation_POST("POST", "/purchase", XmlRequest, objToken);
                logger.Info("QBExpense - Get Xmlresponse in Create_Expense - " + xmlResonse);

                if (xmlResonse.Contains("OK_"))
                {
                    logger.Info("QBExpense - Create_Expense Success xmlResonse  - " + xmlResonse);
                    objRes.ID = GetExpenseId(xmlResonse.Replace("OK_", "")).ToString();
                    objRes.SyncToken = GetExpenseSyncToken(xmlResonse.Replace("OK_", "")).ToString();
                    objRes.ID = objRes.ID.ToString();
                    objRes.Status = "Done";
                    objRes.Message = "";
                    logger.Info("QBExpense - Create_Expense Success xmlResonse returnid  - " + objRes.ID + objRes.SyncToken);
                }
                else if (xmlResonse.Contains("Token Expired") || xmlResonse.Contains("AuthenticationFailed") || xmlResonse.Contains("Authorization"))
                {
                    objRes.ID = "0";
                    objRes.Status = "AuthenticationFailed";
                    objRes.Message = "AuthenticationFailed";
                    logger.Info("QBExpense - Create_Expense Error xmlResonse - " + xmlResonse);
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
                            objRes.Status = "Expense Create Error";
                            objRes.Message = oNode["Detail"].InnerXml.ToString();
                            clsCommon.WriteErrorLogs("Create Expense Request: " + XmlRequest);
                            clsCommon.WriteErrorLogs("Create Expense Message: " + oNode["Message"].InnerXml);
                        }
                    }
                    logger.Info("QBExpense - Create_Expense Error xmlResonse error foreach - " + xmlResonse);
                }
            }
            catch (Exception ex)
            {
                objRes.Status = "Create Expense. Message:" + ex.Message;
                objRes.Message = ex.Message;
                objRes.ID = "";
                clsCommon.WriteErrorLogs("Create Expense Request: " + XmlRequest);
                clsCommon.WriteErrorLogs("Create Expense. Message:" + ex.Message);
            }
            logger.Info("QBExpense - Create_Expense End - " + DateTime.Now);
        }

        private static string GenerateXmlRequest_Expense(Expense obj, List<ExpenseDetail> objDetail)
        {
            logger.Info("QBExpense - GenerateXmlRequest_Expense Start - " + DateTime.Now);
            StringBuilder sbRequest = new StringBuilder();

            try
            {
                sbRequest.Append("<Purchase xmlns=\"http://schema.intuit.com/finance/v3\" domain=\"QBO\" sparse=\"false\">");

                sbRequest.Append("<AccountRef>" + obj.AccountID.ToString() + "</AccountRef>");
                if (obj.VendorID != null)
                {
                    sbRequest.Append("<EntityRef type=\"" + obj.EntityType.ToString() + "\">" + obj.VendorID.ToString() + "</EntityRef>");
                }
                if (obj.TxnDate != null)
                    sbRequest.Append("<TxnDate>" + clsCommon.ConvertDate_QbFormat(obj.TxnDate.ToString()) + "</TxnDate>");
                if (obj.RefNumber != null)
                    sbRequest.Append("<DocNumber>" + clsCommon.RemoveSpecialCharacters(obj.RefNumber.ToString(), "State") + "</DocNumber>");
                if (obj.Memo != null)
                    sbRequest.Append("<PrivateNote>" + clsCommon.RemoveSpecialCharacters(obj.Memo.ToString(), "Memo") + "</PrivateNote>");
                if (obj.Amount != null)
                    sbRequest.Append("<TotalAmt>" + clsCommon.RemoveSpecialCharacters(obj.Amount.ToString(), "Amount") + "</TotalAmt>");
                if (obj.PrintStatus != null)
                    sbRequest.Append("<PrintStatus>" + clsCommon.RemoveSpecialCharacters(obj.PrintStatus.ToString(), "PrintStatus") + "</PrintStatus>");
                if (obj.PaymentMethod != null)
                    sbRequest.Append("<PaymentType>" + clsCommon.RemoveSpecialCharacters(obj.PaymentMethod.ToString(), "") + "</PaymentType>");
                if (obj.PaymentMethodId != null)
                    sbRequest.Append("<PaymentMethodRef>" + clsCommon.RemoveSpecialCharacters(obj.PaymentMethodId.ToString(), "") + "</PaymentMethodRef>");

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

                sbRequest.Append("</Purchase>");
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs("Build XML Request for VendorCredit. Message:" + ex.Message);
            }

            logger.Info("QBExpense - GenerateXmlRequest_Expense End - " + sbRequest.ToString() + DateTime.Now);
            return sbRequest.ToString();
        }

        public static string GetExpenseId(string Response)
        {
            string Id = "";

            try
            {
                XmlDocument oDoc = new XmlDocument();
                oDoc.LoadXml(Response);
                XmlNodeList oListVendor = oDoc.GetElementsByTagName("Purchase");

                foreach (XmlNode oNode in oListVendor)
                {
                    if (oNode["Id"] != null)
                    {
                        Id = oNode["Id"].InnerXml;
                    }
                }
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs("Check GetExpenseId Response Extract. Message:" + ex.Message);
            }

            return Id;
        }


        public static string GetExpenseSyncToken(string Response)
        {
            string SyncToken = "";

            try
            {
                XmlDocument oDoc = new XmlDocument();
                oDoc.LoadXml(Response);
                XmlNodeList oListVendor = oDoc.GetElementsByTagName("Purchase");

                foreach (XmlNode oNode in oListVendor)
                {
                    if (oNode["SyncToken"] != null)
                    {
                        SyncToken = oNode["SyncToken"].InnerXml;
                    }
                }
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs("Check GetExpenseSyncToken Response Extract. Message:" + ex.Message);
            }

            return SyncToken;
        }

        //Himanshu 02-01-2025
        public static void Update_Expense(Expense obj, List<ExpenseDetail> objDetail, ref QBResponse objRes, QBOnlineconfiguration objToken)
        {
            logger.Info("QBExpense - Update_Expense Start - " + DateTime.Now);
            string XmlRequest = "";
            try
            {
                XmlRequest = GenerateXmlRequest_UpdateExpense(obj, objDetail);
                string xmlResponse = QBRequest.LiveQBConnectionOperation_POST("POST", "/purchase", XmlRequest, objToken);
                logger.Info("QBExpense - Get Xmlresponse in Update_Expense - " + xmlResponse);

                if (xmlResponse.Contains("OK_"))
                {
                    logger.Info("QBExpense - Update_Expense Success xmlResonse  - " + xmlResponse);
                    objRes.ID = GetExpenseId(xmlResponse.Replace("OK_", "")).ToString();
                    objRes.SyncToken = GetExpenseSyncToken(xmlResponse.Replace("OK_", "")).ToString();
                    objRes.Status = "Done";
                    objRes.Message = "";
                    logger.Info("QBExpense - Update_Expense Success xmlResonse returnid  - " + objRes.ID + objRes.SyncToken);
                }
                else if (xmlResponse.Contains("Token Expired") || xmlResponse.Contains("AuthenticationFailed") || xmlResponse.Contains("Authorization"))
                {
                    objRes.ID = "0";
                    objRes.Status = "AuthenticationFailed";
                    objRes.Message = "AuthenticationFailed";
                    logger.Info("QBExpense - Update_Expense Error xmlResonse - " + xmlResponse);
                }
                else if (xmlResponse.Contains("Error") || xmlResponse.Contains("error"))
                {
                    XmlDocument oDoc = new XmlDocument();
                    oDoc.LoadXml(xmlResponse.Replace("Error:", "").Replace("error:", ""));
                    XmlNodeList oListVen = oDoc.GetElementsByTagName("Error");

                    foreach (XmlNode oNode in oListVen)
                    {
                        if (oNode["Message"] != null)
                        {
                            objRes.ID = "";
                            objRes.Status = "Expense Update Error";
                            objRes.Message = oNode["Detail"].InnerXml.ToString();
                            clsCommon.WriteErrorLogs("Update Expense Request: " + XmlRequest);
                            clsCommon.WriteErrorLogs("Update Expense Message: " + oNode["Message"].InnerXml);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objRes.Status = "Update Expense. Message:" + ex.Message;
                objRes.Message = ex.Message;
                objRes.ID = "";
                clsCommon.WriteErrorLogs("Update Expense Request: " + XmlRequest);
                clsCommon.WriteErrorLogs("Update Expense. Message:" + ex.Message);
            }
            logger.Info("QBExpense - Update_Expense End - " + DateTime.Now);
        }

        private static string GenerateXmlRequest_UpdateExpense(Expense obj, List<ExpenseDetail> objDetail)
        {
            StringBuilder sbRequest = new StringBuilder();

            try
            {
                sbRequest.Append("<Purchase xmlns=\"http://schema.intuit.com/finance/v3\" domain=\"QBO\" sparse=\"false\">");

                if (obj.ID != null)
                {
                    sbRequest.Append("<Id>" + obj.ID.ToString() + "</Id>");
                }
                if (obj.SyncToken != null)
                {
                    sbRequest.Append("<SyncToken>" + obj.SyncToken.ToString() + "</SyncToken>");
                }

                sbRequest.Append("<AccountRef>" + obj.AccountID.ToString() + "</AccountRef>");
                if (obj.VendorID != null)
                {
                    sbRequest.Append("<EntityRef type=\"" + obj.EntityType.ToString() + "\">" + obj.VendorID.ToString() + "</EntityRef>");
                }
                if (obj.TxnDate != null)
                    sbRequest.Append("<TxnDate>" + clsCommon.ConvertDate_QbFormat(obj.TxnDate.ToString()) + "</TxnDate>");
                if (obj.RefNumber != null)
                    sbRequest.Append("<DocNumber>" + clsCommon.RemoveSpecialCharacters(obj.RefNumber.ToString(), "State") + "</DocNumber>");
                if (obj.Memo != null)
                    sbRequest.Append("<PrivateNote>" + clsCommon.RemoveSpecialCharacters(obj.Memo.ToString(), "Memo") + "</PrivateNote>");
                if (obj.Amount != null)
                    sbRequest.Append("<TotalAmt>" + clsCommon.RemoveSpecialCharacters(obj.Amount.ToString(), "Amount") + "</TotalAmt>");
                if (obj.PrintStatus != null)
                    sbRequest.Append("<PrintStatus>" + clsCommon.RemoveSpecialCharacters(obj.PrintStatus.ToString(), "PrintStatus") + "</PrintStatus>");
                if (obj.PaymentMethod != null)
                    sbRequest.Append("<PaymentType>" + clsCommon.RemoveSpecialCharacters(obj.PaymentMethod.ToString(), "") + "</PaymentType>");
                if (obj.PaymentMethodId != null)
                    sbRequest.Append("<PaymentMethodRef>" + clsCommon.RemoveSpecialCharacters(obj.PaymentMethodId.ToString(), "") + "</PaymentMethodRef>");

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

                sbRequest.Append("</Purchase>");
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs("Build XML Request for UpdateExpense. Message:" + ex.Message);
            }

            logger.Info("QBExpense - GenerateXmlRequest_UpdateExpense End - " + sbRequest.ToString() + DateTime.Now);
            return sbRequest.ToString();
        }

    }
}
