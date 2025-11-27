namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RightsStore : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RightsStores",
                c => new
                    {
                        RightsStoreId = c.Int(nullable: false, identity: true),
                        ModuleId = c.Int(nullable: false),
                        UserTypeId = c.Int(nullable: false),
                        StoreId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RightsStoreId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RightsStores");
        }
    }
}
