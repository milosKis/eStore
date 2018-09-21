namespace eStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserMadeNotRequiredBidModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Bids", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Bids", new[] { "User_Id" });
            AlterColumn("dbo.Bids", "User_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Bids", "User_Id");
            AddForeignKey("dbo.Bids", "User_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bids", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Bids", new[] { "User_Id" });
            AlterColumn("dbo.Bids", "User_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Bids", "User_Id");
            AddForeignKey("dbo.Bids", "User_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
