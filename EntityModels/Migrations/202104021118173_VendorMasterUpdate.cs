namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VendorMasterUpdate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.VendorMaster", "StateId", "dbo.StateMaster");
            DropIndex("dbo.VendorMaster", new[] { "StateId" });
            AddColumn("dbo.VendorMaster", "State", c => c.String(maxLength: 50));
            AddColumn("dbo.VendorMaster", "IsSync", c => c.Boolean(nullable: false));
            AddColumn("dbo.VendorMaster", "SyncDate", c => c.DateTime());
            AlterColumn("dbo.VendorMaster", "CreatedBy", c => c.Int());
            AlterColumn("dbo.VendorMaster", "ModifiedBy", c => c.Int());
            DropColumn("dbo.VendorMaster", "StateId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VendorMaster", "StateId", c => c.Int());
            AlterColumn("dbo.VendorMaster", "ModifiedBy", c => c.String(maxLength: 50));
            AlterColumn("dbo.VendorMaster", "CreatedBy", c => c.String(maxLength: 50));
            DropColumn("dbo.VendorMaster", "SyncDate");
            DropColumn("dbo.VendorMaster", "IsSync");
            DropColumn("dbo.VendorMaster", "State");
            CreateIndex("dbo.VendorMaster", "StateId");
            AddForeignKey("dbo.VendorMaster", "StateId", "dbo.StateMaster", "StateId");
        }
    }
}
