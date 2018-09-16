namespace eStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedBidModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bids",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateTimeCreated = c.DateTime(nullable: false),
                        NumOfTokens = c.Double(nullable: false),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bids", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Bids", new[] { "User_Id" });
            DropTable("dbo.Bids");
        }
    }
}
