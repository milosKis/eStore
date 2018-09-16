namespace eStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTokenOrderModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TokenOrders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NumOfTokens = c.Int(nullable: false),
                        Price = c.Double(nullable: false),
                        State = c.String(nullable: false),
                        Currency = c.String(nullable: false),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TokenOrders", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.TokenOrders", new[] { "User_Id" });
            DropTable("dbo.TokenOrders");
        }
    }
}
