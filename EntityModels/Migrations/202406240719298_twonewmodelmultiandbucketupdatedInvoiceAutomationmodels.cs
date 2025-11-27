namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class twonewmodelmultiandbucketupdatedInvoiceAutomationmodels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BucketSizeMaster",
                c => new
                    {
                        BucketId = c.Int(nullable: false, identity: true),
                        BucketSize = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BucketId);
            
            CreateTable(
                "dbo.MultithreadingInvoiceLogs",
                c => new
                    {
                        MultithreadingInvoiceLogId = c.Int(nullable: false, identity: true),
                        UploadPdfAutomationId = c.Int(nullable: false),
                        UploadStartFinish = c.String(),
                        UploadStartFinishDate = c.DateTime(nullable: false),
                        MovetoConsole = c.String(),
                        MovetoConsoleDate = c.DateTime(nullable: false),
                        SentAWSExtract = c.String(),
                        SentAWSExtractDate = c.DateTime(nullable: false),
                        ReceivedAWSExtract = c.String(),
                        ReceivedAWSExtractDate = c.DateTime(nullable: false),
                        JSONtoCHATGPT = c.String(),
                        JSONtoCHATGPTDate = c.DateTime(nullable: false),
                        ResponseCHATGPT = c.String(),
                        ResponseCHATGPTDate = c.DateTime(nullable: false),
                        InvoiceApproveStatus = c.String(),
                        InvoiceApproveStatusDate = c.DateTime(nullable: false),
                        InvoiceCreatedStatus = c.String(),
                        InvoiceCreatedStatusDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MultithreadingInvoiceLogId)
                .ForeignKey("dbo.UploadPDFAutomation", t => t.UploadPdfAutomationId, cascadeDelete: true)
                .Index(t => t.UploadPdfAutomationId);
            
            AddColumn("dbo.CustomersReceiveablesManagement", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.InvoiceAutomation", "IsDeleted", c => c.Boolean(nullable: false));
            AlterColumn("dbo.CustomersReceiveablesManagement", "Address", c => c.String(maxLength: 1000));
            AlterColumn("dbo.InvoiceAutomation", "InvoiceDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MultithreadingInvoiceLogs", "UploadPdfAutomationId", "dbo.UploadPDFAutomation");
            DropIndex("dbo.MultithreadingInvoiceLogs", new[] { "UploadPdfAutomationId" });
            AlterColumn("dbo.InvoiceAutomation", "InvoiceDate", c => c.DateTime());
            AlterColumn("dbo.CustomersReceiveablesManagement", "Address", c => c.String());
            DropColumn("dbo.InvoiceAutomation", "IsDeleted");
            DropColumn("dbo.CustomersReceiveablesManagement", "IsDeleted");
            DropTable("dbo.MultithreadingInvoiceLogs");
            DropTable("dbo.BucketSizeMaster");
        }
    }
}
