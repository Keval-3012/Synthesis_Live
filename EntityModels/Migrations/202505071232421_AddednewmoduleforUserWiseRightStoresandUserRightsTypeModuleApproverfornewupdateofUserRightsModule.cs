namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddednewmoduleforUserWiseRightStoresandUserRightsTypeModuleApproverfornewupdateofUserRightsModule : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserRightsTypeModuleApprover",
                c => new
                    {
                        UserRightsTypeModuleApproverId = c.Int(nullable: false, identity: true),
                        UserTypeId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        ModuleId = c.Int(nullable: false),
                        LevelsApproverId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserRightsTypeModuleApproverId)
                .ForeignKey("dbo.LevelsApprovers", t => t.LevelsApproverId, cascadeDelete: true)
                .ForeignKey("dbo.ModuleMaster", t => t.ModuleId, cascadeDelete: true)
                .ForeignKey("dbo.UserMaster", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.UserTypeMaster", t => t.UserTypeId, cascadeDelete: true)
                .Index(t => t.UserTypeId)
                .Index(t => t.UserId)
                .Index(t => t.ModuleId)
                .Index(t => t.LevelsApproverId);
            
            CreateTable(
                "dbo.UserWiseRightsStores",
                c => new
                    {
                        UserWiseRightsStoresId = c.Int(nullable: false, identity: true),
                        UserTypeId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Role = c.String(nullable: false),
                        StoreId = c.Int(),
                        ModuleId = c.Int(),
                    })
                .PrimaryKey(t => t.UserWiseRightsStoresId)
                .ForeignKey("dbo.ModuleMaster", t => t.ModuleId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .ForeignKey("dbo.UserMaster", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.UserTypeMaster", t => t.UserTypeId, cascadeDelete: true)
                .Index(t => t.UserTypeId)
                .Index(t => t.UserId)
                .Index(t => t.StoreId)
                .Index(t => t.ModuleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserWiseRightsStores", "UserTypeId", "dbo.UserTypeMaster");
            DropForeignKey("dbo.UserWiseRightsStores", "UserId", "dbo.UserMaster");
            DropForeignKey("dbo.UserWiseRightsStores", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.UserWiseRightsStores", "ModuleId", "dbo.ModuleMaster");
            DropForeignKey("dbo.UserRightsTypeModuleApprover", "UserTypeId", "dbo.UserTypeMaster");
            DropForeignKey("dbo.UserRightsTypeModuleApprover", "UserId", "dbo.UserMaster");
            DropForeignKey("dbo.UserRightsTypeModuleApprover", "ModuleId", "dbo.ModuleMaster");
            DropForeignKey("dbo.UserRightsTypeModuleApprover", "LevelsApproverId", "dbo.LevelsApprovers");
            DropIndex("dbo.UserWiseRightsStores", new[] { "ModuleId" });
            DropIndex("dbo.UserWiseRightsStores", new[] { "StoreId" });
            DropIndex("dbo.UserWiseRightsStores", new[] { "UserId" });
            DropIndex("dbo.UserWiseRightsStores", new[] { "UserTypeId" });
            DropIndex("dbo.UserRightsTypeModuleApprover", new[] { "LevelsApproverId" });
            DropIndex("dbo.UserRightsTypeModuleApprover", new[] { "ModuleId" });
            DropIndex("dbo.UserRightsTypeModuleApprover", new[] { "UserId" });
            DropIndex("dbo.UserRightsTypeModuleApprover", new[] { "UserTypeId" });
            DropTable("dbo.UserWiseRightsStores");
            DropTable("dbo.UserRightsTypeModuleApprover");
        }
    }
}
