using iTextSharp.text;
using NLog;
using SyntesisApi.BAL;
using SyntesisApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Services;
using static SyntesisApi.Models.ApiModel;




namespace SyntesisApi.Controllers
{
    public class EmployeeTrainingController : ApiController
    {
        DataTable Dt1 = new DataTable();
        ResponseModel Response = new ResponseModel();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [Authorize]
        [HttpPost]
        [Route("api/EmployeeTraining/UpdateLastSlide")]
        public async Task<IHttpActionResult> UpdateLastSlide(LastSlide lastSlide)
        {
            try
            {
                logger.Info("EmployeeTrainingController - UpdateLastSlide - " + DateTime.Now.ToString());
                int id= new BALHRTraining().UpdateLastSlide(lastSlide);
                if (id == 1)
                {
                    Response.responseStatus = "200";
                    Response.responseData = null;
                    Response.message = "Slide Updated Successfully!!";
                    logger.Info("EmployeeTrainingController - UpdateLastSlide - " + DateTime.Now.ToString() + " - " + "Slide Updated Successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Employee does Not Exist!!";
                    logger.Info("EmployeeTrainingController - UpdateLastSlide - " + DateTime.Now.ToString() + " - " + "Employee Does Not Exist!!");
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("EmployeeTrainingController - UpdateLastSlide - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/EmployeeTraining/ResetTraining")]
        public async Task<IHttpActionResult> ResetTraining(EmployeeList employee)
        {
            try
            {
                logger.Info("EmployeeTrainingController - ResetTraining - " + DateTime.Now.ToString());
                int id = new BALHRTraining().ResetTraining(employee);
                if (id == 1)
                {
                    Response.responseStatus = "200";
                    Response.responseData = null;
                    Response.message = "Employee Reset Training Successfully!!";
                    logger.Info("EmployeeTrainingController - ResetTraining - " + DateTime.Now.ToString() + " - " + "Employee Reset Training Successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Employee Does Not Exist!!";
                    logger.Info("EmployeeTrainingController - ResetTraining - " + DateTime.Now.ToString() + " - " + "Employee Does Not Exist!!");
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("EmployeeTrainingController - ResetTraining - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }


        [Authorize]
        [HttpPost]
        [Route("api/EmployeeTraining/CompleteTraining")]
        public async Task<IHttpActionResult> CompleteTraining(CompleteyTraining data)
        {
            try
            {
                Dt1 = new DataTable();
                string htmlString = data.Data;
                string baseUrl = string.Empty;
                int pFrom = htmlString.IndexOf("curentDate") + "curentDate".Length + 2;
                Dt1 = new BALHREmployee().GetEmployeeById(data.EmployeeId);
                if (Dt1.Rows.Count > 0)
                {
                    if (Convert.ToBoolean(Dt1.Rows[0]["IsTraningCompleted"]) == true)
                    {
                        if (!String.IsNullOrEmpty(Dt1.Rows[0]["Date"].ToString()))
                        {
                            string aa = htmlString.Substring(htmlString.IndexOf("curentDate\">") + "curentDate\">".Length);
                            int pStart = aa.IndexOf("</span>");
                            string bb = aa.Substring(0, pStart);
                            htmlString = htmlString.Replace(bb, Dt1.Rows[0]["Date"].ToString());
                        }

                        if (!String.IsNullOrEmpty(Dt1.Rows[0]["Time"].ToString()))
                        {
                            string aa1 = htmlString.Substring(htmlString.IndexOf("curentTime\">") + "curentTime\">".Length);
                            int pStart1 = aa1.IndexOf("</span>");
                            string bb1 = aa1.Substring(0, pStart1);
                            htmlString = htmlString.Replace(bb1, Dt1.Rows[0]["Time"].ToString());
                        }
                    }

                }
                string pdf_page_size = SelectPdf.PdfPageSize.A4.ToString();
                SelectPdf.PdfPageSize pageSize = (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize),
                    pdf_page_size, true);

                string pdf_orientation = SelectPdf.PdfPageOrientation.Landscape.ToString();
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);

                int webPageWidth = 1024;
                try
                {
                    webPageWidth = Convert.ToInt32("1024");
                }
                catch { }

                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();

                // set converter options
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = 0;
                converter.Options.MarginLeft = 0;
                converter.Options.MarginRight = 7;
                converter.Options.MarginTop = 0;
                converter.Options.MarginBottom = 0;

                // create a new pdf document converting an url
                SelectPdf.PdfDocument doc = converter.ConvertHtmlString(htmlString, baseUrl);

                string Name = "-" + data.EmployeeId.ToString() + '-' + Guid.NewGuid().ToString();
                string synthesisRepoPath = ConfigurationManager.AppSettings["SynthesisRepoPath"];
                string CertificatesPath = Path.Combine(synthesisRepoPath, "Certificates");
                if (!Directory.Exists(CertificatesPath))
                {
                    Directory.CreateDirectory(CertificatesPath);
                }
                string FileName = "Traning" + Name + ".pdf";
                var newFile = Path.Combine(CertificatesPath + "\\" + FileName);
                doc.Save(newFile);
                doc.Close();
                string[] formats = { "MM/dd/yyyy" };
                int id = 0;
                if (Dt1.Rows.Count > 0)
                {
                    if (Convert.ToBoolean(Dt1.Rows[0]["IsTraningCompleted"]) == false)
                    {
                        CompleteyTrainingPost post = new CompleteyTrainingPost();
                        post.TraningFilePath = newFile;
                        post.TraningContent = FileName;
                        post.TrainingCompletedDateTime = DateTime.ParseExact(data.TrainingDate, formats, new CultureInfo("en-US"), DateTimeStyles.None);
                        var time = TimeSpan.Parse(data.TrainingTime);
                        post.TrainingCompletedTime = Convert.ToDateTime("1900-01-01").Add(time);
                        post.EmployeeId = data.EmployeeId;
                        id = new BALHRTraining().UpdateEmployeeTraining(post);
                        if (id == 1)
                        {
                            DataTable dt = new DataTable();
                            dt.Columns.Add(new DataColumn("URL"));
                            var SentURL = ConfigurationManager.AppSettings["SentURL"] + "UserFiles/HR_File/Certificates/" + FileName;
                            dt.Rows.Add(SentURL);
                            Response.responseStatus = "200";
                            Response.responseData = dt;
                            Response.message = "Employee Training Completed Successfully!!";
                            logger.Info("EmployeeTrainingController - CompleteTraining - " + DateTime.Now.ToString() + " - " + "Employee Training Completed Successfully!!");
                        }
                        else
                        {
                            Response.responseStatus = "400";
                            Response.responseData = null;
                            Response.message = "Employee Not Exist!!";
                            logger.Info("EmployeeTrainingController - CompleteTraining - " + DateTime.Now.ToString() + " - " + "Employee Does Not Exist!!");
                        }

                    }
                    else
                    {
                        CompleteyTrainingPost post = new CompleteyTrainingPost();
                        post.TraningFilePath = newFile;
                        post.TraningContent = FileName;
                        post.EmployeeId = data.EmployeeId;
                        id = new BALHRTraining().EmployeeTrainingDownload(post);
                        if (id == 1)
                        {
                            DataTable dt = new DataTable();
                            dt.Columns.Add(new DataColumn("URL"));
                            var SentURL = ConfigurationManager.AppSettings["SentURL"] + "Certificates/" + FileName;
                            dt.Rows.Add(SentURL);
                            Response.responseStatus = "200";
                            Response.responseData = dt;
                            Response.message = "Employee Training Completed Successfully!!";
                            logger.Info("EmployeeTrainingController - CompleteTraining - " + DateTime.Now.ToString() + " - " + "Employee Training Completed Successfully!!");
                        }
                        else
                        {
                            Response.responseStatus = "400";
                            Response.responseData = null;
                            Response.message = "Employee Not Exist!!";
                            logger.Info("EmployeeTrainingController - CompleteTraining - " + DateTime.Now.ToString() + " - " + "Employee Does Not Exist!!");
                        }
                    }
                }
                else
                {
                    try
                    {
                        if (File.Exists(newFile))
                        {
                            File.Delete(newFile);
                        }
                        Response.responseStatus = "400";
                        Response.responseData = null;
                        Response.message = "Employee Not Exist!!";
                        logger.Info("EmployeeTrainingController - CompleteTraining - " + DateTime.Now.ToString() + " - " + "Employee Does Not Exist!!");
                    }
                    catch (Exception ex)
                    {
                        Response.responseStatus = "400";
                        Response.responseData = null;
                        Response.message = ex.Message;
                        logger.Error("EmployeeTrainingController - CompleteTraining - " + DateTime.Now.ToString() + " - " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("EmployeeTrainingController - CompleteTraining - " + DateTime.Now.ToString() + " - " + ex.Message);
            }

            return Ok(Response);
        }

        [Authorize]
        [HttpGet]
        [Route("api/EmployeeTraining/TrainingSlides")]
        public async Task<IHttpActionResult> TrainingSlides(Language language)
        {
            try
            {
                logger.Info("EmployeeTrainingController - TrainingSlides - " + DateTime.Now.ToString());
                List<trainingSlides> list = new List<trainingSlides>();
                DataTable dt = new DataTable();
                string URL = ConfigurationManager.AppSettings["VideoURL"].ToString();
                if (language.LanguageId == 1)
                {
                    list.Add(new trainingSlides(1,false,null, "Important Notice", ".video-div-1"));
                    list.Add(new trainingSlides(2, false, URL + "Segment1.mp4", ".video-div-1", ".video-div-1-1"));
                    list.Add(new trainingSlides(3, false, URL + "Segment111.mp4", ".video-div-1-1", ".video-div-1-2"));
                    list.Add(new trainingSlides(4, true, URL + "Segment112.mp4", ".video-div-1-2", ".q1Data"));
                    list.Add(new trainingSlides(5, false, null, ".q1Data", ".video-div-2"));
                    list.Add(new trainingSlides(6, true, URL + "Segment2.mp4", ".video-div-2", ".q2Data"));
                    list.Add(new trainingSlides(7, false, null, ".q2Data", ".video-div-3"));
                    list.Add(new trainingSlides(8, true, URL + "Segment3.mp4", ".video-div-3", ".q3Data"));
                    list.Add(new trainingSlides(9, false, null, ".q3Data", ".video-div-4"));
                    list.Add(new trainingSlides(10, true, URL + "Segment4.mp4", ".video-div-4", ".q4Data"));
                    list.Add(new trainingSlides(11, false, null, ".q4Data", ".video-div-5"));
                    list.Add(new trainingSlides(12, true, URL + "Segment5.mp4", ".video-div-5", ".q5Data"));
                    list.Add(new trainingSlides(13, false, null, ".q5Data", ".video-div-6"));
                    list.Add(new trainingSlides(14, true, URL + "Segment6.mp4", ".video-div-6", ".q6Data"));
                    list.Add(new trainingSlides(15, false, null, ".q6Data", ".video-div-7"));
                    list.Add(new trainingSlides(16, true, URL + "Segment7.mp4", ".video-div-7", ".q7Data"));
                    list.Add(new trainingSlides(17, false, null, ".q7Data", ".video-div-8"));
                    list.Add(new trainingSlides(18, false, URL + "Segment8.mp4", ".video-div-8", ".video-div-8-1"));
                    list.Add(new trainingSlides(19, true, URL + "Segment81.mp4", ".video-div-8-1", ".q8Data"));
                    list.Add(new trainingSlides(20, false, null, ".q8Data", ".video-div-9"));
                    list.Add(new trainingSlides(21, false, URL + "Segment9.mp4", ".video-div-9", ".video-div-9-1"));
                    list.Add(new trainingSlides(22, true, URL + "Segment91.mp4", ".video-div-9-1", ".q9Data"));
                    list.Add(new trainingSlides(23, false, null, ".q9Data", ".video-div-10"));
                    list.Add(new trainingSlides(24, true, URL + "Segment10.mp4", ".video-div-10", ".q10Data"));
                    list.Add(new trainingSlides(25, false, null, ".q10Data", ".video-div-11"));
                    list.Add(new trainingSlides(26, true, URL + "Segment11.mp4", ".video-div-11", ".q11Data"));
                    list.Add(new trainingSlides(27, false, null, ".q11Data", ".video-div-12"));
                    list.Add(new trainingSlides(28, true, URL + "Segment12.mp4", ".video-div-12", ".q12Data"));
                    list.Add(new trainingSlides(29, false, null, ".q12Data", ".video-div-13"));
                    list.Add(new trainingSlides(30, true, URL + "Segment13.mp4", ".video-div-13", ".q13Data"));
                    list.Add(new trainingSlides(31, false, null, ".q13Data", ".video-div-14"));
                    list.Add(new trainingSlides(32, true, URL + "Segment14.mp4", ".video-div-14", ".q14Data"));
                    list.Add(new trainingSlides(33, false, null, ".q14Data", null));

                    dt = Utility.Common.LINQToDataTable(list);  

                    Response.responseStatus = "200";
                    Response.responseData = dt;
                    Response.message = "Successfully!!";
                    logger.Info("EmployeeTrainingController - TrainingSlides - " + DateTime.Now.ToString() + " - " + "Successfully!!");
                }
                else if(language.LanguageId == 2)
                {

                    list.Add(new trainingSlides(1, false, null, "Important Notice", ".video-div-1"));
                    list.Add(new trainingSlides(2, false, URL + "Spanish/Segment1.mp4", ".video-div-1", ".video-div-1-1"));
                    list.Add(new trainingSlides(3, false, URL + "Spanish/Segment111.mp4", ".video-div-1-1", ".video-div-1-2"));
                    list.Add(new trainingSlides(4, true, URL + "Spanish/Segment112.mp4", ".video-div-1-2", ".q1DataSp"));
                    list.Add(new trainingSlides(5, false, null, ".q1DataSp", ".video-div-2"));
                    list.Add(new trainingSlides(6, true, URL + "Spanish/Segment2.mp4", ".video-div-2", ".q2DataSp"));
                    list.Add(new trainingSlides(7, false, null, ".q2DataSp", ".video-div-3"));
                    list.Add(new trainingSlides(8, true, URL + "Spanish/Segment3.mp4", ".video-div-3", ".q3DataSp"));
                    list.Add(new trainingSlides(9, false, null, ".q3DataSp", ".video-div-4"));
                    list.Add(new trainingSlides(10, true, URL + "Spanish/Segment4.mp4", ".video-div-4", ".q4DataSp"));
                    list.Add(new trainingSlides(11, false, null, ".q4DataSp", ".video-div-5"));
                    list.Add(new trainingSlides(12, true, URL + "Spanish/Segment5.mp4", ".video-div-5", ".q5DataSp"));
                    list.Add(new trainingSlides(13, false, null, ".q5DataSp", ".video-div-6"));
                    list.Add(new trainingSlides(14, true, URL + "Spanish/Segment6.mp4", ".video-div-6", ".q6DataSp"));
                    list.Add(new trainingSlides(15, false, null, ".q6DataSp", ".video-div-7"));
                    list.Add(new trainingSlides(16, true, URL + "Spanish/Segment7.mp4", ".video-div-7", ".q7DataSp"));
                    list.Add(new trainingSlides(17, false, null, ".q7DataSp", ".video-div-8"));
                    list.Add(new trainingSlides(18, false, URL + "Spanish/Segment8.mp4", ".video-div-8", ".video-div-8-1"));
                    list.Add(new trainingSlides(19, true, URL + "Spanish/Segment81.mp4", ".video-div-8-1", ".q8DataSp"));
                    list.Add(new trainingSlides(20, false, null, ".q8DataSp", ".video-div-9"));
                    list.Add(new trainingSlides(21, false, URL + "Spanish/Segment9.mp4", ".video-div-9", ".video-div-9-1"));
                    list.Add(new trainingSlides(22, true, URL + "Spanish/Segment91.mp4", ".video-div-9-1", ".q9DataSp"));
                    list.Add(new trainingSlides(23, false, null, ".q9DataSp", ".video-div-10"));
                    list.Add(new trainingSlides(24, true, URL + "Spanish/Segment10.mp4", ".video-div-10", ".q10DataSp"));
                    list.Add(new trainingSlides(25, false, null, ".q10DataSp", ".video-div-11"));
                    list.Add(new trainingSlides(26, true, URL + "Spanish/Segment11.mp4", ".video-div-11", ".q11DataSp"));
                    list.Add(new trainingSlides(27, false, null, ".q11DataSp", ".video-div-12"));
                    list.Add(new trainingSlides(28, true, URL + "Spanish/Segment12.mp4", ".video-div-12", ".q12DataSp"));
                    list.Add(new trainingSlides(29, false, null, ".q12DataSp", ".video-div-13"));
                    list.Add(new trainingSlides(30, true, URL + "Spanish/Segment13.mp4", ".video-div-13", ".q13DataSp"));
                    list.Add(new trainingSlides(31, false, null, ".q13DataSp", ".video-div-14"));
                    list.Add(new trainingSlides(32, true, URL + "Spanish/Segment14.mp4", ".video-div-14", ".q14DataSp"));
                    list.Add(new trainingSlides(33, false, null, ".q14DataSp", null));

                    dt = Utility.Common.LINQToDataTable(list);

                    Response.responseStatus = "200";
                    Response.responseData = dt;
                    Response.message = "Successfully!!";
                    logger.Info("EmployeeTrainingController - TrainingSlides - " + DateTime.Now.ToString() + " - " + "Successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Record Does Not Exist!!";
                    logger.Info("EmployeeTrainingController - TrainingSlides - " + DateTime.Now.ToString() + " - " + "Record Does Not Exist!!");
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("EmployeeTrainingController - TrainingSlides - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/EmployeeTraining/UpdateLanguage")]
        public async Task<IHttpActionResult> UpdateLanguage(LanguageUpdate employee)
        {
            try
            {
                logger.Info("EmployeeTrainingController - UpdateLanguage - " + DateTime.Now.ToString());
                int id = new BALHRTraining().UpdateLanguage(employee);
                if (id == 1)
                {
                    Response.responseStatus = "200";
                    Response.responseData = null;
                    Response.message = "Language Updated Successfully!!";
                    logger.Info("EmployeeTrainingController - UpdateLanguage - " + DateTime.Now.ToString() + " - " + "Language Updated Successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Employee Does Not Exist!!";
                    logger.Info("EmployeeTrainingController - UpdateLanguage - " + DateTime.Now.ToString() + " - " + "Somwthing went wrong!!");
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("EmployeeTrainingController - ResetTraining - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }
    }
}
