namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VendorMasterQBCopy : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VendorMaster_QBCopy",
                c => new
                    {
                        VendorId = c.Int(nullable: false, identity: true),
                        VendorName = c.String(maxLength: 300),
                        CompanyName = c.String(maxLength: 300),
                        PrintOnCheck = c.String(maxLength: 200),
                        Address = c.String(maxLength: 1000),
                        PhoneNumber = c.String(maxLength: 50),
                        Country = c.String(maxLength: 50),
                        State = c.String(maxLength: 50),
                        City = c.String(maxLength: 50),
                        PostalCode = c.String(maxLength: 50),
                        StoreId = c.Int(),
                        ListId = c.String(maxLength: 50),
                        IsActive = c.Boolean(nullable: false),
                        IsNeedUpdate = c.Boolean(nullable: false),
                        EMail = c.String(maxLength: 80),
                    })
                .PrimaryKey(t => t.VendorId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.VendorMaster_QBCopy");
        }
    }
}
