namespace eStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SqlSilverGoldPlatinum : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO AppSettings VALUES('Silver', '30')");
            Sql("INSERT INTO AppSettings VALUES('Gold', '50')");
            Sql("INSERT INTO AppSettings VALUES('Platinum', '100')");
        }
        
        public override void Down()
        {
        }
    }
}
