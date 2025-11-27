namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class storemasterupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StoreMaster", "Location", c => c.String());
            AddColumn("dbo.StoreMaster", "CloudFrontDomain", c => c.String());
            AddColumn("dbo.StoreMaster", "BucketName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StoreMaster", "BucketName");
            DropColumn("dbo.StoreMaster", "CloudFrontDomain");
            DropColumn("dbo.StoreMaster", "Location");
        }
    }
}
