namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedVendorMasterwithBankAccountNumberandRoutingNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VendorMaster", "RoutingNumber", c => c.String());
            AddColumn("dbo.VendorMaster", "BankAccountNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VendorMaster", "BankAccountNumber");
            DropColumn("dbo.VendorMaster", "RoutingNumber");
        }
    }
}
