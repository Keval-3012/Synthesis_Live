namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveGroupAccountMaster : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GroupAccountMaster", "DepartmentId", "dbo.DepartmentMaster");
            DropForeignKey("dbo.GroupAccountMaster", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.DepartmentConfiguration", "GroupAccountId", "dbo.GroupAccountMaster");
            DropIndex("dbo.DepartmentConfiguration", new[] { "GroupAccountId" });
            DropIndex("dbo.GroupAccountMaster", new[] { "DepartmentId" });
            DropIndex("dbo.GroupAccountMaster", new[] { "StoreId" });
            RenameColumn(table: "dbo.DepartmentConfiguration", name: "ConfigurationGroup_ConfigurationGroupId", newName: "ConfigurationGroupId");
            RenameIndex(table: "dbo.DepartmentConfiguration", name: "IX_ConfigurationGroup_ConfigurationGroupId", newName: "IX_ConfigurationGroupId");
            AddColumn("dbo.DepartmentConfiguration", "Departmentconfiguration_DepartmentConfigurationId", c => c.Int());
            CreateIndex("dbo.DepartmentConfiguration", "Departmentconfiguration_DepartmentConfigurationId");
            AddForeignKey("dbo.DepartmentConfiguration", "Departmentconfiguration_DepartmentConfigurationId", "dbo.DepartmentConfiguration", "DepartmentConfigurationId");
            DropColumn("dbo.DepartmentConfiguration", "GroupAccountId");
            DropTable("dbo.GroupAccountMaster");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.GroupAccountMaster",
                c => new
                    {
                        GroupAccountId = c.Int(nullable: false, identity: true),
                        AccountName = c.String(nullable: false, maxLength: 200),
                        TypicalBalanceId = c.Int(nullable: false),
                        Memo = c.String(maxLength: 2000),
                        DepartmentId = c.Int(nullable: false),
                        StoreId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.GroupAccountId);
            
            AddColumn("dbo.DepartmentConfiguration", "GroupAccountId", c => c.Int());
            DropForeignKey("dbo.DepartmentConfiguration", "Departmentconfiguration_DepartmentConfigurationId", "dbo.DepartmentConfiguration");
            DropIndex("dbo.DepartmentConfiguration", new[] { "Departmentconfiguration_DepartmentConfigurationId" });
            DropColumn("dbo.DepartmentConfiguration", "Departmentconfiguration_DepartmentConfigurationId");
            RenameIndex(table: "dbo.DepartmentConfiguration", name: "IX_ConfigurationGroupId", newName: "IX_ConfigurationGroup_ConfigurationGroupId");
            RenameColumn(table: "dbo.DepartmentConfiguration", name: "ConfigurationGroupId", newName: "ConfigurationGroup_ConfigurationGroupId");
            CreateIndex("dbo.GroupAccountMaster", "StoreId");
            CreateIndex("dbo.GroupAccountMaster", "DepartmentId");
            CreateIndex("dbo.DepartmentConfiguration", "GroupAccountId");
            AddForeignKey("dbo.DepartmentConfiguration", "GroupAccountId", "dbo.GroupAccountMaster", "GroupAccountId");
            AddForeignKey("dbo.GroupAccountMaster", "StoreId", "dbo.StoreMaster", "StoreId");
            AddForeignKey("dbo.GroupAccountMaster", "DepartmentId", "dbo.DepartmentMaster", "DepartmentId");
        }
    }
}
