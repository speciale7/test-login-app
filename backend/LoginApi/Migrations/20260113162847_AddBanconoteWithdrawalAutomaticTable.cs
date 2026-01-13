using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoginApi.Migrations
{
    /// <inheritdoc />
    public partial class AddBanconoteWithdrawalAutomaticTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BanconoteWithdrawalAutomatic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    id_store = table.Column<int>(type: "INTEGER", nullable: false),
                    store_alias = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    security_envelope_code = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    counting_amount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    counting_difference = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    counting_coins = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    withdrawal_date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    counting_courier_date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    import_date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    counting_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    counting_by = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    counting_date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    counting_time = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    user = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    totale = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    note = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BanconoteWithdrawalAutomatic", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BanconoteWithdrawalAutomatic");
        }
    }
}
