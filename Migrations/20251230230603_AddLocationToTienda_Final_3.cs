using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiendaApi.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationToTienda_Final_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ciudad",
                table: "Tiendas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Departamento",
                table: "Tiendas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Direccion",
                table: "Tiendas",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pais",
                table: "Tiendas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ciudad",
                table: "Tiendas");

            migrationBuilder.DropColumn(
                name: "Departamento",
                table: "Tiendas");

            migrationBuilder.DropColumn(
                name: "Direccion",
                table: "Tiendas");

            migrationBuilder.DropColumn(
                name: "Pais",
                table: "Tiendas");
        }
    }
}
