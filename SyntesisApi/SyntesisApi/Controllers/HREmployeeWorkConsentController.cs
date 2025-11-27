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
using static SyntesisApi.Models.ApiModel;

namespace SyntesisApi.Controllers
{
    public class HREmployeeWorkConsentController : ApiController
    {
        ResponseModel Response = new ResponseModel();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #region Start Harsh 11-03-2024
        [Authorize]
        [HttpPost]
        [Route("api/HREmployeeWorkConsent/EmployeeWorkConsent")]
        public async Task<IHttpActionResult> EmployeeWorkConsent([FromBody] EmployeeWorkConsent objEmp)
        {
            try
            {
                logger.Info("HREmployeeWorkConsentController - EmployeeWorkConsent - " + DateTime.Now.ToString());

                if (objEmp != null)
                {
                    string number = String.Format("{0:d9}", (DateTime.Now.Ticks / 10) % 1000000000);

                    string synthesisRepoPath = ConfigurationManager.AppSettings["SynthesisRepoPath"];
                    string employeeDocumentPath = Path.Combine(synthesisRepoPath, "EmployeeDocument");
                    string empFolderPath = Path.Combine(employeeDocumentPath, objEmp.EmployeeID.ToString());

                    if (!Directory.Exists(empFolderPath))
                    {
                        Directory.CreateDirectory(empFolderPath);
                    }
                    string pdfTemplate = "";
                    if (objEmp.languageId == "2")
                    {
                        pdfTemplate = Path.Combine(synthesisRepoPath, "PDF\\Concert\\ConsentOfWork_Span.pdf");
                    }
                    else
                    {
                        pdfTemplate = Path.Combine(synthesisRepoPath, "PDF\\Concert\\ConsentOfWork.pdf");
                    }
                    string FileName = "ScheduleChange_" + number + ".pdf";
                    var newFile = Path.Combine(synthesisRepoPath, empFolderPath + "\\" + FileName);

                    PdfReader pdfReader = new PdfReader(pdfTemplate);
                    PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFile, FileMode.Create));
                    AcroFields pdfFormFields = pdfStamper.AcroFields;
                    pdfFormFields.SetField("CompanyName", objEmp.CompanyName);
                    pdfFormFields.SetField("Date", DateTime.Now.ToString("MM-dd-yyyy"));
                    pdfFormFields.SetField("EmployeeName", objEmp.EmployeeName);
                    pdfFormFields.SetField("SMName", objEmp.StoreManagerName);
                    pdfFormFields.SetField("SMName1", objEmp.StoreManagerName);
                    pdfFormFields.SetField("Signature", objEmp.EmployeeSignature.Replace("data:image/png;base64,", ""));
                    pdfFormFields.SetField("SMSignature", objEmp.StoreManagerSignature.Replace("data:image/png;base64,", ""));

                    int iCount = 1;
                    foreach (var item in objEmp.EmployeeDetails)
                    {
                        pdfFormFields.SetField("DT" + iCount, item.DateTimeOfRequest);
                        pdfFormFields.SetField("OS" + iCount, item.OriginalShift);
                        pdfFormFields.SetField("RS" + iCount, item.RequestedShift);
                        iCount += 1;
                    }
                    pdfStamper.FormFlattening = false;
                    pdfStamper.Close();
                    pdfReader.Close();
                    pdfReader.Dispose();

                    int iEmpId = Convert.ToInt32(objEmp.EmployeeID);
                    EmployeeDocument objDetail = new EmployeeDocument();
                    objDetail.EmployeeID = Convert.ToInt32(objEmp.EmployeeID);
                    objDetail.DocFileName = FileName;
                    objDetail.LocationFrom = 2;
                    objDetail.DocumentType = ".pdf";
                    objDetail.DocumentTypeId = (int)DocumentType.ScheduleChange;
                    objDetail.CreatedID = 0;
                    objDetail.Status = true;
                    objDetail.Comment = "";
                    objDetail.StoreId = Convert.ToInt32(objEmp.StoreId);
                    objDetail.CreatedID = objEmp.CreatedId;
                    objDetail.EmployeeChildID = objEmp.EmployeeChildID;
                    objDetail.LanguageID = Convert.ToInt32(objEmp.languageId);
                    new BALHRConsent().saveEmpDoc(objDetail);
                    objDetail = null;

                    StreamWriter sw = new StreamWriter(Path.Combine(synthesisRepoPath, "PDF\\Test.txt"), false);
                    sw.WriteLine("CompanyName:" + objEmp.CompanyName);
                    sw.WriteLine("EmpID:" + objEmp.EmployeeID);
                    sw.WriteLine("EmployeeName:" + objEmp.EmployeeName);
                    sw.WriteLine("Date:" + objEmp.Date);
                    sw.WriteLine("SMName:" + objEmp.StoreManagerName);
                    sw.WriteLine("EmployeeSignature:" + objEmp.EmployeeSignature);
                    sw.WriteLine("SMSignature:" + objEmp.StoreManagerSignature);
                    sw.Close();

                    Response.responseStatus = "200";
                    Response.message = "Schedule Change Submitted Successfully!!";
                    logger.Info("HREmployeeWorkConsentController - EmployeeWorkConsent - " + DateTime.Now.ToString() + " - " + "Schedule Change Submitted Successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Someting Went Wrong!";
                    logger.Error("HREmployeeWorkConsentController - EmployeeWorkConsent - " + DateTime.Now.ToString() + " - " + "Someting Went Wrong!");
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("HREmployeeWorkConsentController - EmployeeWorkConsent - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }

        }
        #endregion
    }
}
