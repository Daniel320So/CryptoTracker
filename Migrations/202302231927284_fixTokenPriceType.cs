namespace CryptoTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixTokenPriceType : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tokens", "TokenPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tokens", "TokenPrice", c => c.Int(nullable: false));
        }
    }
}
