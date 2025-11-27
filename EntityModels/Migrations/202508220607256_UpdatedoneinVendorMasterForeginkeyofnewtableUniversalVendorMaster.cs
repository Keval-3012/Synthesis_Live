namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedoneinVendorMasterForeginkeyofnewtableUniversalVendorMaster : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VendorMaster", "UniversalVendorMasterId", c => c.Int());
            CreateIndex("dbo.VendorMaster", "UniversalVendorMasterId");
            AddForeignKey("dbo.VendorMaster", "UniversalVendorMasterId", "dbo.UniversalVendorMaster", "UniversalVendorMasterId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VendorMaster", "UniversalVendorMasterId", "dbo.UniversalVendorMaster");
            DropIndex("dbo.VendorMaster", new[] { "UniversalVendorMasterId" });
            DropColumn("dbo.VendorMaster", "UniversalVendorMasterId");
        }
    }
}
