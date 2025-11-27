namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ErrorLog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ErrorLog",
                c => new
                    {
                        ErrorLogId = c.Int(nullable: false, identity: true),
                        ControllerName = c.String(maxLength: 100),
                        FunctionName = c.String(maxLength: 100),
                        Error = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        StoreId = c.Int(nullable: false),
                        DocumentId = c.Int(),
                        InvoiceId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ErrorLogId)
                .ForeignKey("dbo.Invoice", t => t.InvoiceId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .Index(t => t.StoreId)
                .Index(t => t.InvoiceId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ErrorLog", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.ErrorLog", "InvoiceId", "dbo.Invoice");
            DropIndex("dbo.ErrorLog", new[] { "InvoiceId" });
            DropIndex("dbo.ErrorLog", new[] { "StoreId" });
            DropTable("dbo.ErrorLog");
        }
    }
}
