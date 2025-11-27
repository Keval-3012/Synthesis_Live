namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeforeignkeyCustomersReceiveablesReceiptsandnullablecompanynameid : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CustomersReceiveablesReceipts", "CompanyNameId", "dbo.CustomersReceiveablesManagement");
            DropIndex("dbo.CustomersReceiveablesReceipts", new[] { "CompanyNameId" });
            AlterColumn("dbo.CustomersReceiveablesReceipts", "CompanyNameId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CustomersReceiveablesReceipts", "CompanyNameId", c => c.Int(nullable: false));
            CreateIndex("dbo.CustomersReceiveablesReceipts", "CompanyNameId");
            AddForeignKey("dbo.CustomersReceiveablesReceipts", "CompanyNameId", "dbo.CustomersReceiveablesManagement", "CompanyNameId", cascadeDelete: true);
        }
    }
}
