namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TopSellerNormalizationRatio : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TopSellerNormalizationRatio",
                c => new
                    {
                        TopSellerNormalizationID = c.Int(nullable: false, identity: true),
                        storeID = c.Int(nullable: false),
                        Normalizationvalue = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.TopSellerNormalizationID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TopSellerNormalizationRatio");
        }
    }
}
