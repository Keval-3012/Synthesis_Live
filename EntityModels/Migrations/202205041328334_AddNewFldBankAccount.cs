namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewFldBankAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BankAccountSettingModels", "StoreID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BankAccountSettingModels", "StoreID");
        }
    }
}
