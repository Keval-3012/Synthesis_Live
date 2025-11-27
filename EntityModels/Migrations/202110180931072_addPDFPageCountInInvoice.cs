namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPDFPageCountInInvoice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invoice", "PDFPageCount", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Invoice", "PDFPageCount");
        }
    }
}
