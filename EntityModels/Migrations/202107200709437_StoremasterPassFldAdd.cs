namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StoremasterPassFldAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StoreMaster", "Password", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StoreMaster", "Password");
        }
    }
}
