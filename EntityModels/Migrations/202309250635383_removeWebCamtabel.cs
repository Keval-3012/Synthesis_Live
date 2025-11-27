namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeWebCamtabel : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.WebcamRecordingHistory");
        }
        
        public override void Down()
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
    }
}
