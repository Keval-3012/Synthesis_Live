namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addendhourcolumnwebcamhistory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WebcamRecordingHistory", "EndHour", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WebcamRecordingHistory", "EndHour");
        }
    }
}
