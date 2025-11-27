namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserMasteraddednewDatastorefield : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserMaster", "DataStoreAccess", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserMaster", "DataStoreAccess");
        }
    }
}
