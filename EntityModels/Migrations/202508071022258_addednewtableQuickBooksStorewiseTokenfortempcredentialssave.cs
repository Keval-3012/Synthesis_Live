namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addednewtableQuickBooksStorewiseTokenfortempcredentialssave : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QuickBooksStorewiseToken",
                c => new
                    {
                        QuickBooksStorewiseTokenId = c.Int(nullable: false, identity: true),
                        StoreID = c.Int(nullable: false),
                        ClientID = c.String(),
                        ClientSecret = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.QuickBooksStorewiseTokenId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.QuickBooksStorewiseToken");
        }
    }
}
