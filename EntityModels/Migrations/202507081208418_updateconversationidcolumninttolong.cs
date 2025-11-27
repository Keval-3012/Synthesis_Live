namespace EntityModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateconversationidcolumninttolong : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ChatMessenger", "ConversationId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ChatMessenger", "ConversationId", c => c.Int(nullable: false));
        }
    }
}
