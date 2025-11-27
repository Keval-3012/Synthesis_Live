namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddItemLibraryDepartment : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ItemLibraryDepartments",
                c => new
                    {
                        ItemLibraryDepartmentId = c.Int(nullable: false, identity: true),
                        ItemLibraryDepartmentName = c.String(),
                    })
                .PrimaryKey(t => t.ItemLibraryDepartmentId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ItemLibraryDepartments");
        }
    }
}
