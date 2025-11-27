namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InvoiceandOtherRelatedForms : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InvoiceDepartmentDetails",
                c => new
                    {
                        InvoiceDepartmentId = c.Int(nullable: false, identity: true),
                        InvoiceId = c.Int(nullable: false),
                        DepartmentId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.InvoiceDepartmentId)
                .ForeignKey("dbo.DepartmentMaster", t => t.DepartmentId)
                .ForeignKey("dbo.Invoices", t => t.InvoiceId, cascadeDelete: true)
                .Index(t => t.InvoiceId)
                .Index(t => t.DepartmentId);
            
            CreateTable(
                "dbo.Invoices",
                c => new
                    {
                        InvoiceId = c.Int(nullable: false, identity: true),
                        StoreId = c.Int(nullable: false),
                        InvoiceTypeId = c.Int(nullable: false),
                        PaymentTypeId = c.Int(nullable: false),
                        VendorId = c.Int(nullable: false),
                        InvoiceDate = c.DateTime(nullable: false),
                        InvoiceNumber = c.String(maxLength: 100),
                        Note = c.String(maxLength: 2000),
                        UploadInvoice = c.String(maxLength: 100),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StatusValue = c.Int(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ApproveRejectBy = c.Int(),
                        ApproveRejectDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        TXNId = c.String(maxLength: 50),
                        IsSync = c.Int(),
                        SyncDate = c.DateTime(),
                        NotificationColor = c.Boolean(nullable: false),
                        QBTransfer = c.Boolean(nullable: false),
                        DiscountTypeId = c.Int(),
                        DiscountPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DiscountAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RefInvoiceId = c.Int(),
                        Source = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.InvoiceId)
                .ForeignKey("dbo.DiscountTypeMasters", t => t.DiscountTypeId)
                .ForeignKey("dbo.InvoiceTypeMaster", t => t.InvoiceTypeId)
                .ForeignKey("dbo.PaymentTypeMaster", t => t.PaymentTypeId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .ForeignKey("dbo.VendorMaster", t => t.VendorId)
                .Index(t => t.StoreId)
                .Index(t => t.InvoiceTypeId)
                .Index(t => t.PaymentTypeId)
                .Index(t => t.VendorId)
                .Index(t => t.DiscountTypeId);
            
            CreateTable(
                "dbo.DiscountTypeMasters",
                c => new
                    {
                        DiscountTypeId = c.Int(nullable: false, identity: true),
                        DiscountType = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.DiscountTypeId);
            
            CreateTable(
                "dbo.InvoiceTypeMaster",
                c => new
                    {
                        InvoiceTypeId = c.Int(nullable: false, identity: true),
                        InvoiceType = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.InvoiceTypeId);
            
            CreateTable(
                "dbo.PaymentTypeMaster",
                c => new
                    {
                        PaymentTypeId = c.Int(nullable: false, identity: true),
                        PaymentType = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.PaymentTypeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Invoices", "VendorId", "dbo.VendorMaster");
            DropForeignKey("dbo.Invoices", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.Invoices", "PaymentTypeId", "dbo.PaymentTypeMaster");
            DropForeignKey("dbo.Invoices", "InvoiceTypeId", "dbo.InvoiceTypeMaster");
            DropForeignKey("dbo.InvoiceDepartmentDetails", "InvoiceId", "dbo.Invoices");
            DropForeignKey("dbo.Invoices", "DiscountTypeId", "dbo.DiscountTypeMasters");
            DropForeignKey("dbo.InvoiceDepartmentDetails", "DepartmentId", "dbo.DepartmentMaster");
            DropIndex("dbo.Invoices", new[] { "DiscountTypeId" });
            DropIndex("dbo.Invoices", new[] { "VendorId" });
            DropIndex("dbo.Invoices", new[] { "PaymentTypeId" });
            DropIndex("dbo.Invoices", new[] { "InvoiceTypeId" });
            DropIndex("dbo.Invoices", new[] { "StoreId" });
            DropIndex("dbo.InvoiceDepartmentDetails", new[] { "DepartmentId" });
            DropIndex("dbo.InvoiceDepartmentDetails", new[] { "InvoiceId" });
            DropTable("dbo.PaymentTypeMaster");
            DropTable("dbo.InvoiceTypeMaster");
            DropTable("dbo.DiscountTypeMasters");
            DropTable("dbo.Invoices");
            DropTable("dbo.InvoiceDepartmentDetails");
        }
    }
}
