namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addednewenumcolumnnamedlangauageselectioninUserMasterModule : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserMaster", "LanguageId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserMaster", "LanguageId");
        }
    }
}
