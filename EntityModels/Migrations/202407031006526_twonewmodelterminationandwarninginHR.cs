namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class twonewmodelterminationandwarninginHR : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HR_EmployeeTermination",
                c => new
                    {
                        EmployeeTerminationId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(),
                        EmployeeChildId = c.Int(),
                        DocFileName = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.EmployeeTerminationId)
                .ForeignKey("dbo.HR_EmployeeChild", t => t.EmployeeChildId)
                .ForeignKey("dbo.HR_EmployeeMaster", t => t.EmployeeId)
                .Index(t => t.EmployeeId)
                .Index(t => t.EmployeeChildId);
            
            CreateTable(
                "dbo.HR_EmployeeWarning",
                c => new
                    {
                        EmployeeWarningId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(),
                        EmployeeChildId = c.Int(),
                        Warning = c.String(),
                        Remarks = c.String(),
                        DocFileName = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.EmployeeWarningId)
                .ForeignKey("dbo.HR_EmployeeChild", t => t.EmployeeChildId)
                .ForeignKey("dbo.HR_EmployeeMaster", t => t.EmployeeId)
                .Index(t => t.EmployeeId)
                .Index(t => t.EmployeeChildId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HR_EmployeeWarning", "EmployeeId", "dbo.HR_EmployeeMaster");
            DropForeignKey("dbo.HR_EmployeeWarning", "EmployeeChildId", "dbo.HR_EmployeeChild");
            DropForeignKey("dbo.HR_EmployeeTermination", "EmployeeId", "dbo.HR_EmployeeMaster");
            DropForeignKey("dbo.HR_EmployeeTermination", "EmployeeChildId", "dbo.HR_EmployeeChild");
            DropIndex("dbo.HR_EmployeeWarning", new[] { "EmployeeChildId" });
            DropIndex("dbo.HR_EmployeeWarning", new[] { "EmployeeId" });
            DropIndex("dbo.HR_EmployeeTermination", new[] { "EmployeeChildId" });
            DropIndex("dbo.HR_EmployeeTermination", new[] { "EmployeeId" });
            DropTable("dbo.HR_EmployeeWarning");
            DropTable("dbo.HR_EmployeeTermination");
        }
    }
}
