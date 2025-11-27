namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewFieldUserMaster : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserMaster", "TrackHours", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserMaster", "TrackHours");
        }
    }
}
