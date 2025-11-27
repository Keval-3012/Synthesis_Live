namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BankAccountSetting : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BankAccountSettingModels",
                c => new
                    {
                        BankAccountID = c.Int(nullable: false, identity: true),
                        ItemID = c.String(),
                        AccessToken = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.BankAccountID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BankAccountSettingModels");
        }
    }
}
