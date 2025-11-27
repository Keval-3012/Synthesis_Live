namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CashPaidoutInvoice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CashPaidoutInvoice", "SalesActivitySummaryId", c => c.Int());
            CreateIndex("dbo.CashPaidoutInvoice", "SalesActivitySummaryId");
            AddForeignKey("dbo.CashPaidoutInvoice", "SalesActivitySummaryId", "dbo.SalesActivitySummary", "SalesActivitySummaryId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CashPaidoutInvoice", "SalesActivitySummaryId", "dbo.SalesActivitySummary");
            DropIndex("dbo.CashPaidoutInvoice", new[] { "SalesActivitySummaryId" });
            DropColumn("dbo.CashPaidoutInvoice", "SalesActivitySummaryId");
        }
    }
}
