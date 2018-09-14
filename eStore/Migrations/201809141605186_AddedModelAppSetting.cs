namespace eStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedModelAppSetting : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AppSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            Sql("INSERT INTO AppSettings VALUES('Currency', 'EUR')");
            Sql("INSERT INTO AppSettings VALUES('Currency', 'USD')");
            Sql("INSERT INTO AppSettings VALUES('Currency', 'RSD')");
            Sql("INSERT INTO AppSettings VALUES('CurrentCurrency', 'EUR')");
            Sql("INSERT INTO AppSettings VALUES('NumOfItemsPerPage', '10')");
            Sql("INSERT INTO AppSettings VALUES('TokenValue', '5')");
            Sql("INSERT INTO AppSettings VALUES('Duration', '3600')");

        }
        
        public override void Down()
        {
            DropTable("dbo.AppSettings");
        }
    }
}
