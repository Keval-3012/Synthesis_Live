namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddfieldInUserTimeTrackInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserTimeTrackInfoes", "IsSystemEntry", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserTimeTrackInfoes", "IsSystemEntry");
        }
    }
}
