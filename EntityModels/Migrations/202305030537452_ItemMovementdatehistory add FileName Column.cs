namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemMovementdatehistoryaddFileNameColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemMovementdatehistory", "FileName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ItemMovementdatehistory", "FileName");
        }
    }
}
