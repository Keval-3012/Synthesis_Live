namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserTypeGroupIdAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserTypeMaster", "GroupId", c => c.Int());
            CreateIndex("dbo.UserTypeMaster", "GroupId");
            AddForeignKey("dbo.UserTypeMaster", "GroupId", "dbo.GroupMaster", "GroupId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserTypeMaster", "GroupId", "dbo.GroupMaster");
            DropIndex("dbo.UserTypeMaster", new[] { "GroupId" });
            DropColumn("dbo.UserTypeMaster", "GroupId");
        }
    }
}
