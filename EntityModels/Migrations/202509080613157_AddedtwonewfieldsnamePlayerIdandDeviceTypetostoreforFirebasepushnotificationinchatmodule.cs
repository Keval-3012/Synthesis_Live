namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedtwonewfieldsnamePlayerIdandDeviceTypetostoreforFirebasepushnotificationinchatmodule : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserMaster", "PlayerId", c => c.String());
            AddColumn("dbo.UserMaster", "DeviceType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserMaster", "DeviceType");
            DropColumn("dbo.UserMaster", "PlayerId");
        }
    }
}
