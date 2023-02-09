namespace CryptoTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fix : DbMigration
    {
        public override void Up()
        {
            //DropTable("dbo.Samples");
            //DropTable("dbo.TokenBalances");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TokenBalances",
                c => new
                    {
                        TokenBalanceId = c.Int(nullable: false, identity: true),
                        Balance = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TokenBalanceId);
            
            CreateTable(
                "dbo.Samples",
                c => new
                    {
                        TokenBalanceId = c.Int(nullable: false, identity: true),
                        Balance = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TokenBalanceId);
            
        }
    }
}
