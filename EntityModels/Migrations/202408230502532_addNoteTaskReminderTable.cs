namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addNoteTaskReminderTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserWiseNoteManages",
                c => new
                    {
                        NoteId = c.Int(nullable: false, identity: true),
                        NoteName = c.String(maxLength: 250),
                        NoteDescription = c.String(),
                        Createdby = c.Int(nullable: false),
                        CreatedOn = c.DateTime(),
                        ModifiedOn = c.DateTime(),
                        Isdeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.NoteId);
            
            CreateTable(
                "dbo.UserWiseReminderManages",
                c => new
                    {
                        ReminderId = c.Int(nullable: false, identity: true),
                        ReminderName = c.String(maxLength: 250),
                        ReminderDescription = c.String(),
                        ReminderDate = c.DateTime(nullable: false),
                        Createdby = c.Int(nullable: false),
                        CreatedOn = c.DateTime(),
                        ModifiedOn = c.DateTime(),
                        Isdeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ReminderId);
            
            CreateTable(
                "dbo.UserWiseTaskManages",
                c => new
                    {
                        TaskId = c.Int(nullable: false, identity: true),
                        TaskName = c.String(maxLength: 250),
                        TaskDescription = c.String(),
                        PriorityId = c.Int(nullable: false),
                        AssignTo = c.Int(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        IsCompleted = c.Boolean(nullable: false),
                        Createdby = c.Int(nullable: false),
                        CreatedOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.TaskId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserWiseTaskManages");
            DropTable("dbo.UserWiseReminderManages");
            DropTable("dbo.UserWiseNoteManages");
        }
    }
}
