namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExpenseCheck_Setting : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExpenseCheck_Setting",
                c => new
                    {
                        ExpenseCheckId = c.Int(nullable: false, identity: true),
                        AccountId = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ExpenseCheckId)
                .ForeignKey("dbo.DepartmentMaster", t => t.AccountId)
                .Index(t => t.AccountId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ExpenseCheck_Setting", "AccountId", "dbo.DepartmentMaster");
            DropIndex("dbo.ExpenseCheck_Setting", new[] { "AccountId" });
            DropTable("dbo.ExpenseCheck_Setting");
        }
    }
}
