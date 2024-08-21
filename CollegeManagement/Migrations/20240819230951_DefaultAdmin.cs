using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegeManagement.Migrations
{
    /// <inheritdoc />
    public partial class DefaultAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Admins",
                keyColumn: "Email",
                keyValue: "admin@default.com");

            migrationBuilder.InsertData(
                table: "Admins",
                columns: new[] { "Email", "Password" },
                values: new object[] { "Admin@default.com", "$2a$11$V/J2zx4LOS74DUr8FyfN4.tjoIJefACUcMobRaSkiQC6aC01Tc/be" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Admins",
                keyColumn: "Email",
                keyValue: "Admin@default.com");

            migrationBuilder.InsertData(
                table: "Admins",
                columns: new[] { "Email", "Password" },
                values: new object[] { "admin@default.com", "$2a$11$iXKoSEs61CnkA4pQ9.i41.ZkJ1S0B3AzoiBGJ37uSxnsz5Ef044Iy" });
        }
    }
}
