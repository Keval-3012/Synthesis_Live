using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EntityModels.Models;
using NLog;
using Repository;
using Repository.IRepository;
using Syncfusion.EJ2.Base;
using Syncfusion.EJ2.GridExport;
using Syncfusion.Pdf;
using Utility;


namespace SysnthesisRepo.Controllers
{
    public class UserLogController : Controller
    {
        private readonly IUserLogRepository _userlogRepository;
        private readonly ISynthesisApiRepository _SynthesisApiRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        public UserLogController()
        {
            this._userlogRepository = new UserLogRepository(new DBContext());
            this._SynthesisApiRepository = new SynthesisApiRepository();
        }


        protected static string StatusMessage = "";
        // GET: UserLog
        /// <summary>
        /// this method is return grid view of User Log data.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Grid()
        {
            try
            {
                //Get all user.
                string UserID = await _SynthesisApiRepository.GetUser();
                string[] id = UserID.Split(',');
                //Get user Log data by Id
                List<UserList> userList = _userlogRepository.GetUserLogData(id);
                ViewBag.User = userList;
                ViewBag.EndDate = DateTime.Now;
                ViewBag.StartDate = DateTime.Now.AddDays(-15);
                ViewBag.Sdate = DateTime.Today.ToString("MM-dd-yyyy");
                //Using this db class get Module and action.
                DataSet ds = await _SynthesisApiRepository.GetModuleandAction();
                DataTable dt = ds.Tables[0];
                List<ModuleList> moduleandActions = new List<ModuleList>();
                foreach (DataRow row in dt.Rows)
                {
                    ModuleList action = new ModuleList();
                    action.ModuleName = row[0].ToString();
                    moduleandActions.Add(action);
                }
                moduleandActions.Insert(0, new ModuleList { ModuleName = "All Module" });
                ViewBag.Module = moduleandActions;

                DataTable dt1 = ds.Tables[1];
                List<ActionList> Action = new List<ActionList>();
                foreach (DataRow row in dt1.Rows)
                {
                    ActionList action = new ActionList();
                    action.ActionName = row[0].ToString();
                    Action.Add(action);
                }
                Action.Insert(0, new ActionList { ActionName = "All Action" });
                ViewBag.Action = Action;

            }
            catch (Exception ex)
            {
                logger.Error("UserLogController - Grid - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return View();
        }

        /// <summary>
        /// This method get User Log list.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="UserName"></param>
        /// <param name="ModuleName"></param>
        /// <param name="ActionName"></param>
        /// <returns></returns>
        public async Task<ActionResult> UserLogList(string value, string UserName, string ModuleName, string ActionName)
        {
            try
            {
                UserLogDetails userLog = new UserLogDetails();
                userLog.UserID = UserName;
                if (ModuleName == "All Module")
                {
                    userLog.ModuleName = null;
                }
                else
                {
                    userLog.ModuleName = ModuleName;
                }
                if (ActionName == "All Action")
                {
                    userLog.ActionName = null;
                }
                else
                {
                    userLog.ActionName = ActionName;
                }
                if (value.Count() == 0)
                {
                    DateTime currentDate = DateTime.Now;
                    //userLog.EndDate = currentDate.AddDays(15).ToString("dd-MMM-yyyy");
                    //userLog.StartDate = currentDate.ToString("dd-MMM-yyyy");
                    userLog.StartDate = DateTime.ParseExact(userLog.StartDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    DateTime parsedDate;
                    string format = "MM-dd-yyyy";
                    userLog.StartDate = value;
                    if (DateTime.TryParseExact(userLog.StartDate, format, null, System.Globalization.DateTimeStyles.None, out parsedDate))
                    {
                        userLog.StartDate = value;
                    }
                }

                Session["UserLog"] = userLog;

            }
            catch (Exception ex)
            {
                logger.Error("UserLogController - UserLogList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView("_UserLog");
            //await _SynthesisApiRepository.GetUserLogDetails(userLog);
            //var jsonResult = Json(JsonConvert.SerializeObject(json), JsonRequestBehavior.AllowGet);
            //jsonResult.MaxJsonLength = int.MaxValue;
            //return jsonResult;
        }

        /// <summary>
        /// This method is url Data Sourc Unmapped.
        /// </summary>
        /// <param name="dm"></param>
        /// <returns></returns>
        public async Task<ActionResult> UrlDatasourceUnmapped(DataManagerRequest dm)
        {
            UserLogDetails userLog = Session["UserLog"] as UserLogDetails;
            List<LogUserList> logUsers = new List<LogUserList>();
            //Using this class get User log details.
            DataTable dt = await _SynthesisApiRepository.GetUserLogDetails(userLog);
            logUsers = (from DataRow row in dt.Rows
                        select new LogUserList
                        {
                            ModuleName = row["ModuleName"].ToString(),
                            LandingDate = row["LandingDate"].ToString(),
                            Action = row["Action"].ToString(),
                            Message = row["Message"].ToString(),
                            InvoiceID = row["InvoiceID"].ToString(),
                            ProductId = row["ProductId"].ToString()
                        }).ToList();
            IEnumerable DataSource = logUsers;
            int count = 0;
            try
            {
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
                count = DataSource.Cast<LogUserList>().Count();
                if (dm.Skip != 0)
                {
                    DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
                }
                if (dm.Take != 0)
                {
                    DataSource = operation.PerformTake(DataSource, dm.Take);
                }
            }
            catch (Exception ex)
            {
                logger.Error("UserLogController - UrlDatasourceUnmapped - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);

        }

        public async Task<ActionResult> ExcelExport(string gridModel)
        {
            UserLogDetails userLog = Session["UserLog"] as UserLogDetails;
            List<LogUserList> logUsers = new List<LogUserList>();
            //Using this class get User log details.
            DataTable dt = await _SynthesisApiRepository.GetUserLogDetails(userLog);
            logUsers = (from DataRow row in dt.Rows
                        select new LogUserList
                        {
                            ModuleName = row["ModuleName"].ToString(),
                            LandingDate = row["LandingDate"].ToString(),
                            Action = row["Action"].ToString(),
                            Message = row["Message"].ToString(),
                            InvoiceID = row["InvoiceID"].ToString(),
                            ProductId = row["ProductId"].ToString()
                        }).ToList();
            GridExcelExport exp = new GridExcelExport();
            exp.FileName = "UserLogs.xlsx";
            Syncfusion.EJ2.Grids.Grid gridProperty = ConvertGridObject(gridModel);
            return exp.ExcelExport<LogUserList>(gridProperty, logUsers);

        }

        private Syncfusion.EJ2.Grids.Grid ConvertGridObject(string gridProperty)
        {
            Syncfusion.EJ2.Grids.Grid GridModel = (Syncfusion.EJ2.Grids.Grid)Newtonsoft.Json.JsonConvert.DeserializeObject(gridProperty, typeof(Syncfusion.EJ2.Grids.Grid));

            GridColumnModel cols = (GridColumnModel)Newtonsoft.Json.JsonConvert.DeserializeObject("{\"allowGrouping\":false,\"allowPaging\":false,\"pageSettings\":{\"currentPage\":1,\"pageCount\":2,\"pageSize\":100},\"sortSettings\":{},\"allowPdfExport\":true,\"allowExcelExport\":true,\"aggregates\":[],\"filterSettings\":{\"immediateModeDelay\":1500,\"type\":\"CheckBox\"},\"groupSettings\":{\"columns\":[],\"enableLazyLoading\":false,\"disablePageWiseAggregates\":false},\"columns\":[],\"locale\":\"en-US\",\"searchSettings\":{\"key\":\"\",\"fields\":[]}}", typeof(GridColumnModel));
            cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "ModuleName", HeaderText = "Module Name" });
            cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "LandingDate", HeaderText = "Date/Time" });

            cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "Action", HeaderText = "Action" });
            cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "Message", HeaderText = "Message" });
            
            foreach (var item in cols.columns)
            {
                item.AutoFit = true;
            }
            GridModel.Columns = cols.columns;
            return GridModel;
        }

        public async Task<ActionResult> PdfExport(string gridModel)
        {
            UserLogDetails userLog = Session["UserLog"] as UserLogDetails;
            List<LogUserList> logUsers = new List<LogUserList>();
            //Using this class get User log details.
            DataTable dt = await _SynthesisApiRepository.GetUserLogDetails(userLog);
            logUsers = (from DataRow row in dt.Rows
                        select new LogUserList
                        {
                            ModuleName = row["ModuleName"].ToString(),
                            LandingDate = row["LandingDate"].ToString(),
                            Action = row["Action"].ToString(),
                            Message = row["Message"].ToString(),
                            InvoiceID = row["InvoiceID"].ToString(),
                            ProductId = row["ProductId"].ToString()
                        }).ToList();

            PdfDocument doc = new PdfDocument();
            doc.PageSettings.Orientation = PdfPageOrientation.Landscape;
            doc.PageSettings.Size = PdfPageSize.A3;


            GridPdfExport exp = new GridPdfExport();
            exp.Theme = "flat-saffron";
            exp.FileName = "UserLogDetails.pdf";
            exp.PdfDocument = doc;

            Syncfusion.EJ2.Grids.Grid gridProperty = ConvertGridObject(gridModel);
            return exp.PdfExport<ExpenseCheckSelect>(gridProperty, logUsers);
        }

        #region new grid
        public async Task<ActionResult> Grid_New()
        {
            try
            {
                //Get all user.
                string UserID = await _SynthesisApiRepository.GetUser();
                string[] id = UserID.Split(',');
                //Get user Log data by Id
                List<UserList> userList = _userlogRepository.GetUserLogData(id);
                ViewBag.User = userList;
                ViewBag.EndDate = DateTime.Now;
                ViewBag.StartDate = DateTime.Now.AddDays(-15);
                ViewBag.Sdate = DateTime.Today.ToString("MM-dd-yyyy");
                //Using this db class get Module and action.
                DataSet ds = await _SynthesisApiRepository.GetModuleandAction();
                DataTable dt = ds.Tables[0];
                List<ModuleList> moduleandActions = new List<ModuleList>();
                foreach (DataRow row in dt.Rows)
                {
                    ModuleList action = new ModuleList();
                    action.ModuleName = row[0].ToString();
                    moduleandActions.Add(action);
                }
                moduleandActions.Insert(0, new ModuleList { ModuleName = "All Module" });
                ViewBag.Module = moduleandActions;

                DataTable dt1 = ds.Tables[1];
                List<ActionList> Action = new List<ActionList>();
                foreach (DataRow row in dt1.Rows)
                {
                    ActionList action = new ActionList();
                    action.ActionName = row[0].ToString();
                    Action.Add(action);
                }
                Action.Insert(0, new ActionList { ActionName = "All Action" });
                ViewBag.Action = Action;

            }
            catch (Exception ex)
            {
                logger.Error("UserLogController - Grid - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return View();
        }

        public async Task<ActionResult> UserLogList_New(string value, string UserName, string ModuleName)
        {
            try
            {
                UserLogDetails userLog = new UserLogDetails();
                userLog.UserID = UserName;
                if (ModuleName == "All Module")
                {
                    userLog.ModuleName = null;
                }
                else
                {
                    userLog.ModuleName = ModuleName;
                }
                if (value.Count() == 0)
                {
                    DateTime currentDate = DateTime.Now;
                    //userLog.EndDate = currentDate.AddDays(15).ToString("dd-MMM-yyyy");
                    //userLog.StartDate = currentDate.ToString("dd-MMM-yyyy");
                    userLog.StartDate = DateTime.ParseExact(userLog.StartDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    DateTime parsedDate;
                    string format = "MM-dd-yyyy";
                    userLog.StartDate = value;
                    if (DateTime.TryParseExact(userLog.StartDate, format, null, System.Globalization.DateTimeStyles.None, out parsedDate))
                    {
                        userLog.StartDate = value;
                    }
                }

                Session["UserLog"] = userLog;

            }
            catch (Exception ex)
            {
                logger.Error("UserLogController - UserLogList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PartialView("_UserLog_New");
           
        }

        public async Task<ActionResult> UrlDatasourceUnmapped_New(DataManagerRequest dm)
        {
            UserLogDetails userLog = Session["UserLog"] as UserLogDetails;
            List<LogUserList> logUsers = new List<LogUserList>();
            //Using this class get User log details.
            DataTable dt = await _SynthesisApiRepository.GetUserLogDetailsNew(userLog);
            logUsers = (from DataRow row in dt.Rows
                        select new LogUserList
                        {
                            ModuleName = row["PageName"].ToString(),
                            StartTime = row["StartTime"].ToString(),
                            EndTime = row["EndTime"].ToString(),
                            TotalTimeSpent = row["TotalTimeSpent"].ToString(),
                            LandingDate = row["ActivityDate"].ToString(),
                            Action = row["Action"].ToString(),
                            AllData = row["AllData"].ToString()
                        }).ToList();
            IEnumerable DataSource = logUsers;
            int count = 0;
            try
            {
                DataOperations operation = new DataOperations();
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search

                }
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);

                }
                count = DataSource.Cast<LogUserList>().Count();
                if (dm.Skip != 0)
                {
                    DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
                }
                if (dm.Take != 0)
                {
                    DataSource = operation.PerformTake(DataSource, dm.Take);
                }
            }
            catch (Exception ex)
            {
                logger.Error("UserLogController - UrlDatasourceChildList_New - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);

        }

        public async Task<ActionResult> UrlDatasourceChildList_New(DataManagerRequest dm)
        {
            UserLogDetails userLog = Session["UserLog"] as UserLogDetails;
            List<LogUserList> logUsers = new List<LogUserList>();
            //Using this class get User log details.

            string Alldata = Convert.ToString(dm.Where[0].value);
            string[] split = Alldata.Split('/');

            for (int i = 0; i < split.Length; i++)
            {
                split[i] = split[i].Trim();
            }

            UserLogDetails data = new UserLogDetails();
            data.StartDate = split[0];
            data.EndDate = split[1];
            data.ModuleName = split[2];
            data.UserID = split[3];

            DataTable dt = await _SynthesisApiRepository.GetUserLogDetailsNewChild(data);
            logUsers = (from DataRow row in dt.Rows
                        select new LogUserList
                        {
                            LandingDate = row["LandingDate"].ToString(),
                            Action = row["Action"].ToString(),
                            Message = row["Message"].ToString(),
                            InvoiceID = row["InvoiceID"].ToString(),
                            ModuleName = row["ModuleName"].ToString(),
                            ProductId = row["ProductId"].ToString()
                        }).ToList();
            IEnumerable DataSource = logUsers;
            int count = 0;
            try
            {
                DataOperations operation = new DataOperations();
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);

                }
                 count = DataSource.Cast<LogUserList>().Count();
                if (dm.Skip != 0)
                {
                    DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
                }
                if (dm.Take != 0)
                {
                    DataSource = operation.PerformTake(DataSource, dm.Take);
                }
            }
            catch (Exception ex)
            {
                logger.Error("UserLogController - UrlDatasourceChildList_New - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet) : Json(DataSource, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);

        }

        #endregion
    }
}