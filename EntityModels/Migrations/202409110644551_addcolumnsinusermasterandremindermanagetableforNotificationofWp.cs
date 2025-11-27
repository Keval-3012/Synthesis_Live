namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnsinusermasterandremindermanagetableforNotificationofWp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserMaster", "PhoneNumber", c => c.String());
            AddColumn("dbo.UserMaster", "ForWhatsAppNotification", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserWiseReminderManages", "ForWhatsAppNotification", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserWiseReminderManages", "ForWhatsAppNotification");
            DropColumn("dbo.UserMaster", "ForWhatsAppNotification");
            DropColumn("dbo.UserMaster", "PhoneNumber");
        }
    }
}
