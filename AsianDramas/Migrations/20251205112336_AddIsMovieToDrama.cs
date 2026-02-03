using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AsianDramas.Migrations
{
    /// <inheritdoc />
    public partial class AddIsMovieToDrama : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMovie",
                table: "Dramas",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMovie",
                table: "Dramas");
        }
    }
}
