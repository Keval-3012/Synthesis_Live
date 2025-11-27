namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addinvoiceidinUserWiseTaskManagetable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserWiseTaskManages", "InvoiceId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserWiseTaskManages", "InvoiceId");
        }
    }
}
