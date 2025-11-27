namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserMstGroupFldAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserMaster", "GroupId", c => c.Int());
            CreateIndex("dbo.UserMaster", "GroupId");
            AddForeignKey("dbo.UserMaster", "GroupId", "dbo.GroupMaster", "GroupId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserMaster", "GroupId", "dbo.GroupMaster");
            DropIndex("dbo.UserMaster", new[] { "GroupId" });
            DropColumn("dbo.UserMaster", "GroupId");
        }
    }
}
