namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccountTypeNdDetailMaster : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountDetailTypeMaster",
                c => new
                    {
                        AccountDetailTypeId = c.Int(nullable: false, identity: true),
                        AccountTypeId = c.Int(nullable: false),
                        DetailType = c.String(maxLength: 100),
                        QBDetailType = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.AccountDetailTypeId)
                .ForeignKey("dbo.AccountTypeMaster", t => t.AccountTypeId)
                .Index(t => t.AccountTypeId);
            
            CreateTable(
                "dbo.AccountTypeMaster",
                c => new
                    {
                        AccountTypeId = c.Int(nullable: false, identity: true),
                        AccountType = c.String(nullable: false, maxLength: 100),
                        Flag = c.String(maxLength: 50),
                        CommonType = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.AccountTypeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccountDetailTypeMaster", "AccountTypeId", "dbo.AccountTypeMaster");
            DropIndex("dbo.AccountDetailTypeMaster", new[] { "AccountTypeId" });
            DropTable("dbo.AccountTypeMaster");
            DropTable("dbo.AccountDetailTypeMaster");
        }
    }
}
