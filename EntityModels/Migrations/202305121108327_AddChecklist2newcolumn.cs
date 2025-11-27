namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddChecklist2newcolumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CheckList", "UserID", c => c.Int());
            AddColumn("dbo.CheckList", "UpdateDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CheckList", "UpdateDate");
            DropColumn("dbo.CheckList", "UserID");
        }
    }
}
