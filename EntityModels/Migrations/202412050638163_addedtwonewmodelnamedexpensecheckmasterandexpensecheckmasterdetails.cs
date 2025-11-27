namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedtwonewmodelnamedexpensecheckmasterandexpensecheckmasterdetails : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExpenseCheckMaster",
                c => new
                    {
                        ExpenseCheckMasterId = c.Int(nullable: false, identity: true),
                        BankAccountId = c.Int(),
                        PaymentMethodId = c.Int(),
                        VendorId = c.Int(),
                        StoreId = c.Int(),
                        TotalAmt = c.Decimal(precision: 18, scale: 2),
                        DocNumber = c.String(),
                        Memo = c.String(maxLength: 700),
                        RefType = c.String(),
                        FileName = c.String(),
                        SyncStatus = c.String(),
                        TXNId = c.String(maxLength: 50),
                        TxnDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ExpenseCheckMasterId)
                .ForeignKey("dbo.DepartmentMaster", t => t.BankAccountId)
                .ForeignKey("dbo.ExpensePaymentMethodMaster", t => t.PaymentMethodId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .ForeignKey("dbo.VendorMaster", t => t.VendorId)
                .Index(t => t.BankAccountId)
                .Index(t => t.PaymentMethodId)
                .Index(t => t.VendorId)
                .Index(t => t.StoreId);
            
            CreateTable(
                "dbo.ExpenseCheckMasterDetails",
                c => new
                    {
                        ExpenseCheckMasterDetailId = c.Int(nullable: false, identity: true),
                        ExpenseCheckMasterId = c.Int(nullable: false),
                        DepartmentId = c.Int(),
                        Amount = c.Decimal(precision: 18, scale: 2),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ExpenseCheckMasterDetailId)
                .ForeignKey("dbo.DepartmentMaster", t => t.DepartmentId)
                .ForeignKey("dbo.ExpenseCheckMaster", t => t.ExpenseCheckMasterId, cascadeDelete: true)
                .Index(t => t.ExpenseCheckMasterId)
                .Index(t => t.DepartmentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ExpenseCheckMaster", "VendorId", "dbo.VendorMaster");
            DropForeignKey("dbo.ExpenseCheckMaster", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.ExpenseCheckMaster", "PaymentMethodId", "dbo.ExpensePaymentMethodMaster");
            DropForeignKey("dbo.ExpenseCheckMasterDetails", "ExpenseCheckMasterId", "dbo.ExpenseCheckMaster");
            DropForeignKey("dbo.ExpenseCheckMasterDetails", "DepartmentId", "dbo.DepartmentMaster");
            DropForeignKey("dbo.ExpenseCheckMaster", "BankAccountId", "dbo.DepartmentMaster");
            DropIndex("dbo.ExpenseCheckMasterDetails", new[] { "DepartmentId" });
            DropIndex("dbo.ExpenseCheckMasterDetails", new[] { "ExpenseCheckMasterId" });
            DropIndex("dbo.ExpenseCheckMaster", new[] { "StoreId" });
            DropIndex("dbo.ExpenseCheckMaster", new[] { "VendorId" });
            DropIndex("dbo.ExpenseCheckMaster", new[] { "PaymentMethodId" });
            DropIndex("dbo.ExpenseCheckMaster", new[] { "BankAccountId" });
            DropTable("dbo.ExpenseCheckMasterDetails");
            DropTable("dbo.ExpenseCheckMaster");
        }
    }
}
