using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialDataService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAggTradeColumnsToFinancialInstruments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "LastTradePrice",
                table: "FinancialInstruments",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LastTradeQuantity",
                table: "FinancialInstruments",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastTradeTime",
                table: "FinancialInstruments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastTradePrice",
                table: "FinancialInstruments");

            migrationBuilder.DropColumn(
                name: "LastTradeQuantity",
                table: "FinancialInstruments");

            migrationBuilder.DropColumn(
                name: "LastTradeTime",
                table: "FinancialInstruments");
        }
    }
}
