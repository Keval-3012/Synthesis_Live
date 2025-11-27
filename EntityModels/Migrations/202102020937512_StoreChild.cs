namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StoreChild : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.StoreMasters", newName: "StoreMaster");
            CreateTable(
                "dbo.StoreChild",
                c => new
                    {
                        StoreChildId = c.Int(nullable: false, identity: true),
                        StoreId = c.Int(nullable: false),
                        UserType = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.StoreChildId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId, cascadeDelete: true)
                .ForeignKey("dbo.UserMaster", t => t.UserId)
                .Index(t => t.StoreId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StoreChild", "UserId", "dbo.UserMaster");
            DropForeignKey("dbo.StoreChild", "StoreId", "dbo.StoreMaster");
            DropIndex("dbo.StoreChild", new[] { "UserId" });
            DropIndex("dbo.StoreChild", new[] { "StoreId" });
            DropTable("dbo.StoreChild");
            RenameTable(name: "dbo.StoreMaster", newName: "StoreMasters");
        }
    }
}
