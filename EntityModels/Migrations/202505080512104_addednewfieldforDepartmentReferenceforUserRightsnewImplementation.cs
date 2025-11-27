namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addednewfieldforDepartmentReferenceforUserRightsnewImplementation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserWiseRightsStores", "DepartmentId", c => c.Int(nullable: false));
            CreateIndex("dbo.UserWiseRightsStores", "DepartmentId");
            AddForeignKey("dbo.UserWiseRightsStores", "DepartmentId", "dbo.DepartmentMaster", "DepartmentId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserWiseRightsStores", "DepartmentId", "dbo.DepartmentMaster");
            DropIndex("dbo.UserWiseRightsStores", new[] { "DepartmentId" });
            DropColumn("dbo.UserWiseRightsStores", "DepartmentId");
        }
    }
}
