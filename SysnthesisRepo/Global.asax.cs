using Syncfusion.Licensing;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using EntityModels.Migrations;
using NLog;
using Aspose.Pdf.Operators;

namespace SysnthesisRepo
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            if (File.Exists(Server.MapPath("Syncfusion.txt")))
            {
                string licenseKey = System.IO.File.ReadAllText(Server.MapPath("Syncfusion.txt"), Encoding.UTF8);
                SyncfusionLicenseProvider.RegisterLicense(licenseKey);
            }
            Aspose.Pdf.License licensePDF = new Aspose.Pdf.License();
            licensePDF.SetLicense(Server.MapPath("~/LicenceFiles/Aspose.Total.lic"));
            Aspose.Cells.License licenseCells = new Aspose.Cells.License();
            licenseCells.SetLicense(Server.MapPath("~/LicenceFiles/Aspose.Total.lic"));
            Database.SetInitializer<EntityModels.Models.DBContext>(new MigrateDatabaseToLatestVersion<EntityModels.Models.DBContext, Configuration>());
        }
        protected void Application_Error()
        {
            Exception exception = Server.GetLastError();
            if (exception.Message != null)
            {
                logger.Error("Global - Application_Error - " + DateTime.Now + " - " + exception.Message.ToString());
            }
            Server.ClearError();
            Response.Redirect("~/AdminInclude/SomethingWentWrong");
        }
    }
}
