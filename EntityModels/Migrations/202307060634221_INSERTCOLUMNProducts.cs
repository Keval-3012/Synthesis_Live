namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class INSERTCOLUMNProducts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "CreatedBy", c => c.Int());
            AddColumn("dbo.ProductVendors", "Flag", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductVendors", "Flag");
            DropColumn("dbo.Products", "CreatedBy");
        }
    }
}
