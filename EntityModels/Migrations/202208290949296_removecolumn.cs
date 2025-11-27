namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removecolumn : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.WeekMasters", "WeekDateRange");
        }
        
        public override void Down()
        {
            AddColumn("dbo.WeekMasters", "WeekDateRange", c => c.String());
        }
    }
}
