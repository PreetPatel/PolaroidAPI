using Microsoft.EntityFrameworkCore.Migrations;

namespace PolaroidPostsApi.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PostItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(nullable: true),
                    ImageURL = table.Column<string>(nullable: true),
                    Caption = table.Column<string>(nullable: true),
                    Uploaded = table.Column<string>(nullable: true),
                    Likes = table.Column<int>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    AvatarURL = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostItem", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostItem");
        }
    }
}
