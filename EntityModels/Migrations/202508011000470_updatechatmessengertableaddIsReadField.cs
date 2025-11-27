namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatechatmessengertableaddIsReadField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChatMessenger", "IsRead", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ChatMessenger", "IsRead");
        }
    }
}
