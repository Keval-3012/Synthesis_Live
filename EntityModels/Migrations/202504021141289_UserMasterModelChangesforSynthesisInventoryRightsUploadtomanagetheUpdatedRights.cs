namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserMasterModelChangesforSynthesisInventoryRightsUploadtomanagetheUpdatedRights : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserMaster", "UpdateProductDetails", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserMaster", "IsAbleExpiryChange", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserMaster", "IsAbleExpiryChange");
            DropColumn("dbo.UserMaster", "UpdateProductDetails");
        }
    }
}
