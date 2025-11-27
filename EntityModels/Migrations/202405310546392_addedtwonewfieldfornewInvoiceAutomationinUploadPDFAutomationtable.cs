namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedtwonewfieldfornewInvoiceAutomationinUploadPDFAutomationtable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UploadPDFAutomation", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.UploadPDFAutomation", "Is_Processing_Enabled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UploadPDFAutomation", "Is_Processing_Enabled");
            DropColumn("dbo.UploadPDFAutomation", "IsDeleted");
        }
    }
}
