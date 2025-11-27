namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewfiledsinUniversalVendorMaster : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UniversalVendorMaster", "Email", c => c.String(maxLength: 80));
            AddColumn("dbo.UniversalVendorMaster", "CreatedOn", c => c.DateTime());
            AddColumn("dbo.UniversalVendorMaster", "CreatedBy", c => c.Int());
            AddColumn("dbo.UniversalVendorMaster", "ModifiedOn", c => c.DateTime());
            AddColumn("dbo.UniversalVendorMaster", "ModifiedBy", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UniversalVendorMaster", "ModifiedBy");
            DropColumn("dbo.UniversalVendorMaster", "ModifiedOn");
            DropColumn("dbo.UniversalVendorMaster", "CreatedBy");
            DropColumn("dbo.UniversalVendorMaster", "CreatedOn");
            DropColumn("dbo.UniversalVendorMaster", "Email");
        }
    }
}
