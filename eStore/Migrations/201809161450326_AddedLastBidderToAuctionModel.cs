namespace eStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedLastBidderToAuctionModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Auctions", "LastBidder_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Auctions", "LastBidder_Id");
            AddForeignKey("dbo.Auctions", "LastBidder_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Auctions", "LastBidder_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Auctions", new[] { "LastBidder_Id" });
            DropColumn("dbo.Auctions", "LastBidder_Id");
        }
    }
}
