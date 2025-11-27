namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InsertColumnStoreID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InvoiceProduct", "StoreID", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.InvoiceProduct", "StoreID");
        }
    }
}
