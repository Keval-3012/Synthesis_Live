namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserRoleRightsFK : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.RightsStores", "UserTypeId");
            CreateIndex("dbo.RightsStores", "StoreId");
            CreateIndex("dbo.UserRoles", "UserTypeId");
            AddForeignKey("dbo.RightsStores", "StoreId", "dbo.StoreMaster", "StoreId");
            AddForeignKey("dbo.RightsStores", "UserTypeId", "dbo.UserTypeMaster", "UserTypeId");
            AddForeignKey("dbo.UserRoles", "UserTypeId", "dbo.UserTypeMaster", "UserTypeId");
            DropColumn("dbo.UserMaster", "LastName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserMaster", "LastName", c => c.String(nullable: false));
            DropForeignKey("dbo.UserRoles", "UserTypeId", "dbo.UserTypeMaster");
            DropForeignKey("dbo.RightsStores", "UserTypeId", "dbo.UserTypeMaster");
            DropForeignKey("dbo.RightsStores", "StoreId", "dbo.StoreMaster");
            DropIndex("dbo.UserRoles", new[] { "UserTypeId" });
            DropIndex("dbo.RightsStores", new[] { "StoreId" });
            DropIndex("dbo.RightsStores", new[] { "UserTypeId" });
        }
    }
}
