namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FldProductIdAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InvoiceProduct", "ProductId", c => c.Int());
            CreateIndex("dbo.InvoiceProduct", "ProductId");
            AddForeignKey("dbo.InvoiceProduct", "ProductId", "dbo.Products", "ProductId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InvoiceProduct", "ProductId", "dbo.Products");
            DropIndex("dbo.InvoiceProduct", new[] { "ProductId" });
            DropColumn("dbo.InvoiceProduct", "ProductId");
        }
    }
}
