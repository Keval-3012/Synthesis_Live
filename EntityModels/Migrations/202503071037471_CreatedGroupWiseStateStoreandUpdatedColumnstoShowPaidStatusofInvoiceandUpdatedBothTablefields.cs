namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatedGroupWiseStateStoreandUpdatedColumnstoShowPaidStatusofInvoiceandUpdatedBothTablefields : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GroupWiseStateStore",
                c => new
                    {
                        GroupWiseStateStoreId = c.Int(nullable: false, identity: true),
                        GroupName = c.String(),
                        StoreName = c.String(),
                        CreatedBy = c.Int(),
                        CreatedDate = c.DateTime(),
                        ModifiedBy = c.Int(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.GroupWiseStateStoreId);
            
            AddColumn("dbo.Invoice", "QbPaymentMethod", c => c.String());
            AddColumn("dbo.Invoice", "QbAccountPaidName", c => c.String());
            AddColumn("dbo.Invoice", "QbCheckNo", c => c.String());
            AddColumn("dbo.Invoice", "QbPaymentDate", c => c.DateTime());
            AddColumn("dbo.Invoice", "IsPaid", c => c.Boolean());
            AddColumn("dbo.UserMaster", "GroupWiseStateStoreId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserMaster", "GroupWiseStateStoreId");
            DropColumn("dbo.Invoice", "IsPaid");
            DropColumn("dbo.Invoice", "QbPaymentDate");
            DropColumn("dbo.Invoice", "QbCheckNo");
            DropColumn("dbo.Invoice", "QbAccountPaidName");
            DropColumn("dbo.Invoice", "QbPaymentMethod");
            DropTable("dbo.GroupWiseStateStore");
        }
    }
}
