using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AsianDramas.Migrations
{
    /// <inheritdoc />
    public partial class AddActorReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PopularityScore",
                table: "Actors",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ActorReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActorId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActorReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActorReviews_Actors_ActorId",
                        column: x => x.ActorId,
                        principalTable: "Actors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActorReviews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActorReviews_ActorId_UserId",
                table: "ActorReviews",
                columns: new[] { "ActorId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActorReviews_UserId",
                table: "ActorReviews",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActorReviews");

            migrationBuilder.AlterColumn<long>(
                name: "PopularityScore",
                table: "Actors",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
