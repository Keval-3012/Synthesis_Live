namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addExpensePaymentMethodMasterTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExpensePaymentMethodMaster",
                c => new
                    {
                        PaymentMethodId = c.Int(nullable: false, identity: true),
                        PaymentMethod = c.String(maxLength: 200),
                        PaymentQBId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PaymentMethodId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ExpensePaymentMethodMaster");
        }
    }
}
