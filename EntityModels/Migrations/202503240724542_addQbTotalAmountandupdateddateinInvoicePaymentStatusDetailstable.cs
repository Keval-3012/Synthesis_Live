namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addQbTotalAmountandupdateddateinInvoicePaymentStatusDetailstable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InvoicePaymentStatusDetails", "QbTotalAmount", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.InvoicePaymentStatusDetails", "UpdatedDate", c => c.DateTime());
            DropColumn("dbo.InvoicePaymentStatusDetails", "TxnDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InvoicePaymentStatusDetails", "TxnDate", c => c.DateTime());
            DropColumn("dbo.InvoicePaymentStatusDetails", "UpdatedDate");
            DropColumn("dbo.InvoicePaymentStatusDetails", "QbTotalAmount");
        }
    }
}
