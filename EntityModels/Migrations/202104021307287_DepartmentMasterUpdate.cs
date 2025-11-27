namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DepartmentMasterUpdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DepartmentMaster", "CreatedBy", c => c.Int());
            AlterColumn("dbo.DepartmentMaster", "ModifiedBy", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DepartmentMaster", "ModifiedBy", c => c.String(maxLength: 50));
            AlterColumn("dbo.DepartmentMaster", "CreatedBy", c => c.String(maxLength: 50));
        }
    }
}
