namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addednewcolumnupdateddateinproductstable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "UpdatedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "UpdatedDate");
        }
    }
}
