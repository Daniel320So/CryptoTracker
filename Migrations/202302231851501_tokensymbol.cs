namespace CryptoTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tokensymbol : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tokens", "TokenSymbol", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tokens", "TokenSymbol");
        }
    }
}
