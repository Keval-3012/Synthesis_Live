namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateChatGroupMemberTableAddColumnIsAdmin : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChatGroupMembers", "IsAdmin", c => c.Boolean(nullable: false));
            AddColumn("dbo.ChatGroups", "Createdby", c => c.Int(nullable: false));
            DropColumn("dbo.ChatGroups", "IsAdmin");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ChatGroups", "IsAdmin", c => c.Int(nullable: false));
            DropColumn("dbo.ChatGroups", "Createdby");
            DropColumn("dbo.ChatGroupMembers", "IsAdmin");
        }
    }
}
