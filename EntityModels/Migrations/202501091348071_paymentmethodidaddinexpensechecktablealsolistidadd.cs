namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class paymentmethodidaddinexpensechecktablealsolistidadd : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ExpenseCheckMaster", "PaymentMethodId", "dbo.ExpensePaymentMethodMaster");
            DropIndex("dbo.ExpenseCheckMaster", new[] { "PaymentMethodId" });
            AddColumn("dbo.ExpenseCheck", "PaymentMethodId", c => c.Int());
            AddColumn("dbo.ExpensePaymentMethodMaster", "ListId", c => c.String(maxLength: 50));
            CreateIndex("dbo.ExpenseCheck", "PaymentMethodId");
            AddForeignKey("dbo.ExpenseCheck", "PaymentMethodId", "dbo.ExpensePaymentMethodMaster", "PaymentMethodId");
            DropColumn("dbo.ExpensePaymentMethodMaster", "PaymentQBId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ExpensePaymentMethodMaster", "PaymentQBId", c => c.Int(nullable: false));
            DropForeignKey("dbo.ExpenseCheck", "PaymentMethodId", "dbo.ExpensePaymentMethodMaster");
            DropIndex("dbo.ExpenseCheck", new[] { "PaymentMethodId" });
            DropColumn("dbo.ExpensePaymentMethodMaster", "ListId");
            DropColumn("dbo.ExpenseCheck", "PaymentMethodId");
            CreateIndex("dbo.ExpenseCheckMaster", "PaymentMethodId");
            AddForeignKey("dbo.ExpenseCheckMaster", "PaymentMethodId", "dbo.ExpensePaymentMethodMaster", "PaymentMethodId");
        }
    }
}
