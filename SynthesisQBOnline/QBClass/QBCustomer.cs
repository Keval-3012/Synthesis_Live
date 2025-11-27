using SynthesisQBOnline.BAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SynthesisQBOnline.QBClass
{
    public static class QBCustomer
    {

        public static List<CustomerMaster> GetCustomer_All(QBOnlineconfiguration obj, ref QBResponse objRes)
        {
            List<CustomerMaster> CustomerList = new List<CustomerMaster>();
            try
            {
                Int32 PageNumber = 1;
                Int32 StartPosition = 1;

                NextPage:
                clsCommon.WriteErrorLogs("GetCustomer_All. Read Page Number :" + PageNumber);
                string SrResponse = "";

                SrResponse = QBRequest.LiveQBConnectionOperation_GET("GET", "select%20*%20FROM%20customer%20where%20Active%20in(true,false)%20STARTPOSITION%20" + StartPosition + "%20MAXRESULTS%20500%20", obj);

                if (SrResponse.Contains("OK_"))
                {
                    objRes.Status = "Get Customer";
                    objRes.Message = "";
                    ExtractResponse(SrResponse.Replace("OK_", ""), "CUSTOMER", ref StartPosition, ref PageNumber, ref CustomerList);
                    if (PageNumber != -1)
                    {
                        goto NextPage;
                    }
                }
                else if (SrResponse.Contains("Token Expired") || SrResponse.Contains("AuthenticationFailed") || SrResponse.Contains("Authorization"))
                {
                    objRes.ID = "0";
                    objRes.Status = "AuthenticationFailed";
                    objRes.Message = "AuthenticationFailed";
                    clsCommon.WriteErrorLogs("GetCustomer_All : AuthenticationFailed");
                }
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs("GetCustomer_All. Error: " + ex.InnerException.ToString() + "  Message: " + ex.Message.ToString());
                objRes.Message = ex.Message;
                objRes.Status = "Error: Get Customer, Message :" + ex.Message;
            }

            return CustomerList;
        }

        public static List<CustomerMaster> ExtractResponse(string Response, string Mode, ref Int32 StartPosition, ref Int32 PageNumber, ref List<CustomerMaster> LstVendor)
        {
            try
            {
                XmlDocument oDoc = new XmlDocument();
                oDoc.LoadXml(Response);

                switch (Mode)
                {
                    case "CUSTOMER":
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

                        XmlNodeList oListVen = oDoc.GetElementsByTagName("Customer");
                        foreach (XmlNode oNode in oListVen)
                        {
                            try
                            {
                                CustomerMaster objVendor = new CustomerMaster();
                                objVendor.ID = (oNode["Id"] == null ? "0" : oNode["Id"].InnerXml);
                                objVendor.SyncToken = (oNode["SyncToken"] == null ? "" : oNode["SyncToken"].InnerXml);
                                objVendor.DisplayName = (oNode["DisplayName"] == null ? "" : oNode["DisplayName"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                objVendor.PrintOnCheckName = (oNode["PrintOnCheckName"] == null ? "" : oNode["PrintOnCheckName"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                objVendor.FirstName = (oNode["GivenName"] == null ? "" : oNode["GivenName"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                objVendor.MiddleName = (oNode["MiddleName"] == null ? "" : oNode["MiddleName"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                objVendor.Active = (oNode["Active"] == null ? "" : oNode["Active"].InnerXml);
                                objVendor.LastName = (oNode["FamilyName"] == null ? "" : oNode["FamilyName"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                objVendor.CompanyName = (oNode["CompanyName"] == null ? "" : oNode["CompanyName"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));

                                objVendor.Notes = (oNode["Notes"] == null ? "" : oNode["Notes"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                objVendor.Balance = (oNode["Balance"] == null ? "0" : oNode["Balance"].InnerXml);

                                if (oNode["PrimaryEmailAddr"] != null)
                                {
                                    objVendor.PrimaryEmailAddr = (oNode["PrimaryEmailAddr"]["Address"] == null ? "" : oNode["PrimaryEmailAddr"]["Address"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                }
                                if (oNode["PrimaryPhone"] != null)
                                {
                                    objVendor.PrimaryPhone = (oNode["PrimaryPhone"]["FreeFormNumber"] == null ? "" : oNode["PrimaryPhone"]["FreeFormNumber"].InnerXml);
                                }


                                if (oNode["BillAddr"] != null)
                                {
                                    objVendor.BAddress1 = (oNode["BillAddr"]["Line1"] == null ? "" : oNode["BillAddr"]["Line1"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                    objVendor.BAddress2 = (oNode["BillAddr"]["Line2"] == null ? "" : oNode["BillAddr"]["Line2"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                    objVendor.BCity = (oNode["BillAddr"]["City"] == null ? "" : oNode["BillAddr"]["City"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                    objVendor.BState = (oNode["BillAddr"]["CountrySubDivisionCode"] == null ? "" : oNode["BillAddr"]["CountrySubDivisionCode"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                    objVendor.BCountry = (oNode["BillAddr"]["Country"] == null ? "" : oNode["BillAddr"]["Country"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                    objVendor.BZipCode = (oNode["BillAddr"]["PostalCode"] == null ? "" : oNode["BillAddr"]["PostalCode"].InnerXml);
                                }

                                if (oNode["ShipAddr"] != null)
                                {
                                    objVendor.SAddress1 = (oNode["ShipAddr"]["Line1"] == null ? "" : oNode["ShipAddr"]["Line1"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                    objVendor.SAddress2 = (oNode["ShipAddr"]["Line2"] == null ? "" : oNode["ShipAddr"]["Line2"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                    objVendor.SCity = (oNode["ShipAddr"]["City"] == null ? "" : oNode["ShipAddr"]["City"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                    objVendor.SState = (oNode["ShipAddr"]["CountrySubDivisionCode"] == null ? "" : oNode["ShipAddr"]["CountrySubDivisionCode"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                    objVendor.SCountry = (oNode["ShipAddr"]["Country"] == null ? "" : oNode["ShipAddr"]["Country"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                    objVendor.SZipCode = (oNode["ShipAddr"]["PostalCode"] == null ? "" : oNode["ShipAddr"]["PostalCode"].InnerXml);
                                }


                                LstVendor.Add(objVendor);
                                objVendor = null;
                            }
                            catch (Exception ex)
                            {
                                clsCommon.WriteErrorLogs("Customer_ExtractResponse. Error: " + ex.InnerException.ToString() + "  Message: " + ex.Message.ToString());
                            }
                        }
                        break;

                }
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs("Customer_ExtractResponse. Mode:" + Mode + ". Message:" + ex.Message);
            }
            return LstVendor;
        }

        public static void CheckCustomerID(string ID, QBOnlineconfiguration obj, ref QBResponse objRes)
        {
            try
            {
                clsCommon.WriteErrorLogs("Retrieve Customer from QuickBooks.");
                string SrResponse = "";
                SrResponse = QBRequest.LiveQBConnectionOperation_GET("GET", "select%20*%20from%20customer%20where%20ID%3d%27" + ID + "%27&minorversion=4", obj);

                if (SrResponse.Contains("OK_"))
                {
                    objRes.Status = "Get Customer";
                    objRes.Message = "";
                    objRes.ID = GetCustomerId(SrResponse.Replace("OK_", "")).ToString();
                    objRes.SyncToken = GetCustomerSyncToken(SrResponse.Replace("OK_", "")).ToString();
                }
                else if (SrResponse.Contains("Token Expired") || SrResponse.Contains("AuthenticationFailed") || SrResponse.Contains("Authorization"))
                {
                    objRes.ID = "0";
                    objRes.Status = "AuthenticationFailed";
                    objRes.Message = "AuthenticationFailed";
                    clsCommon.WriteErrorLogs("CheckCustomerID. : AuthenticationFailed:");
                }
                else if (SrResponse.Contains("Error") || SrResponse.Contains("error"))
                {
                    XmlDocument oDoc = new XmlDocument();
                    oDoc.LoadXml(SrResponse.Replace("Error:", "").Replace("error:", ""));
                    XmlNodeList oListVen = oDoc.GetElementsByTagName("Error");

                    foreach (XmlNode oNode in oListVen)
                    {
                        if (oNode["Message"] != null)
                        {
                            objRes.ID = "0";
                            objRes.Status = "Get Customer Message:" + oNode["Message"].InnerXml;
                            objRes.Message = oNode["Message"].InnerXml.ToString();
                            clsCommon.WriteErrorLogs("CheckCustomerID :" + oNode["Message"].InnerXml);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs("CheckCustomerID Error: " + ex.InnerException.ToString() + "  Message:" + ex.Message.ToString());
                objRes.Message = ex.Message;
                objRes.Status = "Error: Customer not found";
            }
        }

        public static string GetCustomerId(string Response)
        {
            string Id = "";
            try
            {
                XmlDocument oDoc = new XmlDocument();
                oDoc.LoadXml(Response);
                XmlNodeList oListVen = oDoc.GetElementsByTagName("Customer");

                foreach (XmlNode oNode in oListVen)
                {
                    if (oNode["Id"] != null)
                    {
                        Id = oNode["Id"].InnerXml;
                    }

                }
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs("GetCustomerId Error: " + ex.InnerException.ToString() + "  Message:" + ex.Message.ToString());
            }

            return Id;
        }

        public static string GetCustomerSyncToken(string Response)
        {
            string SyncToken = "";
            try
            {
                XmlDocument oDoc = new XmlDocument();
                oDoc.LoadXml(Response);
                XmlNodeList oListVendor = oDoc.GetElementsByTagName("Customer");

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
                clsCommon.WriteErrorLogs("GetCustomerSyncToken Error: " + ex.InnerException.ToString() + "  Message:" + ex.Message.ToString());
            }
            return SyncToken;
        }


    }
}
