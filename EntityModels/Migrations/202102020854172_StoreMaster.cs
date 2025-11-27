namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StoreMaster : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StoreMasters",
                c => new
                    {
                        StoreId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Address1 = c.String(nullable: false),
                        Address2 = c.String(),
                        StoreNo = c.String(nullable: false),
                        FaxNo = c.String(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        EmailId = c.String(nullable: false),
                        NickName = c.String(),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(nullable: false),
                        CreatedOn = c.DateTime(),
                        ModifiedOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.StoreId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.StoreMasters");
        }
    }
}
