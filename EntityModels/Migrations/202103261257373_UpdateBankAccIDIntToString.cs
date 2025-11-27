namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateBankAccIDIntToString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.QBDesktopConfiguration", "BankAccID", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.QBDesktopConfiguration", "BankAccID", c => c.Int(nullable: false));
        }
    }
}
