namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SalesGeneralEntryTables : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.OtherDeposit", new[] { "PaymentMethodId" });
            CreateTable(
                "dbo.SalesGeneralEntries",
                c => new
                    {
                        SalesGeneralId = c.Int(nullable: false, identity: true),
                        StoreId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        SalesDate = c.DateTime(),
                        CreatedDate = c.DateTime(),
                        Status = c.Int(),
                        SyncStatus = c.Int(),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NoOfPos = c.Int(),
                        TxnId = c.String(maxLength: 50),
                        IsSync = c.Int(),
                    })
                .PrimaryKey(t => t.SalesGeneralId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .ForeignKey("dbo.UserMaster", t => t.UserId)
                .Index(t => t.StoreId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.SalesChildEntries",
                c => new
                    {
                        SalesChildId = c.Int(nullable: false, identity: true),
                        SalesGeneralId = c.Int(nullable: false),
                        GroupAccountId = c.Int(),
                        DepartmentId = c.Int(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Title = c.String(maxLength: 500),
                        Memo = c.String(maxLength: 1000),
                        TypeId = c.Int(),
                    })
                .PrimaryKey(t => t.SalesChildId)
                .ForeignKey("dbo.DepartmentMaster", t => t.DepartmentId)
                .ForeignKey("dbo.SalesGeneralEntries", t => t.SalesGeneralId, cascadeDelete: true)
                .Index(t => t.SalesGeneralId)
                .Index(t => t.DepartmentId);
            
            CreateTable(
                "dbo.SalesGeneralEntriesHistory",
                c => new
                    {
                        SalesGeneralHistoryId = c.Int(nullable: false, identity: true),
                        StoreId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        SalesDate = c.DateTime(),
                        CreatedDate = c.DateTime(),
                        Status = c.Int(),
                        SyncStatus = c.Int(),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.SalesGeneralHistoryId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .ForeignKey("dbo.UserMaster", t => t.UserId)
                .Index(t => t.StoreId)
                .Index(t => t.UserId);
            
            AlterColumn("dbo.OtherDeposit", "PaymentMethodId", c => c.Int());
            CreateIndex("dbo.OtherDeposit", "PaymentMethodId");
            CreateIndex("dbo.OtherDeposit", "ShiftId");
            AddForeignKey("dbo.OtherDeposit", "ShiftId", "dbo.ShiftMaster", "ShiftId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SalesGeneralEntriesHistory", "UserId", "dbo.UserMaster");
            DropForeignKey("dbo.SalesGeneralEntriesHistory", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.SalesGeneralEntries", "UserId", "dbo.UserMaster");
            DropForeignKey("dbo.SalesGeneralEntries", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.SalesChildEntries", "SalesGeneralId", "dbo.SalesGeneralEntries");
            DropForeignKey("dbo.SalesChildEntries", "DepartmentId", "dbo.DepartmentMaster");
            DropForeignKey("dbo.OtherDeposit", "ShiftId", "dbo.ShiftMaster");
            DropIndex("dbo.SalesGeneralEntriesHistory", new[] { "UserId" });
            DropIndex("dbo.SalesGeneralEntriesHistory", new[] { "StoreId" });
            DropIndex("dbo.SalesChildEntries", new[] { "DepartmentId" });
            DropIndex("dbo.SalesChildEntries", new[] { "SalesGeneralId" });
            DropIndex("dbo.SalesGeneralEntries", new[] { "UserId" });
            DropIndex("dbo.SalesGeneralEntries", new[] { "StoreId" });
            DropIndex("dbo.OtherDeposit", new[] { "ShiftId" });
            DropIndex("dbo.OtherDeposit", new[] { "PaymentMethodId" });
            AlterColumn("dbo.OtherDeposit", "PaymentMethodId", c => c.Int(nullable: false));
            DropTable("dbo.SalesGeneralEntriesHistory");
            DropTable("dbo.SalesChildEntries");
            DropTable("dbo.SalesGeneralEntries");
            CreateIndex("dbo.OtherDeposit", "PaymentMethodId");
        }
    }
}
