using EntityModels.Models;
using NLog;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class TopsellerPriceRepository : ITopsellerPriceRepository
    {
        private DBContext _context;
        private readonly ICommonRepository _ICommonRepositoryRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        public TopsellerPriceRepository(DBContext context)
        {
            _context = context;
            _ICommonRepositoryRepository = new CommonRepository(context);
        }

        public DataTable TopSellerPriceGrid(string[] value, string Upccode, string Departmentname, string ColorFilter)
        {
            DataTable dt = new DataTable();
            try
            {
                if (Departmentname == "All Departments")
                {
                    Departmentname = "";
                }
                string EndDate = "";
                string StartDate = "";
                if (value.Count() == 0)
                {
                    DateTime currentDate = DateTime.Now;
                    EndDate = currentDate.AddDays(15).ToString("yyyy-MM-dd");
                    currentDate = currentDate.AddDays(-15);
                    StartDate = "2022-01-01";
                }
                else
                {
                    DateTime date = Convert.ToDateTime(value[0]);
                    DateTime date1 = Convert.ToDateTime(value[1]);
                    StartDate = date.ToString("yyyy-MM-dd");
                    EndDate = date1.ToString("yyyy-MM-dd");
                }
                if (ColorFilter == "")
                {
                    ColorFilter = null;
                }

                var color = ColorFilter == null ? null : (ColorFilter.Substring(0, ColorFilter.Length - 1));
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_TOPSeller_Report_Master_Proc";
                Cmd.Parameters.AddWithValue("@StartDate", StartDate);
                Cmd.Parameters.AddWithValue("@EndDate", EndDate);
                Cmd.Parameters.AddWithValue("@Departmentsearch", Departmentname);
                dt = _ICommonRepositoryRepository.Select(Cmd);
            }
            catch (Exception ex)
            {
                logger.Error("TopsellerPriceRepository - TopSellerPriceGrid - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return dt;
        }

        public List<VendorNameList> ViewBagVendor()
        {
            List<VendorNameList> list = new List<VendorNameList>();
            try
            {
                list = _context.Database.SqlQuery<VendorNameList>("SP_ProductPrice @Spara = {0}", 5).ToList();
                list.Insert(0, new VendorNameList { VendorName = "All Vendors" });
            }
            catch (Exception ex)
            {
                logger.Error("TopsellerPriceRepository - ViewBagVendor - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return list;
        }

        public List<DepartmetItemList> ViewBagDepartment()
        {
            List<DepartmetItemList> list = new List<DepartmetItemList>();
            try
            {
                var DepartmentMasters = _context.Database.SqlQuery<DepartmetItemList>("SP_ItemMovementBySupplier @Mode = {0},@ID={1}", "GetDepartment_TopSeller", 0).ToList();
                list = (from I in DepartmentMasters
                                      select new DepartmetItemList
                                      {
                                          DepartmentName = I.DepartmentName
                                      }).Distinct().ToList();
            }
            catch (Exception ex)
            {
                logger.Error("TopsellerPriceRepository - ViewBagDepartment - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return list;
        }
    }
}
