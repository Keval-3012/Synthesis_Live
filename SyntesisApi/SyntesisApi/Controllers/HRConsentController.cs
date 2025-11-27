using iTextSharp.text.pdf;
using NLog;
using SyntesisApi.BAL;
using SyntesisApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using static SyntesisApi.BAL.BALHREmployee;
using static SyntesisApi.Models.ApiModel;

namespace SyntesisApi.Controllers
{
    public class HRConsentController : ApiController
    {
        DataTable Dt1 = new DataTable();
        ResponseModel Response = new ResponseModel();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #region Start Harsh 11-03-2024
        [Authorize]
        [HttpPost]
        [Route("api/HRConsent/Consent")]
        public async Task<IHttpActionResult> SaveConsent([FromBody] GetConcentData objConcent)
        {
            try
            {
                logger.Info("HRConsentController - SaveConsent - " + DateTime.Now.ToString());

                if (objConcent != null)
                {
                    string number = String.Format("{0:d9}", (DateTime.Now.Ticks / 10) % 1000000000);
                   
                    string synthesisRepoPath = ConfigurationManager.AppSettings["SynthesisRepoPath"];
                    string employeeDocumentPath = Path.Combine(synthesisRepoPath, "EmployeeDocument");
                    string empFolderPath = Path.Combine(employeeDocumentPath, objConcent.EmployeeID.ToString());

                    if (!Directory.Exists(empFolderPath))
                    {
                        Directory.CreateDirectory(empFolderPath);
                    }

                    string pdfTemplate = "";
                    if (objConcent.LanguageID == "1")
                    {
                        pdfTemplate = Path.Combine(synthesisRepoPath, "PDF\\Concert\\Consent.pdf");
                    }
                    else
                    {
                        pdfTemplate = Path.Combine(synthesisRepoPath, "PDF\\Concert\\Consent_Span.pdf");
                    }
                    string FileName = "Consent_" + number + ".pdf";

                    var newFile = Path.Combine(synthesisRepoPath, empFolderPath + "\\" + FileName);


                    PdfReader pdfReader = new PdfReader(pdfTemplate);
                    PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFile, FileMode.Create));
                    AcroFields pdfFormFields = pdfStamper.AcroFields;
                    pdfStamper.AcroFields.SetFieldProperty("names", "textsize", 4f, null);

                    pdfFormFields.SetField("BusinessName", objConcent.StoreName);
                    pdfFormFields.SetField("ManagerName", objConcent.StoreManagerName);
                    pdfFormFields.SetField("Date", objConcent.Date);
                    pdfFormFields.SetField("EmployeeName", objConcent.EmployeeName);
                    String[] checkboxstates = pdfFormFields.GetAppearanceStates("Chk1");
                    pdfFormFields.SetField("Chk1", (objConcent.Chk1 == "true" ? checkboxstates[1] == "Off"? checkboxstates[0] : checkboxstates[1]: checkboxstates[0]), true);
                    String[] checkboxstates2 = pdfFormFields.GetAppearanceStates("Chk2");
                    pdfFormFields.SetField("Chk2", (objConcent.Chk2 == "true" ? checkboxstates2[1] == "Off" ? checkboxstates[0] : checkboxstates[1] : checkboxstates[0]), true);
                    pdfFormFields.SetField("Signature", objConcent.SignatureFileName.Replace("data:image/png;base64,", ""));
                    BaseFont bold = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
                    pdfFormFields.SetFieldProperty("BusinessName1", "textfont", bold, null);
                    pdfFormFields.SetField("BusinessName1", objConcent.EmployeeName);

                    pdfStamper.FormFlattening = false;
                    pdfStamper.Close();

                    EmployeeDocument objDetail = new EmployeeDocument();
                    objDetail.EmployeeID = Convert.ToInt32(objConcent.EmployeeID);
                    objDetail.DocFileName = FileName;
                    objDetail.LocationFrom = 2;
                    objDetail.DocumentType = ".pdf";
                    objDetail.DocumentTypeId = (int)DocumentType.Consent;
                    objDetail.CreatedID = 0;
                    objDetail.Status = true;
                    objDetail.Comment = "";
                    objDetail.StoreId = Convert.ToInt32(objConcent.StoreId);
                    objDetail.LanguageID = Convert.ToInt32(objConcent.LanguageID);
                    objDetail.EmployeeChildID = objConcent.EmployeeChildID;
                    objDetail.CreatedID = objConcent.CreatedId;
                    new BALHRConsent().saveEmpDoc(objDetail);


                    StreamWriter sw = new StreamWriter(Path.Combine(synthesisRepoPath, "PDF\\Test.txt"), false);
                    sw.WriteLine("BusinessName:" + objConcent.StoreName);
                    sw.WriteLine("EmpID:" + objConcent.EmployeeID);
                    sw.WriteLine("ManagerName:" + objConcent.StoreManagerName);
                    sw.WriteLine("Date:" + objConcent.Date);
                    sw.WriteLine("EmployeeName:" + objConcent.EmployeeName);
                    sw.WriteLine("Chk1:" + objConcent.Chk1);
                    sw.WriteLine("Chk2:" + objConcent.Chk2);
                    sw.WriteLine("Chk3:" + objConcent.Chk3);
                    sw.WriteLine("Signature:" + objConcent.SignatureFileName);
                    sw.Close();

                    Response.responseStatus = "200";
                    Response.message = "Consent Status Updated Successfully!!";
                    logger.Info("HRConsentController - SaveConsent - " + DateTime.Now.ToString() + " - " + "Consent Status Updated Successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Someting Went Wrong!";
                    logger.Error("HRConsentController - SaveConsent - " + DateTime.Now.ToString() + " - " + "Someting Went Wrong!");
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("HRConsentController - SaveConsent - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
            
        }
        #endregion
    }
}
