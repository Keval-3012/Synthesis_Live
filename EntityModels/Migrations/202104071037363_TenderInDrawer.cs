namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TenderInDrawer : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TenderInDrawer",
                c => new
                    {
                        TenderInDrawerId = c.Int(nullable: false, identity: true),
                        SalesActivitySummaryId = c.Int(nullable: false),
                        Title = c.String(maxLength: 500),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        IsManual = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TenderInDrawerId)
                .ForeignKey("dbo.SalesActivitySummary", t => t.SalesActivitySummaryId)
                .Index(t => t.SalesActivitySummaryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TenderInDrawer", "SalesActivitySummaryId", "dbo.SalesActivitySummary");
            DropIndex("dbo.TenderInDrawer", new[] { "SalesActivitySummaryId" });
            DropTable("dbo.TenderInDrawer");
        }
    }
}
