namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChecklistTableamountfieldaddded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CheckList", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.CheckList", "Is_cleared", c => c.String(maxLength: 50));
            AlterColumn("dbo.CheckList", "Account_name", c => c.String(maxLength: 500));
            AlterColumn("dbo.CheckList", "Doc_no", c => c.String(maxLength: 500));
            AlterColumn("dbo.CheckList", "EntityName", c => c.String(maxLength: 500));
            AlterColumn("dbo.CheckList", "Pmt_method", c => c.String(maxLength: 100));
            AlterColumn("dbo.CheckList", "Txn_type", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CheckList", "Txn_type", c => c.String());
            AlterColumn("dbo.CheckList", "Pmt_method", c => c.String());
            AlterColumn("dbo.CheckList", "EntityName", c => c.String());
            AlterColumn("dbo.CheckList", "Doc_no", c => c.String());
            AlterColumn("dbo.CheckList", "Account_name", c => c.String());
            AlterColumn("dbo.CheckList", "Is_cleared", c => c.String());
            DropColumn("dbo.CheckList", "Amount");
        }
    }
}
