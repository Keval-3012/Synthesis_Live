namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewfieldaddProducts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "DateCreated", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "DateCreated");
        }
    }
}
