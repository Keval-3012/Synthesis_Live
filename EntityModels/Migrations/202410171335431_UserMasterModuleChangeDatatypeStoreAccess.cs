namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserMasterModuleChangeDatatypeStoreAccess : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserMaster", "StoreAccess", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserMaster", "StoreAccess", c => c.Int(nullable: false));
        }
    }
}
