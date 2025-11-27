using SynthesisQBOnline.BAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SynthesisQBOnline.QBClass
{
    public static class QBPaymentMethod
    {
        public static List<PaymentMethod> GetPaymentMethod_All(QBOnlineconfiguration obj, ref QBResponse objRes)
        {
            List<PaymentMethod> VendorList = new List<PaymentMethod>();
            try
            {
                Int32 PageNumber = 1;
                Int32 StartPosition = 1;

                NextPage:
                clsCommon.WriteErrorLogs("Retrieve All PaymentMethod from QuickBooks. Read Page Number :" + PageNumber);
                string SrResponse = "";

                SrResponse = QBRequest.LiveQBConnectionOperation_GET("GET", "select%20*%20FROM%20PaymentMethod%20where%20Active%20in(true,false)%20STARTPOSITION%20" + StartPosition + "%20MAXRESULTS%20500%20", obj);

                if (SrResponse.Contains("OK_"))
                {
                    objRes.Status = "Get Vendor";
                    objRes.Message = "";
                    ExtractResponse(SrResponse.Replace("OK_", ""), ref StartPosition, ref PageNumber, ref VendorList);
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
                objRes.Status = "Error: Get PaymentMethod, Message :" + ex.Message;
            }

            return VendorList;
        }
        public static List<PaymentMethod> ExtractResponse(string Response, ref Int32 StartPosition, ref Int32 PageNumber, ref List<PaymentMethod> LstVendor)
        {
            //List<VendorMaster> LstVendor = new List<VendorMaster>();
            try
            {
                XmlDocument oDoc = new XmlDocument();
                oDoc.LoadXml(Response);


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

                XmlNodeList oListVen = oDoc.GetElementsByTagName("PaymentMethod");
                foreach (XmlNode oNode in oListVen)
                {
                    try
                    {
                        PaymentMethod objMethod = new PaymentMethod();
                        objMethod.Id = (oNode["Id"] == null ? "0" : oNode["Id"].InnerXml);
                        objMethod.Name = (oNode["Name"] == null ? "" : oNode["Name"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                        objMethod.Active = (oNode["Active"] == null ? "" : oNode["Active"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));
                        objMethod.type = (oNode["Type"] == null ? "" : oNode["Type"].InnerXml.Replace("&amp;", "&").Replace("&apos;", "'"));

                        LstVendor.Add(objMethod);
                        objMethod = null;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                clsCommon.WriteErrorLogs("ExtractResponse, Message:" + ex.Message);
            }
            return LstVendor;
        }

    }
}
