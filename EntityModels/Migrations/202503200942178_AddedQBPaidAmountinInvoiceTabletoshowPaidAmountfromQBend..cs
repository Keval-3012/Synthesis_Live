namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedQBPaidAmountinInvoiceTabletoshowPaidAmountfromQBend : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invoice", "QbCheckAmount", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Invoice", "QbCheckAmount");
        }
    }
}
