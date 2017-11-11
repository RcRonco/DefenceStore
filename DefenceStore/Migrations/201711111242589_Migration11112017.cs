namespace DefenceStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration11112017 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderProducts", "OrderProduct_ID", c => c.Int());
            CreateIndex("dbo.OrderProducts", "OrderProduct_ID");
            AddForeignKey("dbo.OrderProducts", "OrderProduct_ID", "dbo.OrderProducts", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderProducts", "OrderProduct_ID", "dbo.OrderProducts");
            DropIndex("dbo.OrderProducts", new[] { "OrderProduct_ID" });
            DropColumn("dbo.OrderProducts", "OrderProduct_ID");
        }
    }
}
