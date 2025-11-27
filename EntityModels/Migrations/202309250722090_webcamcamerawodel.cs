namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class webcamcamerawodel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WebCamCameraLists",
                c => new
                    {
                        WebCamCameraListId = c.Int(nullable: false, identity: true),
                        CameraName = c.String(nullable: false, maxLength: 500),
                        StoreId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.WebCamCameraListId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId, cascadeDelete: true)
                .Index(t => t.StoreId);
            
            CreateTable(
                "dbo.WebcamRecordingHistory",
                c => new
                    {
                        WebcamRecordingHistoryID = c.Int(nullable: false, identity: true),
                        WebCamCameraListID = c.Int(nullable: false),
                        RecordingDate = c.DateTime(nullable: false),
                        RecordingStartTime = c.String(nullable: false, maxLength: 50),
                        RecordingEndTime = c.String(nullable: false, maxLength: 50),
                        FileName = c.String(maxLength: 2000),
                        IsArchive = c.Int(nullable: false),
                        IsDownload = c.Int(nullable: false),
                        IsUploaded = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        DownloadDate = c.DateTime(nullable: false),
                        UploadDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.WebcamRecordingHistoryID)
                .ForeignKey("dbo.WebCamCameraLists", t => t.WebCamCameraListID, cascadeDelete: true)
                .Index(t => t.WebCamCameraListID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WebcamRecordingHistory", "WebCamCameraListID", "dbo.WebCamCameraLists");
            DropForeignKey("dbo.WebCamCameraLists", "StoreId", "dbo.StoreMaster");
            DropIndex("dbo.WebcamRecordingHistory", new[] { "WebCamCameraListID" });
            DropIndex("dbo.WebCamCameraLists", new[] { "StoreId" });
            DropTable("dbo.WebcamRecordingHistory");
            DropTable("dbo.WebCamCameraLists");
        }
    }
}
