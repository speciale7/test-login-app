using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoginApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCountingTimeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "E001",
                table: "MonetaryFund");

            migrationBuilder.DropColumn(
                name: "E002",
                table: "MonetaryFund");

            migrationBuilder.DropColumn(
                name: "E005",
                table: "MonetaryFund");

            migrationBuilder.DropColumn(
                name: "E010",
                table: "MonetaryFund");

            migrationBuilder.DropColumn(
                name: "E020",
                table: "MonetaryFund");

            migrationBuilder.DropColumn(
                name: "E050",
                table: "MonetaryFund");

            migrationBuilder.DropColumn(
                name: "E100",
                table: "MonetaryFund");

            migrationBuilder.DropColumn(
                name: "E1000",
                table: "MonetaryFund");

            migrationBuilder.DropColumn(
                name: "E200",
                table: "MonetaryFund");

            migrationBuilder.DropColumn(
                name: "E2000",
                table: "MonetaryFund");

            migrationBuilder.DropColumn(
                name: "E500",
                table: "MonetaryFund");

            migrationBuilder.DropColumn(
                name: "E5000",
                table: "MonetaryFund");

            migrationBuilder.DropColumn(
                name: "E001",
                table: "ExpenseFund");

            migrationBuilder.DropColumn(
                name: "E002",
                table: "ExpenseFund");

            migrationBuilder.DropColumn(
                name: "E005",
                table: "ExpenseFund");

            migrationBuilder.DropColumn(
                name: "E010",
                table: "ExpenseFund");

            migrationBuilder.DropColumn(
                name: "E020",
                table: "ExpenseFund");

            migrationBuilder.DropColumn(
                name: "E050",
                table: "ExpenseFund");

            migrationBuilder.DropColumn(
                name: "E1",
                table: "ExpenseFund");

            migrationBuilder.DropColumn(
                name: "E10",
                table: "ExpenseFund");

            migrationBuilder.DropColumn(
                name: "E100",
                table: "ExpenseFund");

            migrationBuilder.DropColumn(
                name: "E100c",
                table: "ExpenseFund");

            migrationBuilder.DropColumn(
                name: "E2",
                table: "ExpenseFund");

            migrationBuilder.DropColumn(
                name: "E20",
                table: "ExpenseFund");

            migrationBuilder.DropColumn(
                name: "E200",
                table: "ExpenseFund");

            migrationBuilder.DropColumn(
                name: "E200c",
                table: "ExpenseFund");

            migrationBuilder.DropColumn(
                name: "E5",
                table: "ExpenseFund");

            migrationBuilder.DropColumn(
                name: "E50",
                table: "ExpenseFund");

            migrationBuilder.DropColumn(
                name: "E001",
                table: "CashFund");

            migrationBuilder.DropColumn(
                name: "E002",
                table: "CashFund");

            migrationBuilder.DropColumn(
                name: "E005",
                table: "CashFund");

            migrationBuilder.DropColumn(
                name: "E010",
                table: "CashFund");

            migrationBuilder.DropColumn(
                name: "E020",
                table: "CashFund");

            migrationBuilder.DropColumn(
                name: "E050",
                table: "CashFund");

            migrationBuilder.RenameColumn(
                name: "E500",
                table: "ExpenseFund",
                newName: "fornitore");

            migrationBuilder.AlterColumn<decimal>(
                name: "counting_coins",
                table: "MonetaryFund",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "counting_at",
                table: "MonetaryFund",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "counting_time",
                table: "MonetaryFund",
                type: "TEXT",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "note",
                table: "MonetaryFund",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "totale",
                table: "MonetaryFund",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "totale_banconote",
                table: "MonetaryFund",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "totale_monete",
                table: "MonetaryFund",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "user",
                table: "MonetaryFund",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "counting_coins",
                table: "ExpenseFund",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "counting_at",
                table: "ExpenseFund",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "counting_time",
                table: "ExpenseFund",
                type: "TEXT",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "fattura",
                table: "ExpenseFund",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "note",
                table: "ExpenseFund",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tipo",
                table: "ExpenseFund",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "totale",
                table: "ExpenseFund",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "user",
                table: "ExpenseFund",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "counting_coins",
                table: "CashFund",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "counting_at",
                table: "CashFund",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cassa",
                table: "CashFund",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "counting_time",
                table: "CashFund",
                type: "TEXT",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "notes",
                table: "CashFund",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "totale",
                table: "CashFund",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "user",
                table: "CashFund",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "counting_coins",
                table: "BanconoteWithdrawalAutomatic",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "counting_at",
                table: "BanconoteWithdrawalAutomatic",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "counting_time",
                table: "BanconoteWithdrawalAutomatic",
                type: "TEXT",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "note",
                table: "BanconoteWithdrawalAutomatic",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "totale",
                table: "BanconoteWithdrawalAutomatic",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "user",
                table: "BanconoteWithdrawalAutomatic",
                type: "TEXT",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "counting_time",
                table: "MonetaryFund");

            migrationBuilder.DropColumn(
                name: "note",
                table: "MonetaryFund");

            migrationBuilder.DropColumn(
                name: "totale",
                table: "MonetaryFund");

            migrationBuilder.DropColumn(
                name: "totale_banconote",
                table: "MonetaryFund");

            migrationBuilder.DropColumn(
                name: "totale_monete",
                table: "MonetaryFund");

            migrationBuilder.DropColumn(
                name: "user",
                table: "MonetaryFund");

            migrationBuilder.DropColumn(
                name: "counting_time",
                table: "ExpenseFund");

            migrationBuilder.DropColumn(
                name: "fattura",
                table: "ExpenseFund");

            migrationBuilder.DropColumn(
                name: "note",
                table: "ExpenseFund");

            migrationBuilder.DropColumn(
                name: "tipo",
                table: "ExpenseFund");

            migrationBuilder.DropColumn(
                name: "totale",
                table: "ExpenseFund");

            migrationBuilder.DropColumn(
                name: "user",
                table: "ExpenseFund");

            migrationBuilder.DropColumn(
                name: "cassa",
                table: "CashFund");

            migrationBuilder.DropColumn(
                name: "counting_time",
                table: "CashFund");

            migrationBuilder.DropColumn(
                name: "notes",
                table: "CashFund");

            migrationBuilder.DropColumn(
                name: "totale",
                table: "CashFund");

            migrationBuilder.DropColumn(
                name: "user",
                table: "CashFund");

            migrationBuilder.DropColumn(
                name: "counting_time",
                table: "BanconoteWithdrawalAutomatic");

            migrationBuilder.DropColumn(
                name: "note",
                table: "BanconoteWithdrawalAutomatic");

            migrationBuilder.DropColumn(
                name: "totale",
                table: "BanconoteWithdrawalAutomatic");

            migrationBuilder.DropColumn(
                name: "user",
                table: "BanconoteWithdrawalAutomatic");

            migrationBuilder.RenameColumn(
                name: "fornitore",
                table: "ExpenseFund",
                newName: "E500");

            migrationBuilder.AlterColumn<string>(
                name: "counting_coins",
                table: "MonetaryFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "counting_at",
                table: "MonetaryFund",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E001",
                table: "MonetaryFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E002",
                table: "MonetaryFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E005",
                table: "MonetaryFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E010",
                table: "MonetaryFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E020",
                table: "MonetaryFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E050",
                table: "MonetaryFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E100",
                table: "MonetaryFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E1000",
                table: "MonetaryFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E200",
                table: "MonetaryFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E2000",
                table: "MonetaryFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E500",
                table: "MonetaryFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E5000",
                table: "MonetaryFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "counting_coins",
                table: "ExpenseFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "counting_at",
                table: "ExpenseFund",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E001",
                table: "ExpenseFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E002",
                table: "ExpenseFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E005",
                table: "ExpenseFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E010",
                table: "ExpenseFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E020",
                table: "ExpenseFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E050",
                table: "ExpenseFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E1",
                table: "ExpenseFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E10",
                table: "ExpenseFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E100",
                table: "ExpenseFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E100c",
                table: "ExpenseFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E2",
                table: "ExpenseFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E20",
                table: "ExpenseFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E200",
                table: "ExpenseFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E200c",
                table: "ExpenseFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E5",
                table: "ExpenseFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E50",
                table: "ExpenseFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "counting_coins",
                table: "CashFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "counting_at",
                table: "CashFund",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E001",
                table: "CashFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E002",
                table: "CashFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E005",
                table: "CashFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E010",
                table: "CashFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E020",
                table: "CashFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E050",
                table: "CashFund",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "counting_coins",
                table: "BanconoteWithdrawalAutomatic",
                type: "TEXT",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "counting_at",
                table: "BanconoteWithdrawalAutomatic",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
