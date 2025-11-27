namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddednewTwocolumnsnameFCMTokenWebandFCMTokenAppforNotificationAPKofChatModule : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserMaster", "FCMTokenApp", c => c.String());
            AddColumn("dbo.UserMaster", "FCMTokenWeb", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserMaster", "FCMTokenWeb");
            DropColumn("dbo.UserMaster", "FCMTokenApp");
        }
    }
}
