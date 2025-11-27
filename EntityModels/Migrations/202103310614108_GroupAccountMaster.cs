namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GroupAccountMaster : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GroupAccountMaster",
                c => new
                    {
                        GroupAccountId = c.Int(nullable: false, identity: true),
                        AccountName = c.String(nullable: false, maxLength: 200),
                        TypicalBalanceId = c.Int(nullable: false),
                        Memo = c.String(maxLength: 2000),
                        DepartmentId = c.Int(nullable: false),
                        StoreId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.GroupAccountId)
                .ForeignKey("dbo.DepartmentMaster", t => t.DepartmentId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .Index(t => t.DepartmentId)
                .Index(t => t.StoreId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GroupAccountMaster", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.GroupAccountMaster", "DepartmentId", "dbo.DepartmentMaster");
            DropIndex("dbo.GroupAccountMaster", new[] { "StoreId" });
            DropIndex("dbo.GroupAccountMaster", new[] { "DepartmentId" });
            DropTable("dbo.GroupAccountMaster");
        }
    }
}
