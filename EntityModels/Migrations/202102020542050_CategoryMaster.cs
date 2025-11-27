namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CategoryMaster : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DocumentCategories",
                c => new
                    {
                        DocumentCategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 30),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                        ModifiedOn = c.DateTime(),
                        CreatedOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.DocumentCategoryId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DocumentCategories");
        }
    }
}
