namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewTableOperatingRatioExpenseDepartment : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OperatingRatioExpenseDepartment",
                c => new
                    {
                        OperatingRatioExpenseDepartmentId = c.Int(nullable: false, identity: true),
                        AccountNumber = c.Int(nullable: false),
                        DepartmentName = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.OperatingRatioExpenseDepartmentId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.OperatingRatioExpenseDepartment");
        }
    }
}
