using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;

namespace SyntesisApi.Models
{
    public class ApiModel
    {
        public class StoreList
        {
            public int StoreId { get; set; }
            public int DepartmentId { get; set; }
            public string DepartmentName { get; set; }
        }

        public class LastSlide
        {
            public int EmployeeId { get; set; }
            public string LastSlideName { get; set; }
        }

        public class CompleteyTraining
        {
            public string Data { get; set; }
            public int EmployeeId { get; set; }
            public string TrainingDate { get; set; }
            public string TrainingTime { get; set; }
            public string LastSlidename { get; set; }
        }

        public class CompleteyTrainingPost
        {
            public int EmployeeId { get; set; }
            public bool IsTraningCompleted { get; set; }
            public string TraningContent { get; set; }
            public DateTime? TrainingCompletedDateTime { get; set; }

            public DateTime? TrainingCompletedTime { get; set; }
            public string TraningFilePath { get; set; }
            public string LastSlidename { get; set; }
        }
        #region Start Harsh
        public class EmployeeList
        {
            public int EmployeeId { get; set; }
        }
        
        public class GetConcentData
        {
            public string StoreName { get; set; }
            public string StoreManagerName { get; set; }
            public string Date { get; set; }
            public string EmployeeName { get; set; }
            public string Chk1 { get; set; }
            public string Chk2 { get; set; }
            public string Chk3 { get; set; }
            public string SignatureFileName { get; set; }
            public string EmployeeID { get; set; }
            public string LanguageID { get; set; }
            public string StoreId { get; set; }
            public int EmployeeChildID { get; set; }
            public int CreatedId { get; set; }
        }

        public class EmployeeWorkConsent
        {
            public string EmployeeID { get; set; }
            public string CompanyName { get; set; }
            public string Date { get; set; }
            public string EmployeeName { get; set; }
            public string EmployeeSignature { get; set; }
            public string StoreManagerName { get; set; }
            public string StoreManagerSignature { get; set; }
            public string languageId { get; set; }
            public string StoreId { get; set; }
            public int EmployeeChildID { get; set; }
            public int CreatedId { get; set; }
            public List<EmployeeDetail> EmployeeDetails { get; set; }
        }
        public class EmployeeDetail
        {
            public string DateTimeOfRequest { get; set; }
            public string OriginalShift { get; set; }
            public string RequestedShift { get; set; }
        }
        public partial class EmployeeDocument
        {
            public int DocumentId { get; set; }
            public Nullable<int> EmployeeID { get; set; }
            public string DocFileName { get; set; }
            public string Comment { get; set; }
            public string DocumentType { get; set; }
            public Nullable<int> LocationFrom { get; set; }
            public Nullable<int> CreatedID { get; set; }
            public Nullable<System.DateTime> CreatedDate { get; set; }
            public Nullable<bool> Status { get; set; }
            public Nullable<System.DateTime> UpdatedDate { get; set; }
            public Nullable<int> DocumentTypeId { get; set; }
            public Nullable<int> StoreId { get; set; }
            public int LanguageID { get; set; }
            public int EmployeeChildID { get; set; }
        }
        public enum DocumentType
        {
            Consent = 1,
            ScheduleChange = 2
        }

        public class UploadFiles
        {
            public string Image { get; set; }
            public string EmployeeId { get; set; }
            public bool IsVaccine { get; set; }
            public bool IsExemption { get; set; }
            public int StoreId { get; set; }
            public int EmployeeChildID { get; set; }
            public int CreatedID { get; set; }
        }

        public partial class VaccineCertificateInfo
        {
            public int Id { get; set; }
            public int EmployeeID { get; set; }
            public bool IsVaccine { get; set; }
            public bool IsExemption { get; set; }
            public string FileName { get; set; }
            public int CreatedID { get; set; }
            public bool Status { get; set; }
            public int StoreId { get; set; }
            public int EmployeeChildID { get; set; }
        }

        public class EmpRetirementInfo
        {
            public int EmployeeChildID { get; set; }
            public int EmployeeID { get; set; }
            public int OptStatus { get; set; }
            public string EmployeeSignature { get; set; }
            public string EmployeeName { get; set; }
            public string languageId { get; set; }
            public int CreatedID { get; set; }
            public string FileName { get; set; }
        }

        public class EmployeeHealthBenefit
        {
            public int EmployeeID { get; set; }
            public string EmployeeName { get; set; }
            public string MaritalStatus { get; set; }
            public string EmployementDate { get; set; }
            public string DOB { get; set; }
            public bool OtherCoverage { get; set; }
            public bool RefusedCoverage { get; set; }
            public string OtherCoverageDetail { get; set; }
            public string EmployeeSignature { get; set; }
            public string languageId { get; set; }
            public string DocFileName {  get; set; }
            public int EmployeeChildID { get; set; }
            public int CreatedID { get; set; }
        }
        public class EmployeeWarning
        {
            public string StoreName { get; set; }
            public string StoreManagerName { get; set; }
            public string Date { get; set; }
            public int EmployeeID { get; set; }
            public int EmployeeChildID { get; set; }
            public string EmployeeName { get; set; }
            public string Chk1 { get; set; }
            public string Chk2 { get; set; }
            public string Chk3 { get; set; }
            public string Warning { get; set; }
            public int StoreId { get; set; }
            public string Remarks { get; set; }
            public string EmployeeSignature { get; set; }
            public string StoreManagerSignature { get; set; }
            public int LanguageId { get; set; }
            public string DocFileName { get; set; }
            public int CreatedBy { get; set; }
            public bool IsActive { get; set; }
        }
        public class EmployeeTermination
        {
            public string StoreName { get; set;}
            public string StoreManagerName { get; set;}
            public string Date { get; set;}
            public int EmployeeID { get; set; }
            public int EmployeeChildID { get; set;}
            public string EmployeeName { get; set;}
            public int StoreId { get; set; }
            public string EmployeeSignature { get; set; }
            public int LanguageId { get; set; }
            public string DocFileName { get; set; }
            public int CreatedBy { get; set; }
            public bool IsActive { get; set; }
        }

            public class Store
        {
            public int StoreID { get; set; }
            public int Sign_UnSigned { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
        }

        public class Language
        {
            public int LanguageId { get; set; }
        }
        public class trainingSlides
        {
            public trainingSlides(int SrNo, bool Questions, string URL, string SlideName, string NextSlideName)
            {
                this.SrNo = SrNo;
                this.Questions = Questions;
                this.URL = URL;
                this.SlideName = SlideName;
                this.NextSlideName = NextSlideName;
            }
            public int SrNo { get; set; }
            public bool Questions { get; set; }
            public string URL { get; set; }
            public string SlideName { get; set; }
            public string NextSlideName { get; set; }
        }

        public class LanguageUpdate
        {
            public int EmployeeId { get; set; }
            public int LanguageId { get; set; }
        }

        public class ApkWatchList
        {
            public int ApkWatchListID { get; set; }
            public int StoreId { get; set; } 
            public string UPCCode { get; set; } 
            public string ProductName { get; set; } 
            public string DepartmentName { get; set; } 
            public decimal YesterdayAvgPrice { get; set; }
            public decimal YesterdayQty { get; set; }
            public decimal LastWeekAvgPrice { get; set; }
            public decimal LastWeekQty { get; set; }
            public decimal Last15DaysAvgPrice { get; set; }
            public decimal Last15DaysQty { get; set; }
            public decimal Last30DaysAvgPrice { get; set; }
            public decimal Last30DaysQty { get; set; }
            public decimal MonthtoDateAvgPrice { get; set; }
            public decimal MonthtoDateQty { get; set; }
            public decimal LastMonthAvgPrice { get; set; }
            public decimal LastMonthQty { get; set; }
            public decimal YearTillDateAvgPrice { get; set; }
            public decimal YearTillDateQty { get; set; } 
            public decimal ThisWeekQty { get; set; } 
            public decimal LastQuarterAvgPrice { get; set; } 
            public decimal LastQuarterQty { get; set; } 
            public decimal LastYearAvgPrice { get; set; } 
            public decimal LastYearQty { get; set; } 
        }

        public class UserDetail
        {
            public int UserTypeId { get; set; }
            public int UserId { get; set; }
            public int ProductImageUpload { get; set; }
            public int UpdateProductDetails { get; set; }
            public int IsAbleExpiryChange { get; set; }
            public int DesignatedStore { get; set; }
            public string UserName { get; set; }            
            public string Name { get; set; }            
            public string UserRightsforStoreAccess { get; set; }
            public ModuleAccess ModuleAccess { get; set; }
            public List<StoreDetail> StoreDetails { get; set; }                        
        }

        public class StoreDetail
        {
            public int StoreId { get; set; }
            public string StoreName { get; set; }
            public string StoreNickName { get; set; }
            public string StoreAddress { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public int Radius { get; set; }
            public int LocationAccess { get; set; }
            public int ViewStoreDataAccess { get; set; }
        }
        #endregion

        public class VersionManager
        {
            public int VersionId { get; set; }
            public int UserId { get; set; }
            public string VersionNumber { get; set; }
            public string Title { get; set; } 
            public string Description { get; set; }
            public string ForceUpdate { get; set; }
            public string ForwardURL { get; set; }
            public string Type { get; set; }
        }

        public class ReportIssue
        {
            public int ReportId { get; set; }
            public int IssueType { get; set; }            
        }

        public class ReportIssueDump
        {
            public int IssueReportId { get; set; }
            public string ItemCode { get; set; }
            public string ItemName { get; set; }
            public int ReportedOptionID { get; set; }
            public int UserId { get; set; }
            public string AdditionalNotes { get; set; }            
            public string UserName { get; set; }            
        }

        public class ProductImageRequest
        {
            public string ItemCode { get; set; }
            public string[] ImageURLs { get; set; }
        }

        public class DietaryAttributeSearchKeywords
        {
            public int DietaryAttributeId { get; set; }
            public string MainTitle { get; set; }
            public string SearchKeywords { get; set; }
        }

        public class SearchbyKeyword
        {
            public int SearchKeywordId { get; set; }
            public string SearchableKeyword { get; set; }
            public string ItemCode { get; set; }
            public string ItemName { get; set; }         
            public int UserId { get; set; }                       
        }

        public class ItemCodeExpiryDate
        {
            public string ItemCode { get; set; }
            public string ItemName { get; set; }
            public string DepartmentName { get; set; }
            public int StoreId { get; set; }
            public int UserId { get; set; }
            public string ExpiryDate { get; set; }
            public bool ExpiryDateFormatFlag { get; set; }
        }

        public class ModuleAccess
        {
            public bool Chat { get; set; } = true;
            public bool ProductScan { get; set; } = true;
            public bool ProductDetails { get; set; } = true;
            public bool CompetitionDashboard { get; set; } = true;
            public bool BestSeller { get; set; } = true;
            public bool WatchList { get; set; } = true;
            public bool Search { get; set; } = true;
            public bool SlowMover { get; set; } = true;
            public bool History { get; set; } = true;
            public bool ExpirationDate { get; set; } = true;
        }
    }
}