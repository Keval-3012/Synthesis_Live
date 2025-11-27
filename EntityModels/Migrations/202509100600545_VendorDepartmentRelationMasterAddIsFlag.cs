namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VendorDepartmentRelationMasterAddIsFlag : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VendorDepartmentRelationMaster", "IsFlag", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VendorDepartmentRelationMaster", "IsFlag");
        }
    }
}
