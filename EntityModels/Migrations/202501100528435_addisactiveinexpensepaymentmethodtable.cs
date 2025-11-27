namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addisactiveinexpensepaymentmethodtable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExpensePaymentMethodMaster", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ExpensePaymentMethodMaster", "IsActive");
        }
    }
}
