namespace eStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserNotRequiredToTokenOrder : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TokenOrders", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.TokenOrders", new[] { "User_Id" });
            AlterColumn("dbo.TokenOrders", "User_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.TokenOrders", "User_Id");
            AddForeignKey("dbo.TokenOrders", "User_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TokenOrders", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.TokenOrders", new[] { "User_Id" });
            AlterColumn("dbo.TokenOrders", "User_Id", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.TokenOrders", "User_Id");
            AddForeignKey("dbo.TokenOrders", "User_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
