namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addqberrorandpaymentdateinexpensechecktable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExpenseCheck", "PaymentDate", c => c.DateTime());
            AddColumn("dbo.ExpenseCheck", "QBErrorMessage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ExpenseCheck", "QBErrorMessage");
            DropColumn("dbo.ExpenseCheck", "PaymentDate");
        }
    }
}
