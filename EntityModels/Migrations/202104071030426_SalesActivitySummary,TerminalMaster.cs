namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SalesActivitySummaryTerminalMaster : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SalesActivitySummary",
                c => new
                    {
                        SalesActivitySummaryId = c.Int(nullable: false, identity: true),
                        StoreId = c.Int(nullable: false),
                        TerminalId = c.Int(nullable: false),
                        TransactionStartTime = c.DateTime(),
                        TransactionEndTime = c.DateTime(),
                        CustomerCount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetSalesWithTax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalTaxCollected = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AverageSale = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FileName = c.String(maxLength: 500),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        ShiftName = c.String(maxLength: 100),
                        CashierName = c.String(maxLength: 200),
                        StartDate = c.DateTime(),
                        CashierNegative = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Notes = c.String(),
                    })
                .PrimaryKey(t => t.SalesActivitySummaryId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .ForeignKey("dbo.TerminalMaster", t => t.TerminalId)
                .Index(t => t.StoreId)
                .Index(t => t.TerminalId);
            
            CreateTable(
                "dbo.TerminalMaster",
                c => new
                    {
                        TerminalId = c.Int(nullable: false, identity: true),
                        StoreId = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.TerminalId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .Index(t => t.StoreId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SalesActivitySummary", "TerminalId", "dbo.TerminalMaster");
            DropForeignKey("dbo.TerminalMaster", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.SalesActivitySummary", "StoreId", "dbo.StoreMaster");
            DropIndex("dbo.TerminalMaster", new[] { "StoreId" });
            DropIndex("dbo.SalesActivitySummary", new[] { "TerminalId" });
            DropIndex("dbo.SalesActivitySummary", new[] { "StoreId" });
            DropTable("dbo.TerminalMaster");
            DropTable("dbo.SalesActivitySummary");
        }
    }
}
