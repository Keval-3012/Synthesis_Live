namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCreditCardDetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CreditcardDetails", "Amount_CCOffline", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CreditcardDetails", "Amount_CCOffline");
        }
    }
}
