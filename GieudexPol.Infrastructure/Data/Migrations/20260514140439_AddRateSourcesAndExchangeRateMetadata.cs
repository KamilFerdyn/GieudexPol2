using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GieudexPol.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRateSourcesAndExchangeRateMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Currencies_CurrencyId1",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_Currencies_CurrencyId1",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_Wallets_CurrencyId1",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_CurrencyId1",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_ExchangeRates_CurrencyId",
                table: "ExchangeRates");

            migrationBuilder.DropColumn(
                name: "CurrencyId1",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "CurrencyId1",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "ExchangeRates",
                newName: "FetchedAt");

            migrationBuilder.AlterColumn<decimal>(
                name: "SellPrice",
                table: "ExchangeRates",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BuyPrice",
                table: "ExchangeRates",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveDate",
                table: "ExchangeRates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "RateSourceId",
                table: "ExchangeRates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RateSources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RateSources", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_CurrencyId_RateSourceId_EffectiveDate",
                table: "ExchangeRates",
                columns: new[] { "CurrencyId", "RateSourceId", "EffectiveDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_RateSourceId",
                table: "ExchangeRates",
                column: "RateSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_RateSources_Code",
                table: "RateSources",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ExchangeRates_RateSources_RateSourceId",
                table: "ExchangeRates",
                column: "RateSourceId",
                principalTable: "RateSources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExchangeRates_RateSources_RateSourceId",
                table: "ExchangeRates");

            migrationBuilder.DropTable(
                name: "RateSources");

            migrationBuilder.DropIndex(
                name: "IX_ExchangeRates_CurrencyId_RateSourceId_EffectiveDate",
                table: "ExchangeRates");

            migrationBuilder.DropIndex(
                name: "IX_ExchangeRates_RateSourceId",
                table: "ExchangeRates");

            migrationBuilder.DropColumn(
                name: "EffectiveDate",
                table: "ExchangeRates");

            migrationBuilder.DropColumn(
                name: "RateSourceId",
                table: "ExchangeRates");

            migrationBuilder.RenameColumn(
                name: "FetchedAt",
                table: "ExchangeRates",
                newName: "Timestamp");

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId1",
                table: "Wallets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId1",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "SellPrice",
                table: "ExchangeRates",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "BuyPrice",
                table: "ExchangeRates",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_CurrencyId1",
                table: "Wallets",
                column: "CurrencyId1");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CurrencyId1",
                table: "Transactions",
                column: "CurrencyId1");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_CurrencyId",
                table: "ExchangeRates",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Currencies_CurrencyId1",
                table: "Transactions",
                column: "CurrencyId1",
                principalTable: "Currencies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_Currencies_CurrencyId1",
                table: "Wallets",
                column: "CurrencyId1",
                principalTable: "Currencies",
                principalColumn: "Id");
        }
    }
}
