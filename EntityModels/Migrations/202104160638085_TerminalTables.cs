namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TerminalTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CashPaidoutInvoice",
                c => new
                    {
                        CashPaidoutInvoiceId = c.Int(nullable: false, identity: true),
                        InvoiceId = c.Int(nullable: false),
                        StoreId = c.Int(nullable: false),
                        TerminalId = c.Int(),
                        ShiftId = c.Int(),
                        PaidOutId = c.Int(),
                        CreatedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.CashPaidoutInvoiceId)
                .ForeignKey("dbo.Invoice", t => t.InvoiceId)
                .ForeignKey("dbo.PaidOut", t => t.PaidOutId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .ForeignKey("dbo.TerminalMaster", t => t.TerminalId)
                .Index(t => t.InvoiceId)
                .Index(t => t.StoreId)
                .Index(t => t.TerminalId)
                .Index(t => t.PaidOutId);
            
            CreateTable(
                "dbo.OtherDeposit",
                c => new
                    {
                        OtherDepositId = c.Int(nullable: false, identity: true),
                        StoreId = c.Int(nullable: false),
                        Name = c.String(maxLength: 50),
                        PaymentMethodId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OptionId = c.Int(),
                        VendorId = c.Int(),
                        TerminalId = c.Int(),
                        ShiftId = c.Int(),
                        UploadDocument = c.String(maxLength: 200),
                        SalesActivitySummaryId = c.Int(),
                        CreateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.OtherDepositId)
                .ForeignKey("dbo.OptionMaster", t => t.OptionId)
                .ForeignKey("dbo.PaymentMethodMaster", t => t.PaymentMethodId)
                .ForeignKey("dbo.SalesActivitySummary", t => t.SalesActivitySummaryId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .ForeignKey("dbo.TerminalMaster", t => t.TerminalId)
                .ForeignKey("dbo.VendorMaster", t => t.VendorId)
                .Index(t => t.StoreId)
                .Index(t => t.PaymentMethodId)
                .Index(t => t.OptionId)
                .Index(t => t.VendorId)
                .Index(t => t.TerminalId)
                .Index(t => t.SalesActivitySummaryId);
            
            CreateTable(
                "dbo.OptionMaster",
                c => new
                    {
                        OptionId = c.Int(nullable: false, identity: true),
                        OptionName = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.OptionId);
            
            CreateTable(
                "dbo.PaymentMethodMaster",
                c => new
                    {
                        PaymentMethodId = c.Int(nullable: false, identity: true),
                        PaymentMethod = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.PaymentMethodId);
            
            CreateTable(
                "dbo.CreditcardDetails",
                c => new
                    {
                        CreditcardDetailId = c.Int(nullable: false, identity: true),
                        StoreId = c.Int(nullable: false),
                        ShiftId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Amount_AMEX = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Amount_Discover = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Amount_Master = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Amount_Visa = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ShiftName = c.String(),
                        TerminalId = c.Int(),
                        SalesActivitySummaryId = c.Int(),
                    })
                .PrimaryKey(t => t.CreditcardDetailId)
                .ForeignKey("dbo.SalesActivitySummary", t => t.SalesActivitySummaryId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .ForeignKey("dbo.TerminalMaster", t => t.TerminalId)
                .Index(t => t.StoreId)
                .Index(t => t.TerminalId)
                .Index(t => t.SalesActivitySummaryId);
            
            CreateTable(
                "dbo.PaidOut",
                c => new
                    {
                        PaidOutId = c.Int(nullable: false, identity: true),
                        SalesActivitySummaryId = c.Int(nullable: false),
                        Title = c.String(maxLength: 500),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(nullable: false),
                        CreatedOn = c.DateTime(),
                        ModifiedOn = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.PaidOutId)
                .ForeignKey("dbo.SalesActivitySummary", t => t.SalesActivitySummaryId)
                .Index(t => t.SalesActivitySummaryId);
            
            CreateTable(
                "dbo.PaidOutSettlement",
                c => new
                    {
                        PaidOutSettlementId = c.Int(nullable: false, identity: true),
                        SalesActivitySummaryId = c.Int(nullable: false),
                        Title = c.String(maxLength: 500),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedBy = c.Int(nullable: false),
                        CreatedOn = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.PaidOutSettlementId)
                .ForeignKey("dbo.SalesActivitySummary", t => t.SalesActivitySummaryId)
                .Index(t => t.SalesActivitySummaryId);
            
            AddColumn("dbo.TerminalMaster", "Terminal", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CashPaidoutInvoice", "TerminalId", "dbo.TerminalMaster");
            DropForeignKey("dbo.CashPaidoutInvoice", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.CashPaidoutInvoice", "PaidOutId", "dbo.PaidOut");
            DropForeignKey("dbo.CashPaidoutInvoice", "InvoiceId", "dbo.Invoice");
            DropForeignKey("dbo.OtherDeposit", "VendorId", "dbo.VendorMaster");
            DropForeignKey("dbo.OtherDeposit", "TerminalId", "dbo.TerminalMaster");
            DropForeignKey("dbo.OtherDeposit", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.OtherDeposit", "SalesActivitySummaryId", "dbo.SalesActivitySummary");
            DropForeignKey("dbo.PaidOutSettlement", "SalesActivitySummaryId", "dbo.SalesActivitySummary");
            DropForeignKey("dbo.PaidOut", "SalesActivitySummaryId", "dbo.SalesActivitySummary");
            DropForeignKey("dbo.CreditcardDetails", "TerminalId", "dbo.TerminalMaster");
            DropForeignKey("dbo.CreditcardDetails", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.CreditcardDetails", "SalesActivitySummaryId", "dbo.SalesActivitySummary");
            DropForeignKey("dbo.OtherDeposit", "PaymentMethodId", "dbo.PaymentMethodMaster");
            DropForeignKey("dbo.OtherDeposit", "OptionId", "dbo.OptionMaster");
            DropIndex("dbo.PaidOutSettlement", new[] { "SalesActivitySummaryId" });
            DropIndex("dbo.PaidOut", new[] { "SalesActivitySummaryId" });
            DropIndex("dbo.CreditcardDetails", new[] { "SalesActivitySummaryId" });
            DropIndex("dbo.CreditcardDetails", new[] { "TerminalId" });
            DropIndex("dbo.CreditcardDetails", new[] { "StoreId" });
            DropIndex("dbo.OtherDeposit", new[] { "SalesActivitySummaryId" });
            DropIndex("dbo.OtherDeposit", new[] { "TerminalId" });
            DropIndex("dbo.OtherDeposit", new[] { "VendorId" });
            DropIndex("dbo.OtherDeposit", new[] { "OptionId" });
            DropIndex("dbo.OtherDeposit", new[] { "PaymentMethodId" });
            DropIndex("dbo.OtherDeposit", new[] { "StoreId" });
            DropIndex("dbo.CashPaidoutInvoice", new[] { "PaidOutId" });
            DropIndex("dbo.CashPaidoutInvoice", new[] { "TerminalId" });
            DropIndex("dbo.CashPaidoutInvoice", new[] { "StoreId" });
            DropIndex("dbo.CashPaidoutInvoice", new[] { "InvoiceId" });
            DropColumn("dbo.TerminalMaster", "Terminal");
            DropTable("dbo.PaidOutSettlement");
            DropTable("dbo.PaidOut");
            DropTable("dbo.CreditcardDetails");
            DropTable("dbo.PaymentMethodMaster");
            DropTable("dbo.OptionMaster");
            DropTable("dbo.OtherDeposit");
            DropTable("dbo.CashPaidoutInvoice");
        }
    }
}
