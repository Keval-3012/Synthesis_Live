namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QBUnclearBalanceInfo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QBUnclearBalanceInfoes",
                c => new
                    {
                        QBUnclearBalanceInfoId = c.Int(nullable: false, identity: true),
                        StoreID = c.Int(nullable: false),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LastSyncDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.QBUnclearBalanceInfoId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.QBUnclearBalanceInfoes");
        }
    }
}
