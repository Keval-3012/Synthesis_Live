namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DailyTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CreditcardDetailsDaily",
                c => new
                    {
                        CreditcardDetailsDailyId = c.Int(nullable: false, identity: true),
                        StoreId = c.Int(nullable: false),
                        ShiftId = c.Int(),
                        Date = c.DateTime(nullable: false),
                        Amount_AMEX = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Amount_Discover = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Amount_Master = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Amount_Visa = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Amount_CCOffline = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StoreTerminalDailylId = c.Int(),
                        SalesActivitySummaryDailyId = c.Int(),
                    })
                .PrimaryKey(t => t.CreditcardDetailsDailyId)
                .ForeignKey("dbo.SalesActivitySummaryDaily", t => t.SalesActivitySummaryDailyId)
                .ForeignKey("dbo.ShiftMaster", t => t.ShiftId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .ForeignKey("dbo.StoreTerminalDaily", t => t.StoreTerminalDailylId)
                .Index(t => t.StoreId)
                .Index(t => t.ShiftId)
                .Index(t => t.StoreTerminalDailylId)
                .Index(t => t.SalesActivitySummaryDailyId);
            
            CreateTable(
                "dbo.SalesActivitySummaryDaily",
                c => new
                    {
                        SalesActivitySummaryDailyId = c.Int(nullable: false, identity: true),
                        StoreId = c.Int(),
                        StoreTerminalDailylId = c.Int(),
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
                        StoreTerminal_StoreTerminalId = c.Int(),
                    })
                .PrimaryKey(t => t.SalesActivitySummaryDailyId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .ForeignKey("dbo.StoreTerminalDaily", t => t.StoreTerminalDailylId)
                .ForeignKey("dbo.StoreTerminal", t => t.StoreTerminal_StoreTerminalId)
                .Index(t => t.StoreId)
                .Index(t => t.StoreTerminalDailylId)
                .Index(t => t.StoreTerminal_StoreTerminalId);
            
            CreateTable(
                "dbo.DepartmentNetSalesDaily",
                c => new
                    {
                        DepartmentNetSalesDailyId = c.Int(nullable: false, identity: true),
                        SalesActivitySummaryDailyId = c.Int(),
                        Title = c.String(maxLength: 500),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedOn = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DepartmentNetSalesDailyId)
                .ForeignKey("dbo.SalesActivitySummaryDaily", t => t.SalesActivitySummaryDailyId)
                .Index(t => t.SalesActivitySummaryDailyId);
            
            CreateTable(
                "dbo.PaidOutDaily",
                c => new
                    {
                        PaidOutDailyId = c.Int(nullable: false, identity: true),
                        SalesActivitySummaryDailyId = c.Int(),
                        Title = c.String(maxLength: 500),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedOn = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        SalesActivitySummary_SalesActivitySummaryId = c.Int(),
                    })
                .PrimaryKey(t => t.PaidOutDailyId)
                .ForeignKey("dbo.SalesActivitySummaryDaily", t => t.SalesActivitySummaryDailyId)
                .ForeignKey("dbo.SalesActivitySummary", t => t.SalesActivitySummary_SalesActivitySummaryId)
                .Index(t => t.SalesActivitySummaryDailyId)
                .Index(t => t.SalesActivitySummary_SalesActivitySummaryId);
            
            CreateTable(
                "dbo.StoreTerminalDaily",
                c => new
                    {
                        StoreTerminalDailylId = c.Int(nullable: false, identity: true),
                        StoreId = c.Int(),
                        TerminalDailyId = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.StoreTerminalDailylId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .ForeignKey("dbo.TerminalDailyMaster", t => t.TerminalDailyId)
                .Index(t => t.StoreId)
                .Index(t => t.TerminalDailyId);
            
            CreateTable(
                "dbo.TerminalDailyMaster",
                c => new
                    {
                        TerminalDailyId = c.Int(nullable: false, identity: true),
                        TerminalName = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.TerminalDailyId);
            
            CreateTable(
                "dbo.TenderInDrawerDaily",
                c => new
                    {
                        TenderInDrawerDailyId = c.Int(nullable: false, identity: true),
                        SalesActivitySummaryDailyId = c.Int(),
                        Title = c.String(maxLength: 500),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedOn = c.DateTime(),
                        IsManual = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TenderInDrawerDailyId)
                .ForeignKey("dbo.SalesActivitySummaryDaily", t => t.SalesActivitySummaryDailyId)
                .Index(t => t.SalesActivitySummaryDailyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PaidOutDaily", "SalesActivitySummary_SalesActivitySummaryId", "dbo.SalesActivitySummary");
            DropForeignKey("dbo.SalesActivitySummaryDaily", "StoreTerminal_StoreTerminalId", "dbo.StoreTerminal");
            DropForeignKey("dbo.CreditcardDetailsDaily", "StoreTerminalDailylId", "dbo.StoreTerminalDaily");
            DropForeignKey("dbo.CreditcardDetailsDaily", "StoreId", "dbo.StoreMaster");
            //DropForeignKey("dbo.CreditcardDetailsDaily", "ShiftId", "dbo.ShiftMaster");
            DropForeignKey("dbo.CreditcardDetailsDaily", "SalesActivitySummaryDailyId", "dbo.SalesActivitySummaryDaily");
            DropForeignKey("dbo.TenderInDrawerDaily", "SalesActivitySummaryDailyId", "dbo.SalesActivitySummaryDaily");
            DropForeignKey("dbo.SalesActivitySummaryDaily", "StoreTerminalDailylId", "dbo.StoreTerminalDaily");
            DropForeignKey("dbo.StoreTerminalDaily", "TerminalDailyId", "dbo.TerminalDailyMaster");
            DropForeignKey("dbo.StoreTerminalDaily", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.SalesActivitySummaryDaily", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.PaidOutDaily", "SalesActivitySummaryDailyId", "dbo.SalesActivitySummaryDaily");
            DropForeignKey("dbo.DepartmentNetSalesDaily", "SalesActivitySummaryDailyId", "dbo.SalesActivitySummaryDaily");
            DropIndex("dbo.TenderInDrawerDaily", new[] { "SalesActivitySummaryDailyId" });
            DropIndex("dbo.StoreTerminalDaily", new[] { "TerminalDailyId" });
            DropIndex("dbo.StoreTerminalDaily", new[] { "StoreId" });
            DropIndex("dbo.PaidOutDaily", new[] { "SalesActivitySummary_SalesActivitySummaryId" });
            DropIndex("dbo.PaidOutDaily", new[] { "SalesActivitySummaryDailyId" });
            DropIndex("dbo.DepartmentNetSalesDaily", new[] { "SalesActivitySummaryDailyId" });
            DropIndex("dbo.SalesActivitySummaryDaily", new[] { "StoreTerminal_StoreTerminalId" });
            DropIndex("dbo.SalesActivitySummaryDaily", new[] { "StoreTerminalDailylId" });
            DropIndex("dbo.SalesActivitySummaryDaily", new[] { "StoreId" });
            DropIndex("dbo.CreditcardDetailsDaily", new[] { "SalesActivitySummaryDailyId" });
            DropIndex("dbo.CreditcardDetailsDaily", new[] { "StoreTerminalDailylId" });
            //DropIndex("dbo.CreditcardDetailsDaily", new[] { "ShiftId" });
            DropIndex("dbo.CreditcardDetailsDaily", new[] { "StoreId" });
            DropTable("dbo.TenderInDrawerDaily");
            DropTable("dbo.TerminalDailyMaster");
            DropTable("dbo.StoreTerminalDaily");
            DropTable("dbo.PaidOutDaily");
            DropTable("dbo.DepartmentNetSalesDaily");
            DropTable("dbo.SalesActivitySummaryDaily");
            DropTable("dbo.CreditcardDetailsDaily");
        }
    }
}
