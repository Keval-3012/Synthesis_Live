namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UploadFileAndInvoiceProductTblAdd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InvoiceProduct",
                c => new
                    {
                        InvoiceProductId = c.Int(nullable: false, identity: true),
                        InvoiceId = c.Int(nullable: false),
                        UPCCode = c.String(),
                        ItemNo = c.String(),
                        Description = c.String(),
                        Department = c.Int(nullable: false),
                        Qty = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScannedTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.InvoiceProductId)
                .ForeignKey("dbo.Invoice", t => t.InvoiceId, cascadeDelete: true)
                .Index(t => t.InvoiceId);
            
            CreateTable(
                "dbo.UploadPdf",
                c => new
                    {
                        UploadPdfId = c.Int(nullable: false, identity: true),
                        FileName = c.String(maxLength: 150),
                        IsProcess = c.Int(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        PageCount = c.Int(nullable: false),
                        ReadyForProcess = c.Int(nullable: false),
                        StoreId = c.Int(),
                        ReferenceId = c.Int(),
                        Errors = c.String(storeType: "ntext"),
                    })
                .PrimaryKey(t => t.UploadPdfId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .Index(t => t.StoreId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UploadPdf", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.InvoiceProduct", "InvoiceId", "dbo.Invoice");
            DropIndex("dbo.UploadPdf", new[] { "StoreId" });
            DropIndex("dbo.InvoiceProduct", new[] { "InvoiceId" });
            DropTable("dbo.UploadPdf");
            DropTable("dbo.InvoiceProduct");
        }
    }
}
