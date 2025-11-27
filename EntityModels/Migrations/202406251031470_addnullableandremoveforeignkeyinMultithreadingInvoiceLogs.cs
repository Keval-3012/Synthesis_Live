namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnullableandremoveforeignkeyinMultithreadingInvoiceLogs : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MultithreadingInvoiceLogs", "UploadPdfAutomationId", "dbo.UploadPDFAutomation");
            DropIndex("dbo.MultithreadingInvoiceLogs", new[] { "UploadPdfAutomationId" });
            AlterColumn("dbo.MultithreadingInvoiceLogs", "UploadPdfAutomationId", c => c.Int());
            AlterColumn("dbo.MultithreadingInvoiceLogs", "UploadStartFinishDate", c => c.DateTime());
            AlterColumn("dbo.MultithreadingInvoiceLogs", "MovetoConsoleDate", c => c.DateTime());
            AlterColumn("dbo.MultithreadingInvoiceLogs", "SentAWSExtractDate", c => c.DateTime());
            AlterColumn("dbo.MultithreadingInvoiceLogs", "ReceivedAWSExtractDate", c => c.DateTime());
            AlterColumn("dbo.MultithreadingInvoiceLogs", "JSONtoCHATGPTDate", c => c.DateTime());
            AlterColumn("dbo.MultithreadingInvoiceLogs", "ResponseCHATGPTDate", c => c.DateTime());
            AlterColumn("dbo.MultithreadingInvoiceLogs", "InvoiceApproveStatusDate", c => c.DateTime());
            AlterColumn("dbo.MultithreadingInvoiceLogs", "InvoiceCreatedStatusDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MultithreadingInvoiceLogs", "InvoiceCreatedStatusDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.MultithreadingInvoiceLogs", "InvoiceApproveStatusDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.MultithreadingInvoiceLogs", "ResponseCHATGPTDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.MultithreadingInvoiceLogs", "JSONtoCHATGPTDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.MultithreadingInvoiceLogs", "ReceivedAWSExtractDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.MultithreadingInvoiceLogs", "SentAWSExtractDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.MultithreadingInvoiceLogs", "MovetoConsoleDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.MultithreadingInvoiceLogs", "UploadStartFinishDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.MultithreadingInvoiceLogs", "UploadPdfAutomationId", c => c.Int(nullable: false));
            CreateIndex("dbo.MultithreadingInvoiceLogs", "UploadPdfAutomationId");
            AddForeignKey("dbo.MultithreadingInvoiceLogs", "UploadPdfAutomationId", "dbo.UploadPDFAutomation", "UploadPdfAutomationId", cascadeDelete: true);
        }
    }
}
