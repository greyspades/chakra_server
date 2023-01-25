using FluentMigrator;

namespace Recruitment.Migrations
{
    [Migration(202125100001)]
    public class Initial_202125100001 : Migration
    {
        // Drop the tables
        public override void Down()
        {
            Delete.Table("Candidates");
            Delete.Table("Roles");
        }

        // Create the tables
        public override void Up()
        {
            Create.Table("Candidates")
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("Firstname").AsString(50).NotNullable()
                .WithColumn("Lastname").AsString(60).NotNullable()
                .WithColumn("Email").AsString(50).NotNullable()
                .WithColumn("RoleId").AsString(50).NotNullable()
                .WithColumn("Status").AsString(50).NotNullable()
                .WithColumn("StatusId").AsString(50).NotNullable();

            Create.Table("Roles")
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("Title").AsString(50).NotNullable()
                .WithColumn("Status").AsString(10).NotNullable()
                .WithColumn("Description").AsString().NotNullable();
                // .WithColumn("UserId").AsGuid().NotNullable().ForeignKey("Users", "Id");
        }
    }
}