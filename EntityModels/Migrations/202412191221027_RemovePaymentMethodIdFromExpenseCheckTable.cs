namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovePaymentMethodIdFromExpenseCheckTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ExpenseCheck", "PaymentMethodId", "dbo.ExpensePaymentMethodMaster");
            DropIndex("dbo.ExpenseCheck", new[] { "PaymentMethodId" });
            DropColumn("dbo.ExpenseCheck", "PaymentMethodId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ExpenseCheck", "PaymentMethodId", c => c.Int());
            CreateIndex("dbo.ExpenseCheck", "PaymentMethodId");
            AddForeignKey("dbo.ExpenseCheck", "PaymentMethodId", "dbo.ExpensePaymentMethodMaster", "PaymentMethodId");
        }
    }
}
