namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QBWebhook : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QBWebhook",
                c => new
                    {
                        QBWebhookId = c.Int(nullable: false, identity: true),
                        QBOnlineId = c.Int(nullable: false),
                        RealmId = c.String(),
                        Type = c.String(maxLength: 50),
                        TXNId = c.String(maxLength: 50),
                        Operation = c.String(),
                        LastUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.QBWebhookId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.QBWebhook");
        }
    }
}
