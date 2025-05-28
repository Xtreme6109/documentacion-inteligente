using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentacionInteligente.BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class NombreDeLaMigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DOCUMENTOS_CATEGORIAS_CATEGORIASID",
                table: "DOCUMENTOS");

            migrationBuilder.RenameColumn(
                name: "CATEGORIASID",
                table: "DOCUMENTOS",
                newName: "VERSIONID");

            migrationBuilder.RenameIndex(
                name: "IX_DOCUMENTOS_CATEGORIASID",
                table: "DOCUMENTOS",
                newName: "IX_DOCUMENTOS_VERSIONID");

            migrationBuilder.AddColumn<int>(
                name: "CATEGORIAID",
                table: "DOCUMENTOS",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DOCUMENTOS_CATEGORIAID",
                table: "DOCUMENTOS",
                column: "CATEGORIAID");

            migrationBuilder.AddForeignKey(
                name: "FK_DOCUMENTOS_CATEGORIAS_CATEGORIAID",
                table: "DOCUMENTOS",
                column: "CATEGORIAID",
                principalTable: "CATEGORIAS",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_DOCUMENTOS_VERSIONES_VERSIONID",
                table: "DOCUMENTOS",
                column: "VERSIONID",
                principalTable: "VERSIONES",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DOCUMENTOS_CATEGORIAS_CATEGORIAID",
                table: "DOCUMENTOS");

            migrationBuilder.DropForeignKey(
                name: "FK_DOCUMENTOS_VERSIONES_VERSIONID",
                table: "DOCUMENTOS");

            migrationBuilder.DropIndex(
                name: "IX_DOCUMENTOS_CATEGORIAID",
                table: "DOCUMENTOS");

            migrationBuilder.DropColumn(
                name: "CATEGORIAID",
                table: "DOCUMENTOS");

            migrationBuilder.RenameColumn(
                name: "VERSIONID",
                table: "DOCUMENTOS",
                newName: "CATEGORIASID");

            migrationBuilder.RenameIndex(
                name: "IX_DOCUMENTOS_VERSIONID",
                table: "DOCUMENTOS",
                newName: "IX_DOCUMENTOS_CATEGORIASID");

            migrationBuilder.AddForeignKey(
                name: "FK_DOCUMENTOS_CATEGORIAS_CATEGORIASID",
                table: "DOCUMENTOS",
                column: "CATEGORIASID",
                principalTable: "CATEGORIAS",
                principalColumn: "ID");
        }
    }
}
