using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiendaApi.Migrations
{
    /// <inheritdoc />
    public partial class AddUltimaSesionProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Usuarios_UsuarioId",
                table: "Pedidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Tiendas_Usuarios_UsuarioId",
                table: "Tiendas");

            migrationBuilder.DropForeignKey(
                name: "FK_Transacciones_Usuarios_UsuarioId",
                table: "Transacciones");

            migrationBuilder.DropIndex(
                name: "IX_Pedidos_UsuarioId",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Pedidos");

            migrationBuilder.AddColumn<string>(
                name: "ModoTema",
                table: "Usuarios",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimaSesion",
                table: "Usuarios",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Apto",
                table: "Tiendas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bloque",
                table: "Tiendas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentoIdentidad",
                table: "Tiendas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Tiendas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HoraApertura",
                table: "Tiendas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HoraCierre",
                table: "Tiendas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Latitud",
                table: "Tiendas",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitud",
                table: "Tiendas",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinutoApertura",
                table: "Tiendas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinutoCierre",
                table: "Tiendas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Referencia",
                table: "Tiendas",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Tiendas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoTiendaId",
                table: "Tiendas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Torre",
                table: "Tiendas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TipoTiendas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Icono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoTiendas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tiendas_TipoTiendaId",
                table: "Tiendas",
                column: "TipoTiendaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tiendas_TipoTiendas_TipoTiendaId",
                table: "Tiendas",
                column: "TipoTiendaId",
                principalTable: "TipoTiendas",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Tiendas_Usuarios_UsuarioId",
                table: "Tiendas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transacciones_Usuarios_UsuarioId",
                table: "Transacciones",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tiendas_TipoTiendas_TipoTiendaId",
                table: "Tiendas");

            migrationBuilder.DropForeignKey(
                name: "FK_Tiendas_Usuarios_UsuarioId",
                table: "Tiendas");

            migrationBuilder.DropForeignKey(
                name: "FK_Transacciones_Usuarios_UsuarioId",
                table: "Transacciones");

            migrationBuilder.DropTable(
                name: "TipoTiendas");

            migrationBuilder.DropIndex(
                name: "IX_Tiendas_TipoTiendaId",
                table: "Tiendas");

            migrationBuilder.DropColumn(
                name: "ModoTema",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "UltimaSesion",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Apto",
                table: "Tiendas");

            migrationBuilder.DropColumn(
                name: "Bloque",
                table: "Tiendas");

            migrationBuilder.DropColumn(
                name: "DocumentoIdentidad",
                table: "Tiendas");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Tiendas");

            migrationBuilder.DropColumn(
                name: "HoraApertura",
                table: "Tiendas");

            migrationBuilder.DropColumn(
                name: "HoraCierre",
                table: "Tiendas");

            migrationBuilder.DropColumn(
                name: "Latitud",
                table: "Tiendas");

            migrationBuilder.DropColumn(
                name: "Longitud",
                table: "Tiendas");

            migrationBuilder.DropColumn(
                name: "MinutoApertura",
                table: "Tiendas");

            migrationBuilder.DropColumn(
                name: "MinutoCierre",
                table: "Tiendas");

            migrationBuilder.DropColumn(
                name: "Referencia",
                table: "Tiendas");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Tiendas");

            migrationBuilder.DropColumn(
                name: "TipoTiendaId",
                table: "Tiendas");

            migrationBuilder.DropColumn(
                name: "Torre",
                table: "Tiendas");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Pedidos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_UsuarioId",
                table: "Pedidos",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Usuarios_UsuarioId",
                table: "Pedidos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tiendas_Usuarios_UsuarioId",
                table: "Tiendas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transacciones_Usuarios_UsuarioId",
                table: "Transacciones",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
