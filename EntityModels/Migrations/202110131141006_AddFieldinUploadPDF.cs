namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFieldinUploadPDF : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UploadPdf", "FileType", c => c.String(maxLength: 1));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UploadPdf", "FileType");
        }
    }
}
