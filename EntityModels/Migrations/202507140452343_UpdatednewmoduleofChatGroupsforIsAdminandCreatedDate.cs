namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatednewmoduleofChatGroupsforIsAdminandCreatedDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChatGroups", "IsAdmin", c => c.Int(nullable: false));
            AddColumn("dbo.ChatGroups", "CreatedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ChatGroups", "CreatedDate");
            DropColumn("dbo.ChatGroups", "IsAdmin");
        }
    }
}
