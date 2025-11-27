namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsOnlineinUserMasterAndChatModuleTablesCreated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChatGroupMembers",
                c => new
                    {
                        GroupMemberId = c.Int(nullable: false, identity: true),
                        GroupId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.GroupMemberId)
                .ForeignKey("dbo.ChatGroups", t => t.GroupId)
                .ForeignKey("dbo.UserMaster", t => t.UserId)
                .Index(t => t.GroupId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.ChatGroups",
                c => new
                    {
                        GroupId = c.Int(nullable: false, identity: true),
                        GroupName = c.String(),
                    })
                .PrimaryKey(t => t.GroupId);
            
            CreateTable(
                "dbo.ChatMessenger",
                c => new
                    {
                        ChatId = c.Int(nullable: false, identity: true),
                        ConversationId = c.Int(nullable: false),
                        SenderId = c.Int(nullable: false),
                        ReceiverId = c.Int(),
                        GroupId = c.Int(),
                        Message = c.String(),
                        Timestamp = c.DateTime(),
                        DeletedByUsers = c.String(),
                    })
                .PrimaryKey(t => t.ChatId)
                .ForeignKey("dbo.ChatGroups", t => t.GroupId)
                .ForeignKey("dbo.UserMaster", t => t.ReceiverId)
                .ForeignKey("dbo.UserMaster", t => t.SenderId)
                .Index(t => t.SenderId)
                .Index(t => t.ReceiverId)
                .Index(t => t.GroupId);
            
            CreateTable(
                "dbo.ChatReactions",
                c => new
                    {
                        ReactionId = c.Int(nullable: false, identity: true),
                        ChatId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        ReactionEmoji = c.String(),
                        Timestamp = c.DateTime(),
                    })
                .PrimaryKey(t => t.ReactionId)
                .ForeignKey("dbo.ChatMessenger", t => t.ChatId)
                .ForeignKey("dbo.UserMaster", t => t.UserId)
                .Index(t => t.ChatId)
                .Index(t => t.UserId);
            
            AddColumn("dbo.UserMaster", "IsOnline", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChatGroupMembers", "UserId", "dbo.UserMaster");
            DropForeignKey("dbo.ChatGroupMembers", "GroupId", "dbo.ChatGroups");
            DropForeignKey("dbo.ChatMessenger", "SenderId", "dbo.UserMaster");
            DropForeignKey("dbo.ChatMessenger", "ReceiverId", "dbo.UserMaster");
            DropForeignKey("dbo.ChatReactions", "UserId", "dbo.UserMaster");
            DropForeignKey("dbo.ChatReactions", "ChatId", "dbo.ChatMessenger");
            DropForeignKey("dbo.ChatMessenger", "GroupId", "dbo.ChatGroups");
            DropIndex("dbo.ChatReactions", new[] { "UserId" });
            DropIndex("dbo.ChatReactions", new[] { "ChatId" });
            DropIndex("dbo.ChatMessenger", new[] { "GroupId" });
            DropIndex("dbo.ChatMessenger", new[] { "ReceiverId" });
            DropIndex("dbo.ChatMessenger", new[] { "SenderId" });
            DropIndex("dbo.ChatGroupMembers", new[] { "UserId" });
            DropIndex("dbo.ChatGroupMembers", new[] { "GroupId" });
            DropColumn("dbo.UserMaster", "IsOnline");
            DropTable("dbo.ChatReactions");
            DropTable("dbo.ChatMessenger");
            DropTable("dbo.ChatGroups");
            DropTable("dbo.ChatGroupMembers");
        }
    }
}
