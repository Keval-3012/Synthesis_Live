namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PayrollCashAnalysisFieldChange : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PayrollCashAnalysisDetail", "PayrollId", "dbo.PayrollMaster");
            AddForeignKey("dbo.PayrollCashAnalysisDetail", "PayrollId", "dbo.PayrollMaster", "PayrollId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PayrollCashAnalysisDetail", "PayrollId", "dbo.PayrollMaster");
            AddForeignKey("dbo.PayrollCashAnalysisDetail", "PayrollId", "dbo.PayrollMaster", "PayrollId");
        }
    }
}
