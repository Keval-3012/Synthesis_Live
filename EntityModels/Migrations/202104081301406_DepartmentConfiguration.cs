namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DepartmentConfiguration : DbMigration
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
            CreateTable(
                "dbo.DepartmentConfiguration",
                c => new
                    {
                        DepartmentConfigurationId = c.Int(nullable: false, identity: true),
                        DepartmentId = c.Int(nullable: false),
                        StoreId = c.Int(nullable: false),
                        ConfigurationGroupId = c.Int(nullable: false),
                        Title = c.String(maxLength: 1000),
                        Memo = c.String(maxLength: 2000),
                        TypicalBalanceId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DepartmentConfigurationId)
                .ForeignKey("dbo.ConfigurationGroup", t => t.ConfigurationGroupId)
                .ForeignKey("dbo.DepartmentMaster", t => t.DepartmentId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .ForeignKey("dbo.TypicalBalanceMaster", t => t.TypicalBalanceId)
                .Index(t => t.DepartmentId)
                .Index(t => t.StoreId)
                .Index(t => t.ConfigurationGroupId)
                .Index(t => t.TypicalBalanceId);
            
            AlterColumn("dbo.ConfigurationGroup", "TypicalBalanceId", c => c.Int(nullable: false));
            AlterColumn("dbo.ConfigurationGroup", "DepartmentId", c => c.Int(nullable: false));
            AlterColumn("dbo.ConfigurationGroup", "StoreId", c => c.Int(nullable: false));
            AlterColumn("dbo.Configuration", "DepartmentId", c => c.Int(nullable: false));
            AlterColumn("dbo.Configuration", "StoreId", c => c.Int(nullable: false));
            AlterColumn("dbo.Configuration", "ConfigurationGroupId", c => c.Int(nullable: false));
            AlterColumn("dbo.Configuration", "TypicalBalanceId", c => c.Int(nullable: false));
            AlterColumn("dbo.SalesActivitySummary", "StoreId", c => c.Int(nullable: false));
            AlterColumn("dbo.SalesActivitySummary", "TerminalId", c => c.Int(nullable: false));
            AlterColumn("dbo.DepartmentNetSales", "SalesActivitySummaryId", c => c.Int(nullable: false));
            AlterColumn("dbo.TenderInDrawer", "SalesActivitySummaryId", c => c.Int(nullable: false));
            AlterColumn("dbo.TerminalMaster", "StoreId", c => c.Int(nullable: false));
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
            DropForeignKey("dbo.DepartmentConfiguration", "TypicalBalanceId", "dbo.TypicalBalanceMaster");
            DropForeignKey("dbo.DepartmentConfiguration", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.DepartmentConfiguration", "DepartmentId", "dbo.DepartmentMaster");
            DropForeignKey("dbo.DepartmentConfiguration", "ConfigurationGroupId", "dbo.ConfigurationGroup");
            DropIndex("dbo.TerminalMaster", new[] { "StoreId" });
            DropIndex("dbo.TenderInDrawer", new[] { "SalesActivitySummaryId" });
            DropIndex("dbo.DepartmentNetSales", new[] { "SalesActivitySummaryId" });
            DropIndex("dbo.SalesActivitySummary", new[] { "TerminalId" });
            DropIndex("dbo.SalesActivitySummary", new[] { "StoreId" });
            DropIndex("dbo.DepartmentConfiguration", new[] { "TypicalBalanceId" });
            DropIndex("dbo.DepartmentConfiguration", new[] { "ConfigurationGroupId" });
            DropIndex("dbo.DepartmentConfiguration", new[] { "StoreId" });
            DropIndex("dbo.DepartmentConfiguration", new[] { "DepartmentId" });
            DropIndex("dbo.Configuration", new[] { "TypicalBalanceId" });
            DropIndex("dbo.Configuration", new[] { "ConfigurationGroupId" });
            DropIndex("dbo.Configuration", new[] { "StoreId" });
            DropIndex("dbo.Configuration", new[] { "DepartmentId" });
            DropIndex("dbo.ConfigurationGroup", new[] { "StoreId" });
            DropIndex("dbo.ConfigurationGroup", new[] { "DepartmentId" });
            DropIndex("dbo.ConfigurationGroup", new[] { "TypicalBalanceId" });
            AlterColumn("dbo.TerminalMaster", "StoreId", c => c.Int());
            AlterColumn("dbo.TenderInDrawer", "SalesActivitySummaryId", c => c.Int());
            AlterColumn("dbo.DepartmentNetSales", "SalesActivitySummaryId", c => c.Int());
            AlterColumn("dbo.SalesActivitySummary", "TerminalId", c => c.Int());
            AlterColumn("dbo.SalesActivitySummary", "StoreId", c => c.Int());
            AlterColumn("dbo.Configuration", "TypicalBalanceId", c => c.Int());
            AlterColumn("dbo.Configuration", "ConfigurationGroupId", c => c.Int());
            AlterColumn("dbo.Configuration", "StoreId", c => c.Int());
            AlterColumn("dbo.Configuration", "DepartmentId", c => c.Int());
            AlterColumn("dbo.ConfigurationGroup", "StoreId", c => c.Int());
            AlterColumn("dbo.ConfigurationGroup", "DepartmentId", c => c.Int());
            AlterColumn("dbo.ConfigurationGroup", "TypicalBalanceId", c => c.Int());
            DropTable("dbo.DepartmentConfiguration");
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
