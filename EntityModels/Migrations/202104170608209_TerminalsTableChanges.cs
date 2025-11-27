namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TerminalsTableChanges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TerminalMaster", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.CreditcardDetails", "TerminalId", "dbo.TerminalMaster");
            DropForeignKey("dbo.SalesActivitySummary", "TerminalId", "dbo.TerminalMaster");
            DropForeignKey("dbo.OtherDeposit", "TerminalId", "dbo.TerminalMaster");
            DropForeignKey("dbo.CashPaidoutInvoice", "TerminalId", "dbo.TerminalMaster");
            DropIndex("dbo.ConfigurationGroup", new[] { "TypicalBalanceId" });
            DropIndex("dbo.ConfigurationGroup", new[] { "DepartmentId" });
            DropIndex("dbo.ConfigurationGroup", new[] { "StoreId" });
            DropIndex("dbo.Configuration", new[] { "DepartmentId" });
            DropIndex("dbo.Configuration", new[] { "StoreId" });
            DropIndex("dbo.Configuration", new[] { "ConfigurationGroupId" });
            DropIndex("dbo.Configuration", new[] { "TypicalBalanceId" });
            DropIndex("dbo.CashPaidoutInvoice", new[] { "TerminalId" });
            DropIndex("dbo.OtherDeposit", new[] { "TerminalId" });
            DropIndex("dbo.SalesActivitySummary", new[] { "TerminalId" });
            DropIndex("dbo.CreditcardDetails", new[] { "TerminalId" });
            DropIndex("dbo.TerminalMaster", new[] { "StoreId" });
            DropIndex("dbo.DepartmentNetSales", new[] { "SalesActivitySummaryId" });
            DropIndex("dbo.TenderInDrawer", new[] { "SalesActivitySummaryId" });
            DropIndex("dbo.DepartmentConfiguration", new[] { "DepartmentId" });
            DropIndex("dbo.DepartmentConfiguration", new[] { "StoreId" });
            DropIndex("dbo.DepartmentConfiguration", new[] { "ConfigurationGroupId" });
            DropIndex("dbo.DepartmentConfiguration", new[] { "TypicalBalanceId" });
            CreateTable(
                "dbo.ShiftMaster",
                c => new
                    {
                        ShiftId = c.Int(nullable: false, identity: true),
                        ShiftName = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.ShiftId);
            
            CreateTable(
                "dbo.StoreTerminal",
                c => new
                    {
                        StoreTerminalId = c.Int(nullable: false, identity: true),
                        StoreId = c.Int(nullable: false),
                        TerminalId = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.StoreTerminalId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .ForeignKey("dbo.TerminalMaster", t => t.TerminalId)
                .Index(t => t.StoreId)
                .Index(t => t.TerminalId);
            
            CreateTable(
                "dbo.StorewisePDFUpload",
                c => new
                    {
                        StorewisePDFUploadId = c.Int(nullable: false, identity: true),
                        StoreId = c.Int(nullable: false),
                        CreatedOn = c.DateTime(),
                        IsSync = c.Int(nullable: false),
                        FileName = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.StorewisePDFUploadId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .Index(t => t.StoreId);
            
            AddColumn("dbo.CashPaidoutInvoice", "StoreTerminalId", c => c.Int());
            AddColumn("dbo.OtherDeposit", "StoreTerminalId", c => c.Int());
            AddColumn("dbo.SalesActivitySummary", "StoreTerminalId", c => c.Int());
            AddColumn("dbo.SalesActivitySummary", "ShiftId", c => c.Int());
            AddColumn("dbo.CreditcardDetails", "StoreTerminalId", c => c.Int());
            AddColumn("dbo.TerminalMaster", "TerminalName", c => c.String(maxLength: 500));
            AlterColumn("dbo.ConfigurationGroup", "TypicalBalanceId", c => c.Int());
            AlterColumn("dbo.ConfigurationGroup", "DepartmentId", c => c.Int());
            AlterColumn("dbo.ConfigurationGroup", "StoreId", c => c.Int());
            AlterColumn("dbo.Configuration", "DepartmentId", c => c.Int());
            AlterColumn("dbo.Configuration", "StoreId", c => c.Int());
            AlterColumn("dbo.Configuration", "ConfigurationGroupId", c => c.Int());
            AlterColumn("dbo.Configuration", "TypicalBalanceId", c => c.Int());
            AlterColumn("dbo.CreditcardDetails", "ShiftId", c => c.Int());
            AlterColumn("dbo.DepartmentNetSales", "SalesActivitySummaryId", c => c.Int());
            AlterColumn("dbo.TenderInDrawer", "SalesActivitySummaryId", c => c.Int());
            AlterColumn("dbo.DepartmentConfiguration", "DepartmentId", c => c.Int());
            AlterColumn("dbo.DepartmentConfiguration", "StoreId", c => c.Int());
            AlterColumn("dbo.DepartmentConfiguration", "ConfigurationGroupId", c => c.Int());
            AlterColumn("dbo.DepartmentConfiguration", "TypicalBalanceId", c => c.Int());
            CreateIndex("dbo.ConfigurationGroup", "TypicalBalanceId");
            CreateIndex("dbo.ConfigurationGroup", "DepartmentId");
            CreateIndex("dbo.ConfigurationGroup", "StoreId");
            CreateIndex("dbo.Configuration", "DepartmentId");
            CreateIndex("dbo.Configuration", "StoreId");
            CreateIndex("dbo.Configuration", "ConfigurationGroupId");
            CreateIndex("dbo.Configuration", "TypicalBalanceId");
            CreateIndex("dbo.CashPaidoutInvoice", "StoreTerminalId");
            CreateIndex("dbo.CashPaidoutInvoice", "ShiftId");
            CreateIndex("dbo.OtherDeposit", "StoreTerminalId");
            CreateIndex("dbo.SalesActivitySummary", "StoreTerminalId");
            CreateIndex("dbo.SalesActivitySummary", "ShiftId");
            CreateIndex("dbo.CreditcardDetails", "ShiftId");
            CreateIndex("dbo.CreditcardDetails", "StoreTerminalId");
            CreateIndex("dbo.DepartmentNetSales", "SalesActivitySummaryId");
            CreateIndex("dbo.TenderInDrawer", "SalesActivitySummaryId");
            CreateIndex("dbo.DepartmentConfiguration", "DepartmentId");
            CreateIndex("dbo.DepartmentConfiguration", "StoreId");
            CreateIndex("dbo.DepartmentConfiguration", "ConfigurationGroupId");
            CreateIndex("dbo.DepartmentConfiguration", "TypicalBalanceId");
            AddForeignKey("dbo.CreditcardDetails", "ShiftId", "dbo.ShiftMaster", "ShiftId");
            AddForeignKey("dbo.CreditcardDetails", "StoreTerminalId", "dbo.StoreTerminal", "StoreTerminalId");
            AddForeignKey("dbo.SalesActivitySummary", "ShiftId", "dbo.ShiftMaster", "ShiftId");
            AddForeignKey("dbo.SalesActivitySummary", "StoreTerminalId", "dbo.StoreTerminal", "StoreTerminalId");
            AddForeignKey("dbo.OtherDeposit", "StoreTerminalId", "dbo.StoreTerminal", "StoreTerminalId");
            AddForeignKey("dbo.CashPaidoutInvoice", "ShiftId", "dbo.ShiftMaster", "ShiftId");
            AddForeignKey("dbo.CashPaidoutInvoice", "StoreTerminalId", "dbo.StoreTerminal", "StoreTerminalId");
            DropColumn("dbo.CashPaidoutInvoice", "TerminalId");
            DropColumn("dbo.OtherDeposit", "TerminalId");
            DropColumn("dbo.SalesActivitySummary", "TerminalId");
            DropColumn("dbo.SalesActivitySummary", "ModifiedOn");
            DropColumn("dbo.SalesActivitySummary", "ModifiedBy");
            DropColumn("dbo.SalesActivitySummary", "ShiftName");
            DropColumn("dbo.CreditcardDetails", "ShiftName");
            DropColumn("dbo.CreditcardDetails", "TerminalId");
            DropColumn("dbo.TerminalMaster", "StoreId");
            DropColumn("dbo.TerminalMaster", "Terminal");
            DropColumn("dbo.TerminalMaster", "IsActive");
            DropColumn("dbo.TerminalMaster", "CreatedOn");
            DropColumn("dbo.TerminalMaster", "CreatedBy");
            DropColumn("dbo.TerminalMaster", "ModifiedOn");
            DropColumn("dbo.TerminalMaster", "ModifiedBy");
            DropColumn("dbo.DepartmentNetSales", "CreatedBy");
            DropColumn("dbo.DepartmentNetSales", "ModifiedOn");
            DropColumn("dbo.DepartmentNetSales", "ModifiedBy");
            DropColumn("dbo.PaidOut", "CreatedBy");
            DropColumn("dbo.PaidOut", "ModifiedBy");
            DropColumn("dbo.PaidOut", "ModifiedOn");
            DropColumn("dbo.TenderInDrawer", "CreatedBy");
            DropColumn("dbo.TenderInDrawer", "ModifiedOn");
            DropColumn("dbo.TenderInDrawer", "ModifiedBy");
            DropColumn("dbo.TenderInDrawer", "IsActive");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TenderInDrawer", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.TenderInDrawer", "ModifiedBy", c => c.Int());
            AddColumn("dbo.TenderInDrawer", "ModifiedOn", c => c.DateTime());
            AddColumn("dbo.TenderInDrawer", "CreatedBy", c => c.Int());
            AddColumn("dbo.PaidOut", "ModifiedOn", c => c.DateTime());
            AddColumn("dbo.PaidOut", "ModifiedBy", c => c.Int(nullable: false));
            AddColumn("dbo.PaidOut", "CreatedBy", c => c.Int(nullable: false));
            AddColumn("dbo.DepartmentNetSales", "ModifiedBy", c => c.Int());
            AddColumn("dbo.DepartmentNetSales", "ModifiedOn", c => c.DateTime());
            AddColumn("dbo.DepartmentNetSales", "CreatedBy", c => c.Int());
            AddColumn("dbo.TerminalMaster", "ModifiedBy", c => c.Int());
            AddColumn("dbo.TerminalMaster", "ModifiedOn", c => c.DateTime());
            AddColumn("dbo.TerminalMaster", "CreatedBy", c => c.Int());
            AddColumn("dbo.TerminalMaster", "CreatedOn", c => c.DateTime());
            AddColumn("dbo.TerminalMaster", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.TerminalMaster", "Terminal", c => c.String(maxLength: 500));
            AddColumn("dbo.TerminalMaster", "StoreId", c => c.Int(nullable: false));
            AddColumn("dbo.CreditcardDetails", "TerminalId", c => c.Int());
            AddColumn("dbo.CreditcardDetails", "ShiftName", c => c.String());
            AddColumn("dbo.SalesActivitySummary", "ShiftName", c => c.String(maxLength: 100));
            AddColumn("dbo.SalesActivitySummary", "ModifiedBy", c => c.Int());
            AddColumn("dbo.SalesActivitySummary", "ModifiedOn", c => c.DateTime());
            AddColumn("dbo.SalesActivitySummary", "TerminalId", c => c.Int(nullable: false));
            AddColumn("dbo.OtherDeposit", "TerminalId", c => c.Int());
            AddColumn("dbo.CashPaidoutInvoice", "TerminalId", c => c.Int());
            DropForeignKey("dbo.StorewisePDFUpload", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.CashPaidoutInvoice", "StoreTerminalId", "dbo.StoreTerminal");
            DropForeignKey("dbo.CashPaidoutInvoice", "ShiftId", "dbo.ShiftMaster");
            DropForeignKey("dbo.OtherDeposit", "StoreTerminalId", "dbo.StoreTerminal");
            DropForeignKey("dbo.SalesActivitySummary", "StoreTerminalId", "dbo.StoreTerminal");
            DropForeignKey("dbo.SalesActivitySummary", "ShiftId", "dbo.ShiftMaster");
            DropForeignKey("dbo.CreditcardDetails", "StoreTerminalId", "dbo.StoreTerminal");
            DropForeignKey("dbo.StoreTerminal", "TerminalId", "dbo.TerminalMaster");
            DropForeignKey("dbo.StoreTerminal", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.CreditcardDetails", "ShiftId", "dbo.ShiftMaster");
            DropIndex("dbo.StorewisePDFUpload", new[] { "StoreId" });
            DropIndex("dbo.DepartmentConfiguration", new[] { "TypicalBalanceId" });
            DropIndex("dbo.DepartmentConfiguration", new[] { "ConfigurationGroupId" });
            DropIndex("dbo.DepartmentConfiguration", new[] { "StoreId" });
            DropIndex("dbo.DepartmentConfiguration", new[] { "DepartmentId" });
            DropIndex("dbo.TenderInDrawer", new[] { "SalesActivitySummaryId" });
            DropIndex("dbo.DepartmentNetSales", new[] { "SalesActivitySummaryId" });
            DropIndex("dbo.StoreTerminal", new[] { "TerminalId" });
            DropIndex("dbo.StoreTerminal", new[] { "StoreId" });
            DropIndex("dbo.CreditcardDetails", new[] { "StoreTerminalId" });
            DropIndex("dbo.CreditcardDetails", new[] { "ShiftId" });
            DropIndex("dbo.SalesActivitySummary", new[] { "ShiftId" });
            DropIndex("dbo.SalesActivitySummary", new[] { "StoreTerminalId" });
            DropIndex("dbo.OtherDeposit", new[] { "StoreTerminalId" });
            DropIndex("dbo.CashPaidoutInvoice", new[] { "ShiftId" });
            DropIndex("dbo.CashPaidoutInvoice", new[] { "StoreTerminalId" });
            DropIndex("dbo.Configuration", new[] { "TypicalBalanceId" });
            DropIndex("dbo.Configuration", new[] { "ConfigurationGroupId" });
            DropIndex("dbo.Configuration", new[] { "StoreId" });
            DropIndex("dbo.Configuration", new[] { "DepartmentId" });
            DropIndex("dbo.ConfigurationGroup", new[] { "StoreId" });
            DropIndex("dbo.ConfigurationGroup", new[] { "DepartmentId" });
            DropIndex("dbo.ConfigurationGroup", new[] { "TypicalBalanceId" });
            AlterColumn("dbo.DepartmentConfiguration", "TypicalBalanceId", c => c.Int(nullable: false));
            AlterColumn("dbo.DepartmentConfiguration", "ConfigurationGroupId", c => c.Int(nullable: false));
            AlterColumn("dbo.DepartmentConfiguration", "StoreId", c => c.Int(nullable: false));
            AlterColumn("dbo.DepartmentConfiguration", "DepartmentId", c => c.Int(nullable: false));
            AlterColumn("dbo.TenderInDrawer", "SalesActivitySummaryId", c => c.Int(nullable: false));
            AlterColumn("dbo.DepartmentNetSales", "SalesActivitySummaryId", c => c.Int(nullable: false));
            AlterColumn("dbo.CreditcardDetails", "ShiftId", c => c.Int(nullable: false));
            AlterColumn("dbo.Configuration", "TypicalBalanceId", c => c.Int(nullable: false));
            AlterColumn("dbo.Configuration", "ConfigurationGroupId", c => c.Int(nullable: false));
            AlterColumn("dbo.Configuration", "StoreId", c => c.Int(nullable: false));
            AlterColumn("dbo.Configuration", "DepartmentId", c => c.Int(nullable: false));
            AlterColumn("dbo.ConfigurationGroup", "StoreId", c => c.Int(nullable: false));
            AlterColumn("dbo.ConfigurationGroup", "DepartmentId", c => c.Int(nullable: false));
            AlterColumn("dbo.ConfigurationGroup", "TypicalBalanceId", c => c.Int(nullable: false));
            DropColumn("dbo.TerminalMaster", "TerminalName");
            DropColumn("dbo.CreditcardDetails", "StoreTerminalId");
            DropColumn("dbo.SalesActivitySummary", "ShiftId");
            DropColumn("dbo.SalesActivitySummary", "StoreTerminalId");
            DropColumn("dbo.OtherDeposit", "StoreTerminalId");
            DropColumn("dbo.CashPaidoutInvoice", "StoreTerminalId");
            DropTable("dbo.StorewisePDFUpload");
            DropTable("dbo.StoreTerminal");
            DropTable("dbo.ShiftMaster");
            CreateIndex("dbo.DepartmentConfiguration", "TypicalBalanceId");
            CreateIndex("dbo.DepartmentConfiguration", "ConfigurationGroupId");
            CreateIndex("dbo.DepartmentConfiguration", "StoreId");
            CreateIndex("dbo.DepartmentConfiguration", "DepartmentId");
            CreateIndex("dbo.TenderInDrawer", "SalesActivitySummaryId");
            CreateIndex("dbo.DepartmentNetSales", "SalesActivitySummaryId");
            CreateIndex("dbo.TerminalMaster", "StoreId");
            CreateIndex("dbo.CreditcardDetails", "TerminalId");
            CreateIndex("dbo.SalesActivitySummary", "TerminalId");
            CreateIndex("dbo.OtherDeposit", "TerminalId");
            CreateIndex("dbo.CashPaidoutInvoice", "TerminalId");
            CreateIndex("dbo.Configuration", "TypicalBalanceId");
            CreateIndex("dbo.Configuration", "ConfigurationGroupId");
            CreateIndex("dbo.Configuration", "StoreId");
            CreateIndex("dbo.Configuration", "DepartmentId");
            CreateIndex("dbo.ConfigurationGroup", "StoreId");
            CreateIndex("dbo.ConfigurationGroup", "DepartmentId");
            CreateIndex("dbo.ConfigurationGroup", "TypicalBalanceId");
            AddForeignKey("dbo.CashPaidoutInvoice", "TerminalId", "dbo.TerminalMaster", "TerminalId");
            AddForeignKey("dbo.OtherDeposit", "TerminalId", "dbo.TerminalMaster", "TerminalId");
            AddForeignKey("dbo.SalesActivitySummary", "TerminalId", "dbo.TerminalMaster", "TerminalId");
            AddForeignKey("dbo.CreditcardDetails", "TerminalId", "dbo.TerminalMaster", "TerminalId");
            AddForeignKey("dbo.TerminalMaster", "StoreId", "dbo.StoreMaster", "StoreId");
        }
    }
}
