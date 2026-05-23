using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockControl.Admin.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRoleAndPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "users",
                type: "int",
                nullable: false,
                defaultValue: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "users");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "users");
        }
    }
}
