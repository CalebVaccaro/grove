using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace grove.Migrations.EventDbMigrations
{
    /// <inheritdoc />
    public partial class AddressImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Secret",
                table: "Events",
                newName: "Image");

            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "Events",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "address",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Events",
                newName: "Secret");
        }
    }
}
