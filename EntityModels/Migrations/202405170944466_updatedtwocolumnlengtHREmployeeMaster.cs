namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedtwocolumnlengtHREmployeeMaster : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.HR_EmployeeMaster", "Phone", c => c.String(maxLength: 50));
            AlterColumn("dbo.HR_EmployeeMaster", "MobileNo", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.HR_EmployeeMaster", "MobileNo", c => c.String(maxLength: 10));
            AlterColumn("dbo.HR_EmployeeMaster", "Phone", c => c.String(maxLength: 10));
        }
    }
}
