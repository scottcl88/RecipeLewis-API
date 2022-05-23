using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    public partial class UpdateTagsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tag_Recipes_RecipeId",
                table: "Tag");

            migrationBuilder.DropForeignKey(
                name: "FK_Tag_Users_CreatedByUserId",
                table: "Tag");

            migrationBuilder.DropForeignKey(
                name: "FK_Tag_Users_DeletedByUserId",
                table: "Tag");

            migrationBuilder.DropForeignKey(
                name: "FK_Tag_Users_ModifiedByUserId",
                table: "Tag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tag",
                table: "Tag");

            migrationBuilder.RenameTable(
                name: "Tag",
                newName: "Tags");

            migrationBuilder.RenameIndex(
                name: "IX_Tag_RecipeId",
                table: "Tags",
                newName: "IX_Tags_RecipeId");

            migrationBuilder.RenameIndex(
                name: "IX_Tag_ModifiedByUserId",
                table: "Tags",
                newName: "IX_Tags_ModifiedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Tag_DeletedByUserId",
                table: "Tags",
                newName: "IX_Tags_DeletedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Tag_CreatedByUserId",
                table: "Tags",
                newName: "IX_Tags_CreatedByUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tags",
                table: "Tags",
                column: "TagId");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 1,
                column: "CreatedDateTime",
                value: new DateTime(2022, 5, 23, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 2,
                column: "CreatedDateTime",
                value: new DateTime(2022, 5, 23, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 3,
                column: "CreatedDateTime",
                value: new DateTime(2022, 5, 23, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 4,
                column: "CreatedDateTime",
                value: new DateTime(2022, 5, 23, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 5,
                column: "CreatedDateTime",
                value: new DateTime(2022, 5, 23, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 6,
                column: "CreatedDateTime",
                value: new DateTime(2022, 5, 23, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Recipes_RecipeId",
                table: "Tags",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "RecipeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Users_CreatedByUserId",
                table: "Tags",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Users_DeletedByUserId",
                table: "Tags",
                column: "DeletedByUserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Users_ModifiedByUserId",
                table: "Tags",
                column: "ModifiedByUserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Recipes_RecipeId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Users_CreatedByUserId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Users_DeletedByUserId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Users_ModifiedByUserId",
                table: "Tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tags",
                table: "Tags");

            migrationBuilder.RenameTable(
                name: "Tags",
                newName: "Tag");

            migrationBuilder.RenameIndex(
                name: "IX_Tags_RecipeId",
                table: "Tag",
                newName: "IX_Tag_RecipeId");

            migrationBuilder.RenameIndex(
                name: "IX_Tags_ModifiedByUserId",
                table: "Tag",
                newName: "IX_Tag_ModifiedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Tags_DeletedByUserId",
                table: "Tag",
                newName: "IX_Tag_DeletedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Tags_CreatedByUserId",
                table: "Tag",
                newName: "IX_Tag_CreatedByUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tag",
                table: "Tag",
                column: "TagId");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 1,
                column: "CreatedDateTime",
                value: new DateTime(2022, 5, 23, 18, 58, 12, 534, DateTimeKind.Utc).AddTicks(9080));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 2,
                column: "CreatedDateTime",
                value: new DateTime(2022, 5, 23, 18, 58, 12, 534, DateTimeKind.Utc).AddTicks(9100));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 3,
                column: "CreatedDateTime",
                value: new DateTime(2022, 5, 23, 18, 58, 12, 534, DateTimeKind.Utc).AddTicks(9106));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 4,
                column: "CreatedDateTime",
                value: new DateTime(2022, 5, 23, 18, 58, 12, 534, DateTimeKind.Utc).AddTicks(9111));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 5,
                column: "CreatedDateTime",
                value: new DateTime(2022, 5, 23, 18, 58, 12, 534, DateTimeKind.Utc).AddTicks(9149));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 6,
                column: "CreatedDateTime",
                value: new DateTime(2022, 5, 23, 18, 58, 12, 534, DateTimeKind.Utc).AddTicks(9157));

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_Recipes_RecipeId",
                table: "Tag",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "RecipeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_Users_CreatedByUserId",
                table: "Tag",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_Users_DeletedByUserId",
                table: "Tag",
                column: "DeletedByUserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_Users_ModifiedByUserId",
                table: "Tag",
                column: "ModifiedByUserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
