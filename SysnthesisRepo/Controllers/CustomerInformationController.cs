using Aspose.Pdf.Operators;
using EntityModels.Models;
using HtmlAgilityPack;
using NLog;
using Repository;
using Repository.IRepository;
using Syncfusion.EJ2.Base;
using SynthesisCF.Migrations;
using SynthesisViewModal;
using SysnthesisRepo.QBAuth;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Utility;


namespace SysnthesisRepo.Controllers
{
    public class CustomerInformationController : Controller
    {
        private readonly ICustomerInfoRepository _customerInfoRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly ICustomerRecipt _customerRecipt;
        Logger logger = LogManager.GetCurrentClassLogger();
        string SuccessMessage = string.Empty;
        string ErrorMessage = string.Empty;
        protected static int SaveFile = 0;

        public CustomerInformationController()
        {
            this._customerInfoRepository = new CustomerInfoRepository(new DBContext());
            this._commonRepository = new CommonRepository(new DBContext());
            this._customerRecipt = new CustomerReciptRepository(new DBContext());
        }

        // GET: CustomerInformation

        [Authorize(Roles = "Administrator,ViewCustomerModule,CreateCustomersCustomerModule")]
        public ActionResult Index()
        {
            ViewBag.Title = "Customers Information - Synthesis";
            if (Session["StoreId"] != null)
            {
                if (!string.IsNullOrEmpty(Session["StoreId"].ToString()))
                {

                    int store_idval = Convert.ToInt32(Session["StoreId"]);
                    ViewBag.Storeidvalue = store_idval;
                }
            }
            return View();
        }

        public ActionResult UrlDatasource(DataManagerRequest dm)
        {
            if (Session["StoreId"] != null)
            {
                if (!string.IsNullOrEmpty(Session["StoreId"].ToString()))
                {

                    int store_idval = Convert.ToInt32(Session["StoreId"]);
                    ViewBag.Storeidvalue = store_idval;
                }
            }
            List<CustomersReceiveablesManagement> Cum = new List<CustomersReceiveablesManagement>();
            // IEnumerable DataSource = new List<CustomersReceiveablesManagement>();
            int count = 0;
            try
            {
                Cum = _customerInfoRepository.GetInformation(ViewBag.Storeidvalue);
                IEnumerable DataSource = Cum;

                DataOperations operation = new DataOperations();
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
                }
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                }
                count = DataSource.Cast<CustomersReceiveablesManagement>().Count();
                if (dm.Skip != 0)
                {
                    DataSource = operation.PerformSkip(DataSource, dm.Skip);//Paging
                }
                if (dm.Take != 0)
                {
                    DataSource = operation.PerformTake(DataSource, dm.Take);
                }
                return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerInformationController - UrlDatasource - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult CustomerAddPartial()
        {
            CustomersReceiveablesManagement Cim = new CustomersReceiveablesManagement();
            return PartialView("_Customer");
        }

        public ActionResult CustomerEditPartial(CustomersReceiveablesManagement Cm)
        {
            if (Session["StoreId"] != null)
            {
                if (!string.IsNullOrEmpty(Session["StoreId"].ToString()))
                {

                    int store_idval = Convert.ToInt32(Session["StoreId"]);
                    ViewBag.Storeidvalue = store_idval;
                }
            }
            int StoreId = Convert.ToInt32(ViewBag.Storeidvalue);
            CustomersReceiveablesManagement _Cus = _customerInfoRepository.GetInformation(StoreId).Where(x => x.CompanyNameId == Cm.CompanyNameId).FirstOrDefault();
            return PartialView("_CustomerEdit", _Cus);
        }

        public async Task<ActionResult> InsertCustomerInfo(CRUDModel<CustomersReceiveablesManagement> Customerinfo)
        {
            CustomersReceiveablesManagement CM = new CustomersReceiveablesManagement();
            SuccessMessage = null;
            ErrorMessage = null;

            if (ModelState.IsValid)
            {
                try
                {
                    logger.Info("CustomerInformationController - InsertCustomerInformation - " + DateTime.Now);
                    if (Session["StoreId"] != null)
                    {
                        if (!string.IsNullOrEmpty(Session["StoreId"].ToString()))
                        {

                            int store_idval = Convert.ToInt32(Session["StoreId"]);
                            ViewBag.Storeidvalue = store_idval;
                        }
                    }
                    CM = _customerInfoRepository.InsertInformation(Customerinfo.Value, ViewBag.Storeidvalue);
                    if (CM != null)
                    {
                        SuccessMessage = "Customer Information Saved Successfully.";
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message.ToString();
                    logger.Error("CustomerInformationController - InsertCustomerInformation - " + DateTime.Now + " - " + ex.Message.ToString());
                }
            }

            return Json(new { data = CM, success = SuccessMessage, Error = ErrorMessage });
        }

        public async Task<ActionResult> UpdateCustomerInfo(CRUDModel<CustomersReceiveablesManagement> Cm)
        {
            CustomersReceiveablesManagement obj = new CustomersReceiveablesManagement();
            SuccessMessage = null;
            ErrorMessage = null;
            List<CustomersReceiveablesManagement> _Cus = new List<CustomersReceiveablesManagement>();
            try
            {
                logger.Info("CustomerInformationController - UpdateCustomerInformation - " + DateTime.Now);
                if (Session["StoreId"] != null)
                {
                    if (!string.IsNullOrEmpty(Session["StoreId"].ToString()))
                    {

                        int store_idval = Convert.ToInt32(Session["StoreId"]);
                        ViewBag.Storeidvalue = store_idval;
                    }
                }
                int StoreId = Convert.ToInt32(ViewBag.Storeidvalue);
                CustomersReceiveablesManagement objCus = _customerInfoRepository.GetInformation(StoreId).Where(x => x.CompanyNameId == Cm.Value.CompanyNameId).FirstOrDefault();

                if (objCus != null)
                {
                    objCus.CompanyName = Cm.Value.CompanyName;
                    if (!String.IsNullOrEmpty(Cm.Value.PhoneNumber))
                    {
                        objCus.PhoneNumber = Cm.Value.PhoneNumber = Convert.ToInt64(Cm.Value.PhoneNumber).ToString("(###) ###-####");
                    }
                    objCus.ContactPersonName = Cm.Value.ContactPersonName;
                    objCus.Address = Cm.Value.Address;
                    objCus.EmailAddress = Cm.Value.EmailAddress;
                    objCus.StoreId = ViewBag.Storeidvalue;
                    obj = _customerInfoRepository.UpdateInformation(objCus);
                    if (obj != null)
                    {
                        SuccessMessage = "CustomerInformation Updated Successfully.";
                    }                   
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Info("CustomerInformationController - InsertCustomerInformation - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { data = Cm.Value, success = SuccessMessage, Error = ErrorMessage });
        }

        public async Task<ActionResult> DeleteCustomerInfo(CRUDModel<CustomersReceiveablesManagement> Cm)
        {
            CustomersReceiveablesManagement CM = new CustomersReceiveablesManagement();
            SuccessMessage = null;
            ErrorMessage = null;

            try
            {
                logger.Info("CustomerInformationController - RemoveCustomerInformation - " + DateTime.Now);
                CM = _customerInfoRepository.DeleteInformation(Convert.ToInt32(Cm.Key));
                SuccessMessage = "Customer Information Deleted Succeesfully.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Info("CustomerInformationController - RemoveCustomerInformation - " + DateTime.Now + " - " + ex.Message.ToString());

            }
            return Json(new { data = CM, success = SuccessMessage, Error = ErrorMessage });
        }

        [Authorize(Roles = "Administrator,ViewCustomerModule,CreateCustomersCustomerModule")]
        public ActionResult CustomerReciept()
        {
            ViewBag.Title = "Customers Reciept - Synthesis";
            if (Session["StoreId"] != null)
            {
                if (!string.IsNullOrEmpty(Session["StoreId"].ToString()))
                {

                    int store_idval = Convert.ToInt32(Session["StoreId"]);
                    ViewBag.Storeidvalue = store_idval;
                }
            }
            int? StoreId = Convert.ToInt32(ViewBag.Storeidvalue);
            var data = _customerRecipt.GetCustomerRecieptList(StoreId);
            ViewBag.CustomerReciptID = _customerInfoRepository.GetInformation(StoreId).Select(c => new { c.CompanyNameId, c.CompanyName }).ToList();
            return View(data);
        }

        [AcceptVerbs("Post")]
        public void Save()
        {
            SaveFile++;
            AdminSiteConfiguration.WriteErrorLogs("Customer File Upload Count:" + SaveFile);
            try
            {
                int? StoreID = null;
                //string Sacn_Title = "";
                if (Session["StoreId"] != null && !string.IsNullOrEmpty(Session["StoreId"].ToString()))
                {
                    StoreID = Convert.ToInt32(Session["StoreId"]);
                }
                if (System.Web.HttpContext.Current.Request.Files.AllKeys.Length > 0)
                {
                    var httpPostedFile = System.Web.HttpContext.Current.Request.Files["UploadFiles"];
                    //Sacn_Title = AdminSiteConfiguration.GetRandomNo() + Path.GetFileName(httpPostedFile.FileName);
                    //Sacn_Title = AdminSiteConfiguration.RemoveSpecialCharacter(Sacn_Title);
                    if (httpPostedFile != null)
                    {
                        var baseFolderPath = System.Web.HttpContext.Current.Server.MapPath("~/UserFiles/CustomerReciepts");
                        if (!Directory.Exists(baseFolderPath))
                        {
                            Directory.CreateDirectory(baseFolderPath);
                        }
                        var storeFolderPath = Path.Combine(baseFolderPath, StoreID.ToString());
                        if (!Directory.Exists(storeFolderPath))
                        {
                            Directory.CreateDirectory(storeFolderPath);
                        }
                        var userIdFolderPath = Path.Combine(storeFolderPath, UserModule.getUserId().ToString());
                        if (!Directory.Exists(userIdFolderPath))
                        {
                            Directory.CreateDirectory(userIdFolderPath);
                        }

                        var fileName = DateTime.Now.ToString("MMddyyyy") + "_" + DateTime.Now.ToString("HHmmssfff") + "_" + Path.GetFileNameWithoutExtension(httpPostedFile.FileName) + Path.GetExtension(httpPostedFile.FileName);
                        var fileSavePath = Path.Combine(userIdFolderPath, fileName);
                        if (!System.IO.File.Exists(fileSavePath))
                        {
                            httpPostedFile.SaveAs(fileSavePath);
                            var relativePath = fileSavePath.Substring(baseFolderPath.Length).TrimStart('\\', '/');
                            CustomersReceiveablesReceipts _Cm = new CustomersReceiveablesReceipts();
                            _Cm.CompanyNameId = 0;
                            _Cm.FileName = relativePath;
                            _Cm.StoreId = (int)StoreID;
                            CustomersReceiveablesReceipts records = _customerRecipt.InsertRecipt(_Cm);
                            HttpResponse Response = System.Web.HttpContext.Current.Response;
                            Response.Clear();
                            Response.Headers.Add("name", httpPostedFile.FileName);
                            Response.ContentType = "application/json; charset=utf-8";
                            Response.StatusDescription = "File uploaded succesfully";
                            var responseObj = new { name = relativePath };
                            var jsonResponse = new JavaScriptSerializer().Serialize(responseObj);
                            Response.Write(jsonResponse);
                            Response.End();
                            AdminSiteConfiguration.WriteErrorLogs(fileName + " File uploaded succesfully");
                        }
                        else
                        {
                            HttpResponse Response = System.Web.HttpContext.Current.Response;
                            Response.Clear();
                            Response.Status = "204 File already exists";
                            Response.StatusCode = 204;
                            Response.StatusDescription = "File already exists";
                            Response.End();
                            AdminSiteConfiguration.WriteErrorLogs(fileName + " File already exists.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("CustomerInformationController - Save - " + DateTime.Now + " - " + ex.Message.ToString());
                AdminSiteConfiguration.WriteErrorLogs(ex.Message);
                HttpResponse Response = System.Web.HttpContext.Current.Response;
                Response.Clear();
                Response.ContentType = "application/json; charset=utf-8";
                Response.StatusCode = 204;
                Response.Status = "204 No Content";
                Response.StatusDescription = ex.Message;
                Response.End();
            }
        }

        [HttpPost]
        public ActionResult SaveFile1(string FileName, int CompanyNameId)
        {
            SuccessMessage = null;
            ErrorMessage = null;
            try
            {
                logger.Info("CustomerInformationController - SaveFile - " + DateTime.Now);
                if (ModelState.IsValid)
                {
                    CustomersReceiveablesReceipts _Cm = new CustomersReceiveablesReceipts();
                    _Cm.CompanyNameId = CompanyNameId;
                    _Cm.FileName = FileName;

                    CustomersReceiveablesReceipts records = _customerRecipt.InsertRecipt(_Cm);
                    if (records != null)
                    {
                        SuccessMessage = "Success";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Info("CustomerInformationController - SaveFile - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { success = SuccessMessage, Error = ErrorMessage });
        }

        //Himanshu 21-06-2024
        public ActionResult GetCustomerReceivedReceiptData(int CustomersReceiveablesReceiptsId)
        {
            CustomersReceiveablesReceipts customerreceipt = new CustomersReceiveablesReceipts();
            string fileName = "";
            try
            {
                customerreceipt = _customerRecipt.getcustomerreciptdata(CustomersReceiveablesReceiptsId);
                fileName = Path.GetFileName(customerreceipt.FileName);

            }
            catch (Exception ex)
            {
                logger.Info("CustomerInformationController - GetCustomerReceivedReceiptData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json(new { CustomersReceiveablesReceiptsId = customerreceipt.CustomersReceiveablesReceiptsId, FileName = fileName, IsEmailTriggered = customerreceipt.IsEmailTriggered });
        }

        [HttpPost]
        public ActionResult SendReceiptEmailToCustomer(int customerreceiptid)
        {
            string message = "";
            try
            {
                CustomersReceiveablesReceipts data = _customerRecipt.getcustomerreciptdata(customerreceiptid);
                CustomersReceiveablesManagement data1 = _customerRecipt.getcustomerdata(customerreceiptid);
                if (data != null)
                {
                    var Filepath = Request.PhysicalApplicationPath + "UserFiles\\CustomerReciepts" + "\\" + data.FileName;

                    //send email code
                    string FromEmail = ConfigurationManager.AppSettings["FromEmail"].ToString();
                    string Password = ConfigurationManager.AppSettings["EmailPassword"].ToString();
                    string ToEmail = data1.EmailAddress;
                    string Port = ConfigurationManager.AppSettings["EmailPort"].ToString();
                    string Smtp = ConfigurationManager.AppSettings["Smtp"].ToString();

                    // Email body content
                    string body = @"
                        <p style=""margin-bottom:30px;"">Dear Customer,</p>
                        <p style=""margin:10px 0px;"">We hope this message finds you well. Attached to this email, you will find the detailed list of your recently ordered items along with the receipt.</p>
                        <p style=""margin:10px 0px;"">Please review the attached documents and process the payment at your earliest convenience. If you have any questions or require further assistance, please do not hesitate to reach out to us.</p>
                        <p style=""margin:10px 0px;"">Thank you for your prompt attention to this matter. We appreciate your business and look forward to serving you again.</p>
                        <p style=""margin-top:30px;"">Warm regards,<br></p>
                        <p style=""font-weight:bold;"">Westside Market NYC (110th street)</p>";

                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient(Smtp.ToString());
                    mail.Sender = new MailAddress(FromEmail);
                    mail.From = new MailAddress(FromEmail);
                    mail.To.Add(ToEmail);
                    mail.Subject = "Westside Market NYC - Delivery of Your Order and Payment Request";
                    mail.Body = String.Format(body);
                    mail.IsBodyHtml = true;

                    // Attaching the file
                    if (System.IO.File.Exists(Filepath))
                    {
                        Attachment attachment = new Attachment(Filepath);
                        mail.Attachments.Add(attachment);
                    }

                    SmtpServer.UseDefaultCredentials = false;
                    SmtpServer.Port = Convert.ToInt32(Port.ToString());
                    SmtpServer.Credentials = new System.Net.NetworkCredential(FromEmail.ToString(), Password.ToString());
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Send(mail);

                    //update IsEmailTriggered column
                    _customerRecipt.UpdateIsEmailTriggered(customerreceiptid);
                    message = "Success";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message.ToString();
                logger.Info("CustomerInformationController - SendReceiptEmailToCustomer - " + DateTime.Now + " - " + ex.Message.ToString());
                message = "Error";
            }
            return Json(new { success = message });
        }

        public ActionResult DeleteCustomerReceipt(int? id)
        {
            string message = "";
            try
            {
                _customerRecipt.DeleteCustomerReceipt(id);
                message = "Delete";
            }
            catch (Exception ex)
            {
                logger.Error("CustomerInformationController - DeleteCustomerReceipt - " + DateTime.Now + " - " + ex.Message.ToString());
                message = "Error";
            }

            return Json(new { success = message });
        }

        //public ActionResult CustomerReceiptDownload(int id)
        //{
        //    try
        //    {
        //        CustomersReceiveablesReceipts data = _customerRecipt.getcustomerreciptdata(id);
        //        if (data != null)
        //        {
        //            var FileName = System.IO.Path.GetFileName(data.FileName);
        //            var folderPath = Path.Combine(Request.PhysicalApplicationPath, "UserFiles", "CustomerReciepts");
        //            var filePath = Path.Combine(folderPath, data.FileName);

        //            if (Directory.Exists(folderPath))
        //            {
        //                if (System.IO.File.Exists(filePath))
        //                {
        //                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
        //                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, FileName.Trim());
        //                }
        //                else
        //                {
        //                    ViewBag.Message = "File Not Found..";
        //                    return Redirect(Request.Headers["Referer"].ToString());
        //                }
        //            }
        //            else
        //            {
        //                ViewBag.Message = "Directory Not Found..";
        //                return Redirect(Request.Headers["Referer"].ToString());
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("CustomerInformationController - CustomerReceiptDownload - " + DateTime.Now + " - " + ex.Message.ToString());
        //    }
        //    return Json("Success", JsonRequestBehavior.AllowGet);
        //}

        public ActionResult CustomerReceiptDownload(int id)
        {
            try
            {
                CustomersReceiveablesReceipts data = _customerRecipt.getcustomerreciptdata(id);
                if (data != null)
                {
                    var FileName = System.IO.Path.GetFileName(data.FileName);
                    var folderPath = Path.Combine(Request.PhysicalApplicationPath, "UserFiles", "CustomerReciepts");
                    var filePath = Path.Combine(folderPath, data.FileName);

                    if (Directory.Exists(folderPath))
                    {
                        if (System.IO.File.Exists(filePath))
                        {
                            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                            string contentType = GetContentType(filePath);

                            // Set Content-Disposition to inline for files that can be viewed in the browser
                            Response.AppendHeader("Content-Disposition", $"inline; filename=\"{FileName.Trim()}\"");
                            return File(fileBytes, contentType);
                        }
                        else
                        {
                            ViewBag.Message = "File Not Found..";
                            return Redirect(Request.Headers["Referer"].ToString());
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Directory Not Found..";
                        return Redirect(Request.Headers["Referer"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("CustomerInformationController - CustomerReceiptDownload - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        private string GetContentType(string filePath)
        {
            var contentType = "application/octet-stream";
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            switch (extension)
            {
                case ".pdf":
                    contentType = "application/pdf";
                    break;
                case ".jpg":
                case ".jpeg":
                    contentType = "image/jpeg";
                    break;
                case ".png":
                    contentType = "image/png";
                    break;
                case ".gif":
                    contentType = "image/gif";
                    break;
                case ".html":
                    contentType = "text/html";
                    break;
                case ".txt":
                    contentType = "text/plain";
                    break;
                    // Add more cases as needed for other file types
            }
            return contentType;
        }

        public ActionResult UpdateCompanyName(int CustomersReceiveablesReceiptsId,int CompanyNameId)
        {
            string message = "";
            try
            {
                _customerRecipt.UpdateCompanyNameCustomerReceipt(CustomersReceiveablesReceiptsId, CompanyNameId);
                message = "Success";
            }
            catch (Exception ex)
            {
                logger.Error("CustomerInformationController - UpdateCompanyName - " + DateTime.Now + " - " + ex.Message.ToString());
                message = "Error";
            }

            return Json(new { success = message });
        }

        public ActionResult GetCompanyName(int CustomersReceiveablesReceiptsId)
        {
            CustomersReceiveablesReceipts customerreceipt = new CustomersReceiveablesReceipts();
            string message = "";
            try
            {

                customerreceipt = _customerRecipt.getcustomerreciptdata(CustomersReceiveablesReceiptsId);
            }
            catch (Exception ex)
            {
                logger.Error("CustomerInformationController - UpdateCompanyName - " + DateTime.Now + " - " + ex.Message.ToString());
                message = "Error";
            }

            return Json(new { CompanyNameId = customerreceipt.CompanyNameId });
        }

    }
}