namespace DefenceStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration111120172 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrderProducts", "OrderProduct_ID", "dbo.OrderProducts");
            DropIndex("dbo.OrderProducts", new[] { "OrderProduct_ID" });
            DropColumn("dbo.OrderProducts", "OrderProduct_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderProducts", "OrderProduct_ID", c => c.Int());
            CreateIndex("dbo.OrderProducts", "OrderProduct_ID");
            AddForeignKey("dbo.OrderProducts", "OrderProduct_ID", "dbo.OrderProducts", "ID");
        }
    }
}
