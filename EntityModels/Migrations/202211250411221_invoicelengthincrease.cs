namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class invoicelengthincrease : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Invoice", "UploadInvoice", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Invoice", "UploadInvoice", c => c.String(maxLength: 100));
        }
    }
}
