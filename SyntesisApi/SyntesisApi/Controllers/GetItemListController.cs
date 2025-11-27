using iTextSharp.text.pdf;
using Newtonsoft.Json;
using NLog;
using SyntesisApi.BAL;
using SyntesisApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.Emit;
using System.Security.Claims;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Web.Services.Description;
using static SyntesisApi.Models.ApiModel;

namespace SyntesisApi.Controllers
{
    public class GetItemListController : ApiController
    {
        DataTable Dt1 = new DataTable();
        DataTable Dt2 = new DataTable();
        DataTable Dt3 = new DataTable();
        ResponseModel Response = new ResponseModel();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [HttpGet]
        [Route("api/GetItemList/GetItemListbyCode")]
        public async Task<IHttpActionResult> GetItemListbyCode(string ItemCode, int? UserId, int? ScannedStoreId, double? Longitude, double? Latitude, int? UserRightsforStoreAccess)
        {
            ItemListCode itemData = new ItemListCode();
            ResponseModelItem ItemResponse = new ResponseModelItem();
            DataTable Dt1 = new DataTable();

            if (UserId != 0)
            {
                int scannedStoreIdValue = ScannedStoreId ?? default(int);
                int userId = UserId ?? default(int);
                int userrightsstoreaccess = UserRightsforStoreAccess ?? default(int);
                double longitudeValue = Longitude ?? default(double);
                double latitudeValue = Latitude ?? default(double);

                Dt1 = new BALGetItemList().InsertItemScanLogbyUserId(ItemCode, userId, scannedStoreIdValue, longitudeValue, latitudeValue, userrightsstoreaccess);
            }
            try
            {
                logger.Info("GetItemListController - GetItemListbyCode - " + DateTime.Now.ToString());
                Dt1 = new BALGetItemList().GetItemList(ItemCode);

                if (Dt1.Rows.Count > 0)
                {
                    string baseUrl = ConfigurationManager.AppSettings["ProductImage"].ToString();

                    string firstItemCode = Dt1.Rows[0]["ItemCode"].ToString();
                    string firstItemName = Dt1.Rows[0]["ItemName"].ToString();
                    string firstDepartmentName = Dt1.Rows[0]["DepartmentName"].ToString();
                    string ImageUrls = Dt1.Rows[0]["ImageURLs"]?.ToString();
                    string expiryDateCount = Dt1.Rows[0]["ExpiryDateCount"].ToString();
                    string[] ImageUrlsArray = string.IsNullOrWhiteSpace(ImageUrls) ? Array.Empty<string>() : ImageUrls.Split(',').Select(url => baseUrl.TrimEnd('/') + "/" + url.Trim()).Where(url => !string.IsNullOrWhiteSpace(url)).ToArray();
                    itemData.ItemCode = firstItemCode;
                    itemData.ItemName = firstItemName;
                    itemData.DepartmentName = firstDepartmentName;
                    itemData.ImageURLs = ImageUrlsArray.Length > 0 ? ImageUrlsArray : Array.Empty<string>();
                    itemData.ExpiryDateCount = expiryDateCount;
                    itemData.ItemDetails = Dt1;
                    ItemResponse.responseStatus = "200";
                    ItemResponse.responseData = itemData;
                    ItemResponse.message = "Successfully!!";
                    logger.Info("GetItemListController - GetItemListbyCode - " + DateTime.Now.ToString() + " - " + "Successfully!!");
                }
                else
                {
                    ItemResponse.responseStatus = "400";
                    ItemResponse.responseData = null;
                    ItemResponse.message = "Item Not Found!";
                    logger.Error("GetItemListController - GetItemListbyCode - " + DateTime.Now.ToString() + " - " + "Something Went Wrong!");
                }

                return Ok(ItemResponse);
            }
            catch (Exception ex)
            {
                ItemResponse.responseStatus = "400";
                ItemResponse.responseData = null;
                ItemResponse.message = ex.Message;
                logger.Error("GetItemListController - GetItemListbyCode - " + DateTime.Now.ToString() + " - " + ex.Message);

                return Ok(ItemResponse);
            }
        }

        [HttpGet]
        [Route("api/GetItemList/GetItemListScanbyCode")]
        public async Task<IHttpActionResult> GetItemListScanbyCode(string ItemCode)
        {
            ItemListCode itemData = new ItemListCode();
            ResponseModelItem ItemResponse = new ResponseModelItem();
            DataTable Dt1 = new DataTable();
            try
            {
                logger.Info("GetItemListController - GetItemListScanbyCode - " + DateTime.Now.ToString());
                Dt1 = new BALGetItemList().GetItemList(ItemCode);

                if (Dt1.Rows.Count > 0)
                {
                    string baseUrl = ConfigurationManager.AppSettings["ProductImage"].ToString();

                    string firstItemCode = Dt1.Rows[0]["ItemCode"].ToString();
                    string firstItemName = Dt1.Rows[0]["ItemName"].ToString();
                    string firstDepartmentName = Dt1.Rows[0]["DepartmentName"].ToString();
                    string ImageUrls = Dt1.Rows[0]["ImageURLs"]?.ToString();
                    string expiryDateCount = Dt1.Rows[0]["ExpiryDateCount"].ToString();
                    string[] ImageUrlsArray = string.IsNullOrWhiteSpace(ImageUrls) ? Array.Empty<string>() : ImageUrls.Split(',').Select(url => baseUrl.TrimEnd('/') + "/" + url.Trim()).Where(url => !string.IsNullOrWhiteSpace(url)).ToArray();
                    itemData.ItemCode = firstItemCode;
                    itemData.ItemName = firstItemName;
                    itemData.DepartmentName = firstDepartmentName;
                    itemData.ImageURLs = ImageUrlsArray.Length > 0 ? ImageUrlsArray : Array.Empty<string>();
                    itemData.ExpiryDateCount = expiryDateCount;
                    itemData.ItemDetails = Dt1;
                    ItemResponse.responseStatus = "200";
                    ItemResponse.responseData = itemData;
                    ItemResponse.message = "Successfully!!";
                    logger.Info("GetItemListController - GetItemListScanbyCode - " + DateTime.Now.ToString() + " - " + "Successfully!!");
                }
                else
                {
                    ItemResponse.responseStatus = "400";
                    ItemResponse.responseData = null;
                    ItemResponse.message = "Item Not Found!";
                    logger.Error("GetItemListController - GetItemListScanbyCode - " + DateTime.Now.ToString() + " - " + "Something Went Wrong!");
                }

                return Ok(ItemResponse);
            }
            catch (Exception ex)
            {
                ItemResponse.responseStatus = "400";
                ItemResponse.responseData = null;
                ItemResponse.message = ex.Message;
                logger.Error("GetItemListController - GetItemListScanbyCode - " + DateTime.Now.ToString() + " - " + ex.Message);

                return Ok(ItemResponse);
            }
        }

        [HttpGet]
        [Route("api/GetItemList/ProductItemLogScanDBDump")]
        public async Task<IHttpActionResult> ProductItemLogScanDBDump(string ItemCode, int? UserId, int? ScannedStoreId, double? Longitude, double? Latitude, int? UserRightsforStoreAccess, string DeviceName, string OSVersion, bool LocationAccess)
        {
            ItemListCode itemData = new ItemListCode();
            ResponseModelItem ItemResponse = new ResponseModelItem();
            DataTable Dt1 = new DataTable();

            if (UserId != 0)
            {
                int scannedStoreIdValue = ScannedStoreId ?? default(int);
                int userId = UserId ?? default(int);
                int userrightsstoreaccess = UserRightsforStoreAccess ?? default(int);
                double longitudeValue = Longitude ?? default(double);
                double latitudeValue = Latitude ?? default(double);
                string deviceNameValue = DeviceName ?? string.Empty;
                string osVersionValue = OSVersion ?? string.Empty;

                Dt1 = new BALGetItemList().InsertItemScanLogBasedbyUserId(ItemCode, userId, scannedStoreIdValue, longitudeValue, latitudeValue, userrightsstoreaccess, deviceNameValue, osVersionValue, LocationAccess);
            }
            try
            {
                logger.Info("GetItemListController - ProductItemLogScanDBDump - " + DateTime.Now.ToString());
                Dt1 = new BALGetItemList().GetItemList(ItemCode);

                if (Dt1.Rows.Count > 0)
                {
                    string baseUrl = ConfigurationManager.AppSettings["ProductImage"].ToString();

                    string firstItemCode = Dt1.Rows[0]["ItemCode"].ToString();
                    string firstItemName = Dt1.Rows[0]["ItemName"].ToString();
                    string firstDepartmentName = Dt1.Rows[0]["DepartmentName"].ToString();
                    string ImageUrls = Dt1.Rows[0]["ImageURLs"]?.ToString();
                    string[] ImageUrlsArray = string.IsNullOrWhiteSpace(ImageUrls) ? Array.Empty<string>() : ImageUrls.Split(',').Select(url => baseUrl.TrimEnd('/') + "/" + url.Trim()).Where(url => !string.IsNullOrWhiteSpace(url)).ToArray();
                    string expiryDateCount = Dt1.Rows[0]["ExpiryDateCount"].ToString();
                    itemData.ItemCode = firstItemCode;
                    itemData.ItemName = firstItemName;
                    itemData.DepartmentName = firstDepartmentName;
                    itemData.ImageURLs = ImageUrlsArray.Length > 0 ? ImageUrlsArray : Array.Empty<string>();
                    itemData.ExpiryDateCount = expiryDateCount;
                    itemData.ItemDetails = Dt1;
                    ItemResponse.responseStatus = "200";
                    ItemResponse.responseData = itemData;
                    ItemResponse.message = "Successfully!!";
                    logger.Info("GetItemListController - ProductItemLogScanDBDump - " + DateTime.Now.ToString() + " - " + "Successfully!!");
                }
                else
                {
                    ItemResponse.responseStatus = "400";
                    ItemResponse.responseData = null;
                    ItemResponse.message = "Item Not Found!";
                    logger.Error("GetItemListController - ProductItemLogScanDBDump - " + DateTime.Now.ToString() + " - " + "Something Went Wrong!");
                }

                return Ok(ItemResponse);
            }
            catch (Exception ex)
            {
                ItemResponse.responseStatus = "400";
                ItemResponse.responseData = null;
                ItemResponse.message = ex.Message;
                logger.Error("GetItemListController - ProductItemLogScanDBDump - " + DateTime.Now.ToString() + " - " + ex.Message);

                return Ok(ItemResponse);
            }
        }

        [HttpGet]
        [Route("api/GetItemList/GetAllItemList")]
        public async Task<IHttpActionResult> GetAllItemList(int StoreId)
        {
            Dt1 = new DataTable();
            try
            {
                logger.Info("GetItemListController - GetAllItemList - " + DateTime.Now.ToString());
                Dt1 = new BALGetItemList().GetAllItemList(StoreId);
                if (Dt1.Rows.Count > 0)
                {
                    Response.responseStatus = "200";
                    Response.responseData = Dt1;
                    Response.message = "Successfully!!";
                    logger.Info("GetItemListController - GetAllItemList - " + DateTime.Now.ToString() + " - " + "Successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Someting Went Wrong!";
                    logger.Error("GetItemListController - GetAllItemList - " + DateTime.Now.ToString() + " - " + "Someting Went Wrong!");
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("GetItemListController - GetAllItemList - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        [HttpGet]
        [Route("api/GetItemList/GetTop50ItemsList")]
        public async Task<IHttpActionResult> GetTop50ItemsList(int StoreId, string DepartmentIds)
        {
            Dt1 = new DataTable();
            Dt2 = new DataTable();
            Dt3 = new DataTable();
            var response = new TopItemsResponse();
            try
            {
                logger.Info("GetItemListController - GetTop50ItemsList - " + DateTime.Now.ToString());
                Dt1 = new BALGetItemList().GetYesterdayTopItemList(StoreId, DepartmentIds);
                Dt2 = new BALGetItemList().GetLastWeekTopItemList(StoreId, DepartmentIds);
                Dt3 = new BALGetItemList().GetLastMonthTopItemList(StoreId, DepartmentIds);

                response.YesterdayData = Dt1.AsEnumerable().Select(row => new ItemData
                {
                    ItemName = row["ItemName"].ToString(),
                    ItemCode = row["ItemCode"].ToString(),
                    DepartmentName = row["DepartmentName"].ToString(),
                    StoreID = Convert.ToInt32(row["StoreID"]),
                    QtySold = Convert.ToDecimal(row["QtySold"])
                }).ToList();

                response.LastWeekData = Dt2.AsEnumerable().Select(row => new ItemData
                {
                    ItemName = row["ItemName"].ToString(),
                    ItemCode = row["ItemCode"].ToString(),
                    DepartmentName = row["DepartmentName"].ToString(),
                    StoreID = Convert.ToInt32(row["StoreID"]),
                    QtySold = Convert.ToDecimal(row["QtySold"])
                }).ToList();

                response.LastMonthData = Dt3.AsEnumerable().Select(row => new ItemData
                {
                    ItemName = row["ItemName"].ToString(),
                    ItemCode = row["ItemCode"].ToString(),
                    DepartmentName = row["DepartmentName"].ToString(),
                    StoreID = Convert.ToInt32(row["StoreID"]),
                    QtySold = Convert.ToDecimal(row["QtySold"])
                }).ToList();

                response.responseStatus = "200";
                response.message = "Successfully!!";
                logger.Info("GetItemListController - GetTop50ItemsList - " + DateTime.Now.ToString() + " - " + "Successfully!!");

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.responseStatus = "400";
                response.message = ex.Message;
                logger.Error("GetItemListController - GetTop50ItemsList - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(response);
            }
        }

        [HttpGet]
        [Route("api/GetItemList/GetLowest50ItemsList")]
        public async Task<IHttpActionResult> GetLowest50ItemsList(int StoreId, string DepartmentIds)
        {
            Dt1 = new DataTable();
            Dt2 = new DataTable();
            Dt3 = new DataTable();
            var response = new TopItemsResponse();
            try
            {
                logger.Info("GetItemListController - GetLowest50ItemsList - " + DateTime.Now.ToString());
                Dt1 = new BALGetItemList().GetYesterdayLowestItemList(StoreId, DepartmentIds);
                Dt2 = new BALGetItemList().GetLastWeekLowestItemList(StoreId, DepartmentIds);
                Dt3 = new BALGetItemList().GetLastMonthLowestItemList(StoreId, DepartmentIds);

                response.YesterdayData = Dt1.AsEnumerable().Select(row => new ItemData
                {
                    ItemName = row["ItemName"].ToString(),
                    ItemCode = row["ItemCode"].ToString(),
                    DepartmentName = row["DepartmentName"].ToString(),
                    StoreID = Convert.ToInt32(row["StoreID"]),
                    QtySold = Convert.ToDecimal(row["QtySold"])
                }).ToList();

                response.LastWeekData = Dt2.AsEnumerable().Select(row => new ItemData
                {
                    ItemName = row["ItemName"].ToString(),
                    ItemCode = row["ItemCode"].ToString(),
                    DepartmentName = row["DepartmentName"].ToString(),
                    StoreID = Convert.ToInt32(row["StoreID"]),
                    QtySold = Convert.ToDecimal(row["QtySold"])
                }).ToList();

                response.LastMonthData = Dt3.AsEnumerable().Select(row => new ItemData
                {
                    ItemName = row["ItemName"].ToString(),
                    ItemCode = row["ItemCode"].ToString(),
                    DepartmentName = row["DepartmentName"].ToString(),
                    StoreID = Convert.ToInt32(row["StoreID"]),
                    QtySold = Convert.ToDecimal(row["QtySold"])
                }).ToList();

                response.responseStatus = "200";
                response.message = "Successfully!!";
                logger.Info("GetItemListController - GetLowest50ItemsList - " + DateTime.Now.ToString() + " - " + "Successfully!!");

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.responseStatus = "400";
                response.message = ex.Message;
                logger.Error("GetItemListController - GetLowest50ItemsList - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(response);
            }
        }

        [HttpGet]
        [Route("api/GetItemList/GetOffLineItemList")]
        public async Task<IHttpActionResult> GetOfflineItemList(DateTime StartDate)
        {
            Dt1 = new DataTable();
            var response = new GetListData();
            try
            {
                logger.Info("GetItemListController - GetOffLineItemList - " + DateTime.Now.ToString());
                Dt1 = await new BALGetItemList().GetOfflineItemList(StartDate);
                response.getOfflineItemLists = Dt1.AsEnumerable().Select(row => new GetOfflineItemList
                {
                    ItemMovementId = Convert.ToInt32(row["ItemMovementId"]),
                    ItemCode = row["ItemCode"].ToString(),
                    ItemName = row["ItemName"].ToString(),
                    QtySold = Convert.ToDecimal(row["QtySold"]),
                    BasePrice = Convert.ToDecimal(row["BasePrice"]),
                    ItemMovementHistoryID = Convert.ToInt32(row["ItemMovementHistoryID"]),
                    ItemMovementdatehistoryID = Convert.ToInt32(row["ItemMovementdatehistoryID"]),
                    StoreID = Convert.ToInt32(row["StoreID"]),
                    Startdate = Convert.ToDateTime(row["Startdate"]),
                    Enddate = Convert.ToDateTime(row["Enddate"])
                }).ToList();
                response.responseStatus = "200";
                response.message = "Successfully!!";
                logger.Info("GetItemListController - GetOffLineItemList - " + DateTime.Now.ToString() + " - " + "Successfully!!");

                return Ok(response.getOfflineItemLists);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("GetItemListController - GetOffLineItemList - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        [HttpPost]
        [Route("api/GetItemList/ApkWatchList")]
        public IHttpActionResult ApkWatchList([FromBody] ApkWatchList apkWatchList)
        {
            try
            {
                logger.Info("GetItemListController - ApkWatchList - " + DateTime.Now.ToString());
                bool Umst = false;
                //(CheckWatchList) to check watch list already exist or not
                Umst = new BALGetItemList().CheckWatchList(apkWatchList.StoreId, apkWatchList.UPCCode);
                if (Umst)
                {
                    Response.responseStatus = "200";
                    Response.message = "Item already added to watchList!";
                    return Ok(Response);
                }
                if (apkWatchList != null)
                {
                    new BALGetItemList().ApkWatchList(apkWatchList);

                    Response.responseStatus = "200";
                    Response.message = "Item added to watchList successfully!!";
                    logger.Info("GetItemListController - ApkWatchList - " + DateTime.Now.ToString() + " - " + "Item added to watchList successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Someting Went Wrong!";
                    logger.Error("GetItemListController - ApkWatchList - " + DateTime.Now.ToString() + " - " + "Someting Went Wrong!");

                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("GetItemListController - ApkWatchList - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        [HttpGet]
        [Route("api/GetItemList/GetAllWatchList")]
        public async Task<IHttpActionResult> GetAllWatchList(int StoreId)
        {
            Dt1 = new DataTable();
            try
            {
                logger.Info("GetItemListController - GetAllWatchList - " + DateTime.Now.ToString());
                Dt1 = new BALGetItemList().GetAllWatchList(StoreId);
                if (Dt1.Rows.Count > 0)
                {
                    Response.responseStatus = "200";
                    Response.responseData = Dt1;
                    Response.message = "Successfully!!";
                    logger.Info("GetItemListController - GetAllWatchList - " + DateTime.Now.ToString() + " - " + "Successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "No Watchlist Available for this Store!";
                    logger.Error("GetItemListController - GetAllWatchList - " + DateTime.Now.ToString() + " - " + "Someting Went Wrong!");
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("GetItemListController - GetAllWatchList - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        [HttpPost]
        [Route("api/GetItemList/DeleteApkWatchList")]
        public async Task<IHttpActionResult> DeleteApkWatchList(int StoreId, string UPCCode)
        {

            try
            {
                logger.Info("GetItemListController - DeleteApkWatchList - " + DateTime.Now.ToString());
                bool IsDelete = new BALGetItemList().DeleteApkWatchList(StoreId, UPCCode);
                if (IsDelete == true)
                {
                    Response.responseStatus = "200";
                    //Response.responseData = Dt1;
                    Response.message = "Item removed from watchList successfully!!";
                    logger.Info("GetItemListController - DeleteApkWatchList - " + DateTime.Now.ToString() + " - " + "Item removed from watchList successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Someting Went Wrong!";
                    logger.Error("GetItemListController - DeleteApkWatchList - " + DateTime.Now.ToString() + " - " + "Someting Went Wrong!");
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("GetItemListController - DeleteApkWatchList - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        [HttpGet]
        [Route("api/GetItemList/GetItembySearch")]
        public async Task<IHttpActionResult> GetItembySearch(string Search)
        {
            DataTable Dt1 = new DataTable();

            //Added Array for dummyImageUrls
            List<string> dummyImageUrls = new List<string>
            {
                Url.Content("~/Images/Products.png"),
            };

            try
            {
                logger.Info("GetItemListController - GetItembySearch - " + DateTime.Now.ToString());
                Dt1 = new BALGetItemList().GetItemBasedonSearching(Search);

                if (Dt1.Rows.Count > 0)
                {
                    Response.responseStatus = "200";
                    Response.responseData = Dt1;
                    Response.message = "Successfully!!";
                    logger.Info("GetItemListController - GetItembySearch - " + DateTime.Now.ToString() + " - " + "Successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "No search result is available for it.";
                    logger.Error("GetItemListController - GetItembySearch - " + DateTime.Now.ToString() + " - " + "Something Went Wrong!");
                }

                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("GetItemListController - GetItembySearch - " + DateTime.Now.ToString() + " - " + ex.Message);

                return Ok(Response);
            }
        }

        [HttpGet]
        [Route("api/GetItemList/GetAllItemMovementDepartment")]
        public async Task<IHttpActionResult> GetAllItemMovementDepartment()
        {
            Dt1 = new DataTable();
            try
            {
                logger.Info("GetItemListController - GetAllItemMovementDepartment - " + DateTime.Now.ToString());
                Dt1 = new BALGetItemList().GetAllItemMovementDepartment();
                if (Dt1.Rows.Count > 0)
                {
                    Response.responseStatus = "200";
                    Response.responseData = Dt1;
                    Response.message = "Successfully!!";
                    logger.Info("GetItemListController - GetAllItemMovementDepartment - " + DateTime.Now.ToString() + " - " + "Successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Someting Went Wrong!";
                    logger.Error("GetItemListController - GetAllItemMovementDepartment - " + DateTime.Now.ToString() + " - " + "Someting Went Wrong!");
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("GetItemListController - GetAllItemMovementDepartment - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        //[HttpGet]
        //[Route("api/GetItemList/GetForceUpdate")]
        //public async Task<IHttpActionResult> GetForceUpdate(string VersionNumber, string Type)
        //{
        //    try
        //    {
        //        logger.Info("GetItemListController - GetForceUpdate - " + DateTime.Now.ToString());
        //        var dtVersion = new BALGetItemList().GetVersionNumber(Type);

        //        if (Type == "android")
        //        {
        //            if (dtVersion != null && dtVersion.Rows.Count > 0)
        //            {
        //                string dbVersion = dtVersion.Rows[0]["VersionNumber"].ToString();
        //                string ForwardURL = dtVersion.Rows[0]["ForwardURL"].ToString();
        //                if (string.Compare(VersionNumber, dbVersion, StringComparison.Ordinal) < 0)
        //                {
        //                    return Ok(new
        //                    {
        //                        Type = Type,
        //                        ForceUpdate = 1,
        //                        MessageTitle = "APP UPDATES REQUIRED",
        //                        Message = "A new version of the app is now available! Please update your app to continue using it.",
        //                        Url = ForwardURL
        //                    });
        //                }
        //                return Ok(new
        //                {
        //                    Message = "Your app is up to date."
        //                });
        //            }
        //        }
        //        else
        //        {
        //            if (dtVersion != null && dtVersion.Rows.Count > 0)
        //            {
        //                string dbVersion = dtVersion.Rows[0]["VersionNumber"].ToString();
        //                string ForwardURL = dtVersion.Rows[0]["ForwardURL"].ToString();
        //                if (string.Compare(VersionNumber, dbVersion, StringComparison.Ordinal) < 0)
        //                {
        //                    return Ok(new
        //                    {
        //                        Type = Type,
        //                        ForceUpdate = 1,
        //                        MessageTitle = "APP UPDATES REQUIRED",
        //                        Message = "NEW VERSION OF THIS AVAILABLE. KINDLY UPDATE TO THE LATEST VERSION TO CONTINUE USING YOUR APP.",
        //                        Url = ForwardURL
        //                    });
        //                }
        //                return Ok(new
        //                {
        //                    Message = "Your app is up to date."
        //                });
        //            }
        //        }

        //        return BadRequest("Version number not found.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.responseStatus = "400";
        //        Response.responseData = null;
        //        Response.message = ex.Message;
        //        logger.Error("GetItemListController - GetForceUpdate - " + DateTime.Now.ToString() + " - " + ex.Message);
        //        return Ok(Response);
        //    }
        //}

        [HttpGet]
        [Route("api/GetItemList/GetAllUserDetails")]
        public async Task<IHttpActionResult> GetAllUserDetails(int UserId)
        {
            ResponseTokenModelforUserDetail Response = new ResponseTokenModelforUserDetail();
            try
            {
                logger.Info("GetItemListController - GetAllUserDetails - " + DateTime.Now.ToString());
                UserDetail user = new BALHRLogin().GetALLUserDetails(UserId);
                if (user != null)
                {
                    user.ModuleAccess = new ModuleAccess();
                    Response.responseStatus = "200";
                    Response.responseData = user;
                    Response.message = "User Details got Successfully!!";
                    logger.Info("GetItemListController - GetAllUserDetails - " + DateTime.Now.ToString() + " - " + "Successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Error in getting User Details!";
                    logger.Info("GetItemListController - GetAllUserDetail - " + DateTime.Now.ToString() + " - " + "Someting Went Wrong!");
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("GetItemListController - GetAllUserDetails - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        [HttpGet]
        [Route("api/GetItemList/GetSentryURL")]
        public async Task<IHttpActionResult> GetSentryURL()
        {
            DataTable Dt1 = new DataTable();
            try
            {
                logger.Info("GetItemListController - GetSentryURL - " + DateTime.Now.ToString());
                Dt1 = new BALGetItemList().GetSentryURL();
                string url = "";
                if (Dt1.Rows.Count > 0)
                {
                    url = Dt1.Rows[0]["SentryURLName"].ToString();
                    Response.responseStatus = "200";
                    Response.message = "Sentry URL got Successfully!!";
                    logger.Info("GetItemListController - GetSentryURL - " + DateTime.Now.ToString() + " - " + "Successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Sentry URL Not Found!";
                    logger.Error("GetItemListController - GetSentryURL - " + DateTime.Now.ToString() + " - " + "Something Went Wrong!");
                }
                return Ok(url);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("GetItemListController - GetSentryURL - " + DateTime.Now.ToString() + " - " + ex.Message);

                return Ok(Response);
            }
        }

        //Changes Done by Dani on 27-03-2025
        [HttpGet]
        [Route("api/GetItemList/GetItembyKeywordSearch")]
        public async Task<IHttpActionResult> GetItembyKeywordSearch(string Search, string Departments)
        {
            DataTable Dt1 = new DataTable();

            try
            {
                logger.Info("GetItemListController - GetItembyKeywordSearch - " + DateTime.Now.ToString());
                Dt1 = new BALGetItemList().GetItemBasedonKeywordSearching(Search, Departments);

                if (Dt1.Rows.Count > 0)
                {
                    Response.responseStatus = "200";
                    Response.responseData = Dt1;
                    Response.message = "Successfully!!";
                    logger.Info("GetItemListController - GetItembyKeywordSearch - " + DateTime.Now.ToString() + " - " + "Successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "No search result is available for it.";
                    logger.Error("GetItemListController - GetItembyKeywordSearch - " + DateTime.Now.ToString() + " - " + "Something Went Wrong!");
                }

                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("GetItemListController - GetItembySearch - " + DateTime.Now.ToString() + " - " + ex.Message);

                return Ok(Response);
            }
        }

        //Created Code by Dani on 12-03-2025
        [HttpGet]
        [Route("api/GetItemList/GetAllOptionsToReportIssue")]
        public async Task<IHttpActionResult> GetAllOptionsToReportIssue()
        {
            DataTable Dt1 = new DataTable();
            try
            {
                logger.Info("GetItemListController - GetAllOptionsToReportIssue - " + DateTime.Now.ToString());
                Dt1 = new BALGetItemList().GetALLReportIssueOptions();
                if (Dt1.Rows.Count > 0)
                {
                    Response.responseStatus = "200";
                    Response.responseData = Dt1;
                    Response.message = "Report Issue Options got Successfully!!";
                    logger.Info("GetItemListController - GetAllOptionsToReportIssue - " + DateTime.Now.ToString() + " - " + "Successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Error in getting Report Issue Options!";
                    logger.Info("GetItemListController - GetAllOptionsToReportIssue - " + DateTime.Now.ToString() + " - " + "Someting Went Wrong!");
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("GetItemListController - GetAllOptionsToReportIssue - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        //Created Code by Dani on 12-03-2025
        [HttpPost]
        [Route("api/GetItemList/ReportIssueDump")]
        public IHttpActionResult ReportIssueDump([FromBody] ReportIssueDump reportIssueDump)
        {
            try
            {
                logger.Info("GetItemListController - ReportIssueDump - " + DateTime.Now.ToString());
                if (reportIssueDump != null)
                {
                    new BALGetItemList().ReportIssueDump(reportIssueDump);

                    Response.responseStatus = "200";
                    Response.message = "Issue Reported Successfully!!";
                    logger.Info("GetItemListController - ReportIssueDump - " + DateTime.Now.ToString() + " - " + "Item added to watchList successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Someting Went Wrong!";
                    logger.Error("GetItemListController - ReportIssueDump - " + DateTime.Now.ToString() + " - " + "Someting Went Wrong!");

                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("GetItemListController - ReportIssueDump - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        //Created Code by Dani on 22-03-2025
        //[HttpPost]
        //[Route("api/GetItemList/InsertorUpdateProductImage")]
        //public IHttpActionResult InsertorUpdateProductImage([FromBody] ProductImageRequest request)
        //{
        //    try
        //    {
        //        logger.Info("GetItemListController - InsertorUpdateProductImage - " + DateTime.Now.ToString());
        //        if (!string.IsNullOrEmpty(request.ItemCode))
        //        {
        //            if (request.ImageURLs != null && request.ImageURLs.Length > 0)
        //            {
        //                int id = new BALGetItemList().ProductImageUpdate(request);

        //                if (id > 0)
        //                {
        //                    Response.responseStatus = "200";
        //                    Response.message = "Product Image Updated Successfully!!";
        //                    logger.Info("GetItemListController - InsertorUpdateProductImage - " + DateTime.Now.ToString() + " - " + "Product Image Updated Successfully!!");

        //                }
        //                else
        //                {
        //                    Response.responseStatus = "400";
        //                    Response.message = "No product found or update failed!";
        //                    logger.Warn("GetItemListController - InsertorUpdateProductImage - " + DateTime.Now.ToString() + " - " + "No product found or update failed!");
        //                }
        //            }
        //        }
        //        else
        //        {
        //            Response.responseStatus = "400";
        //            Response.responseData = null;
        //            Response.message = "ItemCode cannot be null or empty!";
        //            logger.Error("GetItemListController - InsertorUpdateProductImage - " + DateTime.Now.ToString() + " - " + "ItemCode cannot be null or empty!");
        //        }
        //        return Ok(Response);
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.responseStatus = "400";
        //        Response.responseData = null;
        //        Response.message = ex.Message;
        //        logger.Error("GetItemListController - InsertorUpdateProductImage - " + DateTime.Now.ToString() + " - " + ex.Message);
        //        return Ok(Response);
        //    }
        //}

        // Created by Himanshu on 02-04-2025
        [HttpPost]
        [Route("api/GetItemList/InsertorUpdateProductImage")]
        public IHttpActionResult InsertorUpdateProductImage()
        {
            try
            {
                logger.Info("GetItemListController - InsertorUpdateProductImage - " + DateTime.Now.ToString());
                var httpRequest = HttpContext.Current.Request;

                if (httpRequest.Files.Count == 0 || string.IsNullOrEmpty(httpRequest.Form["ItemCode"]))
                {
                    return Ok(new { responseStatus = "400", message = "ItemCode and Images are required!" });
                }

                string itemCode = httpRequest.Form["ItemCode"];
                HttpFileCollection files = httpRequest.Files;

                int id = new BALGetItemList().ProductImageUpdate(itemCode, files);

                if (id > 0)
                {
                    return Ok(new { responseStatus = "200", message = "Product Image Updated Successfully!!" });
                }
                else
                {
                    return Ok(new { responseStatus = "400", message = "No product found or update failed!" });
                }
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("GetItemListController - InsertorUpdateProductImage - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        //Created Code by Himanshu on 25-03-2025
        [HttpPost]
        [Route("api/GetItemList/DietaryAttributeDump")]
        public IHttpActionResult DietaryAttributeDump([FromBody] DietaryAttributeSearchKeywords obj)
        {
            try
            {
                logger.Info("GetItemListController - DietaryAttributeDump - " + DateTime.Now.ToString());
                if (obj != null)
                {
                    new BALGetItemList().DietaryAttributeDump(obj);

                    Response.responseStatus = "200";
                    Response.message = "Dietary Attribute Inserted Successfully!!";
                    logger.Info("GetItemListController - DietaryAttributeDump - " + DateTime.Now.ToString() + " - " + "Item added to watchList successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Someting Went Wrong!";
                    logger.Error("GetItemListController - DietaryAttributeDump - " + DateTime.Now.ToString() + " - " + "Someting Went Wrong!");

                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("GetItemListController - DietaryAttributeDump - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        //Created Code by Himanshu on 25-03-2025
        [HttpGet]
        [Route("api/GetItemList/GetAllDietaryAttributeSearchKeywords")]
        public async Task<IHttpActionResult> GetAllDietaryAttributeSearchKeywords()
        {
            DataTable Dt1 = new DataTable();
            try
            {
                logger.Info("GetItemListController - GetAllDietaryAttributeSearchKeywords - " + DateTime.Now.ToString());
                Dt1 = new BALGetItemList().GetALLDietaryAttributeSearchKeywords();
                if (Dt1.Rows.Count > 0)
                {
                    Response.responseStatus = "200";
                    Response.responseData = Dt1;
                    Response.message = "Dieatary Attributes data got Successfully!!";
                    logger.Info("GetItemListController - GetAllDietaryAttributeSearchKeywords - " + DateTime.Now.ToString() + " - " + "Successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Error in getting Report Issue Options!";
                    logger.Info("GetItemListController - GetAllDietaryAttributeSearchKeywords - " + DateTime.Now.ToString() + " - " + "Someting Went Wrong!");
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("GetItemListController - GetAllDietaryAttributeSearchKeywords - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        //Created Code by Dani on 27-03-2025
        [HttpPost]
        [Route("api/GetItemList/SearchedKeywordDump")]
        public IHttpActionResult SearchedKeywordDump([FromBody] SearchbyKeyword searchbyKeyword)
        {
            try
            {
                logger.Info("GetItemListController - SearchedKeywordDump - " + DateTime.Now.ToString());
                if (searchbyKeyword != null)
                {
                    new BALGetItemList().InsertSearchedItem(searchbyKeyword);

                    Response.responseStatus = "200";
                    Response.message = "Searched Keyword Data Inserted Successfully!!";
                    logger.Info("GetItemListController - SearchedKeywordDump - " + DateTime.Now.ToString() + " - " + "Searched Keyword Data Inserted Successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Someting Went Wrong!";
                    logger.Error("GetItemListController - SearchedKeywordDump - " + DateTime.Now.ToString() + " - " + "Someting Went Wrong!");
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("GetItemListController - SearchedKeywordDump - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        //Created Code by Dani on 01-04-2025
        [HttpGet]
        [Route("api/GetItemList/GetALLSearchKeywordbyUserId")]
        public async Task<IHttpActionResult> GetALLSearchKeywordbyUserId(int UserId)
        {
            DataTable dt1 = new DataTable();
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                logger.Info("GetItemListController - GetALLSearchKeywordbyUserId - " + DateTime.Now.ToString());
                stopwatch.Start();
                dt1 = new BALGetItemList().GetAllSearchedKeywordbyUserId(UserId);
                stopwatch.Stop();
                logger.Info("GetItemListController - GetALLSearchKeywordbyUserId - " + DateTime.Now.ToString() + " - " + "Successfully!!" + ($"Execution Time: {stopwatch.ElapsedMilliseconds} ms"));
                if (dt1 != null)
                {
                    Response.responseStatus = "200";
                    Response.responseData = dt1;
                    Response.message = "User Search Keyword Details got Successfully!!";
                    logger.Info("GetItemListController - GetALLSearchKeywordbyUserId - " + DateTime.Now.ToString() + " - " + "Successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Error in getting User Search Keyword Details!";
                    logger.Info("GetItemListController - GetALLSearchKeywordbyUserId - " + DateTime.Now.ToString() + " - " + "Someting Went Wrong!");
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("GetItemListController - GetALLSearchKeywordbyUserId - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        //Updated Code by Dani on 18-04-2025
        [HttpPost]
        [Route("api/GetItemList/InsertProductExpiryDate")]
        public IHttpActionResult InsertProductExpiryDate([FromBody] ItemCodeExpiryDate itemCodeExpiryDate)
        {
            try
            {
                logger.Info("GetItemListController - InsertProductExpiryDate - " + DateTime.Now.ToString());
                if (itemCodeExpiryDate != null)
                {
                    new BALGetItemList().InsertProductExpiryDate(itemCodeExpiryDate);

                    Response.responseStatus = "200";
                    Response.message = "Expiry Date for the Product has been Reported Successfully!!";
                    logger.Info("GetItemListController - InsertProductExpiryDate - " + DateTime.Now.ToString() + " - " + "Expiry Date for the Product has been Reported Successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Someting Went Wrong!";
                    logger.Error("GetItemListController - InsertProductExpiryDate - " + DateTime.Now.ToString() + " - " + "Someting Went Wrong!");

                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("GetItemListController - InsertProductExpiryDate - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        //Created Code by Dani on 03-04-2025
        [HttpGet]
        [Route("api/GetItemList/GetJsonFilePathForProductData")]
        public async Task<IHttpActionResult> GetJsonFilePathForProductData(string VersionNumber, int UserId)
        {
            DataTable Dt1 = new DataTable();
            ProductFileModel obj = new ProductFileModel();
            if (VersionNumber != "" && UserId != 0)
            {
                Dt1 = new BALGetItemList().InsertUpdateFileLocalbyUserId(VersionNumber, UserId);
            }
            try
            {
                logger.Info("GetItemListController - GetJsonFilePathForProductData - " + DateTime.Now.ToString());
                Dt1 = new BALGetItemList().GetJsonFilePathforProductData(VersionNumber);
                string baseUrl = ConfigurationManager.AppSettings["ProductJsonFile"].ToString();
                if (Dt1.Rows.Count > 0)
                {
                    string fileName = Dt1.Rows[0]["FileName"].ToString();
                    string filePath = $"{baseUrl}{fileName}";
                    obj.FileName = filePath;
                    obj.Version = Dt1.Rows[0]["Version"].ToString();
                    obj.LastUpdatedDate = Convert.ToDateTime(Dt1.Rows[0]["CreatedDate"].ToString());
                    obj.responseStatus = "200";
                    obj.message = "Product Json File Path got Successfully!!";
                    logger.Info("GetItemListController - GetJsonFilePathForProductData - " + DateTime.Now.ToString() + " - " + "Product Json File Path got Successfully!!");
                }
                else
                {
                    obj.responseStatus = "200";
                    obj.messagetitle = "You're All Set";
                    obj.message = "Your product data is already latest.No new updates are available at the moment.";
                    logger.Info("GetItemListController - GetJsonFilePathForProductData - " + DateTime.Now.ToString() + " - " + "Someting Went Wrong!");
                }
                return Ok(obj);
            }
            catch (Exception ex)
            {
                obj.responseStatus = "400";
                obj.message = ex.Message;
                logger.Error("GetItemListController - GetJsonFilePathForProductData - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(obj);
            }
        }

        //Created Code by Dani on 14-04-2025
        [HttpPost]
        [Route("api/GetItemList/DeleteItemCodeExpiryDate")]
        public async Task<IHttpActionResult> DeleteItemCodeExpiryDate(string ItemCode, int StoreId, string ExpiryDate)
        {
            try
            {
                logger.Info("GetItemListController - DeleteItemCodeExpiryDate - " + DateTime.Now.ToString());
                Dt1 = new BALGetItemList().DeleteItemCodeExpiryDate(ItemCode, StoreId, ExpiryDate);
                Response.responseStatus = "200";
                Response.message = "Expiry Date for ItemCode has been removed successfully!!";
                logger.Info("GetItemListController - DeleteItemCodeExpiryDate - " + DateTime.Now.ToString() + " - " + "Expiry Date for ItemCode has been removed successfully!!");

                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.message = ex.Message;
                logger.Error("GetItemListController - DeleteItemCodeExpiryDate - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }

        //Updated Code by Dani on 18-04-2025
        [HttpGet]
        [Route("api/GetItemList/GetAllItemListExpiryDateList")]
        public async Task<IHttpActionResult> GetAllItemListExpiryDateList(int StoreId)
        {
            var response = new ExpiryDateModelResponse();
            try
            {
                logger.Info($"GetItemListController - GetAllItemListExpiryDateList - {DateTime.Now}");

                // Initialize DataTables
                DataTable dt1 = new BALGetItemList().GetAllExpiryDateListCurrentMonth(StoreId);
                DataTable dt2 = new BALGetItemList().GetAllExpiryDateListNextMonth(StoreId);

                // Base URL for images
                string baseUrl = ConfigurationManager.AppSettings["ProductImage"]?.ToString()?.TrimEnd('/') ?? string.Empty;

                // Process CurrentMonth
                if (dt1?.Rows.Count > 0)
                {
                    response.CurrentMonth = dt1.AsEnumerable().Select(row =>
                    {
                        string imageUrls = row["ImageURLs"]?.ToString() ?? string.Empty;
                        var imageUrlsArray = string.IsNullOrWhiteSpace(imageUrls)
                            ? Array.Empty<string>()
                            : imageUrls.Split(',')
                                       .Select(url => $"{baseUrl}/{url.Trim()}")
                                       .Where(url => !string.IsNullOrWhiteSpace(url))
                                       .ToArray();

                        return new ExpiryProductDate
                        {
                            ItemCode = row["ItemCode"]?.ToString() ?? string.Empty,
                            ItemName = row["ItemName"]?.ToString() ?? string.Empty,
                            DepartmentName = row["DepartmentName"]?.ToString() ?? string.Empty,
                            ExpiryDate = row["ExpiryDate"] != DBNull.Value ? Convert.ToDateTime(row["ExpiryDate"]).ToString("yyyy-MM-dd") : string.Empty,
                            ExpiryDateFormatFlag = row["expiryDateformatflag"] != DBNull.Value && Convert.ToBoolean(row["expiryDateformatflag"]),
                            ImageURLs = imageUrlsArray // Assuming ExpiryProductDate has this property
                        };
                    }).ToList();
                }
                else
                {
                    response.CurrentMonth = new List<ExpiryProductDate>();
                }

                // Process NextMonth
                if (dt2?.Rows.Count > 0)
                {
                    response.NextMonth = dt2.AsEnumerable().Select(row =>
                    {
                        string imageUrls = row["ImageURLs"]?.ToString() ?? string.Empty;
                        var imageUrlsArray = string.IsNullOrWhiteSpace(imageUrls)
                            ? Array.Empty<string>()
                            : imageUrls.Split(',')
                                       .Select(url => $"{baseUrl}/{url.Trim()}")
                                       .Where(url => !string.IsNullOrWhiteSpace(url))
                                       .ToArray();

                        return new ExpiryProductDate
                        {
                            ItemCode = row["ItemCode"]?.ToString() ?? string.Empty,
                            ItemName = row["ItemName"]?.ToString() ?? string.Empty,
                            DepartmentName = row["DepartmentName"]?.ToString() ?? string.Empty,
                            ExpiryDate = row["ExpiryDate"] != DBNull.Value? Convert.ToDateTime(row["ExpiryDate"]).ToString("yyyy-MM-dd"): string.Empty,
                            ExpiryDateFormatFlag = row["expiryDateformatflag"] != DBNull.Value && Convert.ToBoolean(row["expiryDateformatflag"]),
                            ImageURLs = imageUrlsArray
                        };
                    }).ToList();
                }
                else
                {
                    response.NextMonth = new List<ExpiryProductDate>();
                }

                // Set response status
                response.responseStatus = "200";
                response.message = "Expiry Date Data for given store retrieved successfully!";
                logger.Info($"GetItemListController - GetAllItemListExpiryDateList - {DateTime.Now} - Expiry Date Data for given store retrieved successfully!");

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.responseStatus = "400";
                response.message = $"Error retrieving expiry date data: {ex.Message}";
                response.CurrentMonth = new List<ExpiryProductDate>();
                response.NextMonth = new List<ExpiryProductDate>();
                logger.Error($"GetItemListController - GetAllItemListExpiryDateList - {DateTime.Now} - Error: {ex.Message}", ex);

                return Ok(response);
            }
        }

        //Updated Code by Dani on 18-04-2025
        [HttpGet]
        [Route("api/GetItemList/GetAllItemListExpiryDatebyItemCode")]
        public async Task<IHttpActionResult> GetAllItemListExpiryDatebyItemCode(string ItemCode, int StoreId)
        {
            ResponseModelGetProductExpiryDate responseModelGetProductExpiryDate = new ResponseModelGetProductExpiryDate();
            Dt1 = new DataTable();
            try
            {
                logger.Info("GetItemListController - GetAllItemListExpiryDatebyItemCode - " + DateTime.Now.ToString());
                Dt1 = new BALGetItemList().GetAllExpiryDatebyItemCode(ItemCode, StoreId);
                if (Dt1.Rows.Count > 0)
                {
                    string baseUrl = ConfigurationManager.AppSettings["ProductImage"].ToString();
                    string ImageUrls = Dt1.Rows[0]["ImageURLs"]?.ToString();
                    string[] ImageUrlsArray = string.IsNullOrWhiteSpace(ImageUrls) ? Array.Empty<string>() : ImageUrls.Split(',').Select(url => baseUrl.TrimEnd('/') + "/" + url.Trim()).Where(url => !string.IsNullOrWhiteSpace(url)).ToArray();
                    responseModelGetProductExpiryDate.responseStatus = "200";
                    responseModelGetProductExpiryDate.ImageURLs = ImageUrlsArray.Length > 0 ? ImageUrlsArray : Array.Empty<string>();
                    responseModelGetProductExpiryDate.responseData = Dt1;
                    responseModelGetProductExpiryDate.message = "Expiry Date Data for the given Item Code get Successfully!!";
                    logger.Info("GetItemListController - GetAllItemListExpiryDatebyItemCode - " + DateTime.Now.ToString() + " - " + "Expiry Date Data for the given Item Code get Successfully!!");
                }
                else
                {
                    responseModelGetProductExpiryDate.responseStatus = "400";
                    responseModelGetProductExpiryDate.responseData = null;
                    responseModelGetProductExpiryDate.message = "No Expiry Date Data is Present for this Item Code!!";
                    logger.Error("GetItemListController - GetAllItemListExpiryDatebyItemCode - " + DateTime.Now.ToString() + " - " + "No Expiry Date Data is Present for this Item Code!!");
                }
                return Ok(responseModelGetProductExpiryDate);
            }
            catch (Exception ex)
            {
                responseModelGetProductExpiryDate.responseStatus = "400";
                responseModelGetProductExpiryDate.responseData = null;
                responseModelGetProductExpiryDate.message = ex.Message;
                logger.Error("GetItemListController - GetAllItemListExpiryDatebyItemCode - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(responseModelGetProductExpiryDate);
            }
        }


        //Updated Code by Dani on 13-07-2025
        [HttpGet]
        [Route("api/GetItemList/GetCompetitorDetailsByUserId")]
        public async Task<IHttpActionResult> GetCompetitorDetailsByUserId(int UserId)
        {
            DataTable dt1 = new DataTable();
            try
            {
                logger.Info("GetItemListController - GetCompetitorDetailsByUserId - " + DateTime.Now.ToString());               
                dt1 = new BALGetItemList().GetAllCompetitorsDetails(UserId);
                logger.Info("GetItemListController - GetCompetitorDetailsByUserId - " + DateTime.Now.ToString() + " - " + "Successfully!!");
                if (dt1 != null)
                {
                    Response.responseStatus = "200";
                    Response.responseData = dt1;
                    Response.message = "Competitors Details got Successfully!!";
                    logger.Info("GetItemListController - GetCompetitorDetailsByUserId - " + DateTime.Now.ToString() + " - " + "Successfully!!");
                }
                else
                {
                    Response.responseStatus = "400";
                    Response.responseData = null;
                    Response.message = "Error in getting Competitors Details!";
                    logger.Info("GetItemListController - GetCompetitorDetailsByUserId - " + DateTime.Now.ToString() + " - " + "Someting Went Wrong!");
                }
                return Ok(Response);
            }
            catch (Exception ex)
            {
                Response.responseStatus = "400";
                Response.responseData = null;
                Response.message = ex.Message;
                logger.Error("GetItemListController - GetCompetitorDetailsByUserId - " + DateTime.Now.ToString() + " - " + ex.Message);
                return Ok(Response);
            }
        }
    }
}

