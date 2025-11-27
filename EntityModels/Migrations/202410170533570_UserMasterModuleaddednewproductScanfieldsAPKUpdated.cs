namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserMasterModuleaddednewproductScanfieldsAPKUpdated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserMaster", "UserRightsforStoreAccess", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserMaster", "UserRightsforStoreAccess");
        }
    }
}
