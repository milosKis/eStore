namespace eStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifiedTypeNumberOfTokensFromIntToDouble : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "NumOfTokens", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "NumOfTokens", c => c.Int(nullable: false));
        }
    }
}
