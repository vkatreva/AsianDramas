using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AsianDramas.Migrations
{
    /// <inheritdoc />
    public partial class AddIdToDramaActor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "DramaActors",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "DramaActors");
        }
    }
}
