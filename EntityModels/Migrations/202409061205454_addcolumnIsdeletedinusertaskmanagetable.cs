namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnIsdeletedinusertaskmanagetable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserWiseTaskManages", "Isdeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserWiseTaskManages", "Isdeleted");
        }
    }
}
