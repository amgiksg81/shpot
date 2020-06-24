namespace SHPOT_DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PremiumFlow : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserPremiums",
                c => new
                    {
                        UserPremiumlID = c.Int(nullable: false, identity: true),
                        ClientID = c.String(maxLength: 500),
                        ClienToken = c.String(maxLength: 500),
                        PaymentMode = c.String(maxLength: 15),
                        Amount = c.Double(nullable: false),
                        CreatedDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                        UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserPremiumlID)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID);
            
            AddColumn("dbo.Businesses", "PremiumImageContent", c => c.String(maxLength: 2000));
            AddColumn("dbo.BusinessImages", "IsPremiumImage", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserPremiums", "UserID", "dbo.Users");
            DropIndex("dbo.UserPremiums", new[] { "UserID" });
            DropColumn("dbo.BusinessImages", "IsPremiumImage");
            DropColumn("dbo.Businesses", "PremiumImageContent");
            DropTable("dbo.UserPremiums");
        }
    }
}
