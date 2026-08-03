using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourEgypt.Data.Migrations
{
    /// <inheritdoc />
    public partial class RestrictCategoryDeleteOnPlace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Places_Categories_CategoryId",
                table: "Places");

            migrationBuilder.AddForeignKey(
                name: "FK_Places_Categories_CategoryId",
                table: "Places",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Places_Categories_CategoryId",
                table: "Places");

            migrationBuilder.AddForeignKey(
                name: "FK_Places_Categories_CategoryId",
                table: "Places",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
