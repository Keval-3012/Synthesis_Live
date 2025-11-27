namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SalesActivityTableFieldsAddAndChange : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalesActivitySummaryHourly", "AllVoidsAmt", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.SalesActivitySummaryHourly", "ItemCorrectsAmt", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.SalesActivitySummaryHourly", "ReturnsAmt", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.SalesActivitySummaryHourly", "AllVoids", c => c.Int(nullable: false));
            AlterColumn("dbo.SalesActivitySummaryHourly", "ItemCorrects", c => c.Int(nullable: false));
            AlterColumn("dbo.SalesActivitySummaryHourly", "ItemReturns", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SalesActivitySummaryHourly", "ItemReturns", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.SalesActivitySummaryHourly", "ItemCorrects", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.SalesActivitySummaryHourly", "AllVoids", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.SalesActivitySummaryHourly", "ReturnsAmt");
            DropColumn("dbo.SalesActivitySummaryHourly", "ItemCorrectsAmt");
            DropColumn("dbo.SalesActivitySummaryHourly", "AllVoidsAmt");
        }
    }
}
