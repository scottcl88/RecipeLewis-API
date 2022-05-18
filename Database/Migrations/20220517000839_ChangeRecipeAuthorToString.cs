using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    public partial class ChangeRecipeAuthorToString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Users_AuthorUserId",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_AuthorUserId",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "AuthorUserId",
                table: "Recipes");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "Recipes");

            migrationBuilder.AddColumn<int>(
                name: "AuthorUserId",
                table: "Recipes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_AuthorUserId",
                table: "Recipes",
                column: "AuthorUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Users_AuthorUserId",
                table: "Recipes",
                column: "AuthorUserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
