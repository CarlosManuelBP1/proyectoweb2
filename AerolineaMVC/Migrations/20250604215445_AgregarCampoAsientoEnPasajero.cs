using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AerolineaMVC.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCampoAsientoEnPasajero : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Asiento",
                table: "Pasajeros",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Asiento",
                table: "Pasajeros");
        }
    }
}
