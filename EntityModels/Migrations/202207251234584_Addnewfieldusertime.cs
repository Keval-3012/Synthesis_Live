namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addnewfieldusertime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserTimeTrackInfoes", "TimeDuration", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserTimeTrackInfoes", "TimeDuration");
        }
    }
}
