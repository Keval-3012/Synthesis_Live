namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newmodelHRStoremanageradded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HR_StoreManagers",
                c => new
                    {
                        StoreManagerId = c.Int(nullable: false, identity: true),
                        StoreId = c.Int(nullable: false),
                        FirstName = c.String(),
                        UserName = c.String(),
                        Password = c.String(),
                        ConfirmPassword = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.StoreManagerId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.HR_StoreManagers");
        }
    }
}
