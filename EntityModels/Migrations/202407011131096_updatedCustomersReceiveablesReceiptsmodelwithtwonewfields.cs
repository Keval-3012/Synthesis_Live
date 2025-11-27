namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedCustomersReceiveablesReceiptsmodelwithtwonewfields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomersReceiveablesReceipts", "IsEmailSentDate", c => c.DateTime());
            AddColumn("dbo.CustomersReceiveablesReceipts", "StoreId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomersReceiveablesReceipts", "StoreId");
            DropColumn("dbo.CustomersReceiveablesReceipts", "IsEmailSentDate");
        }
    }
}
