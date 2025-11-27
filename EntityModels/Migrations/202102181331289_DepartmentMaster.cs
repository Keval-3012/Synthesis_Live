namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DepartmentMaster : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DepartmentMaster",
                c => new
                    {
                        DepartmentId = c.Int(nullable: false, identity: true),
                        DepartmentName = c.String(nullable: false, maxLength: 300),
                        AccountNumber = c.String(maxLength: 50),
                        AccountTypeId = c.Int(nullable: false),
                        AccountDetailTypeId = c.Int(nullable: false),
                        StoreId = c.Int(),
                        IsSubAccount = c.String(maxLength: 100),
                        IsSync = c.Int(),
                        SyncDate = c.DateTime(nullable: false),
                        Description = c.String(),
                        ListId = c.String(maxLength: 50),
                        DListId = c.String(maxLength: 50),
                        IsActive = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(maxLength: 50),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.DepartmentId)
                .ForeignKey("dbo.AccountDetailTypeMaster", t => t.AccountDetailTypeId)
                .ForeignKey("dbo.AccountTypeMaster", t => t.AccountTypeId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .Index(t => t.AccountTypeId)
                .Index(t => t.AccountDetailTypeId)
                .Index(t => t.StoreId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DepartmentMaster", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.DepartmentMaster", "AccountTypeId", "dbo.AccountTypeMaster");
            DropForeignKey("dbo.DepartmentMaster", "AccountDetailTypeId", "dbo.AccountDetailTypeMaster");
            DropIndex("dbo.DepartmentMaster", new[] { "StoreId" });
            DropIndex("dbo.DepartmentMaster", new[] { "AccountDetailTypeId" });
            DropIndex("dbo.DepartmentMaster", new[] { "AccountTypeId" });
            DropTable("dbo.DepartmentMaster");
        }
    }
}
