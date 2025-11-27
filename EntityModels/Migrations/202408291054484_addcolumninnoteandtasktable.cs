namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumninnoteandtasktable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserWiseNoteManages", "InvoiceId", c => c.Int());
            AddColumn("dbo.UserWiseTaskManages", "ModifiedOn", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserWiseTaskManages", "ModifiedOn");
            DropColumn("dbo.UserWiseNoteManages", "InvoiceId");
        }
    }
}
