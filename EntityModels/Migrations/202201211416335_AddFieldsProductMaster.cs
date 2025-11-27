namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFieldsProductMaster : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "Brand", c => c.String(maxLength: 500));
            AddColumn("dbo.Products", "Size", c => c.String(maxLength: 300));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "Size");
            DropColumn("dbo.Products", "Brand");
        }
    }
}
