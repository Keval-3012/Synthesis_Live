namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedAPIKeyConfigmodel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApiKeyConfiguartion", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.ApiKeyConfiguartion", "BucketSize", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ApiKeyConfiguartion", "BucketSize");
            DropColumn("dbo.ApiKeyConfiguartion", "Status");
        }
    }
}
