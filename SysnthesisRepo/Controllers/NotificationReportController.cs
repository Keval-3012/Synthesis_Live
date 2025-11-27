using EntityModels.Models;
using NLog;
using Repository;
using Repository.IRepository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Utility;

namespace SysnthesisRepo.Controllers
{
    public class NotificationReportController : Controller
    {
        // GET: Admin/NotificationReport
        protected static string StatusMessage = "";
        private const int FirstPageIndex = 1;
        protected static int TotalDataCount;
        protected static Array Arr;
        protected static bool IsArray;
        protected static IEnumerable BindData;
        protected static bool IsEdit = false;
        protected static string storenamevalue = "";
        private readonly INotificationReportRepository _notificationReportRepository;
        private readonly ICommonRepository _commonRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        public NotificationReportController()
        {
            this._notificationReportRepository = new NotificationReportRepository(new DBContext());
            this._commonRepository = new CommonRepository(new DBContext());
        }

        /// <summary>
        /// This method return index view.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string storenameval)
        {
            try
            {
                storenamevalue = storenameval;
                ViewBag.storenamevalue = storenameval;
            }
            catch (Exception ex)
            {
                logger.Error("NotificationReportController - Index - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return View();
        }
        public ActionResult Grid(int IsBindData = 1, int currentPageIndex = 1, string orderby = "InvoiceId", int IsAsc = 0, int PageSize = 100, int SearchRecords = 1, string Alpha = "", string SearchTitle = "")
        {
            #region MyRegion_Array
            try
            {
                if (IsArray == true)
                {
                    foreach (string a1 in Arr)
                    {
                        if (a1.Split(':')[0].ToString() == "IsBindData")
                        {
                            IsBindData = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "currentPageIndex")
                        {
                            currentPageIndex = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "orderby")
                        {
                            orderby = Convert.ToString(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "IsAsc")
                        {
                            IsAsc = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "PageSize")
                        {
                            PageSize = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "SearchRecords")
                        {
                            SearchRecords = Convert.ToInt32(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "Alpha")
                        {
                            Alpha = Convert.ToString(a1.Split(':')[1].ToString());
                        }
                        if (a1.Split(':')[0].ToString() == "SearchTitle")
                        {
                            SearchTitle = Convert.ToString(a1.Split(':')[1].ToString());
                        }

                    }
                }
            }
            catch { }

            IsArray = false;
            Arr = new string[]
            {  "IsBindData:" + IsBindData
                ,"currentPageIndex:" + currentPageIndex
                ,"orderby:" + orderby
                ,"IsAsc:" + IsAsc
                ,"PageSize:" + PageSize
                ,"SearchRecords:" + SearchRecords
                ,"Alpha:" + Alpha
                ,"SearchTitle:" + SearchTitle
            };
            #endregion

            #region MyRegion_BindData
            int startIndex = ((currentPageIndex - 1) * PageSize) + 1;
            int endIndex = startIndex + PageSize - 1;

            if (IsBindData == 1 || IsEdit == true)
            {
                BindData = GetData(SearchRecords, SearchTitle).OfType<Invoice_Notification>().ToList();
                TotalDataCount = BindData.OfType<Invoice_Notification>().ToList().Count();
            }

            if (TotalDataCount == 0)
            {
                StatusMessage = "NoItem";
            }

            ViewBag.IsBindData = IsBindData;
            ViewBag.CurrentPageIndex = currentPageIndex;
            ViewBag.LastPageIndex = this.getLastPageIndex(PageSize);
            ViewBag.OrderByVal = orderby;
            ViewBag.IsAscVal = IsAsc;
            ViewBag.PageSize = PageSize;
            ViewBag.Alpha = Alpha;
            ViewBag.SearchRecords = SearchRecords;
            ViewBag.SearchTitle = SearchTitle;
            ViewBag.StatusMessage = StatusMessage;
            ViewBag.startindex = startIndex;
            if (TotalDataCount < endIndex)
            {
                ViewBag.endIndex = TotalDataCount;
            }
            else
            {
                ViewBag.endIndex = endIndex;
            }
            ViewBag.TotalDataCount = TotalDataCount;
            var ColumnName = typeof(Invoice_Notification).GetProperties().Where(p => p.Name == orderby).FirstOrDefault();

            IEnumerable Data = null;
            if (IsAsc == 1)
            {
                ViewBag.AscVal = 0;
                Data = BindData.OfType<Invoice_Notification>().ToList().OrderBy(n => ColumnName.GetValue(n, null)).Skip(startIndex - 1).Take(PageSize);
            }
            else
            {

                ViewBag.AscVal = 1;

                Data = BindData.OfType<Invoice_Notification>().ToList().OrderByDescending(n => ColumnName.GetValue(n, null)).Skip(startIndex - 1).Take(PageSize);
            }
            StatusMessage = "";
            // ViewBag.Pagetext = "Showing " + iStartsRecods + " to " + iEndRecord + " of " + iTotalRecords + " entries";
            return View(Data);
            #endregion
        }
        private int getLastPageIndex(int PageSize)
        {
            int lastPageIndex = Convert.ToInt32(TotalDataCount) / PageSize;
            if (TotalDataCount % PageSize > 0)
                lastPageIndex += 1;
            return lastPageIndex;
        }
        /// <summary>
        /// This method is get Notification Report with Id.
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        private IEnumerable GetData(int SearchRecords = 0, string SearchTitle = "")
        {
            List<Invoice_Notification> invoice_Notifications = new List<Invoice_Notification>();
            try
            {
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                var UserId = _commonRepository.getUserId(UserName);
                invoice_Notifications = _notificationReportRepository.getNotificationList(UserId);
            }
            catch (Exception ex)
            {
                logger.Error("NotificationReportController - Index - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return invoice_Notifications;
        }
    }
}