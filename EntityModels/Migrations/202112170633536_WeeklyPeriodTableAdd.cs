namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WeeklyPeriodTableAdd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WeeklyPeriod",
                c => new
                    {
                        WeeklyPeriodId = c.Int(nullable: false, identity: true),
                        WeekNo = c.String(nullable: false, maxLength: 100),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Year = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.WeeklyPeriodId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WeeklyPeriod");
        }
    }
}
