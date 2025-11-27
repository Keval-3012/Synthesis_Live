namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddednewmigrationforDesignatedStoreAccessinUserMasterforSynthesisInventoryApplication : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserMaster", "DesignatedStore", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserMaster", "DesignatedStore");
        }
    }
}
