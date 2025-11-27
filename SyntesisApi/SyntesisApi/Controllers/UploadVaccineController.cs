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
    public class UploadVaccineController : ApiController
    {
        ResponseModel Response = new ResponseModel();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #region Start Harsh 11-03-2024
        [Authorize]
        [HttpPost]
        [Route("api/UploadVaccine/UploadVaccine")]
        public async Task<IHttpActionResult> UploadVaccine([FromBody] UploadFiles objFile)
        {
            try
            {
                logger.Info("UploadVaccineController - UploadVaccine - " + DateTime.Now.ToString());

                if (objFile != null)
                {
                    var NewFileName = "";

                    string synthesisRepoPath = ConfigurationManager.AppSettings["SynthesisRepoPath"];
                    string employeeDocumentPath = Path.Combine(synthesisRepoPath, "VaccineCertificate");
                    

                    if (!Directory.Exists(employeeDocumentPath))
                    {
                        Directory.CreateDirectory(employeeDocumentPath);
                    }

                    NewFileName = objFile.EmployeeId.ToString() + '-' + Guid.NewGuid().ToString() + ".jpg";
                    var filePath = Path.Combine(synthesisRepoPath, employeeDocumentPath + "\\" + NewFileName);
                    try
                    {
                        byte[] bytes = Convert.FromBase64String(objFile.Image.Replace("data:image/png;base64,", ""));
                        System.IO.FileStream stream = new FileStream(filePath, FileMode.CreateNew);
                        System.IO.BinaryWriter writer = new BinaryWriter(stream);
                        writer.Write(bytes, 0, bytes.Length);
                        writer.Close();
                    }
                    catch (Exception ex) {
                        logger.Error("UploadVaccineController - UploadVaccine - " + DateTime.Now.ToString() + " - " + ex.Message);
                    }

                    VaccineCertificateInfo objDoc = new VaccineCertificateInfo();
                    objDoc.EmployeeID = Convert.ToInt32(objFile.EmployeeId);
                    objDoc.FileName = NewFileName;
                    objDoc.IsVaccine = objFile.IsVaccine;
                    objDoc.IsExemption = objFile.IsExemption;
                    objDoc.CreatedID = objFile.CreatedID;
                    objDoc.EmployeeChildID = objFile.EmployeeChildID;
                    new BALHRVaccine().saveVaccineDetails(objDoc);
                    objDoc = null;

                    Response.responseStatus = "200";
                    Response.message = "Vaccine Status Submitted Successfully!!";
                    logger.Info("UploadVaccineController - UploadVaccine - " + DateTime.Now.ToString() + " - " + "Vaccine Status Submitted Successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Someting Went Wrong!";
                    logger.Error("UploadVaccineController - UploadVaccine - " + DateTime.Now.ToString() + " - " + "Someting Went Wrong!");
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("UploadVaccineController - UploadVaccine - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
            
        }
        #endregion
    }
}
