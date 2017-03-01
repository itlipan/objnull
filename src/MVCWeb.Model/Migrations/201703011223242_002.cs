namespace MVCWeb.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _002 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Resume",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        Public = c.Boolean(nullable: false),
                        MD = c.String(unicode: false),
                        Html = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Resume");
        }
    }
}
