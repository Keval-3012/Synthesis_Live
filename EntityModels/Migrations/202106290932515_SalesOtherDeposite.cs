namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SalesOtherDeposite : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SalesOtherDeposite",
                c => new
                    {
                        SalesOtherDepositeId = c.Int(nullable: false, identity: true),
                        SalesGeneralId = c.Int(nullable: false),
                        StoreId = c.Int(nullable: false),
                        OtherDepositId = c.Int(nullable: false),
                        OtherDepositDate = c.DateTime(),
                        TxnId = c.String(maxLength: 50),
                        IsSync = c.Int(),
                        Status = c.Int(),
                    })
                .PrimaryKey(t => t.SalesOtherDepositeId)
                .ForeignKey("dbo.OtherDeposit", t => t.OtherDepositId)
                .ForeignKey("dbo.SalesGeneralEntries", t => t.SalesGeneralId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .Index(t => t.SalesGeneralId)
                .Index(t => t.StoreId)
                .Index(t => t.OtherDepositId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SalesOtherDeposite", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.SalesOtherDeposite", "SalesGeneralId", "dbo.SalesGeneralEntries");
            DropForeignKey("dbo.SalesOtherDeposite", "OtherDepositId", "dbo.OtherDeposit");
            DropIndex("dbo.SalesOtherDeposite", new[] { "OtherDepositId" });
            DropIndex("dbo.SalesOtherDeposite", new[] { "StoreId" });
            DropIndex("dbo.SalesOtherDeposite", new[] { "SalesGeneralId" });
            DropTable("dbo.SalesOtherDeposite");
        }
    }
}
