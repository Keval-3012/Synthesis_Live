namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addstoreidinexpensechecksetting : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExpenseCheck_Setting", "StoreId", c => c.Int());
            CreateIndex("dbo.ExpenseCheck_Setting", "StoreId");
            AddForeignKey("dbo.ExpenseCheck_Setting", "StoreId", "dbo.StoreMaster", "StoreId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ExpenseCheck_Setting", "StoreId", "dbo.StoreMaster");
            DropIndex("dbo.ExpenseCheck_Setting", new[] { "StoreId" });
            DropColumn("dbo.ExpenseCheck_Setting", "StoreId");
        }
    }
}
