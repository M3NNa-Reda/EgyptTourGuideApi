using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourEgypt.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFavouritesCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "favoriteCount",
                table: "Places",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "favoriteCount",
                table: "Places");
        }
    }
}
