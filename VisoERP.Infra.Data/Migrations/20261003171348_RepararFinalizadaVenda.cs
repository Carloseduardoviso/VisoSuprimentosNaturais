using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisoERP.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class RepararFinalizadaVenda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF COL_LENGTH(N'dbo.Vendas', N'Finalizada') IS NULL
                BEGIN
                    ALTER TABLE [dbo].[Vendas]
                        ADD [Finalizada] bit NOT NULL
                            CONSTRAINT [DF_Vendas_Finalizada_Reparo] DEFAULT (0);
                END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF COL_LENGTH(N'dbo.Vendas', N'Finalizada') IS NOT NULL
                BEGIN
                    IF OBJECT_ID(N'dbo.DF_Vendas_Finalizada_Reparo', N'D') IS NOT NULL
                        ALTER TABLE [dbo].[Vendas] DROP CONSTRAINT [DF_Vendas_Finalizada_Reparo];
                    ALTER TABLE [dbo].[Vendas] DROP COLUMN [Finalizada];
                END");
        }
    }
}
