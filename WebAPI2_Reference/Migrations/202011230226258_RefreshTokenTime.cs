namespace WebAPI2_Reference.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RefreshTokenTime : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RefreshTokens", "IssuedUtc", c => c.DateTime(nullable: false));
            AlterColumn("dbo.RefreshTokens", "ExpiresUtc", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RefreshTokens", "ExpiresUtc", c => c.DateTimeOffset(precision: 7));
            AlterColumn("dbo.RefreshTokens", "IssuedUtc", c => c.DateTimeOffset(precision: 7));
        }
    }
}
