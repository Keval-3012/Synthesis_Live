namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OtherDepositeSetting : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OtherDepositeSetting",
                c => new
                    {
                        OtherDepositeSettingId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 200),
                        StoreId = c.Int(nullable: false),
                        BankAccountId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OtherDepositeSettingId)
                .ForeignKey("dbo.DepartmentMaster", t => t.BankAccountId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .Index(t => t.StoreId)
                .Index(t => t.BankAccountId);
            
            AlterColumn("dbo.CustomerMaster", "DisplayName", c => c.String(maxLength: 200));
            AlterColumn("dbo.CustomerMaster", "LastName", c => c.String(maxLength: 200));
            AlterColumn("dbo.CustomerMaster", "FirstName", c => c.String(maxLength: 200));
            AlterColumn("dbo.CustomerMaster", "PrimaryEmailAddr", c => c.String(maxLength: 500));
            AlterColumn("dbo.CustomerMaster", "PrimaryPhone", c => c.String(maxLength: 50));
            AlterColumn("dbo.CustomerMaster", "PrintOnCheckName", c => c.String(maxLength: 200));
            AlterColumn("dbo.CustomerMaster", "MiddleName", c => c.String(maxLength: 200));
            AlterColumn("dbo.CustomerMaster", "CompanyName", c => c.String(maxLength: 200));
            AlterColumn("dbo.CustomerMaster", "Notes", c => c.String(maxLength: 200));
            AlterColumn("dbo.CustomerMaster", "Balance", c => c.String(maxLength: 100));
            AlterColumn("dbo.CustomerMaster", "BAddress1", c => c.String(maxLength: 200));
            AlterColumn("dbo.CustomerMaster", "BAddress2", c => c.String(maxLength: 200));
            AlterColumn("dbo.CustomerMaster", "BCity", c => c.String(maxLength: 50));
            AlterColumn("dbo.CustomerMaster", "BState", c => c.String(maxLength: 50));
            AlterColumn("dbo.CustomerMaster", "BCountry", c => c.String(maxLength: 50));
            AlterColumn("dbo.CustomerMaster", "BZipCode", c => c.String(maxLength: 50));
            AlterColumn("dbo.CustomerMaster", "CreatedBy", c => c.String(maxLength: 50));
            AlterColumn("dbo.CustomerMaster", "ListID", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OtherDepositeSetting", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.OtherDepositeSetting", "BankAccountId", "dbo.DepartmentMaster");
            DropIndex("dbo.OtherDepositeSetting", new[] { "BankAccountId" });
            DropIndex("dbo.OtherDepositeSetting", new[] { "StoreId" });
            AlterColumn("dbo.CustomerMaster", "ListID", c => c.String());
            AlterColumn("dbo.CustomerMaster", "CreatedBy", c => c.String());
            AlterColumn("dbo.CustomerMaster", "BZipCode", c => c.String());
            AlterColumn("dbo.CustomerMaster", "BCountry", c => c.String());
            AlterColumn("dbo.CustomerMaster", "BState", c => c.String());
            AlterColumn("dbo.CustomerMaster", "BCity", c => c.String());
            AlterColumn("dbo.CustomerMaster", "BAddress2", c => c.String());
            AlterColumn("dbo.CustomerMaster", "BAddress1", c => c.String());
            AlterColumn("dbo.CustomerMaster", "Balance", c => c.String());
            AlterColumn("dbo.CustomerMaster", "Notes", c => c.String());
            AlterColumn("dbo.CustomerMaster", "CompanyName", c => c.String());
            AlterColumn("dbo.CustomerMaster", "MiddleName", c => c.String());
            AlterColumn("dbo.CustomerMaster", "PrintOnCheckName", c => c.String());
            AlterColumn("dbo.CustomerMaster", "PrimaryPhone", c => c.String());
            AlterColumn("dbo.CustomerMaster", "PrimaryEmailAddr", c => c.String());
            AlterColumn("dbo.CustomerMaster", "FirstName", c => c.String());
            AlterColumn("dbo.CustomerMaster", "LastName", c => c.String());
            AlterColumn("dbo.CustomerMaster", "DisplayName", c => c.String());
            DropTable("dbo.OtherDepositeSetting");
        }
    }
}
