namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateisreadcolumntostringinchatmessengertable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ChatMessenger", "IsRead", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ChatMessenger", "IsRead", c => c.Boolean(nullable: false));
        }
    }
}
