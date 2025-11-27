namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StateNdVendorMaster : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VendorMaster",
                c => new
                    {
                        VendorId = c.Int(nullable: false, identity: true),
                        VendorName = c.String(nullable: false, maxLength: 300),
                        CompanyName = c.String(maxLength: 300),
                        PrintOnCheck = c.String(maxLength: 200),
                        Address = c.String(maxLength: 1000),
                        Address2 = c.String(maxLength: 1000),
                        PhoneNumber = c.String(maxLength: 50),
                        Country = c.String(maxLength: 50),
                        StateId = c.Int(),
                        City = c.String(maxLength: 50),
                        PostalCode = c.String(maxLength: 50),
                        StoreId = c.Int(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(maxLength: 50),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.String(maxLength: 50),
                        ListId = c.String(maxLength: 50),
                        DListId = c.String(maxLength: 50),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.VendorId)
                .ForeignKey("dbo.StateMaster", t => t.StateId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .Index(t => t.StateId)
                .Index(t => t.StoreId);
            
            CreateTable(
                "dbo.StateMaster",
                c => new
                    {
                        StateId = c.Int(nullable: false, identity: true),
                        StateName = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.StateId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VendorMaster", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.VendorMaster", "StateId", "dbo.StateMaster");
            DropIndex("dbo.VendorMaster", new[] { "StoreId" });
            DropIndex("dbo.VendorMaster", new[] { "StateId" });
            DropTable("dbo.StateMaster");
            DropTable("dbo.VendorMaster");
        }
    }
}
