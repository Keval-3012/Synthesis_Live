namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetFkNullable : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ConfigurationGroup", new[] { "TypicalBalanceId" });
            DropIndex("dbo.ConfigurationGroup", new[] { "DepartmentId" });
            DropIndex("dbo.ConfigurationGroup", new[] { "StoreId" });
            DropIndex("dbo.Configuration", new[] { "DepartmentId" });
            DropIndex("dbo.Configuration", new[] { "StoreId" });
            DropIndex("dbo.Configuration", new[] { "ConfigurationGroupId" });
            DropIndex("dbo.Configuration", new[] { "TypicalBalanceId" });
            DropIndex("dbo.SalesActivitySummary", new[] { "StoreId" });
            DropIndex("dbo.SalesActivitySummary", new[] { "TerminalId" });
            DropIndex("dbo.DepartmentNetSales", new[] { "SalesActivitySummaryId" });
            DropIndex("dbo.TenderInDrawer", new[] { "SalesActivitySummaryId" });
            DropIndex("dbo.TerminalMaster", new[] { "StoreId" });
            AlterColumn("dbo.ConfigurationGroup", "TypicalBalanceId", c => c.Int());
            AlterColumn("dbo.ConfigurationGroup", "DepartmentId", c => c.Int());
            AlterColumn("dbo.ConfigurationGroup", "StoreId", c => c.Int());
            AlterColumn("dbo.Configuration", "DepartmentId", c => c.Int());
            AlterColumn("dbo.Configuration", "StoreId", c => c.Int());
            AlterColumn("dbo.Configuration", "ConfigurationGroupId", c => c.Int());
            AlterColumn("dbo.Configuration", "TypicalBalanceId", c => c.Int());
            AlterColumn("dbo.SalesActivitySummary", "StoreId", c => c.Int());
            AlterColumn("dbo.SalesActivitySummary", "TerminalId", c => c.Int());
            AlterColumn("dbo.DepartmentNetSales", "SalesActivitySummaryId", c => c.Int());
            AlterColumn("dbo.TenderInDrawer", "SalesActivitySummaryId", c => c.Int());
            AlterColumn("dbo.TerminalMaster", "StoreId", c => c.Int());
            CreateIndex("dbo.ConfigurationGroup", "TypicalBalanceId");
            CreateIndex("dbo.ConfigurationGroup", "DepartmentId");
            CreateIndex("dbo.ConfigurationGroup", "StoreId");
            CreateIndex("dbo.Configuration", "DepartmentId");
            CreateIndex("dbo.Configuration", "StoreId");
            CreateIndex("dbo.Configuration", "ConfigurationGroupId");
            CreateIndex("dbo.Configuration", "TypicalBalanceId");
            CreateIndex("dbo.SalesActivitySummary", "StoreId");
            CreateIndex("dbo.SalesActivitySummary", "TerminalId");
            CreateIndex("dbo.DepartmentNetSales", "SalesActivitySummaryId");
            CreateIndex("dbo.TenderInDrawer", "SalesActivitySummaryId");
            CreateIndex("dbo.TerminalMaster", "StoreId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.TerminalMaster", new[] { "StoreId" });
            DropIndex("dbo.TenderInDrawer", new[] { "SalesActivitySummaryId" });
            DropIndex("dbo.DepartmentNetSales", new[] { "SalesActivitySummaryId" });
            DropIndex("dbo.SalesActivitySummary", new[] { "TerminalId" });
            DropIndex("dbo.SalesActivitySummary", new[] { "StoreId" });
            DropIndex("dbo.Configuration", new[] { "TypicalBalanceId" });
            DropIndex("dbo.Configuration", new[] { "ConfigurationGroupId" });
            DropIndex("dbo.Configuration", new[] { "StoreId" });
            DropIndex("dbo.Configuration", new[] { "DepartmentId" });
            DropIndex("dbo.ConfigurationGroup", new[] { "StoreId" });
            DropIndex("dbo.ConfigurationGroup", new[] { "DepartmentId" });
            DropIndex("dbo.ConfigurationGroup", new[] { "TypicalBalanceId" });
            AlterColumn("dbo.TerminalMaster", "StoreId", c => c.Int(nullable: false));
            AlterColumn("dbo.TenderInDrawer", "SalesActivitySummaryId", c => c.Int(nullable: false));
            AlterColumn("dbo.DepartmentNetSales", "SalesActivitySummaryId", c => c.Int(nullable: false));
            AlterColumn("dbo.SalesActivitySummary", "TerminalId", c => c.Int(nullable: false));
            AlterColumn("dbo.SalesActivitySummary", "StoreId", c => c.Int(nullable: false));
            AlterColumn("dbo.Configuration", "TypicalBalanceId", c => c.Int(nullable: false));
            AlterColumn("dbo.Configuration", "ConfigurationGroupId", c => c.Int(nullable: false));
            AlterColumn("dbo.Configuration", "StoreId", c => c.Int(nullable: false));
            AlterColumn("dbo.Configuration", "DepartmentId", c => c.Int(nullable: false));
            AlterColumn("dbo.ConfigurationGroup", "StoreId", c => c.Int(nullable: false));
            AlterColumn("dbo.ConfigurationGroup", "DepartmentId", c => c.Int(nullable: false));
            AlterColumn("dbo.ConfigurationGroup", "TypicalBalanceId", c => c.Int(nullable: false));
            CreateIndex("dbo.TerminalMaster", "StoreId");
            CreateIndex("dbo.TenderInDrawer", "SalesActivitySummaryId");
            CreateIndex("dbo.DepartmentNetSales", "SalesActivitySummaryId");
            CreateIndex("dbo.SalesActivitySummary", "TerminalId");
            CreateIndex("dbo.SalesActivitySummary", "StoreId");
            CreateIndex("dbo.Configuration", "TypicalBalanceId");
            CreateIndex("dbo.Configuration", "ConfigurationGroupId");
            CreateIndex("dbo.Configuration", "StoreId");
            CreateIndex("dbo.Configuration", "DepartmentId");
            CreateIndex("dbo.ConfigurationGroup", "StoreId");
            CreateIndex("dbo.ConfigurationGroup", "DepartmentId");
            CreateIndex("dbo.ConfigurationGroup", "TypicalBalanceId");
        }
    }
}
