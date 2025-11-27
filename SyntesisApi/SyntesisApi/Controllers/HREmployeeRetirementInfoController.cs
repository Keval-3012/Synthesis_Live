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
using System.Web;
using System.Web.Http;
using static SyntesisApi.Models.ApiModel;

namespace SyntesisApi.Controllers
{
    public class HREmployeeRetirementInfoController : ApiController
    {
        ResponseModel Response = new ResponseModel();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #region Start Harsh 11-03-2024
        [Authorize]
        [HttpPost]
        [Route("api/HREmployeeRetirementInfo/EmployeeRetirementInfo")]
        public async Task<IHttpActionResult> EmployeeRetirement([FromBody] EmpRetirementInfo objEmp)
        {
            try
            {
                logger.Info("HREmployeeRetirementInfoController - EmployeeRetirement - " + DateTime.Now.ToString());

                if (objEmp != null)
                {
                    string number = String.Format("{0:d9}", (DateTime.Now.Ticks / 10) % 1000000000);
                    string synthesisRepoPath = ConfigurationManager.AppSettings["SynthesisRepoPath"];
                    string employeeDocumentPath = Path.Combine(synthesisRepoPath, "EmployeeDocument_401K");
                    string empFolderPath = Path.Combine(employeeDocumentPath, objEmp.EmployeeID.ToString());

                    if (!Directory.Exists(empFolderPath))
                    {
                        Directory.CreateDirectory(empFolderPath);
                    }

                    string pdfTemplate = "";
                    if (objEmp.languageId == "2")
                    {
                        pdfTemplate = Path.Combine(synthesisRepoPath, "PDF\\Concert\\401K_Spenish.pdf");
                    }
                    else
                    {
                        pdfTemplate = Path.Combine(synthesisRepoPath, "PDF\\Concert\\401K_English.pdf");
                    }


                    string FileName = "EmployeeDocument_401K_" + number + ".pdf";
                    var newFile = Path.Combine(synthesisRepoPath, empFolderPath + "\\" + FileName);

                    PdfReader pdfReader = new PdfReader(pdfTemplate);
                    PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFile, FileMode.Create));
                    AcroFields pdfFormFields = pdfStamper.AcroFields;
                    pdfFormFields.SetField("Date", DateTime.Now.Date.ToString("MM/dd/yyyy"));
                    pdfFormFields.SetField("EmployeeName", objEmp.EmployeeName);
                    if (objEmp.OptStatus == 1)
                    {
                        pdfFormFields.SetField("OptIn", "OptIn");
                    }
                    else
                    {
                        pdfFormFields.SetField("OptIn", "OptOut");
                    }
                    pdfFormFields.SetField("Emp_Signature", objEmp.EmployeeSignature.Replace("data:image/png;base64,", ""));
                    pdfStamper.FormFlattening = false;
                    pdfStamper.Close();
                    pdfReader.Close();
                    pdfReader.Dispose();
                    objEmp.FileName = FileName;
                    new BALHRRetirementInfo().saveRetirementInfo(objEmp);

                    Response.responseStatus = "200";
                    Response.message = "Retirement Plan 401(K) Submitted Successfully!!";
                    logger.Info("HREmployeeRetirementInfoController - EmployeeRetirement - " + DateTime.Now.ToString() + " - " + "Retirement Plan 401(K) Submitted Successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Someting Went Wrong!";
                    logger.Error("HREmployeeRetirementInfoController - EmployeeRetirement - " + DateTime.Now.ToString() + " - " + "Someting Went Wrong!");
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("HREmployeeRetirementInfoController - EmployeeRetirement - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }

        }
        #endregion
    }
}
