namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DepartmentFldAddRightsStoreTbl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RightsStores", "DepartmentId", c => c.Int());
            CreateIndex("dbo.RightsStores", "DepartmentId");
            AddForeignKey("dbo.RightsStores", "DepartmentId", "dbo.DepartmentMaster", "DepartmentId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RightsStores", "DepartmentId", "dbo.DepartmentMaster");
            DropIndex("dbo.RightsStores", new[] { "DepartmentId" });
            DropColumn("dbo.RightsStores", "DepartmentId");
        }
    }
}
