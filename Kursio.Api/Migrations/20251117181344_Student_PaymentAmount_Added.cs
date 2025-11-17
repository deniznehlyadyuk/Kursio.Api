using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kursio.Api.Migrations
{
    /// <inheritdoc />
    public partial class Student_PaymentAmount_Added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentAmount",
                table: "Students",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentAmount",
                table: "Students");
        }
    }
}
