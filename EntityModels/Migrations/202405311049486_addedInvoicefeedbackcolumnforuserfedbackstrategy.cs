namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedInvoicefeedbackcolumnforuserfedbackstrategy : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invoice", "InvoiceReview", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Invoice", "InvoiceReview");
        }
    }
}
