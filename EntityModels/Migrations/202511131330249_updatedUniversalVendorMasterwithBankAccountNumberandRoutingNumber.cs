namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedUniversalVendorMasterwithBankAccountNumberandRoutingNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UniversalVendorMaster", "RoutingNumber", c => c.String());
            AddColumn("dbo.UniversalVendorMaster", "BankAccountNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UniversalVendorMaster", "BankAccountNumber");
            DropColumn("dbo.UniversalVendorMaster", "RoutingNumber");
        }
    }
}
