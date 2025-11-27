namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserWiseStickyNotesTblAdd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserWiseStickyNotes",
                c => new
                    {
                        UserWiseStickyNoteId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Notes = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserWiseStickyNoteId)
                .ForeignKey("dbo.UserMaster", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserWiseStickyNotes", "UserId", "dbo.UserMaster");
            DropIndex("dbo.UserWiseStickyNotes", new[] { "UserId" });
            DropTable("dbo.UserWiseStickyNotes");
        }
    }
}
