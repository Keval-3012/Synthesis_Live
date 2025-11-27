namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InvoiceUpdate1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Invoice", "InvoiceNumber", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Invoice", "InvoiceNumber", c => c.String(maxLength: 100));
        }
    }
}
