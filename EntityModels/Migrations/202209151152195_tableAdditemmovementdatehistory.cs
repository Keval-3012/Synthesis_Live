namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tableAdditemmovementdatehistory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ItemMovementdatehistory",
                c => new
                    {
                        ItemMovementdatehistoryID = c.Int(nullable: false, identity: true),
                        StoreID = c.Int(nullable: false),
                        Startdate = c.DateTime(nullable: false),
                        Enddate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ItemMovementdatehistoryID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ItemMovementdatehistory");
        }
    }
}
