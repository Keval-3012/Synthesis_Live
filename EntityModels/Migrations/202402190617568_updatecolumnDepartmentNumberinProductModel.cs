namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatecolumnDepartmentNumberinProductModel : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Products", "DepartmentNumber", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Products", "DepartmentNumber", c => c.Int(nullable: false));
        }
    }
}
