namespace eStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDateTimeOpenedDeletedFile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Auctions", "DateTimeOpened", c => c.DateTime());
            AlterColumn("dbo.Auctions", "Image", c => c.Binary());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Auctions", "Image", c => c.Binary(nullable: false));
            DropColumn("dbo.Auctions", "DateTimeOpened");
        }
    }
}
