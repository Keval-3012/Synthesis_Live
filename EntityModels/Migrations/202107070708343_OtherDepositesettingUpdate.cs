namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OtherDepositesettingUpdate : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.OtherDepositeSetting", new[] { "BankAccountId" });
            AlterColumn("dbo.OtherDepositeSetting", "BankAccountId", c => c.Int());
            CreateIndex("dbo.OtherDepositeSetting", "BankAccountId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.OtherDepositeSetting", new[] { "BankAccountId" });
            AlterColumn("dbo.OtherDepositeSetting", "BankAccountId", c => c.Int(nullable: false));
            CreateIndex("dbo.OtherDepositeSetting", "BankAccountId");
        }
    }
}
