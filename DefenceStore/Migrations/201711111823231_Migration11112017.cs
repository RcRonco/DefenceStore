namespace DefenceStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration11112017 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Manufactors", "TotalProduct", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Manufactors", "TotalProduct");
        }
    }
}
