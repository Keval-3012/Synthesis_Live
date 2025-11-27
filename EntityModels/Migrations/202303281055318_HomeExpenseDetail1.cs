namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HomeExpenseDetail1 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.HomeExpenseWeeklySalesSetting", new[] { "StoreId" });
            DropIndex("dbo.HomeExpenseWeeklySalesSetting", new[] { "DepartmentId" });
            DropIndex("dbo.HomeExpenseWeeklySalesSetting", new[] { "VendorId" });
            CreateTable(
                "dbo.HomeSome_Expenses",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        StoreID = c.Int(nullable: false),
                        PaymentFees = c.Decimal(precision: 18, scale: 2),
                        DeliveryCost = c.Decimal(precision: 18, scale: 2),
                        TipsPaid = c.Decimal(precision: 18, scale: 2),
                        CreatedOn = c.DateTime(),
                        PaymentListID = c.Int(),
                        DeliveryListID = c.Int(),
                        TipListID = c.Int(),
                        ExpenseDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.StoreMaster", t => t.StoreID)
                .Index(t => t.StoreID);
            
            AlterColumn("dbo.HomeExpenseWeeklySalesSetting", "StoreId", c => c.Int(nullable: false));
            AlterColumn("dbo.HomeExpenseWeeklySalesSetting", "DepartmentId", c => c.Int(nullable: false));
            AlterColumn("dbo.HomeExpenseWeeklySalesSetting", "VendorId", c => c.Int(nullable: false));
            CreateIndex("dbo.HomeExpenseWeeklySalesSetting", "StoreId");
            CreateIndex("dbo.HomeExpenseWeeklySalesSetting", "DepartmentId");
            CreateIndex("dbo.HomeExpenseWeeklySalesSetting", "VendorId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HomeSome_Expenses", "StoreID", "dbo.StoreMaster");
            DropIndex("dbo.HomeSome_Expenses", new[] { "StoreID" });
            DropIndex("dbo.HomeExpenseWeeklySalesSetting", new[] { "VendorId" });
            DropIndex("dbo.HomeExpenseWeeklySalesSetting", new[] { "DepartmentId" });
            DropIndex("dbo.HomeExpenseWeeklySalesSetting", new[] { "StoreId" });
            AlterColumn("dbo.HomeExpenseWeeklySalesSetting", "VendorId", c => c.Int());
            AlterColumn("dbo.HomeExpenseWeeklySalesSetting", "DepartmentId", c => c.Int());
            AlterColumn("dbo.HomeExpenseWeeklySalesSetting", "StoreId", c => c.Int());
            DropTable("dbo.HomeSome_Expenses");
            CreateIndex("dbo.HomeExpenseWeeklySalesSetting", "VendorId");
            CreateIndex("dbo.HomeExpenseWeeklySalesSetting", "DepartmentId");
            CreateIndex("dbo.HomeExpenseWeeklySalesSetting", "StoreId");
        }
    }
}
