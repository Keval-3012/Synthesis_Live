namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChecklistTablecreateddateaddedfield : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CheckList", "CreatedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CheckList", "CreatedDate");
        }
    }
}
