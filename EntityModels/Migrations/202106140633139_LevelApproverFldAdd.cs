namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LevelApproverFldAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LevelsApprovers", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LevelsApprovers", "IsActive");
        }
    }
}
