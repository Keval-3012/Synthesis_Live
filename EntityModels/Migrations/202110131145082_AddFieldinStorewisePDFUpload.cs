namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFieldinStorewisePDFUpload : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StorewisePDFUpload", "FileType", c => c.String(maxLength: 1));
            DropColumn("dbo.UploadPdf", "FileType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UploadPdf", "FileType", c => c.String(maxLength: 1));
            DropColumn("dbo.StorewisePDFUpload", "FileType");
        }
    }
}
