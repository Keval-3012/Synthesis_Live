namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewFieldsProductMaster : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductVendors", "CreatedBy", c => c.Int());
            AddColumn("dbo.ProductVendors", "DateCreated", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductVendors", "DateCreated");
            DropColumn("dbo.ProductVendors", "CreatedBy");
        }
    }
}
