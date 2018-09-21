namespace eStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDateTimeCreatedToTokenOrders1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TokenOrders", "DateTimeCreated", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TokenOrders", "DateTimeCreated");
        }
    }
}
