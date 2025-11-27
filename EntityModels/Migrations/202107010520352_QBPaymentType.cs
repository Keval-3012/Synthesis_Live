namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QBPaymentType : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QBPaymentType",
                c => new
                    {
                        QBPaymentTypetId = c.Int(nullable: false, identity: true),
                        PaymentType = c.String(maxLength: 300),
                        StoreId = c.Int(nullable: false),
                        ListId = c.String(maxLength: 50),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.QBPaymentTypetId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .Index(t => t.StoreId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QBPaymentType", "StoreId", "dbo.StoreMaster");
            DropIndex("dbo.QBPaymentType", new[] { "StoreId" });
            DropTable("dbo.QBPaymentType");
        }
    }
}
