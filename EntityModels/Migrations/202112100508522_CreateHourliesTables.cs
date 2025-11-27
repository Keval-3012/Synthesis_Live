namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateHourliesTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CreditcardDetailsHourly",
                c => new
                    {
                        CreditcardDetailsHourlyId = c.Int(nullable: false, identity: true),
                        StoreId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Amount_AMEX = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Amount_Discover = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Amount_Master = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Amount_Visa = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Amount_CCOffline = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StoreTerminalHourlylId = c.Int(),
                        SalesActivitySummaryHourlyId = c.Int(),
                        HSequence = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CreditcardDetailsHourlyId)
                .ForeignKey("dbo.SalesActivitySummaryHourly", t => t.SalesActivitySummaryHourlyId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .ForeignKey("dbo.StoreTerminalHourly", t => t.StoreTerminalHourlylId)
                .Index(t => t.StoreId)
                .Index(t => t.StoreTerminalHourlylId)
                .Index(t => t.SalesActivitySummaryHourlyId);
            
            CreateTable(
                "dbo.SalesActivitySummaryHourly",
                c => new
                    {
                        SalesActivitySummaryHourlyId = c.Int(nullable: false, identity: true),
                        StoreId = c.Int(),
                        StoreTerminalHourlylId = c.Int(),
                        TransactionStartTime = c.DateTime(),
                        TransactionEndTime = c.DateTime(),
                        CustomerCount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetSalesWithTax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalTaxCollected = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AverageSale = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FileName = c.String(maxLength: 500),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        CashierName = c.String(maxLength: 200),
                        StartDate = c.DateTime(),
                        CashierNegative = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Notes = c.String(),
                        AllVoids = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ItemCorrects = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ItemReturns = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HSequence = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SalesActivitySummaryHourlyId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .ForeignKey("dbo.StoreTerminalHourly", t => t.StoreTerminalHourlylId)
                .Index(t => t.StoreId)
                .Index(t => t.StoreTerminalHourlylId);
            
            CreateTable(
                "dbo.DepartmentNetSalesHourly",
                c => new
                    {
                        DepartmentNetSalesHourlyId = c.Int(nullable: false, identity: true),
                        SalesActivitySummaryHourlyId = c.Int(),
                        Title = c.String(maxLength: 500),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedOn = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        HSequence = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DepartmentNetSalesHourlyId)
                .ForeignKey("dbo.SalesActivitySummaryHourly", t => t.SalesActivitySummaryHourlyId)
                .Index(t => t.SalesActivitySummaryHourlyId);
            
            CreateTable(
                "dbo.PaidOutHourly",
                c => new
                    {
                        PaidOutHourlyId = c.Int(nullable: false, identity: true),
                        SalesActivitySummaryHourlyId = c.Int(),
                        Title = c.String(maxLength: 500),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedOn = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        HSequence = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PaidOutHourlyId)
                .ForeignKey("dbo.SalesActivitySummaryHourly", t => t.SalesActivitySummaryHourlyId)
                .Index(t => t.SalesActivitySummaryHourlyId);
            
            CreateTable(
                "dbo.StoreTerminalHourly",
                c => new
                    {
                        StoreTerminalHourlylId = c.Int(nullable: false, identity: true),
                        StoreId = c.Int(),
                        TerminalHourlyId = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        HSequence = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.StoreTerminalHourlylId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .ForeignKey("dbo.TerminalHourlyMaster", t => t.TerminalHourlyId)
                .Index(t => t.StoreId)
                .Index(t => t.TerminalHourlyId);
            
            CreateTable(
                "dbo.TerminalHourlyMaster",
                c => new
                    {
                        TerminalHourlyId = c.Int(nullable: false, identity: true),
                        TerminalName = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.TerminalHourlyId);
            
            CreateTable(
                "dbo.TenderInDrawerHourly",
                c => new
                    {
                        TenderInDrawerHourlyId = c.Int(nullable: false, identity: true),
                        SalesActivitySummaryHourlyId = c.Int(),
                        Title = c.String(maxLength: 500),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedOn = c.DateTime(),
                        IsManual = c.Int(nullable: false),
                        HSequence = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TenderInDrawerHourlyId)
                .ForeignKey("dbo.SalesActivitySummaryHourly", t => t.SalesActivitySummaryHourlyId)
                .Index(t => t.SalesActivitySummaryHourlyId);
            
            AddColumn("dbo.StorewisePDFUpload", "HSequence", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CreditcardDetailsHourly", "StoreTerminalHourlylId", "dbo.StoreTerminalHourly");
            DropForeignKey("dbo.CreditcardDetailsHourly", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.CreditcardDetailsHourly", "SalesActivitySummaryHourlyId", "dbo.SalesActivitySummaryHourly");
            DropForeignKey("dbo.TenderInDrawerHourly", "SalesActivitySummaryHourlyId", "dbo.SalesActivitySummaryHourly");
            DropForeignKey("dbo.SalesActivitySummaryHourly", "StoreTerminalHourlylId", "dbo.StoreTerminalHourly");
            DropForeignKey("dbo.StoreTerminalHourly", "TerminalHourlyId", "dbo.TerminalHourlyMaster");
            DropForeignKey("dbo.StoreTerminalHourly", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.SalesActivitySummaryHourly", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.PaidOutHourly", "SalesActivitySummaryHourlyId", "dbo.SalesActivitySummaryHourly");
            DropForeignKey("dbo.DepartmentNetSalesHourly", "SalesActivitySummaryHourlyId", "dbo.SalesActivitySummaryHourly");
            DropIndex("dbo.TenderInDrawerHourly", new[] { "SalesActivitySummaryHourlyId" });
            DropIndex("dbo.StoreTerminalHourly", new[] { "TerminalHourlyId" });
            DropIndex("dbo.StoreTerminalHourly", new[] { "StoreId" });
            DropIndex("dbo.PaidOutHourly", new[] { "SalesActivitySummaryHourlyId" });
            DropIndex("dbo.DepartmentNetSalesHourly", new[] { "SalesActivitySummaryHourlyId" });
            DropIndex("dbo.SalesActivitySummaryHourly", new[] { "StoreTerminalHourlylId" });
            DropIndex("dbo.SalesActivitySummaryHourly", new[] { "StoreId" });
            DropIndex("dbo.CreditcardDetailsHourly", new[] { "SalesActivitySummaryHourlyId" });
            DropIndex("dbo.CreditcardDetailsHourly", new[] { "StoreTerminalHourlylId" });
            DropIndex("dbo.CreditcardDetailsHourly", new[] { "StoreId" });
            DropColumn("dbo.StorewisePDFUpload", "HSequence");
            DropTable("dbo.TenderInDrawerHourly");
            DropTable("dbo.TerminalHourlyMaster");
            DropTable("dbo.StoreTerminalHourly");
            DropTable("dbo.PaidOutHourly");
            DropTable("dbo.DepartmentNetSalesHourly");
            DropTable("dbo.SalesActivitySummaryHourly");
            DropTable("dbo.CreditcardDetailsHourly");
        }
    }
}
