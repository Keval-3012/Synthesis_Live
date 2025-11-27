namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LocationTypefieldaddIpAdressInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.IpAdressInfo", "LocationType", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.IpAdressInfo", "LocationType");
        }
    }
}
