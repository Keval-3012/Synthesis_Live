using EntityModels.Models;
using NLog;
using Repository;
using Repository.IRepository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SysnthesisRepo.Controllers
{
    public class UserActivityLogController : Controller
    {
        // GET: ActivityLog

        protected static string StatusMessage = "";
        protected static string ActivityLogMessage = "";
        private const int FirstPageIndex = 1;
        protected static int TotalDataCount;
        protected static Array Arr;
        protected static bool IsArray;
        protected static IEnumerable BindData;
        protected static bool IsEdit = false;
        private readonly IUserActivityLogRepository _activityLogRepository;
        private readonly ISynthesisApiRepository _synthesisApiRepository;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public UserActivityLogController()
        {
            this._activityLogRepository = new UserActivityLogRepository(new DBContext());
            this._synthesisApiRepository = new SynthesisApiRepository();
        }
        /// <summary>
        /// This Action method return view for viewbag 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.Title = "User Activity Log - Synthesis";
            return View();
        }

        /// <summary>
        /// This is Grid 
        /// </summary>
        /// <param name="IsBindData"></param>
        /// <param name="currentPageIndex"></param>
        /// <param name="orderby"></param>
        /// <param name="IsAsc"></param>
        /// <param name="PageSize"></param>
        /// <param name="SearchRecords"></param>
        /// <param name="Alpha"></param>
        /// <param name="SearchTitle"></param>
        /// <returns></returns>
        public ActionResult Grid(int IsBindData = 1, int currentPageIndex = 1, string orderby = "id", int IsAsc = 0, int PageSize = 100, int SearchRecords = 1, string Alpha = "", string SearchTitle = "")
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
            catch (Exception ex) 
            {
                logger.Error("ActivityLogController - Grid - "  + DateTime.Now + " - " + ex.Message.ToString());
            }

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
            IEnumerable Data = null;
            try
            {
                if (IsBindData == 1 || IsEdit == true)
                {
                    BindData = _activityLogRepository.GetData();
                    TotalDataCount = BindData.OfType<ActivityLog>().ToList().Count();
                }


                if (TotalDataCount == 0)
                {
                    StatusMessage = "NoItem";
                }

                ViewBag.IsBindData = IsBindData;
                ViewBag.CurrentPageIndex = currentPageIndex;
                ViewBag.LastPageIndex = _activityLogRepository.getLastPageIndex(PageSize, TotalDataCount);
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
                var ColumnName = typeof(ActivityLog).GetProperties().Where(p => p.Name == orderby).FirstOrDefault();


                //Response.Write(ColumnName);
                //Response.End();
                if (IsAsc == 1)
                {
                    ViewBag.AscVal = 0;
                    Data = BindData.OfType<ActivityLog>().ToList().OrderBy(n => ColumnName.GetValue(n, null)).Skip(startIndex - 1).Take(PageSize);
                }
                else
                {

                    ViewBag.AscVal = 1;

                    Data = BindData.OfType<ActivityLog>().ToList().Skip(startIndex - 1).Take(PageSize);
                }
                StatusMessage = "";
                // ViewBag.Pagetext = "Showing " + iStartsRecods + " to " + iEndRecord + " of " + iTotalRecords + " entries";
            }
            catch (Exception ex)
            {
                logger.Error("ActivityLogController - Grid - "  + DateTime.Now + " - "  + ex.Message.ToString());
            }
            return View(Data);
            #endregion
        }

        /// <summary>
        ///   This is a AddActivityLoad Class
        /// </summary>
        /// <param name="onClickText"></param>
        /// <param name="ActionName"></param>
        /// <param name="ControllerName"></param>
        /// <param name="ModuleName"></param>
        /// <param name="PageName"></param>
        /// <param name="LblAction"></param>
        /// <param name="Message"></param>
        public void AddActivityLoad(string onClickText, string ActionName, string ControllerName, string ModuleName, string PageName, string LblAction, string Message)
        {
            try
            {
                clsActivityLog Log = new clsActivityLog();
                //Log = api.GetActivityDetails(ControllerName, ActionName);
                Log.ModuleName = ModuleName;
                Log.PageName = PageName;
                Log.Message = Message;
                Log.CreatedBy = Convert.ToInt32(Session["UserID"]);
                Log.Action = LblAction;
                _synthesisApiRepository.CreateLog(Log);
            }
            catch (Exception ex)
            {
                logger.Error("ActivityLogController - AddActivityLoad - "  + DateTime.Now + " - "  + ex.Message.ToString());
            }
           
        }
    }
}