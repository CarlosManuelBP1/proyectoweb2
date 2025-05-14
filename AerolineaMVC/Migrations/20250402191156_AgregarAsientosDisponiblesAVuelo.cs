using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AerolineaMVC.Migrations
{
    /// <inheritdoc />
    public partial class AgregarAsientosDisponiblesAVuelo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AsientosDisponibles",
                table: "Vuelos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AsientosDisponibles",
                table: "Vuelos");
        }
    }
}
