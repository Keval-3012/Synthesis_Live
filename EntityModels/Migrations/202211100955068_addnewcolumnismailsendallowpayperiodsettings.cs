namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewcolumnismailsendallowpayperiodsettings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PayPeriodSettings", "IsMailSendAllow", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PayPeriodSettings", "IsMailSendAllow");
        }
    }
}
