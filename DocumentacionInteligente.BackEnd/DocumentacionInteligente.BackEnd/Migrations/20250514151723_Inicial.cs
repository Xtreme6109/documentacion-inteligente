using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentacionInteligente.BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CATEGORIAS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NOMBRE = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DESCRIPCION = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CREATE_DATE = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CATEGORIAS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "IA_PROCESAMIENTOS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DOCUMENTO_ID = table.Column<int>(type: "int", nullable: false),
                    TIPO = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RESULTADO = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FECHA = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IA_PROCESAMIENTOS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "LOGS_ACCESO",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    USUARIO_ID = table.Column<int>(type: "int", nullable: false),
                    ACCION = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DOCUMENTO_ID = table.Column<int>(type: "int", nullable: true),
                    FECHA = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOGS_ACCESO", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PALABRAS_CLAVE",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DOCUMENTO_ID = table.Column<int>(type: "int", nullable: false),
                    PALABRA = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PALABRAS_CLAVE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "USUARIOS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NOMBRE = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CORREO = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PASSWORD_HASH = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ROL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CREATE_DATE = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIOS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "VERSIONES",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DOCUMENTO_ID = table.Column<int>(type: "int", nullable: false),
                    NUMERO_VERSION = table.Column<int>(type: "int", nullable: false),
                    RUTA_ARCHIVO = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FECHA_CREACION = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NOTAS = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VERSIONES", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DOCUMENTOS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TITULO = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RUTA_ARCHIVO = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DESCRIPCION = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    USUARIO_ID = table.Column<int>(type: "int", nullable: false),
                    CATEGORIA_ID = table.Column<int>(type: "int", nullable: true),
                    CREATE_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CREADO_IA = table.Column<bool>(type: "bit", nullable: true),
                    ESTADO = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VERSION_ACTUAL = table.Column<int>(type: "int", nullable: true),
                    CATEGORIASID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOCUMENTOS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DOCUMENTOS_CATEGORIAS_CATEGORIASID",
                        column: x => x.CATEGORIASID,
                        principalTable: "CATEGORIAS",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DOCUMENTOS_CATEGORIASID",
                table: "DOCUMENTOS",
                column: "CATEGORIASID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DOCUMENTOS");

            migrationBuilder.DropTable(
                name: "IA_PROCESAMIENTOS");

            migrationBuilder.DropTable(
                name: "LOGS_ACCESO");

            migrationBuilder.DropTable(
                name: "PALABRAS_CLAVE");

            migrationBuilder.DropTable(
                name: "USUARIOS");

            migrationBuilder.DropTable(
                name: "VERSIONES");

            migrationBuilder.DropTable(
                name: "CATEGORIAS");
        }
    }
}
