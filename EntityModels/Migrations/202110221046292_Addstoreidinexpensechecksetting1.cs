namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addstoreidinexpensechecksetting1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExpenseCheck_Setting", "Type", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ExpenseCheck_Setting", "Type");
        }
    }
}
