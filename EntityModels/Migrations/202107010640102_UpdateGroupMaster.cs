namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateGroupMaster : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GroupMaster", "CustomerId", c => c.Int());
            AddColumn("dbo.GroupMaster", "VendorId", c => c.Int());
            CreateIndex("dbo.GroupMaster", "CustomerId");
            CreateIndex("dbo.GroupMaster", "VendorId");
            AddForeignKey("dbo.GroupMaster", "CustomerId", "dbo.CustomerMaster", "CustomerId");
            AddForeignKey("dbo.GroupMaster", "VendorId", "dbo.VendorMaster", "VendorId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GroupMaster", "VendorId", "dbo.VendorMaster");
            DropForeignKey("dbo.GroupMaster", "CustomerId", "dbo.CustomerMaster");
            DropIndex("dbo.GroupMaster", new[] { "VendorId" });
            DropIndex("dbo.GroupMaster", new[] { "CustomerId" });
            DropColumn("dbo.GroupMaster", "VendorId");
            DropColumn("dbo.GroupMaster", "CustomerId");
        }
    }
}
