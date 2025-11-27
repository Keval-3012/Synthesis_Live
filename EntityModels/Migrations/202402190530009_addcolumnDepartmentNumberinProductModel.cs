namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnDepartmentNumberinProductModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "DepartmentNumber", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "DepartmentNumber");
        }
    }
}
