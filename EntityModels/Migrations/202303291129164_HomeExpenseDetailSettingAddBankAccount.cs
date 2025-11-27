namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HomeExpenseDetailSettingAddBankAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HomeExpenseWeeklySalesSetting", "BankAccountId", c => c.Int());
            CreateIndex("dbo.HomeExpenseWeeklySalesSetting", "BankAccountId");
            AddForeignKey("dbo.HomeExpenseWeeklySalesSetting", "BankAccountId", "dbo.DepartmentMaster", "DepartmentId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HomeExpenseWeeklySalesSetting", "BankAccountId", "dbo.DepartmentMaster");
            DropIndex("dbo.HomeExpenseWeeklySalesSetting", new[] { "BankAccountId" });
            DropColumn("dbo.HomeExpenseWeeklySalesSetting", "BankAccountId");
        }
    }
}
