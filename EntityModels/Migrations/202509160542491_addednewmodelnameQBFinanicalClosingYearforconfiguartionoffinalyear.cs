namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addednewmodelnameQBFinanicalClosingYearforconfiguartionoffinalyear : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QBFinanicalClosingYear",
                c => new
                    {
                        QBFinanicalClosingYearId = c.Int(nullable: false, identity: true),
                        ClosingYear = c.String(),
                        Createdby = c.Int(),
                        CreatedDate = c.DateTime(),
                        Updatedby = c.Int(),
                        UpdatedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.QBFinanicalClosingYearId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.QBFinanicalClosingYear");
        }
    }
}
