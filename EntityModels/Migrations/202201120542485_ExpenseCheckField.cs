namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExpenseCheckField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExpenseCheck", "QBType", c => c.String());
            AddColumn("dbo.ExpenseCheck", "RefType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ExpenseCheck", "RefType");
            DropColumn("dbo.ExpenseCheck", "QBType");
        }
    }
}
