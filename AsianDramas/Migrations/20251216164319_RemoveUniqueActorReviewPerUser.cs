using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AsianDramas.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqueActorReviewPerUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ActorReviews_ActorId_UserId",
                table: "ActorReviews");

            migrationBuilder.CreateIndex(
                name: "IX_ActorReviews_ActorId",
                table: "ActorReviews",
                column: "ActorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ActorReviews_ActorId",
                table: "ActorReviews");

            migrationBuilder.CreateIndex(
                name: "IX_ActorReviews_ActorId_UserId",
                table: "ActorReviews",
                columns: new[] { "ActorId", "UserId" },
                unique: true);
        }
    }
}
