namespace CryptoTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tokenPrice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tokens", "TokenPrice", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tokens", "TokenPrice");
        }
    }
}
