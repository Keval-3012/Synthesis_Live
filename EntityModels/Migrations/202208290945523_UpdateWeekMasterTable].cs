namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateWeekMasterTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WeekMasters", "StartDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.WeekMasters", "EndDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WeekMasters", "EndDate");
            DropColumn("dbo.WeekMasters", "StartDate");
        }
    }
}
