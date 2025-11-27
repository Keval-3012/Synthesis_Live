namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedstoreIdasforeginkeyinExpensePaymentMethodMaster : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExpensePaymentMethodMaster", "StoreId", c => c.Int());
            CreateIndex("dbo.ExpensePaymentMethodMaster", "StoreId");
            AddForeignKey("dbo.ExpensePaymentMethodMaster", "StoreId", "dbo.StoreMaster", "StoreId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ExpensePaymentMethodMaster", "StoreId", "dbo.StoreMaster");
            DropIndex("dbo.ExpensePaymentMethodMaster", new[] { "StoreId" });
            DropColumn("dbo.ExpensePaymentMethodMaster", "StoreId");
        }
    }
}
