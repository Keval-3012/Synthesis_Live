namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateVendorMasterEmail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VendorMaster", "EMail", c => c.String(maxLength: 80));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VendorMaster", "EMail");
        }
    }
}
