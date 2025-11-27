namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QBOnlineConfiguration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QBOnlineConfiguration",
                c => new
                    {
                        QBOnlineId = c.Int(nullable: false, identity: true),
                        RealmId = c.String(),
                        ClientId = c.String(),
                        ClientSecretKey = c.String(),
                        QBToken = c.String(),
                        QBRefreshToken = c.String(),
                        CreatedBy = c.Int(nullable: false),
                        CreatedOn = c.DateTime(),
                        UpdatedOn = c.DateTime(),
                        LastSyncDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        StoreId = c.Int(nullable: false),
                        Flag = c.Int(nullable: false),
                        BankAccID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.QBOnlineId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .Index(t => t.StoreId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QBOnlineConfiguration", "StoreId", "dbo.StoreMaster");
            DropIndex("dbo.QBOnlineConfiguration", new[] { "StoreId" });
            DropTable("dbo.QBOnlineConfiguration");
        }
    }
}
