    using Giovany.Data;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Migrations;

    #nullable disable
    namespace Giovany.Migrations;

    [DbContext(typeof(FolhaDbContext))]
    [Migration("20261019000000_CriarFolhas")]
    public partial class CriarFolhas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Folhas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Cpf = table.Column<string>(type: "TEXT", nullable: false),
                    Mes = table.Column<int>(type: "INTEGER", nullable: false),
                    Ano = table.Column<int>(type: "INTEGER", nullable: false),
                    HorasTrabalhadas = table.Column<decimal>(type: "TEXT", nullable: false),
                    ValorHora = table.Column<decimal>(type: "TEXT", nullable: false),
                    SalarioBruto = table.Column<decimal>(type: "TEXT", nullable: false),
                    ImpostoRenda = table.Column<decimal>(type: "TEXT", nullable: false),
                    Inss = table.Column<decimal>(type: "TEXT", nullable: false),
                    Fgts = table.Column<decimal>(type: "TEXT", nullable: false),
                    SalarioLiquido = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_Folhas", x => x.Id));
            migrationBuilder.CreateIndex(name: "IX_Folhas_Cpf_Mes_Ano", table: "Folhas", columns: new[] { "Cpf", "Mes", "Ano" }, unique: true);
        }
        protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "Folhas");
    }
