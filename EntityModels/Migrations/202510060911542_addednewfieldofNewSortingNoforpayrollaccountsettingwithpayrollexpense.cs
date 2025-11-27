namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addednewfieldofNewSortingNoforpayrollaccountsettingwithpayrollexpense : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PayrollCashAnalysis", "NewSortingNo", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PayrollCashAnalysis", "NewSortingNo");
        }
    }
}
