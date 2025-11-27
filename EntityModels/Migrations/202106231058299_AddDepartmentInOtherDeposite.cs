
namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDepartmentInOtherDeposite : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OtherDeposit", "DepartmentId", c => c.Int(nullable: false));
            CreateIndex("dbo.OtherDeposit", "DepartmentId");
            AddForeignKey("dbo.OtherDeposit", "DepartmentId", "dbo.DepartmentMaster", "DepartmentId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OtherDeposit", "DepartmentId", "dbo.DepartmentMaster");
            DropIndex("dbo.OtherDeposit", new[] { "DepartmentId" });
            DropColumn("dbo.OtherDeposit", "DepartmentId");
        }
    }
}
