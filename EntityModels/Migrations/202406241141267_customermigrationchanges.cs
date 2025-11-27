namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class customermigrationchanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomersReceiveablesReceipts", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.CustomersReceiveablesReceipts", "DeletedDateTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomersReceiveablesReceipts", "DeletedDateTime");
            DropColumn("dbo.CustomersReceiveablesReceipts", "IsDeleted");
        }
    }
}
