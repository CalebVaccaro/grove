using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace grove.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserSecret : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Secret",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "address",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Secret",
                table: "Users",
                type: "TEXT",
                nullable: true);
        }
    }
}
