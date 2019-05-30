using Microsoft.EntityFrameworkCore.Migrations;

namespace NewsTech.Data.Migrations
{
    public partial class contentCreatorIdUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contents_Employees_CreatedUserId1",
                table: "Contents");

            migrationBuilder.DropIndex(
                name: "IX_Contents_CreatedUserId1",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "CreatedUserId1",
                table: "Contents");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedUserId",
                table: "Contents",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateIndex(
                name: "IX_Contents_CreatedUserId",
                table: "Contents",
                column: "CreatedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contents_Employees_CreatedUserId",
                table: "Contents",
                column: "CreatedUserId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contents_Employees_CreatedUserId",
                table: "Contents");

            migrationBuilder.DropIndex(
                name: "IX_Contents_CreatedUserId",
                table: "Contents");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedUserId",
                table: "Contents",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedUserId1",
                table: "Contents",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contents_CreatedUserId1",
                table: "Contents",
                column: "CreatedUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Contents_Employees_CreatedUserId1",
                table: "Contents",
                column: "CreatedUserId1",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
