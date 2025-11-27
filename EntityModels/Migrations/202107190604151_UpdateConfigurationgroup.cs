namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateConfigurationgroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ConfigurationGroup", "CustomerId", c => c.Int());
            AddColumn("dbo.ConfigurationGroup", "VendorId", c => c.Int());
            CreateIndex("dbo.ConfigurationGroup", "CustomerId");
            CreateIndex("dbo.ConfigurationGroup", "VendorId");
            AddForeignKey("dbo.ConfigurationGroup", "CustomerId", "dbo.CustomerMaster", "CustomerId");
            AddForeignKey("dbo.ConfigurationGroup", "VendorId", "dbo.VendorMaster", "VendorId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ConfigurationGroup", "VendorId", "dbo.VendorMaster");
            DropForeignKey("dbo.ConfigurationGroup", "CustomerId", "dbo.CustomerMaster");
            DropIndex("dbo.ConfigurationGroup", new[] { "VendorId" });
            DropIndex("dbo.ConfigurationGroup", new[] { "CustomerId" });
            DropColumn("dbo.ConfigurationGroup", "VendorId");
            DropColumn("dbo.ConfigurationGroup", "CustomerId");
        }
    }
}
