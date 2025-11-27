using EntityModels.Models;
using Repository;
using Repository.IRepository;
using SynthesisViewModal;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Xml;
using SysnthesisRepo.QBAuth;
using NLog;
using System.Web;

namespace SysnthesisRepo.Controllers
{
    public class QBConfigurationController : Controller
    {
        private readonly ICommonRepository _commonRepository;
        private readonly IUserActivityLogRepository _activityLogRepository;
        private readonly ISynthesisApiRepository _synthesisapirepository;
        private readonly IQBRepository _qBRepository;
        private readonly ICompaniesRepository _companiesRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        private string logPath = "~/Log/";
        public string Message { get; set; }
        SynthesisQBOnline.BAL.QBResponse objResponse = new SynthesisQBOnline.BAL.QBResponse();
        public QBConfigurationController()
        {
            this._commonRepository = new CommonRepository(new DBContext());
            this._activityLogRepository = new UserActivityLogRepository(new DBContext());
            this._synthesisapirepository = new SynthesisApiRepository();
            this._commonRepository = new CommonRepository(new DBContext());
            this._activityLogRepository = new UserActivityLogRepository(new DBContext());
            this._qBRepository = new QBRepository(new DBContext());
            this._companiesRepository = new CompaniesRepository(new DBContext());
        }
        // GET: QBConfiguration
        /// <summary>
        /// This method is get Quick books Online configuration
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            ViewBag.Title = "Quickbooks Configuration - Synthesis";
            _commonRepository.LogEntries();     //Harsh's code
            //This class is Get QB Online Configuration
            var lstdata = _qBRepository.GetQBOnlineConfiguration1();
            try
            {
                if (!System.IO.Directory.Exists(Server.MapPath(logPath)))
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(logPath));
                }
            }
            catch (Exception ex)
            {
                logger.Error("QBConfigurationController - Index - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            
            return View(lstdata);
        }
        /// <summary>
        /// This method is go to Auth Grant
        /// </summary>
        /// <param name="StoreID"></param>
        /// <returns></returns>
        public ActionResult GotoAuthGrant(int StoreID)
        {
            TempData["FromQBConfig"] = "WebQBConfig";
            TempData["StoreID"] = StoreID;

            // Truncate QuickBooksStorewiseToken table
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_QBOnlineConfiguration", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Mode", "TruncateQBTokenStorewise");
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            var userName = System.Web.HttpContext.Current.User.Identity.Name;

            try
            {
                // Get UserID by Username
                if (_commonRepository.getUserId(userName) != 0)
                {
                    // Fetch clientId and clientSecret from StoreMaster using StoreID
                    var storeDetails = _companiesRepository.GetStoreMastersbyID(StoreID); // <-- You’ll define this
                    if (storeDetails != null)
                    {
                        string clientId = storeDetails.ClientID;
                        string clientSecret = storeDetails.ClientSecret;

                        // Insert into QuickBookStorewiseToken
                        bool isInserted = _companiesRepository.InsertQuickBookStorewiseToken(StoreID, clientId, clientSecret);

                        if (!isInserted)
                        {
                            logger.Warn($"Failed to insert QuickBookStorewiseToken for StoreID: {StoreID}");
                            return RedirectToAction("Index", "Login");
                        }

                        // Safely encode values for URL
                        string redirectUrl = $"~/OauthGrant.aspx";
                        return Redirect(redirectUrl);
                    }
                    else
                    {
                        // Handle missing store details
                        logger.Warn($"Store details not found for StoreID: {StoreID}");
                        return RedirectToAction("Index", "Login");
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception ex)
            {
                logger.Error("QBConfigurationController - GotoAuthGrant - " + DateTime.Now + " - " + ex.Message.ToString());
                return RedirectToAction("Index", "Login");
            }

        }
        /// <summary>
        /// This method is eeturn view of QB Desktop Configurayion
        /// </summary>
        /// <param name="qBConfigurationViewModal"></param>
        /// <returns></returns>
        public ActionResult QBDesktopConfiguration(QBConfigurationViewModal qBConfigurationViewModal)
        {
            QBDesktopConfiguration obj = new QBDesktopConfiguration();
            try
            {
                obj.StoreId = qBConfigurationViewModal.StoreID;
                //This class is Get QB Online Configuration
                ViewData["ManagDepartment"] = _qBRepository.GetQBDesktopConfiguration(qBConfigurationViewModal);
                TempData["StoreID"] = qBConfigurationViewModal.StoreID;

                //This class is Get QB Online Configuration
                if (_qBRepository.GetQBDesktopConfiguration(qBConfigurationViewModal) != null)
                {
                    //This class is Get QB Online Configuration
                    string Admassword = _qBRepository.GetQBDesktopConfiguration(qBConfigurationViewModal).Select(a => a.Password).ToString();
                    ViewBag.Passwords = Admassword;
                }
            }
            catch (Exception ex)
            {
                logger.Error("QBConfigurationController - QBDesktopConfiguration - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           
            //This class is Get QB Online Configuration
            return View(_qBRepository.GetQBDesktopConfiguration(qBConfigurationViewModal));
        }
        /// <summary>
        /// This methos is Update bank details
        /// </summary>
        /// <param name="updateBank"></param>
        /// <returns></returns>
        public ActionResult UpdateBank(UpdateBank updateBank)
        {
            //This class is Update Back Details
            _qBRepository.UpdateBankDetails(updateBank);
            return RedirectToAction("Index", "QBConfiguration");
        }
        /// <summary>
        /// This method is Genrate Xml d
        /// </summary>
        /// <param name="generateXML"></param>
        /// <returns></returns>
        public FileStreamResult GenerateXML(GenerateXML generateXML)
        {
            var url = Request.Url.GetLeftPart(UriPartial.Authority).Replace("http", "https");
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            try
            {
                //Get UserID by Username.
                if (_commonRepository.getUserId(UserName) != 0)
                {
                    if (generateXML.ID != 0)
                    {
                        int StoreID = Convert.ToInt32(TempData["StoreID"].ToString());
                        //This class Generate XML 
                        QBDesktopConfiguration QBDesktopConfiguration = _qBRepository.GenerateXML(generateXML, StoreID);

                        MemoryStream ms = new MemoryStream();
                        XmlWriterSettings xws = new XmlWriterSettings();
                        xws.Indent = true;

                        using (XmlWriter xw = XmlWriter.Create(ms, xws))
                        {
                            XDocument doc = new XDocument
                                (
                              new XDeclaration("1.0", null, null),
                              new XElement("QBWCXML",
                              new XElement("AppName", QBDesktopConfiguration.AppName),
                              new XElement("AppID", QBDesktopConfiguration.QBDesktopId),
                              new XElement("AppURL", Request.Url.GetLeftPart(UriPartial.Authority) + "/QBClass/wcwebservice.asmx"),
                              new XElement("AppDescription", QBDesktopConfiguration.Description),
                              new XElement("AppSupport", Request.Url.GetLeftPart(UriPartial.Authority) + "/QBClass/wcwebservice.asmx?wsdl"),
                              new XElement("OwnerID", "{" + QBDesktopConfiguration.OwnerID + "}"),
                              new XElement("FileID", "{" + QBDesktopConfiguration.FileID + "}"),
                              new XElement("UserName", QBDesktopConfiguration.UserName),
                              new XElement("Password", QBDesktopConfiguration.Password),
                              new XElement("QBType", "QBFS"),
                              new XElement("Style", "Document"),
                              new XElement("AuthFlags", "0xF")
                             )
                            );
                            doc.Save(xw);
                        }
                        ms.Position = 0;
                        return File(ms, "text/xml", QBDesktopConfiguration.AppName.Replace(" ", "").Trim() + ".qwc");
                    }
                    else
                    {
                        return null;
                    }

                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.Error("QBConfigurationController - GenerateXML - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return null;

        }
        /// <summary>
        /// This method is QB Desktop Configuration
        /// </summary>
        /// <param name="objModel"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QBDesktopConfiguration(QBDesktopConfiguration objModel)
        {
            TempData["FromQBConfig"] = "DesktopQBConfig";
            Message = string.Empty;
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            try
            {
                //Get UserID by Username.
                if (_commonRepository.getUserId(UserName) != 0)
                {
                    if (ModelState.IsValid)
                    {
                        if (!objModel.QBCompanyPath.Contains(".qbw"))
                        {
                            ModelState.AddModelError("QBCompanyPath", "Select QuickBook Company Location Valid File.");
                            return View();
                        }

                        string SimpleAppName = Regex.Replace(objModel.AppName, @"[^0-9a-zA-Z]+", ",");

                        Message = "success";
                    }
                }
                else
                {
                    Message = "User Expired";
                }
            }
            catch (Exception ex)
            {
                logger.Error("QBConfigurationController - QBDesktopConfiguration - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           
            return Json(Message, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method save QB Online Configurations
        /// </summary>
        /// <returns></returns>
        public ActionResult SaveData()
        {
            bool isqbSynce = false;
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            //Get UserID by Username.
            int CurrentUserId = _commonRepository.getUserId(UserName);
            if (CurrentUserId != 0)
            {
                try
                {
                    if (QBRequest.AuObj.RealmId != null && TempData["StoreID"].ToString() != "")
                    {
                        //Get select By Store id.
                        var Dt = _qBRepository.SelectByStoreID(Convert.ToInt32(TempData["StoreID"].ToString()));
                        if (Dt.Count == 0)
                        {
                            QBOnlineConfiguration QBOnlines = new QBOnlineConfiguration();
                            QBOnlines.RealmId = QBRequest.AuObj.RealmId;
                            QBOnlines.ClientId = QBRequest.AuObj.ClientId;
                            QBOnlines.ClientSecretKey = QBRequest.AuObj.ClientSecretKey;
                            QBOnlines.QBToken = QBRequest.AuObj.QBToken;
                            QBOnlines.QBRefreshToken = QBRequest.AuObj.QBRefreshToken;
                            QBOnlines.CreatedOn = DateTime.Now.Date;
                            QBOnlines.CreatedBy = CurrentUserId;
                            QBOnlines.StoreId = Convert.ToInt32(TempData["StoreID"]);
                            QBOnlines.IsActive = true;
                            QBOnlines.IsTokenSuspend = 0;
                            QBOnlines.Flag = 1;
                            //Save Qb Online Configuration
                            _qBRepository.SaveQBOnlineConfigurations(QBOnlines);
                            QBOnlines = null;
                        }
                        else
                        {
                            //Get Qb Online ID.
                            QBOnlineConfiguration obj2 = _qBRepository.GetQBOnlineId(Dt.FirstOrDefault().QBOnlineId);
                            obj2.RealmId = QBRequest.AuObj.RealmId;
                            obj2.ClientId = QBRequest.AuObj.ClientId;
                            obj2.ClientSecretKey = QBRequest.AuObj.ClientSecretKey;
                            obj2.QBToken = QBRequest.AuObj.QBToken;
                            obj2.QBRefreshToken = QBRequest.AuObj.QBRefreshToken;
                            obj2.CreatedOn = DateTime.Now.Date;
                            obj2.CreatedBy = CurrentUserId;
                            obj2.UpdatedOn = DateTime.Now;
                            obj2.StoreId = Convert.ToInt32(TempData["StoreID"]);
                            obj2.IsActive = true;
                            obj2.IsTokenSuspend = 0;
                            obj2.Flag = 1;
                            //Update Qb Online Configuration

                            _qBRepository.UpdateQBOnlineConfigurations(obj2);
                            obj2 = null;
                        }

                        if (Dt.Count > 0)
                        {
                            QBRequest.AuObj.ClientId = Dt.FirstOrDefault().ClientId.ToString();
                            QBRequest.AuObj.QBToken = Dt.FirstOrDefault().QBToken.ToString();
                            QBRequest.AuObj.RealmId = Dt.FirstOrDefault().RealmId.ToString();
                            QBRequest.AuObj.ClientSecretKey = Dt.FirstOrDefault().ClientSecretKey.ToString();
                            QBRequest.AuObj.QBRefreshToken = Dt.FirstOrDefault().QBRefreshToken.ToString();
                            Session["QBConnectionType"] = Utility.AdminSiteConfiguration.QBConnectionType.Online;
                        }

                        //this class is Select By Store ID for desktop
                        var Dt1 = _qBRepository.SelectByStoreIDForDesktop(Convert.ToInt32(TempData["StoreID"].ToString()));
                        if (Dt1.Count > 0)
                        {
                            //Get Qb Desktop ID
                            _qBRepository.GetQBDesktopId(Dt1.FirstOrDefault().QBDesktopId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("QBConfigurationController - SaveData - " + DateTime.Now + " - " + ex.Message.ToString());
                }
               

                SaveHistory();
                TempData["sMessage"] = "success";
                //if (isqbSynce == false)
                //{
                //    GetVendor_Department(Convert.ToInt32(TempData["StoreID"].ToString()));
                //}
                return RedirectToAction("Index", "QBConfiguration");
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        /// <summary>
        /// This class is save History
        /// </summary>
        public void SaveHistory()
        {
            try
            {
                //This class is Select By ID Qb History
                var Dt1 = _qBRepository.SelectByIdQBHistory(Convert.ToInt32(TempData["StoreID"].ToString()));
                if (Dt1.Count == 0)
                {
                    QBHistory QBHistorys = new QBHistory();
                    QBHistorys.StoreId = Convert.ToInt32(TempData["StoreID"].ToString());
                    QBHistorys.StartDate = DateTime.Now;
                    QBHistorys.Operation = Convert.ToInt32(Utility.AdminSiteConfiguration.QBConnectionType.Online);
                    //Save QB Historys
                    _qBRepository.SaveQBHistorys(QBHistorys);
                    QBHistorys = null;
                }
                else
                {
                    string QBHistoryId = Dt1.FirstOrDefault().QBHistoryId.ToString();
                    // Update QB Historys
                    _qBRepository.UpdateQBHistory(Dt1.FirstOrDefault().QBHistoryId);




                    QBHistory QBHistorys = new QBHistory();
                    QBHistorys.StoreId = Convert.ToInt32(TempData["StoreID"].ToString());
                    QBHistorys.StartDate = DateTime.Now;
                    QBHistorys.Operation = Convert.ToInt32(Utility.AdminSiteConfiguration.QBConnectionType.Online);
                    //Save QB Historys
                    _qBRepository.SaveQBHistorys(QBHistorys);
                    QBHistorys = null;
                }
            }
            catch (Exception ex)
            {
                logger.Error("QBConfigurationController - SaveHistory - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            
        }

        /// <summary>
        /// This Method is Get Vendor Department using storeid
        /// </summary>
        /// <param name="StoreID"></param>
        /// <returns></returns>
        public ActionResult GetVendor_Department(int StoreID)
        {           
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            //Get UserID by Username.
            if (_commonRepository.getUserId(UserName) != 0)
            {
                SynthesisQBOnline.BAL.QBOnlineconfiguration objOnline = new SynthesisQBOnline.BAL.QBOnlineconfiguration();
                //Get select By Store id.
                var Dt = _qBRepository.SelectByStoreID(StoreID);
                if (Dt.Count > 0)
                {
                    objOnline.URL = ConfigurationManager.AppSettings["qboBaseUrl"];
                    objOnline.RealmID = Dt.LastOrDefault().RealmId.ToString();
                    objOnline.ClientId = Dt.LastOrDefault().ClientId.ToString();
                    objOnline.ClientSecretKey = Dt.LastOrDefault().ClientSecretKey.ToString();
                    objOnline.QBToken = Dt.LastOrDefault().QBToken.ToString();
                    objOnline.QBRefreshToken = Dt.LastOrDefault().QBRefreshToken.ToString();
                }

                List<SynthesisQBOnline.BAL.VendorMaster> dtVendor = SynthesisQBOnline.QBClass.QBVendor.GetVendor_All(objOnline, ref objResponse);

                output("Vendor Total: " + dtVendor.Count);
                output("Status: " + objResponse.Status);
                output("Message: " + objResponse.Message);

                if (dtVendor.Count > 0)
                {
                    foreach (var item in dtVendor)
                    {
                        try
                        {
                            var name = item.DisplayName;
                            //Get select By Store id name vendor.
                            var vData1 = _qBRepository.SelectByStoreID_NameVendor(StoreID, item.DisplayName);
                            if (vData1 != null)
                            {
                                //Get vendor Master using Vendorid
                                VendorMaster objVendor = _qBRepository.VendorMasters().Where(a => a.VendorId == vData1.VendorId && vData1.StoreId == StoreID).FirstOrDefault();
                                objVendor.ListId = item.ID;
                                objVendor.VendorName = item.DisplayName;
                                objVendor.PhoneNumber = item.Mobile;
                                objVendor.CompanyName = item.CompanyName;
                                objVendor.PrintOnCheck = item.PrintOnCheckas;
                                objVendor.IsActive = Convert.ToBoolean(item.IsActive);
                                objVendor.Address = item.Address1;
                                //objVendor.Address2 = item.Address2 ;
                                objVendor.City = item.City;
                                objVendor.State = item.State;
                                objVendor.Country = item.Country;
                                objVendor.PostalCode = item.ZipCode;
                                objVendor.EMail = item.Email;
                                //Get UserID by Username.
                                objVendor.ModifiedBy = _commonRepository.getUserId(UserName);
                                objVendor.ModifiedOn = DateTime.Now;
                                objVendor.StoreId = StoreID;
                                objVendor.IsSync = true;
                                //This class update vendor Master data
                                _qBRepository.UpdateVendorMaster(objVendor);
                                objVendor = null;

                            }
                            else
                            {
                                if (item.IsActive == "true")
                                {
                                    //Get select By StoreId and Vendor.
                                    var vData = _qBRepository.SelectByStoreID_IDVendor(StoreID, Convert.ToInt32(item.ID));
                                    if (vData != null)
                                    {
                                        //Get vendor Master using Vendorid
                                        var objVendor = _qBRepository.VendorMasters().Where(a => a.VendorId == vData.VendorId && a.StoreId == StoreID).FirstOrDefault();
                                        objVendor.ListId = item.ID;
                                        objVendor.VendorName = item.DisplayName;
                                        objVendor.PhoneNumber = item.Mobile;
                                        objVendor.CompanyName = item.CompanyName;
                                        objVendor.PrintOnCheck = item.PrintOnCheckas;
                                        objVendor.IsActive = Convert.ToBoolean(item.IsActive);
                                        objVendor.Address = item.Address1;
                                        //objVendor.Address2 = item.Address2 ;
                                        objVendor.City = item.City;
                                        objVendor.State = item.State;
                                        objVendor.Country = item.Country;
                                        objVendor.PostalCode = item.ZipCode;
                                        objVendor.EMail = item.Email;
                                        //Get UserID by Username.
                                        objVendor.ModifiedBy = _commonRepository.getUserId(UserName);
                                        objVendor.ModifiedOn = DateTime.Now;
                                        objVendor.StoreId = StoreID;
                                        objVendor.IsSync = true;
                                        _qBRepository.UpdateVendorMaster(objVendor);
                                        objVendor = null;
                                    }
                                    else
                                    {
                                        VendorMaster objVendor = new VendorMaster();
                                        objVendor.ListId = item.ID;
                                        objVendor.VendorName = item.DisplayName;
                                        objVendor.PhoneNumber = item.Mobile;
                                        objVendor.CompanyName = item.CompanyName;
                                        objVendor.PrintOnCheck = item.PrintOnCheckas;
                                        objVendor.IsActive = Convert.ToBoolean(item.IsActive);
                                        objVendor.Address = item.Address1;
                                        //objVendor.Address2 = item.Address2 ;
                                        objVendor.City = item.City;
                                        objVendor.State = item.State;
                                        objVendor.Country = item.Country;
                                        objVendor.PostalCode = item.ZipCode;
                                        objVendor.EMail = item.Email;
                                        //Get UserID by Username.
                                        objVendor.CreatedBy = _commonRepository.getUserId(UserName);
                                        objVendor.CreatedOn = DateTime.Now;
                                        objVendor.StoreId = StoreID;
                                        objVendor.ModifiedOn = DateTime.Now;
                                        objVendor.IsSync = true;
                                        objVendor.SyncDate = DateTime.Now;
                                        //This class Save vendor Master data
                                        _qBRepository.SaveVendorMasters(objVendor);
                                        objVendor = null;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error("QBConfigurationController - GetVendor_Department - " + DateTime.Now + " - " + ex.Message.ToString());
                        }
                       
                    }
                }

                List<SynthesisQBOnline.BAL.AccountMaster> dtAccount = SynthesisQBOnline.QBAccount.GetDept_All(objOnline, ref objResponse);
                output("Account Total: " + dtAccount.Count);
                output("Status: " + objResponse.Status);
                output("Message: " + objResponse.Message);

                if (dtAccount.Count > 0)
                {
                    foreach (var item in dtAccount)
                    {
                        try
                        {
                            //Get select By Store id using Department.
                            var vData1 = _qBRepository.SelectByStoreID_Department(StoreID, item.Department);
                            if (vData1.Count > 0)
                            {
                                DepartmentMaster objDept = new DepartmentMaster();
                                objDept.ListId = item.ID;
                                objDept.DepartmentName = item.Department;
                                //Get Account type Master list using accountype
                                var AccountTypeId = _qBRepository.AccountTypeMastersList().Where(s => s.AccountType == item.AccountType).FirstOrDefault().AccountTypeId;
                                objDept.AccountTypeId = AccountTypeId;
                                //Get all account Type
                                var DtAcc = _qBRepository.GetAccountType(item.AccountType, "O");
                                if (DtAcc != null)
                                {
                                    objDept.AccountTypeId = DtAcc.AccountTypeId;
                                }
                                else
                                {
                                    AccountTypeMaster objAcc = new AccountTypeMaster();
                                    objAcc.AccountType = item.AccountType;
                                    objAcc.Flag = "O";
                                    objAcc.CommonType = item.AccountType;
                                    //Save Account Type Master data.
                                    _qBRepository.SaveAccountTypeMasters(objAcc);
                                    int id = objAcc.AccountTypeId;
                                    objAcc = null;
                                    objDept.AccountTypeId = id;
                                }
                                //Get all Qb details type
                                var DtAccDetail = _qBRepository.GetQBDetailType(item.DetailType);
                                if (DtAccDetail != null)
                                {
                                    objDept.AccountDetailTypeId = DtAccDetail.AccountDetailTypeId;
                                    if (objDept.AccountDetailTypeId == 0 || objDept.AccountDetailTypeId == null)
                                    {
                                        AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
                                        objDetail.DetailType = item.DetailType;
                                        objDetail.QBDetailType = item.DetailType;
                                        objDetail.AccountTypeId = objDept.AccountTypeId;
                                        //This class save account details Type masters
                                        _qBRepository.SaveAccountDetailTypeMasters(objDetail);
                                        objDetail = null;
                                    }
                                }
                                else
                                {
                                    AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
                                    objDetail.DetailType = item.DetailType;
                                    objDetail.QBDetailType = item.DetailType;
                                    objDetail.AccountTypeId = objDept.AccountTypeId;
                                    //This class save account details Type masters
                                    _qBRepository.SaveAccountDetailTypeMasters(objDetail);
                                    int id = objDetail.AccountDetailTypeId;
                                    objDetail = null;
                                    objDept.AccountDetailTypeId = id;
                                }

                                objDept.Description = item.Description;
                                objDept.AccountNumber = item.AccountNumber;
                                objDept.IsSubAccount = item.SubAccount;
                                objDept.IsActive = Convert.ToBoolean(item.IsActive);
                                objDept.StoreId = StoreID;
                                objDept.ModifiedOn = DateTime.Now;
                                //Get UserID by Username.
                                objDept.ModifiedBy = _commonRepository.getUserId(UserName);
                                objDept.IsSync = 1;
                                objDept.SyncDate = DateTime.Now;
                                objDept.DepartmentId = vData1.FirstOrDefault().DepartmentId;
                                //Update department Master data
                                _qBRepository.UpdateDepartmentMaster(objDept);
                                objDept = null;
                            }
                            else
                            {
                                try
                                {
                                    if (item.IsActive == "true")
                                    {
                                        //Get select By Store id and List Id of department.
                                        var vData = _qBRepository.SelectByStoreID_ListIDDepartment(StoreID, item.ID);
                                        if (vData != null)
                                        {
                                            //Get all Department master list.
                                            DepartmentMaster objDept = _qBRepository.DepartmentMastersList().Where(a => a.DepartmentId == vData.DepartmentId && vData.StoreId == StoreID).FirstOrDefault();
                                            objDept.ListId = item.ID;
                                            objDept.DepartmentName = item.Department;
                                            //This class get Account type
                                            var DtAcc = _qBRepository.GetAccountType(item.AccountType, "O");
                                            if (DtAcc != null)
                                            {
                                                objDept.AccountTypeId = DtAcc.AccountTypeId;
                                            }
                                            else
                                            {
                                                AccountTypeMaster objAcc = new AccountTypeMaster();
                                                objAcc.AccountType = item.AccountType;
                                                objAcc.Flag = "O";
                                                objAcc.CommonType = item.AccountType;
                                                //This class save account  Type masters
                                                _qBRepository.SaveAccountTypeMasters(objAcc);
                                                int id = objAcc.AccountTypeId;
                                                objAcc = null;
                                                objDept.AccountTypeId = id;
                                            }
                                            //Get all Qb details type
                                            var DtAccDetail = _qBRepository.GetQBDetailType(item.DetailType);
                                            if (DtAccDetail != null)
                                            {
                                                objDept.AccountDetailTypeId = DtAccDetail.AccountDetailTypeId;
                                                if (objDept.AccountDetailTypeId == 0 || objDept.AccountDetailTypeId == null)
                                                {
                                                    AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
                                                    objDetail.DetailType = item.DetailType;
                                                    objDetail.QBDetailType = item.DetailType;
                                                    objDetail.AccountTypeId = objDept.AccountTypeId;
                                                    //This class save account details Type masters
                                                    _qBRepository.SaveAccountDetailTypeMasters(objDetail);
                                                    objDetail = null;
                                                }
                                            }
                                            else
                                            {
                                                AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
                                                objDetail.DetailType = item.DetailType;
                                                objDetail.QBDetailType = item.DetailType;
                                                objDetail.AccountTypeId = objDept.AccountTypeId;
                                                //This class save account details Type masters
                                                _qBRepository.SaveAccountDetailTypeMasters(objDetail);
                                                int id = objDetail.AccountDetailTypeId;
                                                objDetail = null;
                                                objDept.AccountDetailTypeId = id;
                                            }

                                            objDept.Description = item.Description;
                                            objDept.AccountNumber = item.AccountNumber;
                                            objDept.IsSubAccount = item.SubAccount;
                                            objDept.IsActive = Convert.ToBoolean(item.IsActive);
                                            objDept.StoreId = StoreID;
                                            objDept.ModifiedOn = DateTime.Now;
                                            //Get UserID by Username.
                                            objDept.ModifiedBy = _commonRepository.getUserId(UserName);
                                            objDept.IsSync = 1;
                                            objDept.SyncDate = DateTime.Now;
                                            objDept.DepartmentId = vData.DepartmentId;
                                            //Update department Master data
                                            _qBRepository.UpdateDepartmentMaster(objDept);
                                            objDept = null;
                                        }
                                        else
                                        {
                                            DepartmentMaster objDept = new DepartmentMaster();
                                            objDept.ListId = item.ID;
                                            objDept.DepartmentName = item.Department;
                                            //Get all Account type
                                            var DtAcc = _qBRepository.GetAccountType(item.AccountType, "O");
                                            if (DtAcc != null)
                                            {
                                                objDept.AccountTypeId = DtAcc.AccountTypeId;
                                            }
                                            else
                                            {
                                                AccountTypeMaster objAcc = new AccountTypeMaster();
                                                objAcc.AccountType = item.AccountType;
                                                objAcc.Flag = "O";
                                                objAcc.CommonType = item.AccountType;
                                                //This class save account Type masters
                                                _qBRepository.SaveAccountTypeMasters(objAcc);
                                                int id = objAcc.AccountTypeId;
                                                objAcc = null;
                                                objDept.AccountTypeId = id;
                                            }
                                            //Get all Qb details type
                                            var DtAccDetail = _qBRepository.GetQBDetailType(item.DetailType);
                                            if (DtAccDetail != null)
                                            {
                                                objDept.AccountDetailTypeId = DtAccDetail.AccountDetailTypeId;
                                                if (objDept.AccountDetailTypeId == 0)
                                                {
                                                    AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
                                                    objDetail.DetailType = item.DetailType;
                                                    objDetail.QBDetailType = item.DetailType;
                                                    objDetail.AccountTypeId = objDept.AccountTypeId;
                                                    //This class save account details Type masters
                                                    _qBRepository.SaveAccountDetailTypeMasters(objDetail);
                                                    objDetail = null;
                                                }
                                            }
                                            else
                                            {
                                                AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
                                                objDetail.DetailType = item.DetailType;
                                                objDetail.QBDetailType = item.DetailType;
                                                objDetail.AccountTypeId = objDept.AccountTypeId;
                                                //This class save account details Type masters
                                                _qBRepository.SaveAccountDetailTypeMasters(objDetail);
                                                int id = objDetail.AccountDetailTypeId;
                                                objDetail = null;
                                                objDept.AccountDetailTypeId = id;
                                            }
                                            objDept.Description = item.Description;
                                            objDept.AccountNumber = item.AccountNumber;
                                            objDept.IsSubAccount = item.SubAccount;
                                            objDept.IsActive = Convert.ToBoolean(item.IsActive);
                                            objDept.StoreId = StoreID;
                                            objDept.CreatedOn = DateTime.Now;
                                            objDept.ModifiedOn = DateTime.Now;
                                            //Get UserID by Username.
                                            objDept.CreatedBy = _commonRepository.getUserId(UserName);
                                            objDept.ModifiedBy = _commonRepository.getUserId(UserName);
                                            objDept.IsSync = 1;
                                            objDept.SyncDate = DateTime.Now;
                                            //Save department Master data
                                            _qBRepository.SaveDepartmentMasters(objDept);
                                            objDept = null;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logger.Error("QBConfigurationController - GetVendor_Department - " + DateTime.Now + " - " + ex.Message.ToString());
                                }
                                
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error("QBConfigurationController - GetVendor_Department - " + DateTime.Now + " - " + ex.Message.ToString());
                        }
                       
                    }
                }
                output("Department-Vednor Complete");
                return RedirectToAction("Index", "QBConfiguration");


            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        /// <summary>
        /// this class is output log message
        /// </summary>
        /// <param name="logMsg"></param>
        public void output(string logMsg)
        {
            //Console.WriteLine(logMsg);
            logPath = "~/Log/";
            System.IO.StreamWriter sw = System.IO.File.AppendText(Server.MapPath(logPath) + "OAuth2SampleAppLogs.txt");
            try
            {
                string logLine = System.String.Format(
                    "{0:G}: {1}.", System.DateTime.Now, logMsg);
                sw.WriteLine(logLine);
            }
            catch (Exception ex)
            {
                logger.Error("QBConfigurationController - output - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            finally
            {
                sw.Close();
            }
        }

        /// <summary>
        /// This Method is QB Synchronized Invoices
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        public ActionResult QBSynchronizedInvoices()
        {
            try
            {
                if (Convert.ToString(Session["storeid"]) != "0")
                {
                    int StoreId = Convert.ToInt32(Session["storeid"]);
                    //Get store masters List
                    ViewBag.StoreId = new SelectList(_qBRepository.StoreMastersList().Where(s => s.IsActive == true).Select(s => new { s.StoreId, s.Name }).OrderBy(o => o.Name), "StoreId", "Name", StoreId);
                }
                else
                {
                    //Get store masters List
                    ViewBag.StoreId = new SelectList(_qBRepository.StoreMastersList().Where(s => s.IsActive == true).Select(s => new { s.StoreId, s.Name }).OrderBy(o => o.Name), "StoreId", "Name");
                }
            }
            catch (Exception ex)
            {
                logger.Error("QBConfigurationController - QBSynchronizedInvoices - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           
            return View();
        }
        /// <summary>
        /// This method is Get Sync Invoce data
        /// </summary>
        /// <param name="StoreID"></param>
        /// <param name="txtSearchTitle"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult getSyncInvoiceData(int? StoreID, string txtSearchTitle)
        {
            List<Invoice> InvoiceList = new List<Invoice>();
            try
            {
                if (StoreID == 0 || StoreID == null)
                {
                    //InvoiceList = db.Invoices.Where(s => s.StatusValue == InvoiceStatusEnm.Approved && (s.TXNId != null && s.TXNId != "")).Select(s => new
                    //{
                    //    VendorName = s.VendorMasters.VendorName,
                    //    InvoiceId = s.InvoiceId,
                    //    StoreId = s.StoreId,
                    //    PaymentType = s.PaymentTypeMasters.PaymentType,
                    //    InvoiceType = s.InvoiceTypeMasters.InvoiceType,
                    //    InvoiceDate = s.InvoiceDate,
                    //    InvoiceNumber = s.InvoiceNumber,
                    //    TotalAmount = s.TotalAmount

                    //}).ToList().Select(k => new Invoice()
                    //{
                    //    VendorName = k.VendorName,
                    //    InvoiceId = k.InvoiceId,
                    //    StoreId = k.StoreId,
                    //    PaymentType = k.PaymentType,
                    //    InvoiceType = k.InvoiceType,
                    //    InvoiceDate = k.InvoiceDate,
                    //    InvoiceNumber = k.InvoiceNumber,
                    //    TotalAmount = k.TotalAmount,
                    //    strInvoiceDate = AdminSiteConfiguration.GetDateformat(Convert.ToString(k.InvoiceDate))
                    //}).OrderByDescending(o => o.InvoiceId).ToList();
                }
                else
                {
                    //Get Invoices masters List
                    InvoiceList = _qBRepository.InvoicesList().Where(s => s.StatusValue == InvoiceStatusEnm.Approved && (s.TXNId == null || s.TXNId == "") && s.StoreId == StoreID).Select(s => new
                    {
                        VendorName = s.VendorMasters.VendorName,
                        InvoiceId = s.InvoiceId,
                        StoreId = s.StoreId,
                        PaymentType = s.PaymentTypeMasters.PaymentType,
                        InvoiceType = s.InvoiceTypeMasters.InvoiceType,
                        InvoiceDate = s.InvoiceDate,
                        InvoiceNumber = s.InvoiceNumber,
                        TotalAmount = s.TotalAmount

                    }).ToList().Select(k => new Invoice()
                    {
                        VendorName = k.VendorName,
                        InvoiceId = k.InvoiceId,
                        StoreId = k.StoreId,
                        PaymentType = k.PaymentType,
                        InvoiceType = k.InvoiceType,
                        InvoiceDate = k.InvoiceDate,
                        InvoiceNumber = k.InvoiceNumber,
                        TotalAmount = k.TotalAmount,
                        strInvoiceDate = Utility.AdminSiteConfiguration.GetDateformat(Convert.ToString(k.InvoiceDate))
                    }).OrderByDescending(o => o.InvoiceId).ToList();
                }
                if (txtSearchTitle != "" && txtSearchTitle != null)
                {
                    InvoiceList = InvoiceList.Where(s => s.VendorName.ToLower().Trim().Contains(txtSearchTitle.Trim().ToLower()) || s.InvoiceNumber.ToString().ToLower().Trim().Contains(txtSearchTitle.Trim().ToLower())).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error("QBConfigurationController - getSyncInvoiceData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           
            return PartialView("_QBSynchronizedInvoicesList", InvoiceList);

        }
        /// <summary>
        /// This method is Get Qb Sync Online data
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        public ActionResult QBSyncOnlineData()
        {
            ViewBag.Title = "QuickBook Sync Start/Stop - Synthesis";
            _commonRepository.LogEntries();     //Harsh's code
            //Get QB Sync data
            var StoreModel = _qBRepository.GetQBSyncData();
            try
            {
                foreach (var item in StoreModel)
                {
                    if (item.QBtype == "Online")
                    {
                        if (item.QBOnlineFlag == 1)
                        {
                            //Get total count using store id
                            var DtTotal = _qBRepository.GetTotalCount(item.StoreId);
                            if (DtTotal.FirstOrDefault().TotalCount == 0)
                            {
                            }
                            else
                            {
                                item.OnlineStatus = "true";
                                item.OnlineCount = DtTotal.FirstOrDefault().TotalCount.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("QBConfigurationController - QBSyncOnlineData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
          

            return View(StoreModel);
        }
        /// <summary>
        /// This method is get Unsccesss Full invoice data
        /// </summary>
        /// <param name="StoreID"></param>
        /// <returns></returns>
        public ActionResult GetUnsuccessfullInvoice(int StoreID)
        {
            //Get Unsuccess Full Invoice
            var Dt = _qBRepository.GetUnsuccessfullInvoice(StoreID);
            return View(Dt);
        }

        /// <summary>
        /// This method is Update Unsccesss Full Details
        /// </summary>
        /// <param name="InvoiceID"></param>
        /// <returns></returns>
        public ActionResult UpdateUnsuccessfullDetail(string InvoiceID)
        {
            try
            {
                if (InvoiceID != "" && InvoiceID != null)
                {
                    //string username = WebSecurity.CurrentUserName;
                    int[] Inv = Array.ConvertAll(InvoiceID.TrimEnd(',').Split(','), int.Parse);
                    //This class is update Unsuccess Invoice
                    _qBRepository.InvoicesUpdateUnsuccess(Inv);
                    Message = "success";
                }
            }
            catch (Exception ex)
            {
                logger.Error("QBConfigurationController - UpdateUnsuccessfullDetail - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           
            return Json(Message, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Ignore  Unsccesss Full Details
        /// </summary>
        /// <param name="InvoiceID"></param>
        /// <returns></returns>
        public ActionResult IgnoreUnsuccessfullDetail(string InvoiceID)
        {
            try
            {
                if (InvoiceID != "" && InvoiceID != null)
                {
                    //string username = WebSecurity.CurrentUserName;
                    int[] Inv = Array.ConvertAll(InvoiceID.TrimEnd(',').Split(','), int.Parse);
                    //This class is Ignore Unsuccess Full Invoice
                    _qBRepository.InvoicesIgnoreUnsuccess(Inv);
                    Message = "success";
                }
            }
            catch (Exception ex)
            {
                logger.Error("QBConfigurationController - IgnoreUnsuccessfullDetail - " + DateTime.Now + " - " + ex.Message.ToString());
            }
          
            return Json(Message, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult SyncData(int StoreID, string Flag)
        //{
        //    if (Flag == "1")
        //    {
        //        var Dt = db.Database.SqlQuery<VendorMaster>("SP_VendorMaster @Mode,@StoreId", new SqlParameter("@Mode", "GetUnSyncVendor"), new SqlParameter("@StoreId", StoreID)).ToList();
        //        VendorMaster obj;
        //        foreach(var item in Dt)
        //        {
        //            int iVendorId = 0;
        //            obj = new VendorMaster();
        //            obj.VendorName = item.VendorName.ToString();
        //            obj.VendorId = item.VendorId;
        //            obj.CompanyName = item.CompanyName.ToString();
        //            obj.PhoneNumber = item.PhoneNumber.ToString();
        //            obj.Address = item.Address.ToString();
        //            obj.Address2 = item.Address2.ToString();
        //            obj.State = item.State.ToString();
        //            obj.City = item.City.ToString();
        //            obj.Country = item.Country.ToString();
        //            obj.PostalCode = item.PostalCode.ToString();
        //            SynthesisQBOnline.BAL.QBResponse objResponse = new SynthesisQBOnline.BAL.QBResponse();
        //            QBClass.QBSyncVendor(obj, StoreID, ref objResponse);

        //            if (objResponse.ID != "0" && objResponse.Status == "Done")
        //            {
        //                var vData1 = db.Database.SqlQuery<VendorMaster>("SP_VendorMaster @Mode,@StoreId,@VendorName", new SqlParameter("@Mode", "SelectByStoreID_Name"), new SqlParameter("@StoreId", StoreID), new SqlParameter("@VendorName", obj.VendorName)).ToList();
        //                if (vData1.Count > 0)
        //                {
        //                    var vData = db.Database.SqlQuery<VendorMaster>("SP_VendorMaster @Mode,@StoreId,@VendorName,@VendorId", new SqlParameter("@Mode", "SelectForUpdate"), new SqlParameter("@StoreId", StoreID), new SqlParameter("@VendorName", obj.VendorName), new SqlParameter("@VendorId", obj.VendorId)).ToList();
        //                    if (vData.Count > 0)
        //                    {
        //                        Message = "Vendor Name Already Exist";
        //                        TempData["Sucessme"] = Message;
        //                        return RedirectToAction("Create", "VendorMasters");
        //                    }
        //                    VendorMaster objVendor = new VendorMaster();
        //                    objVendor.ListId = objResponse.ID;
        //                    objVendor.VendorName = obj.VendorName;
        //                    objVendor.PhoneNumber = obj.PhoneNumber;
        //                    objVendor.CompanyName = obj.CompanyName;
        //                    objVendor.IsActive = true;
        //                    objVendor.Address = obj.Address;
        //                    objVendor.Address2 = obj.Address2;
        //                    objVendor.City = obj.City;
        //                    objVendor.State = obj.State;
        //                    objVendor.Country = obj.Country;
        //                    objVendor.PostalCode = obj.PostalCode;
        //                    objVendor.ModifiedBy = UserModule.getUserId();
        //                    objVendor.ModifiedOn = DateTime.Now;
        //                    objVendor.StoreId = StoreID;
        //                    objVendor.IsSync = true;
        //                    db.Entry(objVendor).State = EntityState.Modified;
        //                    db.SaveChanges();
        //                    iVendorId = obj.VendorId;
        //                    if (iVendorId == 0)
        //                    {
        //                        Message = "Vendor Not Synched in QuickBook.";
        //                        TempData["Sucessme"] = Message;
        //                    }
        //                    else
        //                    {
        //                        TempData["Sucessme"] = "Vendor Save Successfully..";
        //                    }
        //                    objVendor = null;
        //                }
        //                else
        //                {
        //                    var vData = db.Database.SqlQuery<VendorMaster>("SP_VendorMaster @Mode,@StoreId,@VendorName,@VendorId", new SqlParameter("@Mode", "SelectForUpdate"), new SqlParameter("@StoreId", StoreID), new SqlParameter("@VendorName", obj.VendorName), new SqlParameter("@VendorId", obj.VendorId)).ToList();
        //                    if(vData.Count > 0)
        //                    {
        //                        Message = "Vendor Name Already Exist";
        //                        TempData["Sucessme"] = Message;
        //                    }
        //                    VendorMaster objVendor = new VendorMaster();
        //                    objVendor.ListId = item.ListId;
        //                    objVendor.VendorName = item.VendorName;
        //                    objVendor.PhoneNumber = item.PhoneNumber;
        //                    objVendor.CompanyName = item.CompanyName;
        //                    objVendor.PrintOnCheck = item.PrintOnCheck;
        //                    objVendor.IsActive = Convert.ToBoolean(item.IsActive);
        //                    objVendor.Address = item.Address;
        //                    objVendor.Address2 = item.Address2;
        //                    objVendor.City = item.City;
        //                    objVendor.State = item.State;
        //                    objVendor.Country = item.Country;
        //                    objVendor.PostalCode = item.PostalCode;
        //                    objVendor.CreatedBy = UserModule.getUserId();
        //                    objVendor.CreatedOn = DateTime.Now;
        //                    objVendor.StoreId = StoreID;
        //                    objVendor.ModifiedOn = DateTime.Now;
        //                    objVendor.IsSync = true;
        //                    objVendor.SyncDate = DateTime.Now;
        //                    db.VendorMasters.Add(objVendor);
        //                    db.SaveChanges();
        //                    objVendor = null;
        //                    TempData["Sucessme"] = "Vendor Save Successfully..";
        //                    objVendor = null;
        //                }
        //            }
        //            else
        //            {
        //                Message = objResponse.Status.ToString();
        //            }
        //        }
        //        Dt = null;
        //        obj = null;

        //        var Dt2 = db.Database.SqlQuery<DepartmentMaster>("SP_DepartmentMaster @Mode,@StoreId", new SqlParameter("@Mode", "GetUnSyncDepartment"), new SqlParameter("@StoreId", StoreID)).ToList();
        //        DepartmentMaster objDepts;
        //        foreach (var item in Dt2)
        //        {
        //            int iDeptId = 0;
        //            objDepts = new DepartmentMaster();
        //            objDepts.DepartmentName = item.DepartmentName.ToString();
        //            objDepts.DepartmentId = item.DepartmentId;
        //            objDepts.AccountTypeId = item.AccountTypeId;
        //            objDepts.AccountDetailTypeId = item.AccountDetailTypeId;
        //            objDepts.Description = item.Description.ToString();
        //            objDepts.AccountType = item.AccountTypeMasters.AccountType;
        //            objDepts.AccountDetailType = item.AccountDetailTypeMasters.DetailType;
        //            objDepts.QBDetailType = item.AccountDetailTypeMasters.QBDetailType;
        //            SynthesisQBOnline.BAL.QBResponse objResponse = new SynthesisQBOnline.BAL.QBResponse();
        //            QBClass.QBSyncDepartment(item.DepartmentName, item.AccountType, item.AccountDetailType, item.Description, item.AccountNumber, item.IsSubAccount, StoreID, ref objResponse);
        //            if (objResponse.ID != "0" || objResponse.Status == "Done")
        //            {
        //                var vData1 = db.Database.SqlQuery<DepartmentMaster>("SP_DepartmentMaster @Mode,@StoreId,@DepartmentName", new SqlParameter("@Mode", "SelectByStoreID_Name"), new SqlParameter("@StoreId", StoreID), new SqlParameter("@DepartmentName", objDepts.DepartmentName)).ToList();
        //                if (vData1.Count > 0)
        //                {
        //                    var vData = db.Database.SqlQuery<DepartmentMaster>("SP_DepartmentMaster @Mode,@StoreId,@DepartmentName,@DepartmentId", new SqlParameter("@Mode", "SelectForUpdate"), new SqlParameter("@StoreId", StoreID), new SqlParameter("@DepartmentName", objDepts.DepartmentName), new SqlParameter("@DepartmentId", objDepts.DepartmentId)).ToList();
        //                    if (vData.Count > 0)
        //                    {
        //                        Message = "Department Name Already Exist";
        //                        TempData["Sucessme"] = Message;
        //                        return RedirectToAction("Create", "DepartmentMasters");
        //                    }

        //                    DepartmentMaster objDept = db.DepartmentMasters.Where(a => a.DepartmentId == vData.LastOrDefault().DepartmentId && vData.LastOrDefault().StoreId == StoreID).FirstOrDefault();
        //                    objDept.ListId = item.ListId;
        //                    objDept.DepartmentName = item.DepartmentName;
        //                    objDept.AccountTypeId = Convert.ToInt32(item.AccountType);

        //                    var DtAcc = db.Database.SqlQuery<AccountTypeMaster>("SP_AccountTypeMaster @Mode,@AccountType,@Flag", new SqlParameter("@Mode", "GetAccountType"), new SqlParameter("@AccountType", item.AccountType), new SqlParameter("@Flag", "O")).ToList();
        //                    if (DtAcc.Count > 0)
        //                    {
        //                        objDept.AccountTypeId = DtAcc.FirstOrDefault().AccountTypeId;
        //                    }
        //                    else
        //                    {
        //                        AccountTypeMaster objAcc = new AccountTypeMaster();
        //                        objAcc.AccountType = item.AccountType;
        //                        objAcc.Flag = "O";
        //                        objAcc.CommonType = item.AccountType;
        //                        db.AccountTypeMasters.Add(objAcc);
        //                        db.SaveChanges();
        //                        int id = objAcc.AccountTypeId;
        //                        objAcc = null;
        //                        objDept.AccountTypeId = id;
        //                    }

        //                    var DtAccDetail = db.Database.SqlQuery<AccountDetailTypeMaster>("SP_AccountDetailTypeMaster @Mode,@QBDetailType", new SqlParameter("@Mode", "GetQBDetailType"), new SqlParameter("@QBDetailType", item.QBDetailType)).ToList();
        //                    if (DtAccDetail.Count > 0)
        //                    {
        //                        objDept.AccountDetailTypeId = DtAccDetail.FirstOrDefault().AccountDetailTypeId;
        //                        if (objDept.AccountDetailTypeId == 0 || objDept.AccountDetailTypeId == null)
        //                        {
        //                            AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
        //                            objDetail.DetailType = item.AccountType;
        //                            objDetail.QBDetailType = item.QBDetailType;
        //                            objDetail.AccountTypeId = objDept.AccountTypeId;
        //                            db.AccountDetailTypeMasters.Add(objDetail);
        //                            db.SaveChanges();
        //                            objDetail = null;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
        //                        objDetail.DetailType = item.AccountType;
        //                        objDetail.QBDetailType = item.QBDetailType;
        //                        objDetail.AccountTypeId = objDept.AccountTypeId;
        //                        db.AccountDetailTypeMasters.Add(objDetail);
        //                        db.SaveChanges();
        //                        int id = objDetail.AccountDetailTypeId;
        //                        objDetail = null;
        //                        objDept.AccountDetailTypeId = id;
        //                    }

        //                    objDept.Description = item.Description;
        //                    objDept.AccountNumber = item.AccountNumber;
        //                    objDept.IsSubAccount = item.IsSubAccount;
        //                    objDept.IsActive = Convert.ToBoolean(item.IsActive);
        //                    objDept.StoreId = StoreID;
        //                    objDept.ModifiedOn = DateTime.Now;
        //                    objDept.ModifiedBy = UserModule.getUserId();
        //                    objDept.IsSync = 1;
        //                    objDept.SyncDate = DateTime.Now;
        //                    db.Entry(objDept).State = EntityState.Modified;
        //                    db.SaveChanges();
        //                    objDept = null;
        //                }
        //                else
        //                {
        //                    var vData = db.Database.SqlQuery<DepartmentMaster>("SP_DepartmentMaster @Mode,@StoreId,@DepartmentName,@DepartmentId", new SqlParameter("@Mode", "SelectForUpdate"), new SqlParameter("@StoreId", StoreID), new SqlParameter("@DepartmentName", objDepts.DepartmentName), new SqlParameter("@DepartmentId", objDepts.DepartmentId)).ToList();
        //                    if (vData.Count > 0)
        //                    {
        //                        Message = "Department Name Already Exist";
        //                        TempData["Sucessme"] = Message;
        //                        return RedirectToAction("Create", "DepartmentMasters");
        //                    }

        //                    DepartmentMaster objDept = new DepartmentMaster();
        //                    objDept.ListId = item.ListId;
        //                    objDept.DepartmentName = item.DepartmentName;
        //                    var DtAcc = db.Database.SqlQuery<AccountTypeMaster>("SP_AccountTypeMaster @Mode,@AccountType,@Flag", new SqlParameter("@Mode", "GetAccountType"), new SqlParameter("@AccountType", item.AccountType), new SqlParameter("@Flag", "O")).ToList();
        //                    if (DtAcc.Count > 0)
        //                    {
        //                        objDept.AccountTypeId = DtAcc.FirstOrDefault().AccountTypeId;
        //                    }
        //                    else
        //                    {
        //                        AccountTypeMaster objAcc = new AccountTypeMaster();
        //                        objAcc.AccountType = item.AccountType;
        //                        objAcc.Flag = "O";
        //                        objAcc.CommonType = item.AccountType;
        //                        db.AccountTypeMasters.Add(objAcc);
        //                        db.SaveChanges();
        //                        int id = objAcc.AccountTypeId;
        //                        objAcc = null;
        //                        objDept.AccountTypeId = id;
        //                    }

        //                    var DtAccDetail = db.Database.SqlQuery<AccountDetailTypeMaster>("SP_AccountDetailTypeMaster @Mode,@QBDetailType", new SqlParameter("@Mode", "GetQBDetailType"), new SqlParameter("@QBDetailType", item.QBDetailType)).ToList();
        //                    if (DtAccDetail.Count > 0)
        //                    {
        //                        objDept.AccountDetailTypeId = DtAccDetail.FirstOrDefault().AccountDetailTypeId;
        //                        if (objDept.AccountDetailTypeId == 0 || objDept.AccountDetailTypeId == null)
        //                        {
        //                            AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
        //                            objDetail.DetailType = item.AccountDetailType;
        //                            objDetail.QBDetailType = item.QBDetailType;
        //                            objDetail.AccountTypeId = objDept.AccountTypeId;
        //                            db.AccountDetailTypeMasters.Add(objDetail);
        //                            db.SaveChanges();
        //                            objDetail = null;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        AccountDetailTypeMaster objDetail = new AccountDetailTypeMaster();
        //                        objDetail.DetailType = item.AccountDetailType;
        //                        objDetail.QBDetailType = item.QBDetailType;
        //                        objDetail.AccountTypeId = objDept.AccountTypeId;
        //                        db.AccountDetailTypeMasters.Add(objDetail);
        //                        db.SaveChanges();
        //                        int id = objDetail.AccountDetailTypeId;
        //                        objDetail = null;
        //                        objDept.AccountDetailTypeId = id;
        //                    }
        //                    objDept.Description = item.Description;
        //                    objDept.AccountNumber = item.AccountNumber;
        //                    objDept.IsSubAccount = item.IsSubAccount;
        //                    objDept.IsActive = Convert.ToBoolean(item.IsActive);
        //                    objDept.StoreId = StoreID;
        //                    objDept.CreatedOn = DateTime.Now;
        //                    objDept.ModifiedOn = DateTime.Now;
        //                    objDept.CreatedBy = UserModule.getUserId();
        //                    objDept.ModifiedBy = UserModule.getUserId();
        //                    objDept.IsSync = 1;
        //                    objDept.SyncDate = DateTime.Now;
        //                    db.DepartmentMasters.Add(objDept);
        //                    db.SaveChanges();
        //                    objDept = null;
        //                }
        //            }
        //        }
        //        Dt2 = null;
        //        objDepts = null;

        //        var Dt3 = db.Database.SqlQuery<Invoice>("SP_Invoice @Mode,@StoreId", new SqlParameter("@Mode", "GetUnSyncInvoice"), new SqlParameter("@StoreId", StoreID)).ToList();
        //        Invoice objInvoice;
        //        foreach(var item in Dt3)
        //        {
        //            objInvoice = new Invoice();
        //            objInvoice.InvoiceId = item.InvoiceId;
        //            Invoice Invoice_data = db.Invoices.Find(item.InvoiceId);
        //            SynthesisQBOnline.BAL.QBResponse objResponse = new SynthesisQBOnline.BAL.QBResponse();
        //            if (!Invoice_data.InvoiceNumber.ToString().Contains("_cr"))
        //            {
        //                List<SynthesisQBOnline.BAL.BillDetail> objList = new List<SynthesisQBOnline.BAL.BillDetail>();
        //                var invDept = db.Database.SqlQuery<InvoiceDepartmentDetail>("SPInvoiceDepartment @Mode,@InvoiceId", new SqlParameter("@Mode", "GetUnSyncInvoice"), new SqlParameter("@InvoiceId", objInvoice.InvoiceId)).ToList();
        //                foreach(var itemm in invDept)
        //                {
        //                    SynthesisQBOnline.BAL.BillDetail objDetail = new SynthesisQBOnline.BAL.BillDetail();
        //                    objDetail.Description = "";
        //                    objDetail.Amount = itemm.Amount; ;
        //                    objDetail.DepartmentID = itemm.ListID;
        //                    objList.Add(objDetail);
        //                    objDetail = null;
        //                }
        //                if (QBClass.QBSyncBillData(Invoice_data.InvoiceId, Invoice_data.InvoiceNumber, Convert.ToDateTime(Invoice_data.InvoiceDate), Invoice_data.Note, Convert.ToInt32(Invoice_data.StoreId), Convert.ToInt32(Invoice_data.VendorId), objList, ref objResponse) == true)
        //                {
        //                    if (objResponse.ID != "")
        //                    {
        //                        Invoice_data.StatusValue = (int)Invoice_Status.Approved;
        //                        Invoice_data.ApproveRejectBy = UserModule.getUserId();
        //                        Invoice_data.ApproveRejectDate = DateTime.Now.AddHours(1);
        //                        Invoice_data.TXNId = objResponse.ID;
        //                        Invoice_data.IsSync = 1;
        //                        db.Entry(Invoice_data).State = System.Data.Entity.EntityState.Modified;
        //                        db.SaveChanges();

        //                        db.Database.ExecuteSqlCommand("SPInvoiceDepartment @Mode,@StoreId,@TxnID,@InvoiceId", new SqlParameter("@Mode", "SP_Invoice"), new SqlParameter("@StoreId", StoreID), new SqlParameter("@TxnID", objResponse.ID), new SqlParameter("@InvoiceId", objInvoice.InvoiceId));
        //                        //ActivityLogMessage = "Invoice Number " + "<a href='/Admin/InvoiceReport/Detail/" + Invoice_data.id + "'>" + Invoice_data.Invoice_Number + "</a> Approved by " + fullname + " on " + wwwroot.Class.AdminSiteConfiguration.GetDate(DateTime.Now.AddHours(1).ToString());
        //                        //var successActivity = db.tbl_Activity_log_Insert(userid, ActivityLogMessage, username, 4);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                List<SynthesisQBOnline.BAL.VendorCredirDetail> objList = new List<SynthesisQBOnline.BAL.VendorCredirDetail>();
        //                var invDept = db.Database.SqlQuery<InvoiceDepartmentDetail>("SPInvoiceDepartment @Mode,@InvoiceId", new SqlParameter("@Mode", "GetUnSyncInvoice"), new SqlParameter("@InvoiceId", objInvoice.InvoiceId)).ToList();
        //                foreach (var itemm in invDept)
        //                {
        //                    SynthesisQBOnline.BAL.VendorCredirDetail objDetail = new SynthesisQBOnline.BAL.VendorCredirDetail();
        //                    objDetail.Description = "";
        //                    objDetail.Amount = itemm.Amount; ;
        //                    objDetail.DepartmentID = itemm.ListID;
        //                    objList.Add(objDetail);
        //                    objDetail = null;
        //                }

        //                if (QBClass.QBSyncVendorCreditData(Invoice_data.InvoiceId, Invoice_data.InvoiceNumber, Convert.ToDateTime(Invoice_data.InvoiceDate), Invoice_data.Note, Convert.ToInt32(Invoice_data.StoreId), Convert.ToInt32(Invoice_data.VendorId), objList, ref objResponse) == true)
        //                {
        //                    if (objResponse.ID != "")
        //                    {
        //                        Invoice_data.StatusValue = (int)Invoice_Status.Approved;
        //                        Invoice_data.ApproveRejectBy = UserModule.getUserId();
        //                        Invoice_data.ApproveRejectDate = DateTime.Now.AddHours(1);
        //                        Invoice_data.TXNId = objResponse.ID;
        //                        Invoice_data.IsSync = 1;
        //                        db.Entry(Invoice_data).State = System.Data.Entity.EntityState.Modified;
        //                        db.SaveChanges();

        //                        db.Database.ExecuteSqlCommand("SPInvoiceDepartment @Mode,@StoreId,@TxnID,@InvoiceId", new SqlParameter("@Mode", "SP_Invoice"), new SqlParameter("@StoreId", StoreID), new SqlParameter("@TxnID", objResponse.ID), new SqlParameter("@InvoiceId", objInvoice.InvoiceId));
        //                        //ActivityLogMessage = "Invoice Number " + "<a href='/Admin/InvoiceReport/Detail/" + Invoice_data.id + "'>" + Invoice_data.Invoice_Number + "</a> Approved by " + fullname + " on " + wwwroot.Class.AdminSiteConfiguration.GetDate(DateTime.Now.AddHours(1).ToString());
        //                        //var successActivity = db.tbl_Activity_log_Insert(userid, ActivityLogMessage, username, 4);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return RedirectToAction("QBSyncOnlineData");
        //}

        /// <summary>
        /// This method is get QB Sync Store Id.
        /// </summary>
        /// <param name="StoreID"></param>
        /// <param name="Flag"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public ActionResult QBSyncStoreIdwise(int StoreID, int Flag, string Type)
        {
            //Save QB Onlin eConfigurations ById
            _qBRepository.SaveQBOnlineConfigurationsById(StoreID, Flag, Type);
            return RedirectToAction("QBSyncOnlineData");
        }

        /// <summary>
        /// this method is Check Sync.
        /// </summary>
        /// <returns></returns>
        public ActionResult CheckIsSync()
        {
            try
            {
                //Get list of QB Desktop Configurations using desktopid
                var Dt = _qBRepository.QBDesktopConfigurationsList().OrderByDescending(a => a.QBDesktopId).Take(1).FirstOrDefault();
                //Get list of QB Desktop Configurations using onlineid
                var Dt1 = _qBRepository.QBOnlineConfigurationsList().OrderByDescending(a => a.QBOnlineId).Take(1).FirstOrDefault();
                int data1 = 0;
                int data2 = 0;
                if (Dt != null)
                {
                    data1 = (Dt.Flag == 0 ? 0 : Convert.ToInt32(Dt.Flag));
                }
                if (Dt1 != null)
                {
                    data2 = (Dt1.Flag == 0 ? 0 : Convert.ToInt32(Dt1.Flag));
                }

                if (data1 == 0 && data2 == 0)
                {
                    Message = "success";
                }
                else if (Convert.ToInt32(data1) == 1 && data2 == 0)
                {
                    Message = "success";
                }
                else if (Convert.ToInt32(data1) == 0 && Convert.ToInt32(data2) == 0)
                {
                    Message = "NOTsuccess";
                }
                else if (data1 == 0 && Convert.ToInt32(data2) == 1)
                {
                    Message = "success";
                }
                else if (Convert.ToInt32(data1) == 1 && Convert.ToInt32(data2) == 1)
                {
                    Message = "success";
                }
                else
                {
                    Message = "NOTsuccess";
                }

            }
            catch (Exception ex)
            {
                logger.Error("QBConfigurationController - CheckIsSync - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           
            return Json(Message, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Update Qb Sync data
        /// </summary>
        /// <param name="Class"></param>
        /// <returns></returns>
        public ActionResult QBSyncUpdateData(string Class)
        {
            var ClassName = Class;
            try
            {
                if (Class == "red_cor")
                {
                    //Update QB Flag Data Online with class red_cor
                    _qBRepository.UpdateQBFlagDataOnline();
                    Message = "Notsuccess";
                }
                else
                {
                    //Update QB Flag Data Online 
                    _qBRepository.UpdateQBFlagDataOnline1();
                    Message = "success";
                }
                return Json(Message, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("QBConfigurationController - QBSyncUpdateData - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
          
        }

        /// <summary>
        /// This method is Get QB type and Flag
        /// </summary>
        /// <param name="StoreID"></param>
        /// <returns></returns>
        public ActionResult GetQBTypeAndFlag(int[] StoreID)
        {
            QBOnlineConfiguration1 CommonModel = new QBOnlineConfiguration1();
            CommonModel.DesktopTrue = "False";
            try
            {
                if (StoreID == null)
                {
                }
                else
                {
                    foreach (int Store in StoreID)
                    {
                        //Get QB Configuration By Store
                        var Dt = _qBRepository.GetQBConfigurationBystore(Store);
                        if (Dt != null)
                        {
                            string QBType = Dt.QBType.ToString();
                            if (QBType != "")
                            {
                                if (QBType == "Online")
                                {
                                    CommonModel.Flag = Dt.QBOnlineFlag.ToString();
                                    CommonModel.QBType = QBType;
                                    CommonModel.StoreId = Store;
                                    ViewBag.QBtypechek = QBType;
                                }
                                else if (QBType == "Desktop")
                                {
                                    CommonModel.Flag = Dt.QBWebFlag.ToString();
                                    CommonModel.QBType = QBType;
                                    CommonModel.StoreId = Store;
                                    ViewBag.QBtypechek = QBType;
                                    CommonModel.DesktopTrue = "True";
                                }
                            }
                            else
                            {
                                CommonModel.Flag = Dt.QBOnlineFlag.ToString();
                                CommonModel.QBType = QBType;
                                CommonModel.StoreId = Store;
                                ViewBag.QBtypechek = QBType;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("QBConfigurationController - GetQBTypeAndFlag - " + DateTime.Now + " - " + ex.Message.ToString());
            }
           
            return Json(CommonModel, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// this method is Get Account type
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public ActionResult GetAccountType(string Type)
        {
            try
            {
                string QBFlg = "O";
                if (Type != "Online")
                {
                    QBFlg = "D";
                }
                //this class is get Account Type using Flag
                var UserData = _qBRepository.GetAccountTypeFlag(QBFlg);
                //var UserData = db.Database.SqlQuery<AccountTypeMaster>("SP_AccountTypeMaster @Mode = {0}", "Select").ToList();
                return Json(UserData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("QBConfigurationController - GetAccountType - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
           
        }

        /// <summary>
        /// this method is Get details of Account type
        /// </summary>
        /// <param name="GetAccountType"></param>
        /// <returns></returns>
        public ActionResult GetDetailAccountType(string GetAccountType)
        {
            //Get ccount Details Type using Flag
            var UserData = _qBRepository.GetAccountDetailTypeFlag(GetAccountType);
            return Json(UserData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is Get Sub Account Type
        /// </summary>
        /// <param name="GetAccountType"></param>
        /// <param name="StoreID"></param>
        /// <returns></returns>
        public ActionResult IsSubAccounttype(string GetAccountType, int StoreID)
        {
            try
            {
                //Get Store Online desktop 
                string Store = _qBRepository.GetStoreOnlineDesktop(StoreID);
                string flg = "";
                if (Store == "Online")
                {
                    flg = "O";
                }
                else
                {
                    flg = "D";
                }

                //This class is Common Type Flag Storeid
                var dt = _qBRepository.CommonType_Flag_StoreId(GetAccountType, flg, StoreID);
                return Json(dt, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error("QBConfigurationController - IsSubAccounttype - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
          

        }
    }
}