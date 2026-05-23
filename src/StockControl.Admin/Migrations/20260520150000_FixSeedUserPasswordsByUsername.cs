using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockControl.Admin.Migrations
{
    /// <summary>
    /// Seeds Password + Role by Username (admin, pda). Plaintext: Pda2!Stock — see readme/LOGIN-TEST-USERS.md.
    /// </summary>
    public partial class FixSeedUserPasswordsByUsername : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE users SET Password = N'AQAAAAIAAYagAAAAENM7cNEPU0XZAiMR0XcY705PEzelzlRIcw+bQ8MnAAqhLuMXpmDCmIEmDpz/Vm507g==', Role = 1
                WHERE Username = N'admin';

                UPDATE users SET Password = N'AQAAAAIAAYagAAAAEMU13AnOAmBi32tMf9ZNQB5/x6NUm9sqUnyFSqpCghpxemFcQHV/KhNAaeN8IKqLvA==', Role = 2
                WHERE Username = N'pda';
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE users SET Password = NULL WHERE Username IN (N'admin', N'pda');
                """);
        }
    }
}
