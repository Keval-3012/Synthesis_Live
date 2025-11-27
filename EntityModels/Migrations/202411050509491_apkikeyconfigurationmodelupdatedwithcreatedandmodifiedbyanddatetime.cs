namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class apkikeyconfigurationmodelupdatedwithcreatedandmodifiedbyanddatetime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApiKeyConfiguartion", "CreatedBy", c => c.Int(nullable: false));
            AddColumn("dbo.ApiKeyConfiguartion", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.ApiKeyConfiguartion", "ModifiedBy", c => c.Int(nullable: false));
            AddColumn("dbo.ApiKeyConfiguartion", "ModifiedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ApiKeyConfiguartion", "ModifiedDate");
            DropColumn("dbo.ApiKeyConfiguartion", "ModifiedBy");
            DropColumn("dbo.ApiKeyConfiguartion", "CreatedDate");
            DropColumn("dbo.ApiKeyConfiguartion", "CreatedBy");
        }
    }
}
