namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewFieldVendorMaster : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VendorMaster", "Instruction", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VendorMaster", "Instruction");
        }
    }
}
