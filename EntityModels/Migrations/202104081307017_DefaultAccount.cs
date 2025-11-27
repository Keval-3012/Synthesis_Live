namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DefaultAccount : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DefaultAccount",
                c => new
                    {
                        DefaultAccountId = c.Int(nullable: false, identity: true),
                        DefaultAccountName = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.DefaultAccountId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DefaultAccount");
        }
    }
}
