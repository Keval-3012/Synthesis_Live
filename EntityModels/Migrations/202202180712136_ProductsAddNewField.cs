namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductsAddNewField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "KeyWord", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "KeyWord");
        }
    }
}
