namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExpenseCheckDocumentsTableAdd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExpenseCheckDocuments",
                c => new
                    {
                        ExpenseCheckDocumentId = c.Int(nullable: false, identity: true),
                        ExpenseCheckId = c.Int(nullable: false),
                        DocumentName = c.String(nullable: false, maxLength: 1000),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.ExpenseCheckDocumentId)
                .ForeignKey("dbo.ExpenseCheck", t => t.ExpenseCheckId)
                .Index(t => t.ExpenseCheckId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ExpenseCheckDocuments", "ExpenseCheckId", "dbo.ExpenseCheck");
            DropIndex("dbo.ExpenseCheckDocuments", new[] { "ExpenseCheckId" });
            DropTable("dbo.ExpenseCheckDocuments");
        }
    }
}
