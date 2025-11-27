namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addforeingnkeyinstoremasterandaccountinvendor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StoreMaster", "StateID", c => c.Int());
            AddColumn("dbo.VendorMaster", "AccountNumber", c => c.String(maxLength: 60));
            CreateIndex("dbo.StoreMaster", "StateID");
            AddForeignKey("dbo.StoreMaster", "StateID", "dbo.StateMaster", "StateId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StoreMaster", "StateID", "dbo.StateMaster");
            DropIndex("dbo.StoreMaster", new[] { "StateID" });
            DropColumn("dbo.VendorMaster", "AccountNumber");
            DropColumn("dbo.StoreMaster", "StateID");
        }
    }
}
