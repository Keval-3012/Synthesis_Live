namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateOtherDepositedeptID : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.OtherDeposit", new[] { "DepartmentId" });
            AlterColumn("dbo.OtherDeposit", "DepartmentId", c => c.Int());
            CreateIndex("dbo.OtherDeposit", "DepartmentId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.OtherDeposit", new[] { "DepartmentId" });
            AlterColumn("dbo.OtherDeposit", "DepartmentId", c => c.Int(nullable: false));
            CreateIndex("dbo.OtherDeposit", "DepartmentId");
        }
    }
}
