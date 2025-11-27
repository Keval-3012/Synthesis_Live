namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uploadpdfAddnewfld : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UploadPdf", "Synthesis_Live_InvID", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UploadPdf", "Synthesis_Live_InvID");
        }
    }
}
