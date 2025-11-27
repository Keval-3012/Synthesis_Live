namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addednewfieldinUserMasterTableforImageUploadRightinProductScanApp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserMaster", "ProductImageUpload", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserMaster", "ProductImageUpload");
        }
    }
}
