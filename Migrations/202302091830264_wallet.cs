namespace CryptoTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class wallet : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Wallets",
                c => new
                    {
                        WalletId = c.Int(nullable: false, identity: true),
                        WalletName = c.String(),
                        WalletDescription = c.String(),
                        WalletType = c.String(),
                    })
                .PrimaryKey(t => t.WalletId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Wallets");
        }
    }
}
