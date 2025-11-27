namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddnewfldproductMaster : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "Flag", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "Flag");
        }
    }
}
