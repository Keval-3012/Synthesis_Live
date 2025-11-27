namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FieldaddTxnID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CheckList", "TxnId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CheckList", "TxnId");
        }
    }
}
