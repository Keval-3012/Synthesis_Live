namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedExpenseCheckModuleforaddupdateanddeletefunctionality : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExpenseCheck", "PaymentMethodId", c => c.Int());
            AddColumn("dbo.ExpenseCheck", "CreatedBy", c => c.Int());
            AddColumn("dbo.ExpenseCheck", "UpdatedBy", c => c.Int());
            AddColumn("dbo.ExpenseCheck", "UpdatedDate", c => c.DateTime());
            AddColumn("dbo.ExpenseCheck", "Isdeleted", c => c.Boolean());
            CreateIndex("dbo.ExpenseCheck", "PaymentMethodId");
            AddForeignKey("dbo.ExpenseCheck", "PaymentMethodId", "dbo.ExpensePaymentMethodMaster", "PaymentMethodId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ExpenseCheck", "PaymentMethodId", "dbo.ExpensePaymentMethodMaster");
            DropIndex("dbo.ExpenseCheck", new[] { "PaymentMethodId" });
            DropColumn("dbo.ExpenseCheck", "Isdeleted");
            DropColumn("dbo.ExpenseCheck", "UpdatedDate");
            DropColumn("dbo.ExpenseCheck", "UpdatedBy");
            DropColumn("dbo.ExpenseCheck", "CreatedBy");
            DropColumn("dbo.ExpenseCheck", "PaymentMethodId");
        }
    }
}
