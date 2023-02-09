namespace CryptoTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newField : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WalletxTokens",
                c => new
                    {
                        WalletxTokenId = c.Int(nullable: false, identity: true),
                        WalletId = c.Int(nullable: false),
                        TokenId = c.Int(nullable: false),
                        balance = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.WalletxTokenId)
                .ForeignKey("dbo.Tokens", t => t.TokenId, cascadeDelete: true)
                .ForeignKey("dbo.Wallets", t => t.WalletId, cascadeDelete: true)
                .Index(t => t.WalletId)
                .Index(t => t.TokenId);
            
            AddColumn("dbo.Tokens", "newField", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WalletxTokens", "WalletId", "dbo.Wallets");
            DropForeignKey("dbo.WalletxTokens", "TokenId", "dbo.Tokens");
            DropIndex("dbo.WalletxTokens", new[] { "TokenId" });
            DropIndex("dbo.WalletxTokens", new[] { "WalletId" });
            DropColumn("dbo.Tokens", "newField");
            DropTable("dbo.WalletxTokens");
        }
    }
}
