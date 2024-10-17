using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Repositorio.Migrations
{
    /// <inheritdoc />
    public partial class AgregarFiltros : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Filtro",
                columns: table => new
                {
                    IdFiltro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Filtro", x => x.IdFiltro);
                });

            migrationBuilder.CreateTable(
                name: "CategoriaFiltro",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdFiltro = table.Column<int>(type: "int", nullable: false),
                    IdCategoria = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaFiltro", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoriaFiltro_Categoria_IdCategoria",
                        column: x => x.IdCategoria,
                        principalTable: "Categoria",
                        principalColumn: "IdCategoria",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoriaFiltro_Filtro_IdFiltro",
                        column: x => x.IdFiltro,
                        principalTable: "Filtro",
                        principalColumn: "IdFiltro",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoriaFiltro_IdCategoria",
                table: "CategoriaFiltro",
                column: "IdCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriaFiltro_IdFiltro",
                table: "CategoriaFiltro",
                column: "IdFiltro");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoriaFiltro");

            migrationBuilder.DropTable(
                name: "Filtro");
        }
    }
}
