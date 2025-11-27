namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddtableProducts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductId = c.Int(nullable: false, identity: true),
                        UPCCode = c.String(maxLength: 300),
                        ItemNo = c.String(maxLength: 500),
                        SynthesisId = c.String(maxLength: 100),
                        Description = c.String(maxLength: 1000),
                        Vendor = c.String(maxLength: 3000),
                        Departments = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.ProductId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Products");
        }
    }
}
