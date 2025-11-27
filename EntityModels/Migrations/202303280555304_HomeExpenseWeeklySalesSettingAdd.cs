namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HomeExpenseWeeklySalesSettingAdd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HomeExpenseWeeklySalesSetting",
                c => new
                    {
                        HomeExpenseWeeklySalesSettingId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 200),
                        StoreId = c.Int(),
                        PaymentFeesDeptId = c.Int(),
                        DeliveryCostDeptId = c.Int(),
                        TipsPaidDeptId = c.Int(),
                        PaymentFeesVendorId = c.Int(),
                        DeliveryCostVendorId = c.Int(),
                        TipsPaidVendorId = c.Int(),
                    })
                .PrimaryKey(t => t.HomeExpenseWeeklySalesSettingId)
                .ForeignKey("dbo.DepartmentMaster", t => t.PaymentFeesDeptId)
                .ForeignKey("dbo.DepartmentMaster", t => t.DeliveryCostDeptId)
                .ForeignKey("dbo.DepartmentMaster", t => t.TipsPaidDeptId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .ForeignKey("dbo.VendorMaster", t => t.PaymentFeesVendorId)
                .ForeignKey("dbo.VendorMaster", t => t.DeliveryCostVendorId)
                .ForeignKey("dbo.VendorMaster", t => t.TipsPaidVendorId)
                .Index(t => t.StoreId)
                .Index(t => t.PaymentFeesDeptId)
                .Index(t => t.DeliveryCostDeptId)
                .Index(t => t.TipsPaidDeptId)
                .Index(t => t.PaymentFeesVendorId)
                .Index(t => t.DeliveryCostVendorId)
                .Index(t => t.TipsPaidVendorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HomeExpenseWeeklySalesSetting", "TipsPaidVendorId", "dbo.VendorMaster");
            DropForeignKey("dbo.HomeExpenseWeeklySalesSetting", "DeliveryCostVendorId", "dbo.VendorMaster");
            DropForeignKey("dbo.HomeExpenseWeeklySalesSetting", "PaymentFeesVendorId", "dbo.VendorMaster");
            DropForeignKey("dbo.HomeExpenseWeeklySalesSetting", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.HomeExpenseWeeklySalesSetting", "TipsPaidDeptId", "dbo.DepartmentMaster");
            DropForeignKey("dbo.HomeExpenseWeeklySalesSetting", "DeliveryCostDeptId", "dbo.DepartmentMaster");
            DropForeignKey("dbo.HomeExpenseWeeklySalesSetting", "PaymentFeesDeptId", "dbo.DepartmentMaster");
            DropIndex("dbo.HomeExpenseWeeklySalesSetting", new[] { "TipsPaidVendorId" });
            DropIndex("dbo.HomeExpenseWeeklySalesSetting", new[] { "DeliveryCostVendorId" });
            DropIndex("dbo.HomeExpenseWeeklySalesSetting", new[] { "PaymentFeesVendorId" });
            DropIndex("dbo.HomeExpenseWeeklySalesSetting", new[] { "TipsPaidDeptId" });
            DropIndex("dbo.HomeExpenseWeeklySalesSetting", new[] { "DeliveryCostDeptId" });
            DropIndex("dbo.HomeExpenseWeeklySalesSetting", new[] { "PaymentFeesDeptId" });
            DropIndex("dbo.HomeExpenseWeeklySalesSetting", new[] { "StoreId" });
            DropTable("dbo.HomeExpenseWeeklySalesSetting");
        }
    }
}
