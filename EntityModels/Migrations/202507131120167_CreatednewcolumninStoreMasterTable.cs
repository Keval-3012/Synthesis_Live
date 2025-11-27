namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatednewcolumninStoreMasterTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StoreMaster", "CompetitorsId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StoreMaster", "CompetitorsId");
        }
    }
}
