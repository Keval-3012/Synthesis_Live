namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SalesActivityDailyNewFieldsAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SalesActivitySummaryDaily", "AllVoids", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.SalesActivitySummaryDaily", "ItemCorrects", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.SalesActivitySummaryDaily", "ItemReturns", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SalesActivitySummaryDaily", "ItemReturns");
            DropColumn("dbo.SalesActivitySummaryDaily", "ItemCorrects");
            DropColumn("dbo.SalesActivitySummaryDaily", "AllVoids");
        }
    }
}
