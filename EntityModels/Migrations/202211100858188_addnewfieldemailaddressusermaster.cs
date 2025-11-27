namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewfieldemailaddressusermaster : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserMaster", "EmailAddress", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserMaster", "EmailAddress");
        }
    }
}
