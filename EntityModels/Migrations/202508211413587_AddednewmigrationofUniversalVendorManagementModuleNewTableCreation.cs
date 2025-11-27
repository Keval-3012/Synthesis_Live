namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddednewmigrationofUniversalVendorManagementModuleNewTableCreation : DbMigration
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
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UniversalVendorMaster");
        }
    }
}
