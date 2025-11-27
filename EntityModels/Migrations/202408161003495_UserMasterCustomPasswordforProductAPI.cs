namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserMasterCustomPasswordforProductAPI : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserMaster", "CustomPassword", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserMaster", "CustomPassword");
        }
    }
}
