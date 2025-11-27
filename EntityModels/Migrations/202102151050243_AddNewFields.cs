namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StoreMaster", "GroupId", c => c.Int(nullable: false));
            AddColumn("dbo.UserMaster", "UserTypeId", c => c.Int(nullable: false));
            CreateIndex("dbo.StoreMaster", "GroupId");
            CreateIndex("dbo.UserMaster", "UserTypeId");
            AddForeignKey("dbo.StoreMaster", "GroupId", "dbo.GroupMaster", "GroupId");
            AddForeignKey("dbo.UserMaster", "UserTypeId", "dbo.UserTypeMaster", "UserTypeId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserMaster", "UserTypeId", "dbo.UserTypeMaster");
            DropForeignKey("dbo.StoreMaster", "GroupId", "dbo.GroupMaster");
            DropIndex("dbo.UserMaster", new[] { "UserTypeId" });
            DropIndex("dbo.StoreMaster", new[] { "GroupId" });
            DropColumn("dbo.UserMaster", "UserTypeId");
            DropColumn("dbo.StoreMaster", "GroupId");
        }
    }
}
