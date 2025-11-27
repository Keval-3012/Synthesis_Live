namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateExpenseCheckTableAddIsSyncField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExpenseCheck", "IsSync", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ExpenseCheck", "IsSync");
        }
    }
}
