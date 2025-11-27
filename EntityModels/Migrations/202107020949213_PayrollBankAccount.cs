namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PayrollBankAccount : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PayrollBankAccount",
                c => new
                    {
                        PayrollBankAccountId = c.Int(nullable: false, identity: true),
                        StoreId = c.Int(),
                        BankAccountId = c.Int(),
                        VendorId = c.Int(),
                    })
                .PrimaryKey(t => t.PayrollBankAccountId)
                .ForeignKey("dbo.DepartmentMaster", t => t.BankAccountId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .ForeignKey("dbo.VendorMaster", t => t.VendorId)
                .Index(t => t.StoreId)
                .Index(t => t.BankAccountId)
                .Index(t => t.VendorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PayrollBankAccount", "VendorId", "dbo.VendorMaster");
            DropForeignKey("dbo.PayrollBankAccount", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.PayrollBankAccount", "BankAccountId", "dbo.DepartmentMaster");
            DropIndex("dbo.PayrollBankAccount", new[] { "VendorId" });
            DropIndex("dbo.PayrollBankAccount", new[] { "BankAccountId" });
            DropIndex("dbo.PayrollBankAccount", new[] { "StoreId" });
            DropTable("dbo.PayrollBankAccount");
        }
    }
}
