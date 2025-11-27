namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatethependingmigrationforInvoiceAutomation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InvoiceAutomation", "VendorName", c => c.String());
            AddColumn("dbo.InvoiceAutomation", "FlagDeleted", c => c.Boolean(nullable: false));
            DropColumn("dbo.UploadPDFAutomation", "ReadyForProcess");
            DropColumn("dbo.UploadPDFAutomation", "ReferenceId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UploadPDFAutomation", "ReferenceId", c => c.Int());
            AddColumn("dbo.UploadPDFAutomation", "ReadyForProcess", c => c.Int(nullable: false));
            DropColumn("dbo.InvoiceAutomation", "FlagDeleted");
            DropColumn("dbo.InvoiceAutomation", "VendorName");
        }
    }
}
