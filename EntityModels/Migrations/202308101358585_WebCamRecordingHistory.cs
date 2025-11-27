namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WebCamRecordingHistory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WebcamRecordingHistory",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FileUrl = c.String(),
                        Date = c.DateTime(),
                        StoreId = c.Int(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WebcamRecordingHistory");
        }
    }
}
