namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PayperiodSettingtableUpdate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PayPeriodSettings",
                c => new
                    {
                        PayPeriodSettingId = c.Int(nullable: false, identity: true),
                        Day = c.Int(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.PayPeriodSettingId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PayPeriodSettings");
        }
    }
}
