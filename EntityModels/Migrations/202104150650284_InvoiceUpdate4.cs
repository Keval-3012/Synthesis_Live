namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InvoiceUpdate4 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Invoice", "DiscountPercent", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Invoice", "DiscountAmount", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Invoice", "DiscountAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Invoice", "DiscountPercent", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
