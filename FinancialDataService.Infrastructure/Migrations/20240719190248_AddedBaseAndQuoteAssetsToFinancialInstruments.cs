using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialDataService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedBaseAndQuoteAssetsToFinancialInstruments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Symbol",
                table: "FinancialInstruments",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AddColumn<string>(
                name: "BaseAsset",
                table: "FinancialInstruments",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "QuoteAsset",
                table: "FinancialInstruments",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseAsset",
                table: "FinancialInstruments");

            migrationBuilder.DropColumn(
                name: "QuoteAsset",
                table: "FinancialInstruments");

            migrationBuilder.AlterColumn<string>(
                name: "Symbol",
                table: "FinancialInstruments",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);
        }
    }
}
