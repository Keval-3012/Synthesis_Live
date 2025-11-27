using iTextSharp.text.pdf;
using NLog;
using SyntesisApi.BAL;
using SyntesisApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
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
    public class EmployeeHealthBenefitsController : ApiController
    {
        ResponseModel Response = new ResponseModel();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #region Start Harsh  11/03/2024
        [Authorize]
        [HttpPost]
        [Route("api/EmployeeHealthBenefits/EmployeeHealthBenefit")]
        public async Task<IHttpActionResult> EmployeeHealthBenefit([FromBody] EmployeeHealthBenefit objEmp)
        {
            try
            {
                logger.Info("EmployeeHealthBenefitsController - EmployeeHealthBenefit - " + DateTime.Now.ToString());
                if (objEmp != null)
                {
                    string number = String.Format("{0:d9}", (DateTime.Now.Ticks / 10) % 1000000000);

                    string synthesisRepoPath = ConfigurationManager.AppSettings["SynthesisRepoPath"];
                    string employeeDocumentPath = Path.Combine(synthesisRepoPath, "EmployeeHealthBenefitDocument");
                    string empFolderPath = Path.Combine(employeeDocumentPath, objEmp.EmployeeID.ToString());

                    if (!Directory.Exists(empFolderPath))
                    {
                        Directory.CreateDirectory(empFolderPath);
                    }
                    string pdfTemplate = "";
                    if (objEmp.languageId == "2")
                    {
                        pdfTemplate = Path.Combine(synthesisRepoPath, "PDF\\Concert\\waiverFillable_S.pdf");
                    }
                    else
                    {
                        pdfTemplate = Path.Combine(synthesisRepoPath, "PDF\\Concert\\waiverFillable.pdf");
                    }
                    string FileName = "EmployeeHealth_" + number + ".pdf";
                    var newFile = Path.Combine(synthesisRepoPath, empFolderPath + "\\" + FileName);
                    objEmp.DocFileName = FileName;
                    PdfReader pdfReader = new PdfReader(pdfTemplate);
                    PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(newFile, FileMode.Create));
                    AcroFields pdfFormFields = pdfStamper.AcroFields;
                    pdfFormFields.SetField("CreatedDate", DateTime.Now.ToString("MM-dd-yyyy"));
                    pdfFormFields.SetField("EmployeeName", objEmp.EmployeeName);
                    if (objEmp.MaritalStatus == "Single")
                    {
                        pdfFormFields.SetField("MaritalStatus", "Single");
                    }
                    else if (objEmp.MaritalStatus == "Married")
                    {
                        pdfFormFields.SetField("MaritalStatus", "Married");
                    }
                    else if (objEmp.MaritalStatus == "Divorced")
                    {
                        pdfFormFields.SetField("MaritalStatus", "Divorced");
                    }
                    else if (objEmp.MaritalStatus == "Widowed")
                    {
                        pdfFormFields.SetField("MaritalStatus", "Widowed");
                    }
                    pdfFormFields.SetField("EmployementDate", objEmp.EmployementDate);
                    pdfFormFields.SetField("DOB", objEmp.DOB);

                    if (objEmp.OtherCoverage == true)
                    {
                        pdfFormFields.SetField("OtherCoverage", "Yes");
                        string[] Details = objEmp.OtherCoverageDetail.ToString().Split(',');
                        if (Details.Contains("1"))
                        {
                            pdfFormFields.SetField("OtherCoverage_Spouse", "On");
                        }
                        if (Details.Contains("2"))
                        {
                            pdfFormFields.SetField("OtherCoverage_Medicare", "Yes");
                        }
                        if (Details.Contains("3"))
                        {
                            pdfFormFields.SetField("OtherCoverage_Medicaid", "Yes");
                        }
                        if (Details.Contains("4"))
                        {
                            pdfFormFields.SetField("OtherCoverage_Veteran", "Yes");
                        }
                        if (Details.Contains("5"))
                        {
                            pdfFormFields.SetField("OtherCoverage_Other", "Yes");
                        }
                    }
                    if (objEmp.RefusedCoverage == true)
                    {
                        pdfFormFields.SetField("RefusedCoverage", "Yes");
                    }
                    pdfFormFields.SetField("EmpSignature", objEmp.EmployeeSignature.Replace("data:image/png;base64,", ""));
                    pdfStamper.FormFlattening = false;
                    pdfStamper.Close();
                    pdfReader.Close();
                    pdfReader.Dispose();
                    new BALHRHealthBenefits().saveHealthBenefits(objEmp);

                    Response.responseStatus = "200";
                    Response.message = "Medical Insurance Submitted Successfully!!";
                    logger.Info("EmployeeHealthBenefitsController - EmployeeHealthBenefit - " + DateTime.Now.ToString() + " - " + "Medical Insurance Submitted Successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Someting Went Wrong!";
                    logger.Error("EmployeeHealthBenefitsController - EmployeeHealthBenefit - " + DateTime.Now.ToString() + " - " + "Someting Went Wrong!");
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("EmployeeHealthBenefitsController - EmployeeHealthBenefit - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }

        }
        #endregion
    }
}
