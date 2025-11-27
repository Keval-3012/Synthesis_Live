namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExpenseCheckDetailIncludeBySettingfieldadd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExpenseCheckDetail", "IncludeBySetting", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ExpenseCheckDetail", "IncludeBySetting");
        }
    }
}
