namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addednewnamedCompaniesCompetitiorsandUseofCompaniesCompetitorsinSynthesisAPKInventoryinCompaniesPage : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompaniesCompetitors",
                c => new
                    {
                        CompaniesCompetitorsId = c.Int(nullable: false, identity: true),
                        CompetitorsName1 = c.String(maxLength: 200),
                        CompetitorsName2 = c.String(maxLength: 200),
                        ZipCode = c.String(),
                        StoreId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CompaniesCompetitorsId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId, cascadeDelete: true)
                .Index(t => t.StoreId);
            
            AddColumn("dbo.StoreMaster", "CompetitorsName1", c => c.String());
            AddColumn("dbo.StoreMaster", "CompetitorsName2", c => c.String());
            AddColumn("dbo.StoreMaster", "ZipCode", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CompaniesCompetitors", "StoreId", "dbo.StoreMaster");
            DropIndex("dbo.CompaniesCompetitors", new[] { "StoreId" });
            DropColumn("dbo.StoreMaster", "ZipCode");
            DropColumn("dbo.StoreMaster", "CompetitorsName2");
            DropColumn("dbo.StoreMaster", "CompetitorsName1");
            DropTable("dbo.CompaniesCompetitors");
        }
    }
}
