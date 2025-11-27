namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedmenumasterwithnewcolumnnameMenuRoleList : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MenuMaster", "MenuRoleList", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MenuMaster", "MenuRoleList");
        }
    }
}
