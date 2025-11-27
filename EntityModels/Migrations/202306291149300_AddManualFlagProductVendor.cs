namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddManualFlagProductVendor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductVendors", "IsManually", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductVendors", "IsManually");
        }
    }
}
