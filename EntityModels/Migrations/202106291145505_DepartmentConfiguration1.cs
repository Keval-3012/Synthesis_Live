namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DepartmentConfiguration1 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.DepartmentConfiguration", name: "ConfigurationGroupId", newName: "ConfigurationGroup_ConfigurationGroupId");
            RenameIndex(table: "dbo.DepartmentConfiguration", name: "IX_ConfigurationGroupId", newName: "IX_ConfigurationGroup_ConfigurationGroupId");
            AddColumn("dbo.DepartmentConfiguration", "GroupAccountId", c => c.Int());
            CreateIndex("dbo.DepartmentConfiguration", "GroupAccountId");
            AddForeignKey("dbo.DepartmentConfiguration", "GroupAccountId", "dbo.GroupAccountMaster", "GroupAccountId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DepartmentConfiguration", "GroupAccountId", "dbo.GroupAccountMaster");
            DropIndex("dbo.DepartmentConfiguration", new[] { "GroupAccountId" });
            DropColumn("dbo.DepartmentConfiguration", "GroupAccountId");
            RenameIndex(table: "dbo.DepartmentConfiguration", name: "IX_ConfigurationGroup_ConfigurationGroupId", newName: "IX_ConfigurationGroupId");
            RenameColumn(table: "dbo.DepartmentConfiguration", name: "ConfigurationGroup_ConfigurationGroupId", newName: "ConfigurationGroupId");
        }
    }
}
