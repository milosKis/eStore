namespace eStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AuctionModelCreated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Auctions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Image = c.Binary(nullable: false),
                        Duration = c.Long(nullable: false),
                        StartingPrice = c.Double(nullable: false),
                        CurrentPrice = c.Double(nullable: false),
                        DateTimeCreated = c.DateTime(),
                        DateTimeClosed = c.DateTime(),
                        State = c.String(maxLength: 10),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Auctions", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Auctions", new[] { "User_Id" });
            DropTable("dbo.Auctions");
        }
    }
}
