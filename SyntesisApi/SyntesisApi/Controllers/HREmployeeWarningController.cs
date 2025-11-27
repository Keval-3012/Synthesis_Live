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
    public class HREmployeeWarningController : ApiController
    {
        DataTable Dt1 = new DataTable();
        ResponseModel Response = new ResponseModel();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [Authorize]
        [HttpPost]
        [Route("api/HREmployeeWarning/SaveEmployeeWarning")]
        public async Task<IHttpActionResult> SaveEmployeeWarning([FromBody] EmployeeWarning objWarning)
        {
            try
            {
                logger.Info("HREmployeeWarningController - SaveEmployeeWarning - " + DateTime.Now.ToString());

                if (objWarning != null)
                {
                    string number = String.Format("{0:d9}", (DateTime.Now.Ticks / 10) % 1000000000);

                    string synthesisRepoPath = ConfigurationManager.AppSettings["SynthesisRepoPath"];
                    string employeeDocumentPath = Path.Combine(synthesisRepoPath, "EmployeeWarning");
                    string empFolderPath = Path.Combine(employeeDocumentPath, objWarning.EmployeeID.ToString());

                    if (!Directory.Exists(empFolderPath))
                    {
                        Directory.CreateDirectory(empFolderPath);
                    }

                    string pdfTemplate = "";
                    if (objWarning.LanguageId == 1)
                    {
                        pdfTemplate = Path.Combine(synthesisRepoPath, "PDF\\Concert\\EmployeeWarning.pdf");
                    }
                    else
                    {
                        pdfTemplate = Path.Combine(synthesisRepoPath, "PDF\\Concert\\EmployeeWarning_Span.pdf");
                    }
                    string FileName = "EmployeeWarning_" + number + ".pdf";

                    var newFile = Path.Combine(synthesisRepoPath, empFolderPath + "\\" + FileName);

                    PdfReader pdfReader = new PdfReader(pdfTemplate);
                    PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFile, FileMode.Create));
                    AcroFields pdfFormFields = pdfStamper.AcroFields;
                    pdfStamper.AcroFields.SetFieldProperty("names", "textsize", 4f, null);

                    pdfFormFields.SetField("BusinessName", objWarning.StoreName);
                    pdfFormFields.SetField("ManagerName", objWarning.StoreManagerName);
                    pdfFormFields.SetField("Date", DateTime.Now.ToString("MM-dd-yyyy"));
                    pdfFormFields.SetField("EmployeeName", objWarning.EmployeeName);
                    String[] checkboxstates = pdfFormFields.GetAppearanceStates("Chk1");
                    pdfFormFields.SetField("Chk1", (objWarning.Chk1 == "true" ? checkboxstates[0] : checkboxstates[1]), true);
                    String[] checkboxstates2 = pdfFormFields.GetAppearanceStates("Chk2");
                    pdfFormFields.SetField("Chk2", (objWarning.Chk2 == "true" ? checkboxstates2[0] : checkboxstates2[1]), true);
                    String[] checkboxstates3 = pdfFormFields.GetAppearanceStates("Chk3");
                    pdfFormFields.SetField("Chk3", (objWarning.Chk3 == "true" ? checkboxstates3[0] : checkboxstates3[1]), true);
                    pdfFormFields.SetField("Remarks", objWarning.Remarks);
                    pdfFormFields.SetField("Signature", objWarning.EmployeeSignature.Replace("data:image/png;base64,", ""));
                    pdfFormFields.SetField("Signature0", objWarning.StoreManagerSignature.Replace("data:image/png;base64,", ""));
                    BaseFont bold = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
                    pdfStamper.FormFlattening = false;
                    pdfStamper.Close();

                    EmployeeWarning objDetail = new EmployeeWarning();
                    objDetail.EmployeeID = Convert.ToInt32(objWarning.EmployeeID);
                    objDetail.EmployeeChildID = Convert.ToInt32(objWarning.EmployeeChildID);
                    objDetail.Warning = objWarning.Chk1 == "true" ? "First" : (objWarning.Chk2 == "true" ? "Second" : "Final");
                    objDetail.Remarks = "";
                    objDetail.DocFileName = FileName;
                    objDetail.CreatedBy = Convert.ToInt32(objWarning.CreatedBy);
                    objDetail.StoreId = Convert.ToInt32(objWarning.StoreId);
                    objDetail.LanguageId = Convert.ToInt32(objWarning.LanguageId);
                    new BALHRWarning().SaveEmployeeWarning(objDetail);

                    StreamWriter sw = new StreamWriter(Path.Combine(synthesisRepoPath, "PDF\\Test.txt"), false);
                    sw.WriteLine("BusinessName:" + objWarning.StoreName);
                    sw.WriteLine("EmpID:" + objWarning.EmployeeID);
                    sw.WriteLine("ManagerName:" + objWarning.StoreManagerName);
                    sw.WriteLine("Date:" + DateTime.Now.ToString("MM-dd-yyyy"));
                    sw.WriteLine("EmployeeName:" + objWarning.EmployeeName);
                    sw.WriteLine("Chk1:" + objWarning.Chk1);
                    sw.WriteLine("Chk2:" + objWarning.Chk2);
                    sw.WriteLine("Chk3:" + objWarning.Chk3);
                    sw.WriteLine("Remarks:" + objWarning.Remarks);
                    sw.WriteLine("Signature:" + objWarning.EmployeeSignature);
                    sw.WriteLine("Signature0:" + objWarning.StoreManagerSignature);
                    sw.Close();

                    Response.responseStatus = "200";
                    Response.message = "Employee Warning Sent Successfully!!";
                    logger.Info("HREmployeeWarningController - SaveEmployeeWarning - " + DateTime.Now.ToString() + " - " + "Employee Warning Sent Successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Someting Went Wrong!";
                    logger.Error("HREmployeeWarningController - SaveEmployeeWarning - " + DateTime.Now.ToString() + " - " + "Someting Went Wrong!");
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("HREmployeeWarningController - SaveEmployeeWarning - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }

        }
    }
}