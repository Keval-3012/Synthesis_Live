namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HREmployeeMasterModifiedPassworddatecolumnaddedinTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HR_EmployeeMaster", "ModifiedPasswordDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.HR_EmployeeMaster", "ModifiedPasswordDate");
        }
    }
}
