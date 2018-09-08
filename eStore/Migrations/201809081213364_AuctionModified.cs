namespace eStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AuctionModified : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Auctions", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Auctions", new[] { "User_Id" });
            AlterColumn("dbo.Auctions", "Image", c => c.Binary(nullable: false));
            AlterColumn("dbo.Auctions", "DateTimeCreated", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Auctions", "State", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("dbo.Auctions", "User_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Auctions", "User_Id");
            AddForeignKey("dbo.Auctions", "User_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Auctions", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Auctions", new[] { "User_Id" });
            AlterColumn("dbo.Auctions", "User_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Auctions", "State", c => c.String(maxLength: 10));
            AlterColumn("dbo.Auctions", "DateTimeCreated", c => c.DateTime());
            AlterColumn("dbo.Auctions", "Image", c => c.Binary());
            CreateIndex("dbo.Auctions", "User_Id");
            AddForeignKey("dbo.Auctions", "User_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
