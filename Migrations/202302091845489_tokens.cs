namespace CryptoTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tokens : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tokens",
                c => new
                    {
                        TokenID = c.Int(nullable: false, identity: true),
                        TokenName = c.String(),
                        TokenDescription = c.String(),
                        TokenRiskLevel = c.String(),
                    })
                .PrimaryKey(t => t.TokenID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Tokens");
        }
    }
}
