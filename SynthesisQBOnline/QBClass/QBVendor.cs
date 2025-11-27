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
    public static class QBVendor
    {
        public static void CheckVendorID(string ID, QBOnlineconfiguration obj, ref QBResponse objRes)
        {
            try
            {
                clsCommon.WriteErrorLogs("Retrieve Vendor from QuickBooks.");
                string SrResponse = "";
                SrResponse = QBRequest.LiveQBConnectionOperation_GET("GET", "select%20*%20from%20vendor%20where%20ID%3d%27" + ID + "%27&minorversion=4", obj);

                if (SrResponse.Contains("OK_"))
                {
                    objRes.Status = "Get Vendor";
                    objRes.Message = "";
                    objRes.ID = GetVendorId(SrResponse.Replace("OK_", "")).ToString();
                    objRes.SyncToken = GetVendorSyncToken(SrResponse.Replace("OK_", "")).ToString();
                    objRes.QBStatus = GetVendorQBStatus(SrResponse.Replace("OK_", "")).ToString();
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
                    XmlNodeList oListVen = oDoc.GetElementsByTagName("Error");

                    foreach (XmlNode oNode in oListVen)
                    {
                        if (oNode["Message"] != null)
                        {
                            objRes.ID = "0";
                            objRes.Status = "Get Vendor Message:" + oNode["Message"].InnerXml;
                            objRes.Message = oNode["Message"].InnerXml.ToString();
                            clsCommon.WriteErrorLogs("Get Vendor Message:" + oNode["Message"].InnerXml);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs(ex.Message);
                objRes.Message = ex.Message;
                objRes.Status = "Error: Vendor not found";
            }
        }

        public static string GetVendorSyncToken(string Response)
        {
            string SyncToken = "";
            try
            {
                XmlDocument oDoc = new XmlDocument();
                oDoc.LoadXml(Response);
                XmlNodeList oListVendor = oDoc.GetElementsByTagName("Vendor");

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
                clsCommon.WriteErrorLogs("Check Vendor Response Extract. Message:" + ex.Message);
            }
            return SyncToken;
        }

        public static string GetVendorQBStatus(string Response)
        {
            string Status = "";
            try
            {
                XmlDocument oDoc = new XmlDocument();
                oDoc.LoadXml(Response);
                XmlNodeList oListVendor = oDoc.GetElementsByTagName("Vendor");

                foreach (XmlNode oNode in oListVendor)
                {
                    if (oNode["Active"] != null)
                    {
                        Status = oNode["Active"].InnerXml.ToString().ToLower();
                    }
                }
                if (Status == "true")
                    Status = "Active";
                else
                    Status = "InActive";
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs("Check Vendor Response Extract,GetVendorQBStatus Message:" + ex.Message);
            }
            return Status;
        }

        public static void CreateVendor(VendorMaster obj, ref QBResponse objRes, QBOnlineconfiguration objToken)
        {
            string XmlRequest = "";
            try
            {
                XmlRequest = GenerateXmlRequest_Vendor(obj);
                string xmlResonse = QBRequest.LiveQBConnectionOperation_POST("POST", "/vendor", XmlRequest, objToken);
                clsCommon.WriteErrorLogs(XmlRequest.ToString());
                clsCommon.WriteErrorLogs(xmlResonse.ToString());

                if (xmlResonse.Contains("OK_"))
                {
                    objRes.ID = GetVendorId(xmlResonse.Replace("OK_", "")).ToString();
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
                            objRes.Status = "Create Vendor Message:" + oNode["Message"].InnerXml;
                            objRes.Message = oNode["Message"].InnerXml.ToString();
                            clsCommon.WriteErrorLogs("Create Vendor Request: " + XmlRequest);
                            clsCommon.WriteErrorLogs("Create Vendor Message:" + oNode["Message"].InnerXml);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objRes.Status = "Create Vendor. Message:" + ex.Message;
                objRes.Message = ex.Message; objRes.ID = "";
                clsCommon.WriteErrorLogs("Create Vendor Request: " + XmlRequest);
                clsCommon.WriteErrorLogs("Create Vendor. Message:" + ex.Message);
            }
        }

        private static string GenerateXmlRequest_Vendor(VendorMaster obj)
        {
            StringBuilder sbRequest = new StringBuilder();
            try
            {
                sbRequest.Append("<Vendor xmlns=\"http://schema.intuit.com/finance/v3\" domain=\"QBO\" sparse=\"false\">");

                if (obj.DisplayName != "")
                {
                    sbRequest.Append("<DisplayName>" + clsCommon.RemoveSpecialCharacters(obj.DisplayName.ToString(), "DisplayName") + "</DisplayName>");
                }
                if (obj.PrintOnCheckas != "" && obj.PrintOnCheckas != null)
                {
                    sbRequest.Append("<PrintOnCheckName>" + clsCommon.RemoveSpecialCharacters(obj.PrintOnCheckas.ToString(), "DisplayName") + "</PrintOnCheckName>");
                }
                if (obj.FirstName != null)
                {
                    sbRequest.Append("<GivenName>" + clsCommon.RemoveSpecialCharacters(obj.FirstName.ToString(), "Customer") + "</GivenName>");
                }
                if (obj.MiddleName != null)
                {
                    sbRequest.Append("<MiddleName>" + clsCommon.RemoveSpecialCharacters(obj.MiddleName.ToString(), "Customer") + "</MiddleName>");
                }
                if (obj.LastName != null)
                {
                    sbRequest.Append("<FamilyName>" + clsCommon.RemoveSpecialCharacters(obj.LastName.ToString(), "Customer") + "</FamilyName>");
                }
                if (obj.Suffix != null)
                {
                    sbRequest.Append("<Suffix>" + clsCommon.RemoveSpecialCharacters(obj.Suffix.ToString(), "Customer") + "</Suffix>");
                }
                if (obj.CompanyName != null)
                {
                    sbRequest.Append("<CompanyName>" + clsCommon.RemoveSpecialCharacters(obj.CompanyName.ToString(), "Customer") + "</CompanyName>");
                }
                if (obj.Phone != null)
                {
                    sbRequest.Append("<PrimaryPhone><FreeFormNumber>" + clsCommon.RemoveSpecialCharacters(obj.Phone.ToString(), "Mobile") + "</FreeFormNumber></PrimaryPhone>");
                }
                if (obj.Mobile != null)
                {
                    sbRequest.Append("<Mobile><FreeFormNumber>" + clsCommon.RemoveSpecialCharacters(obj.Mobile.ToString(), "Mobile") + "</FreeFormNumber></Mobile>");
                }
                if (obj.Email != null)
                {
                    sbRequest.Append("<PrimaryEmailAddr><Address>" + clsCommon.RemoveSpecialCharacters(obj.Email.ToString(), "") + "</Address></PrimaryEmailAddr>");
                }

                sbRequest.Append("<Active>true</Active>");

                sbRequest.Append("<BillAddr>");
                if (obj.Address1 != null)
                {
                    sbRequest.Append("<Line1>" + clsCommon.RemoveSpecialCharacters(obj.Address1.ToString(), "") + "</Line1>");
                }

                if (obj.Address2 != null)
                {
                    sbRequest.Append("<Line2>" + clsCommon.RemoveSpecialCharacters(obj.Address2.ToString(), "") + "</Line2>");
                }
                if (obj.City != null)
                {
                    sbRequest.Append("<City>" + clsCommon.RemoveSpecialCharacters(obj.City.ToString(), "Country") + "</City>");
                }
                if (obj.Country != null)
                {
                    sbRequest.Append("<Country>" + clsCommon.RemoveSpecialCharacters(obj.Country.ToString(), "Country") + "</Country>");
                }
                if (obj.State != null)
                {
                    sbRequest.Append("<CountrySubDivisionCode>" + clsCommon.RemoveSpecialCharacters(obj.State.ToString(), "Country") + "</CountrySubDivisionCode>");
                }
                if (obj.ZipCode != null)
                {
                    sbRequest.Append("<PostalCode>" + obj.ZipCode.ToString() + "</PostalCode>");
                }
              
                sbRequest.Append("</BillAddr>");
                if (obj.AcctNum != "" && obj.AcctNum != null)
                {
                    sbRequest.Append("<AcctNum>" + clsCommon.RemoveSpecialCharacters(obj.AcctNum.ToString(), "") + "</AcctNum>");
                }
                sbRequest.Append("</Vendor>");
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs("Build XML Request for Vendor. Message:" + ex.Message);
            }

            return sbRequest.ToString();
        }

        public static string GetVendorId(string Response)
        {
            string Id = "";
            try
            {
                XmlDocument oDoc = new XmlDocument();
                oDoc.LoadXml(Response);
                XmlNodeList oListVen = oDoc.GetElementsByTagName("Vendor");

                foreach (XmlNode oNode in oListVen)
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
                clsCommon.WriteErrorLogs("Check Vendor Response Extract. Message:" + ex.Message);
            }

            return Id;
        }

        public static List<VendorMaster> GetVendor_All(QBOnlineconfiguration obj, ref QBResponse objRes)
        {
            List<VendorMaster> VendorList = new List<VendorMaster>();
            try
            {
                Int32 PageNumber = 1;
                Int32 StartPosition = 1;

                NextPage:
                clsCommon.WriteErrorLogs("Retrieve All Vendor from QuickBooks. Read Page Number :" + PageNumber);
                string SrResponse = "";

                SrResponse = QBRequest.LiveQBConnectionOperation_GET("GET", "select%20*%20FROM%20vendor%20where%20Active%20in(true,false)%20STARTPOSITION%20" + StartPosition + "%20MAXRESULTS%20500%20", obj);

                if (SrResponse.Contains("OK_"))
                {
                    objRes.Status = "Get Vendor";
                    objRes.Message = "";
                    ExtractResponse(SrResponse.Replace("OK_", ""), "VENDOR", ref StartPosition, ref PageNumber,ref VendorList);
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
                }
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs(ex.Message);
                objRes.Message = ex.Message;
                objRes.Status = "Error: Get Vendor, Message :" + ex.Message;
            }

            return VendorList;
        }

        public static List<VendorMaster> ExtractResponse(string Response, string Mode, ref Int32 StartPosition, ref Int32 PageNumber,ref List<VendorMaster> LstVendor)
        {
            //List<VendorMaster> LstVendor = new List<VendorMaster>();
            try
            {
                XmlDocument oDoc = new XmlDocument();
                oDoc.LoadXml(Response);

                switch (Mode)
                {
                    case "VENDOR":
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

                        XmlNodeList oListVen = oDoc.GetElementsByTagName("Vendor");
                        foreach (XmlNode oNode in oListVen)
                        {
                            try
                            {
                                VendorMaster objVendor = new VendorMaster();
                                objVendor.ID = (oNode["Id"] == null ? "0" : oNode["Id"].InnerXml);
                                objVendor.SyncToken = (oNode["SyncToken"] == null ? "" : oNode["SyncToken"].InnerXml);
                                objVendor.DisplayName = (oNode["DisplayName"] == null ? "" : oNode["DisplayName"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                objVendor.PrintOnCheckas = (oNode["PrintOnCheckName"] == null ? "" : oNode["PrintOnCheckName"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                objVendor.Title = (oNode["Title"] == null ? "" : oNode["Title"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                objVendor.FirstName = (oNode["GivenName"] == null ? "" : oNode["GivenName"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                objVendor.MiddleName = (oNode["MiddleName"] == null ? "" : oNode["MiddleName"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                objVendor.LastName = (oNode["FamilyName"] == null ? "" : oNode["FamilyName"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                objVendor.Suffix = (oNode["Suffix"] == null ? "" : oNode["Suffix"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                objVendor.CompanyName = (oNode["CompanyName"] == null ? "" : oNode["CompanyName"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                objVendor.IsActive = (oNode["Active"] == null ? "" : oNode["Active"].InnerXml);
                                if (oNode["PrimaryEmailAddr"] != null)
                                {
                                    objVendor.Email = (oNode["PrimaryEmailAddr"]["Address"] == null ? "" : oNode["PrimaryEmailAddr"]["Address"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                }
                                if (oNode["PrimaryPhone"] != null)
                                {
                                    objVendor.Phone = (oNode["PrimaryPhone"]["FreeFormNumber"] == null ? "" : oNode["PrimaryPhone"]["FreeFormNumber"].InnerXml);
                                }
                                if (oNode["Mobile"] != null)
                                {
                                    objVendor.Mobile = (oNode["Mobile"]["FreeFormNumber"] == null ? "" : oNode["Mobile"]["FreeFormNumber"].InnerXml);
                                }
                                if (oNode["Fax"] != null)
                                {
                                    objVendor.Fax = (oNode["Fax"]["FreeFormNumber"] == null ? "" : oNode["Fax"]["FreeFormNumber"].InnerXml);
                                }
                                if (oNode["WebAddr"] != null)
                                {
                                    objVendor.Website = (oNode["WebAddr"]["URI"] == null ? "" : oNode["WebAddr"]["URI"].InnerXml);
                                }
                                if (oNode["BillAddr"] != null)
                                {
                                    objVendor.Address1 = (oNode["BillAddr"]["Line1"] == null ? "" : oNode["BillAddr"]["Line1"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                    objVendor.Address2 = (oNode["BillAddr"]["Line2"] == null ? "" : oNode["BillAddr"]["Line2"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                    objVendor.City = (oNode["BillAddr"]["City"] == null ? "" : oNode["BillAddr"]["City"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                    objVendor.State = (oNode["BillAddr"]["CountrySubDivisionCode"] == null ? "" : oNode["BillAddr"]["CountrySubDivisionCode"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                    objVendor.Country = (oNode["BillAddr"]["Country"] == null ? "" : oNode["BillAddr"]["Country"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                                    objVendor.ZipCode = (oNode["BillAddr"]["PostalCode"] == null ? "" : oNode["BillAddr"]["PostalCode"].InnerXml);
                                }
                                if (oNode["AcctNum"] != null)
                                {
                                    objVendor.AcctNum = (oNode["AcctNum"] == null ? "" : oNode["AcctNum"].InnerXml);
                                }
                                LstVendor.Add(objVendor);
                                objVendor = null;
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        break;

                }
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs("Extract Resonse. Mode:" + Mode + ". Message:" + ex.Message);
            }
            return LstVendor;
        }

        public static void ModVendor(VendorMaster obj, ref QBResponse objRes, QBOnlineconfiguration objToken)
        {
            try
            {
                string XmlRequest = GenerateXmlRequest_ModVendor(obj);
                string xmlResonse = QBRequest.LiveQBConnectionOperation_POST("POST", "/vendor", XmlRequest, objToken);

                if (xmlResonse.Contains("OK_"))
                {
                    objRes.ID = GetVendorId(xmlResonse.Replace("OK_", "")).ToString();
                    objRes.SyncToken = GetVendorSyncToken(xmlResonse.Replace("OK_", "")).ToString();
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
                            objRes.Status = "Vendor Update Error";
                            objRes.Message = oNode["Message"].InnerXml.ToString();
                            clsCommon.WriteErrorLogs("Create Vendor Message: " + oNode["Message"].InnerXml);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objRes.Status = "Update Vendor. Message:" + ex.Message;
                objRes.Message = ex.Message;
                objRes.ID = "";
                clsCommon.WriteErrorLogs("Update Vendor. Message:" + ex.Message);
            }
        }

        private static string GenerateXmlRequest_ModVendor(VendorMaster obj)
        {
            StringBuilder sbRequest = new StringBuilder();

            try
            {
                sbRequest.Append("<Vendor xmlns=\"http://schema.intuit.com/finance/v3\" domain=\"QBO\" sparse=\"false\">");

                sbRequest.Append("<Id>" + obj.ID.ToString() + "</Id>");
                sbRequest.Append("<SyncToken>" + obj.SyncToken.ToString() + "</SyncToken>");
                if (obj.DisplayName != "")
                {
                    sbRequest.Append("<DisplayName>" + clsCommon.RemoveSpecialCharacters(obj.DisplayName.ToString(), "DisplayName") + "</DisplayName>");
                }
                if (obj.PrintOnCheckas != "" && obj.PrintOnCheckas != null)
                {
                    sbRequest.Append("<PrintOnCheckName>" + clsCommon.RemoveSpecialCharacters(obj.PrintOnCheckas.ToString(), "DisplayName") + "</PrintOnCheckName>");
                }
                if (obj.FirstName != null)
                {
                    sbRequest.Append("<GivenName>" + clsCommon.RemoveSpecialCharacters(obj.FirstName.ToString(), "Customer") + "</GivenName>");
                }
                if (obj.MiddleName != null)
                {
                    sbRequest.Append("<MiddleName>" + clsCommon.RemoveSpecialCharacters(obj.MiddleName.ToString(), "Customer") + "</MiddleName>");
                }
                if (obj.LastName != null)
                {
                    sbRequest.Append("<FamilyName>" + clsCommon.RemoveSpecialCharacters(obj.LastName.ToString(), "Customer") + "</FamilyName>");
                }
                if (obj.Suffix != null)
                {
                    sbRequest.Append("<Suffix>" + clsCommon.RemoveSpecialCharacters(obj.Suffix.ToString(), "Customer") + "</Suffix>");
                }
                if (obj.CompanyName != null)
                {
                    sbRequest.Append("<CompanyName>" + clsCommon.RemoveSpecialCharacters(obj.CompanyName.ToString(), "Customer") + "</CompanyName>");
                }
                if (obj.Phone != null)
                {
                    sbRequest.Append("<PrimaryPhone><FreeFormNumber>" + clsCommon.RemoveSpecialCharacters(obj.Phone.ToString(), "Mobile") + "</FreeFormNumber></PrimaryPhone>");
                }
                if (obj.Mobile != null)
                {
                    sbRequest.Append("<Mobile><FreeFormNumber>" + clsCommon.RemoveSpecialCharacters(obj.Mobile.ToString(), "Mobile") + "</FreeFormNumber></Mobile>");
                }
                if (obj.Email != null)
                {
                    sbRequest.Append("<PrimaryEmailAddr><Address>" + clsCommon.RemoveSpecialCharacters(obj.Email.ToString(), "") + "</Address></PrimaryEmailAddr>");
                }
                sbRequest.Append("<Active>"+ obj.IsActive.ToString() + "</Active>");

                sbRequest.Append("<BillAddr>");
                if (obj.Address1 != null)
                {
                    sbRequest.Append("<Line1>" + clsCommon.RemoveSpecialCharacters(obj.Address1.ToString(), "") + "</Line1>");
                }

                if (obj.Address2 != null)
                {
                    sbRequest.Append("<Line2>" + clsCommon.RemoveSpecialCharacters(obj.Address2.ToString(), "") + "</Line2>");
                }
                if (obj.City != null)
                {
                    sbRequest.Append("<City>" + clsCommon.RemoveSpecialCharacters(obj.City.ToString(), "Country") + "</City>");
                }
                if (obj.Country != null)
                {
                    sbRequest.Append("<Country>" + clsCommon.RemoveSpecialCharacters(obj.Country.ToString(), "Country") + "</Country>");
                }
                if (obj.State != null)
                {
                    sbRequest.Append("<CountrySubDivisionCode>" + clsCommon.RemoveSpecialCharacters(obj.State.ToString(), "Country") + "</CountrySubDivisionCode>");
                }
                if (obj.ZipCode != null)
                {
                    sbRequest.Append("<PostalCode>" + obj.ZipCode.ToString() + "</PostalCode>");
                }
              
                sbRequest.Append("</BillAddr>");
                if (obj.AcctNum != "" && obj.AcctNum != null)
                {
                    sbRequest.Append("<AcctNum>" + clsCommon.RemoveSpecialCharacters(obj.AcctNum.ToString(), "") + "</AcctNum>");
                }
                sbRequest.Append("</Vendor>");
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs("Build XML Request for Vendor. Message:" + ex.Message);
            }
            return sbRequest.ToString();
        }

        
    }
}
