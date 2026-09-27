using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisoERP.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class FinanceiroEInvestimentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContasPagar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntradaEstoqueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FornecedorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ValorPago = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Vencimento = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContasPagar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContasPagar_EntradasEstoque_EntradaEstoqueId",
                        column: x => x.EntradaEstoqueId,
                        principalTable: "EntradasEstoque",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContasPagar_Fornecedores_FornecedorId",
                        column: x => x.FornecedorId,
                        principalTable: "Fornecedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Despesas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DataCompetencia = table.Column<DateOnly>(type: "date", nullable: false),
                    PagaEm = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Despesas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Investimentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Data = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Investimentos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PagamentosContasPagar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContaPagarId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Data = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagamentosContasPagar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagamentosContasPagar_ContasPagar_ContaPagarId",
                        column: x => x.ContaPagarId,
                        principalTable: "ContasPagar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContasPagar_EntradaEstoqueId",
                table: "ContasPagar",
                column: "EntradaEstoqueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContasPagar_FornecedorId",
                table: "ContasPagar",
                column: "FornecedorId");

            migrationBuilder.CreateIndex(
                name: "IX_ContasPagar_Vencimento",
                table: "ContasPagar",
                column: "Vencimento");

            migrationBuilder.CreateIndex(
                name: "IX_Despesas_DataCompetencia",
                table: "Despesas",
                column: "DataCompetencia");

            migrationBuilder.CreateIndex(
                name: "IX_Despesas_PagaEm",
                table: "Despesas",
                column: "PagaEm");

            migrationBuilder.CreateIndex(
                name: "IX_Investimentos_Data",
                table: "Investimentos",
                column: "Data");

            migrationBuilder.CreateIndex(
                name: "IX_PagamentosContasPagar_ContaPagarId",
                table: "PagamentosContasPagar",
                column: "ContaPagarId");

            migrationBuilder.CreateIndex(
                name: "IX_PagamentosContasPagar_Data",
                table: "PagamentosContasPagar",
                column: "Data");

            migrationBuilder.Sql("""
                INSERT INTO [ContasPagar] ([Id], [EntradaEstoqueId], [FornecedorId],
                    [Descricao], [Valor], [ValorPago], [Vencimento])
                SELECT NEWID(), e.[Id], p.[FornecedorId], N'Compra de suprimentos',
                    CAST(ROUND(SUM(i.[Quantidade] * i.[CustoUnitario]), 2) AS decimal(18,2)),
                    0, CAST(e.[CriadaEm] AS date)
                FROM [EntradasEstoque] e
                JOIN [ItensEntradasEstoque] i ON i.[EntradaEstoqueId] = e.[Id]
                LEFT JOIN [PedidosFornecedores] p ON p.[Id] = e.[PedidoFornecedorId]
                GROUP BY e.[Id], e.[CriadaEm], p.[FornecedorId]
                HAVING ROUND(SUM(i.[Quantidade] * i.[CustoUnitario]), 2) > 0;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Despesas");

            migrationBuilder.DropTable(
                name: "Investimentos");

            migrationBuilder.DropTable(
                name: "PagamentosContasPagar");

            migrationBuilder.DropTable(
                name: "ContasPagar");
        }
    }
}
