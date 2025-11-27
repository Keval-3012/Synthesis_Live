namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class invoiceproductaddnewfld : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InvoiceProduct", "Approved", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InvoiceProduct", "Approved");
        }
    }
}
