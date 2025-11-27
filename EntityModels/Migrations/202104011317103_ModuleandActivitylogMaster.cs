namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModuleandActivitylogMaster : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActivityLog",
                c => new
                    {
                        ActivityLogId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Comment = c.String(maxLength: 1000),
                        Action = c.Int(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.ActivityLogId);
            
            CreateTable(
                "dbo.ModuleMaster",
                c => new
                    {
                        ModuleId = c.Int(nullable: false, identity: true),
                        ModuleNo = c.Int(nullable: false),
                        ModuleName = c.String(),
                        DisplayName = c.String(),
                    })
                .PrimaryKey(t => t.ModuleId);

           
        }
        
        public override void Down()
        {
            DropTable("dbo.ModuleMaster");
            DropTable("dbo.ActivityLog");
        }
    }
}
