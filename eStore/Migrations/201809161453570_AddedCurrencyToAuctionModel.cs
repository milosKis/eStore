namespace eStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCurrencyToAuctionModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Auctions", "Currency", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Auctions", "Currency");
        }
    }
}
