namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addednewModifiedondateforQBRefnewconsoletoupdatethemaintable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DepartmentMaster", "LastModifiedOn", c => c.DateTime());
            AddColumn("dbo.Invoice", "LastModifiedOn", c => c.DateTime());
            AddColumn("dbo.VendorMaster", "LastModifiedOn", c => c.DateTime());
            AddColumn("dbo.ExpenseCheck", "LastModifiedOn", c => c.DateTime());
            DropColumn("dbo.Invoice", "QbPaymentMethod");
            DropColumn("dbo.Invoice", "QbAccountPaidName");
            DropColumn("dbo.Invoice", "QbCheckAmount");
            DropColumn("dbo.Invoice", "QbCheckNo");
            DropColumn("dbo.Invoice", "QbPaymentDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Invoice", "QbPaymentDate", c => c.DateTime());
            AddColumn("dbo.Invoice", "QbCheckNo", c => c.String());
            AddColumn("dbo.Invoice", "QbCheckAmount", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Invoice", "QbAccountPaidName", c => c.String());
            AddColumn("dbo.Invoice", "QbPaymentMethod", c => c.String());
            DropColumn("dbo.ExpenseCheck", "LastModifiedOn");
            DropColumn("dbo.VendorMaster", "LastModifiedOn");
            DropColumn("dbo.Invoice", "LastModifiedOn");
            DropColumn("dbo.DepartmentMaster", "LastModifiedOn");
        }
    }
}
