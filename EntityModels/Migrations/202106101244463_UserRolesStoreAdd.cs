namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserRolesStoreAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserRoles", "StoreId", c => c.Int());
            CreateIndex("dbo.UserRoles", "StoreId");
            AddForeignKey("dbo.UserRoles", "StoreId", "dbo.StoreMaster", "StoreId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserRoles", "StoreId", "dbo.StoreMaster");
            DropIndex("dbo.UserRoles", new[] { "StoreId" });
            DropColumn("dbo.UserRoles", "StoreId");
        }
    }
}
