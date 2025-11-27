namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewfieldistimecardentryusertimetrackinfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserTimeTrackInfoes", "IsTimeCardEntry", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserTimeTrackInfoes", "IsTimeCardEntry");
        }
    }
}
