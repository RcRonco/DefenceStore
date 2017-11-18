namespace DefenceStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGender : DbMigration
    {
        public override void Up()
        {
            Sql(@"
                    UPDATE dbo.Customers
                    SET Gender =
                        CASE Gender
                            WHEN 'Male' THEN 0
                            WHEN 'Female' THEN 1
                            ELSE 2
                        END
                 ");

            AlterColumn("dbo.Customers", "Gender", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Customers", "Gender", c => c.String(nullable: false));
        }
    }
}
