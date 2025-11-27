namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmployeeMaster : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmployeeMaster",
                c => new
                    {
                        EmployeeId = c.Int(nullable: false, identity: true),
                        FirstName = c.String(maxLength: 250),
                        LastName = c.String(maxLength: 250),
                        DisplayName = c.String(maxLength: 750),
                        Email = c.String(maxLength: 500),
                        StoreId = c.Int(nullable: false),
                        ListId = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.EmployeeId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .Index(t => t.StoreId);
            
            AddColumn("dbo.PayrollHoursDetails", "SalaryTime", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmployeeMaster", "StoreId", "dbo.StoreMaster");
            DropIndex("dbo.EmployeeMaster", new[] { "StoreId" });
            DropColumn("dbo.PayrollHoursDetails", "SalaryTime");
            DropTable("dbo.EmployeeMaster");
        }
    }
}
