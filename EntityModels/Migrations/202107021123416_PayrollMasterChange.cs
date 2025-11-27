namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PayrollMasterChange : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PayrollReport", "TxnID", c => c.String(maxLength: 50));
            AddColumn("dbo.PayrollReport", "IsSync", c => c.Int());
            AddColumn("dbo.PayrollReport", "TransactionNo", c => c.String(maxLength: 50));
            AddColumn("dbo.PayrollReport", "FIleNo", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PayrollReport", "FIleNo");
            DropColumn("dbo.PayrollReport", "TransactionNo");
            DropColumn("dbo.PayrollReport", "IsSync");
            DropColumn("dbo.PayrollReport", "TxnID");
        }
    }
}
