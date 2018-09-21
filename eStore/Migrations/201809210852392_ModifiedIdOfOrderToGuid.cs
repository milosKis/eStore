namespace eStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifiedIdOfOrderToGuid : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.TokenOrders");

            DropColumn("dbo.TokenOrders", "Id");
            AddColumn("dbo.TokenOrders", "Id", c => c.Guid(nullable: false, identity: true));

            AddPrimaryKey("dbo.TokenOrders", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.TokenOrders");

            DropColumn("dbo.TokenOrders", "Id");
            AddColumn("dbo.TokenOrders", "Id", c => c.Int(nullable: false, identity: true));

            AddPrimaryKey("dbo.TokenOrders", "Id");
        }
    }
}
