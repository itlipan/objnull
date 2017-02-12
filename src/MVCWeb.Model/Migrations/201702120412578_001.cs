namespace MVCWeb.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _001 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GitHubEvent", "RepoStarCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GitHubEvent", "RepoStarCount");
        }
    }
}
