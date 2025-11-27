namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Migrations;
    
    public partial class AddExpenseCheck : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExpenseCheck",
                c => new
                    {
                        ExpenseCheckId = c.Int(nullable: false, identity: true),
                        BankAccountId = c.Int(),
                        PaymentType = c.Int(nullable: false),
                        VendorId = c.Int(),
                        StoreId = c.Int(),
                        TotalAmt = c.Decimal(precision: 18, scale: 2),
                        TXNId = c.String(maxLength: 50),
                        SyncToken = c.String(),
                        CreateTime = c.DateTime(),
                        LastUpdatedTime = c.DateTime(),
                        CreateOn = c.DateTime(nullable: false),
                        DocNumber = c.String(),
                        TxnDate = c.String(),
                        CheckNumber = c.String(),
                        Memo = c.String(),
                        QBType = c.String(),
                        RefType = c.String(),
                })
                .PrimaryKey(t => t.ExpenseCheckId)
                .ForeignKey("dbo.DepartmentMaster", t => t.BankAccountId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .ForeignKey("dbo.VendorMaster", t => t.VendorId)
                .Index(t => t.BankAccountId)
                .Index(t => t.VendorId)
                .Index(t => t.StoreId);
            
            CreateTable(
                "dbo.ExpenseCheckDetail",
                c => new
                    {
                        ExpenseCheckDetailId = c.Int(nullable: false, identity: true),
                        ExpenseCheckId = c.Int(nullable: false),
                        DepartmentId = c.Int(),
                        Amount = c.Decimal(precision: 18, scale: 2),
                        CustomerId = c.Int(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ExpenseCheckDetailId)
                .ForeignKey("dbo.CustomerMaster", t => t.CustomerId)
                .ForeignKey("dbo.DepartmentMaster", t => t.DepartmentId)
                .ForeignKey("dbo.ExpenseCheck", t => t.ExpenseCheckId, cascadeDelete: true)
                .Index(t => t.ExpenseCheckId)
                .Index(t => t.DepartmentId)
                .Index(t => t.CustomerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ExpenseCheck", "VendorId", "dbo.VendorMaster");
            DropForeignKey("dbo.ExpenseCheck", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.ExpenseCheckDetail", "ExpenseCheckId", "dbo.ExpenseCheck");
            DropForeignKey("dbo.ExpenseCheckDetail", "DepartmentId", "dbo.DepartmentMaster");
            DropForeignKey("dbo.ExpenseCheckDetail", "CustomerId", "dbo.CustomerMaster");
            DropForeignKey("dbo.ExpenseCheck", "BankAccountId", "dbo.DepartmentMaster");
            DropIndex("dbo.ExpenseCheckDetail", new[] { "CustomerId" });
            DropIndex("dbo.ExpenseCheckDetail", new[] { "DepartmentId" });
            DropIndex("dbo.ExpenseCheckDetail", new[] { "ExpenseCheckId" });
            DropIndex("dbo.ExpenseCheck", new[] { "StoreId" });
            DropIndex("dbo.ExpenseCheck", new[] { "VendorId" });
            DropIndex("dbo.ExpenseCheck", new[] { "BankAccountId" });
            DropTable("dbo.ExpenseCheckDetail");
            DropTable("dbo.ExpenseCheck");
        }
    }
}
