namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateColumnWebcamHistory : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.WebcamRecordingHistory", "IsDownload", c => c.Int());
            AlterColumn("dbo.WebcamRecordingHistory", "IsUploaded", c => c.Int());
            AlterColumn("dbo.WebcamRecordingHistory", "DownloadDate", c => c.DateTime());
            AlterColumn("dbo.WebcamRecordingHistory", "UploadDate", c => c.DateTime());
            AlterColumn("dbo.WebcamRecordingHistory", "IsDeleted", c => c.Int());
            AlterColumn("dbo.WebcamRecordingHistory", "DeletedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.WebcamRecordingHistory", "DeletedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.WebcamRecordingHistory", "IsDeleted", c => c.Int(nullable: false));
            AlterColumn("dbo.WebcamRecordingHistory", "UploadDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.WebcamRecordingHistory", "DownloadDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.WebcamRecordingHistory", "IsUploaded", c => c.Int(nullable: false));
            AlterColumn("dbo.WebcamRecordingHistory", "IsDownload", c => c.Int(nullable: false));
        }
    }
}
