namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HomeSomeExpensesWeeklySettingUpdate1 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.HomeExpenseWeeklySalesSetting", name: "DepartmentId", newName: "DeliveryCostId");
            RenameIndex(table: "dbo.HomeExpenseWeeklySalesSetting", name: "IX_DepartmentId", newName: "IX_DeliveryCostId");
            AddColumn("dbo.HomeExpenseWeeklySalesSetting", "PaymentFeesId", c => c.Int());
            AddColumn("dbo.HomeExpenseWeeklySalesSetting", "TipsPaidId", c => c.Int());
            CreateIndex("dbo.HomeExpenseWeeklySalesSetting", "PaymentFeesId");
            CreateIndex("dbo.HomeExpenseWeeklySalesSetting", "TipsPaidId");
            AddForeignKey("dbo.HomeExpenseWeeklySalesSetting", "PaymentFeesId", "dbo.DepartmentMaster", "DepartmentId");
            AddForeignKey("dbo.HomeExpenseWeeklySalesSetting", "TipsPaidId", "dbo.DepartmentMaster", "DepartmentId");
            DropColumn("dbo.HomeExpenseWeeklySalesSetting", "HomeExpenseType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.HomeExpenseWeeklySalesSetting", "HomeExpenseType", c => c.Int(nullable: false));
            DropForeignKey("dbo.HomeExpenseWeeklySalesSetting", "TipsPaidId", "dbo.DepartmentMaster");
            DropForeignKey("dbo.HomeExpenseWeeklySalesSetting", "PaymentFeesId", "dbo.DepartmentMaster");
            DropIndex("dbo.HomeExpenseWeeklySalesSetting", new[] { "TipsPaidId" });
            DropIndex("dbo.HomeExpenseWeeklySalesSetting", new[] { "PaymentFeesId" });
            DropColumn("dbo.HomeExpenseWeeklySalesSetting", "TipsPaidId");
            DropColumn("dbo.HomeExpenseWeeklySalesSetting", "PaymentFeesId");
            RenameIndex(table: "dbo.HomeExpenseWeeklySalesSetting", name: "IX_DeliveryCostId", newName: "IX_DepartmentId");
            RenameColumn(table: "dbo.HomeExpenseWeeklySalesSetting", name: "DeliveryCostId", newName: "DepartmentId");
        }
    }
}
