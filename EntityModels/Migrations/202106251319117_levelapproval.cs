namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class levelapproval : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserTypeModuleApprover",
                c => new
                    {
                        UserTypeModuleApproverId = c.Int(nullable: false, identity: true),
                        UserTypeId = c.Int(nullable: false),
                        ModuleId = c.Int(nullable: false),
                        LevelsApproverId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserTypeModuleApproverId)
                .ForeignKey("dbo.LevelsApprovers", t => t.LevelsApproverId)
                .ForeignKey("dbo.ModuleMaster", t => t.ModuleId)
                .ForeignKey("dbo.UserTypeMaster", t => t.UserTypeId)
                .Index(t => t.UserTypeId)
                .Index(t => t.ModuleId)
                .Index(t => t.LevelsApproverId);
            
            AddColumn("dbo.UserTypeMaster", "LevelsApproverId", c => c.Int());
            CreateIndex("dbo.UserTypeMaster", "LevelsApproverId");
            AddForeignKey("dbo.UserTypeMaster", "LevelsApproverId", "dbo.LevelsApprovers", "LevelsApproverId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserTypeMaster", "LevelsApproverId", "dbo.LevelsApprovers");
            DropForeignKey("dbo.UserTypeModuleApprover", "UserTypeId", "dbo.UserTypeMaster");
            DropForeignKey("dbo.UserTypeModuleApprover", "ModuleId", "dbo.ModuleMaster");
            DropForeignKey("dbo.UserTypeModuleApprover", "LevelsApproverId", "dbo.LevelsApprovers");
            DropIndex("dbo.UserTypeModuleApprover", new[] { "LevelsApproverId" });
            DropIndex("dbo.UserTypeModuleApprover", new[] { "ModuleId" });
            DropIndex("dbo.UserTypeModuleApprover", new[] { "UserTypeId" });
            DropIndex("dbo.UserTypeMaster", new[] { "LevelsApproverId" });
            DropColumn("dbo.UserTypeMaster", "LevelsApproverId");
            DropTable("dbo.UserTypeModuleApprover");
        }
    }
}
