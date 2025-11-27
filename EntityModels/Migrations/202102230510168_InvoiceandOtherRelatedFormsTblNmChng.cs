namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InvoiceandOtherRelatedFormsTblNmChng : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.InvoiceDepartmentDetails", newName: "InvoiceDepartmentDetail");
            RenameTable(name: "dbo.Invoices", newName: "Invoice");
            RenameTable(name: "dbo.DiscountTypeMasters", newName: "DiscountTypeMaster");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.DiscountTypeMaster", newName: "DiscountTypeMasters");
            RenameTable(name: "dbo.Invoice", newName: "Invoices");
            RenameTable(name: "dbo.InvoiceDepartmentDetail", newName: "InvoiceDepartmentDetails");
        }
    }
}
