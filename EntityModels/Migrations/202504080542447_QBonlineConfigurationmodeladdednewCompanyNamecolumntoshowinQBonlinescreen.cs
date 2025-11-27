namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QBonlineConfigurationmodeladdednewCompanyNamecolumntoshowinQBonlinescreen : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.QBOnlineConfiguration", "CompanyName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.QBOnlineConfiguration", "CompanyName");
        }
    }
}
