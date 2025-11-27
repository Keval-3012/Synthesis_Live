namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removefieldfromcreditcarddetaildaily : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CreditcardDetailsDaily", "ShiftId", "dbo.ShiftMaster");
            DropIndex("dbo.CreditcardDetailsDaily", new[] { "ShiftId" });
            DropColumn("dbo.CreditcardDetailsDaily", "ShiftId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CreditcardDetailsDaily", "ShiftId", c => c.Int());
            CreateIndex("dbo.CreditcardDetailsDaily", "ShiftId");
            AddForeignKey("dbo.CreditcardDetailsDaily", "ShiftId", "dbo.ShiftMaster", "ShiftId");
        }
    }
}
