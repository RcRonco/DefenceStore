namespace DefenceStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration08112017 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderProducts", "Quantity", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderProducts", "Quantity");
        }
    }
}
