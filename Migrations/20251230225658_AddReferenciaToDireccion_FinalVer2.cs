using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiendaApi.Migrations
{
    /// <inheritdoc />
    public partial class AddReferenciaToDireccion_FinalVer2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Referencia",
                table: "DireccionesUsuarios",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Referencia",
                table: "DireccionesUsuarios");
        }
    }
}
