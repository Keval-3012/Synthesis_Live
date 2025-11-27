namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class twonewmodelCustomerRecieveablesandApiKeyConfig : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApiKeyConfiguartion",
                c => new
                    {
                        APIKeyId = c.Int(nullable: false, identity: true),
                        APIKeyName = c.String(maxLength: 200),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.APIKeyId);
            
            CreateTable(
                "dbo.CustomersReceiveablesManagement",
                c => new
                    {
                        CompanyNameId = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(nullable: false, maxLength: 100),
                        ContactPersonName = c.String(maxLength: 50),
                        EmailAddress = c.String(nullable: false, maxLength: 50),
                        PhoneNumber = c.String(maxLength: 50),
                        Address = c.String(maxLength: 50),
                        StoreId = c.Int(),
                    })
                .PrimaryKey(t => t.CompanyNameId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .Index(t => t.StoreId);
            
            CreateTable(
                "dbo.CustomersReceiveablesReceipts",
                c => new
                    {
                        CustomersReceiveablesReceiptsId = c.Int(nullable: false, identity: true),
                        CompanyNameId = c.Int(nullable: false),
                        FileName = c.String(maxLength: 250),
                        IsEmailTriggered = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CustomersReceiveablesReceiptsId)
                .ForeignKey("dbo.CustomersReceiveablesManagement", t => t.CompanyNameId, cascadeDelete: true)
                .Index(t => t.CompanyNameId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomersReceiveablesReceipts", "CompanyNameId", "dbo.CustomersReceiveablesManagement");
            DropForeignKey("dbo.CustomersReceiveablesManagement", "StoreId", "dbo.StoreMaster");
            DropIndex("dbo.CustomersReceiveablesReceipts", new[] { "CompanyNameId" });
            DropIndex("dbo.CustomersReceiveablesManagement", new[] { "StoreId" });
            DropTable("dbo.CustomersReceiveablesReceipts");
            DropTable("dbo.CustomersReceiveablesManagement");
            DropTable("dbo.ApiKeyConfiguartion");
        }
    }
}
