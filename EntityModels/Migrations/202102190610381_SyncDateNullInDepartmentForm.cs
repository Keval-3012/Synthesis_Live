namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SyncDateNullInDepartmentForm : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DepartmentMaster", "SyncDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DepartmentMaster", "SyncDate", c => c.DateTime(nullable: false));
        }
    }
}
