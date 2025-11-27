using SyntesisApi.DAL;
using SyntesisApi.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using static SyntesisApi.Models.ApiModel;
using System.IO;
using System.Net.Http;
using System.Configuration;
using System.Web.Http;
using System.Web.Razor.Tokenizer;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SyntesisApi.BAL
{
    public class BALGetItemList
    {
        clsDAL db = new clsDAL();
        public DataTable GetItemList(string ItemCode)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_GetItemListBasedonItemCode";
                Cmd.Parameters.AddWithValue("@Mode", "GetItemListbyProductItemCode");
                Cmd.Parameters.AddWithValue("@ItemCode", ItemCode);
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public DataTable GetAllItemList(int StoreId)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_GetItemListBasedonItemCode";
                Cmd.Parameters.AddWithValue("@Mode", "GetAllItemList");
                Cmd.Parameters.AddWithValue("@StoreId", StoreId);
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public async Task<DataTable> GetOfflineItemList(DateTime StartDate)
        {
            // List<GetOfflineItemList> ds = new List<GetOfflineItemList>();
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_GetItemListBasedonItemCode";
                Cmd.Parameters.AddWithValue("@Mode", "GetOfflineItemList");
                Cmd.Parameters.AddWithValue("@StartDate", StartDate);
                dt = new DataTable();
                dt = db.Select(Cmd);

                //   ds = ConvertToList<GetOfflineItemList>(dt);
            }
            catch (Exception ex)
            {
            }
            return await Task.Run(() => dt);
        }

        public DataTable GetYesterdayTopItemList(int StoreId, string DepartmentIds)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_GetItemListBasedonItemCode";
                Cmd.Parameters.AddWithValue("@Mode", "GetYesterdayTop50Items");
                Cmd.Parameters.AddWithValue("@StoreId", StoreId);
                Cmd.Parameters.AddWithValue("@DepartmentIds", DepartmentIds);
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public DataTable GetLastWeekTopItemList(int StoreId, string DepartmentIds)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_GetItemListBasedonItemCode";
                Cmd.Parameters.AddWithValue("@Mode", "GetLastWeekTop50Items");
                Cmd.Parameters.AddWithValue("@StoreId", StoreId);
                Cmd.Parameters.AddWithValue("@DepartmentIds", DepartmentIds);
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public DataTable GetLastMonthTopItemList(int StoreId, string DepartmentIds)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_GetItemListBasedonItemCode";
                Cmd.Parameters.AddWithValue("@Mode", "GetLastMonthTop50Items");
                Cmd.Parameters.AddWithValue("@StoreId", StoreId);
                Cmd.Parameters.AddWithValue("@DepartmentIds", DepartmentIds);
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public DataTable GetYesterdayLowestItemList(int StoreId, string DepartmentIds)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_GetItemListBasedonItemCode";
                Cmd.Parameters.AddWithValue("@Mode", "GetYesterdayLowest50Items");
                Cmd.Parameters.AddWithValue("@StoreId", StoreId);
                Cmd.Parameters.AddWithValue("@DepartmentIds", DepartmentIds);
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public DataTable GetLastWeekLowestItemList(int StoreId, string DepartmentIds)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_GetItemListBasedonItemCode";
                Cmd.Parameters.AddWithValue("@Mode", "GetLastWeekLowest50Items");
                Cmd.Parameters.AddWithValue("@StoreId", StoreId);
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public DataTable GetLastMonthLowestItemList(int StoreId, string DepartmentIds)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_GetItemListBasedonItemCode";
                Cmd.Parameters.AddWithValue("@Mode", "GetLastMonthLowest50Items");
                Cmd.Parameters.AddWithValue("@StoreId", StoreId);
                Cmd.Parameters.AddWithValue("@DepartmentIds", DepartmentIds);
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public int ApkWatchList(ApkWatchList obj)
        {
            int id = 0;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_GetItemListBasedonItemCode";
                Cmd.Parameters.AddWithValue("@Mode", "InsertApkWatchList");
                Cmd.Parameters.AddWithValue("@ApkWatchListID", obj.ApkWatchListID);
                Cmd.Parameters.AddWithValue("@StoreId", obj.StoreId);
                Cmd.Parameters.AddWithValue("@UPCCode", obj.UPCCode);
                Cmd.Parameters.AddWithValue("@ProductName", obj.ProductName);
                Cmd.Parameters.AddWithValue("@DepartmentName", obj.DepartmentName);
                Cmd.Parameters.AddWithValue("@YesterdayAvgPrice", obj.YesterdayAvgPrice);
                Cmd.Parameters.AddWithValue("@YesterdayQty", obj.YesterdayQty);
                Cmd.Parameters.AddWithValue("@LastWeekAvgPrice", obj.LastWeekAvgPrice);
                Cmd.Parameters.AddWithValue("@LastWeekQty", obj.LastWeekQty);
                Cmd.Parameters.AddWithValue("@Last15DaysAvgPrice", obj.Last15DaysAvgPrice);
                Cmd.Parameters.AddWithValue("@Last15DaysQty", obj.Last15DaysQty);
                Cmd.Parameters.AddWithValue("@Last30DaysAvgPrice", obj.Last30DaysAvgPrice);
                Cmd.Parameters.AddWithValue("@Last30DaysQty", obj.Last30DaysQty);
                Cmd.Parameters.AddWithValue("@MonthtoDateAvgPrice", obj.MonthtoDateAvgPrice);
                Cmd.Parameters.AddWithValue("@MonthtoDateQty", obj.MonthtoDateQty);
                Cmd.Parameters.AddWithValue("@LastMonthAvgPrice", obj.LastMonthAvgPrice);
                Cmd.Parameters.AddWithValue("@LastMonthQty", obj.LastMonthQty);
                Cmd.Parameters.AddWithValue("@YearTillDateAvgPrice", obj.YearTillDateAvgPrice);
                Cmd.Parameters.AddWithValue("@YearTillDateQty", obj.YearTillDateQty);
                Cmd.Parameters.AddWithValue("@ThisWeekQty", obj.ThisWeekQty);
                Cmd.Parameters.AddWithValue("@LastQuarterAvgPrice", obj.LastQuarterAvgPrice);
                Cmd.Parameters.AddWithValue("@LastQuarterQty", obj.LastQuarterQty);
                Cmd.Parameters.AddWithValue("@LastYearAvgPrice", obj.LastYearAvgPrice);
                Cmd.Parameters.AddWithValue("@LastYearQty", obj.LastYearQty);
                db.Insert_Update_Del(Cmd);
            }
            catch (Exception)
            {
                throw;
            }
            return id;
        }

        public DataTable GetAllWatchList(int StoreId)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_GetItemListBasedonItemCode";
                Cmd.Parameters.AddWithValue("@Mode", "GetAllWatchList");
                Cmd.Parameters.AddWithValue("@StoreId", StoreId);
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public bool DeleteApkWatchList(int StoreId, string UPCCode)
        {
            bool ds = false;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_GetItemListBasedonItemCode";
                Cmd.Parameters.AddWithValue("@Mode", "DeleteWatchList");
                Cmd.Parameters.AddWithValue("@StoreId", StoreId);
                Cmd.Parameters.AddWithValue("@UPCCode", UPCCode);
                DataTable dt = new DataTable();
                clsDAL obj = new clsDAL();
                dt = obj.Select(Cmd);
                if (dt != null && dt.Rows.Count > 0)
                {
                    ds = Convert.ToBoolean(dt.Rows[0]["IsDelete"]);
                }
            }
            catch (Exception ex)
            {
            }
            return ds;
        }

        public bool CheckWatchList(int StoreId, string UPCCode)
        {
            bool ds = false;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_GetItemListBasedonItemCode";
                Cmd.Parameters.AddWithValue("@Mode", "CheckWatchList");
                Cmd.Parameters.AddWithValue("@StoreId", StoreId);
                Cmd.Parameters.AddWithValue("@UPCCode", UPCCode);
                DataTable dt = new DataTable();
                clsDAL obj = new clsDAL();
                dt = obj.Select(Cmd);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        ds = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return ds;
        }

        public DataTable GetItemBasedonSearching(string Search)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_GetItemListBasedonItemCode";
                Cmd.Parameters.AddWithValue("@Mode", "SearchByItemCode");
                Cmd.Parameters.AddWithValue("@Search", Search);
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public DataTable GetAllItemMovementDepartment()
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_GetItemListBasedonItemCode";
                Cmd.Parameters.AddWithValue("@Mode", "GetAllDepartment");
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public DataTable GetVersionNumber(string Type)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_GetItemListBasedonItemCode";
                Cmd.Parameters.AddWithValue("@Mode", "GetVersionCode");
                Cmd.Parameters.AddWithValue("@Type", Type);
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public DataTable InsertItemScanLogbyUserId(string ItemCode, int UserId, int ScannedStoreId, double Longitude, double Latitude, int UserRightsforStoreAccess)
        {
            DataTable dt = null;
            try
            {
                if (UserRightsforStoreAccess == 1)
                {
                    SqlCommand Cmd = new SqlCommand();
                    Cmd.CommandText = "SP_InsertProductItemScanLog";
                    Cmd.Parameters.AddWithValue("@Mode", "InsertItemScanLog");
                    Cmd.Parameters.AddWithValue("@ItemCode", ItemCode);
                    Cmd.Parameters.AddWithValue("@UserId", UserId);
                    Cmd.Parameters.AddWithValue("@ScannedStoreId", ScannedStoreId);
                    Cmd.Parameters.AddWithValue("@Longitude", Longitude);
                    Cmd.Parameters.AddWithValue("@Latitude", Latitude);
                    Cmd.Parameters.AddWithValue("@UserRightsforStoreAccess", UserRightsforStoreAccess);
                    dt = new DataTable();
                    dt = db.SelectLog(Cmd);
                }
                else
                {
                    SqlCommand Cmd = new SqlCommand();
                    Cmd.CommandText = "SP_InsertProductItemScanLog";
                    Cmd.Parameters.AddWithValue("@Mode", "InsertItemScanLog");
                    Cmd.Parameters.AddWithValue("@ItemCode", ItemCode);
                    Cmd.Parameters.AddWithValue("@UserId", UserId);
                    Cmd.Parameters.AddWithValue("@ScannedStoreId", ScannedStoreId);
                    Cmd.Parameters.AddWithValue("@Longitude", Longitude);
                    Cmd.Parameters.AddWithValue("@Latitude", Latitude);
                    Cmd.Parameters.AddWithValue("@UserRightsforStoreAccess", UserRightsforStoreAccess);
                    dt = new DataTable();
                    dt = new DataTable();
                    dt = db.SelectLog(Cmd);
                }
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public DataTable InsertItemScanLogBasedbyUserId(string ItemCode, int UserId, int ScannedStoreId, double Longitude, double Latitude, int UserRightsforStoreAccess, string DeviceName, string OSVersion, bool LocationAccess)
        {
            DataTable dt = null;
            try
            {
                if (UserRightsforStoreAccess == 1)
                {
                    SqlCommand Cmd = new SqlCommand();
                    Cmd.CommandText = "SP_InsertProductItemScanLog";
                    Cmd.Parameters.AddWithValue("@Mode", "InsertItemScanLogUpdate");
                    Cmd.Parameters.AddWithValue("@ItemCode", ItemCode);
                    Cmd.Parameters.AddWithValue("@UserId", UserId);
                    Cmd.Parameters.AddWithValue("@ScannedStoreId", ScannedStoreId);
                    Cmd.Parameters.AddWithValue("@Longitude", Longitude);
                    Cmd.Parameters.AddWithValue("@Latitude", Latitude);
                    Cmd.Parameters.AddWithValue("@UserRightsforStoreAccess", UserRightsforStoreAccess);
                    Cmd.Parameters.AddWithValue("@DeviceName", DeviceName);
                    Cmd.Parameters.AddWithValue("@OSVersion", OSVersion);
                    Cmd.Parameters.AddWithValue("@LocationAccess", LocationAccess);
                    dt = new DataTable();
                    dt = db.SelectLog(Cmd);
                }
                else
                {
                    SqlCommand Cmd = new SqlCommand();
                    Cmd.CommandText = "SP_InsertProductItemScanLog";
                    Cmd.Parameters.AddWithValue("@Mode", "InsertItemScanLogUpdate");
                    Cmd.Parameters.AddWithValue("@ItemCode", ItemCode);
                    Cmd.Parameters.AddWithValue("@UserId", UserId);
                    Cmd.Parameters.AddWithValue("@ScannedStoreId", ScannedStoreId);
                    Cmd.Parameters.AddWithValue("@Longitude", Longitude);
                    Cmd.Parameters.AddWithValue("@Latitude", Latitude);
                    Cmd.Parameters.AddWithValue("@UserRightsforStoreAccess", UserRightsforStoreAccess);
                    Cmd.Parameters.AddWithValue("@DeviceName", DeviceName);
                    Cmd.Parameters.AddWithValue("@OSVersion", OSVersion);
                    Cmd.Parameters.AddWithValue("@LocationAccess", LocationAccess);
                    dt = new DataTable();
                    dt = new DataTable();
                    dt = db.SelectLog(Cmd);
                }
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public DataTable GetSentryURL()
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_GetItemListBasedonItemCode";
                Cmd.Parameters.AddWithValue("@Mode", "GetSentryURL");
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        //Updated Code by Dani on 07-03-2025
        public DataTable GetItemListProduct(string ItemCode)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_GetItemListBasedonItemCode";
                Cmd.Parameters.AddWithValue("@Mode", "GetItemListbyProductItemCode");
                Cmd.Parameters.AddWithValue("@ItemCode", ItemCode);
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        //Created Code by Himanshu 07-03-2025
        public DataTable GetItemBasedonKeywordSearching(string Search, string Departments)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_GetItemListBasedonItemCode";
                Cmd.Parameters.AddWithValue("@Mode", "SearchByKeyword");
                Cmd.Parameters.AddWithValue("@Search", Search);
                Cmd.Parameters.AddWithValue("@Departments", Departments);
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        //Created Code by Dani 12-03-2025
        public DataTable GetALLReportIssueOptions()
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_BarcodeProductComparison";
                Cmd.Parameters.AddWithValue("@Mode", "GetAllReportIssueOptions");
                dt = new DataTable();
                dt = db.SelectLog(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public int ReportIssueDump(ReportIssueDump obj)
        {
            int id = 0;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_BarcodeProductComparison";
                Cmd.Parameters.AddWithValue("@Mode", "InsertReportIssue");
                Cmd.Parameters.AddWithValue("@ItemCode", obj.ItemCode);
                Cmd.Parameters.AddWithValue("@ItemName", obj.ItemName);
                Cmd.Parameters.AddWithValue("@ReportedOptionID", obj.ReportedOptionID);
                Cmd.Parameters.AddWithValue("@AdditionalNotes", obj.AdditionalNotes);
                Cmd.Parameters.AddWithValue("@UserName", obj.UserName);
                Cmd.Parameters.AddWithValue("@UserId", obj.UserId);
                db.Insert_Update_Del_Log(Cmd);
            }
            catch (Exception ex)
            {
            }
            return id;
        }

        //public int ProductImageUpdate(ProductImageRequest request)
        //{
        //    int lastId = 0;
        //    try
        //    {
        //        string savePath = ConfigurationManager.AppSettings["ProductImagePath"];
        //        if (!Directory.Exists(savePath))
        //        {
        //            Directory.CreateDirectory(savePath);
        //        }

        //        using (var client = new HttpClient())
        //        {
        //            for (int i = 0; i < request.ImageURLs.Length; i++)
        //            {
        //                string imageUrl = request.ImageURLs[i];

        //                // Download the image
        //                byte[] imageBytes = client.GetByteArrayAsync(imageUrl).Result;
        //                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        //                string fileExtension = Path.GetExtension(new Uri(imageUrl).AbsolutePath) ?? ".jpg";
        //                string fileName = $"{request.ItemCode}_{timestamp}_{i}{fileExtension}";
        //                string filePath = Path.Combine(savePath, fileName);

        //                // Save the image to the server
        //                File.WriteAllBytes(filePath, imageBytes);

        //                // Update the database with the file path and URL
        //                using (SqlCommand Cmd = new SqlCommand())
        //                {
        //                    Cmd.CommandText = "SP_BarcodeProductComparison";
        //                    Cmd.Parameters.AddWithValue("@Mode", "UpdateProductImagesfromAPK");
        //                    Cmd.Parameters.AddWithValue("@BarcodeNumber", request.ItemCode);
        //                    Cmd.Parameters.AddWithValue("@ImageURL", imageUrl);
        //                    Cmd.Parameters.AddWithValue("@ImagePath", filePath);
        //                    lastId = db.Insert_Update_Del_Log_Image(Cmd);

        //                    if (lastId == 0)
        //                    {
        //                        Console.WriteLine($"Failed to update image URL: {imageUrl} with path: {filePath}");
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Error in ProductImageUpdate: " + ex.Message);
        //        return 0;
        //    }
        //    return lastId;
        //}

        //Api Created By Himanshu 02-04-2025
        public int ProductImageUpdate(string itemCode, HttpFileCollection files)
        {
            int lastId = 0;
            try
            {
                string savePath = ConfigurationManager.AppSettings["ProductImagePath"];
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }

                int count = 1;
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };

                foreach (string fileKey in files)
                {
                    HttpPostedFile file = files[fileKey];
                    if (file != null && file.ContentLength > 0)
                    {
                        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                        string fileExtension = Path.GetExtension(file.FileName);
                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            continue;
                        }

                        string fileName = $"img-{itemCode}-{count}{fileExtension}";
                        string filePath = Path.Combine(savePath, fileName);

                        // Save the image to the server
                        file.SaveAs(filePath);

                        // Update database with file path
                        using (SqlCommand Cmd = new SqlCommand())
                        {
                            Cmd.CommandText = "SP_BarcodeProductComparison";
                            Cmd.Parameters.AddWithValue("@Mode", "UpdateProductImagesfromAPK");
                            Cmd.Parameters.AddWithValue("@BarcodeNumber", itemCode);
                            Cmd.Parameters.AddWithValue("@ImagePath", filePath);
                            lastId = db.Insert_Update_Del_Log_Image(Cmd);

                            if (lastId == 0)
                            {
                                Console.WriteLine($"Failed to update image with path: {filePath}");
                                break;
                            }
                        }
                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in ProductImageUpdate: " + ex.Message);
                return 0;
            }
            return lastId;
        }

        //Created Code by Himanshu 25-03-2025
        public int DietaryAttributeDump(DietaryAttributeSearchKeywords obj)
        {
            int id = 0;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_BarcodeProductComparison";
                Cmd.Parameters.AddWithValue("@Mode", "InsertDietaryAttributeSearchKeywords");
                Cmd.Parameters.AddWithValue("@MainTitle", obj.MainTitle);
                Cmd.Parameters.AddWithValue("@SearchKeywords", obj.SearchKeywords);
                db.Insert_Update_Del_Log(Cmd);
            }
            catch (Exception ex)
            {
            }
            return id;
        }

        //Created Code by Himanshu 25-03-2025
        public DataTable GetALLDietaryAttributeSearchKeywords()
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_BarcodeProductComparison";
                Cmd.Parameters.AddWithValue("@Mode", "GetAllDietaryAttributes");
                dt = new DataTable();
                dt = db.SelectLog(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        //Created Code by Dani on 27-03-2025
        public DataTable InsertSearchedItem(SearchbyKeyword searchbyKeyword)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_InsertProductItemScanLog";
                Cmd.Parameters.AddWithValue("@Mode", "InsertSearchedItem");
                Cmd.Parameters.AddWithValue("@SearchableKeyword", searchbyKeyword.SearchableKeyword);
                Cmd.Parameters.AddWithValue("@ItemCode", searchbyKeyword.ItemCode);
                Cmd.Parameters.AddWithValue("@ItemName", searchbyKeyword.ItemName);
                Cmd.Parameters.AddWithValue("@UserId", searchbyKeyword.UserId);
                dt = new DataTable();
                dt = db.SelectLog(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        //Created Code by Dani on 01-04-2025
        public DataTable GetAllSearchedKeywordbyUserId(int UserId)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_InsertProductItemScanLog";
                Cmd.Parameters.AddWithValue("@Mode", "GetALLSearchKeywordbyUserId");
                Cmd.Parameters.AddWithValue("@UserId", UserId);
                dt = new DataTable();
                dt = db.SelectLog(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        //Created Code by Dani on 02-04-2025
        public DataTable InsertProductExpiryDate(ItemCodeExpiryDate itemCodeExpiryDate)
        {
            DataTable dt = null;
            try
            {
                //string expiryDateFormatted = ValidateAndFormatExpiryDate(itemCodeExpiryDate.ExpiryDate);
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_InsertProductItemScanLog";
                Cmd.Parameters.AddWithValue("@Mode", "InsertProductExpiryDate");
                Cmd.Parameters.AddWithValue("@ItemCode", itemCodeExpiryDate.ItemCode);
                Cmd.Parameters.AddWithValue("@ItemName", itemCodeExpiryDate.ItemName);
                Cmd.Parameters.AddWithValue("@DepartmentName", itemCodeExpiryDate.DepartmentName);
                Cmd.Parameters.AddWithValue("@StoreId", itemCodeExpiryDate.StoreId);
                Cmd.Parameters.AddWithValue("@UserID", itemCodeExpiryDate.UserId);
                Cmd.Parameters.AddWithValue("@ExpiryDate", itemCodeExpiryDate.ExpiryDate);
                Cmd.Parameters.AddWithValue("@ExpiryDateFormatFlag", itemCodeExpiryDate.ExpiryDateFormatFlag);
                dt = new DataTable();
                dt = db.SelectLog(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        //Created Code by Dani on 18-04-2025
        private string ValidateAndFormatExpiryDate(string expiryDate)
        {
            if (string.IsNullOrEmpty(expiryDate))
                throw new ArgumentException("Expiry date cannot be empty.");

            // Try parsing as MM/dd/yyyy
            if (DateTime.TryParseExact(expiryDate, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fullDate))
            {
                return fullDate.ToString("MM/dd/yyyy"); // Return in MM/dd/yyyy format
            }

            // Try parsing as MM/yyyy
            if (Regex.IsMatch(expiryDate, @"^\d{2}/\d{4}$"))
            {
                if (DateTime.TryParseExact(expiryDate, "MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime monthYear))
                {
                    return monthYear.ToString("MM/yyyy"); // Return in MM/yyyy format
                }
            }
            throw new ArgumentException("Invalid expiry date format. Use MM/dd/yyyy or MM/yyyy.");
        }

        //Created Code by Dani on 03-04-2025
        public DataTable GetJsonFilePathforProductData(string VersionNumber)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_GetItemListBasedonItemCode";
                Cmd.Parameters.AddWithValue("@Mode", "GetProductFilePath");
                Cmd.Parameters.AddWithValue("@Version", VersionNumber);
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        //Created Code by Dani on 03-04-2025
        public DataTable InsertUpdateFileLocalbyUserId(string VersionNumber, int UserId)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_InsertProductItemScanLog";
                Cmd.Parameters.AddWithValue("@Mode", "InsertVesrionUpdatebyUserId");
                Cmd.Parameters.AddWithValue("@VersionNumber", VersionNumber);
                Cmd.Parameters.AddWithValue("@UserId", UserId);
                dt = new DataTable();
                dt = db.SelectLog(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        //Created Code by Dani on 14-04-2025
        public DataTable DeleteItemCodeExpiryDate(string ItemCode,int StoreId, string ExpiryDate)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_InsertProductItemScanLog";
                Cmd.Parameters.AddWithValue("@Mode", "DeleteItemCodeDateofExpiry");
                Cmd.Parameters.AddWithValue("@ItemCode", ItemCode);
                Cmd.Parameters.AddWithValue("@StoreId", StoreId);
                Cmd.Parameters.AddWithValue("@ExpiryDate", ExpiryDate);
                clsDAL obj = new clsDAL();
                dt = obj.SelectLog(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        //Created Code by Dani on 14-04-2025
        public DataTable GetAllExpiryDateListCurrentMonth(int StoreId)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_InsertProductItemScanLog";
                Cmd.Parameters.AddWithValue("@Mode", "GetAllExpiryDatebyStorewiseCurrentMonth");
                Cmd.Parameters.AddWithValue("@StoreId", StoreId);
                dt = new DataTable();
                dt = db.SelectLog(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        //Created Code by Dani on 15-04-2025
        public DataTable GetAllExpiryDateListNextMonth(int StoreId)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_InsertProductItemScanLog";
                Cmd.Parameters.AddWithValue("@Mode", "GetAllExpiryDatebyStorewiseNextmonth");
                Cmd.Parameters.AddWithValue("@StoreId", StoreId);
                dt = new DataTable();
                dt = db.SelectLog(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        //Created Code by Dani on 14-04-2025
        public DataTable GetAllExpiryDatebyItemCode(string ItemCode,int StoreId)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_InsertProductItemScanLog";
                Cmd.Parameters.AddWithValue("@Mode", "GetAllExpiryDatebyItemCode");
                Cmd.Parameters.AddWithValue("@ItemCode", ItemCode);
                Cmd.Parameters.AddWithValue("@StoreId", StoreId);
                dt = new DataTable();
                dt = db.SelectLog(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        //Created Code by Dani on 13-07-2025
        public DataTable GetAllCompetitorsDetails(int UserId)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_GetCompetitorDetails";
                Cmd.Parameters.AddWithValue("@Mode", "GetAllCompetitorsDetails");
                Cmd.Parameters.AddWithValue("@UserId", UserId);
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }
    }
}