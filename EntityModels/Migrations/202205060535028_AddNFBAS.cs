namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNFBAS : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BankAccountSettingModels", "AccountNo", c => c.String());
            AddColumn("dbo.BankAccountSettingModels", "Balance", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.BankAccountSettingModels", "LastSyncDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BankAccountSettingModels", "LastSyncDate");
            DropColumn("dbo.BankAccountSettingModels", "Balance");
            DropColumn("dbo.BankAccountSettingModels", "AccountNo");
        }
    }
}
