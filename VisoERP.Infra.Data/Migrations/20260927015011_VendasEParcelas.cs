using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisoERP.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class VendasEParcelas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vendas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Data = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Desconto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ValorEntrada = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NumeroParcelas = table.Column<int>(type: "int", nullable: false),
                    PrimeiroVencimento = table.Column<DateOnly>(type: "date", nullable: false),
                    Finalizada = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vendas_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItensVendas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VendaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SuprimentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantidade = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    PrecoCatalogo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Promocional = table.Column<bool>(type: "bit", nullable: false),
                    Data = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CustoUnitarioHistorico = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensVendas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensVendas_Suprimentos_SuprimentoId",
                        column: x => x.SuprimentoId,
                        principalTable: "Suprimentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItensVendas_Vendas_VendaId",
                        column: x => x.VendaId,
                        principalTable: "Vendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParcelasVendas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VendaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Numero = table.Column<int>(type: "int", nullable: false),
                    Vencimento = table.Column<DateOnly>(type: "date", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ValorPago = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PagoEm = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParcelasVendas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParcelasVendas_Vendas_VendaId",
                        column: x => x.VendaId,
                        principalTable: "Vendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecebimentosVendas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VendaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParcelaVendaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Data = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecebimentosVendas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecebimentosVendas_ParcelasVendas_ParcelaVendaId",
                        column: x => x.ParcelaVendaId,
                        principalTable: "ParcelasVendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecebimentosVendas_Vendas_VendaId",
                        column: x => x.VendaId,
                        principalTable: "Vendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItensVendas_SuprimentoId",
                table: "ItensVendas",
                column: "SuprimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensVendas_VendaId_SuprimentoId",
                table: "ItensVendas",
                columns: new[] { "VendaId", "SuprimentoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParcelasVendas_Vencimento",
                table: "ParcelasVendas",
                column: "Vencimento");

            migrationBuilder.CreateIndex(
                name: "IX_ParcelasVendas_VendaId_Numero",
                table: "ParcelasVendas",
                columns: new[] { "VendaId", "Numero" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecebimentosVendas_Data",
                table: "RecebimentosVendas",
                column: "Data");

            migrationBuilder.CreateIndex(
                name: "IX_RecebimentosVendas_ParcelaVendaId",
                table: "RecebimentosVendas",
                column: "ParcelaVendaId");

            migrationBuilder.CreateIndex(
                name: "IX_RecebimentosVendas_VendaId",
                table: "RecebimentosVendas",
                column: "VendaId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_ClienteId_Data",
                table: "Vendas",
                columns: new[] { "ClienteId", "Data" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItensVendas");

            migrationBuilder.DropTable(
                name: "RecebimentosVendas");

            migrationBuilder.DropTable(
                name: "ParcelasVendas");

            migrationBuilder.DropTable(
                name: "Vendas");
        }
    }
}
