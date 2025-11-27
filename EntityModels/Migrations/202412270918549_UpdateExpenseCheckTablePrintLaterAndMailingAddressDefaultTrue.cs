namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateExpenseCheckTablePrintLaterAndMailingAddressDefaultTrue : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExpenseCheck", "PrintLater", c => c.Boolean(nullable: false));
            AddColumn("dbo.ExpenseCheck", "MailingAddress", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ExpenseCheck", "MailingAddress");
            DropColumn("dbo.ExpenseCheck", "PrintLater");
        }
    }
}
