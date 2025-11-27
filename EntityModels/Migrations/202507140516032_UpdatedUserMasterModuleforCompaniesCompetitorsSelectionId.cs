namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedUserMasterModuleforCompaniesCompetitorsSelectionId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserMaster", "CompetitorsId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserMaster", "CompetitorsId");
        }
    }
}
