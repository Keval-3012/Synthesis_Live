namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChecklistTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CheckList",
                c => new
                    {
                        ChecklistId = c.Int(nullable: false, identity: true),
                        Is_cleared = c.String(),
                        Account_name = c.String(),
                        Tx_Date = c.DateTime(nullable: false),
                        Doc_no = c.String(),
                        Name = c.String(),
                        Id = c.Int(nullable: false),
                        Pmt_method = c.String(),
                        Txn_type = c.String(),
                        StoreId = c.Int(nullable: false),
                        MailSent = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ChecklistId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CheckList");
        }
    }
}
