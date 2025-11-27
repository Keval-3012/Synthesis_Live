namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removedforeignkeyinexpensecheckmoduleofvendorandpaymentmethodId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ExpenseCheck", "PaymentMethodId", "dbo.ExpensePaymentMethodMaster");
            DropForeignKey("dbo.ExpenseCheck", "VendorId", "dbo.VendorMaster");
            DropIndex("dbo.ExpenseCheck", new[] { "VendorId" });
            DropIndex("dbo.ExpenseCheck", new[] { "PaymentMethodId" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.ExpenseCheck", "PaymentMethodId");
            CreateIndex("dbo.ExpenseCheck", "VendorId");
            AddForeignKey("dbo.ExpenseCheck", "VendorId", "dbo.VendorMaster", "VendorId");
            AddForeignKey("dbo.ExpenseCheck", "PaymentMethodId", "dbo.ExpensePaymentMethodMaster", "PaymentMethodId");
        }
    }
}
