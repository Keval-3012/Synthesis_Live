namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserRoleModuleIdFldAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserRoles", "ModuleId", c => c.Int());
            CreateIndex("dbo.UserRoles", "ModuleId");
            AddForeignKey("dbo.UserRoles", "ModuleId", "dbo.ModuleMaster", "ModuleId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserRoles", "ModuleId", "dbo.ModuleMaster");
            DropIndex("dbo.UserRoles", new[] { "ModuleId" });
            DropColumn("dbo.UserRoles", "ModuleId");
        }
    }
}
