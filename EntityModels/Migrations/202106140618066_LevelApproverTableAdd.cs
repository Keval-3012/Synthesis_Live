namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LevelApproverTableAdd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LevelsApprovers",
                c => new
                    {
                        LevelsApproverId = c.Int(nullable: false, identity: true),
                        LevelName = c.String(nullable: false, maxLength: 200),
                        LevelSortName = c.String(nullable: false, maxLength: 50),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(),
                        ModifiedOn = c.DateTime(),
                        CreatedOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.LevelsApproverId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.LevelsApprovers");
        }
    }
}
