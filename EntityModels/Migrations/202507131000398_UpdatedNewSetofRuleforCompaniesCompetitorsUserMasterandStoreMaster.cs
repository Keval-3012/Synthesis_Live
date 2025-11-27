namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedNewSetofRuleforCompaniesCompetitorsUserMasterandStoreMaster : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CompaniesCompetitors", "StoreId", "dbo.StoreMaster");
            DropIndex("dbo.CompaniesCompetitors", new[] { "StoreId" });
            AddColumn("dbo.UserMaster", "IsCustomCompetitors", c => c.Boolean(nullable: false));
            AddColumn("dbo.CompaniesCompetitors", "CompetitorsName", c => c.String(maxLength: 200));
            AddColumn("dbo.CompaniesCompetitors", "CompetitorsNickName", c => c.String());
            AddColumn("dbo.CompaniesCompetitors", "CompetitorsStoreId", c => c.String());
            DropColumn("dbo.StoreMaster", "CompetitorsName1");
            DropColumn("dbo.StoreMaster", "CompetitorsName2");
            DropColumn("dbo.StoreMaster", "ZipCode");
            DropColumn("dbo.CompaniesCompetitors", "CompetitorsName1");
            DropColumn("dbo.CompaniesCompetitors", "CompetitorsName2");
            DropColumn("dbo.CompaniesCompetitors", "StoreId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CompaniesCompetitors", "StoreId", c => c.Int(nullable: false));
            AddColumn("dbo.CompaniesCompetitors", "CompetitorsName2", c => c.String(maxLength: 200));
            AddColumn("dbo.CompaniesCompetitors", "CompetitorsName1", c => c.String(maxLength: 200));
            AddColumn("dbo.StoreMaster", "ZipCode", c => c.String());
            AddColumn("dbo.StoreMaster", "CompetitorsName2", c => c.String());
            AddColumn("dbo.StoreMaster", "CompetitorsName1", c => c.String());
            DropColumn("dbo.CompaniesCompetitors", "CompetitorsStoreId");
            DropColumn("dbo.CompaniesCompetitors", "CompetitorsNickName");
            DropColumn("dbo.CompaniesCompetitors", "CompetitorsName");
            DropColumn("dbo.UserMaster", "IsCustomCompetitors");
            CreateIndex("dbo.CompaniesCompetitors", "StoreId");
            AddForeignKey("dbo.CompaniesCompetitors", "StoreId", "dbo.StoreMaster", "StoreId", cascadeDelete: true);
        }
    }
}
