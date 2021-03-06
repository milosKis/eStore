namespace eStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifiedAuctionModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Auctions", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Auctions", new[] { "User_Id" });
            AlterColumn("dbo.Auctions", "User_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Auctions", "User_Id");
            AddForeignKey("dbo.Auctions", "User_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Auctions", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Auctions", new[] { "User_Id" });
            AlterColumn("dbo.Auctions", "User_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Auctions", "User_Id");
            AddForeignKey("dbo.Auctions", "User_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
