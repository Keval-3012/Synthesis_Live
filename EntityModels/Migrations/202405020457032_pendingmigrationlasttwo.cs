namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pendingmigrationlasttwo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InvoiceAutomation",
                c => new
                    {
                        InvoiceAutomationId = c.Int(nullable: false, identity: true),
                        StoreId = c.Int(nullable: false),
                        InvoiceTypeId = c.Int(nullable: false),
                        VendorId = c.Int(nullable: false),
                        InvoiceDate = c.DateTime(nullable: false),
                        InvoiceNumber = c.String(maxLength: 100),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UploadInvoice = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        PDFPageCount = c.Int(),
                    })
                .PrimaryKey(t => t.InvoiceAutomationId);
            
            CreateTable(
                "dbo.UploadPDFAutomation",
                c => new
                    {
                        UploadPdfAutomationId = c.Int(nullable: false, identity: true),
                        FileName = c.String(maxLength: 150),
                        IsProcess = c.Int(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        PageCount = c.Int(nullable: false),
                        ReadyForProcess = c.Int(nullable: false),
                        StoreId = c.Int(),
                        Synthesis_Live_InvID = c.Int(),
                        ReferenceId = c.Int(),
                    })
                .PrimaryKey(t => t.UploadPdfAutomationId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .Index(t => t.StoreId);
            
            AddColumn("dbo.Products", "Ecrs_Item_Id", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UploadPDFAutomation", "StoreId", "dbo.StoreMaster");
            DropIndex("dbo.UploadPDFAutomation", new[] { "StoreId" });
            DropColumn("dbo.Products", "Ecrs_Item_Id");
            DropTable("dbo.UploadPDFAutomation");
            DropTable("dbo.InvoiceAutomation");
        }
    }
}
