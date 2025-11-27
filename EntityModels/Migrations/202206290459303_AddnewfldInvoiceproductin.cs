namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddnewfldInvoiceproductin : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InvoiceProduct", "Accuracy", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InvoiceProduct", "Accuracy");
        }
    }
}
