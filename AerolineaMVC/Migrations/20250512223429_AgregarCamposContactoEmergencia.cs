using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AerolineaMVC.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCamposContactoEmergencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContactoEmergencia",
                table: "Pasajeros",
                newName: "ContactoEmergenciaNombre");

            migrationBuilder.AddColumn<string>(
                name: "ContactoEmergenciaCelular",
                table: "Pasajeros",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactoEmergenciaCorreo",
                table: "Pasajeros",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactoEmergenciaCelular",
                table: "Pasajeros");

            migrationBuilder.DropColumn(
                name: "ContactoEmergenciaCorreo",
                table: "Pasajeros");

            migrationBuilder.RenameColumn(
                name: "ContactoEmergenciaNombre",
                table: "Pasajeros",
                newName: "ContactoEmergencia");
        }
    }
}
