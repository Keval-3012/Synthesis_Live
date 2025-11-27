namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HomeExpenseDetail11 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.HomeExpenseWeeklySalesSetting", new[] { "DepartmentId" });
            DropIndex("dbo.HomeExpenseWeeklySalesSetting", new[] { "VendorId" });
            AlterColumn("dbo.HomeExpenseWeeklySalesSetting", "DepartmentId", c => c.Int());
            AlterColumn("dbo.HomeExpenseWeeklySalesSetting", "VendorId", c => c.Int());
            CreateIndex("dbo.HomeExpenseWeeklySalesSetting", "DepartmentId");
            CreateIndex("dbo.HomeExpenseWeeklySalesSetting", "VendorId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.HomeExpenseWeeklySalesSetting", new[] { "VendorId" });
            DropIndex("dbo.HomeExpenseWeeklySalesSetting", new[] { "DepartmentId" });
            AlterColumn("dbo.HomeExpenseWeeklySalesSetting", "VendorId", c => c.Int(nullable: false));
            AlterColumn("dbo.HomeExpenseWeeklySalesSetting", "DepartmentId", c => c.Int(nullable: false));
            CreateIndex("dbo.HomeExpenseWeeklySalesSetting", "VendorId");
            CreateIndex("dbo.HomeExpenseWeeklySalesSetting", "DepartmentId");
        }
    }
}
