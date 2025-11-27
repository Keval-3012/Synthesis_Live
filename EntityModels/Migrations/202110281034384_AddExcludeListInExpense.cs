namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddExcludeListInExpense : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExpenseCheck_Setting", "ExcludeList", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ExpenseCheck_Setting", "ExcludeList");
        }
    }
}
