namespace SynthesisCF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VendorDepartmentRelationMaster : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VendorDepartmentRelationMaster",
                c => new
                    {
                        VendorDepartmentRelationId = c.Int(nullable: false, identity: true),
                        VendorId = c.Int(nullable: false),
                        StoreId = c.Int(nullable: false),
                        DepartmentId = c.Int(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.VendorDepartmentRelationId)
                .ForeignKey("dbo.DepartmentMaster", t => t.DepartmentId)
                .ForeignKey("dbo.StoreMaster", t => t.StoreId)
                .ForeignKey("dbo.VendorMaster", t => t.VendorId)
                .Index(t => t.VendorId)
                .Index(t => t.StoreId)
                .Index(t => t.DepartmentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VendorDepartmentRelationMaster", "VendorId", "dbo.VendorMaster");
            DropForeignKey("dbo.VendorDepartmentRelationMaster", "StoreId", "dbo.StoreMaster");
            DropForeignKey("dbo.VendorDepartmentRelationMaster", "DepartmentId", "dbo.DepartmentMaster");
            DropIndex("dbo.VendorDepartmentRelationMaster", new[] { "DepartmentId" });
            DropIndex("dbo.VendorDepartmentRelationMaster", new[] { "StoreId" });
            DropIndex("dbo.VendorDepartmentRelationMaster", new[] { "VendorId" });
            DropTable("dbo.VendorDepartmentRelationMaster");
        }
    }
}
