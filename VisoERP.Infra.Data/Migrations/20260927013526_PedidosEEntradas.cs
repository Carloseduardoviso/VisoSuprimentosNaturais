using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisoERP.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class PedidosEEntradas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EstoquesProdutos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SuprimentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantidade = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    CustoMedio = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstoquesProdutos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstoquesProdutos_Suprimentos_SuprimentoId",
                        column: x => x.SuprimentoId,
                        principalTable: "Suprimentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LotesEstoque",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SuprimentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Validade = table.Column<DateOnly>(type: "date", nullable: true),
                    Quantidade = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LotesEstoque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LotesEstoque_Suprimentos_SuprimentoId",
                        column: x => x.SuprimentoId,
                        principalTable: "Suprimentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PedidosFornecedores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FornecedorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataCriacao = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Situacao = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidosFornecedores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PedidosFornecedores_Fornecedores_FornecedorId",
                        column: x => x.FornecedorId,
                        principalTable: "Fornecedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovimentacoesEstoque",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SuprimentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoteEstoqueId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReferenciaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    CustoUnitario = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Data = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimentacoesEstoque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimentacoesEstoque_LotesEstoque_LoteEstoqueId",
                        column: x => x.LoteEstoqueId,
                        principalTable: "LotesEstoque",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimentacoesEstoque_Suprimentos_SuprimentoId",
                        column: x => x.SuprimentoId,
                        principalTable: "Suprimentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EntradasEstoque",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PedidoFornecedorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CriadaEm = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntradasEstoque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntradasEstoque_PedidosFornecedores_PedidoFornecedorId",
                        column: x => x.PedidoFornecedorId,
                        principalTable: "PedidosFornecedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItensPedidosFornecedores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PedidoFornecedorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SuprimentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantidade = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    QuantidadeRecebida = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    PrecoCatalogo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PrecoComDesconto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensPedidosFornecedores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensPedidosFornecedores_PedidosFornecedores_PedidoFornecedorId",
                        column: x => x.PedidoFornecedorId,
                        principalTable: "PedidosFornecedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItensPedidosFornecedores_Suprimentos_SuprimentoId",
                        column: x => x.SuprimentoId,
                        principalTable: "Suprimentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItensEntradasEstoque",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntradaEstoqueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SuprimentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantidade = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    CustoUnitario = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Data = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Promocional = table.Column<bool>(type: "bit", nullable: false),
                    CodigoLote = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Validade = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensEntradasEstoque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensEntradasEstoque_EntradasEstoque_EntradaEstoqueId",
                        column: x => x.EntradaEstoqueId,
                        principalTable: "EntradasEstoque",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItensEntradasEstoque_Suprimentos_SuprimentoId",
                        column: x => x.SuprimentoId,
                        principalTable: "Suprimentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntradasEstoque_CriadaEm",
                table: "EntradasEstoque",
                column: "CriadaEm");

            migrationBuilder.CreateIndex(
                name: "IX_EntradasEstoque_PedidoFornecedorId",
                table: "EntradasEstoque",
                column: "PedidoFornecedorId");

            migrationBuilder.CreateIndex(
                name: "IX_EstoquesProdutos_SuprimentoId",
                table: "EstoquesProdutos",
                column: "SuprimentoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItensEntradasEstoque_EntradaEstoqueId_SuprimentoId",
                table: "ItensEntradasEstoque",
                columns: new[] { "EntradaEstoqueId", "SuprimentoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItensEntradasEstoque_SuprimentoId",
                table: "ItensEntradasEstoque",
                column: "SuprimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensPedidosFornecedores_PedidoFornecedorId_SuprimentoId",
                table: "ItensPedidosFornecedores",
                columns: new[] { "PedidoFornecedorId", "SuprimentoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItensPedidosFornecedores_SuprimentoId",
                table: "ItensPedidosFornecedores",
                column: "SuprimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_LotesEstoque_SuprimentoId_Codigo",
                table: "LotesEstoque",
                columns: new[] { "SuprimentoId", "Codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LotesEstoque_Validade",
                table: "LotesEstoque",
                column: "Validade");

            migrationBuilder.CreateIndex(
                name: "IX_MovimentacoesEstoque_LoteEstoqueId",
                table: "MovimentacoesEstoque",
                column: "LoteEstoqueId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimentacoesEstoque_ReferenciaId",
                table: "MovimentacoesEstoque",
                column: "ReferenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimentacoesEstoque_SuprimentoId_Data",
                table: "MovimentacoesEstoque",
                columns: new[] { "SuprimentoId", "Data" });

            migrationBuilder.CreateIndex(
                name: "IX_PedidosFornecedores_FornecedorId_DataCriacao",
                table: "PedidosFornecedores",
                columns: new[] { "FornecedorId", "DataCriacao" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EstoquesProdutos");

            migrationBuilder.DropTable(
                name: "ItensEntradasEstoque");

            migrationBuilder.DropTable(
                name: "ItensPedidosFornecedores");

            migrationBuilder.DropTable(
                name: "MovimentacoesEstoque");

            migrationBuilder.DropTable(
                name: "EntradasEstoque");

            migrationBuilder.DropTable(
                name: "LotesEstoque");

            migrationBuilder.DropTable(
                name: "PedidosFornecedores");
        }
    }
}
