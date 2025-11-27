namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QBDesktopConfiguration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QBDesktopConfiguration",
                c => new
                    {
                        QBDesktopId = c.Int(nullable: false, identity: true),
                        WebCompanyID = c.Int(nullable: false),
                        QBCompanyPath = c.String(),
                        UserName = c.String(maxLength: 50),
                        Password = c.String(maxLength: 50),
                        CreatedBy = c.Int(nullable: false),
                        CreatedOn = c.DateTime(),
                        UpdatedOn = c.DateTime(),
                        LastSyncDate = c.DateTime(),
                        OwnerID = c.Guid(nullable: false),
                        FileID = c.Guid(nullable: false),
                        AppName = c.String(maxLength: 50),
                        Description = c.String(maxLength: 100),
                        IsActive = c.Boolean(nullable: false),
                        StoreId = c.Int(nullable: false),
                        Flag = c.Int(nullable: false),
                        BankAccID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.QBDesktopId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .Index(t => t.StoreId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QBDesktopConfiguration", "StoreId", "dbo.StoreMaster");
            DropIndex("dbo.QBDesktopConfiguration", new[] { "StoreId" });
            DropTable("dbo.QBDesktopConfiguration");
        }
    }
}
