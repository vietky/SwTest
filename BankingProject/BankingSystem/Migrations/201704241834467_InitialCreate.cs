namespace BankingSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TransactionHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountNumber = c.Long(nullable: false),
                        Action = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OldBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NewBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.AccountNumber, name: "IX_TransactionHistories_AccountNumber");
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountNumber = c.Long(nullable: false),
                        AccountName = c.String(),
                        Password = c.String(),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.AccountNumber, unique: true, name: "IX_Users_AccountNumber");
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Users", "IX_Users_AccountNumber");
            DropIndex("dbo.TransactionHistories", "IX_TransactionHistories_AccountNumber");
            DropTable("dbo.Users");
            DropTable("dbo.TransactionHistories");
        }
    }
}
