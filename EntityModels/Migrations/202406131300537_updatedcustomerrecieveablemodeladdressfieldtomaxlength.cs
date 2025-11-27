namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedcustomerrecieveablemodeladdressfieldtomaxlength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CustomersReceiveablesManagement", "Address", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CustomersReceiveablesManagement", "Address", c => c.String(maxLength: 50));
        }
    }
}
