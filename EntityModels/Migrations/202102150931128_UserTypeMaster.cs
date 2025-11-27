namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserTypeMaster : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserTypeMaster",
                c => new
                    {
                        UserTypeId = c.Int(nullable: false, identity: true),
                        UserType = c.String(nullable: false, maxLength: 100),
                        IsActive = c.Boolean(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                        ModifiedOn = c.DateTime(),
                        CreatedOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.UserTypeId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserTypeMaster");
        }
    }
}
