namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ManageIpAddressAllModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IpAdressInfo",
                c => new
                    {
                        IpAdressInfoID = c.Int(nullable: false, identity: true),
                        StoreId = c.Int(nullable: false),
                        StaticIp = c.String(),
                        StartIP = c.String(),
                        EndIP = c.String(),
                        Location = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.IpAdressInfoID);
            
            CreateTable(
                "dbo.Logins",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        SessionId = c.String(),
                        LoggedIn = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserIpInfo",
                c => new
                    {
                        UserInfoID = c.Int(nullable: false, identity: true),
                        IpAdressInfoID = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserInfoID);
            
            CreateTable(
                "dbo.UserTimeTrackInfoes",
                c => new
                    {
                        UserTimeTrackInfoID = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        StartTime = c.DateTime(),
                        Location = c.String(),
                        IpAddress = c.String(),
                        ActivityType = c.Int(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.Int(),
                        InOutType = c.Int(),
                    })
                .PrimaryKey(t => t.UserTimeTrackInfoID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserTimeTrackInfoes");
            DropTable("dbo.UserIpInfo");
            DropTable("dbo.Logins");
            DropTable("dbo.IpAdressInfo");
        }
    }
}
