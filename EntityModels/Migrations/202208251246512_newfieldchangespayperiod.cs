namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newfieldchangespayperiod : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.PayPeriodSettings");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PayPeriodSettings",
                c => new
                    {
                        PayPeriodSettingId = c.Int(nullable: false, identity: true),
                        Day = c.String(),
                        CreatedBy = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.PayPeriodSettingId);
            
        }
    }
}
