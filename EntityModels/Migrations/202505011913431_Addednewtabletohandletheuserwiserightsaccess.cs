namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addednewtabletohandletheuserwiserightsaccess : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserWiseRights",
                c => new
                    {
                        UserWiseRightsId = c.Int(nullable: false, identity: true),
                        UserTypeId = c.Int(nullable: false),
                        Role = c.String(nullable: false),
                        StoreId = c.Int(),
                        UserId = c.Int(nullable: false),
                        ModuleId = c.Int(),
                    })
                .PrimaryKey(t => t.UserWiseRightsId)
                .ForeignKey("dbo.ModuleMaster", t => t.ModuleId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .ForeignKey("dbo.UserMaster", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.UserTypeMaster", t => t.UserTypeId, cascadeDelete: true)
                .Index(t => t.UserTypeId)
                .Index(t => t.StoreId)
                .Index(t => t.UserId)
                .Index(t => t.ModuleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserWiseRights", "UserTypeId", "dbo.UserTypeMaster");
            DropForeignKey("dbo.UserWiseRights", "UserId", "dbo.UserMaster");
            DropForeignKey("dbo.UserWiseRights", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.UserWiseRights", "ModuleId", "dbo.ModuleMaster");
            DropIndex("dbo.UserWiseRights", new[] { "ModuleId" });
            DropIndex("dbo.UserWiseRights", new[] { "UserId" });
            DropIndex("dbo.UserWiseRights", new[] { "StoreId" });
            DropIndex("dbo.UserWiseRights", new[] { "UserTypeId" });
            DropTable("dbo.UserWiseRights");
        }
    }
}
