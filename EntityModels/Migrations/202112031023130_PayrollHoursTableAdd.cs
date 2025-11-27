namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PayrollHoursTableAdd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PayrollHoursDetails",
                c => new
                    {
                        PayrollHoursDetailId = c.Int(nullable: false, identity: true),
                        PayrollHoursId = c.Int(nullable: false),
                        PayrollDepartmentId = c.Int(nullable: false),
                        RegHours = c.String(nullable: false, maxLength: 15),
                        Overtime = c.String(nullable: false, maxLength: 15),
                    })
                .PrimaryKey(t => t.PayrollHoursDetailId)
                .ForeignKey("dbo.PayrollDepartment", t => t.PayrollDepartmentId)
                .ForeignKey("dbo.PayrollHours", t => t.PayrollHoursId, cascadeDelete: true)
                .Index(t => t.PayrollHoursId)
                .Index(t => t.PayrollDepartmentId);
            
            CreateTable(
                "dbo.PayrollHours",
                c => new
                    {
                        PayrollHoursId = c.Int(nullable: false, identity: true),
                        StoreId = c.Int(nullable: false),
                        FileName = c.String(nullable: false),
                        FileDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PayrollHoursId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .Index(t => t.StoreId);
            
            AddColumn("dbo.ExpenseCheck", "Flag", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PayrollHours", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.PayrollHoursDetails", "PayrollHoursId", "dbo.PayrollHours");
            DropForeignKey("dbo.PayrollHoursDetails", "PayrollDepartmentId", "dbo.PayrollDepartment");
            DropIndex("dbo.PayrollHours", new[] { "StoreId" });
            DropIndex("dbo.PayrollHoursDetails", new[] { "PayrollDepartmentId" });
            DropIndex("dbo.PayrollHoursDetails", new[] { "PayrollHoursId" });
            DropColumn("dbo.ExpenseCheck", "Flag");
            DropTable("dbo.PayrollHours");
            DropTable("dbo.PayrollHoursDetails");
        }
    }
}
