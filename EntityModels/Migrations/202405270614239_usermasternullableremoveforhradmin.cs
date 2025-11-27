namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class usermasternullableremoveforhradmin : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserMaster", "IsHRAdmin", c => c.Boolean(nullable: false));
            AlterColumn("dbo.UserMaster", "IsHRSuperAdmin", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserMaster", "IsHRSuperAdmin", c => c.Boolean());
            AlterColumn("dbo.UserMaster", "IsHRAdmin", c => c.Boolean());
        }
    }
}
