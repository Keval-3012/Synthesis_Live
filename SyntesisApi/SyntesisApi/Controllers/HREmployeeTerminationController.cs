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
    public class HREmployeeTerminationController : ApiController
    {
        DataTable Dt1 = new DataTable();
        ResponseModel Response = new ResponseModel();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [Authorize]
        [HttpPost]
        [Route("api/HREmployeeTermination/SaveEmployeeTermination")]
        public async Task<IHttpActionResult> SaveEmployeeTermination([FromBody] EmployeeTermination objTermination)
        {
            try
            {
                logger.Info("HREmployeeTerminationController - SaveEmployeeTermination - " + DateTime.Now.ToString());

                if (objTermination != null)
                {
                    string number = String.Format("{0:d9}", (DateTime.Now.Ticks / 10) % 1000000000);

                    string synthesisRepoPath = ConfigurationManager.AppSettings["SynthesisRepoPath"];
                    string employeeDocumentPath = Path.Combine(synthesisRepoPath, "EmployeeTermination");
                    string empFolderPath = Path.Combine(employeeDocumentPath, objTermination.EmployeeID.ToString());

                    if (!Directory.Exists(empFolderPath))
                    {
                        Directory.CreateDirectory(empFolderPath);
                    }

                    string pdfTemplate = "";
                    if (objTermination.LanguageId == 1)
                    {
                        pdfTemplate = Path.Combine(synthesisRepoPath, "PDF\\Concert\\EmployeeTermination.pdf");
                    }
                    else
                    {
                        pdfTemplate = Path.Combine(synthesisRepoPath, "PDF\\Concert\\EmployeeTermination_Span.pdf");
                    }
                    string FileName = "EmployeeTermination_" + number + ".pdf";

                    var newFile = Path.Combine(synthesisRepoPath, empFolderPath + "\\" + FileName);

                    PdfReader pdfReader = new PdfReader(pdfTemplate);
                    PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFile, FileMode.Create));
                    AcroFields pdfFormFields = pdfStamper.AcroFields;
                    pdfStamper.AcroFields.SetFieldProperty("names", "textsize", 4f, null);

                    pdfFormFields.SetField("BusinessName", objTermination.StoreName);
                    pdfFormFields.SetField("ManagerName", objTermination.StoreManagerName);
                    pdfFormFields.SetField("Date", DateTime.Now.ToString("MM-dd-yyyy"));
                    pdfFormFields.SetField("EmployeeName", objTermination.EmployeeName);
                    pdfFormFields.SetField("Signature", objTermination.EmployeeSignature.Replace("data:image/png;base64,", ""));
                    BaseFont bold = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
                    pdfStamper.FormFlattening = false;
                    pdfStamper.Close();

                    EmployeeTermination objDetail = new EmployeeTermination();
                    objDetail.EmployeeID = Convert.ToInt32(objTermination.EmployeeID);
                    objDetail.EmployeeChildID = Convert.ToInt32(objTermination.EmployeeChildID);
                    objDetail.DocFileName = FileName;
                    objDetail.CreatedBy = Convert.ToInt32(objTermination.CreatedBy);
                    objDetail.StoreId = Convert.ToInt32(objTermination.StoreId);
                    objDetail.LanguageId = Convert.ToInt32(objTermination.LanguageId);
                    new BALHRTermination().SaveEmployeeTermination(objDetail);

                    StreamWriter sw = new StreamWriter(Path.Combine(synthesisRepoPath, "PDF\\Test.txt"), false);
                    sw.WriteLine("BusinessName:" + objTermination.StoreName);
                    sw.WriteLine("EmpID:" + objTermination.EmployeeID);
                    sw.WriteLine("ManagerName:" + objTermination.StoreManagerName);
                    sw.WriteLine("Date:" + DateTime.Now.ToString("MM-dd-yyyy"));
                    sw.WriteLine("EmployeeName:" + objTermination.EmployeeName);
                    sw.WriteLine("Signature:" + objTermination.EmployeeSignature);
                    sw.Close();

                    Response.responseStatus = "200";
                    Response.message = "Employee Termination Sent Successfully!!";
                    logger.Info("HREmployeeTerminationController - SaveEmployeeTermination - " + DateTime.Now.ToString() + " - " + "Employee Termination Sent Successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Someting Went Wrong!";
                    logger.Error("HREmployeeTerminationController - SaveEmployeeTermination - " + DateTime.Now.ToString() + " - " + "Someting Went Wrong!");
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("HREmployeeTerminationController - SaveEmployeeTermination - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }

        }
    }
}