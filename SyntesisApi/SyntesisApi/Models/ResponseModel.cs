using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using static SyntesisApi.Models.ApiModel;

namespace SyntesisApi.Models
{
    public class ResponseModel
    {
        public string responseStatus { get; set; }
        public DataTable responseData { get; set; }
        public List<ChatRecord> responseData1 { get; set; }
        public string message { get; set; }       
    }    

    public class ResponseModelGetProductExpiryDate
    {
        public string responseStatus { get; set; }
        public string[] ImageURLs { get; set; }
        public DataTable responseData { get; set; }
        public string message { get; set; }        
    }

    public class ResponseTokenModel
    {
        public string responseStatus { get; set; }
        public DataTable responseData { get; set; }
        public string message { get; set; }
        public string Token { get; set; }
    }

    public class ResponseTokenModelUserDetail
    {
        public string responseStatus { get; set; }
        public UserDetail responseData { get; set; }
        public string message { get; set; }
        public string Token { get; set; }
    } 
    
    public class ResponseTokenModelforUserDetail
    {
        public string responseStatus { get; set; }
        public UserDetail responseData { get; set; }
        public string message { get; set; }
    }    

    public class ResponseTokenModelforReportIssue
    {
        public string responseStatus { get; set; }
        public ReportIssue responseData { get; set; }
        public string message { get; set; }
    }

    public class TopItemsResponse
    {
        public string responseStatus { get; set; }
        public string message { get; set; }
        public List<ItemData> YesterdayData { get; set; }
        public List<ItemData> LastWeekData { get; set; }
        public List<ItemData> LastMonthData { get; set; }
    }

    public class ResponseModelItem
    {
        public string responseStatus { get; set; }
        public ItemListCode responseData { get; set; }
        public string message { get; set; }
    }

    public class ItemData
    {
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string DepartmentName { get; set; }
        public decimal QtySold { get; set; }
        public int StoreID { get; set; }
    }

    public class GetSentryURL
    {
        public string responseStatus { get; set; }
        public string SentryURL { get; set; }
        public string message { get; set; }
    }

    public class ItemListCode
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string DepartmentName { get; set; }
        public string[] ImageURLs { get; set; }
        public string ExpiryDateCount { get; set; }
        public DataTable ItemDetails { get; set; }
    }

    public class GetListData
    {
        public string responseStatus { get; set; }
        public string message { get; set; }
        public List<GetOfflineItemList> getOfflineItemLists { get; set; }
    }

    public class GetOfflineItemList
    {
        public int ItemMovementId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal QtySold { get; set; }
        public decimal BasePrice { get; set; }
        public int ItemMovementHistoryID { get; set; }
        public int ItemMovementdatehistoryID { get; set; }
        public int StoreID { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime Enddate { get; set; }
    }

    public class ProductFileModel
    {
        public string FileName { get; set; }
        public string Version { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public string responseStatus { get; set; }
        public string message { get; set; }
        public string messagetitle { get; set; }
    }

    public class ExpiryDateModelResponse
    {
        public string responseStatus { get; set; }
        public string message { get; set; }
        public List<ExpiryProductDate> CurrentMonth { get; set; }
        public List<ExpiryProductDate> NextMonth { get; set; }
    }

    public class ExpiryProductDate
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string DepartmentName { get; set; }
        public string ExpiryDate { get; set; }
        public bool ExpiryDateFormatFlag { get; set; }
        public string[] ImageURLs { get; set; }
    }

    public class CompetitorDetail
    {
        public string CompetitorsName { get; set; }
        public string ZipCode { get; set; }
        public string CompetitorsNickName { get; set; }
        public int CompetitorsStoreId { get; set; }
        public string ImageURL { get; set; }
    }

    public class ResponseCompetitorDetailItem
    {
        public string responseStatus { get; set; }
        public CompetitorDetail responseData { get; set; }
        public string message { get; set; }
    }
}