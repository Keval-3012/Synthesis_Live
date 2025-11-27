namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InvoiceProductModelAddNewFld : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InvoiceProduct", "ProductVendorId", c => c.Int());
            CreateIndex("dbo.InvoiceProduct", "ProductVendorId");
            AddForeignKey("dbo.InvoiceProduct", "ProductVendorId", "dbo.ProductVendors", "ProductVendorId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InvoiceProduct", "ProductVendorId", "dbo.ProductVendors");
            DropIndex("dbo.InvoiceProduct", new[] { "ProductVendorId" });
            DropColumn("dbo.InvoiceProduct", "ProductVendorId");
        }
    }
}
