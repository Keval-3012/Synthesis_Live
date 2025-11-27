namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StoreMasterLongitudeandLatitudefieldadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StoreMaster", "Longitude", c => c.String());
            AddColumn("dbo.StoreMaster", "Latitude", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StoreMaster", "Latitude");
            DropColumn("dbo.StoreMaster", "Longitude");
        }
    }
}
