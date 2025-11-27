namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InvoiceUserProductMapLog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InvoiceUserProductMapLog",
                c => new
                    {
                        InvoiceUserProductMapLogID = c.Int(nullable: false, identity: true),
                        ItemNo = c.String(),
                        UPCCode = c.String(),
                        InvoiceID = c.Int(),
                        ProductID = c.Int(),
                        ProductVendorID = c.Int(),
                        Operation = c.Int(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        UpdatedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.InvoiceUserProductMapLogID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.InvoiceUserProductMapLog");
        }
    }
}
