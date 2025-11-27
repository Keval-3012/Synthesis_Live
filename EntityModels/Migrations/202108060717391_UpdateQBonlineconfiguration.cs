namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateQBonlineconfiguration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.QBOnlineConfiguration", "IsTokenSuspend", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.QBOnlineConfiguration", "IsTokenSuspend");
        }
    }
}
