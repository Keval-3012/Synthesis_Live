namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HomeSomeExpensesUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HomeSome_Expenses", "Status", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.HomeSome_Expenses", "Status");
        }
    }
}
