namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PayrollTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PayrollDepartment",
                c => new
                    {
                        PayrollDepartmentId = c.Int(nullable: false, identity: true),
                        StoreId = c.Int(nullable: false),
                        DepartmentName = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.PayrollDepartmentId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .Index(t => t.StoreId);
            
            CreateTable(
                "dbo.PayrollDepartmentDetails",
                c => new
                    {
                        PayrollDepartmentDetId = c.Int(nullable: false, identity: true),
                        PayrollDepartmentId = c.Int(nullable: false),
                        Name = c.String(maxLength: 100),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PayrollId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PayrollDepartmentDetId)
                .ForeignKey("dbo.PayrollDepartment", t => t.PayrollDepartmentId)
                .ForeignKey("dbo.PayrollMaster", t => t.PayrollId, cascadeDelete: true)
                .Index(t => t.PayrollDepartmentId)
                .Index(t => t.PayrollId);
            
            CreateTable(
                "dbo.PayrollMaster",
                c => new
                    {
                        PayrollId = c.Int(nullable: false, identity: true),
                        StoreId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        PayrollReportId = c.Int(nullable: false),
                        EndCheckDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.PayrollId)
                .ForeignKey("dbo.PayrollReport", t => t.PayrollReportId, cascadeDelete: true)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .Index(t => t.StoreId)
                .Index(t => t.PayrollReportId);
            
            CreateTable(
                "dbo.PayrollReport",
                c => new
                    {
                        PayrollReportId = c.Int(nullable: false, identity: true),
                        StoreId = c.Int(nullable: false),
                        FileName = c.String(maxLength: 200),
                        UploadDate = c.DateTime(nullable: false),
                        IsRead = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.PayrollReportId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .Index(t => t.StoreId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PayrollDepartment", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.PayrollMaster", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.PayrollReport", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.PayrollMaster", "PayrollReportId", "dbo.PayrollReport");
            DropForeignKey("dbo.PayrollDepartmentDetails", "PayrollId", "dbo.PayrollMaster");
            DropForeignKey("dbo.PayrollDepartmentDetails", "PayrollDepartmentId", "dbo.PayrollDepartment");
            DropIndex("dbo.PayrollReport", new[] { "StoreId" });
            DropIndex("dbo.PayrollMaster", new[] { "PayrollReportId" });
            DropIndex("dbo.PayrollMaster", new[] { "StoreId" });
            DropIndex("dbo.PayrollDepartmentDetails", new[] { "PayrollId" });
            DropIndex("dbo.PayrollDepartmentDetails", new[] { "PayrollDepartmentId" });
            DropIndex("dbo.PayrollDepartment", new[] { "StoreId" });
            DropTable("dbo.PayrollReport");
            DropTable("dbo.PayrollMaster");
            DropTable("dbo.PayrollDepartmentDetails");
            DropTable("dbo.PayrollDepartment");
        }
    }
}
