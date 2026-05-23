using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockControl.Admin.Migrations
{
    /// <summary>
    /// Seeds Password + Role for users Id 1, 2, 3. Plaintext passwords documented in readme/LOGIN-TEST-USERS.md.
    /// </summary>
    public partial class SeedUserPasswords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DECLARE @Pwd nvarchar(500) = N'AQAAAAIAAYagAAAAEP+4wjFBuFomgYSUKY/Z+Yae9VchhiHV/EBKaWVVnQ/cg3RyOLIXWoiwrGZJ+VlweA==';

                UPDATE users SET Password = @Pwd WHERE Id IN (1, 2, 3);

                UPDATE users SET Role = 1 WHERE Id = 1;
                UPDATE users SET Role = 2 WHERE Id IN (2, 3);
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE users SET Password = NULL WHERE Id IN (1, 2, 3);
                """);
        }
    }
}
