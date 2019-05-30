using Microsoft.EntityFrameworkCore.Migrations;

namespace NewsTech.Data.Migrations
{
    public partial class Categoryupdateing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Employees_CreatorUserId",
                table: "Categories");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_AspNetUsers_CreatorUserId",
                table: "Categories",
                column: "CreatorUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_AspNetUsers_CreatorUserId",
                table: "Categories");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Employees_CreatorUserId",
                table: "Categories",
                column: "CreatorUserId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
