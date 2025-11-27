namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DocumentFileVersion : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DocumentFileVersions",
                c => new
                    {
                        DocumentFileVersionId = c.Int(nullable: false, identity: true),
                        DocumentId = c.Int(nullable: false),
                        AttachFile = c.String(maxLength: 100),
                        Version = c.Int(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.DocumentFileVersionId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DocumentFileVersions");
        }
    }
}
