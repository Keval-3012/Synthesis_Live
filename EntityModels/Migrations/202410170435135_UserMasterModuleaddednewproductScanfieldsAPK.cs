namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserMasterModuleaddednewproductScanfieldsAPK : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserMaster", "IsProductScanApp", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserMaster", "StoreAccess", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserMaster", "StoreAccess");
            DropColumn("dbo.UserMaster", "IsProductScanApp");
        }
    }
}
