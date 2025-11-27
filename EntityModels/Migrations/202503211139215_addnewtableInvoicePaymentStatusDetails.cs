namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewtableInvoicePaymentStatusDetails : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InvoicePaymentStatusDetails",
                c => new
                    {
                        InvoicePaymentId = c.Int(nullable: false, identity: true),
                        InvoiceId = c.Int(nullable: false),
                        BillPaymentTxnId = c.Int(nullable: false),
                        QbPaymentMethod = c.String(),
                        QbAccountPaidName = c.String(),
                        QbCheckAmount = c.Decimal(precision: 18, scale: 2),
                        QbCheckNo = c.String(),
                        QbPaymentDate = c.DateTime(),
                        TxnDate = c.DateTime(),
                        CreatedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.InvoicePaymentId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.InvoicePaymentStatusDetails");
        }
    }
}
