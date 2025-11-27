namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtableItemMovementBySupplier : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ItemMovementBySupplier",
                c => new
                    {
                        ItemMovementId = c.Int(nullable: false, identity: true),
                        SupplierName = c.String(maxLength: 150),
                        Department = c.String(maxLength: 150),
                        ItemCode = c.String(maxLength: 150),
                        ItemName = c.String(maxLength: 150),
                        QtySold = c.Decimal(nullable: false, precision: 18, scale: 3),
                        LastCOst = c.Decimal(nullable: false, precision: 18, scale: 4),
                        QtyOnHand = c.Decimal(nullable: false, precision: 18, scale: 3),
                        BasePrice = c.Decimal(nullable: false, precision: 18, scale: 4),
                        ProjMargin = c.String(maxLength: 150),
                    })
                .PrimaryKey(t => t.ItemMovementId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ItemMovementBySupplier");
        }
    }
}
