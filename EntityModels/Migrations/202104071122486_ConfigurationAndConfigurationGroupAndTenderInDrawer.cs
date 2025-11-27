namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConfigurationAndConfigurationGroupAndTenderInDrawer : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ConfigurationGroup",
                c => new
                    {
                        ConfigurationGroupId = c.Int(nullable: false, identity: true),
                        GroupName = c.String(maxLength: 200),
                        TypicalBalanceId = c.Int(),
                        Memo = c.String(maxLength: 2000),
                        DepartmentId = c.Int(nullable: false),
                        StoreId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ConfigurationGroupId)
                .ForeignKey("dbo.DepartmentMaster", t => t.DepartmentId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .Index(t => t.DepartmentId)
                .Index(t => t.StoreId);
            
            CreateTable(
                "dbo.Configuration",
                c => new
                    {
                        ConfigurationId = c.Int(nullable: false, identity: true),
                        DepartmentId = c.Int(nullable: false),
                        StoreId = c.Int(nullable: false),
                        ConfigurationGroupId = c.Int(nullable: false),
                        Title = c.String(maxLength: 1000),
                        Memo = c.String(maxLength: 2000),
                        TypicalBalanceId = c.Int(),
                    })
                .PrimaryKey(t => t.ConfigurationId)
                .ForeignKey("dbo.ConfigurationGroup", t => t.ConfigurationGroupId)
                .ForeignKey("dbo.DepartmentMaster", t => t.DepartmentId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .Index(t => t.DepartmentId)
                .Index(t => t.StoreId)
                .Index(t => t.ConfigurationGroupId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ConfigurationGroup", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.ConfigurationGroup", "DepartmentId", "dbo.DepartmentMaster");
            DropForeignKey("dbo.Configuration", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.Configuration", "DepartmentId", "dbo.DepartmentMaster");
            DropForeignKey("dbo.Configuration", "ConfigurationGroupId", "dbo.ConfigurationGroup");
            DropIndex("dbo.Configuration", new[] { "ConfigurationGroupId" });
            DropIndex("dbo.Configuration", new[] { "StoreId" });
            DropIndex("dbo.Configuration", new[] { "DepartmentId" });
            DropIndex("dbo.ConfigurationGroup", new[] { "StoreId" });
            DropIndex("dbo.ConfigurationGroup", new[] { "DepartmentId" });
            DropTable("dbo.Configuration");
            DropTable("dbo.ConfigurationGroup");
        }
    }
}
