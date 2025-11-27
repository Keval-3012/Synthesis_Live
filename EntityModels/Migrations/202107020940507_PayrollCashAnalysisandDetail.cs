namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PayrollCashAnalysisandDetail : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PayrollCashAnalysis",
                c => new
                    {
                        PayrollCashAnalysisId = c.Int(nullable: false, identity: true),
                        StoreId = c.Int(),
                        Name = c.String(maxLength: 500),
                        DepartmentId = c.Int(),
                        ValueIn = c.Int(),
                        Description = c.String(maxLength: 2000),
                        IsActive = c.Boolean(nullable: false),
                        SortingNo = c.Int(),
                    })
                .PrimaryKey(t => t.PayrollCashAnalysisId)
                .ForeignKey("dbo.DepartmentMaster", t => t.DepartmentId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .Index(t => t.StoreId)
                .Index(t => t.DepartmentId);
            
            CreateTable(
                "dbo.PayrollCashAnalysisDetail",
                c => new
                    {
                        PayrollCashAnalysisDetailId = c.Int(nullable: false, identity: true),
                        PayrollCashAnalysisId = c.Int(),
                        Amount = c.Decimal(precision: 18, scale: 2),
                        PayrollId = c.Int(),
                    })
                .PrimaryKey(t => t.PayrollCashAnalysisDetailId)
                .ForeignKey("dbo.PayrollCashAnalysis", t => t.PayrollCashAnalysisId)
                .ForeignKey("dbo.PayrollMaster", t => t.PayrollId)
                .Index(t => t.PayrollCashAnalysisId)
                .Index(t => t.PayrollId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PayrollCashAnalysis", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.PayrollCashAnalysisDetail", "PayrollId", "dbo.PayrollMaster");
            DropForeignKey("dbo.PayrollCashAnalysisDetail", "PayrollCashAnalysisId", "dbo.PayrollCashAnalysis");
            DropForeignKey("dbo.PayrollCashAnalysis", "DepartmentId", "dbo.DepartmentMaster");
            DropIndex("dbo.PayrollCashAnalysisDetail", new[] { "PayrollId" });
            DropIndex("dbo.PayrollCashAnalysisDetail", new[] { "PayrollCashAnalysisId" });
            DropIndex("dbo.PayrollCashAnalysis", new[] { "DepartmentId" });
            DropIndex("dbo.PayrollCashAnalysis", new[] { "StoreId" });
            DropTable("dbo.PayrollCashAnalysisDetail");
            DropTable("dbo.PayrollCashAnalysis");
        }
    }
}
