namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewfieldStateMaster : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StateMaster", "StateCode", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StateMaster", "StateCode");
        }
    }
}
