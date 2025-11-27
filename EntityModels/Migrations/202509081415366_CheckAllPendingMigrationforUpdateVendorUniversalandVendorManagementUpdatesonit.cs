namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CheckAllPendingMigrationforUpdateVendorUniversalandVendorManagementUpdatesonit : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UniversalVendorMaster",
                c => new
                    {
                        UniversalVendorMasterId = c.Int(nullable: false, identity: true),
                        VendorName = c.String(maxLength: 500),
                        DisplayName = c.String(maxLength: 500),
                        PrintCheckName = c.String(maxLength: 500),
                        Address = c.String(maxLength: 1000),
                        City = c.String(maxLength: 50),
                        State = c.String(maxLength: 50),
                        PostalCode = c.String(maxLength: 50),
                        Country = c.String(maxLength: 50),
                        PhoneNumber = c.String(maxLength: 50),
                        VendorProfileImage = c.String(),
                        VendorProfileProgress = c.Int(),
                    })
                .PrimaryKey(t => t.UniversalVendorMasterId);
            
            AddColumn("dbo.VendorMaster", "UniversalVendorMasterId", c => c.Int());
            CreateIndex("dbo.VendorMaster", "UniversalVendorMasterId");
            AddForeignKey("dbo.VendorMaster", "UniversalVendorMasterId", "dbo.UniversalVendorMaster", "UniversalVendorMasterId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VendorMaster", "UniversalVendorMasterId", "dbo.UniversalVendorMaster");
            DropIndex("dbo.VendorMaster", new[] { "UniversalVendorMasterId" });
            DropColumn("dbo.VendorMaster", "UniversalVendorMasterId");
            DropTable("dbo.UniversalVendorMaster");
        }
    }
}
