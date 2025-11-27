namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addednewtablenamedmenumasterandchangesdoneinUserMasterforDesignatedHomePageUserwise : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MenuMaster",
                c => new
                    {
                        MenuId = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 150),
                        MenuUrl = c.String(maxLength: 350),
                        ParentMenuId = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        ModifiedOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.MenuId);
            
            AddColumn("dbo.UserMaster", "DesignatedPageId", c => c.Int());
            CreateIndex("dbo.UserMaster", "DesignatedPageId");
            AddForeignKey("dbo.UserMaster", "DesignatedPageId", "dbo.MenuMaster", "MenuId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserMaster", "DesignatedPageId", "dbo.MenuMaster");
            DropIndex("dbo.UserMaster", new[] { "DesignatedPageId" });
            DropColumn("dbo.UserMaster", "DesignatedPageId");
            DropTable("dbo.MenuMaster");
        }
    }
}
