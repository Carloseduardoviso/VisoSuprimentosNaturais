using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisoERP.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class ClienteEndereco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cep",
                table: "Clientes",
                type: "nvarchar(9)",
                maxLength: 9,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Endereco",
                table: "Clientes",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Numero",
                table: "Clientes",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cep",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Endereco",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Numero",
                table: "Clientes");
        }
    }
}
