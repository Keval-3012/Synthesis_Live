namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductVendor : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductVendors",
                c => new
                    {
                        ProductVendorId = c.Int(nullable: false, identity: true),
                        ItemNo = c.String(maxLength: 500),
                        UPCCode = c.String(maxLength: 300),
                        Description = c.String(maxLength: 1000),
                        Vendors = c.String(maxLength: 3000),
                        Brand = c.String(maxLength: 500),
                        Size = c.String(maxLength: 300),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductId = c.Int(),
                    })
                .PrimaryKey(t => t.ProductVendorId)
                .ForeignKey("dbo.Products", t => t.ProductId)
                .Index(t => t.ProductId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductVendors", "ProductId", "dbo.Products");
            DropIndex("dbo.ProductVendors", new[] { "ProductId" });
            DropTable("dbo.ProductVendors");
        }
    }
}
