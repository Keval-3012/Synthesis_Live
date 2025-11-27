namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AnalysisPdfFileRead : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PdfFieldMapping",
                c => new
                    {
                        PdfFieldMappingId = c.Int(nullable: false, identity: true),
                        PDFName = c.String(),
                        CommonName = c.String(),
                        VendorIds = c.String(),
                    })
                .PrimaryKey(t => t.PdfFieldMappingId);
            
            AddColumn("dbo.Invoice", "PDFProcessStatus", c => c.Int(nullable: false));
            AddColumn("dbo.Invoice", "PDFProcessDate", c => c.DateTime());
            AlterColumn("dbo.InvoiceProduct", "Department", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.InvoiceProduct", "Department", c => c.Int(nullable: false));
            DropColumn("dbo.Invoice", "PDFProcessDate");
            DropColumn("dbo.Invoice", "PDFProcessStatus");
            DropTable("dbo.PdfFieldMapping");
        }
    }
}
