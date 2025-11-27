namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserTypeIsViewInvoiceOnlyFldAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserTypeMaster", "IsViewInvoiceOnly", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserTypeMaster", "IsViewInvoiceOnly");
        }
    }
}
