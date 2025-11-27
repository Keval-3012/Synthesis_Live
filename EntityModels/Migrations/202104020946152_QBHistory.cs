namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QBHistory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QBHistory",
                c => new
                    {
                        QBHistoryId = c.Int(nullable: false, identity: true),
                        StoreId = c.Int(nullable: false),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        Operation = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.QBHistoryId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .Index(t => t.StoreId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QBHistory", "StoreId", "dbo.StoreMaster");
            DropIndex("dbo.QBHistory", new[] { "StoreId" });
            DropTable("dbo.QBHistory");
        }
    }
}
