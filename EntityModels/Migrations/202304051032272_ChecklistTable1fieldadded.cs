namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChecklistTable1fieldadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CheckList", "AccountId", c => c.Int(nullable: false));
            AddColumn("dbo.CheckList", "EntityName", c => c.String());
            AddColumn("dbo.CheckList", "EntityId", c => c.Int(nullable: false));
            DropColumn("dbo.CheckList", "Name");
            DropColumn("dbo.CheckList", "Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CheckList", "Id", c => c.Int(nullable: false));
            AddColumn("dbo.CheckList", "Name", c => c.String());
            DropColumn("dbo.CheckList", "EntityId");
            DropColumn("dbo.CheckList", "EntityName");
            DropColumn("dbo.CheckList", "AccountId");
        }
    }
}
