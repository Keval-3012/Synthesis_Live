namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RightsModuleFkAdd : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.RightsStores", "ModuleId");
            AddForeignKey("dbo.RightsStores", "ModuleId", "dbo.ModuleMaster", "ModuleId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RightsStores", "ModuleId", "dbo.ModuleMaster");
            DropIndex("dbo.RightsStores", new[] { "ModuleId" });
        }
    }
}
