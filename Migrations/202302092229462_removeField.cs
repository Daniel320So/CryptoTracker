namespace CryptoTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeField : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Tokens", "newField");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tokens", "newField", c => c.String());
        }
    }
}
