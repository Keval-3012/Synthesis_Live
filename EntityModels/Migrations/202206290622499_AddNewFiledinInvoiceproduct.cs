namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewFiledinInvoiceproduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InvoiceProduct", "UPCCode_Accuracy", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.InvoiceProduct", "ItemNo_Accuracy", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.InvoiceProduct", "Description_Accuracy", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.InvoiceProduct", "Qty_Accuracy", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.InvoiceProduct", "UnitPrice_Accuracy", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.InvoiceProduct", "Total_Accuracy", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.InvoiceProduct", "Accuracy");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InvoiceProduct", "Accuracy", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.InvoiceProduct", "Total_Accuracy");
            DropColumn("dbo.InvoiceProduct", "UnitPrice_Accuracy");
            DropColumn("dbo.InvoiceProduct", "Qty_Accuracy");
            DropColumn("dbo.InvoiceProduct", "Description_Accuracy");
            DropColumn("dbo.InvoiceProduct", "ItemNo_Accuracy");
            DropColumn("dbo.InvoiceProduct", "UPCCode_Accuracy");
        }
    }
}
