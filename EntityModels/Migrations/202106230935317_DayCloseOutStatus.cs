namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DayCloseOutStatus : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DayCloseOutStatus",
                c => new
                    {
                        DayCloseOutId = c.Int(nullable: false, identity: true),
                        StartDate = c.DateTime(),
                        StoreId = c.Int(nullable: false),
                        DayCloseOutCount = c.Int(),
                    })
                .PrimaryKey(t => t.DayCloseOutId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .Index(t => t.StoreId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DayCloseOutStatus", "StoreId", "dbo.StoreMaster");
            DropIndex("dbo.DayCloseOutStatus", new[] { "StoreId" });
            DropTable("dbo.DayCloseOutStatus");
        }
    }
}
