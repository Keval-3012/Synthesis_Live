namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addnewtableweekmaster : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WeekMasters",
                c => new
                    {
                        WeekMasterID = c.Int(nullable: false, identity: true),
                        StartWeek = c.Int(nullable: false),
                        EndWeek = c.Int(nullable: false),
                        Year = c.Int(nullable: false),
                        WeekNo = c.Int(nullable: false),
                        WeekDateRange = c.String(),
                    })
                .PrimaryKey(t => t.WeekMasterID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WeekMasters");
        }
    }
}
