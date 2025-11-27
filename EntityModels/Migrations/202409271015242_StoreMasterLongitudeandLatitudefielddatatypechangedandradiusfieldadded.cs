namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StoreMasterLongitudeandLatitudefielddatatypechangedandradiusfieldadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StoreMaster", "Radius", c => c.Int());
            AlterColumn("dbo.StoreMaster", "Longitude", c => c.Double());
            AlterColumn("dbo.StoreMaster", "Latitude", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.StoreMaster", "Latitude", c => c.String());
            AlterColumn("dbo.StoreMaster", "Longitude", c => c.String());
            DropColumn("dbo.StoreMaster", "Radius");
        }
    }
}
