namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WeeklyPeriodWNumFldAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WeeklyPeriod", "WNumber", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WeeklyPeriod", "WNumber");
        }
    }
}
