namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedInvoicemodelforInvoiceDate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.InvoiceAutomation", "InvoiceDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.InvoiceAutomation", "InvoiceDate", c => c.DateTime(nullable: false));
        }
    }
}
