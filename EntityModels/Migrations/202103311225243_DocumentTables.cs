namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DocumentTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        DocumentId = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 1000),
                        DocumentCategoryId = c.Int(nullable: false),
                        StoreId = c.Int(nullable: false),
                        Notes = c.String(),
                        Description = c.String(),
                        FilePath = c.String(maxLength: 200),
                        AttachFile = c.String(maxLength: 100),
                        AttachExtention = c.String(maxLength: 100),
                        CreatedBy = c.Int(),
                        CreatedOn = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifiedOn = c.DateTime(),
                        IsPrivate = c.Boolean(nullable: false),
                        IsDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DocumentId)
                .ForeignKey("dbo.DocumentCategories", t => t.DocumentCategoryId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .Index(t => t.DocumentCategoryId)
                .Index(t => t.StoreId);
            
            CreateTable(
                "dbo.DocumentFavorites",
                c => new
                    {
                        DocumentFavoriteId = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        CreatedOn = c.DateTime(),
                        IsFavorite = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DocumentFavoriteId)
                .ForeignKey("dbo.Documents", t => t.DocumentId, cascadeDelete: true)
                .Index(t => t.DocumentId);
            
            CreateTable(
                "dbo.DocumentKeywords",
                c => new
                    {
                        DocumentKeywordId = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        Title = c.String(nullable: false, maxLength: 1000),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DocumentKeywordId)
                .ForeignKey("dbo.Documents", t => t.DocumentId, cascadeDelete: true)
                .Index(t => t.DocumentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Documents", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.DocumentKeywords", "DocumentId", "dbo.Documents");
            DropForeignKey("dbo.DocumentFavorites", "DocumentId", "dbo.Documents");
            DropForeignKey("dbo.Documents", "DocumentCategoryId", "dbo.DocumentCategories");
            DropIndex("dbo.DocumentKeywords", new[] { "DocumentId" });
            DropIndex("dbo.DocumentFavorites", new[] { "DocumentId" });
            DropIndex("dbo.Documents", new[] { "StoreId" });
            DropIndex("dbo.Documents", new[] { "DocumentCategoryId" });
            DropTable("dbo.DocumentKeywords");
            DropTable("dbo.DocumentFavorites");
            DropTable("dbo.Documents");
        }
    }
}
