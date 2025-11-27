namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDepartmentconfigurations1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DepartmentConfiguration", "DepartmentId", "dbo.DepartmentMaster");
            AddForeignKey("dbo.DepartmentConfiguration", "DepartmentId", "dbo.DepartmentMaster", "DepartmentId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DepartmentConfiguration", "DepartmentId", "dbo.DepartmentMaster");
            AddForeignKey("dbo.DepartmentConfiguration", "DepartmentId", "dbo.DepartmentMaster", "DepartmentId", cascadeDelete: true);
        }
    }
}
