using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourEgypt.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewsTours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AverageRating",
                table: "Tours",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "ReviewsCount",
                table: "Tours",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "ReviewsCount",
                table: "Tours");
        }
    }
}
