namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addednewtwofieldsforQBAppchangesnamedClientIdandClientSecret : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StoreMaster", "ClientID", c => c.String());
            AddColumn("dbo.StoreMaster", "ClientSecret", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StoreMaster", "ClientSecret");
            DropColumn("dbo.StoreMaster", "ClientID");
        }
    }
}
