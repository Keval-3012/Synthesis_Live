namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSalesOtherDeposite : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SalesOtherDeposite", "SalesGeneralId", "dbo.SalesGeneralEntries");
            AddForeignKey("dbo.SalesOtherDeposite", "SalesGeneralId", "dbo.SalesGeneralEntries", "SalesGeneralId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SalesOtherDeposite", "SalesGeneralId", "dbo.SalesGeneralEntries");
            AddForeignKey("dbo.SalesOtherDeposite", "SalesGeneralId", "dbo.SalesGeneralEntries", "SalesGeneralId");
        }
    }
}
