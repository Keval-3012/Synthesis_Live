namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TypicalBalanceMaster : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TypicalBalanceMaster",
                c => new
                    {
                        TypicalBalanceId = c.Int(nullable: false, identity: true),
                        TypicalBalanceName = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.TypicalBalanceId);
            
            AlterColumn("dbo.ConfigurationGroup", "TypicalBalanceId", c => c.Int(nullable: false));
            AlterColumn("dbo.Configuration", "TypicalBalanceId", c => c.Int(nullable: false));
            CreateIndex("dbo.ConfigurationGroup", "TypicalBalanceId");
            CreateIndex("dbo.Configuration", "TypicalBalanceId");
            AddForeignKey("dbo.Configuration", "TypicalBalanceId", "dbo.TypicalBalanceMaster", "TypicalBalanceId");
            AddForeignKey("dbo.ConfigurationGroup", "TypicalBalanceId", "dbo.TypicalBalanceMaster", "TypicalBalanceId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ConfigurationGroup", "TypicalBalanceId", "dbo.TypicalBalanceMaster");
            DropForeignKey("dbo.Configuration", "TypicalBalanceId", "dbo.TypicalBalanceMaster");
            DropIndex("dbo.Configuration", new[] { "TypicalBalanceId" });
            DropIndex("dbo.ConfigurationGroup", new[] { "TypicalBalanceId" });
            AlterColumn("dbo.Configuration", "TypicalBalanceId", c => c.Int());
            AlterColumn("dbo.ConfigurationGroup", "TypicalBalanceId", c => c.Int());
            DropTable("dbo.TypicalBalanceMaster");
        }
    }
}
