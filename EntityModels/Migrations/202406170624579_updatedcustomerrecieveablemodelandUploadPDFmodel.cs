namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedcustomerrecieveablemodelandUploadPDFmodel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InvoiceAutomation", "InvoiceApprovedDate", c => c.DateTime());
            AddColumn("dbo.UploadPDFAutomation", "UploadedStatus", c => c.String());
            AddColumn("dbo.UploadPDFAutomation", "QueueId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UploadPDFAutomation", "QueueId");
            DropColumn("dbo.UploadPDFAutomation", "UploadedStatus");
            DropColumn("dbo.InvoiceAutomation", "InvoiceApprovedDate");
        }
    }
}
