namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddnewmodelUploadPdfapi : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UploadPdf_Api",
                c => new
                    {
                        UploadPdfId = c.Int(nullable: false, identity: true),
                        FileName = c.String(maxLength: 150),
                        IsProcess = c.Int(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        PageCount = c.Int(nullable: false),
                        ReadyForProcess = c.Int(nullable: false),
                        StoreId = c.Int(),
                        Synthesis_Live_InvID = c.Int(),
                        ReferenceId = c.Int(),
                        Errors = c.String(storeType: "ntext"),
                    })
                .PrimaryKey(t => t.UploadPdfId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .Index(t => t.StoreId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UploadPdf_Api", "StoreId", "dbo.StoreMaster");
            DropIndex("dbo.UploadPdf_Api", new[] { "StoreId" });
            DropTable("dbo.UploadPdf_Api");
        }
    }
}
