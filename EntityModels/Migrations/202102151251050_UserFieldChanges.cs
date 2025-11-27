namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserFieldChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserRoles", "UserTypeId", c => c.Int(nullable: false));
            DropColumn("dbo.UserRoles", "UserId");
            DropColumn("dbo.UserRoles", "StoreId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserRoles", "StoreId", c => c.Int(nullable: false));
            AddColumn("dbo.UserRoles", "UserId", c => c.Int(nullable: false));
            DropColumn("dbo.UserRoles", "UserTypeId");
        }
    }
}
