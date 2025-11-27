namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newfieldadditemmovementsupplier : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemMovementBySupplier", "ItemMovementHistoryID", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemMovementBySupplier", "ItemMovementHistoryID");
        }
    }
}
