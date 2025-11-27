namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddednewfieldIsActiveinAPIKeyConfigurationmodelforStatusofKey : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApiKeyConfiguartion", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ApiKeyConfiguartion", "IsActive");
        }
    }
}
