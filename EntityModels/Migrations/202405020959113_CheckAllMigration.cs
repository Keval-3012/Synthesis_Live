namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CheckAllMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HR_ConsentDetails",
                c => new
                    {
                        ConsentDetailId = c.Int(nullable: false, identity: true),
                        ConsentId = c.Int(nullable: false),
                        LanguageId = c.Int(),
                        TypeId = c.Int(),
                        Description = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.ConsentDetailId)
                .ForeignKey("dbo.HR_ConsentMaster", t => t.ConsentId, cascadeDelete: true)
                .Index(t => t.ConsentId);
            
            CreateTable(
                "dbo.HR_ConsentMaster",
                c => new
                    {
                        ConsentId = c.Int(nullable: false, identity: true),
                        FormTypeId = c.Int(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.ConsentId)
                .ForeignKey("dbo.HR_FormTypeMaster", t => t.FormTypeId, cascadeDelete: true)
                .Index(t => t.FormTypeId);
            
            CreateTable(
                "dbo.HR_FormTypeMaster",
                c => new
                    {
                        FormTypeId = c.Int(nullable: false, identity: true),
                        FormTypeName = c.String(nullable: false, maxLength: 150),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.FormTypeId);
            
            CreateTable(
                "dbo.HR_DepartmentMaster",
                c => new
                    {
                        DepartmentId = c.Int(nullable: false, identity: true),
                        DepartmentName = c.String(nullable: false, maxLength: 300),
                        IsActive = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.DepartmentId);
            
            CreateTable(
                "dbo.HR_EmployeeChild",
                c => new
                    {
                        EmployeeChildId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        SrNo = c.Int(),
                        OfficeEmployeeID = c.String(maxLength: 255),
                        HireDate = c.DateTime(),
                        TerminationDate = c.DateTime(),
                        Status = c.Int(),
                        EmployeementTypeStatus = c.Int(),
                        StoreId = c.Int(),
                        DepartmentId = c.Int(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.EmployeeChildId)
                .ForeignKey("dbo.HR_DepartmentMaster", t => t.DepartmentId)
                .ForeignKey("dbo.HR_EmployeeMaster", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId)
                .Index(t => t.DepartmentId);
            
            CreateTable(
                "dbo.HR_EmployeeDocument",
                c => new
                    {
                        EmployeeDocumentId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(),
                        EmployeeChildId = c.Int(),
                        FileName = c.String(),
                        Comments = c.String(),
                        Extension = c.String(maxLength: 10),
                        LocationFrom = c.Int(),
                        DocumentType = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.EmployeeDocumentId)
                .ForeignKey("dbo.HR_EmployeeChild", t => t.EmployeeChildId)
                .ForeignKey("dbo.HR_EmployeeMaster", t => t.EmployeeId)
                .Index(t => t.EmployeeId)
                .Index(t => t.EmployeeChildId);
            
            CreateTable(
                "dbo.HR_EmployeeMaster",
                c => new
                    {
                        EmployeeId = c.Int(nullable: false, identity: true),
                        LoginUserId = c.Int(nullable: false),
                        EmployeeUserName = c.String(nullable: false, maxLength: 100),
                        Password = c.String(nullable: false, maxLength: 50),
                        ConfirmPassword = c.String(nullable: false, maxLength: 50),
                        FirstName = c.String(nullable: false, maxLength: 100),
                        MiddleName = c.String(maxLength: 100),
                        LastName = c.String(nullable: false, maxLength: 100),
                        AdditionalLastName = c.String(maxLength: 100),
                        DateofBirth = c.DateTime(nullable: false),
                        SSN = c.String(maxLength: 50),
                        Gender = c.Int(nullable: false),
                        MaritalStatus = c.Int(nullable: false),
                        LanguageId = c.Int(nullable: false),
                        EthnicityId = c.Int(),
                        Email = c.String(maxLength: 50),
                        Phone = c.String(maxLength: 10),
                        MobileNo = c.String(maxLength: 10),
                        Street = c.String(nullable: false, maxLength: 100),
                        BuildingNo = c.String(maxLength: 20),
                        City = c.String(nullable: false, maxLength: 20),
                        State = c.Int(nullable: false),
                        ZipCode = c.String(nullable: false, maxLength: 20),
                        Designation = c.String(),
                        TraningFilePath = c.String(),
                        TraningContent = c.String(),
                        TrainingCompletedDateTime = c.DateTime(),
                        TrainingCompletedTime = c.DateTime(),
                        LastSlidename = c.String(maxLength: 50),
                        FullSSN = c.String(maxLength: 50),
                        IsTraningCompleted = c.Boolean(nullable: false),
                        UseEmailAsLogin = c.Boolean(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        IsLanguageSelect = c.Boolean(nullable: false),
                        IsFirstLogin = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.EmployeeId)
                .ForeignKey("dbo.HR_EthnicityMaster", t => t.EthnicityId)
                .Index(t => t.EthnicityId);
            
            CreateTable(
                "dbo.HR_EmployeeHealthBenefitInfo",
                c => new
                    {
                        EmployeeHealthBenefitInfoID = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(),
                        EmployeeChildId = c.Int(),
                        OtherCoverage = c.Boolean(nullable: false),
                        OtherCoverageDetail = c.String(maxLength: 255),
                        RefusedCoverage = c.Boolean(nullable: false),
                        DocFileName = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.EmployeeHealthBenefitInfoID)
                .ForeignKey("dbo.HR_EmployeeChild", t => t.EmployeeChildId)
                .ForeignKey("dbo.HR_EmployeeMaster", t => t.EmployeeId)
                .Index(t => t.EmployeeId)
                .Index(t => t.EmployeeChildId);
            
            CreateTable(
                "dbo.HR_EmployeeInsurance",
                c => new
                    {
                        EmployeeInsuranceId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(),
                        EmployeeChildId = c.Int(),
                        EffectiveDate = c.DateTime(),
                        EnrollmentStatus = c.Int(),
                        FileName = c.String(),
                        Comments = c.String(),
                        IsDeleted = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.EmployeeInsuranceId)
                .ForeignKey("dbo.HR_EmployeeChild", t => t.EmployeeChildId)
                .ForeignKey("dbo.HR_EmployeeMaster", t => t.EmployeeId)
                .Index(t => t.EmployeeId)
                .Index(t => t.EmployeeChildId);
            
            CreateTable(
                "dbo.HR_EmployeeNotes",
                c => new
                    {
                        EmployeeNoteId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(),
                        EmployeeChildId = c.Int(),
                        Notes = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.EmployeeNoteId)
                .ForeignKey("dbo.HR_EmployeeChild", t => t.EmployeeChildId)
                .ForeignKey("dbo.HR_EmployeeMaster", t => t.EmployeeId)
                .Index(t => t.EmployeeId)
                .Index(t => t.EmployeeChildId);
            
            CreateTable(
                "dbo.HR_EmployeePayRate",
                c => new
                    {
                        EmployeePayRateId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(),
                        EmployeeChildId = c.Int(),
                        PayRateDate = c.DateTime(),
                        PayType = c.Int(),
                        PayRate = c.Decimal(precision: 18, scale: 2),
                        Comments = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.EmployeePayRateId)
                .ForeignKey("dbo.HR_EmployeeChild", t => t.EmployeeChildId)
                .ForeignKey("dbo.HR_EmployeeMaster", t => t.EmployeeId)
                .Index(t => t.EmployeeId)
                .Index(t => t.EmployeeChildId);
            
            CreateTable(
                "dbo.HR_EmployeeRetirementInfo",
                c => new
                    {
                        EmployeeRetirementInfoId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(),
                        EmployeeChildId = c.Int(),
                        OptStatus = c.Int(),
                        FileName = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.EmployeeRetirementInfoId)
                .ForeignKey("dbo.HR_EmployeeChild", t => t.EmployeeChildId)
                .ForeignKey("dbo.HR_EmployeeMaster", t => t.EmployeeId)
                .Index(t => t.EmployeeId)
                .Index(t => t.EmployeeChildId);
            
            CreateTable(
                "dbo.HR_EmployeeSickTimes",
                c => new
                    {
                        EmployeeSickTimeId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(),
                        EmployeeChildId = c.Int(),
                        EffectiveDate = c.DateTime(),
                        TimeType = c.Int(),
                        Time = c.Int(),
                        Comments = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.EmployeeSickTimeId)
                .ForeignKey("dbo.HR_EmployeeChild", t => t.EmployeeChildId)
                .ForeignKey("dbo.HR_EmployeeMaster", t => t.EmployeeId)
                .Index(t => t.EmployeeId)
                .Index(t => t.EmployeeChildId);
            
            CreateTable(
                "dbo.HR_EmployeeTrainingHistory",
                c => new
                    {
                        EmployeeTrainingHistoryId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(),
                        TraningFilePath = c.String(),
                        IsTrainingCompleted = c.Boolean(nullable: false),
                        TrainingContent = c.String(),
                        TrainingCompletedDateTime = c.DateTime(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.EmployeeTrainingHistoryId)
                .ForeignKey("dbo.HR_EmployeeMaster", t => t.EmployeeId)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "dbo.HR_EmployeeVacationTimes",
                c => new
                    {
                        EmployeeVacationTimeId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(),
                        EmployeeChildId = c.Int(),
                        EffectiveDate = c.DateTime(nullable: false),
                        TimeType = c.Int(),
                        Time = c.Int(),
                        Comments = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.EmployeeVacationTimeId)
                .ForeignKey("dbo.HR_EmployeeChild", t => t.EmployeeChildId)
                .ForeignKey("dbo.HR_EmployeeMaster", t => t.EmployeeId)
                .Index(t => t.EmployeeId)
                .Index(t => t.EmployeeChildId);
            
            CreateTable(
                "dbo.HR_EmployeeVaccineCertificateInfo",
                c => new
                    {
                        VaccineCertificateInfoID = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(),
                        EmployeeChildId = c.Int(),
                        IsVaccine = c.Boolean(nullable: false),
                        IsExemption = c.Boolean(nullable: false),
                        FileName = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.VaccineCertificateInfoID)
                .ForeignKey("dbo.HR_EmployeeChild", t => t.EmployeeChildId)
                .ForeignKey("dbo.HR_EmployeeMaster", t => t.EmployeeId)
                .Index(t => t.EmployeeId)
                .Index(t => t.EmployeeChildId);
            
            CreateTable(
                "dbo.HR_EthnicityMaster",
                c => new
                    {
                        EthnicityId = c.Int(nullable: false, identity: true),
                        EthnicityName = c.String(maxLength: 255),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.Int(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.Int(),
                    })
                .PrimaryKey(t => t.EthnicityId);
            
            AddColumn("dbo.StoreMaster", "StoreManageId", c => c.Int());
            AddColumn("dbo.UserMaster", "IsHRAdmin", c => c.Boolean());
            AddColumn("dbo.UserMaster", "IsHRSuperAdmin", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HR_EmployeeMaster", "EthnicityId", "dbo.HR_EthnicityMaster");
            DropForeignKey("dbo.HR_EmployeeVaccineCertificateInfo", "EmployeeId", "dbo.HR_EmployeeMaster");
            DropForeignKey("dbo.HR_EmployeeVaccineCertificateInfo", "EmployeeChildId", "dbo.HR_EmployeeChild");
            DropForeignKey("dbo.HR_EmployeeVacationTimes", "EmployeeId", "dbo.HR_EmployeeMaster");
            DropForeignKey("dbo.HR_EmployeeVacationTimes", "EmployeeChildId", "dbo.HR_EmployeeChild");
            DropForeignKey("dbo.HR_EmployeeTrainingHistory", "EmployeeId", "dbo.HR_EmployeeMaster");
            DropForeignKey("dbo.HR_EmployeeSickTimes", "EmployeeId", "dbo.HR_EmployeeMaster");
            DropForeignKey("dbo.HR_EmployeeSickTimes", "EmployeeChildId", "dbo.HR_EmployeeChild");
            DropForeignKey("dbo.HR_EmployeeRetirementInfo", "EmployeeId", "dbo.HR_EmployeeMaster");
            DropForeignKey("dbo.HR_EmployeeRetirementInfo", "EmployeeChildId", "dbo.HR_EmployeeChild");
            DropForeignKey("dbo.HR_EmployeePayRate", "EmployeeId", "dbo.HR_EmployeeMaster");
            DropForeignKey("dbo.HR_EmployeePayRate", "EmployeeChildId", "dbo.HR_EmployeeChild");
            DropForeignKey("dbo.HR_EmployeeNotes", "EmployeeId", "dbo.HR_EmployeeMaster");
            DropForeignKey("dbo.HR_EmployeeNotes", "EmployeeChildId", "dbo.HR_EmployeeChild");
            DropForeignKey("dbo.HR_EmployeeInsurance", "EmployeeId", "dbo.HR_EmployeeMaster");
            DropForeignKey("dbo.HR_EmployeeInsurance", "EmployeeChildId", "dbo.HR_EmployeeChild");
            DropForeignKey("dbo.HR_EmployeeHealthBenefitInfo", "EmployeeId", "dbo.HR_EmployeeMaster");
            DropForeignKey("dbo.HR_EmployeeHealthBenefitInfo", "EmployeeChildId", "dbo.HR_EmployeeChild");
            DropForeignKey("dbo.HR_EmployeeDocument", "EmployeeId", "dbo.HR_EmployeeMaster");
            DropForeignKey("dbo.HR_EmployeeChild", "EmployeeId", "dbo.HR_EmployeeMaster");
            DropForeignKey("dbo.HR_EmployeeDocument", "EmployeeChildId", "dbo.HR_EmployeeChild");
            DropForeignKey("dbo.HR_EmployeeChild", "DepartmentId", "dbo.HR_DepartmentMaster");
            DropForeignKey("dbo.HR_ConsentMaster", "FormTypeId", "dbo.HR_FormTypeMaster");
            DropForeignKey("dbo.HR_ConsentDetails", "ConsentId", "dbo.HR_ConsentMaster");
            DropIndex("dbo.HR_EmployeeVaccineCertificateInfo", new[] { "EmployeeChildId" });
            DropIndex("dbo.HR_EmployeeVaccineCertificateInfo", new[] { "EmployeeId" });
            DropIndex("dbo.HR_EmployeeVacationTimes", new[] { "EmployeeChildId" });
            DropIndex("dbo.HR_EmployeeVacationTimes", new[] { "EmployeeId" });
            DropIndex("dbo.HR_EmployeeTrainingHistory", new[] { "EmployeeId" });
            DropIndex("dbo.HR_EmployeeSickTimes", new[] { "EmployeeChildId" });
            DropIndex("dbo.HR_EmployeeSickTimes", new[] { "EmployeeId" });
            DropIndex("dbo.HR_EmployeeRetirementInfo", new[] { "EmployeeChildId" });
            DropIndex("dbo.HR_EmployeeRetirementInfo", new[] { "EmployeeId" });
            DropIndex("dbo.HR_EmployeePayRate", new[] { "EmployeeChildId" });
            DropIndex("dbo.HR_EmployeePayRate", new[] { "EmployeeId" });
            DropIndex("dbo.HR_EmployeeNotes", new[] { "EmployeeChildId" });
            DropIndex("dbo.HR_EmployeeNotes", new[] { "EmployeeId" });
            DropIndex("dbo.HR_EmployeeInsurance", new[] { "EmployeeChildId" });
            DropIndex("dbo.HR_EmployeeInsurance", new[] { "EmployeeId" });
            DropIndex("dbo.HR_EmployeeHealthBenefitInfo", new[] { "EmployeeChildId" });
            DropIndex("dbo.HR_EmployeeHealthBenefitInfo", new[] { "EmployeeId" });
            DropIndex("dbo.HR_EmployeeMaster", new[] { "EthnicityId" });
            DropIndex("dbo.HR_EmployeeDocument", new[] { "EmployeeChildId" });
            DropIndex("dbo.HR_EmployeeDocument", new[] { "EmployeeId" });
            DropIndex("dbo.HR_EmployeeChild", new[] { "DepartmentId" });
            DropIndex("dbo.HR_EmployeeChild", new[] { "EmployeeId" });
            DropIndex("dbo.HR_ConsentMaster", new[] { "FormTypeId" });
            DropIndex("dbo.HR_ConsentDetails", new[] { "ConsentId" });
            DropColumn("dbo.UserMaster", "IsHRSuperAdmin");
            DropColumn("dbo.UserMaster", "IsHRAdmin");
            DropColumn("dbo.StoreMaster", "StoreManageId");
            DropTable("dbo.HR_EthnicityMaster");
            DropTable("dbo.HR_EmployeeVaccineCertificateInfo");
            DropTable("dbo.HR_EmployeeVacationTimes");
            DropTable("dbo.HR_EmployeeTrainingHistory");
            DropTable("dbo.HR_EmployeeSickTimes");
            DropTable("dbo.HR_EmployeeRetirementInfo");
            DropTable("dbo.HR_EmployeePayRate");
            DropTable("dbo.HR_EmployeeNotes");
            DropTable("dbo.HR_EmployeeInsurance");
            DropTable("dbo.HR_EmployeeHealthBenefitInfo");
            DropTable("dbo.HR_EmployeeMaster");
            DropTable("dbo.HR_EmployeeDocument");
            DropTable("dbo.HR_EmployeeChild");
            DropTable("dbo.HR_DepartmentMaster");
            DropTable("dbo.HR_FormTypeMaster");
            DropTable("dbo.HR_ConsentMaster");
            DropTable("dbo.HR_ConsentDetails");
        }
    }
}
