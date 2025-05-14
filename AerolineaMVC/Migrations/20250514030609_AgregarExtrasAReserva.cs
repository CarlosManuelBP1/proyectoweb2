using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AerolineaMVC.Migrations
{
    /// <inheritdoc />
    public partial class AgregarExtrasAReserva : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Comida",
                table: "Reservas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Maleta",
                table: "Reservas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Mascota",
                table: "Reservas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Comida",
                table: "Pasajeros",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Maleta",
                table: "Pasajeros",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Mascota",
                table: "Pasajeros",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comida",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "Maleta",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "Mascota",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "Comida",
                table: "Pasajeros");

            migrationBuilder.DropColumn(
                name: "Maleta",
                table: "Pasajeros");

            migrationBuilder.DropColumn(
                name: "Mascota",
                table: "Pasajeros");
        }
    }
}
