namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTestVendorMaster : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TestVendorMasters",
                c => new
                    {
                        VendorId = c.Int(nullable: false, identity: true),
                        VendorName = c.String(nullable: false, maxLength: 300),
                        CompanyName = c.String(maxLength: 300),
                        PrintOnCheck = c.String(maxLength: 200),
                        Address = c.String(maxLength: 1000),
                        Address2 = c.String(maxLength: 1000),
                        PhoneNumber = c.String(maxLength: 50),
                        AccountNumber = c.String(maxLength: 60),
                        Country = c.String(maxLength: 50),
                        State = c.String(maxLength: 50),
                        City = c.String(maxLength: 50),
                        PostalCode = c.String(maxLength: 50),
                        StoreId = c.Int(),
                        EMail = c.String(maxLength: 80),
                        Instruction = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ListId = c.String(maxLength: 50),
                        DListId = c.String(maxLength: 50),
                        IsActive = c.Boolean(nullable: false),
                        IsSync = c.Boolean(nullable: false),
                        SyncDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.VendorId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TestVendorMasters");
        }
    }
}
