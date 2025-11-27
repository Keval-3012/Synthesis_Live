namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WebcamHistoryaddcolumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WebcamRecordingHistory", "FolderName", c => c.String());
            AddColumn("dbo.WebcamRecordingHistory", "IsDeleted", c => c.Int(nullable: false));
            AddColumn("dbo.WebcamRecordingHistory", "DeletedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WebcamRecordingHistory", "DeletedDate");
            DropColumn("dbo.WebcamRecordingHistory", "IsDeleted");
            DropColumn("dbo.WebcamRecordingHistory", "FolderName");
        }
    }
}
