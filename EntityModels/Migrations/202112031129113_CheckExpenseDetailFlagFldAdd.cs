namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CheckExpenseDetailFlagFldAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExpenseCheckDetail", "Flag", c => c.Boolean(nullable: false));
            DropColumn("dbo.ExpenseCheck", "Flag");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ExpenseCheck", "Flag", c => c.Boolean(nullable: false));
            DropColumn("dbo.ExpenseCheckDetail", "Flag");
        }
    }
}
