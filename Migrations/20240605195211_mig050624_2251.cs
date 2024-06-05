using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MafiaAPI.Migrations
{
    /// <inheritdoc />
    public partial class mig050624_2251 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "PlayerStates");

            migrationBuilder.AddColumn<string>(
                name: "RoleId",
                table: "PlayerStates",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStates_RoleId",
                table: "PlayerStates",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerStates_Roles_RoleId",
                table: "PlayerStates",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerStates_Roles_RoleId",
                table: "PlayerStates");

            migrationBuilder.DropIndex(
                name: "IX_PlayerStates_RoleId",
                table: "PlayerStates");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "PlayerStates");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "PlayerStates",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
