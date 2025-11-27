namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExpenseCheckTxnFldTypeChange : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ExpenseCheck", "TxnDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ExpenseCheck", "TxnDate", c => c.String());
        }
    }
}
