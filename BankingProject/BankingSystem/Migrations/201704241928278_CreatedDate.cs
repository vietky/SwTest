namespace BankingSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatedDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TransactionHistories", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Users", "CreatedDate", c => c.DateTime(nullable: false));
            CreateIndex("dbo.TransactionHistories", "CreatedDate", name: "IX_TransactionHistories_CreatedDate");
        }
        
        public override void Down()
        {
            DropIndex("dbo.TransactionHistories", "IX_TransactionHistories_CreatedDate");
            DropColumn("dbo.Users", "CreatedDate");
            DropColumn("dbo.TransactionHistories", "CreatedDate");
        }
    }
}
