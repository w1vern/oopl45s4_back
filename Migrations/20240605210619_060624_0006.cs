using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MafiaAPI.Migrations
{
    /// <inheritdoc />
    public partial class _060624_0006 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Roles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "currentState",
                table: "Matches",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "Roles",
                columns: ["Id", "Name", "Description", "Priority"],
                values: new object[,]
                {
                    { Guid.NewGuid().ToString(), "Host", "GOD", -1 },
                    { Guid.NewGuid().ToString(), "Citizen", "Just Simple Citizen", -1 },
                    { Guid.NewGuid().ToString(), "Mafia", "Serial Killer!", 1 },
                    { Guid.NewGuid().ToString(), "Doctor", "Can rescue lives?", 2 },
                    { Guid.NewGuid().ToString(), "Sheriff", "Make citizens live a lil longer.", 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "currentState",
                table: "Matches");
        }
    }
}
