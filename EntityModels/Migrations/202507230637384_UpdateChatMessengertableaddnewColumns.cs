namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateChatMessengertableaddnewColumns : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChatMessenger", "DeletedBy", c => c.Int());
            AddColumn("dbo.ChatMessenger", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.ChatMessenger", "UpdatedDate", c => c.DateTime());
            AddColumn("dbo.ChatMessenger", "ConversationType", c => c.String());
            AddColumn("dbo.ChatMessenger", "UploadFile", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ChatMessenger", "UploadFile");
            DropColumn("dbo.ChatMessenger", "ConversationType");
            DropColumn("dbo.ChatMessenger", "UpdatedDate");
            DropColumn("dbo.ChatMessenger", "IsDeleted");
            DropColumn("dbo.ChatMessenger", "DeletedBy");
        }
    }
}
