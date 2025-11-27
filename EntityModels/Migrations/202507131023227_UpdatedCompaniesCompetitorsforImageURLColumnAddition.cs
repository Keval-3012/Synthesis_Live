namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedCompaniesCompetitorsforImageURLColumnAddition : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CompaniesCompetitors", "ImageURL", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CompaniesCompetitors", "ImageURL");
        }
    }
}
