namespace eStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropNumOfTokensFromAppUser : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "NumOfTokens");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "NumOfTokens", c => c.Int(nullable: false));
        }
    }
}
