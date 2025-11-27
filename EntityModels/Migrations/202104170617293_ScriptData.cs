namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ScriptData : DbMigration
    {
        public override void Up()
        {
            
            new EntityModels.Models.DBContext().Database.ExecuteSqlCommand("INSERT [dbo].[UserTypeMaster] ([UserType], [IsActive], [CreatedBy], [ModifiedBy], [ModifiedOn], [CreatedOn], [GroupId]) VALUES (N'Administrator', 1, 0, NULL, NULL, NULL, null)");
            new EntityModels.Models.DBContext().Database.ExecuteSqlCommand("INSERT [dbo].[UserMaster] ([FirstName], [UserName], [Password], [CreatedBy], [ModifiedBy], [CreatedOn], [ModifiedOn], [IsActive], [UserTypeId], [GroupId]) VALUES (N'SuperAdmin', N'superadmin', N'123', 0, 0, CAST(N'2021-02-15T17:03:21.237' AS DateTime), CAST(N'2021-02-15T17:03:21.237' AS DateTime), 1, 1, NULL)");
            new EntityModels.Models.DBContext().Database.ExecuteSqlCommand("INSERT [dbo].[UserRoles] ([Role], [UserTypeId]) VALUES (N'Administrator', 1)");


            new EntityModels.Models.DBContext().Database.ExecuteSqlCommand("insert into ModuleMaster values (1, 'Invoice','Invoice')");
            new EntityModels.Models.DBContext().Database.ExecuteSqlCommand("insert into ModuleMaster values (2, 'Document','Document')");
            new EntityModels.Models.DBContext().Database.ExecuteSqlCommand("insert into ModuleMaster values (3, 'SalesSummaryReport','Sales Summary Report')");
            new EntityModels.Models.DBContext().Database.ExecuteSqlCommand("insert into ModuleMaster values (4, 'COGsReport','COGs Report')");
            new EntityModels.Models.DBContext().Database.ExecuteSqlCommand("insert into ModuleMaster values (5, 'OperatingRatioReport','Operating Ratio Report')");
            new EntityModels.Models.DBContext().Database.ExecuteSqlCommand("insert into ModuleMaster values (6, 'PayrollReports','Payroll Reports')");
            new EntityModels.Models.DBContext().Database.ExecuteSqlCommand("insert into ModuleMaster values (7, 'DailyPOSFeeds','Daily POS Feeds')");
            new EntityModels.Models.DBContext().Database.ExecuteSqlCommand("insert into ModuleMaster values (8, 'ShiftWiseReport','Shift Wise Report')");
            new EntityModels.Models.DBContext().Database.ExecuteSqlCommand("insert into ModuleMaster values (9, 'JournalEntries','Journal Entries')");


            new EntityModels.Models.DBContext().Database.ExecuteSqlCommand("insert into PaymentMethodMaster values('Cash')");
            new EntityModels.Models.DBContext().Database.ExecuteSqlCommand("insert into PaymentMethodMaster values('Check')");
            new EntityModels.Models.DBContext().Database.ExecuteSqlCommand("insert into PaymentMethodMaster values('Debit')");
            new EntityModels.Models.DBContext().Database.ExecuteSqlCommand("insert into PaymentMethodMaster values('VISA')");
            new EntityModels.Models.DBContext().Database.ExecuteSqlCommand("insert into PaymentMethodMaster values('AMEX')");
            new EntityModels.Models.DBContext().Database.ExecuteSqlCommand("insert into PaymentMethodMaster values('Master')");
            new EntityModels.Models.DBContext().Database.ExecuteSqlCommand("insert into PaymentMethodMaster values('Discover')");

        }
        
        public override void Down()
        {
        }
    }
}
