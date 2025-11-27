namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MessageMasterandProdectinaddField : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MessageMasters",
                c => new
                    {
                        MessageId = c.Int(nullable: false, identity: true),
                        KeyNum = c.String(),
                        ValueStr = c.String(),
                        Description = c.String(),
                        ModuleName = c.String(),
                    })
                .PrimaryKey(t => t.MessageId);
            
            AddColumn("dbo.Products", "GenericID", c => c.String());
            AddColumn("dbo.PayrollCashAnalysis", "ETaxCalc", c => c.Int());
            AddColumn("dbo.PayrollCashAnalysisDetail", "ETaxCalc", c => c.Int(nullable: false));
            AddColumn("dbo.PayrollReport", "ApproveDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PayrollReport", "ApproveDate");
            DropColumn("dbo.PayrollCashAnalysisDetail", "ETaxCalc");
            DropColumn("dbo.PayrollCashAnalysis", "ETaxCalc");
            DropColumn("dbo.Products", "GenericID");
            DropTable("dbo.MessageMasters");
        }
    }
}
