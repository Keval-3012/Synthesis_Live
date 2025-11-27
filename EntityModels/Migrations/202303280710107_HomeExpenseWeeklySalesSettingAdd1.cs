namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HomeExpenseWeeklySalesSettingAdd1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.HomeExpenseWeeklySalesSetting", "DeliveryCostDeptId", "dbo.DepartmentMaster");
            DropForeignKey("dbo.HomeExpenseWeeklySalesSetting", "TipsPaidDeptId", "dbo.DepartmentMaster");
            DropForeignKey("dbo.HomeExpenseWeeklySalesSetting", "DeliveryCostVendorId", "dbo.VendorMaster");
            DropForeignKey("dbo.HomeExpenseWeeklySalesSetting", "TipsPaidVendorId", "dbo.VendorMaster");
            DropIndex("dbo.HomeExpenseWeeklySalesSetting", new[] { "DeliveryCostDeptId" });
            DropIndex("dbo.HomeExpenseWeeklySalesSetting", new[] { "TipsPaidDeptId" });
            DropIndex("dbo.HomeExpenseWeeklySalesSetting", new[] { "DeliveryCostVendorId" });
            DropIndex("dbo.HomeExpenseWeeklySalesSetting", new[] { "TipsPaidVendorId" });
            RenameColumn(table: "dbo.HomeExpenseWeeklySalesSetting", name: "PaymentFeesDeptId", newName: "DepartmentId");
            RenameColumn(table: "dbo.HomeExpenseWeeklySalesSetting", name: "PaymentFeesVendorId", newName: "VendorId");
            RenameIndex(table: "dbo.HomeExpenseWeeklySalesSetting", name: "IX_PaymentFeesDeptId", newName: "IX_DepartmentId");
            RenameIndex(table: "dbo.HomeExpenseWeeklySalesSetting", name: "IX_PaymentFeesVendorId", newName: "IX_VendorId");
            AddColumn("dbo.HomeExpenseWeeklySalesSetting", "HomeExpenseType", c => c.Int(nullable: false));
            DropColumn("dbo.HomeExpenseWeeklySalesSetting", "Name");
            DropColumn("dbo.HomeExpenseWeeklySalesSetting", "DeliveryCostDeptId");
            DropColumn("dbo.HomeExpenseWeeklySalesSetting", "TipsPaidDeptId");
            DropColumn("dbo.HomeExpenseWeeklySalesSetting", "DeliveryCostVendorId");
            DropColumn("dbo.HomeExpenseWeeklySalesSetting", "TipsPaidVendorId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.HomeExpenseWeeklySalesSetting", "TipsPaidVendorId", c => c.Int());
            AddColumn("dbo.HomeExpenseWeeklySalesSetting", "DeliveryCostVendorId", c => c.Int());
            AddColumn("dbo.HomeExpenseWeeklySalesSetting", "TipsPaidDeptId", c => c.Int());
            AddColumn("dbo.HomeExpenseWeeklySalesSetting", "DeliveryCostDeptId", c => c.Int());
            AddColumn("dbo.HomeExpenseWeeklySalesSetting", "Name", c => c.String(nullable: false, maxLength: 200));
            DropColumn("dbo.HomeExpenseWeeklySalesSetting", "HomeExpenseType");
            RenameIndex(table: "dbo.HomeExpenseWeeklySalesSetting", name: "IX_VendorId", newName: "IX_PaymentFeesVendorId");
            RenameIndex(table: "dbo.HomeExpenseWeeklySalesSetting", name: "IX_DepartmentId", newName: "IX_PaymentFeesDeptId");
            RenameColumn(table: "dbo.HomeExpenseWeeklySalesSetting", name: "VendorId", newName: "PaymentFeesVendorId");
            RenameColumn(table: "dbo.HomeExpenseWeeklySalesSetting", name: "DepartmentId", newName: "PaymentFeesDeptId");
            CreateIndex("dbo.HomeExpenseWeeklySalesSetting", "TipsPaidVendorId");
            CreateIndex("dbo.HomeExpenseWeeklySalesSetting", "DeliveryCostVendorId");
            CreateIndex("dbo.HomeExpenseWeeklySalesSetting", "TipsPaidDeptId");
            CreateIndex("dbo.HomeExpenseWeeklySalesSetting", "DeliveryCostDeptId");
            AddForeignKey("dbo.HomeExpenseWeeklySalesSetting", "TipsPaidVendorId", "dbo.VendorMaster", "VendorId");
            AddForeignKey("dbo.HomeExpenseWeeklySalesSetting", "DeliveryCostVendorId", "dbo.VendorMaster", "VendorId");
            AddForeignKey("dbo.HomeExpenseWeeklySalesSetting", "TipsPaidDeptId", "dbo.DepartmentMaster", "DepartmentId");
            AddForeignKey("dbo.HomeExpenseWeeklySalesSetting", "DeliveryCostDeptId", "dbo.DepartmentMaster", "DepartmentId");
        }
    }
}
