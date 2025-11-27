namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedUniversalVendorMasterTableforUpdateFlag : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UniversalVendorMaster", "NeedsManualReview", c => c.Boolean());
            AddColumn("dbo.UniversalVendorMaster", "ManualReviewReason", c => c.String());
            AddColumn("dbo.UniversalVendorMaster", "LastUpdatedBy", c => c.Int());
            AddColumn("dbo.UniversalVendorMaster", "UpdateCount", c => c.Int());
            AddColumn("dbo.UniversalVendorMaster", "LastUpdatedOn", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UniversalVendorMaster", "LastUpdatedOn");
            DropColumn("dbo.UniversalVendorMaster", "UpdateCount");
            DropColumn("dbo.UniversalVendorMaster", "LastUpdatedBy");
            DropColumn("dbo.UniversalVendorMaster", "ManualReviewReason");
            DropColumn("dbo.UniversalVendorMaster", "NeedsManualReview");
        }
    }
}
