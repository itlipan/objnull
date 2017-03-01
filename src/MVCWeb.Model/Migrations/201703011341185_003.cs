namespace MVCWeb.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _003 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Resume", "UserID", c => c.Guid(nullable: false));
            CreateIndex("dbo.Resume", "UserID");
            AddForeignKey("dbo.Resume", "UserID", "dbo.NullUser", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Resume", "UserID", "dbo.NullUser");
            DropIndex("dbo.Resume", new[] { "UserID" });
            DropColumn("dbo.Resume", "UserID");
        }
    }
}
