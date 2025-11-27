namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveColumnExpenseCheck : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ExpenseCheck", "CheckNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ExpenseCheck", "CheckNumber", c => c.String());
        }
    }
}
