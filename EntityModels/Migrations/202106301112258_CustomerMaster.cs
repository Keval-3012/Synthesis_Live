namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomerMaster : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerMaster",
                c => new
                    {
                        CustomerId = c.Int(nullable: false, identity: true),
                        DisplayName = c.String(),
                        LastName = c.String(),
                        FirstName = c.String(),
                        PrimaryEmailAddr = c.String(),
                        PrimaryPhone = c.String(),
                        Active = c.Boolean(nullable: false),
                        PrintOnCheckName = c.String(),
                        MiddleName = c.String(),
                        CompanyName = c.String(),
                        Notes = c.String(),
                        Balance = c.String(),
                        BAddress1 = c.String(),
                        BAddress2 = c.String(),
                        BCity = c.String(),
                        BState = c.String(),
                        BCountry = c.String(),
                        BZipCode = c.String(),
                        StoreId = c.Int(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(),
                        ListID = c.String(),
                    })
                .PrimaryKey(t => t.CustomerId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CustomerMaster");
        }
    }
}
