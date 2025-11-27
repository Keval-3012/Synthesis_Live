namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDeptconfiguration : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DepartmentConfiguration", "Departmentconfiguration_DepartmentConfigurationId", "dbo.DepartmentConfiguration");
            DropIndex("dbo.DepartmentConfiguration", new[] { "Departmentconfiguration_DepartmentConfigurationId" });
            DropColumn("dbo.DepartmentConfiguration", "Departmentconfiguration_DepartmentConfigurationId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DepartmentConfiguration", "Departmentconfiguration_DepartmentConfigurationId", c => c.Int());
            CreateIndex("dbo.DepartmentConfiguration", "Departmentconfiguration_DepartmentConfigurationId");
            AddForeignKey("dbo.DepartmentConfiguration", "Departmentconfiguration_DepartmentConfigurationId", "dbo.DepartmentConfiguration", "DepartmentConfigurationId");
        }
    }
}
